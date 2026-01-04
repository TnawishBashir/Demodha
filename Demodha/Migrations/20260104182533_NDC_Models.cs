using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace Demodha.Migrations
{
    /// <inheritdoc />
    public partial class NDC_Models : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "AspNetRoles",
                columns: table => new
                {
                    Id = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true),
                    NormalizedName = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true),
                    ConcurrencyStamp = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetRoles", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "AspNetUsers",
                columns: table => new
                {
                    Id = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    DisplayName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    UserType = table.Column<int>(type: "int", nullable: true),
                    User_CNIC = table.Column<string>(type: "nvarchar(450)", nullable: true),
                    File_Number = table.Column<string>(type: "nvarchar(450)", nullable: true),
                    ActivationDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    CreatedDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    UserName = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true),
                    NormalizedUserName = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true),
                    Email = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true),
                    NormalizedEmail = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true),
                    EmailConfirmed = table.Column<bool>(type: "bit", nullable: false),
                    PasswordHash = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    SecurityStamp = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ConcurrencyStamp = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    PhoneNumber = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    PhoneNumberConfirmed = table.Column<bool>(type: "bit", nullable: false),
                    TwoFactorEnabled = table.Column<bool>(type: "bit", nullable: false),
                    LockoutEnd = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: true),
                    LockoutEnabled = table.Column<bool>(type: "bit", nullable: false),
                    AccessFailedCount = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetUsers", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Dealers",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(150)", maxLength: 150, nullable: false),
                    Code = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    Address = table.Column<string>(type: "nvarchar(250)", maxLength: 250, nullable: true),
                    Phone = table.Column<string>(type: "nvarchar(30)", maxLength: 30, nullable: true),
                    Email = table.Column<string>(type: "nvarchar(120)", maxLength: 120, nullable: true),
                    IsActive = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Dealers", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "NdcTaskDefinitions",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Stage = table.Column<byte>(type: "tinyint", nullable: false),
                    Title = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    SortOrder = table.Column<int>(type: "int", nullable: false),
                    IsMandatory = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_NdcTaskDefinitions", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "AspNetRoleClaims",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    RoleId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    ClaimType = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ClaimValue = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetRoleClaims", x => x.Id);
                    table.ForeignKey(
                        name: "FK_AspNetRoleClaims_AspNetRoles_RoleId",
                        column: x => x.RoleId,
                        principalTable: "AspNetRoles",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "AspNetUserClaims",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    UserId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    ClaimType = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ClaimValue = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetUserClaims", x => x.Id);
                    table.ForeignKey(
                        name: "FK_AspNetUserClaims_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "AspNetUserLogins",
                columns: table => new
                {
                    LoginProvider = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    ProviderKey = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    ProviderDisplayName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    UserId = table.Column<string>(type: "nvarchar(450)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetUserLogins", x => new { x.LoginProvider, x.ProviderKey });
                    table.ForeignKey(
                        name: "FK_AspNetUserLogins_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "AspNetUserRoles",
                columns: table => new
                {
                    UserId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    RoleId = table.Column<string>(type: "nvarchar(450)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetUserRoles", x => new { x.UserId, x.RoleId });
                    table.ForeignKey(
                        name: "FK_AspNetUserRoles_AspNetRoles_RoleId",
                        column: x => x.RoleId,
                        principalTable: "AspNetRoles",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_AspNetUserRoles_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "AspNetUserTokens",
                columns: table => new
                {
                    UserId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    LoginProvider = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    Value = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetUserTokens", x => new { x.UserId, x.LoginProvider, x.Name });
                    table.ForeignKey(
                        name: "FK_AspNetUserTokens_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "NdcApplications",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    PlotOrFileNo = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    Block = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    SectorOrPhase = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    SocietyOrScheme = table.Column<string>(type: "nvarchar(150)", maxLength: 150, nullable: false),
                    DealerUserId = table.Column<string>(type: "nvarchar(450)", maxLength: 450, nullable: true),
                    CurrentStage = table.Column<byte>(type: "tinyint", nullable: false),
                    CurrentStatus = table.Column<byte>(type: "tinyint", nullable: false),
                    Remarks = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    CreatedByUserId = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    CreatedOn = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedOn = table.Column<DateTime>(type: "datetime2", nullable: true),
                    IsActive = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_NdcApplications", x => x.Id);
                    table.ForeignKey(
                        name: "FK_NdcApplications_AspNetUsers_DealerUserId",
                        column: x => x.DealerUserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "NdcAppointments",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    NdcApplicationId = table.Column<long>(type: "bigint", nullable: false),
                    AppointmentDateTime = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Location = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    Notes = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    NotifiedOwner = table.Column<bool>(type: "bit", nullable: false),
                    NotifiedDealer = table.Column<bool>(type: "bit", nullable: false),
                    CreatedByUserId = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    CreatedOn = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_NdcAppointments", x => x.Id);
                    table.ForeignKey(
                        name: "FK_NdcAppointments_NdcApplications_NdcApplicationId",
                        column: x => x.NdcApplicationId,
                        principalTable: "NdcApplications",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "NdcDocuments",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    NdcApplicationId = table.Column<long>(type: "bigint", nullable: false),
                    DocType = table.Column<byte>(type: "tinyint", nullable: false),
                    FileName = table.Column<string>(type: "nvarchar(260)", maxLength: 260, nullable: false),
                    FilePath = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: false),
                    ContentType = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    FileSizeBytes = table.Column<long>(type: "bigint", nullable: true),
                    UploadedByUserId = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    UploadedOn = table.Column<DateTime>(type: "datetime2", nullable: false),
                    IsVerified = table.Column<bool>(type: "bit", nullable: true),
                    VerifiedByUserId = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    VerifiedOn = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_NdcDocuments", x => x.Id);
                    table.ForeignKey(
                        name: "FK_NdcDocuments_NdcApplications_NdcApplicationId",
                        column: x => x.NdcApplicationId,
                        principalTable: "NdcApplications",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "NdcParties",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    NdcApplicationId = table.Column<long>(type: "bigint", nullable: false),
                    PartyType = table.Column<byte>(type: "tinyint", nullable: false),
                    FullName = table.Column<string>(type: "nvarchar(150)", maxLength: 150, nullable: false),
                    CNIC = table.Column<string>(type: "nvarchar(25)", maxLength: 25, nullable: false),
                    Phone = table.Column<string>(type: "nvarchar(30)", maxLength: 30, nullable: true),
                    Email = table.Column<string>(type: "nvarchar(150)", maxLength: 150, nullable: true),
                    Address = table.Column<string>(type: "nvarchar(300)", maxLength: 300, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_NdcParties", x => x.Id);
                    table.ForeignKey(
                        name: "FK_NdcParties_NdcApplications_NdcApplicationId",
                        column: x => x.NdcApplicationId,
                        principalTable: "NdcApplications",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "NdcStatusHistories",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    NdcApplicationId = table.Column<long>(type: "bigint", nullable: false),
                    FromStage = table.Column<byte>(type: "tinyint", nullable: true),
                    ToStage = table.Column<byte>(type: "tinyint", nullable: false),
                    FromStatus = table.Column<byte>(type: "tinyint", nullable: true),
                    ToStatus = table.Column<byte>(type: "tinyint", nullable: false),
                    ActionByUserId = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    ActionOn = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Remarks = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_NdcStatusHistories", x => x.Id);
                    table.ForeignKey(
                        name: "FK_NdcStatusHistories_NdcApplications_NdcApplicationId",
                        column: x => x.NdcApplicationId,
                        principalTable: "NdcApplications",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "NdcTasks",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    NdcApplicationId = table.Column<long>(type: "bigint", nullable: false),
                    TaskDefinitionId = table.Column<int>(type: "int", nullable: false),
                    IsCompleted = table.Column<bool>(type: "bit", nullable: false),
                    CompletedByUserId = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    CompletedOn = table.Column<DateTime>(type: "datetime2", nullable: true),
                    Notes = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_NdcTasks", x => x.Id);
                    table.ForeignKey(
                        name: "FK_NdcTasks_NdcApplications_NdcApplicationId",
                        column: x => x.NdcApplicationId,
                        principalTable: "NdcApplications",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_NdcTasks_NdcTaskDefinitions_TaskDefinitionId",
                        column: x => x.TaskDefinitionId,
                        principalTable: "NdcTaskDefinitions",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "NdcVerifications",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    NdcApplicationId = table.Column<long>(type: "bigint", nullable: false),
                    NadraCnicVerisysStatus = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: true),
                    NadraCheckedOn = table.Column<DateTime>(type: "datetime2", nullable: true),
                    NadraCheckedByUserId = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    SellerBiometricStatus = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: true),
                    PurchaserBiometricStatus = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: true),
                    PmsFingerVerificationStatus = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: true),
                    EStampVerificationStatus = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: true),
                    EStampRefNo = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    OriginalDocsReceived = table.Column<bool>(type: "bit", nullable: false),
                    TransferOfficeRecheckDone = table.Column<bool>(type: "bit", nullable: false),
                    Remarks = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_NdcVerifications", x => x.Id);
                    table.ForeignKey(
                        name: "FK_NdcVerifications_NdcApplications_NdcApplicationId",
                        column: x => x.NdcApplicationId,
                        principalTable: "NdcApplications",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "NdcClearances",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    NdcApplicationId = table.Column<long>(type: "bigint", nullable: false),
                    Department = table.Column<byte>(type: "tinyint", nullable: false),
                    Status = table.Column<byte>(type: "tinyint", nullable: false),
                    Remarks = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: true),
                    ReviewedByUserId = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    ReviewedOn = table.Column<DateTime>(type: "datetime2", nullable: true),
                    CompletionCertificateDocumentId = table.Column<long>(type: "bigint", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_NdcClearances", x => x.Id);
                    table.ForeignKey(
                        name: "FK_NdcClearances_NdcApplications_NdcApplicationId",
                        column: x => x.NdcApplicationId,
                        principalTable: "NdcApplications",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_NdcClearances_NdcDocuments_CompletionCertificateDocumentId",
                        column: x => x.CompletionCertificateDocumentId,
                        principalTable: "NdcDocuments",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "NdcFinanceCases",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    NdcApplicationId = table.Column<long>(type: "bigint", nullable: false),
                    OutstandingAmount = table.Column<decimal>(type: "decimal(19,2)", nullable: true),
                    GeneratedChallanNo = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    ChallanGeneratedOn = table.Column<DateTime>(type: "datetime2", nullable: true),
                    ChallanDocumentId = table.Column<long>(type: "bigint", nullable: true),
                    PaymentReceived = table.Column<bool>(type: "bit", nullable: false),
                    PaymentReceiptDocumentId = table.Column<long>(type: "bigint", nullable: true),
                    PaymentReceivedOn = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_NdcFinanceCases", x => x.Id);
                    table.ForeignKey(
                        name: "FK_NdcFinanceCases_NdcApplications_NdcApplicationId",
                        column: x => x.NdcApplicationId,
                        principalTable: "NdcApplications",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_NdcFinanceCases_NdcDocuments_ChallanDocumentId",
                        column: x => x.ChallanDocumentId,
                        principalTable: "NdcDocuments",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_NdcFinanceCases_NdcDocuments_PaymentReceiptDocumentId",
                        column: x => x.PaymentReceiptDocumentId,
                        principalTable: "NdcDocuments",
                        principalColumn: "Id");
                });

            migrationBuilder.InsertData(
                table: "NdcTaskDefinitions",
                columns: new[] { "Id", "IsMandatory", "SortOrder", "Stage", "Title" },
                values: new object[,]
                {
                    { 1, true, 1, (byte)1, "Fill NDC Application" },
                    { 2, true, 2, (byte)1, "Attach 2x Photographs" },
                    { 3, true, 3, (byte)1, "Attach Copy of CNIC" },
                    { 4, true, 4, (byte)1, "Attach Application to Secretary DHAG (Seller)" },
                    { 5, true, 5, (byte)1, "Save and Submit to Dealer" },
                    { 6, true, 1, (byte)2, "Check NDC Application" },
                    { 7, true, 2, (byte)2, "Payment Challan Form" },
                    { 8, true, 3, (byte)2, "Attach Seller Consent Form" },
                    { 9, true, 4, (byte)2, "Attach Purchaser Consent Form" },
                    { 10, true, 5, (byte)2, "Save and Submit to Transfer Br" },
                    { 11, true, 1, (byte)3, "Check NDC Application Form" },
                    { 12, true, 2, (byte)3, "Check Payment Challan Form" },
                    { 13, true, 3, (byte)3, "Check Seller Consent Form" },
                    { 14, true, 4, (byte)3, "Check Purchaser Consent Form" },
                    { 15, true, 5, (byte)3, "Check e-Stamp Paper verification" },
                    { 16, true, 6, (byte)3, "Send for clearances (Record, Legal, Land, Plans, BC, Fin)" },
                    { 17, true, 1, (byte)5, "Call Owner & Dealer for NDC issuance/handover schedule" },
                    { 18, true, 1, (byte)6, "Check all documents" },
                    { 19, true, 2, (byte)6, "Undergo NADRA ID Card Verisys" },
                    { 20, true, 3, (byte)6, "Dealer provided original documents" },
                    { 21, true, 4, (byte)6, "Seller & purchaser finger/biometric verification (PMS/NADRA)" },
                    { 22, true, 5, (byte)6, "Transfer office recheck all details" },
                    { 23, true, 6, (byte)6, "e-Stamp paper verification" },
                    { 24, true, 7, (byte)6, "Execute Transfer" }
                });

            migrationBuilder.CreateIndex(
                name: "IX_AspNetRoleClaims_RoleId",
                table: "AspNetRoleClaims",
                column: "RoleId");

            migrationBuilder.CreateIndex(
                name: "RoleNameIndex",
                table: "AspNetRoles",
                column: "NormalizedName",
                unique: true,
                filter: "[NormalizedName] IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_AspNetUserClaims_UserId",
                table: "AspNetUserClaims",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_AspNetUserLogins_UserId",
                table: "AspNetUserLogins",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_AspNetUserRoles_RoleId",
                table: "AspNetUserRoles",
                column: "RoleId");

            migrationBuilder.CreateIndex(
                name: "EmailIndex",
                table: "AspNetUsers",
                column: "NormalizedEmail");

            migrationBuilder.CreateIndex(
                name: "IX_AspNetUsers_User_CNIC_File_Number",
                table: "AspNetUsers",
                columns: new[] { "User_CNIC", "File_Number" },
                unique: true,
                filter: "[User_CNIC] IS NOT NULL AND [File_Number] IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "UserNameIndex",
                table: "AspNetUsers",
                column: "NormalizedUserName",
                unique: true,
                filter: "[NormalizedUserName] IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_Dealers_Name",
                table: "Dealers",
                column: "Name");

            migrationBuilder.CreateIndex(
                name: "IX_NdcApplications_DealerUserId",
                table: "NdcApplications",
                column: "DealerUserId");

            migrationBuilder.CreateIndex(
                name: "IX_NdcApplications_PlotOrFileNo_Block_SectorOrPhase_SocietyOrScheme",
                table: "NdcApplications",
                columns: new[] { "PlotOrFileNo", "Block", "SectorOrPhase", "SocietyOrScheme" });

            migrationBuilder.CreateIndex(
                name: "IX_NdcAppointments_NdcApplicationId",
                table: "NdcAppointments",
                column: "NdcApplicationId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_NdcClearances_CompletionCertificateDocumentId",
                table: "NdcClearances",
                column: "CompletionCertificateDocumentId");

            migrationBuilder.CreateIndex(
                name: "IX_NdcClearances_NdcApplicationId_Department",
                table: "NdcClearances",
                columns: new[] { "NdcApplicationId", "Department" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_NdcDocuments_NdcApplicationId_DocType",
                table: "NdcDocuments",
                columns: new[] { "NdcApplicationId", "DocType" });

            migrationBuilder.CreateIndex(
                name: "IX_NdcFinanceCases_ChallanDocumentId",
                table: "NdcFinanceCases",
                column: "ChallanDocumentId");

            migrationBuilder.CreateIndex(
                name: "IX_NdcFinanceCases_NdcApplicationId",
                table: "NdcFinanceCases",
                column: "NdcApplicationId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_NdcFinanceCases_PaymentReceiptDocumentId",
                table: "NdcFinanceCases",
                column: "PaymentReceiptDocumentId");

            migrationBuilder.CreateIndex(
                name: "IX_NdcParties_NdcApplicationId_PartyType",
                table: "NdcParties",
                columns: new[] { "NdcApplicationId", "PartyType" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_NdcStatusHistories_NdcApplicationId_ActionOn",
                table: "NdcStatusHistories",
                columns: new[] { "NdcApplicationId", "ActionOn" });

            migrationBuilder.CreateIndex(
                name: "IX_NdcTaskDefinitions_Stage_SortOrder",
                table: "NdcTaskDefinitions",
                columns: new[] { "Stage", "SortOrder" });

            migrationBuilder.CreateIndex(
                name: "IX_NdcTasks_NdcApplicationId_TaskDefinitionId",
                table: "NdcTasks",
                columns: new[] { "NdcApplicationId", "TaskDefinitionId" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_NdcTasks_TaskDefinitionId",
                table: "NdcTasks",
                column: "TaskDefinitionId");

            migrationBuilder.CreateIndex(
                name: "IX_NdcVerifications_NdcApplicationId",
                table: "NdcVerifications",
                column: "NdcApplicationId",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "AspNetRoleClaims");

            migrationBuilder.DropTable(
                name: "AspNetUserClaims");

            migrationBuilder.DropTable(
                name: "AspNetUserLogins");

            migrationBuilder.DropTable(
                name: "AspNetUserRoles");

            migrationBuilder.DropTable(
                name: "AspNetUserTokens");

            migrationBuilder.DropTable(
                name: "Dealers");

            migrationBuilder.DropTable(
                name: "NdcAppointments");

            migrationBuilder.DropTable(
                name: "NdcClearances");

            migrationBuilder.DropTable(
                name: "NdcFinanceCases");

            migrationBuilder.DropTable(
                name: "NdcParties");

            migrationBuilder.DropTable(
                name: "NdcStatusHistories");

            migrationBuilder.DropTable(
                name: "NdcTasks");

            migrationBuilder.DropTable(
                name: "NdcVerifications");

            migrationBuilder.DropTable(
                name: "AspNetRoles");

            migrationBuilder.DropTable(
                name: "NdcDocuments");

            migrationBuilder.DropTable(
                name: "NdcTaskDefinitions");

            migrationBuilder.DropTable(
                name: "NdcApplications");

            migrationBuilder.DropTable(
                name: "AspNetUsers");
        }
    }
}
