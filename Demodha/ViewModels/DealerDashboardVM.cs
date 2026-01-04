using Demodha.Models;
using System.ComponentModel.DataAnnotations;

namespace Demodha.ViewModels;

public class DealerDashboardVM
{
    public int PendingFromOwner { get; set; }
    public int Draft { get; set; }
    public int SubmittedToTransfer { get; set; }
    public int Returned { get; set; }

    public List<DealerApplicationRowVM> Applications { get; set; } = new();
}

public class DealerApplicationRowVM
{
    public long Id { get; set; }
    public string AppNo { get; set; } = "";
    public string Plot { get; set; } = "";
    public string OwnerName { get; set; } = "";

    public string Status { get; set; } = "";
    public string Badge { get; set; } = "secondary";

    // Dealer Step-2 Docs
    public bool HasChallan { get; set; }
    public bool HasSellerConsent { get; set; }
    public bool HasPurchaserConsent { get; set; }

    public int Progress { get; set; }
}
public class DealerApplicationStep2VM
{
    public long ApplicationId { get; set; }
    public string AppNo { get; set; } = "";
    public string Plot { get; set; } = "";
    public string OwnerName { get; set; } = "";
    public bool HasPurchaser { get; set; }

    // Existing docs flags (to show status)
    public bool HasChallan { get; set; }
    public bool HasSellerConsent { get; set; }
    public bool HasPurchaserConsent { get; set; }

    // Uploads
    [Display(Name = "Payment Challan Form")]
    public IFormFile? PaymentChallan { get; set; }

    [Display(Name = "Seller Consent Form")]
    public IFormFile? SellerConsent { get; set; }

    [Display(Name = "Purchaser Consent Form")]
    public IFormFile? PurchaserConsent { get; set; }
}