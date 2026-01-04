namespace Demodha.Models;

public enum NdcStage : byte
{
    Owner = 1,
    Dealer = 2,
    TransferDesk = 3,
    MiscDirectorates = 4,
    CallForNdc = 5,
    Execution = 6,
    Completed = 7,
    Returned = 8,
    Rejected = 9
}

public enum NdcStatus : byte
{
    Draft = 1,
    Submitted = 2,
    UnderReview = 3,
    Returned = 4,
    Approved = 5,
    Completed = 6,
    Rejected = 7
}

public enum NdcPartyType : byte
{
    Seller = 1,
    Purchaser = 2
}

public enum NdcDocumentType : byte
{
    SellerCnicCopy = 1,
    Photo1 = 2,
    Photo2 = 3,
    ApplicationToSecretary = 4,

    PaymentChallanForm = 5,
    SellerConsentForm = 6,
    PurchaserConsentForm = 7,

    PaymentReceipt = 8,

    CompletionCertificate = 9,
    Other = 10,
        EStampPaper = 20
}

public enum NdcDepartment : byte
{
    Plans = 1,
    Legal = 2,
    Land = 3,
    BuildingControl = 4,
    Finance = 5,
    RecordBranch = 6,
    Record = 7,
    BC = 8
}

public enum NdcReviewStatus : byte
{
    Pending = 1,
    Approved = 2,
    Returned = 3,
    Rejected = 4
}
