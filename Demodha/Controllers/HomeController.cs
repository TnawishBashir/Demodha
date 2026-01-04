using System.Diagnostics;
using System.Security.Claims;
using Demodha.Data;
using Demodha.Models;
using Demodha.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Demodha.Controllers;

[Authorize]
public class HomeController : Controller
{
    private readonly ILogger<HomeController> _logger;
    private readonly ApplicationDbContext _db;

    public HomeController(ILogger<HomeController> logger, ApplicationDbContext db)
    {
        _logger = logger;
        _db = db;
    }

    public IActionResult Index()
    {
        var userType = GetUserTypeFromClaims();

        return userType switch
        {
            1 => RedirectToAction(nameof(OwnerDashboard)),
            2 => RedirectToAction(nameof(DealerDashboard)),
            3 => RedirectToAction(nameof(TransferDeskDashboard)),
            4 => RedirectToAction(nameof(DirectoratesDashboard)),
            _ => RedirectToAction(nameof(OwnerDashboard))
        };
    }

    public async Task<IActionResult> OwnerDashboard()
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (string.IsNullOrWhiteSpace(userId))
            return Challenge();

        var q = _db.NdcApplications
            .AsNoTracking()
            .Where(x => x.CreatedByUserId == userId && x.IsActive);

        var total = await q.CountAsync();
        var draft = await q.CountAsync(x => x.CurrentStatus == NdcStatus.Draft);
        var returned = await q.CountAsync(x => x.CurrentStatus == NdcStatus.Returned);

        var submittedToDealer = await q.CountAsync(x =>
            x.CurrentStatus == NdcStatus.Submitted || x.CurrentStatus == NdcStatus.UnderReview);

        var readyForDealer = await _db.NdcApplications
            .AsNoTracking()
            .Where(x => x.CreatedByUserId == userId && x.IsActive && x.CurrentStatus == NdcStatus.Draft)
            .CountAsync(x =>
                _db.NdcDocuments.Any(d => d.NdcApplicationId == x.Id && d.DocType == NdcDocumentType.SellerCnicCopy) &&
                _db.NdcDocuments.Any(d => d.NdcApplicationId == x.Id && d.DocType == NdcDocumentType.Photo1) &&
                _db.NdcDocuments.Any(d => d.NdcApplicationId == x.Id && d.DocType == NdcDocumentType.Photo2) &&
                _db.NdcDocuments.Any(d => d.NdcApplicationId == x.Id && d.DocType == NdcDocumentType.ApplicationToSecretary)
            );

        var apps = await q
            .OrderByDescending(x => x.UpdatedOn ?? x.CreatedOn)
            .Take(10)
            .Select(x => new OwnerAppRowVM
            {
                Id = x.Id,
                AppNo = $"NDC-{x.CreatedOn:yyyy}-{x.Id:D5}",
                Plot = $"{x.PlotOrFileNo}, Block {x.Block} ({x.SectorOrPhase})",
                StatusText = GetStatusText(x.CurrentStatus, x.CurrentStage),
                Badge = GetStatusBadge(x.CurrentStatus),
                Updated = (x.UpdatedOn ?? x.CreatedOn).ToLocalTime().ToString("MMM dd, yyyy"),
                Progress = GetProgress(x.CurrentStage, x.CurrentStatus)
            })
            .ToListAsync();

        var latestDraftId = await q
            .Where(x => x.CurrentStatus == NdcStatus.Draft)
            .OrderByDescending(x => x.UpdatedOn ?? x.CreatedOn)
            .Select(x => (long?)x.Id)
            .FirstOrDefaultAsync();

        var checklist = new List<OwnerDocChecklistVM>();
        if (latestDraftId.HasValue)
        {
            var docs = await _db.NdcDocuments
                .AsNoTracking()
                .Where(d => d.NdcApplicationId == latestDraftId.Value)
                .Select(d => d.DocType)
                .ToListAsync();

            checklist.Add(new OwnerDocChecklistVM { Name = "NDC Application Form", Done = true, Note = "Filled & saved" });
            checklist.Add(new OwnerDocChecklistVM { Name = "2 Photographs", Done = docs.Contains(NdcDocumentType.Photo1) && docs.Contains(NdcDocumentType.Photo2), Note = "Upload Photo1 & Photo2" });
            checklist.Add(new OwnerDocChecklistVM { Name = "CNIC Copy (Seller)", Done = docs.Contains(NdcDocumentType.SellerCnicCopy), Note = "Upload CNIC copy" });
            checklist.Add(new OwnerDocChecklistVM { Name = "Application to Secretary DHAG", Done = docs.Contains(NdcDocumentType.ApplicationToSecretary), Note = "Upload application" });

            checklist.Add(new OwnerDocChecklistVM { Name = "Payment Challan (Dealer will attach)", Done = docs.Contains(NdcDocumentType.PaymentChallanForm), Note = "Dealer step" });
            checklist.Add(new OwnerDocChecklistVM { Name = "Seller Consent Form (Dealer will attach)", Done = docs.Contains(NdcDocumentType.SellerConsentForm), Note = "Dealer step" });
            checklist.Add(new OwnerDocChecklistVM { Name = "Purchaser Consent Form (Dealer will attach)", Done = docs.Contains(NdcDocumentType.PurchaserConsentForm), Note = "Dealer step" });
        }

        var activity = await _db.NdcStatusHistories
            .AsNoTracking()
            .Where(h => h.NdcApplication.CreatedByUserId == userId)
            .OrderByDescending(h => h.ActionOn)
            .Take(8)
            .Select(h => new OwnerActivityVM
            {
                When = h.ActionOn.ToLocalTime().ToString("MMM dd, yyyy hh:mm tt"),
                Text = $"Application #{h.NdcApplicationId} moved to {h.ToStage} ({h.ToStatus})."
            })
            .ToListAsync();

        var vm = new OwnerDashboardVM
        {
            Total = total,
            Draft = draft,
            SubmittedToDealer = submittedToDealer,
            Returned = returned,
            ReadyForDealer = readyForDealer,
            Applications = apps,
            DocChecklist = checklist,
            Activity = activity,
             LatestDraftId = latestDraftId
        };

        return View("OwnerDashboard", vm);
    }

    public async Task<IActionResult> DealerDashboard()
    {
        var userType = GetUserTypeFromClaims();
        if (userType != 2)
            return Forbid();

        var dealerUserId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (string.IsNullOrWhiteSpace(dealerUserId))
            return Challenge();

        var appsQ = _db.NdcApplications
            .AsNoTracking()
            .Where(x => x.IsActive && x.DealerUserId == dealerUserId);

        var pendingFromOwner = await appsQ.CountAsync(x =>
            x.CurrentStage == NdcStage.Dealer &&
            x.CurrentStatus == NdcStatus.Submitted);

        var draft = await appsQ.CountAsync(x =>
            x.CurrentStage == NdcStage.Dealer &&
            x.CurrentStatus == NdcStatus.Draft);

        var returned = await appsQ.CountAsync(x =>
            x.CurrentStage == NdcStage.Dealer &&
            x.CurrentStatus == NdcStatus.Returned);

        var submittedToTransfer = await appsQ.CountAsync(x =>
            x.CurrentStage == NdcStage.TransferDesk);

        var docsQ = _db.NdcDocuments.AsNoTracking();

        // Grid
        var apps = await appsQ
            .Where(x => x.CurrentStage == NdcStage.Dealer)
            .OrderByDescending(x => x.UpdatedOn ?? x.CreatedOn)
            .Select(x => new
            {
                x.Id,
                x.CreatedOn,
                x.PlotOrFileNo,
                x.Block,
                x.CurrentStatus,

                OwnerName = _db.NdcParties
                    .Where(p => p.NdcApplicationId == x.Id && p.PartyType == NdcPartyType.Seller)
                    .Select(p => p.FullName)
                    .FirstOrDefault(),

                HasPurchaser = _db.NdcParties
                    .Any(p => p.NdcApplicationId == x.Id && p.PartyType == NdcPartyType.Purchaser),

                HasChallan = docsQ.Any(d => d.NdcApplicationId == x.Id && d.DocType == NdcDocumentType.PaymentChallanForm),
                HasSellerConsent = docsQ.Any(d => d.NdcApplicationId == x.Id && d.DocType == NdcDocumentType.SellerConsentForm),
                HasPurchaserConsent = docsQ.Any(d => d.NdcApplicationId == x.Id && d.DocType == NdcDocumentType.PurchaserConsentForm)
            })
            .ToListAsync();

        var rows = apps.Select(a =>
        {
            // Progress
            var progress = 20;
            if (a.HasChallan) progress += 25;
            if (a.HasSellerConsent) progress += 25;
            progress += a.HasPurchaser ? (a.HasPurchaserConsent ? 30 : 0) : 30;
            progress = Math.Min(progress, 100);

            var statusText = a.CurrentStatus switch
            {
                NdcStatus.Submitted => "New (Owner Submitted)",
                NdcStatus.Draft => "In Review",
                NdcStatus.Returned => "Returned (Fix & Resubmit)",
                _ => a.CurrentStatus.ToString()
            };

            var badge = a.CurrentStatus switch
            {
                NdcStatus.Submitted => "info",
                NdcStatus.Draft => "primary",
                NdcStatus.Returned => "warning",
                _ => "secondary"
            };

            return new DealerApplicationRowVM
            {
                Id = a.Id,
                AppNo = $"NDC-{a.CreatedOn:yyyy}-{a.Id:D5}",
                Plot = $"{a.PlotOrFileNo}, Block {a.Block}",
                OwnerName = a.OwnerName ?? "",
                Status = statusText,
                Badge = badge,
                HasChallan = a.HasChallan,
                HasSellerConsent = a.HasSellerConsent,
                HasPurchaserConsent = a.HasPurchaserConsent,
                Progress = progress
            };
        }).ToList();

        var vm = new DealerDashboardVM
        {
            PendingFromOwner = pendingFromOwner,
            Draft = draft,
            Returned = returned,
            SubmittedToTransfer = submittedToTransfer,
            Applications = rows
        };

        return View("DealerDashboard", vm);
    }



    public async Task<IActionResult> TransferDeskDashboard()
    {
        var userType = GetUserTypeFromClaims();
        if (userType != 3) // TransferDesk userType
            return Forbid();

        var docsQ = _db.NdcDocuments.AsNoTracking();

        // TransferDesk queue = applications that dealer has submitted to transfer desk
        var q = _db.NdcApplications
            .AsNoTracking()
            .Where(x => x.IsActive && x.CurrentStage == NdcStage.TransferDesk);

        var newFromDealer = await q.CountAsync(x => x.CurrentStatus == NdcStatus.Submitted);
        var returnedToDealer = await q.CountAsync(x => x.CurrentStatus == NdcStatus.Returned);

        // Pending e-stamp (submitted/under review but no estamp)
        var pendingEStamp = await q.CountAsync(x =>
            (x.CurrentStatus == NdcStatus.Submitted || x.CurrentStatus == NdcStatus.UnderReview) &&
            !docsQ.Any(d => d.NdcApplicationId == x.Id && d.DocType == NdcDocumentType.EStampPaper));

        // Sent to directorates = stage MiscDirectorates (or you can mark status UnderReview + stage MiscDirectorates)
        var sentToDirectorates = await _db.NdcApplications
            .AsNoTracking()
            .CountAsync(x => x.IsActive && x.CurrentStage == NdcStage.MiscDirectorates);

        // Grid
        var apps = await q
            .OrderByDescending(x => x.UpdatedOn ?? x.CreatedOn)
            .Take(30)
            .Select(x => new
            {
                x.Id,
                x.CreatedOn,
                x.PlotOrFileNo,
                x.Block,
                x.SectorOrPhase,
                x.CurrentStatus,
                x.DealerUserId,

                OwnerName = _db.NdcParties
                    .Where(p => p.NdcApplicationId == x.Id && p.PartyType == NdcPartyType.Seller)
                    .Select(p => p.FullName)
                    .FirstOrDefault(),

                HasPurchaser = _db.NdcParties.Any(p => p.NdcApplicationId == x.Id && p.PartyType == NdcPartyType.Purchaser),

                HasCnic = docsQ.Any(d => d.NdcApplicationId == x.Id && d.DocType == NdcDocumentType.SellerCnicCopy),
                HasP1 = docsQ.Any(d => d.NdcApplicationId == x.Id && d.DocType == NdcDocumentType.Photo1),
                HasP2 = docsQ.Any(d => d.NdcApplicationId == x.Id && d.DocType == NdcDocumentType.Photo2),
                HasSec = docsQ.Any(d => d.NdcApplicationId == x.Id && d.DocType == NdcDocumentType.ApplicationToSecretary),

                HasChallan = docsQ.Any(d => d.NdcApplicationId == x.Id && d.DocType == NdcDocumentType.PaymentChallanForm),
                HasSellerConsent = docsQ.Any(d => d.NdcApplicationId == x.Id && d.DocType == NdcDocumentType.SellerConsentForm),
                HasPurchaserConsent = docsQ.Any(d => d.NdcApplicationId == x.Id && d.DocType == NdcDocumentType.PurchaserConsentForm),
                HasEStamp = docsQ.Any(d => d.NdcApplicationId == x.Id && d.DocType == NdcDocumentType.EStampPaper),

                DealerName = _db.Users
                    .Where(u => u.Id == x.DealerUserId)
                    .Select(u => u.UserName ?? u.Email)
                    .FirstOrDefault()
            })
            .ToListAsync();

        var rows = apps.Select(a =>
        {
            var hasOwnerDocs = a.HasCnic && a.HasP1 && a.HasP2 && a.HasSec;

            var progress = 0;
            if (hasOwnerDocs) progress += 30;
            if (a.HasChallan) progress += 20;
            if (a.HasSellerConsent) progress += 20;
            progress += a.HasPurchaser ? (a.HasPurchaserConsent ? 10 : 0) : 10;
            if (a.HasEStamp) progress += 20;
            progress = Math.Min(progress, 100);

            var badge = a.CurrentStatus switch
            {
                NdcStatus.Submitted => "info",
                NdcStatus.UnderReview => "primary",
                NdcStatus.Returned => "warning",
                NdcStatus.Approved => "success",
                _ => "secondary"
            };

            var statusText = a.CurrentStatus switch
            {
                NdcStatus.Submitted => "New (Dealer Submitted)",
                NdcStatus.UnderReview => "In Verification (Transfer Desk)",
                NdcStatus.Returned => "Returned",
                _ => a.CurrentStatus.ToString()
            };

            return new TransferDeskApplicationRowVM
            {
                Id = a.Id,
                AppNo = $"NDC-{a.CreatedOn:yyyy}-{a.Id:D5}",
                Plot = $"{a.PlotOrFileNo}, Block {a.Block} ({a.SectorOrPhase})",
                DealerName = a.DealerName ?? "",
                OwnerName = a.OwnerName ?? "",
                Status = statusText,
                Badge = badge,
                HasOwnerDocs = hasOwnerDocs,
                HasChallan = a.HasChallan,
                HasSellerConsent = a.HasSellerConsent,
                HasPurchaserConsent = a.HasPurchaser ? a.HasPurchaserConsent : true,
                HasEStamp = a.HasEStamp,
                Progress = progress
            };
        }).ToList();

        var vm = new TransferDeskDashboardVM
        {
            NewFromDealer = newFromDealer,
            PendingEStamp = pendingEStamp,
            SentToDirectorates = sentToDirectorates,
            ReturnedToDealer = returnedToDealer,
            Applications = rows
        };

        return View("TransferDeskDashboard", vm);
    }

    public IActionResult DirectoratesDashboard() => View("DirectoratesDashboard");

    public IActionResult Privacy() => View();

    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }

    private int GetUserTypeFromClaims()
    {
        var claim = User.FindFirst("userType")?.Value;
        return int.TryParse(claim, out var userType) ? userType : 0;
    }

    private static string GetStatusBadge(NdcStatus status) => status switch
    {
        NdcStatus.Draft => "secondary",
        NdcStatus.Submitted => "info",
        NdcStatus.UnderReview => "primary",
        NdcStatus.Returned => "warning",
        NdcStatus.Approved => "success",
        NdcStatus.Completed => "dark",
        NdcStatus.Rejected => "danger",
        _ => "secondary"
    };

    private static string GetStatusText(NdcStatus status, NdcStage stage)
    {
        if (status == NdcStatus.Submitted && stage == NdcStage.Dealer)
            return "Submitted to Dealer";

        if (status == NdcStatus.UnderReview && stage == NdcStage.TransferDesk)
            return "In Clearance (Transfer Desk)";

        if (status == NdcStatus.Approved && stage == NdcStage.CallForNdc)
            return "Cleared — Awaiting NDC Call";

        return status.ToString();
    }

    private static int GetProgress(NdcStage stage, NdcStatus status)
    {
        if (status == NdcStatus.Completed) return 100;
        if (status == NdcStatus.Rejected) return 0;

        return stage switch
        {
            NdcStage.Owner => 20,
            NdcStage.Dealer => 45,
            NdcStage.TransferDesk => 70,
            NdcStage.MiscDirectorates => 80,
            NdcStage.CallForNdc => 90,
            NdcStage.Execution => 95,
            _ => 20
        };
    }
}
