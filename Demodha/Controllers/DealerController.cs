//using System.Security.Claims;
//using Demodha.Data;
//using Demodha.Models;
//using Demodha.ViewModels;
//using Microsoft.AspNetCore.Authorization;
//using Microsoft.AspNetCore.Mvc;
//using Microsoft.EntityFrameworkCore;

//namespace Demodha.Controllers;

//[Authorize]
//[Route("Dealer")]
//public class DealerController : Controller
//{
//    private readonly ApplicationDbContext _db;

//    public DealerController(ApplicationDbContext db)
//    {
//        _db = db;
//    }

//    [HttpGet("Dashboard")]
//    public async Task<IActionResult> Dashboard()
//    {
//        var dealerId = GetDealerIdFromClaims();
//        if (dealerId == 0)
//            return Forbid();

//        var q = _db.NdcApplications
//            .AsNoTracking()
//            .Where(x => x.DealerUserId == dealerId && x.IsActive);

//        var vm = new DealerDashboardVM
//        {
//            PendingFromOwner = await q.CountAsync(x =>
//                x.CurrentStage == NdcStage.Dealer &&
//                x.CurrentStatus == NdcStatus.Submitted),

//            Draft = await q.CountAsync(x =>
//                x.CurrentStage == NdcStage.Dealer &&
//                x.CurrentStatus == NdcStatus.Draft),

//            SubmittedToTransfer = await q.CountAsync(x =>
//                x.CurrentStage == NdcStage.TransferDesk),

//            Returned = await q.CountAsync(x =>
//                x.CurrentStatus == NdcStatus.Returned),

//            Applications = await q
//                .OrderByDescending(x => x.UpdatedOn ?? x.CreatedOn)
//                .Select(x => new DealerApplicationRowVM
//                {
//                    Id = x.Id,
//                    AppNo = $"NDC-{x.CreatedOn:yyyy}-{x.Id:D5}",
//                    Plot = $"{x.PlotOrFileNo} Block {x.Block}",
//                    OwnerName = _db.NdcParties
//                        .Where(p => p.NdcApplicationId == x.Id && p.PartyType == NdcPartyType.Seller)
//                        .Select(p => p.FullName)
//                        .FirstOrDefault() ?? "",
//                    Status = x.CurrentStatus.ToString()
//                })
//                .ToListAsync()
//        };

//        return View("Dashboard", vm);
//    }

//    private int GetDealerIdFromClaims()
//    {
//        var claim = User.FindFirst("dealerId")?.Value;
//        return int.TryParse(claim, out var id) ? id : 0;
//    }
//}
