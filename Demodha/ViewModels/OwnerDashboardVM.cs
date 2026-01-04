using System;
using System.Collections.Generic;
using Demodha.Models;

namespace Demodha.ViewModels;

public class OwnerDashboardVM
{
    public int Total { get; set; }
    public int Draft { get; set; }
    public int SubmittedToDealer { get; set; }
    public int Returned { get; set; }
    public int ReadyForDealer { get; set; }
    public long? LatestDraftId { get; set; }

    public List<OwnerAppRowVM> Applications { get; set; } = new();
    public List<OwnerDocChecklistVM> DocChecklist { get; set; } = new();
    public List<OwnerActivityVM> Activity { get; set; } = new();
}

public class OwnerAppRowVM
{
    public long Id { get; set; }
    public string AppNo { get; set; } = "";
    public string Plot { get; set; } = "";
    public string StatusText { get; set; } = "";
    public string Badge { get; set; } = "secondary";
    public string Updated { get; set; } = "";
    public int Progress { get; set; }
}

public class OwnerDocChecklistVM
{
    public string Name { get; set; } = "";
    public bool Done { get; set; }
    public string Note { get; set; } = "";
}

public class OwnerActivityVM
{
    public string When { get; set; } = "";
    public string Text { get; set; } = "";
}
