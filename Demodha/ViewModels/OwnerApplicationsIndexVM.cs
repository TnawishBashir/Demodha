using Demodha.Models;

namespace Demodha.ViewModels;

public class OwnerApplicationsIndexVM
{
    public List<OwnerApplicationsIndexRowVM> Rows { get; set; } = new();
}

public class OwnerApplicationsIndexRowVM
{
    public long Id { get; set; }
    public string AppNo { get; set; } = "";
    public string Plot { get; set; } = "";
    public string SellerName { get; set; } = "";
    public string Status { get; set; } = "";
    public string Stage { get; set; } = "";
    public string Updated { get; set; } = "";
    public string Badge { get; set; } = "secondary";

    public List<OwnerApplicationsIndexDocVM> Documents { get; set; } = new();
}

public class OwnerApplicationsIndexDocVM
{
    public NdcDocumentType DocType { get; set; }
    public string FileName { get; set; } = "";
    public string? FilePath { get; set; }
}
