using Demodha.Models;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.ComponentModel.DataAnnotations;

namespace Demodha.ViewModels;

public class NdcApplicationViewVM
{
    public long Id { get; set; }
    public string AppNo { get; set; } = "";

    public string PlotOrFileNo { get; set; } = "";
    public string Block { get; set; } = "";
    public string SectorOrPhase { get; set; } = "";
    public string SocietyOrScheme { get; set; } = "";

    [Required(ErrorMessage = "Dealer is required.")]
    public string DealerUserId { get; set; } = "";

    public List<SelectListItem> DealerOptions { get; set; } = new();

    public NdcStage CurrentStage { get; set; }
    public NdcStatus CurrentStatus { get; set; }

    public NdcParty Seller { get; set; } = default!;
    public NdcParty? Purchaser { get; set; }

    public List<NdcDocument> Documents { get; set; } = new();
    public List<NdcTask> Tasks { get; set; } = new();
    public List<NdcStatusHistory> Timeline { get; set; } = new();
}
