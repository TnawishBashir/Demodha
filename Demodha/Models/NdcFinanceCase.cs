using System.ComponentModel.DataAnnotations;

namespace Demodha.Models;

public class NdcFinanceCase
{
    public long Id { get; set; }

    public long NdcApplicationId { get; set; }
    public NdcApplication NdcApplication { get; set; } = default!;

    public decimal? OutstandingAmount { get; set; }

    [MaxLength(50)]
    public string? GeneratedChallanNo { get; set; }
    public DateTime? ChallanGeneratedOn { get; set; }

    public long? ChallanDocumentId { get; set; }
    public NdcDocument? ChallanDocument { get; set; }

    public bool PaymentReceived { get; set; } = false;

    public long? PaymentReceiptDocumentId { get; set; }
    public NdcDocument? PaymentReceiptDocument { get; set; }

    public DateTime? PaymentReceivedOn { get; set; }
}
