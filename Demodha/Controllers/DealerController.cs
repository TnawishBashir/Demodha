using System;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Demodha.Data;
using Demodha.Models;
using Demodha.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Demodha.Controllers
{
    [Authorize]
    [Route("Dealer")]
    public class DealerController : Controller
    {
        private readonly ApplicationDbContext _db;

        public DealerController(ApplicationDbContext db)
        {
            _db = db;
        }

        private bool IsDealerUser() => User.FindFirst("userType")?.Value == "2";

        [HttpGet("Inbox")]
        public async Task<IActionResult> Inbox()
        {
            if (!IsDealerUser()) return Forbid();

            var dealerUserId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrWhiteSpace(dealerUserId)) return Challenge();

            var docsQ = _db.NdcDocuments.AsNoTracking();

            var q = _db.NdcApplications
                .AsNoTracking()
                .Where(x => x.IsActive
                            && x.DealerUserId == dealerUserId
                            && x.CurrentStage == NdcStage.Dealer);

            var raw = await q
                .OrderByDescending(x => x.UpdatedOn ?? x.CreatedOn)
                .Take(50)
                .Select(x => new
                {
                    x.Id,
                    x.CreatedOn,
                    x.UpdatedOn,
                    x.PlotOrFileNo,
                    x.Block,
                    x.SectorOrPhase,
                    Status = x.CurrentStatus,

                    OwnerName = _db.NdcParties
                        .Where(p => p.NdcApplicationId == x.Id && p.PartyType == NdcPartyType.Seller)
                        .Select(p => p.FullName)
                        .FirstOrDefault(),

                    HasPurchaser = _db.NdcParties.Any(p => p.NdcApplicationId == x.Id && p.PartyType == NdcPartyType.Purchaser),

                    HasChallan = docsQ.Any(d => d.NdcApplicationId == x.Id && d.DocType == NdcDocumentType.PaymentChallanForm),
                    HasSellerConsent = docsQ.Any(d => d.NdcApplicationId == x.Id && d.DocType == NdcDocumentType.SellerConsentForm),
                    HasPurchaserConsent = docsQ.Any(d => d.NdcApplicationId == x.Id && d.DocType == NdcDocumentType.PurchaserConsentForm),
                })
                .ToListAsync();

            var rows = raw.Select(a =>
            {
                var hasPurchaserConsentOk = !a.HasPurchaser || a.HasPurchaserConsent;

                var progress = 0;
                if (a.HasChallan) progress += 34;
                if (a.HasSellerConsent) progress += 33;
                if (hasPurchaserConsentOk) progress += 33;
                progress = Math.Min(progress, 100);

                var (statusText, badge) = a.Status switch
                {
                    NdcStatus.Submitted => ("New (From Owner)", "info"),
                    NdcStatus.UnderReview => ("Under Review (Dealer)", "primary"),
                    NdcStatus.Returned => ("Returned", "warning"),
                    NdcStatus.Draft => ("Draft", "secondary"),
                    _ => (a.Status.ToString(), "secondary")
                };

                return new DealerCaseRowVM
                {
                    Id = a.Id,
                    CreatedOn = a.CreatedOn,
                    UpdatedOn = a.UpdatedOn,
                    AppNo = $"NDC-{a.CreatedOn:yyyy}-{a.Id:D5}",
                    OwnerName = a.OwnerName ?? "",
                    Plot = $"{a.PlotOrFileNo}, Block {a.Block} ({a.SectorOrPhase})",
                    StatusText = statusText,
                    Badge = badge,
                    Progress = progress,
                    HasPurchaser = a.HasPurchaser,
                    HasChallan = a.HasChallan,
                    HasSellerConsent = a.HasSellerConsent,
                    HasPurchaserConsent = a.HasPurchaserConsent
                };
            }).ToList();

            var vm = new DealerInboxVM
            {
                NewCount = rows.Count(x => x.StatusText.Contains("New")),
                PendingDocsCount = rows.Count(x => !(x.HasChallan && x.HasSellerConsent && (!x.HasPurchaser || x.HasPurchaserConsent))),
                ReadyCount = rows.Count(x => x.HasChallan && x.HasSellerConsent && (!x.HasPurchaser || x.HasPurchaserConsent)),
                Cases = rows
            };

            return View("Inbox", vm);
        }

        [HttpGet("Submitted")]
        public async Task<IActionResult> Submitted()
        {
            if (!IsDealerUser()) return Forbid();

            var dealerUserId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrWhiteSpace(dealerUserId)) return Challenge();

            var docsQ = _db.NdcDocuments.AsNoTracking();

            var q = _db.NdcApplications
                .AsNoTracking()
                .Where(x => x.IsActive
                            && x.DealerUserId == dealerUserId
                            && x.CurrentStage != NdcStage.Dealer);

            var raw = await q
                .OrderByDescending(x => x.UpdatedOn ?? x.CreatedOn)
                .Take(100)
                .Select(x => new
                {
                    x.Id,
                    x.CreatedOn,
                    x.UpdatedOn,
                    x.PlotOrFileNo,
                    x.Block,
                    x.SectorOrPhase,
                    x.CurrentStage,
                    Status = x.CurrentStatus,

                    OwnerName = _db.NdcParties
                        .Where(p => p.NdcApplicationId == x.Id && p.PartyType == NdcPartyType.Seller)
                        .Select(p => p.FullName)
                        .FirstOrDefault(),

                    HasPurchaser = _db.NdcParties.Any(p => p.NdcApplicationId == x.Id && p.PartyType == NdcPartyType.Purchaser),

                    HasChallan = docsQ.Any(d => d.NdcApplicationId == x.Id && d.DocType == NdcDocumentType.PaymentChallanForm),
                    HasSellerConsent = docsQ.Any(d => d.NdcApplicationId == x.Id && d.DocType == NdcDocumentType.SellerConsentForm),
                    HasPurchaserConsent = docsQ.Any(d => d.NdcApplicationId == x.Id && d.DocType == NdcDocumentType.PurchaserConsentForm),
                })
                .ToListAsync();

            var rows = raw.Select(a =>
            {
                var (statusText, badge) = a.CurrentStage switch
                {
                    NdcStage.TransferDesk => ("Submitted to Transfer Desk", "success"),
                    NdcStage.MiscDirectorates => ("In Clearance (Directorates)", "primary"),
                    NdcStage.CallForNdc => ("Cleared — Waiting for Call", "info"),
                    NdcStage.Execution => ("Execution Pending", "warning"),
                    NdcStage.Completed => ("Completed", "dark"),
                    _ => (a.CurrentStage.ToString(), "secondary")
                };

                return new DealerCaseRowVM
                {
                    Id = a.Id,
                    CreatedOn = a.CreatedOn,
                    UpdatedOn = a.UpdatedOn,
                    AppNo = $"NDC-{a.CreatedOn:yyyy}-{a.Id:D5}",
                    OwnerName = a.OwnerName ?? "",
                    Plot = $"{a.PlotOrFileNo}, Block {a.Block} ({a.SectorOrPhase})",
                    StatusText = statusText,
                    Badge = badge,
                    Progress = 100, 
                    HasPurchaser = a.HasPurchaser,
                    HasChallan = a.HasChallan,
                    HasSellerConsent = a.HasSellerConsent,
                    HasPurchaserConsent = a.HasPurchaserConsent
                };
            }).ToList();

            var vm = new DealerSubmittedVM
            {
                SubmittedCount = rows.Count,
                Cases = rows
            };

            return View("Submitted", vm);
        }
        [HttpGet("View/{id:long}")]
        public async Task<IActionResult> View(long id)
        {
            if (!IsDealerUser()) return Forbid();

            var dealerUserId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrWhiteSpace(dealerUserId)) return Challenge();

            var app = await _db.NdcApplications
                .AsNoTracking()
                .Include(x => x.Parties)
                .Include(x => x.Documents)
                .Include(x => x.Tasks).ThenInclude(t => t.TaskDefinition)
                .Include(x => x.StatusHistory)
                .FirstOrDefaultAsync(x =>
                    x.Id == id &&
                    x.IsActive &&
                    x.DealerUserId == dealerUserId); 

            if (app == null) return NotFound();

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

                Documents = app.Documents.OrderBy(d => d.DocType).ToList(),
                Tasks = app.Tasks.OrderBy(t => t.TaskDefinition.SortOrder).ToList(),
                Timeline = app.StatusHistory.OrderByDescending(h => h.ActionOn).ToList()
            };

            return View("~/Views/Dealer/View.cshtml", vm);

        }
    }
}
