using System.Security.Claims;
using Demodha.Data;
using Demodha.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Demodha.Controllers;

[Authorize]
[Route("TransferDesk/Applications")]
public class TransferDeskApplicationsController : Controller
{
    private readonly ApplicationDbContext _db;

    public TransferDeskApplicationsController(ApplicationDbContext db)
    {
        _db = db;
    }

    [HttpGet("Open/{id:long}")]
    public async Task<IActionResult> Open(long id)
    {
        // must be transfer desk user
        var userType = User.FindFirst("userType")?.Value;
        if (userType != "3") return Forbid();

        var app = await _db.NdcApplications
            .AsNoTracking()
            .Include(x => x.Parties)
            .Include(x => x.Documents)
            .FirstOrDefaultAsync(x => x.Id == id && x.IsActive);

        if (app == null) return NotFound();

        // ensure it is at TransferDesk stage
        if (app.CurrentStage != NdcStage.TransferDesk)
            return Forbid();

        return View("Open", app);
    }

    [HttpPost("UploadEStamp/{id:long}")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> UploadEStamp(long id, IFormFile eStampFile)
    {
        var userType = User.FindFirst("userType")?.Value;
        if (userType != "3") return Forbid();

        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (string.IsNullOrWhiteSpace(userId)) return Challenge();

        var app = await _db.NdcApplications.FirstOrDefaultAsync(x => x.Id == id && x.IsActive);
        if (app == null) return NotFound();

        if (eStampFile == null || eStampFile.Length == 0)
        {
            TempData["error"] = "Please upload e-Stamp file.";
            return RedirectToAction(nameof(Open), new { id });
        }

        // TODO: your SaveFileAsync/EnsureUploadFolder methods same as owner flow
        // Here: we just store dummy path for now
        var doc = new NdcDocument
        {
            NdcApplicationId = id,
            DocType = NdcDocumentType.EStampPaper,
            FileName = eStampFile.FileName,
            FilePath = $"uploads/ndc/{id}/{Guid.NewGuid()}_{eStampFile.FileName}",
            ContentType = eStampFile.ContentType,
            FileSizeBytes = eStampFile.Length,
            UploadedByUserId = userId,
            UploadedOn = DateTime.UtcNow
        };

        _db.NdcDocuments.Add(doc);

        app.UpdatedOn = DateTime.UtcNow;
        app.CurrentStatus = NdcStatus.UnderReview;

        _db.NdcStatusHistories.Add(new NdcStatusHistory
        {
            NdcApplicationId = id,
            FromStage = app.CurrentStage,
            ToStage = app.CurrentStage,
            FromStatus = app.CurrentStatus,
            ToStatus = NdcStatus.UnderReview,
            ActionByUserId = userId,
            ActionOn = DateTime.UtcNow,
            Remarks = "e-Stamp uploaded and marked UnderReview."
        });

        await _db.SaveChangesAsync();

        TempData["success"] = "e-Stamp uploaded.";
        return RedirectToAction(nameof(Open), new { id });
    }

    [HttpPost("SendToDirectorates/{id:long}")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> SendToDirectorates(long id)
    {
        var userType = User.FindFirst("userType")?.Value;
        if (userType != "3") return Forbid();

        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (string.IsNullOrWhiteSpace(userId)) return Challenge();

        var app = await _db.NdcApplications
            .Include(x => x.Documents)
            .Include(x => x.Parties)
            .FirstOrDefaultAsync(x => x.Id == id && x.IsActive);

        if (app == null) return NotFound();

        // Validate required docs (Step-3)
        bool hasOwnerDocs =
            app.Documents.Any(d => d.DocType == NdcDocumentType.SellerCnicCopy) &&
            app.Documents.Any(d => d.DocType == NdcDocumentType.Photo1) &&
            app.Documents.Any(d => d.DocType == NdcDocumentType.Photo2) &&
            app.Documents.Any(d => d.DocType == NdcDocumentType.ApplicationToSecretary);

        bool hasChallan = app.Documents.Any(d => d.DocType == NdcDocumentType.PaymentChallanForm);
        bool hasSellerConsent = app.Documents.Any(d => d.DocType == NdcDocumentType.SellerConsentForm);

        bool hasPurchaser = app.Parties.Any(p => p.PartyType == NdcPartyType.Purchaser);
        bool hasPurchaserConsent = !hasPurchaser || app.Documents.Any(d => d.DocType == NdcDocumentType.PurchaserConsentForm);

        bool hasEStamp = app.Documents.Any(d => d.DocType == NdcDocumentType.EStampPaper);

        if (!hasOwnerDocs || !hasChallan || !hasSellerConsent || !hasPurchaserConsent || !hasEStamp)
        {
            TempData["error"] = "Cannot send to directorates. Missing required documents/checks (Owner docs, Challan, consents, e-Stamp).";
            return RedirectToAction(nameof(Open), new { id });
        }

        await using var tx = await _db.Database.BeginTransactionAsync();

        try
        {
            // create clearance rows if not exist (Record, Legal, Land, Plans, BC, Finance)
            var existing = await _db.NdcClearances
                .Where(x => x.NdcApplicationId == id)
                .Select(x => x.Department)
                .ToListAsync();

            void AddClearanceIfMissing(NdcDepartment dept)
            {
                if (existing.Contains(dept)) return;

                _db.NdcClearances.Add(new NdcClearance
                {
                    NdcApplicationId = id,
                    Department = dept,
                    //Status = NdcClearanceStatus.Pending,
                    Remarks = "Sent by Transfer Desk",
                });
            }

            AddClearanceIfMissing(NdcDepartment.Record);
            AddClearanceIfMissing(NdcDepartment.Legal);
            AddClearanceIfMissing(NdcDepartment.Land);
            AddClearanceIfMissing(NdcDepartment.Plans);
            AddClearanceIfMissing(NdcDepartment.BC);
            AddClearanceIfMissing(NdcDepartment.Finance);

            // move stage
            var fromStage = app.CurrentStage;
            var fromStatus = app.CurrentStatus;

            app.CurrentStage = NdcStage.MiscDirectorates;
            app.CurrentStatus = NdcStatus.UnderReview;
            app.UpdatedOn = DateTime.UtcNow;

            _db.NdcStatusHistories.Add(new NdcStatusHistory
            {
                NdcApplicationId = id,
                FromStage = fromStage,
                ToStage = NdcStage.MiscDirectorates,
                FromStatus = fromStatus,
                ToStatus = NdcStatus.UnderReview,
                ActionByUserId = userId,
                ActionOn = DateTime.UtcNow,
                Remarks = "Transfer Desk verified docs and sent to directorates for clearance."
            });

            await _db.SaveChangesAsync();
            await tx.CommitAsync();

            TempData["success"] = "Case sent to directorates (Record/Legal/Land/Plans/BC/Finance).";
            return RedirectToAction("TransferDeskDashboard", "Home");
        }
        catch (Exception ex)
        {
            await tx.RollbackAsync();
            TempData["error"] = $"Failed to send case. {ex.Message}";
            return RedirectToAction(nameof(Open), new { id });
        }
    }
}
