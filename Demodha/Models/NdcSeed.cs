namespace Demodha.Models;

public static class NdcSeed
{
    public static readonly NdcTaskDefinition[] TaskDefinitions =
    {
        new() { Id=1, Stage=NdcStage.Owner, Title="Fill NDC Application", SortOrder=1, IsMandatory=true },
        new() { Id=2, Stage=NdcStage.Owner, Title="Attach 2x Photographs", SortOrder=2, IsMandatory=true },
        new() { Id=3, Stage=NdcStage.Owner, Title="Attach Copy of CNIC", SortOrder=3, IsMandatory=true },
        new() { Id=4, Stage=NdcStage.Owner, Title="Attach Application to Secretary DHAG (Seller)", SortOrder=4, IsMandatory=true },
        new() { Id=5, Stage=NdcStage.Owner, Title="Save and Submit to Dealer", SortOrder=5, IsMandatory=true },

        new() { Id=6, Stage=NdcStage.Dealer, Title="Check NDC Application", SortOrder=1, IsMandatory=true },
        new() { Id=7, Stage=NdcStage.Dealer, Title="Payment Challan Form", SortOrder=2, IsMandatory=true },
        new() { Id=8, Stage=NdcStage.Dealer, Title="Attach Seller Consent Form", SortOrder=3, IsMandatory=true },
        new() { Id=9, Stage=NdcStage.Dealer, Title="Attach Purchaser Consent Form", SortOrder=4, IsMandatory=true },
        new() { Id=10, Stage=NdcStage.Dealer, Title="Save and Submit to Transfer Br", SortOrder=5, IsMandatory=true },

        new() { Id=11, Stage=NdcStage.TransferDesk, Title="Check NDC Application Form", SortOrder=1, IsMandatory=true },
        new() { Id=12, Stage=NdcStage.TransferDesk, Title="Check Payment Challan Form", SortOrder=2, IsMandatory=true },
        new() { Id=13, Stage=NdcStage.TransferDesk, Title="Check Seller Consent Form", SortOrder=3, IsMandatory=true },
        new() { Id=14, Stage=NdcStage.TransferDesk, Title="Check Purchaser Consent Form", SortOrder=4, IsMandatory=true },
        new() { Id=15, Stage=NdcStage.TransferDesk, Title="Check e-Stamp Paper verification", SortOrder=5, IsMandatory=true },
        new() { Id=16, Stage=NdcStage.TransferDesk, Title="Send for clearances (Record, Legal, Land, Plans, BC, Fin)", SortOrder=6, IsMandatory=true },

        new() { Id=17, Stage=NdcStage.CallForNdc, Title="Call Owner & Dealer for NDC issuance/handover schedule", SortOrder=1, IsMandatory=true },

        new() { Id=18, Stage=NdcStage.Execution, Title="Check all documents", SortOrder=1, IsMandatory=true },
        new() { Id=19, Stage=NdcStage.Execution, Title="Undergo NADRA ID Card Verisys", SortOrder=2, IsMandatory=true },
        new() { Id=20, Stage=NdcStage.Execution, Title="Dealer provided original documents", SortOrder=3, IsMandatory=true },
        new() { Id=21, Stage=NdcStage.Execution, Title="Seller & purchaser finger/biometric verification (PMS/NADRA)", SortOrder=4, IsMandatory=true },
        new() { Id=22, Stage=NdcStage.Execution, Title="Transfer office recheck all details", SortOrder=5, IsMandatory=true },
        new() { Id=23, Stage=NdcStage.Execution, Title="e-Stamp paper verification", SortOrder=6, IsMandatory=true },
        new() { Id=24, Stage=NdcStage.Execution, Title="Execute Transfer", SortOrder=7, IsMandatory=true },
    };
}
