using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace FacilityService.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class InitialFacilityPanel : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.EnsureSchema(
                name: "panel");

            migrationBuilder.CreateTable(
                name: "Files",
                schema: "panel",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    FileName = table.Column<string>(type: "nvarchar(260)", maxLength: 260, nullable: false),
                    ContentType = table.Column<string>(type: "nvarchar(150)", maxLength: 150, nullable: false),
                    Size = table.Column<long>(type: "bigint", nullable: false),
                    StorageKey = table.Column<string>(type: "nvarchar(300)", maxLength: 300, nullable: false),
                    UploadedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UploadedBy = table.Column<string>(type: "nvarchar(150)", maxLength: 150, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Files", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Reasons",
                schema: "panel",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Type = table.Column<byte>(type: "tinyint", nullable: false),
                    Title = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    IsActive = table.Column<bool>(type: "bit", nullable: false),
                    SortOrder = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Reasons", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Schemes",
                schema: "panel",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Code = table.Column<string>(type: "nvarchar(30)", maxLength: 30, nullable: false),
                    Title = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    IsActive = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Schemes", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Companies",
                schema: "panel",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    SchemeId = table.Column<int>(type: "int", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    NationalId = table.Column<string>(type: "varchar(11)", unicode: false, maxLength: 11, nullable: false),
                    AgreementNumber = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    AccountNumber = table.Column<string>(type: "varchar(24)", unicode: false, maxLength: 24, nullable: false),
                    ContractDate = table.Column<DateOnly>(type: "date", nullable: false),
                    AccountOpeningDate = table.Column<DateOnly>(type: "date", nullable: false),
                    SalaryFromSepah = table.Column<bool>(type: "bit", nullable: false),
                    BranchCode = table.Column<string>(type: "varchar(50)", unicode: false, maxLength: 50, nullable: false),
                    Status = table.Column<byte>(type: "tinyint", nullable: false),
                    Registrar = table.Column<string>(type: "nvarchar(400)", maxLength: 400, nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    AgreementFileId = table.Column<long>(type: "bigint", nullable: true),
                    DeletedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    DeleteDescription = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    BalancesVersion = table.Column<int>(type: "int", nullable: false),
                    Version = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Companies", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Companies_Files_AgreementFileId",
                        column: x => x.AgreementFileId,
                        principalSchema: "panel",
                        principalTable: "Files",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Companies_Schemes_SchemeId",
                        column: x => x.SchemeId,
                        principalSchema: "panel",
                        principalTable: "Schemes",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "SchemeSettings",
                schema: "panel",
                columns: table => new
                {
                    SchemeId = table.Column<int>(type: "int", nullable: false),
                    ContractTypes = table.Column<string>(type: "varchar(50)", unicode: false, maxLength: 50, nullable: false),
                    QardHasanRate = table.Column<decimal>(type: "decimal(5,2)", precision: 5, scale: 2, nullable: false),
                    MurabahaRates = table.Column<string>(type: "varchar(200)", unicode: false, maxLength: 200, nullable: false),
                    RepaymentTermsMonths = table.Column<string>(type: "varchar(200)", unicode: false, maxLength: 200, nullable: false),
                    SalaryFromSepahMultiplierYes = table.Column<decimal>(type: "decimal(5,2)", precision: 5, scale: 2, nullable: false),
                    SalaryFromSepahMultiplierNo = table.Column<decimal>(type: "decimal(5,2)", precision: 5, scale: 2, nullable: false),
                    ContractDurationMonths = table.Column<int>(type: "int", nullable: false),
                    GraceMonthsAfterExpiry = table.Column<int>(type: "int", nullable: false),
                    MaxPlansPerCompany = table.Column<int>(type: "int", nullable: false),
                    PersonnelIntroductionDelayMonths = table.Column<int>(type: "int", nullable: false),
                    WaitingPeriodDays = table.Column<int>(type: "int", nullable: false),
                    DigitalApprovalEnabled = table.Column<bool>(type: "bit", nullable: false),
                    Version = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SchemeSettings", x => x.SchemeId);
                    table.ForeignKey(
                        name: "FK_SchemeSettings_Schemes_SchemeId",
                        column: x => x.SchemeId,
                        principalSchema: "panel",
                        principalTable: "Schemes",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "BalanceDocuments",
                schema: "panel",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    CompanyId = table.Column<long>(type: "bigint", nullable: false),
                    Month = table.Column<string>(type: "varchar(7)", unicode: false, maxLength: 7, nullable: true),
                    DocumentTypeId = table.Column<int>(type: "int", nullable: false),
                    Description = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    FileId = table.Column<long>(type: "bigint", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_BalanceDocuments", x => x.Id);
                    table.ForeignKey(
                        name: "FK_BalanceDocuments_Companies_CompanyId",
                        column: x => x.CompanyId,
                        principalSchema: "panel",
                        principalTable: "Companies",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_BalanceDocuments_Files_FileId",
                        column: x => x.FileId,
                        principalSchema: "panel",
                        principalTable: "Files",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_BalanceDocuments_Reasons_DocumentTypeId",
                        column: x => x.DocumentTypeId,
                        principalSchema: "panel",
                        principalTable: "Reasons",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "ChangeLogs",
                schema: "panel",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    At = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UserId = table.Column<int>(type: "int", nullable: false),
                    User = table.Column<string>(type: "nvarchar(400)", maxLength: 400, nullable: false),
                    Category = table.Column<byte>(type: "tinyint", nullable: false),
                    Action = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Details = table.Column<string>(type: "nvarchar(2000)", maxLength: 2000, nullable: false),
                    Before = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    After = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CompanyId = table.Column<long>(type: "bigint", nullable: true),
                    BranchCode = table.Column<string>(type: "varchar(50)", unicode: false, maxLength: 50, nullable: true),
                    RestoreKind = table.Column<string>(type: "varchar(50)", unicode: false, maxLength: 50, nullable: true),
                    RestorePayload = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    RestoredAt = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ChangeLogs", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ChangeLogs_Companies_CompanyId",
                        column: x => x.CompanyId,
                        principalSchema: "panel",
                        principalTable: "Companies",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "CompanyCancellations",
                schema: "panel",
                columns: table => new
                {
                    CompanyId = table.Column<long>(type: "bigint", nullable: false),
                    CancelDate = table.Column<DateOnly>(type: "date", nullable: false),
                    ReasonId = table.Column<int>(type: "int", nullable: false),
                    ReasonTitle = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Description = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: false),
                    LetterFileId = table.Column<long>(type: "bigint", nullable: false),
                    CancelledBy = table.Column<string>(type: "nvarchar(400)", maxLength: 400, nullable: false),
                    CancelledAt = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CompanyCancellations", x => x.CompanyId);
                    table.ForeignKey(
                        name: "FK_CompanyCancellations_Companies_CompanyId",
                        column: x => x.CompanyId,
                        principalSchema: "panel",
                        principalTable: "Companies",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_CompanyCancellations_Files_LetterFileId",
                        column: x => x.LetterFileId,
                        principalSchema: "panel",
                        principalTable: "Files",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_CompanyCancellations_Reasons_ReasonId",
                        column: x => x.ReasonId,
                        principalSchema: "panel",
                        principalTable: "Reasons",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "MonthlyBalances",
                schema: "panel",
                columns: table => new
                {
                    CompanyId = table.Column<long>(type: "bigint", nullable: false),
                    Month = table.Column<string>(type: "varchar(7)", unicode: false, maxLength: 7, nullable: false),
                    Amount = table.Column<long>(type: "bigint", nullable: false),
                    EditedBy = table.Column<string>(type: "nvarchar(400)", maxLength: 400, nullable: false),
                    EditedAt = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MonthlyBalances", x => new { x.CompanyId, x.Month });
                    table.ForeignKey(
                        name: "FK_MonthlyBalances_Companies_CompanyId",
                        column: x => x.CompanyId,
                        principalSchema: "panel",
                        principalTable: "Companies",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "PersonnelImports",
                schema: "panel",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    CompanyId = table.Column<long>(type: "bigint", nullable: false),
                    FileName = table.Column<string>(type: "nvarchar(260)", maxLength: 260, nullable: false),
                    TotalRows = table.Column<int>(type: "int", nullable: false),
                    Accepted = table.Column<int>(type: "int", nullable: false),
                    Rejected = table.Column<int>(type: "int", nullable: false),
                    ErrorsJson = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Registrar = table.Column<string>(type: "nvarchar(400)", maxLength: 400, nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PersonnelImports", x => x.Id);
                    table.ForeignKey(
                        name: "FK_PersonnelImports_Companies_CompanyId",
                        column: x => x.CompanyId,
                        principalSchema: "panel",
                        principalTable: "Companies",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "SchemePlans",
                schema: "panel",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    SchemeId = table.Column<int>(type: "int", nullable: false),
                    ContractType = table.Column<byte>(type: "tinyint", nullable: false),
                    Rate = table.Column<decimal>(type: "decimal(5,2)", precision: 5, scale: 2, nullable: false),
                    TermMonths = table.Column<int>(type: "int", nullable: false),
                    UsablePercent = table.Column<decimal>(type: "decimal(5,2)", precision: 5, scale: 2, nullable: false),
                    IsActive = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SchemePlans", x => x.Id);
                    table.ForeignKey(
                        name: "FK_SchemePlans_SchemeSettings_SchemeId",
                        column: x => x.SchemeId,
                        principalSchema: "panel",
                        principalTable: "SchemeSettings",
                        principalColumn: "SchemeId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Personnel",
                schema: "panel",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    CompanyId = table.Column<long>(type: "bigint", nullable: false),
                    Priority = table.Column<int>(type: "int", nullable: false),
                    FullName = table.Column<string>(type: "nvarchar(60)", maxLength: 60, nullable: false),
                    NationalCode = table.Column<string>(type: "varchar(10)", unicode: false, maxLength: 10, nullable: false),
                    Mobile = table.Column<string>(type: "varchar(11)", unicode: false, maxLength: 11, nullable: false),
                    PersonnelCode = table.Column<string>(type: "varchar(20)", unicode: false, maxLength: 20, nullable: false),
                    HireDate = table.Column<DateOnly>(type: "date", nullable: false),
                    Status = table.Column<byte>(type: "tinyint", nullable: false),
                    RejectReason = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    DecidedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    Registrar = table.Column<string>(type: "nvarchar(400)", maxLength: 400, nullable: false),
                    ImportBatchId = table.Column<long>(type: "bigint", nullable: true),
                    DeleteDescription = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    DeleteFileId = table.Column<long>(type: "bigint", nullable: true),
                    Version = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Personnel", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Personnel_Companies_CompanyId",
                        column: x => x.CompanyId,
                        principalSchema: "panel",
                        principalTable: "Companies",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Personnel_Files_DeleteFileId",
                        column: x => x.DeleteFileId,
                        principalSchema: "panel",
                        principalTable: "Files",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Personnel_PersonnelImports_ImportBatchId",
                        column: x => x.ImportBatchId,
                        principalSchema: "panel",
                        principalTable: "PersonnelImports",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "CompanyPlans",
                schema: "panel",
                columns: table => new
                {
                    CompanyId = table.Column<long>(type: "bigint", nullable: false),
                    PlanId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CompanyPlans", x => new { x.CompanyId, x.PlanId });
                    table.ForeignKey(
                        name: "FK_CompanyPlans_Companies_CompanyId",
                        column: x => x.CompanyId,
                        principalSchema: "panel",
                        principalTable: "Companies",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_CompanyPlans_SchemePlans_PlanId",
                        column: x => x.PlanId,
                        principalSchema: "panel",
                        principalTable: "SchemePlans",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "LoanBatches",
                schema: "panel",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    CompanyId = table.Column<long>(type: "bigint", nullable: false),
                    PlanId = table.Column<int>(type: "int", nullable: false),
                    Mode = table.Column<byte>(type: "tinyint", nullable: false),
                    BaseDate = table.Column<DateOnly>(type: "date", nullable: false),
                    Description = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: true),
                    Registrar = table.Column<string>(type: "nvarchar(400)", maxLength: 400, nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    BranchCode = table.Column<string>(type: "varchar(50)", unicode: false, maxLength: 50, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_LoanBatches", x => x.Id);
                    table.ForeignKey(
                        name: "FK_LoanBatches_Companies_CompanyId",
                        column: x => x.CompanyId,
                        principalSchema: "panel",
                        principalTable: "Companies",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_LoanBatches_SchemePlans_PlanId",
                        column: x => x.PlanId,
                        principalSchema: "panel",
                        principalTable: "SchemePlans",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "PersonnelHistory",
                schema: "panel",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    PersonnelId = table.Column<long>(type: "bigint", nullable: false),
                    From = table.Column<byte>(type: "tinyint", nullable: true),
                    To = table.Column<byte>(type: "tinyint", nullable: false),
                    Reason = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    Description = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: true),
                    By = table.Column<string>(type: "nvarchar(400)", maxLength: 400, nullable: false),
                    At = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PersonnelHistory", x => x.Id);
                    table.ForeignKey(
                        name: "FK_PersonnelHistory_Personnel_PersonnelId",
                        column: x => x.PersonnelId,
                        principalSchema: "panel",
                        principalTable: "Personnel",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "LoanRequests",
                schema: "panel",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    BatchId = table.Column<long>(type: "bigint", nullable: false),
                    CompanyId = table.Column<long>(type: "bigint", nullable: false),
                    BranchCode = table.Column<string>(type: "varchar(50)", unicode: false, maxLength: 50, nullable: false),
                    PersonnelId = table.Column<long>(type: "bigint", nullable: false),
                    PlanId = table.Column<int>(type: "int", nullable: false),
                    Amount = table.Column<long>(type: "bigint", nullable: false),
                    PaymentOrder = table.Column<int>(type: "int", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    DecidedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    Status = table.Column<byte>(type: "tinyint", nullable: false),
                    PostStage = table.Column<byte>(type: "tinyint", nullable: true),
                    Description = table.Column<string>(type: "nvarchar(1200)", maxLength: 1200, nullable: true),
                    Registrar = table.Column<string>(type: "nvarchar(400)", maxLength: 400, nullable: false),
                    Version = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_LoanRequests", x => x.Id);
                    table.ForeignKey(
                        name: "FK_LoanRequests_Companies_CompanyId",
                        column: x => x.CompanyId,
                        principalSchema: "panel",
                        principalTable: "Companies",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_LoanRequests_LoanBatches_BatchId",
                        column: x => x.BatchId,
                        principalSchema: "panel",
                        principalTable: "LoanBatches",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_LoanRequests_Personnel_PersonnelId",
                        column: x => x.PersonnelId,
                        principalSchema: "panel",
                        principalTable: "Personnel",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_LoanRequests_SchemePlans_PlanId",
                        column: x => x.PlanId,
                        principalSchema: "panel",
                        principalTable: "SchemePlans",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "LoanRequestDocuments",
                schema: "panel",
                columns: table => new
                {
                    RequestId = table.Column<long>(type: "bigint", nullable: false),
                    FileId = table.Column<long>(type: "bigint", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_LoanRequestDocuments", x => new { x.RequestId, x.FileId });
                    table.ForeignKey(
                        name: "FK_LoanRequestDocuments_Files_FileId",
                        column: x => x.FileId,
                        principalSchema: "panel",
                        principalTable: "Files",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_LoanRequestDocuments_LoanRequests_RequestId",
                        column: x => x.RequestId,
                        principalSchema: "panel",
                        principalTable: "LoanRequests",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "LoanRequestHistory",
                schema: "panel",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    RequestId = table.Column<long>(type: "bigint", nullable: false),
                    From = table.Column<byte>(type: "tinyint", nullable: true),
                    To = table.Column<byte>(type: "tinyint", nullable: false),
                    Stage = table.Column<byte>(type: "tinyint", nullable: true),
                    Reason = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    Description = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: true),
                    By = table.Column<string>(type: "nvarchar(400)", maxLength: 400, nullable: false),
                    At = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_LoanRequestHistory", x => x.Id);
                    table.ForeignKey(
                        name: "FK_LoanRequestHistory_LoanRequests_RequestId",
                        column: x => x.RequestId,
                        principalSchema: "panel",
                        principalTable: "LoanRequests",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_BalanceDocuments_CompanyId_Month",
                schema: "panel",
                table: "BalanceDocuments",
                columns: new[] { "CompanyId", "Month" });

            migrationBuilder.CreateIndex(
                name: "IX_BalanceDocuments_DocumentTypeId",
                schema: "panel",
                table: "BalanceDocuments",
                column: "DocumentTypeId");

            migrationBuilder.CreateIndex(
                name: "IX_BalanceDocuments_FileId",
                schema: "panel",
                table: "BalanceDocuments",
                column: "FileId");

            migrationBuilder.CreateIndex(
                name: "IX_ChangeLogs_At",
                schema: "panel",
                table: "ChangeLogs",
                column: "At");

            migrationBuilder.CreateIndex(
                name: "IX_ChangeLogs_BranchCode_Id",
                schema: "panel",
                table: "ChangeLogs",
                columns: new[] { "BranchCode", "Id" });

            migrationBuilder.CreateIndex(
                name: "IX_ChangeLogs_CompanyId_Id",
                schema: "panel",
                table: "ChangeLogs",
                columns: new[] { "CompanyId", "Id" });

            migrationBuilder.CreateIndex(
                name: "IX_Companies_AgreementFileId",
                schema: "panel",
                table: "Companies",
                column: "AgreementFileId");

            migrationBuilder.CreateIndex(
                name: "IX_Companies_BranchCode",
                schema: "panel",
                table: "Companies",
                column: "BranchCode");

            migrationBuilder.CreateIndex(
                name: "IX_Companies_NationalId",
                schema: "panel",
                table: "Companies",
                column: "NationalId");

            migrationBuilder.CreateIndex(
                name: "IX_Companies_SchemeId",
                schema: "panel",
                table: "Companies",
                column: "SchemeId");

            migrationBuilder.CreateIndex(
                name: "IX_CompanyCancellations_LetterFileId",
                schema: "panel",
                table: "CompanyCancellations",
                column: "LetterFileId");

            migrationBuilder.CreateIndex(
                name: "IX_CompanyCancellations_ReasonId",
                schema: "panel",
                table: "CompanyCancellations",
                column: "ReasonId");

            migrationBuilder.CreateIndex(
                name: "IX_CompanyPlans_PlanId",
                schema: "panel",
                table: "CompanyPlans",
                column: "PlanId");

            migrationBuilder.CreateIndex(
                name: "IX_LoanBatches_CompanyId",
                schema: "panel",
                table: "LoanBatches",
                column: "CompanyId");

            migrationBuilder.CreateIndex(
                name: "IX_LoanBatches_PlanId",
                schema: "panel",
                table: "LoanBatches",
                column: "PlanId");

            migrationBuilder.CreateIndex(
                name: "IX_LoanRequestDocuments_FileId",
                schema: "panel",
                table: "LoanRequestDocuments",
                column: "FileId");

            migrationBuilder.CreateIndex(
                name: "IX_LoanRequestHistory_RequestId",
                schema: "panel",
                table: "LoanRequestHistory",
                column: "RequestId");

            migrationBuilder.CreateIndex(
                name: "IX_LoanRequests_BatchId",
                schema: "panel",
                table: "LoanRequests",
                column: "BatchId");

            migrationBuilder.CreateIndex(
                name: "IX_LoanRequests_CompanyId_Status_PlanId",
                schema: "panel",
                table: "LoanRequests",
                columns: new[] { "CompanyId", "Status", "PlanId" });

            migrationBuilder.CreateIndex(
                name: "IX_LoanRequests_PersonnelId_Status",
                schema: "panel",
                table: "LoanRequests",
                columns: new[] { "PersonnelId", "Status" });

            migrationBuilder.CreateIndex(
                name: "IX_LoanRequests_PlanId",
                schema: "panel",
                table: "LoanRequests",
                column: "PlanId");

            migrationBuilder.CreateIndex(
                name: "IX_LoanRequests_Status_BranchCode",
                schema: "panel",
                table: "LoanRequests",
                columns: new[] { "Status", "BranchCode" });

            migrationBuilder.CreateIndex(
                name: "IX_Personnel_CompanyId_NationalCode",
                schema: "panel",
                table: "Personnel",
                columns: new[] { "CompanyId", "NationalCode" });

            migrationBuilder.CreateIndex(
                name: "IX_Personnel_CompanyId_Status_Priority",
                schema: "panel",
                table: "Personnel",
                columns: new[] { "CompanyId", "Status", "Priority" });

            migrationBuilder.CreateIndex(
                name: "IX_Personnel_DeleteFileId",
                schema: "panel",
                table: "Personnel",
                column: "DeleteFileId");

            migrationBuilder.CreateIndex(
                name: "IX_Personnel_ImportBatchId",
                schema: "panel",
                table: "Personnel",
                column: "ImportBatchId");

            migrationBuilder.CreateIndex(
                name: "IX_PersonnelHistory_PersonnelId",
                schema: "panel",
                table: "PersonnelHistory",
                column: "PersonnelId");

            migrationBuilder.CreateIndex(
                name: "IX_PersonnelImports_CompanyId",
                schema: "panel",
                table: "PersonnelImports",
                column: "CompanyId");

            migrationBuilder.CreateIndex(
                name: "IX_Reasons_Type_IsActive_SortOrder",
                schema: "panel",
                table: "Reasons",
                columns: new[] { "Type", "IsActive", "SortOrder" });

            migrationBuilder.CreateIndex(
                name: "IX_SchemePlans_SchemeId_ContractType_Rate_TermMonths",
                schema: "panel",
                table: "SchemePlans",
                columns: new[] { "SchemeId", "ContractType", "Rate", "TermMonths" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Schemes_Code",
                schema: "panel",
                table: "Schemes",
                column: "Code",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "BalanceDocuments",
                schema: "panel");

            migrationBuilder.DropTable(
                name: "ChangeLogs",
                schema: "panel");

            migrationBuilder.DropTable(
                name: "CompanyCancellations",
                schema: "panel");

            migrationBuilder.DropTable(
                name: "CompanyPlans",
                schema: "panel");

            migrationBuilder.DropTable(
                name: "LoanRequestDocuments",
                schema: "panel");

            migrationBuilder.DropTable(
                name: "LoanRequestHistory",
                schema: "panel");

            migrationBuilder.DropTable(
                name: "MonthlyBalances",
                schema: "panel");

            migrationBuilder.DropTable(
                name: "PersonnelHistory",
                schema: "panel");

            migrationBuilder.DropTable(
                name: "Reasons",
                schema: "panel");

            migrationBuilder.DropTable(
                name: "LoanRequests",
                schema: "panel");

            migrationBuilder.DropTable(
                name: "LoanBatches",
                schema: "panel");

            migrationBuilder.DropTable(
                name: "Personnel",
                schema: "panel");

            migrationBuilder.DropTable(
                name: "SchemePlans",
                schema: "panel");

            migrationBuilder.DropTable(
                name: "PersonnelImports",
                schema: "panel");

            migrationBuilder.DropTable(
                name: "SchemeSettings",
                schema: "panel");

            migrationBuilder.DropTable(
                name: "Companies",
                schema: "panel");

            migrationBuilder.DropTable(
                name: "Files",
                schema: "panel");

            migrationBuilder.DropTable(
                name: "Schemes",
                schema: "panel");
        }
    }
}
