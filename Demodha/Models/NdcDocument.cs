using System.ComponentModel.DataAnnotations;

namespace Demodha.Models;

public class NdcDocument
{
    public long Id { get; set; }

    public long NdcApplicationId { get; set; }
    public NdcApplication NdcApplication { get; set; } = default!;

    public NdcDocumentType DocType { get; set; }

    [Required, MaxLength(260)]
    public string FileName { get; set; } = default!;

    [Required, MaxLength(500)]
    public string FilePath { get; set; } = default!;

    [MaxLength(100)]
    public string? ContentType { get; set; }

    public long? FileSizeBytes { get; set; }

    [Required, MaxLength(100)]
    public string UploadedByUserId { get; set; } = default!;
    public DateTime UploadedOn { get; set; } = DateTime.UtcNow;

    public bool? IsVerified { get; set; }
    [MaxLength(100)]
    public string? VerifiedByUserId { get; set; }
    public DateTime? VerifiedOn { get; set; }
}
