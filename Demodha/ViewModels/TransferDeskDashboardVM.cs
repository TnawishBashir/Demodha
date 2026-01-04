namespace Demodha.ViewModels;

public class TransferDeskDashboardVM
{
    public int NewFromDealer { get; set; }
    public int PendingEStamp { get; set; }
    public int SentToDirectorates { get; set; }
    public int ReturnedToDealer { get; set; }

    public List<TransferDeskApplicationRowVM> Applications { get; set; } = new();
}

public class TransferDeskApplicationRowVM
{
    public long Id { get; set; }
    public string AppNo { get; set; } = "";
    public string Plot { get; set; } = "";
    public string DealerName { get; set; } = "";
    public string OwnerName { get; set; } = "";
    public string Status { get; set; } = "";
    public string Badge { get; set; } = "secondary";

    public bool HasOwnerDocs { get; set; }
    public bool HasChallan { get; set; }
    public bool HasSellerConsent { get; set; }
    public bool HasPurchaserConsent { get; set; }
    public bool HasEStamp { get; set; }

    public int Progress { get; set; }
}
