using System.Security.Claims;
using Demodha.Data;
using Demodha.Models;
using Demodha.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace Demodha.Controllers;

[Authorize]
[Route("Owner/Applications")]
public class ApplicationsController : Controller
{
    private readonly ApplicationDbContext _db;
    private readonly IWebHostEnvironment _env;

    public ApplicationsController(ApplicationDbContext db, IWebHostEnvironment env)
    {
        _db = db;
        _env = env;
    }

    [HttpGet("Create")]
    public async Task<IActionResult> Create()
    {
        var vm = new NdcApplicationVM
        {
            DealerOptions = await GetDealersFromUsersAsync()
        };
        return View(vm);
    }

    [HttpPost("Create")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(NdcApplicationVM vm, string actionType)
    {
        vm.DealerOptions = await GetDealersFromUsersAsync();

        if (!string.Equals(actionType, "submit", StringComparison.OrdinalIgnoreCase))
        {
            ModelState.AddModelError("", "Invalid action. Please click Save Draft.");
            return View(vm);
        }

        if (!ModelState.IsValid)
            return View(vm);

        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (string.IsNullOrWhiteSpace(userId))
            return Challenge();

        await using var tx = await _db.Database.BeginTransactionAsync();

        try
        {
            var app = new NdcApplication
            {
                PlotOrFileNo = vm.PlotOrFileNo.Trim(),
                Block = vm.Block.Trim(),
                SectorOrPhase = vm.SectorOrPhase.Trim(),
                SocietyOrScheme = vm.SocietyOrScheme.Trim(),
                DealerUserId = vm.DealerUserId,
                Remarks = vm.Remarks?.Trim(),

                CurrentStage = NdcStage.Dealer,
                CurrentStatus = NdcStatus.Submitted,

                CreatedByUserId = userId,
                CreatedOn = DateTime.UtcNow,
                UpdatedOn = DateTime.UtcNow,
                IsActive = true
            };

            _db.NdcApplications.Add(app);
            await _db.SaveChangesAsync(); 

            _db.NdcParties.Add(new NdcParty
            {
                NdcApplicationId = app.Id,
                PartyType = NdcPartyType.Seller,
                FullName = vm.SellerName.Trim(),
                CNIC = vm.SellerCnic.Trim(),
                Phone = vm.SellerPhone.Trim(),
                Email = vm.SellerEmail?.Trim(),
                Address = vm.SellerAddress?.Trim()
            });

            if (!string.IsNullOrWhiteSpace(vm.PurchaserName) || !string.IsNullOrWhiteSpace(vm.PurchaserCnic))
            {
                _db.NdcParties.Add(new NdcParty
                {
                    NdcApplicationId = app.Id,
                    PartyType = NdcPartyType.Purchaser,
                    FullName = (vm.PurchaserName ?? "").Trim(),
                    CNIC = (vm.PurchaserCnic ?? "").Trim()
                });
            }

            var uploadRoot = EnsureUploadFolder(app.Id);
            var docsToInsert = new List<NdcDocument>();

            async Task AddDocIfProvided(IFormFile? file, NdcDocumentType docType)
            {
                if (file == null || file.Length == 0) return;

                ValidateFile(file, docType); 

                var saved = await SaveFileAsync(file, uploadRoot);

                docsToInsert.Add(new NdcDocument
                {
                    NdcApplicationId = app.Id,
                    DocType = docType,
                    FileName = saved.FileName,
                    FilePath = saved.RelativePath,
                    ContentType = file.ContentType,
                    FileSizeBytes = file.Length,
                    UploadedByUserId = userId,
                    UploadedOn = DateTime.UtcNow
                });
            }

            await AddDocIfProvided(vm.OwnerCnicCopy, NdcDocumentType.SellerCnicCopy);
            await AddDocIfProvided(vm.Photo1, NdcDocumentType.Photo1);
            await AddDocIfProvided(vm.Photo2, NdcDocumentType.Photo2);
            await AddDocIfProvided(vm.ApplicationToSecretary, NdcDocumentType.ApplicationToSecretary);

            if (docsToInsert.Count > 0)
                _db.NdcDocuments.AddRange(docsToInsert);

            var ownerTaskDefs = await _db.NdcTaskDefinitions
                .AsNoTracking()
                .Where(x => x.Stage == NdcStage.Owner)
                .OrderBy(x => x.SortOrder)
                .ToListAsync();

            var tasks = ownerTaskDefs.Select(td => new NdcTask
            {
                NdcApplicationId = app.Id,
                TaskDefinitionId = td.Id,
                IsCompleted = false
            }).ToList();

            _db.NdcTasks.AddRange(tasks);

            AutoCompleteOwnerDraftTasks(userId, tasks, docsToInsert);

            _db.NdcStatusHistories.Add(new NdcStatusHistory
            {
                NdcApplicationId = app.Id,
                FromStage = null,
                ToStage = NdcStage.Dealer,
                FromStatus = null,
                ToStatus = NdcStatus.Submitted,
                ActionByUserId = userId,
                ActionOn = DateTime.UtcNow,
                Remarks = "Submitted to Dealer."
            });


            await _db.SaveChangesAsync();
            await tx.CommitAsync();

            TempData["success"] = "Application saved as Draft successfully.";
            return RedirectToAction("OwnerDashboard", "Home");
        }
        catch (Exception ex)
        {
            await tx.RollbackAsync();
            ModelState.AddModelError("", $"Unable to save draft. {ex.Message}");
            return View(vm);
        }
    }

    [HttpGet("View/{id:long}")]
    public async Task<IActionResult> View(long id)
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (string.IsNullOrWhiteSpace(userId))
            return Challenge();

        var app = await _db.NdcApplications
            .AsNoTracking()
            .Include(x => x.Parties)
            .Include(x => x.Documents)
            .Include(x => x.Tasks)
                .ThenInclude(t => t.TaskDefinition)
            .Include(x => x.StatusHistory)
            .FirstOrDefaultAsync(x =>
                x.Id == id &&
                x.CreatedByUserId == userId &&
                x.IsActive);

        if (app == null)
            return NotFound();

        var vm = new NdcApplicationViewVM
        {
            Id = app.Id,
            AppNo = $"NDC-{app.CreatedOn:yyyy}-{app.Id:D5}",

            PlotOrFileNo = app.PlotOrFileNo,
            Block = app.Block,
            SectorOrPhase = app.SectorOrPhase,
            SocietyOrScheme = app.SocietyOrScheme,

            DealerUserId = app.DealerUserId,
            CurrentStage = app.CurrentStage,
            CurrentStatus = app.CurrentStatus,

            Seller = app.Parties.First(p => p.PartyType == NdcPartyType.Seller),
            Purchaser = app.Parties.FirstOrDefault(p => p.PartyType == NdcPartyType.Purchaser),

            Documents = app.Documents
                .OrderBy(d => d.DocType)
                .ToList(),

            Tasks = app.Tasks
                .OrderBy(t => t.TaskDefinition.SortOrder)
                .ToList(),

            Timeline = app.StatusHistory
                .OrderByDescending(h => h.ActionOn)
                .ToList()
        };

        return View("View", vm);
    }

    [HttpGet("UploadDocs/{id:long}")]
    public async Task<IActionResult> UploadDocs(long id)
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (string.IsNullOrWhiteSpace(userId))
            return Challenge();

        var app = await _db.NdcApplications
            .AsNoTracking()
            .FirstOrDefaultAsync(x =>
                x.Id == id &&
                x.CreatedByUserId == userId &&
                x.IsActive &&
                (x.CurrentStatus == NdcStatus.Draft || x.CurrentStatus == NdcStatus.Returned));

        if (app == null)
            return NotFound();

        var docTypes = await _db.NdcDocuments
            .AsNoTracking()
            .Where(d => d.NdcApplicationId == id)
            .Select(d => d.DocType)
            .ToListAsync();

        var vm = new NdcUploadDocsVM
        {
            ApplicationId = id,
            AppNo = $"NDC-{app.CreatedOn:yyyy}-{app.Id:D5}",

            HasCnic = docTypes.Contains(NdcDocumentType.SellerCnicCopy),
            HasPhoto1 = docTypes.Contains(NdcDocumentType.Photo1),
            HasPhoto2 = docTypes.Contains(NdcDocumentType.Photo2),
            HasApplicationToSecretary = docTypes.Contains(NdcDocumentType.ApplicationToSecretary),
        };

        return View("UploadDocs", vm); 
    }

    [HttpPost("UploadDocs/{id:long}")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> UploadDocs(long id, NdcUploadDocsVM vm)
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (string.IsNullOrWhiteSpace(userId))
            return Challenge();

        if (id != vm.ApplicationId)
            return BadRequest();

        var app = await _db.NdcApplications
            .FirstOrDefaultAsync(x =>
                x.Id == id &&
                x.CreatedByUserId == userId &&
                x.IsActive &&
                (x.CurrentStatus == NdcStatus.Draft || x.CurrentStatus == NdcStatus.Returned));

        if (app == null)
            return NotFound();

        if ((vm.OwnerCnicCopy == null || vm.OwnerCnicCopy.Length == 0) &&
            (vm.Photo1 == null || vm.Photo1.Length == 0) &&
            (vm.Photo2 == null || vm.Photo2.Length == 0) &&
            (vm.ApplicationToSecretary == null || vm.ApplicationToSecretary.Length == 0))
        {
            ModelState.AddModelError("", "Please select at least one file to upload.");
        }

        if (!ModelState.IsValid)
        {
            var docTypes = await _db.NdcDocuments
                .AsNoTracking()
                .Where(d => d.NdcApplicationId == id)
                .Select(d => d.DocType)
                .ToListAsync();

            vm.AppNo = $"NDC-{app.CreatedOn:yyyy}-{app.Id:D5}";
            vm.HasCnic = docTypes.Contains(NdcDocumentType.SellerCnicCopy);
            vm.HasPhoto1 = docTypes.Contains(NdcDocumentType.Photo1);
            vm.HasPhoto2 = docTypes.Contains(NdcDocumentType.Photo2);
            vm.HasApplicationToSecretary = docTypes.Contains(NdcDocumentType.ApplicationToSecretary);

            return View("UploadDocs", vm);
        }

        await using var tx = await _db.Database.BeginTransactionAsync();

        try
        {
            var uploadRoot = EnsureUploadFolder(app.Id);
            var newDocs = new List<NdcDocument>();

            async Task SaveDoc(IFormFile? file, NdcDocumentType docType)
            {
                if (file == null || file.Length == 0) return;

                ValidateFile(file, docType);

                var oldDocs = await _db.NdcDocuments
                    .Where(d => d.NdcApplicationId == id && d.DocType == docType)
                    .ToListAsync();

                if (oldDocs.Any())
                {
                    foreach (var old in oldDocs)
                    {
                        TryDeletePhysicalFile(old.FilePath);
                        _db.NdcDocuments.Remove(old);
                    }
                }

                var saved = await SaveFileAsync(file, uploadRoot);

                newDocs.Add(new NdcDocument
                {
                    NdcApplicationId = id,
                    DocType = docType,
                    FileName = saved.FileName,
                    FilePath = saved.RelativePath,
                    ContentType = file.ContentType,
                    FileSizeBytes = file.Length,
                    UploadedByUserId = userId,
                    UploadedOn = DateTime.UtcNow
                });
            }

            await SaveDoc(vm.OwnerCnicCopy, NdcDocumentType.SellerCnicCopy);
            await SaveDoc(vm.Photo1, NdcDocumentType.Photo1);
            await SaveDoc(vm.Photo2, NdcDocumentType.Photo2);
            await SaveDoc(vm.ApplicationToSecretary, NdcDocumentType.ApplicationToSecretary);

            if (newDocs.Count > 0)
                _db.NdcDocuments.AddRange(newDocs);

            await UpdateOwnerTasksCompletionAsync(id, userId);

            app.UpdatedOn = DateTime.UtcNow;

            _db.NdcStatusHistories.Add(new NdcStatusHistory
            {
                NdcApplicationId = id,
                FromStage = app.CurrentStage,
                ToStage = app.CurrentStage,
                FromStatus = app.CurrentStatus,
                ToStatus = app.CurrentStatus,
                ActionByUserId = userId,
                ActionOn = DateTime.UtcNow,
                Remarks = "Owner uploaded missing documents."
            });

            await _db.SaveChangesAsync();
            await tx.CommitAsync();

            TempData["success"] = "Documents uploaded successfully.";
            return RedirectToAction("View", "Applications", new { id });
        }
        catch (Exception ex)
        {
            await tx.RollbackAsync();
            ModelState.AddModelError("", $"Unable to upload documents. {ex.Message}");
            return View("UploadDocs", vm);
        }
    }

    private async Task UpdateOwnerTasksCompletionAsync(long appId, string userId)
    {
        var docTypes = await _db.NdcDocuments
            .AsNoTracking()
            .Where(d => d.NdcApplicationId == appId)
            .Select(d => d.DocType)
            .ToListAsync();

        var tasks = await _db.NdcTasks
            .Where(t => t.NdcApplicationId == appId)
            .ToListAsync();

        void Mark(int taskDefId, bool done, string note)
        {
            var t = tasks.FirstOrDefault(x => x.TaskDefinitionId == taskDefId);
            if (t == null) return;

            if (done && !t.IsCompleted)
            {
                t.IsCompleted = true;
                t.CompletedByUserId = userId;
                t.CompletedOn = DateTime.UtcNow;
                t.Notes = note;
            }
            else if (!done && t.IsCompleted)
            {
                 t.IsCompleted = false; t.CompletedByUserId = null; t.CompletedOn = null; t.Notes = null;
            }
        }


        Mark(1, true, "Form filled.");

        var hasPhoto1 = docTypes.Contains(NdcDocumentType.Photo1);
        var hasPhoto2 = docTypes.Contains(NdcDocumentType.Photo2);
        Mark(2, hasPhoto1 && hasPhoto2, "Both photos uploaded.");

        Mark(3, docTypes.Contains(NdcDocumentType.SellerCnicCopy), "CNIC uploaded.");
        Mark(4, docTypes.Contains(NdcDocumentType.ApplicationToSecretary), "Application uploaded.");
    }

    private void TryDeletePhysicalFile(string relativePath)
    {
        try
        {
            if (string.IsNullOrWhiteSpace(relativePath)) return;

            var full = Path.Combine(_env.WebRootPath ?? "wwwroot", relativePath.TrimStart('/').Replace("/", Path.DirectorySeparatorChar.ToString()));
            if (System.IO.File.Exists(full))
                System.IO.File.Delete(full);
        }
        catch
        {
        }
        }


    private async Task<List<SelectListItem>> GetDealersFromUsersAsync()
    {
        var dealers = await _db.Users
            .AsNoTracking()
            .Where(u => EF.Property<int>(u, "UserType") == 2) 
            .OrderBy(u => u.UserName)
            .Select(u => new SelectListItem
            {
                Text = string.IsNullOrWhiteSpace(u.UserName) ? u.Email! : u.UserName!,
                Value = u.Id
            })
            .ToListAsync();

        dealers.Insert(0, new SelectListItem("Select Dealer", "", true, false));
        return dealers;
    }

    private string EnsureUploadFolder(long appId)
    {
        var root = Path.Combine(_env.WebRootPath ?? "wwwroot", "uploads", "ndc", appId.ToString());
        if (!Directory.Exists(root))
            Directory.CreateDirectory(root);
        return root;
    }

    private static void ValidateFile(IFormFile file, NdcDocumentType docType)
    {
        const long maxBytes = 10 * 1024 * 1024; 
        if (file.Length > maxBytes)
            throw new InvalidOperationException($"{docType} file too large. Max 10MB allowed.");

        var ext = Path.GetExtension(file.FileName)?.ToLowerInvariant() ?? "";
        var allowed = docType switch
        {
            NdcDocumentType.Photo1 or NdcDocumentType.Photo2 => new[] { ".jpg", ".jpeg", ".png" },
            _ => new[] { ".pdf", ".jpg", ".jpeg", ".png" }
        };

        if (!allowed.Contains(ext))
            throw new InvalidOperationException($"{docType} invalid file type. Allowed: {string.Join(", ", allowed)}");
    }

    private async Task<(string FileName, string RelativePath)> SaveFileAsync(IFormFile file, string uploadRoot)
    {
        var ext = Path.GetExtension(file.FileName);
        var safeName = $"{Guid.NewGuid():N}{ext}";
        var fullPath = Path.Combine(uploadRoot, safeName);

        await using (var fs = new FileStream(fullPath, FileMode.Create))
        {
            await file.CopyToAsync(fs);
        }

        var relative = fullPath
            .Replace(_env.WebRootPath ?? "wwwroot", "")
            .Replace("\\", "/");

        if (!relative.StartsWith("/"))
            relative = "/" + relative;

        return (safeName, relative);
    }

    private static void AutoCompleteOwnerDraftTasks(string userId, List<NdcTask> tasks, List<NdcDocument> uploadedDocs)
    {

        void MarkCompleted(int taskDefId, string note)
        {
            var t = tasks.FirstOrDefault(x => x.TaskDefinitionId == taskDefId);
            if (t == null) return;

            t.IsCompleted = true;
            t.CompletedByUserId = userId;
            t.CompletedOn = DateTime.UtcNow;
            t.Notes = note;
        }

        MarkCompleted(1, "Form filled.");

        var hasPhoto1 = uploadedDocs.Any(d => d.DocType == NdcDocumentType.Photo1);
        var hasPhoto2 = uploadedDocs.Any(d => d.DocType == NdcDocumentType.Photo2);
        if (hasPhoto1 && hasPhoto2)
            MarkCompleted(2, "Both photos uploaded.");

        if (uploadedDocs.Any(d => d.DocType == NdcDocumentType.SellerCnicCopy))
            MarkCompleted(3, "CNIC uploaded.");

        if (uploadedDocs.Any(d => d.DocType == NdcDocumentType.ApplicationToSecretary))
            MarkCompleted(4, "Application uploaded.");
    }
}
