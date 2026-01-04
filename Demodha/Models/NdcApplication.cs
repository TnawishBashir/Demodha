using Demodha.Data.Identity;
using System.ComponentModel.DataAnnotations;

namespace Demodha.Models;

public class NdcApplication
{
    public long Id { get; set; }

    [Required, MaxLength(50)]
    public string PlotOrFileNo { get; set; } = default!;

    [Required, MaxLength(50)]
    public string Block { get; set; } = default!;

    [Required, MaxLength(50)]
    public string SectorOrPhase { get; set; } = default!;

    [Required, MaxLength(150)]
    public string SocietyOrScheme { get; set; } = default!;

    [MaxLength(450)]
    public string? DealerUserId { get; set; }  

    public ApplicationUser? DealerUser { get; set; }

    public NdcStage CurrentStage { get; set; } = NdcStage.Owner;
    public NdcStatus CurrentStatus { get; set; } = NdcStatus.Draft;

    [MaxLength(500)]
    public string? Remarks { get; set; }

    [Required, MaxLength(100)]
    public string CreatedByUserId { get; set; } = default!;
    public DateTime CreatedOn { get; set; } = DateTime.UtcNow;
    public DateTime? UpdatedOn { get; set; }
    public bool IsActive { get; set; } = true;

    public ICollection<NdcParty> Parties { get; set; } = new List<NdcParty>();
    public ICollection<NdcDocument> Documents { get; set; } = new List<NdcDocument>();
    public ICollection<NdcTask> Tasks { get; set; } = new List<NdcTask>();
    public ICollection<NdcClearance> Clearances { get; set; } = new List<NdcClearance>();
    public ICollection<NdcStatusHistory> StatusHistory { get; set; } = new List<NdcStatusHistory>();

    public NdcFinanceCase? FinanceCase { get; set; }
    public NdcAppointment? Appointment { get; set; }
    public NdcVerification? Verification { get; set; }
}
