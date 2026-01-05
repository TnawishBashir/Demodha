using System;
using System.Collections.Generic;

namespace Demodha.ViewModels
{
    public class DealerInboxVM
    {
        public int NewCount { get; set; }
        public int PendingDocsCount { get; set; }
        public int ReadyCount { get; set; }

        public List<DealerCaseRowVM> Cases { get; set; } = new();
    }

    public class DealerSubmittedVM
    {
        public int SubmittedCount { get; set; }
        public List<DealerCaseRowVM> Cases { get; set; } = new();
    }

    public class DealerCaseRowVM
    {
        public long Id { get; set; }
        public string AppNo { get; set; } = "";
        public string OwnerName { get; set; } = "";
        public string Plot { get; set; } = "";

        public string StatusText { get; set; } = "";
        public string Badge { get; set; } = "secondary";
        public int Progress { get; set; }

        public DateTime CreatedOn { get; set; }
        public DateTime? UpdatedOn { get; set; }

        public bool HasChallan { get; set; }
        public bool HasSellerConsent { get; set; }
        public bool HasPurchaserConsent { get; set; }
        public bool HasPurchaser { get; set; }
    }
   

    public class DealerSubmittedRowVM
    {
        public long Id { get; set; }
        public string AppNo { get; set; } = "";
        public string OwnerName { get; set; } = "";
        public string Plot { get; set; } = "";
        public string Updated { get; set; } = "";

        public string Stage { get; set; } = "";
        public string Status { get; set; } = "";
        public string Badge { get; set; } = "secondary";

        public bool HasChallan { get; set; }
        public bool HasSellerConsent { get; set; }
        public bool HasPurchaserConsent { get; set; }
    }
    public class DealerOpenVM
    {
        public long Id { get; set; }
        public string AppNo { get; set; } = "";
        public string PlotOrFileNo { get; set; } = "";
        public string Block { get; set; } = "";
        public string SectorOrPhase { get; set; } = "";

        public string SellerName { get; set; } = "";
        public string SellerCnic { get; set; } = "";
        public string SellerPhone { get; set; } = "";

        public string? PurchaserName { get; set; }
        public string? PurchaserCnic { get; set; }

        public string CurrentStatusText { get; set; } = "";
        public string CurrentBadge { get; set; } = "secondary";

        public bool HasChallan { get; set; }
        public bool HasSellerConsent { get; set; }
        public bool HasPurchaserConsent { get; set; }
        public bool HasPurchaser { get; set; }

        public bool CanSubmitToTransferDesk =>
            HasChallan && HasSellerConsent && (!HasPurchaser || HasPurchaserConsent);
    }
}
