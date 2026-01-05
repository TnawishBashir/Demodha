using System.Security.Claims;
using Demodha.Data;
using Demodha.Models;
using Demodha.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Demodha.Controllers;

[Authorize]
[Route("Dealer/Applications")]
public class DealerApplicationsController : Controller
{
    private readonly ApplicationDbContext _db;
    private readonly IWebHostEnvironment _env;

    public DealerApplicationsController(ApplicationDbContext db, IWebHostEnvironment env)
    {
        _db = db;
        _env = env;
    }

    [HttpGet("Open/{id:long}")]
    public async Task<IActionResult> Open(long id)
    {
        if (GetUserTypeFromClaims() != 2) return Forbid();

        var dealerUserId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (string.IsNullOrWhiteSpace(dealerUserId)) return Challenge();

        var app = await _db.NdcApplications
            .AsNoTracking()
            .Include(x => x.Parties)
            .FirstOrDefaultAsync(x => x.Id == id && x.IsActive && x.DealerUserId == dealerUserId);

        if (app == null) return NotFound();

        var docs = await _db.NdcDocuments
            .AsNoTracking()
            .Where(d => d.NdcApplicationId == id)
            .Select(d => d.DocType)
            .ToListAsync();

        var ownerName = app.Parties.FirstOrDefault(p => p.PartyType == NdcPartyType.Seller)?.FullName ?? "";

        var vm = new DealerApplicationStep2VM
        {
            ApplicationId = app.Id,
            AppNo = $"NDC-{app.CreatedOn:yyyy}-{app.Id:D5}",
            Plot = $"{app.PlotOrFileNo}, Block {app.Block} ({app.SectorOrPhase})",
            OwnerName = ownerName,
            HasPurchaser = app.Parties.Any(p => p.PartyType == NdcPartyType.Purchaser),

            HasChallan = docs.Contains(NdcDocumentType.PaymentChallanForm),
            HasSellerConsent = docs.Contains(NdcDocumentType.SellerConsentForm),
            HasPurchaserConsent = docs.Contains(NdcDocumentType.PurchaserConsentForm)
        };

        return View("Open", vm);
    }

    [HttpPost("Open")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Open(DealerApplicationStep2VM vm, string actionType)
    {
        if (GetUserTypeFromClaims() != 2) return Forbid();

        var dealerUserId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (string.IsNullOrWhiteSpace(dealerUserId)) return Challenge();

        var app = await _db.NdcApplications
            .Include(x => x.Parties)
            .FirstOrDefaultAsync(x => x.Id == vm.ApplicationId && x.IsActive && x.DealerUserId == dealerUserId);

        if (app == null) return NotFound();

        async Task ReloadFlagsAsync()
        {
            var docs = await _db.NdcDocuments.AsNoTracking()
                .Where(d => d.NdcApplicationId == app.Id)
                .Select(d => d.DocType)
                .ToListAsync();

            vm.AppNo = $"NDC-{app.CreatedOn:yyyy}-{app.Id:D5}";
            vm.Plot = $"{app.PlotOrFileNo}, Block {app.Block} ({app.SectorOrPhase})";
            vm.OwnerName = app.Parties.FirstOrDefault(p => p.PartyType == NdcPartyType.Seller)?.FullName ?? "";
            vm.HasPurchaser = app.Parties.Any(p => p.PartyType == NdcPartyType.Purchaser);

            vm.HasChallan = docs.Contains(NdcDocumentType.PaymentChallanForm);
            vm.HasSellerConsent = docs.Contains(NdcDocumentType.SellerConsentForm);
            vm.HasPurchaserConsent = docs.Contains(NdcDocumentType.PurchaserConsentForm);
        }

        var isSubmit = string.Equals(actionType, "submit", StringComparison.OrdinalIgnoreCase);
        var isSave = string.Equals(actionType, "save", StringComparison.OrdinalIgnoreCase);

        if (!isSave && !isSubmit)
        {
            ModelState.AddModelError("", "Invalid action.");
            await ReloadFlagsAsync();
            return View("Open", vm);
        }

        var hasPurchaser = app.Parties.Any(p => p.PartyType == NdcPartyType.Purchaser);

        if (isSubmit)
        {
            if (vm.PaymentChallan == null && !await HasDocAsync(app.Id, NdcDocumentType.PaymentChallanForm))
                ModelState.AddModelError(nameof(vm.PaymentChallan), "Payment Challan is required.");

            if (vm.SellerConsent == null && !await HasDocAsync(app.Id, NdcDocumentType.SellerConsentForm))
                ModelState.AddModelError(nameof(vm.SellerConsent), "Seller Consent is required.");

            if (hasPurchaser)
            {
                if (vm.PurchaserConsent == null && !await HasDocAsync(app.Id, NdcDocumentType.PurchaserConsentForm))
                    ModelState.AddModelError(nameof(vm.PurchaserConsent), "Purchaser Consent is required.");
            }
        }

        if (!ModelState.IsValid)
        {
            await ReloadFlagsAsync();
            return View("Open", vm);
        }

        await using var tx = await _db.Database.BeginTransactionAsync();
        try
        {
            if (app.CurrentStage != NdcStage.Dealer)
                app.CurrentStage = NdcStage.Dealer;

            var uploadRoot = EnsureUploadFolder(app.Id);

            if (vm.PaymentChallan != null)
                await SaveDealerDocAsync(app.Id, vm.PaymentChallan, NdcDocumentType.PaymentChallanForm, uploadRoot, dealerUserId);

            if (vm.SellerConsent != null)
                await SaveDealerDocAsync(app.Id, vm.SellerConsent, NdcDocumentType.SellerConsentForm, uploadRoot, dealerUserId);

            if (vm.PurchaserConsent != null)
                await SaveDealerDocAsync(app.Id, vm.PurchaserConsent, NdcDocumentType.PurchaserConsentForm, uploadRoot, dealerUserId);

            if (isSubmit)
            {
                var fromStage = app.CurrentStage;
                var fromStatus = app.CurrentStatus;

                app.CurrentStage = NdcStage.TransferDesk;
                app.CurrentStatus = NdcStatus.UnderReview;
                app.UpdatedOn = DateTime.UtcNow;

                _db.NdcStatusHistories.Add(new NdcStatusHistory
                {
                    NdcApplicationId = app.Id,
                    FromStage = fromStage,
                    ToStage = NdcStage.TransferDesk,
                    FromStatus = fromStatus,
                    ToStatus = NdcStatus.UnderReview,
                    ActionByUserId = dealerUserId,
                    ActionOn = DateTime.UtcNow,
                    Remarks = "Dealer submitted to Transfer Desk."
                });

                await _db.SaveChangesAsync();
                await tx.CommitAsync();

                TempData["success"] = "Submitted to Transfer Desk successfully.";
                return RedirectToAction("DealerDashboard", "Home");
            }
            else
            {
                var fromStage = app.CurrentStage;
                var fromStatus = app.CurrentStatus;

                app.CurrentStage = NdcStage.Dealer;
                app.CurrentStatus = NdcStatus.Draft;
                app.UpdatedOn = DateTime.UtcNow;

                _db.NdcStatusHistories.Add(new NdcStatusHistory
                {
                    NdcApplicationId = app.Id,
                    FromStage = fromStage,
                    ToStage = NdcStage.Dealer,
                    FromStatus = fromStatus,
                    ToStatus = NdcStatus.Draft,
                    ActionByUserId = dealerUserId,
                    ActionOn = DateTime.UtcNow,
                    Remarks = "Dealer saved Step-2 draft."
                });

                await _db.SaveChangesAsync();
                await tx.CommitAsync();

                TempData["success"] = "Saved Step-2 draft successfully.";
                return RedirectToAction("Open", new { id = app.Id });
            }
        }
        catch (Exception ex)
        {
            await tx.RollbackAsync();
            ModelState.AddModelError("", $"Unable to save. {ex.Message}");
            await ReloadFlagsAsync();
            return View("Open", vm);
        }
    }


    private int GetUserTypeFromClaims()
    {
        var claim = User.FindFirst("userType")?.Value;
        return int.TryParse(claim, out var userType) ? userType : 0;
    }

    private async Task<bool> HasDocAsync(long appId, NdcDocumentType type)
    {
        return await _db.NdcDocuments.AsNoTracking()
            .AnyAsync(d => d.NdcApplicationId == appId && d.DocType == type);
    }

    private string EnsureUploadFolder(long appId)
    {
        var root = Path.Combine(_env.WebRootPath, "uploads", "ndc", appId.ToString());
        Directory.CreateDirectory(root);
        return root;
    }

    private async Task SaveDealerDocAsync(long appId, IFormFile file, NdcDocumentType docType, string uploadRoot, string uploadedByUserId)
    {
        if (file.Length > 10 * 1024 * 1024) 
            throw new InvalidOperationException("File size must be <= 10MB.");

        var ext = Path.GetExtension(file.FileName);
        var safeName = $"{docType}-{Guid.NewGuid():N}{ext}";
        var absPath = Path.Combine(uploadRoot, safeName);

        await using (var fs = new FileStream(absPath, FileMode.Create))
            await file.CopyToAsync(fs);

        var relativePath = $"/uploads/ndc/{appId}/{safeName}";

        var old = await _db.NdcDocuments
            .FirstOrDefaultAsync(d => d.NdcApplicationId == appId && d.DocType == docType);

        if (old != null)
            _db.NdcDocuments.Remove(old);

        _db.NdcDocuments.Add(new NdcDocument
        {
            NdcApplicationId = appId,
            DocType = docType,
            FileName = safeName,
            FilePath = relativePath,
            ContentType = file.ContentType,
            FileSizeBytes = file.Length,
            UploadedByUserId = uploadedByUserId,
            UploadedOn = DateTime.UtcNow
        });

        await _db.SaveChangesAsync();
    }
}
