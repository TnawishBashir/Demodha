using System.ComponentModel.DataAnnotations;

namespace Demodha.Models;

public class NdcStatusHistory
{
    public long Id { get; set; }

    public long NdcApplicationId { get; set; }
    public NdcApplication NdcApplication { get; set; } = default!;

    public NdcStage? FromStage { get; set; }
    public NdcStage ToStage { get; set; }

    public NdcStatus? FromStatus { get; set; }
    public NdcStatus ToStatus { get; set; }

    [Required, MaxLength(100)]
    public string ActionByUserId { get; set; } = default!;
    public DateTime ActionOn { get; set; } = DateTime.UtcNow;

    [MaxLength(500)]
    public string? Remarks { get; set; }
}
