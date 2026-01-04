using System.ComponentModel.DataAnnotations;

namespace Demodha.Models;

public class NdcClearance
{
    public long Id { get; set; }

    public long NdcApplicationId { get; set; }
    public NdcApplication NdcApplication { get; set; } = default!;

    public NdcDepartment Department { get; set; }
    public NdcReviewStatus Status { get; set; } = NdcReviewStatus.Pending;

    [MaxLength(1000)]
    public string? Remarks { get; set; }

    [MaxLength(100)]
    public string? ReviewedByUserId { get; set; }
    public DateTime? ReviewedOn { get; set; }

    public long? CompletionCertificateDocumentId { get; set; }
    public NdcDocument? CompletionCertificateDocument { get; set; }
}
