using System.ComponentModel.DataAnnotations;

namespace Demodha.Models;

public class NdcVerification
{
    public long Id { get; set; }

    public long NdcApplicationId { get; set; }
    public NdcApplication NdcApplication { get; set; } = default!;

    [MaxLength(20)]
    public string? NadraCnicVerisysStatus { get; set; } 
    public DateTime? NadraCheckedOn { get; set; }
    [MaxLength(100)]
    public string? NadraCheckedByUserId { get; set; }

    [MaxLength(20)]
    public string? SellerBiometricStatus { get; set; } 
    [MaxLength(20)]
    public string? PurchaserBiometricStatus { get; set; }

    [MaxLength(20)]
    public string? PmsFingerVerificationStatus { get; set; }

    [MaxLength(20)]
    public string? EStampVerificationStatus { get; set; } 
    [MaxLength(100)]
    public string? EStampRefNo { get; set; }

    public bool OriginalDocsReceived { get; set; } = false;
    public bool TransferOfficeRecheckDone { get; set; } = false;

    [MaxLength(1000)]
    public string? Remarks { get; set; }
}
