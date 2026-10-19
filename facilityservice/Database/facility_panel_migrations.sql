-- جداول و دادهٔ اولیهٔ پنل Facility (طرح نگین میثاق) — اسکریپت idempotent همهٔ Migrationها؛ اجرای دوباره بی‌اثر است.
-- تولید: UPDATE_SQL=1 dotnet test --filter SqlScriptTests  (دستی ویرایش نکن)

IF OBJECT_ID(N'[panel].[__EFMigrationsHistory]') IS NULL
BEGIN
    IF SCHEMA_ID(N'panel') IS NULL EXEC(N'CREATE SCHEMA [panel];');
    CREATE TABLE [panel].[__EFMigrationsHistory] (
        [MigrationId] nvarchar(150) NOT NULL,
        [ProductVersion] nvarchar(32) NOT NULL,
        CONSTRAINT [PK___EFMigrationsHistory] PRIMARY KEY ([MigrationId])
    );
END;
GO

BEGIN TRANSACTION;
GO

IF NOT EXISTS (
    SELECT * FROM [panel].[__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261013044702_InitialFacilityPanel'
)
BEGIN
    IF SCHEMA_ID(N'panel') IS NULL EXEC(N'CREATE SCHEMA [panel];');
END;
GO

IF NOT EXISTS (
    SELECT * FROM [panel].[__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261013044702_InitialFacilityPanel'
)
BEGIN
    CREATE TABLE [panel].[Files] (
        [Id] bigint NOT NULL IDENTITY,
        [FileName] nvarchar(260) NOT NULL,
        [ContentType] nvarchar(150) NOT NULL,
        [Size] bigint NOT NULL,
        [StorageKey] nvarchar(300) NOT NULL,
        [UploadedAt] datetime2 NOT NULL,
        [UploadedBy] nvarchar(150) NOT NULL,
        CONSTRAINT [PK_Files] PRIMARY KEY ([Id])
    );
END;
GO

IF NOT EXISTS (
    SELECT * FROM [panel].[__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261013044702_InitialFacilityPanel'
)
BEGIN
    CREATE TABLE [panel].[Reasons] (
        [Id] int NOT NULL IDENTITY,
        [Type] tinyint NOT NULL,
        [Title] nvarchar(100) NOT NULL,
        [IsActive] bit NOT NULL,
        [SortOrder] int NOT NULL,
        CONSTRAINT [PK_Reasons] PRIMARY KEY ([Id])
    );
END;
GO

IF NOT EXISTS (
    SELECT * FROM [panel].[__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261013044702_InitialFacilityPanel'
)
BEGIN
    CREATE TABLE [panel].[Schemes] (
        [Id] int NOT NULL IDENTITY,
        [Code] nvarchar(30) NOT NULL,
        [Title] nvarchar(100) NOT NULL,
        [IsActive] bit NOT NULL,
        CONSTRAINT [PK_Schemes] PRIMARY KEY ([Id])
    );
END;
GO

IF NOT EXISTS (
    SELECT * FROM [panel].[__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261013044702_InitialFacilityPanel'
)
BEGIN
    CREATE TABLE [panel].[Companies] (
        [Id] bigint NOT NULL IDENTITY,
        [SchemeId] int NOT NULL,
        [Name] nvarchar(100) NOT NULL,
        [NationalId] varchar(11) NOT NULL,
        [AgreementNumber] nvarchar(50) NOT NULL,
        [AccountNumber] varchar(24) NOT NULL,
        [ContractDate] date NOT NULL,
        [AccountOpeningDate] date NOT NULL,
        [SalaryFromSepah] bit NOT NULL,
        [BranchCode] varchar(50) NOT NULL,
        [Status] tinyint NOT NULL,
        [Registrar] nvarchar(400) NOT NULL,
        [CreatedAt] datetime2 NOT NULL,
        [AgreementFileId] bigint NULL,
        [DeletedAt] datetime2 NULL,
        [DeleteDescription] nvarchar(500) NULL,
        [BalancesVersion] int NOT NULL,
        [Version] int NOT NULL,
        CONSTRAINT [PK_Companies] PRIMARY KEY ([Id]),
        CONSTRAINT [FK_Companies_Files_AgreementFileId] FOREIGN KEY ([AgreementFileId]) REFERENCES [panel].[Files] ([Id]) ON DELETE NO ACTION,
        CONSTRAINT [FK_Companies_Schemes_SchemeId] FOREIGN KEY ([SchemeId]) REFERENCES [panel].[Schemes] ([Id]) ON DELETE NO ACTION
    );
END;
GO

IF NOT EXISTS (
    SELECT * FROM [panel].[__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261013044702_InitialFacilityPanel'
)
BEGIN
    CREATE TABLE [panel].[SchemeSettings] (
        [SchemeId] int NOT NULL,
        [ContractTypes] varchar(50) NOT NULL,
        [QardHasanRate] decimal(5,2) NOT NULL,
        [MurabahaRates] varchar(200) NOT NULL,
        [RepaymentTermsMonths] varchar(200) NOT NULL,
        [SalaryFromSepahMultiplierYes] decimal(5,2) NOT NULL,
        [SalaryFromSepahMultiplierNo] decimal(5,2) NOT NULL,
        [ContractDurationMonths] int NOT NULL,
        [GraceMonthsAfterExpiry] int NOT NULL,
        [MaxPlansPerCompany] int NOT NULL,
        [PersonnelIntroductionDelayMonths] int NOT NULL,
        [WaitingPeriodDays] int NOT NULL,
        [DigitalApprovalEnabled] bit NOT NULL,
        [Version] int NOT NULL,
        CONSTRAINT [PK_SchemeSettings] PRIMARY KEY ([SchemeId]),
        CONSTRAINT [FK_SchemeSettings_Schemes_SchemeId] FOREIGN KEY ([SchemeId]) REFERENCES [panel].[Schemes] ([Id]) ON DELETE CASCADE
    );
END;
GO

IF NOT EXISTS (
    SELECT * FROM [panel].[__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261013044702_InitialFacilityPanel'
)
BEGIN
    CREATE TABLE [panel].[BalanceDocuments] (
        [Id] bigint NOT NULL IDENTITY,
        [CompanyId] bigint NOT NULL,
        [Month] varchar(7) NULL,
        [DocumentTypeId] int NOT NULL,
        [Description] nvarchar(500) NULL,
        [FileId] bigint NOT NULL,
        CONSTRAINT [PK_BalanceDocuments] PRIMARY KEY ([Id]),
        CONSTRAINT [FK_BalanceDocuments_Companies_CompanyId] FOREIGN KEY ([CompanyId]) REFERENCES [panel].[Companies] ([Id]) ON DELETE NO ACTION,
        CONSTRAINT [FK_BalanceDocuments_Files_FileId] FOREIGN KEY ([FileId]) REFERENCES [panel].[Files] ([Id]) ON DELETE NO ACTION,
        CONSTRAINT [FK_BalanceDocuments_Reasons_DocumentTypeId] FOREIGN KEY ([DocumentTypeId]) REFERENCES [panel].[Reasons] ([Id]) ON DELETE NO ACTION
    );
END;
GO

IF NOT EXISTS (
    SELECT * FROM [panel].[__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261013044702_InitialFacilityPanel'
)
BEGIN
    CREATE TABLE [panel].[ChangeLogs] (
        [Id] bigint NOT NULL IDENTITY,
        [At] datetime2 NOT NULL,
        [UserId] int NOT NULL,
        [User] nvarchar(400) NOT NULL,
        [Category] tinyint NOT NULL,
        [Action] nvarchar(100) NOT NULL,
        [Details] nvarchar(2000) NOT NULL,
        [Before] nvarchar(max) NULL,
        [After] nvarchar(max) NULL,
        [CompanyId] bigint NULL,
        [BranchCode] varchar(50) NULL,
        [RestoreKind] varchar(50) NULL,
        [RestorePayload] nvarchar(max) NULL,
        [RestoredAt] datetime2 NULL,
        CONSTRAINT [PK_ChangeLogs] PRIMARY KEY ([Id]),
        CONSTRAINT [FK_ChangeLogs_Companies_CompanyId] FOREIGN KEY ([CompanyId]) REFERENCES [panel].[Companies] ([Id]) ON DELETE NO ACTION
    );
END;
GO

IF NOT EXISTS (
    SELECT * FROM [panel].[__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261013044702_InitialFacilityPanel'
)
BEGIN
    CREATE TABLE [panel].[CompanyCancellations] (
        [CompanyId] bigint NOT NULL,
        [CancelDate] date NOT NULL,
        [ReasonId] int NOT NULL,
        [ReasonTitle] nvarchar(100) NOT NULL,
        [Description] nvarchar(1000) NOT NULL,
        [LetterFileId] bigint NOT NULL,
        [CancelledBy] nvarchar(400) NOT NULL,
        [CancelledAt] datetime2 NOT NULL,
        CONSTRAINT [PK_CompanyCancellations] PRIMARY KEY ([CompanyId]),
        CONSTRAINT [FK_CompanyCancellations_Companies_CompanyId] FOREIGN KEY ([CompanyId]) REFERENCES [panel].[Companies] ([Id]) ON DELETE CASCADE,
        CONSTRAINT [FK_CompanyCancellations_Files_LetterFileId] FOREIGN KEY ([LetterFileId]) REFERENCES [panel].[Files] ([Id]) ON DELETE NO ACTION,
        CONSTRAINT [FK_CompanyCancellations_Reasons_ReasonId] FOREIGN KEY ([ReasonId]) REFERENCES [panel].[Reasons] ([Id]) ON DELETE NO ACTION
    );
END;
GO

IF NOT EXISTS (
    SELECT * FROM [panel].[__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261013044702_InitialFacilityPanel'
)
BEGIN
    CREATE TABLE [panel].[MonthlyBalances] (
        [CompanyId] bigint NOT NULL,
        [Month] varchar(7) NOT NULL,
        [Amount] bigint NOT NULL,
        [EditedBy] nvarchar(400) NOT NULL,
        [EditedAt] datetime2 NOT NULL,
        CONSTRAINT [PK_MonthlyBalances] PRIMARY KEY ([CompanyId], [Month]),
        CONSTRAINT [FK_MonthlyBalances_Companies_CompanyId] FOREIGN KEY ([CompanyId]) REFERENCES [panel].[Companies] ([Id]) ON DELETE CASCADE
    );
END;
GO

IF NOT EXISTS (
    SELECT * FROM [panel].[__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261013044702_InitialFacilityPanel'
)
BEGIN
    CREATE TABLE [panel].[PersonnelImports] (
        [Id] bigint NOT NULL IDENTITY,
        [CompanyId] bigint NOT NULL,
        [FileName] nvarchar(260) NOT NULL,
        [TotalRows] int NOT NULL,
        [Accepted] int NOT NULL,
        [Rejected] int NOT NULL,
        [ErrorsJson] nvarchar(max) NOT NULL,
        [Registrar] nvarchar(400) NOT NULL,
        [CreatedAt] datetime2 NOT NULL,
        CONSTRAINT [PK_PersonnelImports] PRIMARY KEY ([Id]),
        CONSTRAINT [FK_PersonnelImports_Companies_CompanyId] FOREIGN KEY ([CompanyId]) REFERENCES [panel].[Companies] ([Id]) ON DELETE NO ACTION
    );
END;
GO

IF NOT EXISTS (
    SELECT * FROM [panel].[__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261013044702_InitialFacilityPanel'
)
BEGIN
    CREATE TABLE [panel].[SchemePlans] (
        [Id] int NOT NULL IDENTITY,
        [SchemeId] int NOT NULL,
        [ContractType] tinyint NOT NULL,
        [Rate] decimal(5,2) NOT NULL,
        [TermMonths] int NOT NULL,
        [UsablePercent] decimal(5,2) NOT NULL,
        [IsActive] bit NOT NULL,
        CONSTRAINT [PK_SchemePlans] PRIMARY KEY ([Id]),
        CONSTRAINT [FK_SchemePlans_SchemeSettings_SchemeId] FOREIGN KEY ([SchemeId]) REFERENCES [panel].[SchemeSettings] ([SchemeId]) ON DELETE CASCADE
    );
END;
GO

IF NOT EXISTS (
    SELECT * FROM [panel].[__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261013044702_InitialFacilityPanel'
)
BEGIN
    CREATE TABLE [panel].[Personnel] (
        [Id] bigint NOT NULL IDENTITY,
        [CompanyId] bigint NOT NULL,
        [Priority] int NOT NULL,
        [FullName] nvarchar(60) NOT NULL,
        [NationalCode] varchar(10) NOT NULL,
        [Mobile] varchar(11) NOT NULL,
        [PersonnelCode] varchar(20) NOT NULL,
        [HireDate] date NOT NULL,
        [Status] tinyint NOT NULL,
        [RejectReason] nvarchar(200) NULL,
        [CreatedAt] datetime2 NOT NULL,
        [DecidedAt] datetime2 NULL,
        [Registrar] nvarchar(400) NOT NULL,
        [ImportBatchId] bigint NULL,
        [DeleteDescription] nvarchar(500) NULL,
        [DeleteFileId] bigint NULL,
        [Version] int NOT NULL,
        CONSTRAINT [PK_Personnel] PRIMARY KEY ([Id]),
        CONSTRAINT [FK_Personnel_Companies_CompanyId] FOREIGN KEY ([CompanyId]) REFERENCES [panel].[Companies] ([Id]) ON DELETE NO ACTION,
        CONSTRAINT [FK_Personnel_Files_DeleteFileId] FOREIGN KEY ([DeleteFileId]) REFERENCES [panel].[Files] ([Id]) ON DELETE NO ACTION,
        CONSTRAINT [FK_Personnel_PersonnelImports_ImportBatchId] FOREIGN KEY ([ImportBatchId]) REFERENCES [panel].[PersonnelImports] ([Id]) ON DELETE NO ACTION
    );
END;
GO

IF NOT EXISTS (
    SELECT * FROM [panel].[__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261013044702_InitialFacilityPanel'
)
BEGIN
    CREATE TABLE [panel].[CompanyPlans] (
        [CompanyId] bigint NOT NULL,
        [PlanId] int NOT NULL,
        CONSTRAINT [PK_CompanyPlans] PRIMARY KEY ([CompanyId], [PlanId]),
        CONSTRAINT [FK_CompanyPlans_Companies_CompanyId] FOREIGN KEY ([CompanyId]) REFERENCES [panel].[Companies] ([Id]) ON DELETE CASCADE,
        CONSTRAINT [FK_CompanyPlans_SchemePlans_PlanId] FOREIGN KEY ([PlanId]) REFERENCES [panel].[SchemePlans] ([Id]) ON DELETE NO ACTION
    );
END;
GO

IF NOT EXISTS (
    SELECT * FROM [panel].[__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261013044702_InitialFacilityPanel'
)
BEGIN
    CREATE TABLE [panel].[LoanBatches] (
        [Id] bigint NOT NULL IDENTITY,
        [CompanyId] bigint NOT NULL,
        [PlanId] int NOT NULL,
        [Mode] tinyint NOT NULL,
        [BaseDate] date NOT NULL,
        [Description] nvarchar(1000) NULL,
        [Registrar] nvarchar(400) NOT NULL,
        [CreatedAt] datetime2 NOT NULL,
        [BranchCode] varchar(50) NOT NULL,
        CONSTRAINT [PK_LoanBatches] PRIMARY KEY ([Id]),
        CONSTRAINT [FK_LoanBatches_Companies_CompanyId] FOREIGN KEY ([CompanyId]) REFERENCES [panel].[Companies] ([Id]) ON DELETE NO ACTION,
        CONSTRAINT [FK_LoanBatches_SchemePlans_PlanId] FOREIGN KEY ([PlanId]) REFERENCES [panel].[SchemePlans] ([Id]) ON DELETE NO ACTION
    );
END;
GO

IF NOT EXISTS (
    SELECT * FROM [panel].[__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261013044702_InitialFacilityPanel'
)
BEGIN
    CREATE TABLE [panel].[PersonnelHistory] (
        [Id] bigint NOT NULL IDENTITY,
        [PersonnelId] bigint NOT NULL,
        [From] tinyint NULL,
        [To] tinyint NOT NULL,
        [Reason] nvarchar(200) NULL,
        [Description] nvarchar(1000) NULL,
        [By] nvarchar(400) NOT NULL,
        [At] datetime2 NOT NULL,
        CONSTRAINT [PK_PersonnelHistory] PRIMARY KEY ([Id]),
        CONSTRAINT [FK_PersonnelHistory_Personnel_PersonnelId] FOREIGN KEY ([PersonnelId]) REFERENCES [panel].[Personnel] ([Id]) ON DELETE CASCADE
    );
END;
GO

IF NOT EXISTS (
    SELECT * FROM [panel].[__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261013044702_InitialFacilityPanel'
)
BEGIN
    CREATE TABLE [panel].[LoanRequests] (
        [Id] bigint NOT NULL IDENTITY,
        [BatchId] bigint NOT NULL,
        [CompanyId] bigint NOT NULL,
        [BranchCode] varchar(50) NOT NULL,
        [PersonnelId] bigint NOT NULL,
        [PlanId] int NOT NULL,
        [Amount] bigint NOT NULL,
        [PaymentOrder] int NOT NULL,
        [CreatedAt] datetime2 NOT NULL,
        [DecidedAt] datetime2 NULL,
        [Status] tinyint NOT NULL,
        [PostStage] tinyint NULL,
        [Description] nvarchar(1200) NULL,
        [Registrar] nvarchar(400) NOT NULL,
        [Version] int NOT NULL,
        CONSTRAINT [PK_LoanRequests] PRIMARY KEY ([Id]),
        CONSTRAINT [FK_LoanRequests_Companies_CompanyId] FOREIGN KEY ([CompanyId]) REFERENCES [panel].[Companies] ([Id]) ON DELETE NO ACTION,
        CONSTRAINT [FK_LoanRequests_LoanBatches_BatchId] FOREIGN KEY ([BatchId]) REFERENCES [panel].[LoanBatches] ([Id]) ON DELETE CASCADE,
        CONSTRAINT [FK_LoanRequests_Personnel_PersonnelId] FOREIGN KEY ([PersonnelId]) REFERENCES [panel].[Personnel] ([Id]) ON DELETE NO ACTION,
        CONSTRAINT [FK_LoanRequests_SchemePlans_PlanId] FOREIGN KEY ([PlanId]) REFERENCES [panel].[SchemePlans] ([Id]) ON DELETE NO ACTION
    );
END;
GO

IF NOT EXISTS (
    SELECT * FROM [panel].[__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261013044702_InitialFacilityPanel'
)
BEGIN
    CREATE TABLE [panel].[LoanRequestDocuments] (
        [RequestId] bigint NOT NULL,
        [FileId] bigint NOT NULL,
        CONSTRAINT [PK_LoanRequestDocuments] PRIMARY KEY ([RequestId], [FileId]),
        CONSTRAINT [FK_LoanRequestDocuments_Files_FileId] FOREIGN KEY ([FileId]) REFERENCES [panel].[Files] ([Id]) ON DELETE NO ACTION,
        CONSTRAINT [FK_LoanRequestDocuments_LoanRequests_RequestId] FOREIGN KEY ([RequestId]) REFERENCES [panel].[LoanRequests] ([Id]) ON DELETE CASCADE
    );
END;
GO

IF NOT EXISTS (
    SELECT * FROM [panel].[__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261013044702_InitialFacilityPanel'
)
BEGIN
    CREATE TABLE [panel].[LoanRequestHistory] (
        [Id] bigint NOT NULL IDENTITY,
        [RequestId] bigint NOT NULL,
        [From] tinyint NULL,
        [To] tinyint NOT NULL,
        [Stage] tinyint NULL,
        [Reason] nvarchar(200) NULL,
        [Description] nvarchar(1000) NULL,
        [By] nvarchar(400) NOT NULL,
        [At] datetime2 NOT NULL,
        CONSTRAINT [PK_LoanRequestHistory] PRIMARY KEY ([Id]),
        CONSTRAINT [FK_LoanRequestHistory_LoanRequests_RequestId] FOREIGN KEY ([RequestId]) REFERENCES [panel].[LoanRequests] ([Id]) ON DELETE CASCADE
    );
END;
GO

IF NOT EXISTS (
    SELECT * FROM [panel].[__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261013044702_InitialFacilityPanel'
)
BEGIN
    CREATE INDEX [IX_BalanceDocuments_CompanyId_Month] ON [panel].[BalanceDocuments] ([CompanyId], [Month]);
END;
GO

IF NOT EXISTS (
    SELECT * FROM [panel].[__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261013044702_InitialFacilityPanel'
)
BEGIN
    CREATE INDEX [IX_BalanceDocuments_DocumentTypeId] ON [panel].[BalanceDocuments] ([DocumentTypeId]);
END;
GO

IF NOT EXISTS (
    SELECT * FROM [panel].[__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261013044702_InitialFacilityPanel'
)
BEGIN
    CREATE INDEX [IX_BalanceDocuments_FileId] ON [panel].[BalanceDocuments] ([FileId]);
END;
GO

IF NOT EXISTS (
    SELECT * FROM [panel].[__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261013044702_InitialFacilityPanel'
)
BEGIN
    CREATE INDEX [IX_ChangeLogs_At] ON [panel].[ChangeLogs] ([At]);
END;
GO

IF NOT EXISTS (
    SELECT * FROM [panel].[__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261013044702_InitialFacilityPanel'
)
BEGIN
    CREATE INDEX [IX_ChangeLogs_BranchCode_Id] ON [panel].[ChangeLogs] ([BranchCode], [Id]);
END;
GO

IF NOT EXISTS (
    SELECT * FROM [panel].[__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261013044702_InitialFacilityPanel'
)
BEGIN
    CREATE INDEX [IX_ChangeLogs_CompanyId_Id] ON [panel].[ChangeLogs] ([CompanyId], [Id]);
END;
GO

IF NOT EXISTS (
    SELECT * FROM [panel].[__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261013044702_InitialFacilityPanel'
)
BEGIN
    CREATE INDEX [IX_Companies_AgreementFileId] ON [panel].[Companies] ([AgreementFileId]);
END;
GO

IF NOT EXISTS (
    SELECT * FROM [panel].[__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261013044702_InitialFacilityPanel'
)
BEGIN
    CREATE INDEX [IX_Companies_BranchCode] ON [panel].[Companies] ([BranchCode]);
END;
GO

IF NOT EXISTS (
    SELECT * FROM [panel].[__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261013044702_InitialFacilityPanel'
)
BEGIN
    CREATE INDEX [IX_Companies_NationalId] ON [panel].[Companies] ([NationalId]);
END;
GO

IF NOT EXISTS (
    SELECT * FROM [panel].[__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261013044702_InitialFacilityPanel'
)
BEGIN
    CREATE INDEX [IX_Companies_SchemeId] ON [panel].[Companies] ([SchemeId]);
END;
GO

IF NOT EXISTS (
    SELECT * FROM [panel].[__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261013044702_InitialFacilityPanel'
)
BEGIN
    CREATE INDEX [IX_CompanyCancellations_LetterFileId] ON [panel].[CompanyCancellations] ([LetterFileId]);
END;
GO

IF NOT EXISTS (
    SELECT * FROM [panel].[__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261013044702_InitialFacilityPanel'
)
BEGIN
    CREATE INDEX [IX_CompanyCancellations_ReasonId] ON [panel].[CompanyCancellations] ([ReasonId]);
END;
GO

IF NOT EXISTS (
    SELECT * FROM [panel].[__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261013044702_InitialFacilityPanel'
)
BEGIN
    CREATE INDEX [IX_CompanyPlans_PlanId] ON [panel].[CompanyPlans] ([PlanId]);
END;
GO

IF NOT EXISTS (
    SELECT * FROM [panel].[__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261013044702_InitialFacilityPanel'
)
BEGIN
    CREATE INDEX [IX_LoanBatches_CompanyId] ON [panel].[LoanBatches] ([CompanyId]);
END;
GO

IF NOT EXISTS (
    SELECT * FROM [panel].[__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261013044702_InitialFacilityPanel'
)
BEGIN
    CREATE INDEX [IX_LoanBatches_PlanId] ON [panel].[LoanBatches] ([PlanId]);
END;
GO

IF NOT EXISTS (
    SELECT * FROM [panel].[__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261013044702_InitialFacilityPanel'
)
BEGIN
    CREATE INDEX [IX_LoanRequestDocuments_FileId] ON [panel].[LoanRequestDocuments] ([FileId]);
END;
GO

IF NOT EXISTS (
    SELECT * FROM [panel].[__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261013044702_InitialFacilityPanel'
)
BEGIN
    CREATE INDEX [IX_LoanRequestHistory_RequestId] ON [panel].[LoanRequestHistory] ([RequestId]);
END;
GO

IF NOT EXISTS (
    SELECT * FROM [panel].[__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261013044702_InitialFacilityPanel'
)
BEGIN
    CREATE INDEX [IX_LoanRequests_BatchId] ON [panel].[LoanRequests] ([BatchId]);
END;
GO

IF NOT EXISTS (
    SELECT * FROM [panel].[__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261013044702_InitialFacilityPanel'
)
BEGIN
    CREATE INDEX [IX_LoanRequests_CompanyId_Status_PlanId] ON [panel].[LoanRequests] ([CompanyId], [Status], [PlanId]);
END;
GO

IF NOT EXISTS (
    SELECT * FROM [panel].[__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261013044702_InitialFacilityPanel'
)
BEGIN
    CREATE INDEX [IX_LoanRequests_PersonnelId_Status] ON [panel].[LoanRequests] ([PersonnelId], [Status]);
END;
GO

IF NOT EXISTS (
    SELECT * FROM [panel].[__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261013044702_InitialFacilityPanel'
)
BEGIN
    CREATE INDEX [IX_LoanRequests_PlanId] ON [panel].[LoanRequests] ([PlanId]);
END;
GO

IF NOT EXISTS (
    SELECT * FROM [panel].[__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261013044702_InitialFacilityPanel'
)
BEGIN
    CREATE INDEX [IX_LoanRequests_Status_BranchCode] ON [panel].[LoanRequests] ([Status], [BranchCode]);
END;
GO

IF NOT EXISTS (
    SELECT * FROM [panel].[__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261013044702_InitialFacilityPanel'
)
BEGIN
    CREATE INDEX [IX_Personnel_CompanyId_NationalCode] ON [panel].[Personnel] ([CompanyId], [NationalCode]);
END;
GO

IF NOT EXISTS (
    SELECT * FROM [panel].[__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261013044702_InitialFacilityPanel'
)
BEGIN
    CREATE INDEX [IX_Personnel_CompanyId_Status_Priority] ON [panel].[Personnel] ([CompanyId], [Status], [Priority]);
END;
GO

IF NOT EXISTS (
    SELECT * FROM [panel].[__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261013044702_InitialFacilityPanel'
)
BEGIN
    CREATE INDEX [IX_Personnel_DeleteFileId] ON [panel].[Personnel] ([DeleteFileId]);
END;
GO

IF NOT EXISTS (
    SELECT * FROM [panel].[__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261013044702_InitialFacilityPanel'
)
BEGIN
    CREATE INDEX [IX_Personnel_ImportBatchId] ON [panel].[Personnel] ([ImportBatchId]);
END;
GO

IF NOT EXISTS (
    SELECT * FROM [panel].[__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261013044702_InitialFacilityPanel'
)
BEGIN
    CREATE INDEX [IX_PersonnelHistory_PersonnelId] ON [panel].[PersonnelHistory] ([PersonnelId]);
END;
GO

IF NOT EXISTS (
    SELECT * FROM [panel].[__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261013044702_InitialFacilityPanel'
)
BEGIN
    CREATE INDEX [IX_PersonnelImports_CompanyId] ON [panel].[PersonnelImports] ([CompanyId]);
END;
GO

IF NOT EXISTS (
    SELECT * FROM [panel].[__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261013044702_InitialFacilityPanel'
)
BEGIN
    CREATE INDEX [IX_Reasons_Type_IsActive_SortOrder] ON [panel].[Reasons] ([Type], [IsActive], [SortOrder]);
END;
GO

IF NOT EXISTS (
    SELECT * FROM [panel].[__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261013044702_InitialFacilityPanel'
)
BEGIN
    CREATE UNIQUE INDEX [IX_SchemePlans_SchemeId_ContractType_Rate_TermMonths] ON [panel].[SchemePlans] ([SchemeId], [ContractType], [Rate], [TermMonths]);
END;
GO

IF NOT EXISTS (
    SELECT * FROM [panel].[__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261013044702_InitialFacilityPanel'
)
BEGIN
    CREATE UNIQUE INDEX [IX_Schemes_Code] ON [panel].[Schemes] ([Code]);
END;
GO

IF NOT EXISTS (
    SELECT * FROM [panel].[__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261013044702_InitialFacilityPanel'
)
BEGIN
    INSERT INTO [panel].[__EFMigrationsHistory] ([MigrationId], [ProductVersion])
    VALUES (N'20261013044702_InitialFacilityPanel', N'8.0.10');
END;
GO

COMMIT;
GO

BEGIN TRANSACTION;
GO

IF NOT EXISTS (
    SELECT * FROM [panel].[__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261013044718_SeedNeginMisaghScheme'
)
BEGIN

    IF NOT EXISTS (SELECT 1 FROM [panel].[Schemes] WHERE [Code] = N'NEGIN')
    BEGIN
        DECLARE @scheme int;
        INSERT INTO [panel].[Schemes] ([Code], [Title], [IsActive]) VALUES (N'NEGIN', N'نگین میثاق', 1);
        SET @scheme = SCOPE_IDENTITY();

        INSERT INTO [panel].[SchemeSettings] ([SchemeId], [ContractTypes], [QardHasanRate], [MurabahaRates], [RepaymentTermsMonths],
            [SalaryFromSepahMultiplierYes], [SalaryFromSepahMultiplierNo], [ContractDurationMonths], [GraceMonthsAfterExpiry],
            [MaxPlansPerCompany], [PersonnelIntroductionDelayMonths], [WaitingPeriodDays], [DigitalApprovalEnabled], [Version])
        VALUES (@scheme, '[1,2]', 4, '[5,10,15,20,23]', '[12,24,36,48,60]', 1.10, 1.00, 12, 4, 5, 1, 0, 0, 1);

        INSERT INTO [panel].[SchemePlans] ([SchemeId], [ContractType], [Rate], [TermMonths], [UsablePercent], [IsActive]) VALUES
        (@scheme, 1, 4, 12, 8, 1),
        (@scheme, 1, 4, 24, 6, 1),
        (@scheme, 1, 4, 36, 5, 1),
        (@scheme, 1, 4, 48, 4, 1),
        (@scheme, 1, 4, 60, 3, 1),
        (@scheme, 2, 5, 12, 10, 1),
        (@scheme, 2, 5, 24, 8, 1),
        (@scheme, 2, 5, 36, 6, 1),
        (@scheme, 2, 5, 48, 4, 1),
        (@scheme, 2, 5, 60, 3, 1),
        (@scheme, 2, 10, 12, 15, 1),
        (@scheme, 2, 10, 24, 10, 1),
        (@scheme, 2, 10, 36, 8, 1),
        (@scheme, 2, 10, 48, 6, 1),
        (@scheme, 2, 10, 60, 5, 1),
        (@scheme, 2, 15, 12, 25, 1),
        (@scheme, 2, 15, 24, 15, 1),
        (@scheme, 2, 15, 36, 10, 1),
        (@scheme, 2, 15, 48, 8, 1),
        (@scheme, 2, 15, 60, 6, 1),
        (@scheme, 2, 20, 12, 35, 1),
        (@scheme, 2, 20, 24, 20, 1),
        (@scheme, 2, 20, 36, 15, 1),
        (@scheme, 2, 20, 48, 10, 1),
        (@scheme, 2, 20, 60, 8, 1),
        (@scheme, 2, 23, 12, 45, 1),
        (@scheme, 2, 23, 24, 25, 1),
        (@scheme, 2, 23, 36, 20, 1),
        (@scheme, 2, 23, 48, 15, 1),
        (@scheme, 2, 23, 60, 10, 1);
    END;

    IF NOT EXISTS (SELECT 1 FROM [panel].[Reasons])
        INSERT INTO [panel].[Reasons] ([Type], [Title], [IsActive], [SortOrder]) VALUES
        (1, N'عدم تمدید به درخواست شرکت', 1, 0),
        (1, N'لغو قرارداد توسط مدیریت شعب منطقه', 1, 1),
        (2, N'عدم کنترل سقف فردی', 1, 0),
        (2, N'پایین بودن رتبه اعتباری', 1, 1),
        (3, N'رتبه اعتباری غیرمجاز', 1, 0),
        (3, N'عدم کفایت سقف تسهیلات بر اساس اعتبارسنجی', 1, 1),
        (4, N'انصراف متقاضی', 1, 0),
        (4, N'ترک خدمت', 1, 1),
        (5, N'اطلاعات نادرست', 1, 0),
        (5, N'عدم احراز شرایط', 1, 1),
        (6, N'تصویر بنکو', 1, 0),
        (6, N'نامه', 1, 1),
        (6, N'گردش حساب', 1, 2);

END;
GO

IF NOT EXISTS (
    SELECT * FROM [panel].[__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261013044718_SeedNeginMisaghScheme'
)
BEGIN
    INSERT INTO [panel].[__EFMigrationsHistory] ([MigrationId], [ProductVersion])
    VALUES (N'20261013044718_SeedNeginMisaghScheme', N'8.0.10');
END;
GO

COMMIT;
GO

BEGIN TRANSACTION;
GO

IF NOT EXISTS (
    SELECT * FROM [panel].[__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261013050730_AddSchemaComments'
)
BEGIN
    DECLARE @description AS sql_variant;
    SET @description = N'اطلاعات پایهٔ طرح — همان فرم صفحهٔ اول نمونه (دکمهٔ «ذخیره تنظیمات پایه»)';
    EXEC sp_addextendedproperty 'MS_Description', @description, 'SCHEMA', N'panel', 'TABLE', N'SchemeSettings';
END;
GO

IF NOT EXISTS (
    SELECT * FROM [panel].[__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261013050730_AddSchemaComments'
)
BEGIN
    DECLARE @description AS sql_variant;
    SET @description = N'طرح کلان — نگین میثاق، شجره طیبه، هم‌پیمان، …';
    EXEC sp_addextendedproperty 'MS_Description', @description, 'SCHEMA', N'panel', 'TABLE', N'Schemes';
END;
GO

IF NOT EXISTS (
    SELECT * FROM [panel].[__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261013050730_AddSchemaComments'
)
BEGIN
    DECLARE @description AS sql_variant;
    SET @description = N'طرح قابل انتخاب — هر ردیف «جدول درصد قابل استفاده»؛ ۳۰ طرح در نمونه';
    EXEC sp_addextendedproperty 'MS_Description', @description, 'SCHEMA', N'panel', 'TABLE', N'SchemePlans';
END;
GO

IF NOT EXISTS (
    SELECT * FROM [panel].[__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261013050730_AddSchemaComments'
)
BEGIN
    DECLARE @description AS sql_variant;
    SET @description = N'فهرست‌های داینامیک — دلایل و انواع مستند؛ حذف نرم';
    EXEC sp_addextendedproperty 'MS_Description', @description, 'SCHEMA', N'panel', 'TABLE', N'Reasons';
END;
GO

IF NOT EXISTS (
    SELECT * FROM [panel].[__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261013050730_AddSchemaComments'
)
BEGIN
    DECLARE @description AS sql_variant;
    SET @description = N'بارگذاری گروهی پرسنل — نتیجهٔ هر فایل اکسل/CSV (US-E2)';
    EXEC sp_addextendedproperty 'MS_Description', @description, 'SCHEMA', N'panel', 'TABLE', N'PersonnelImports';
END;
GO

IF NOT EXISTS (
    SELECT * FROM [panel].[__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261013050730_AddSchemaComments'
)
BEGIN
    DECLARE @description AS sql_variant;
    SET @description = N'تاریخچهٔ وضعیت پرسنل — US-E3: وضعیت‌ها با تاریخ و ساعت';
    EXEC sp_addextendedproperty 'MS_Description', @description, 'SCHEMA', N'panel', 'TABLE', N'PersonnelHistory';
END;
GO

IF NOT EXISTS (
    SELECT * FROM [panel].[__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261013050730_AddSchemaComments'
)
BEGIN
    DECLARE @description AS sql_variant;
    SET @description = N'پرسنل — افراد معرفی‌شدهٔ شرکت؛ قفل پس از استفاده در وام';
    EXEC sp_addextendedproperty 'MS_Description', @description, 'SCHEMA', N'panel', 'TABLE', N'Personnel';
END;
GO

IF NOT EXISTS (
    SELECT * FROM [panel].[__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261013050730_AddSchemaComments'
)
BEGIN
    DECLARE @description AS sql_variant;
    SET @description = N'میانگین موجودی ماهانه — ۱۲ ماه سال جاری قرارداد (US-B4)';
    EXEC sp_addextendedproperty 'MS_Description', @description, 'SCHEMA', N'panel', 'TABLE', N'MonthlyBalances';
END;
GO

IF NOT EXISTS (
    SELECT * FROM [panel].[__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261013050730_AddSchemaComments'
)
BEGIN
    DECLARE @description AS sql_variant;
    SET @description = N'درخواست وام — درخواست یک نفر؛ جریان تأیید چندمرحله‌ای';
    EXEC sp_addextendedproperty 'MS_Description', @description, 'SCHEMA', N'panel', 'TABLE', N'LoanRequests';
END;
GO

IF NOT EXISTS (
    SELECT * FROM [panel].[__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261013050730_AddSchemaComments'
)
BEGIN
    DECLARE @description AS sql_variant;
    SET @description = N'تاریخچهٔ تصمیم‌ها — قاعدهٔ ۵: تاریخ، تصمیم‌گیرنده، وضعیت قبلی و جدید';
    EXEC sp_addextendedproperty 'MS_Description', @description, 'SCHEMA', N'panel', 'TABLE', N'LoanRequestHistory';
END;
GO

IF NOT EXISTS (
    SELECT * FROM [panel].[__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261013050730_AddSchemaComments'
)
BEGIN
    DECLARE @description AS sql_variant;
    SET @description = N'پیوست تصمیم — مصوبه/سند پیوست تصمیم اعتبارات';
    EXEC sp_addextendedproperty 'MS_Description', @description, 'SCHEMA', N'panel', 'TABLE', N'LoanRequestDocuments';
END;
GO

IF NOT EXISTS (
    SELECT * FROM [panel].[__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261013050730_AddSchemaComments'
)
BEGIN
    DECLARE @description AS sql_variant;
    SET @description = N'ثبت تخصیص وام — چند درخواست هم‌طرح که با هم ثبت شدند (US-D3)';
    EXEC sp_addextendedproperty 'MS_Description', @description, 'SCHEMA', N'panel', 'TABLE', N'LoanBatches';
END;
GO

IF NOT EXISTS (
    SELECT * FROM [panel].[__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261013050730_AddSchemaComments'
)
BEGIN
    DECLARE @description AS sql_variant;
    SET @description = N'فایل — متادیتای فایل؛ محتوا در Minio';
    EXEC sp_addextendedproperty 'MS_Description', @description, 'SCHEMA', N'panel', 'TABLE', N'Files';
END;
GO

IF NOT EXISTS (
    SELECT * FROM [panel].[__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261013050730_AddSchemaComments'
)
BEGIN
    DECLARE @description AS sql_variant;
    SET @description = N'طرح‌های تفاهم‌نامه — جدول واسط شرکت ↔ طرح (چند به چند)';
    EXEC sp_addextendedproperty 'MS_Description', @description, 'SCHEMA', N'panel', 'TABLE', N'CompanyPlans';
END;
GO

IF NOT EXISTS (
    SELECT * FROM [panel].[__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261013050730_AddSchemaComments'
)
BEGIN
    DECLARE @description AS sql_variant;
    SET @description = N'لغو قرارداد — مدال لغو (US-B6)؛ حداکثر یکی برای هر شرکت';
    EXEC sp_addextendedproperty 'MS_Description', @description, 'SCHEMA', N'panel', 'TABLE', N'CompanyCancellations';
END;
GO

IF NOT EXISTS (
    SELECT * FROM [panel].[__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261013050730_AddSchemaComments'
)
BEGIN
    DECLARE @description AS sql_variant;
    SET @description = N'شرکت — شرکت طرف تفاهم‌نامه؛ حذف نرم';
    EXEC sp_addextendedproperty 'MS_Description', @description, 'SCHEMA', N'panel', 'TABLE', N'Companies';
END;
GO

IF NOT EXISTS (
    SELECT * FROM [panel].[__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261013050730_AddSchemaComments'
)
BEGIN
    DECLARE @description AS sql_variant;
    SET @description = N'گزارش تغییرات — US-A4: هر ثبت، ویرایش، لغو، حذف و تصمیم';
    EXEC sp_addextendedproperty 'MS_Description', @description, 'SCHEMA', N'panel', 'TABLE', N'ChangeLogs';
END;
GO

IF NOT EXISTS (
    SELECT * FROM [panel].[__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261013050730_AddSchemaComments'
)
BEGIN
    DECLARE @description AS sql_variant;
    SET @description = N'مستند میانگین موجودی — US-B5؛ حذف فقط ادمین';
    EXEC sp_addextendedproperty 'MS_Description', @description, 'SCHEMA', N'panel', 'TABLE', N'BalanceDocuments';
END;
GO

IF NOT EXISTS (
    SELECT * FROM [panel].[__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261013050730_AddSchemaComments'
)
BEGIN
    DECLARE @description AS sql_variant;
    SET @description = N'دورهٔ انتظار معرفی به بانک (روز)';
    EXEC sp_addextendedproperty 'MS_Description', @description, 'SCHEMA', N'panel', 'TABLE', N'SchemeSettings', 'COLUMN', N'WaitingPeriodDays';
END;
GO

IF NOT EXISTS (
    SELECT * FROM [panel].[__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261013050730_AddSchemaComments'
)
BEGIN
    DECLARE @description AS sql_variant;
    SET @description = N'نسخهٔ همزمانی؛ با هر تغییر +۱ (در API: rowVersion)';
    EXEC sp_addextendedproperty 'MS_Description', @description, 'SCHEMA', N'panel', 'TABLE', N'SchemeSettings', 'COLUMN', N'Version';
END;
GO

IF NOT EXISTS (
    SELECT * FROM [panel].[__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261013050730_AddSchemaComments'
)
BEGIN
    DECLARE @description AS sql_variant;
    SET @description = N'ضریب دریافت حقوق از سپه — بله (۱٫۱)';
    EXEC sp_addextendedproperty 'MS_Description', @description, 'SCHEMA', N'panel', 'TABLE', N'SchemeSettings', 'COLUMN', N'SalaryFromSepahMultiplierYes';
END;
GO

IF NOT EXISTS (
    SELECT * FROM [panel].[__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261013050730_AddSchemaComments'
)
BEGIN
    DECLARE @description AS sql_variant;
    SET @description = N'ضریب دریافت حقوق از سپه — خیر (۱)';
    EXEC sp_addextendedproperty 'MS_Description', @description, 'SCHEMA', N'panel', 'TABLE', N'SchemeSettings', 'COLUMN', N'SalaryFromSepahMultiplierNo';
END;
GO

IF NOT EXISTS (
    SELECT * FROM [panel].[__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261013050730_AddSchemaComments'
)
BEGIN
    DECLARE @description AS sql_variant;
    SET @description = N'مدت‌های بازپرداخت (ماه)، JSON';
    EXEC sp_addextendedproperty 'MS_Description', @description, 'SCHEMA', N'panel', 'TABLE', N'SchemeSettings', 'COLUMN', N'RepaymentTermsMonths';
END;
GO

IF NOT EXISTS (
    SELECT * FROM [panel].[__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261013050730_AddSchemaComments'
)
BEGIN
    DECLARE @description AS sql_variant;
    SET @description = N'نرخ قرض‌الحسنه (درصد) — bQarz؛ روی همهٔ طرح‌های قرض‌الحسنه';
    EXEC sp_addextendedproperty 'MS_Description', @description, 'SCHEMA', N'panel', 'TABLE', N'SchemeSettings', 'COLUMN', N'QardHasanRate';
END;
GO

IF NOT EXISTS (
    SELECT * FROM [panel].[__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261013050730_AddSchemaComments'
)
BEGIN
    DECLARE @description AS sql_variant;
    SET @description = N'مهلت مجاز شدن معرفی پرسنل پس از انعقاد (ماه)';
    EXEC sp_addextendedproperty 'MS_Description', @description, 'SCHEMA', N'panel', 'TABLE', N'SchemeSettings', 'COLUMN', N'PersonnelIntroductionDelayMonths';
END;
GO

IF NOT EXISTS (
    SELECT * FROM [panel].[__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261013050730_AddSchemaComments'
)
BEGIN
    DECLARE @description AS sql_variant;
    SET @description = N'نرخ‌های مرابحه، JSON: [5,10,15,20,23]';
    EXEC sp_addextendedproperty 'MS_Description', @description, 'SCHEMA', N'panel', 'TABLE', N'SchemeSettings', 'COLUMN', N'MurabahaRates';
END;
GO

IF NOT EXISTS (
    SELECT * FROM [panel].[__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261013050730_AddSchemaComments'
)
BEGIN
    DECLARE @description AS sql_variant;
    SET @description = N'حداکثر طرح قابل انتخاب برای هر شرکت (۱ تا ۱۰)';
    EXEC sp_addextendedproperty 'MS_Description', @description, 'SCHEMA', N'panel', 'TABLE', N'SchemeSettings', 'COLUMN', N'MaxPlansPerCompany';
END;
GO

IF NOT EXISTS (
    SELECT * FROM [panel].[__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261013050730_AddSchemaComments'
)
BEGIN
    DECLARE @description AS sql_variant;
    SET @description = N'مهلت استفاده از مانده پس از انقضا (ماه)';
    EXEC sp_addextendedproperty 'MS_Description', @description, 'SCHEMA', N'panel', 'TABLE', N'SchemeSettings', 'COLUMN', N'GraceMonthsAfterExpiry';
END;
GO

IF NOT EXISTS (
    SELECT * FROM [panel].[__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261013050730_AddSchemaComments'
)
BEGIN
    DECLARE @description AS sql_variant;
    SET @description = N'مرحلهٔ تأیید شعبهٔ دیجیتال فعال است؟';
    EXEC sp_addextendedproperty 'MS_Description', @description, 'SCHEMA', N'panel', 'TABLE', N'SchemeSettings', 'COLUMN', N'DigitalApprovalEnabled';
END;
GO

IF NOT EXISTS (
    SELECT * FROM [panel].[__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261013050730_AddSchemaComments'
)
BEGIN
    DECLARE @description AS sql_variant;
    SET @description = N'انواع عقد مجاز، JSON: [1,2]';
    EXEC sp_addextendedproperty 'MS_Description', @description, 'SCHEMA', N'panel', 'TABLE', N'SchemeSettings', 'COLUMN', N'ContractTypes';
END;
GO

IF NOT EXISTS (
    SELECT * FROM [panel].[__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261013050730_AddSchemaComments'
)
BEGIN
    DECLARE @description AS sql_variant;
    SET @description = N'مدت قرارداد (ماه، ۱ تا ۱۲۰)';
    EXEC sp_addextendedproperty 'MS_Description', @description, 'SCHEMA', N'panel', 'TABLE', N'SchemeSettings', 'COLUMN', N'ContractDurationMonths';
END;
GO

IF NOT EXISTS (
    SELECT * FROM [panel].[__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261013050730_AddSchemaComments'
)
BEGIN
    DECLARE @description AS sql_variant;
    SET @description = N'شناسهٔ طرح (۱ به ۱ با Schemes)';
    EXEC sp_addextendedproperty 'MS_Description', @description, 'SCHEMA', N'panel', 'TABLE', N'SchemeSettings', 'COLUMN', N'SchemeId';
END;
GO

IF NOT EXISTS (
    SELECT * FROM [panel].[__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261013050730_AddSchemaComments'
)
BEGIN
    DECLARE @description AS sql_variant;
    SET @description = N'نام طرح';
    EXEC sp_addextendedproperty 'MS_Description', @description, 'SCHEMA', N'panel', 'TABLE', N'Schemes', 'COLUMN', N'Title';
END;
GO

IF NOT EXISTS (
    SELECT * FROM [panel].[__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261013050730_AddSchemaComments'
)
BEGIN
    DECLARE @description AS sql_variant;
    SET @description = N'فعال/غیرفعال';
    EXEC sp_addextendedproperty 'MS_Description', @description, 'SCHEMA', N'panel', 'TABLE', N'Schemes', 'COLUMN', N'IsActive';
END;
GO

IF NOT EXISTS (
    SELECT * FROM [panel].[__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261013050730_AddSchemaComments'
)
BEGIN
    DECLARE @description AS sql_variant;
    SET @description = N'کد یکتا؛ مثلاً NEGIN';
    EXEC sp_addextendedproperty 'MS_Description', @description, 'SCHEMA', N'panel', 'TABLE', N'Schemes', 'COLUMN', N'Code';
END;
GO

IF NOT EXISTS (
    SELECT * FROM [panel].[__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261013050730_AddSchemaComments'
)
BEGIN
    DECLARE @description AS sql_variant;
    SET @description = N'شناسهٔ طرح (Identity)';
    EXEC sp_addextendedproperty 'MS_Description', @description, 'SCHEMA', N'panel', 'TABLE', N'Schemes', 'COLUMN', N'Id';
END;
GO

IF NOT EXISTS (
    SELECT * FROM [panel].[__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261013050730_AddSchemaComments'
)
BEGIN
    DECLARE @description AS sql_variant;
    SET @description = N'درصد قابل استفاده از مجموع میانگین';
    EXEC sp_addextendedproperty 'MS_Description', @description, 'SCHEMA', N'panel', 'TABLE', N'SchemePlans', 'COLUMN', N'UsablePercent';
END;
GO

IF NOT EXISTS (
    SELECT * FROM [panel].[__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261013050730_AddSchemaComments'
)
BEGIN
    DECLARE @description AS sql_variant;
    SET @description = N'مدت بازپرداخت (ماه)';
    EXEC sp_addextendedproperty 'MS_Description', @description, 'SCHEMA', N'panel', 'TABLE', N'SchemePlans', 'COLUMN', N'TermMonths';
END;
GO

IF NOT EXISTS (
    SELECT * FROM [panel].[__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261013050730_AddSchemaComments'
)
BEGIN
    DECLARE @description AS sql_variant;
    SET @description = N'طرح کلان';
    EXEC sp_addextendedproperty 'MS_Description', @description, 'SCHEMA', N'panel', 'TABLE', N'SchemePlans', 'COLUMN', N'SchemeId';
END;
GO

IF NOT EXISTS (
    SELECT * FROM [panel].[__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261013050730_AddSchemaComments'
)
BEGIN
    DECLARE @description AS sql_variant;
    SET @description = N'نرخ (درصد)';
    EXEC sp_addextendedproperty 'MS_Description', @description, 'SCHEMA', N'panel', 'TABLE', N'SchemePlans', 'COLUMN', N'Rate';
END;
GO

IF NOT EXISTS (
    SELECT * FROM [panel].[__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261013050730_AddSchemaComments'
)
BEGIN
    DECLARE @description AS sql_variant;
    SET @description = N'ردیف حذف‌شده از جدول غیرفعال می‌شود، نه پاک';
    EXEC sp_addextendedproperty 'MS_Description', @description, 'SCHEMA', N'panel', 'TABLE', N'SchemePlans', 'COLUMN', N'IsActive';
END;
GO

IF NOT EXISTS (
    SELECT * FROM [panel].[__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261013050730_AddSchemaComments'
)
BEGIN
    DECLARE @description AS sql_variant;
    SET @description = N'۱ قرض‌الحسنه، ۲ مرابحه';
    EXEC sp_addextendedproperty 'MS_Description', @description, 'SCHEMA', N'panel', 'TABLE', N'SchemePlans', 'COLUMN', N'ContractType';
END;
GO

IF NOT EXISTS (
    SELECT * FROM [panel].[__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261013050730_AddSchemaComments'
)
BEGIN
    DECLARE @description AS sql_variant;
    SET @description = N'شناسهٔ طرح؛ بعد از ویرایش اطلاعات پایه ثابت می‌ماند';
    EXEC sp_addextendedproperty 'MS_Description', @description, 'SCHEMA', N'panel', 'TABLE', N'SchemePlans', 'COLUMN', N'Id';
END;
GO

IF NOT EXISTS (
    SELECT * FROM [panel].[__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261013050730_AddSchemaComments'
)
BEGIN
    DECLARE @description AS sql_variant;
    SET @description = N'۱ لغو قرارداد، ۲ رد وام، ۳ عدم اعطای غیرحضوری، ۴ لغو درخواست، ۵ رد پرسنل، ۶ نوع مستند';
    EXEC sp_addextendedproperty 'MS_Description', @description, 'SCHEMA', N'panel', 'TABLE', N'Reasons', 'COLUMN', N'Type';
END;
GO

IF NOT EXISTS (
    SELECT * FROM [panel].[__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261013050730_AddSchemaComments'
)
BEGIN
    DECLARE @description AS sql_variant;
    SET @description = N'عنوان';
    EXEC sp_addextendedproperty 'MS_Description', @description, 'SCHEMA', N'panel', 'TABLE', N'Reasons', 'COLUMN', N'Title';
END;
GO

IF NOT EXISTS (
    SELECT * FROM [panel].[__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261013050730_AddSchemaComments'
)
BEGIN
    DECLARE @description AS sql_variant;
    SET @description = N'ترتیب نمایش';
    EXEC sp_addextendedproperty 'MS_Description', @description, 'SCHEMA', N'panel', 'TABLE', N'Reasons', 'COLUMN', N'SortOrder';
END;
GO

IF NOT EXISTS (
    SELECT * FROM [panel].[__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261013050730_AddSchemaComments'
)
BEGIN
    DECLARE @description AS sql_variant;
    SET @description = N'فعال';
    EXEC sp_addextendedproperty 'MS_Description', @description, 'SCHEMA', N'panel', 'TABLE', N'Reasons', 'COLUMN', N'IsActive';
END;
GO

IF NOT EXISTS (
    SELECT * FROM [panel].[__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261013050730_AddSchemaComments'
)
BEGIN
    DECLARE @description AS sql_variant;
    SET @description = N'شناسه';
    EXEC sp_addextendedproperty 'MS_Description', @description, 'SCHEMA', N'panel', 'TABLE', N'Reasons', 'COLUMN', N'Id';
END;
GO

IF NOT EXISTS (
    SELECT * FROM [panel].[__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261013050730_AddSchemaComments'
)
BEGIN
    DECLARE @description AS sql_variant;
    SET @description = N'تعداد ردیف';
    EXEC sp_addextendedproperty 'MS_Description', @description, 'SCHEMA', N'panel', 'TABLE', N'PersonnelImports', 'COLUMN', N'TotalRows';
END;
GO

IF NOT EXISTS (
    SELECT * FROM [panel].[__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261013050730_AddSchemaComments'
)
BEGIN
    DECLARE @description AS sql_variant;
    SET @description = N'ردشده';
    EXEC sp_addextendedproperty 'MS_Description', @description, 'SCHEMA', N'panel', 'TABLE', N'PersonnelImports', 'COLUMN', N'Rejected';
END;
GO

IF NOT EXISTS (
    SELECT * FROM [panel].[__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261013050730_AddSchemaComments'
)
BEGIN
    DECLARE @description AS sql_variant;
    SET @description = N'عکس فوری کاربر (JSON): شناسه، نام، کد کاربر، نام و کد شعبه';
    EXEC sp_addextendedproperty 'MS_Description', @description, 'SCHEMA', N'panel', 'TABLE', N'PersonnelImports', 'COLUMN', N'Registrar';
END;
GO

IF NOT EXISTS (
    SELECT * FROM [panel].[__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261013050730_AddSchemaComments'
)
BEGIN
    DECLARE @description AS sql_variant;
    SET @description = N'نام فایل';
    EXEC sp_addextendedproperty 'MS_Description', @description, 'SCHEMA', N'panel', 'TABLE', N'PersonnelImports', 'COLUMN', N'FileName';
END;
GO

IF NOT EXISTS (
    SELECT * FROM [panel].[__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261013050730_AddSchemaComments'
)
BEGIN
    DECLARE @description AS sql_variant;
    SET @description = N'خطای هر ردیف: [{row, column, message}]';
    EXEC sp_addextendedproperty 'MS_Description', @description, 'SCHEMA', N'panel', 'TABLE', N'PersonnelImports', 'COLUMN', N'ErrorsJson';
END;
GO

IF NOT EXISTS (
    SELECT * FROM [panel].[__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261013050730_AddSchemaComments'
)
BEGIN
    DECLARE @description AS sql_variant;
    SET @description = N'زمان';
    EXEC sp_addextendedproperty 'MS_Description', @description, 'SCHEMA', N'panel', 'TABLE', N'PersonnelImports', 'COLUMN', N'CreatedAt';
END;
GO

IF NOT EXISTS (
    SELECT * FROM [panel].[__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261013050730_AddSchemaComments'
)
BEGIN
    DECLARE @description AS sql_variant;
    SET @description = N'شرکت';
    EXEC sp_addextendedproperty 'MS_Description', @description, 'SCHEMA', N'panel', 'TABLE', N'PersonnelImports', 'COLUMN', N'CompanyId';
END;
GO

IF NOT EXISTS (
    SELECT * FROM [panel].[__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261013050730_AddSchemaComments'
)
BEGIN
    DECLARE @description AS sql_variant;
    SET @description = N'ثبت‌شده';
    EXEC sp_addextendedproperty 'MS_Description', @description, 'SCHEMA', N'panel', 'TABLE', N'PersonnelImports', 'COLUMN', N'Accepted';
END;
GO

IF NOT EXISTS (
    SELECT * FROM [panel].[__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261013050730_AddSchemaComments'
)
BEGIN
    DECLARE @description AS sql_variant;
    SET @description = N'شناسه';
    EXEC sp_addextendedproperty 'MS_Description', @description, 'SCHEMA', N'panel', 'TABLE', N'PersonnelImports', 'COLUMN', N'Id';
END;
GO

IF NOT EXISTS (
    SELECT * FROM [panel].[__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261013050730_AddSchemaComments'
)
BEGIN
    DECLARE @description AS sql_variant;
    SET @description = N'وضعیت جدید';
    EXEC sp_addextendedproperty 'MS_Description', @description, 'SCHEMA', N'panel', 'TABLE', N'PersonnelHistory', 'COLUMN', N'To';
END;
GO

IF NOT EXISTS (
    SELECT * FROM [panel].[__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261013050730_AddSchemaComments'
)
BEGIN
    DECLARE @description AS sql_variant;
    SET @description = N'دلیل';
    EXEC sp_addextendedproperty 'MS_Description', @description, 'SCHEMA', N'panel', 'TABLE', N'PersonnelHistory', 'COLUMN', N'Reason';
END;
GO

IF NOT EXISTS (
    SELECT * FROM [panel].[__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261013050730_AddSchemaComments'
)
BEGIN
    DECLARE @description AS sql_variant;
    SET @description = N'پرسنل';
    EXEC sp_addextendedproperty 'MS_Description', @description, 'SCHEMA', N'panel', 'TABLE', N'PersonnelHistory', 'COLUMN', N'PersonnelId';
END;
GO

IF NOT EXISTS (
    SELECT * FROM [panel].[__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261013050730_AddSchemaComments'
)
BEGIN
    DECLARE @description AS sql_variant;
    SET @description = N'وضعیت قبلی';
    EXEC sp_addextendedproperty 'MS_Description', @description, 'SCHEMA', N'panel', 'TABLE', N'PersonnelHistory', 'COLUMN', N'From';
END;
GO

IF NOT EXISTS (
    SELECT * FROM [panel].[__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261013050730_AddSchemaComments'
)
BEGIN
    DECLARE @description AS sql_variant;
    SET @description = N'توضیح';
    EXEC sp_addextendedproperty 'MS_Description', @description, 'SCHEMA', N'panel', 'TABLE', N'PersonnelHistory', 'COLUMN', N'Description';
END;
GO

IF NOT EXISTS (
    SELECT * FROM [panel].[__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261013050730_AddSchemaComments'
)
BEGIN
    DECLARE @description AS sql_variant;
    SET @description = N'عکس فوری کاربر (JSON): شناسه، نام، کد کاربر، نام و کد شعبه';
    EXEC sp_addextendedproperty 'MS_Description', @description, 'SCHEMA', N'panel', 'TABLE', N'PersonnelHistory', 'COLUMN', N'By';
END;
GO

IF NOT EXISTS (
    SELECT * FROM [panel].[__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261013050730_AddSchemaComments'
)
BEGIN
    DECLARE @description AS sql_variant;
    SET @description = N'زمان';
    EXEC sp_addextendedproperty 'MS_Description', @description, 'SCHEMA', N'panel', 'TABLE', N'PersonnelHistory', 'COLUMN', N'At';
END;
GO

IF NOT EXISTS (
    SELECT * FROM [panel].[__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261013050730_AddSchemaComments'
)
BEGIN
    DECLARE @description AS sql_variant;
    SET @description = N'شناسه';
    EXEC sp_addextendedproperty 'MS_Description', @description, 'SCHEMA', N'panel', 'TABLE', N'PersonnelHistory', 'COLUMN', N'Id';
END;
GO

IF NOT EXISTS (
    SELECT * FROM [panel].[__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261013050730_AddSchemaComments'
)
BEGIN
    DECLARE @description AS sql_variant;
    SET @description = N'نسخهٔ همزمانی؛ با هر تغییر +۱ (در API: rowVersion)';
    EXEC sp_addextendedproperty 'MS_Description', @description, 'SCHEMA', N'panel', 'TABLE', N'Personnel', 'COLUMN', N'Version';
END;
GO

IF NOT EXISTS (
    SELECT * FROM [panel].[__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261013050730_AddSchemaComments'
)
BEGIN
    DECLARE @description AS sql_variant;
    SET @description = N'۱ در انتظار مدیر، ۲ تأییدشده، ۳ ردشده، ۴ حذف‌شده';
    EXEC sp_addextendedproperty 'MS_Description', @description, 'SCHEMA', N'panel', 'TABLE', N'Personnel', 'COLUMN', N'Status';
END;
GO

IF NOT EXISTS (
    SELECT * FROM [panel].[__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261013050730_AddSchemaComments'
)
BEGIN
    DECLARE @description AS sql_variant;
    SET @description = N'علت رد';
    EXEC sp_addextendedproperty 'MS_Description', @description, 'SCHEMA', N'panel', 'TABLE', N'Personnel', 'COLUMN', N'RejectReason';
END;
GO

IF NOT EXISTS (
    SELECT * FROM [panel].[__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261013050730_AddSchemaComments'
)
BEGIN
    DECLARE @description AS sql_variant;
    SET @description = N'عکس فوری کاربر (JSON): شناسه، نام، کد کاربر، نام و کد شعبه';
    EXEC sp_addextendedproperty 'MS_Description', @description, 'SCHEMA', N'panel', 'TABLE', N'Personnel', 'COLUMN', N'Registrar';
END;
GO

IF NOT EXISTS (
    SELECT * FROM [panel].[__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261013050730_AddSchemaComments'
)
BEGIN
    DECLARE @description AS sql_variant;
    SET @description = N'اولویت تخصیص (۱ تا ۹۹۹)';
    EXEC sp_addextendedproperty 'MS_Description', @description, 'SCHEMA', N'panel', 'TABLE', N'Personnel', 'COLUMN', N'Priority';
END;
GO

IF NOT EXISTS (
    SELECT * FROM [panel].[__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261013050730_AddSchemaComments'
)
BEGIN
    DECLARE @description AS sql_variant;
    SET @description = N'کد پرسنلی';
    EXEC sp_addextendedproperty 'MS_Description', @description, 'SCHEMA', N'panel', 'TABLE', N'Personnel', 'COLUMN', N'PersonnelCode';
END;
GO

IF NOT EXISTS (
    SELECT * FROM [panel].[__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261013050730_AddSchemaComments'
)
BEGIN
    DECLARE @description AS sql_variant;
    SET @description = N'کد ملی؛ در هر شرکت یکتا';
    EXEC sp_addextendedproperty 'MS_Description', @description, 'SCHEMA', N'panel', 'TABLE', N'Personnel', 'COLUMN', N'NationalCode';
END;
GO

IF NOT EXISTS (
    SELECT * FROM [panel].[__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261013050730_AddSchemaComments'
)
BEGIN
    DECLARE @description AS sql_variant;
    SET @description = N'موبایل (۰۹…)';
    EXEC sp_addextendedproperty 'MS_Description', @description, 'SCHEMA', N'panel', 'TABLE', N'Personnel', 'COLUMN', N'Mobile';
END;
GO

IF NOT EXISTS (
    SELECT * FROM [panel].[__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261013050730_AddSchemaComments'
)
BEGIN
    DECLARE @description AS sql_variant;
    SET @description = N'اگر از فایل اکسل آمده';
    EXEC sp_addextendedproperty 'MS_Description', @description, 'SCHEMA', N'panel', 'TABLE', N'Personnel', 'COLUMN', N'ImportBatchId';
END;
GO

IF NOT EXISTS (
    SELECT * FROM [panel].[__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261013050730_AddSchemaComments'
)
BEGIN
    DECLARE @description AS sql_variant;
    SET @description = N'تاریخ استخدام (آینده ممنوع)';
    EXEC sp_addextendedproperty 'MS_Description', @description, 'SCHEMA', N'panel', 'TABLE', N'Personnel', 'COLUMN', N'HireDate';
END;
GO

IF NOT EXISTS (
    SELECT * FROM [panel].[__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261013050730_AddSchemaComments'
)
BEGIN
    DECLARE @description AS sql_variant;
    SET @description = N'نام و نام خانوادگی';
    EXEC sp_addextendedproperty 'MS_Description', @description, 'SCHEMA', N'panel', 'TABLE', N'Personnel', 'COLUMN', N'FullName';
END;
GO

IF NOT EXISTS (
    SELECT * FROM [panel].[__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261013050730_AddSchemaComments'
)
BEGIN
    DECLARE @description AS sql_variant;
    SET @description = N'فایل مستند حذف';
    EXEC sp_addextendedproperty 'MS_Description', @description, 'SCHEMA', N'panel', 'TABLE', N'Personnel', 'COLUMN', N'DeleteFileId';
END;
GO

IF NOT EXISTS (
    SELECT * FROM [panel].[__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261013050730_AddSchemaComments'
)
BEGIN
    DECLARE @description AS sql_variant;
    SET @description = N'توضیح حذف';
    EXEC sp_addextendedproperty 'MS_Description', @description, 'SCHEMA', N'panel', 'TABLE', N'Personnel', 'COLUMN', N'DeleteDescription';
END;
GO

IF NOT EXISTS (
    SELECT * FROM [panel].[__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261013050730_AddSchemaComments'
)
BEGIN
    DECLARE @description AS sql_variant;
    SET @description = N'زمان تصمیم مدیر';
    EXEC sp_addextendedproperty 'MS_Description', @description, 'SCHEMA', N'panel', 'TABLE', N'Personnel', 'COLUMN', N'DecidedAt';
END;
GO

IF NOT EXISTS (
    SELECT * FROM [panel].[__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261013050730_AddSchemaComments'
)
BEGIN
    DECLARE @description AS sql_variant;
    SET @description = N'زمان معرفی';
    EXEC sp_addextendedproperty 'MS_Description', @description, 'SCHEMA', N'panel', 'TABLE', N'Personnel', 'COLUMN', N'CreatedAt';
END;
GO

IF NOT EXISTS (
    SELECT * FROM [panel].[__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261013050730_AddSchemaComments'
)
BEGIN
    DECLARE @description AS sql_variant;
    SET @description = N'شرکت';
    EXEC sp_addextendedproperty 'MS_Description', @description, 'SCHEMA', N'panel', 'TABLE', N'Personnel', 'COLUMN', N'CompanyId';
END;
GO

IF NOT EXISTS (
    SELECT * FROM [panel].[__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261013050730_AddSchemaComments'
)
BEGIN
    DECLARE @description AS sql_variant;
    SET @description = N'شناسه';
    EXEC sp_addextendedproperty 'MS_Description', @description, 'SCHEMA', N'panel', 'TABLE', N'Personnel', 'COLUMN', N'Id';
END;
GO

IF NOT EXISTS (
    SELECT * FROM [panel].[__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261013050730_AddSchemaComments'
)
BEGIN
    DECLARE @description AS sql_variant;
    SET @description = N'عکس فوری کاربر (JSON): شناسه، نام، کد کاربر، نام و کد شعبه';
    EXEC sp_addextendedproperty 'MS_Description', @description, 'SCHEMA', N'panel', 'TABLE', N'MonthlyBalances', 'COLUMN', N'EditedBy';
END;
GO

IF NOT EXISTS (
    SELECT * FROM [panel].[__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261013050730_AddSchemaComments'
)
BEGIN
    DECLARE @description AS sql_variant;
    SET @description = N'زمان ثبت/ویرایش';
    EXEC sp_addextendedproperty 'MS_Description', @description, 'SCHEMA', N'panel', 'TABLE', N'MonthlyBalances', 'COLUMN', N'EditedAt';
END;
GO

IF NOT EXISTS (
    SELECT * FROM [panel].[__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261013050730_AddSchemaComments'
)
BEGIN
    DECLARE @description AS sql_variant;
    SET @description = N'میانگین موجودی (ریال)';
    EXEC sp_addextendedproperty 'MS_Description', @description, 'SCHEMA', N'panel', 'TABLE', N'MonthlyBalances', 'COLUMN', N'Amount';
END;
GO

IF NOT EXISTS (
    SELECT * FROM [panel].[__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261013050730_AddSchemaComments'
)
BEGIN
    DECLARE @description AS sql_variant;
    SET @description = N'ماه شمسی، مثل 1405/07';
    EXEC sp_addextendedproperty 'MS_Description', @description, 'SCHEMA', N'panel', 'TABLE', N'MonthlyBalances', 'COLUMN', N'Month';
END;
GO

IF NOT EXISTS (
    SELECT * FROM [panel].[__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261013050730_AddSchemaComments'
)
BEGIN
    DECLARE @description AS sql_variant;
    SET @description = N'شرکت';
    EXEC sp_addextendedproperty 'MS_Description', @description, 'SCHEMA', N'panel', 'TABLE', N'MonthlyBalances', 'COLUMN', N'CompanyId';
END;
GO

IF NOT EXISTS (
    SELECT * FROM [panel].[__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261013050730_AddSchemaComments'
)
BEGIN
    DECLARE @description AS sql_variant;
    SET @description = N'نسخهٔ همزمانی؛ با هر تغییر +۱ (در API: rowVersion)';
    EXEC sp_addextendedproperty 'MS_Description', @description, 'SCHEMA', N'panel', 'TABLE', N'LoanRequests', 'COLUMN', N'Version';
END;
GO

IF NOT EXISTS (
    SELECT * FROM [panel].[__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261013050730_AddSchemaComments'
)
BEGIN
    DECLARE @description AS sql_variant;
    SET @description = N'۱ انتظار مدیر، ۲ رد مدیر، ۳ انتظار اعتبارات، ۴ رد اعتبارات، ۵ انتظار دیجیتال، ۶ رد دیجیتال، ۷ تأییدشده، ۸ لغوشده';
    EXEC sp_addextendedproperty 'MS_Description', @description, 'SCHEMA', N'panel', 'TABLE', N'LoanRequests', 'COLUMN', N'Status';
END;
GO

IF NOT EXISTS (
    SELECT * FROM [panel].[__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261013050730_AddSchemaComments'
)
BEGIN
    DECLARE @description AS sql_variant;
    SET @description = N'عکس فوری کاربر (JSON): شناسه، نام، کد کاربر، نام و کد شعبه';
    EXEC sp_addextendedproperty 'MS_Description', @description, 'SCHEMA', N'panel', 'TABLE', N'LoanRequests', 'COLUMN', N'Registrar';
END;
GO

IF NOT EXISTS (
    SELECT * FROM [panel].[__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261013050730_AddSchemaComments'
)
BEGIN
    DECLARE @description AS sql_variant;
    SET @description = N'پس از تأیید: ۱ تشکیل پرونده، ۲ دریافت از امید بانک، ۳ دریافت در شعبه، ۴ عدم اعطای غیرحضوری';
    EXEC sp_addextendedproperty 'MS_Description', @description, 'SCHEMA', N'panel', 'TABLE', N'LoanRequests', 'COLUMN', N'PostStage';
END;
GO

IF NOT EXISTS (
    SELECT * FROM [panel].[__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261013050730_AddSchemaComments'
)
BEGIN
    DECLARE @description AS sql_variant;
    SET @description = N'طرح (کپی از ثبت)';
    EXEC sp_addextendedproperty 'MS_Description', @description, 'SCHEMA', N'panel', 'TABLE', N'LoanRequests', 'COLUMN', N'PlanId';
END;
GO

IF NOT EXISTS (
    SELECT * FROM [panel].[__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261013050730_AddSchemaComments'
)
BEGIN
    DECLARE @description AS sql_variant;
    SET @description = N'پرسنل';
    EXEC sp_addextendedproperty 'MS_Description', @description, 'SCHEMA', N'panel', 'TABLE', N'LoanRequests', 'COLUMN', N'PersonnelId';
END;
GO

IF NOT EXISTS (
    SELECT * FROM [panel].[__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261013050730_AddSchemaComments'
)
BEGIN
    DECLARE @description AS sql_variant;
    SET @description = N'ترتیب پرداخت (پیش‌فرض اولویت پرسنل)';
    EXEC sp_addextendedproperty 'MS_Description', @description, 'SCHEMA', N'panel', 'TABLE', N'LoanRequests', 'COLUMN', N'PaymentOrder';
END;
GO

IF NOT EXISTS (
    SELECT * FROM [panel].[__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261013050730_AddSchemaComments'
)
BEGIN
    DECLARE @description AS sql_variant;
    SET @description = N'دلیل و توضیح رد/لغو';
    EXEC sp_addextendedproperty 'MS_Description', @description, 'SCHEMA', N'panel', 'TABLE', N'LoanRequests', 'COLUMN', N'Description';
END;
GO

IF NOT EXISTS (
    SELECT * FROM [panel].[__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261013050730_AddSchemaComments'
)
BEGIN
    DECLARE @description AS sql_variant;
    SET @description = N'زمان آخرین تصمیم';
    EXEC sp_addextendedproperty 'MS_Description', @description, 'SCHEMA', N'panel', 'TABLE', N'LoanRequests', 'COLUMN', N'DecidedAt';
END;
GO

IF NOT EXISTS (
    SELECT * FROM [panel].[__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261013050730_AddSchemaComments'
)
BEGIN
    DECLARE @description AS sql_variant;
    SET @description = N'زمان ثبت';
    EXEC sp_addextendedproperty 'MS_Description', @description, 'SCHEMA', N'panel', 'TABLE', N'LoanRequests', 'COLUMN', N'CreatedAt';
END;
GO

IF NOT EXISTS (
    SELECT * FROM [panel].[__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261013050730_AddSchemaComments'
)
BEGIN
    DECLARE @description AS sql_variant;
    SET @description = N'شرکت (کپی از ثبت)';
    EXEC sp_addextendedproperty 'MS_Description', @description, 'SCHEMA', N'panel', 'TABLE', N'LoanRequests', 'COLUMN', N'CompanyId';
END;
GO

IF NOT EXISTS (
    SELECT * FROM [panel].[__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261013050730_AddSchemaComments'
)
BEGIN
    DECLARE @description AS sql_variant;
    SET @description = N'کپی شعبهٔ شرکت، برای فیلتر دامنه بدون Join';
    EXEC sp_addextendedproperty 'MS_Description', @description, 'SCHEMA', N'panel', 'TABLE', N'LoanRequests', 'COLUMN', N'BranchCode';
END;
GO

IF NOT EXISTS (
    SELECT * FROM [panel].[__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261013050730_AddSchemaComments'
)
BEGIN
    DECLARE @description AS sql_variant;
    SET @description = N'ثبت';
    EXEC sp_addextendedproperty 'MS_Description', @description, 'SCHEMA', N'panel', 'TABLE', N'LoanRequests', 'COLUMN', N'BatchId';
END;
GO

IF NOT EXISTS (
    SELECT * FROM [panel].[__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261013050730_AddSchemaComments'
)
BEGIN
    DECLARE @description AS sql_variant;
    SET @description = N'مبلغ (۱۰ میلیون تا ۴ میلیارد ریال)';
    EXEC sp_addextendedproperty 'MS_Description', @description, 'SCHEMA', N'panel', 'TABLE', N'LoanRequests', 'COLUMN', N'Amount';
END;
GO

IF NOT EXISTS (
    SELECT * FROM [panel].[__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261013050730_AddSchemaComments'
)
BEGIN
    DECLARE @description AS sql_variant;
    SET @description = N'شناسهٔ درخواست';
    EXEC sp_addextendedproperty 'MS_Description', @description, 'SCHEMA', N'panel', 'TABLE', N'LoanRequests', 'COLUMN', N'Id';
END;
GO

IF NOT EXISTS (
    SELECT * FROM [panel].[__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261013050730_AddSchemaComments'
)
BEGIN
    DECLARE @description AS sql_variant;
    SET @description = N'وضعیت جدید';
    EXEC sp_addextendedproperty 'MS_Description', @description, 'SCHEMA', N'panel', 'TABLE', N'LoanRequestHistory', 'COLUMN', N'To';
END;
GO

IF NOT EXISTS (
    SELECT * FROM [panel].[__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261013050730_AddSchemaComments'
)
BEGIN
    DECLARE @description AS sql_variant;
    SET @description = N'مرحلهٔ پس از تأیید (اگر تغییر مرحله بوده)';
    EXEC sp_addextendedproperty 'MS_Description', @description, 'SCHEMA', N'panel', 'TABLE', N'LoanRequestHistory', 'COLUMN', N'Stage';
END;
GO

IF NOT EXISTS (
    SELECT * FROM [panel].[__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261013050730_AddSchemaComments'
)
BEGIN
    DECLARE @description AS sql_variant;
    SET @description = N'درخواست';
    EXEC sp_addextendedproperty 'MS_Description', @description, 'SCHEMA', N'panel', 'TABLE', N'LoanRequestHistory', 'COLUMN', N'RequestId';
END;
GO

IF NOT EXISTS (
    SELECT * FROM [panel].[__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261013050730_AddSchemaComments'
)
BEGIN
    DECLARE @description AS sql_variant;
    SET @description = N'دلیل';
    EXEC sp_addextendedproperty 'MS_Description', @description, 'SCHEMA', N'panel', 'TABLE', N'LoanRequestHistory', 'COLUMN', N'Reason';
END;
GO

IF NOT EXISTS (
    SELECT * FROM [panel].[__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261013050730_AddSchemaComments'
)
BEGIN
    DECLARE @description AS sql_variant;
    SET @description = N'وضعیت قبلی';
    EXEC sp_addextendedproperty 'MS_Description', @description, 'SCHEMA', N'panel', 'TABLE', N'LoanRequestHistory', 'COLUMN', N'From';
END;
GO

IF NOT EXISTS (
    SELECT * FROM [panel].[__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261013050730_AddSchemaComments'
)
BEGIN
    DECLARE @description AS sql_variant;
    SET @description = N'توضیح';
    EXEC sp_addextendedproperty 'MS_Description', @description, 'SCHEMA', N'panel', 'TABLE', N'LoanRequestHistory', 'COLUMN', N'Description';
END;
GO

IF NOT EXISTS (
    SELECT * FROM [panel].[__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261013050730_AddSchemaComments'
)
BEGIN
    DECLARE @description AS sql_variant;
    SET @description = N'عکس فوری کاربر (JSON): شناسه، نام، کد کاربر، نام و کد شعبه';
    EXEC sp_addextendedproperty 'MS_Description', @description, 'SCHEMA', N'panel', 'TABLE', N'LoanRequestHistory', 'COLUMN', N'By';
END;
GO

IF NOT EXISTS (
    SELECT * FROM [panel].[__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261013050730_AddSchemaComments'
)
BEGIN
    DECLARE @description AS sql_variant;
    SET @description = N'زمان';
    EXEC sp_addextendedproperty 'MS_Description', @description, 'SCHEMA', N'panel', 'TABLE', N'LoanRequestHistory', 'COLUMN', N'At';
END;
GO

IF NOT EXISTS (
    SELECT * FROM [panel].[__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261013050730_AddSchemaComments'
)
BEGIN
    DECLARE @description AS sql_variant;
    SET @description = N'شناسه';
    EXEC sp_addextendedproperty 'MS_Description', @description, 'SCHEMA', N'panel', 'TABLE', N'LoanRequestHistory', 'COLUMN', N'Id';
END;
GO

IF NOT EXISTS (
    SELECT * FROM [panel].[__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261013050730_AddSchemaComments'
)
BEGIN
    DECLARE @description AS sql_variant;
    SET @description = N'فایل (یک فایل می‌تواند پیوست چند درخواست گروهی باشد)';
    EXEC sp_addextendedproperty 'MS_Description', @description, 'SCHEMA', N'panel', 'TABLE', N'LoanRequestDocuments', 'COLUMN', N'FileId';
END;
GO

IF NOT EXISTS (
    SELECT * FROM [panel].[__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261013050730_AddSchemaComments'
)
BEGIN
    DECLARE @description AS sql_variant;
    SET @description = N'درخواست';
    EXEC sp_addextendedproperty 'MS_Description', @description, 'SCHEMA', N'panel', 'TABLE', N'LoanRequestDocuments', 'COLUMN', N'RequestId';
END;
GO

IF NOT EXISTS (
    SELECT * FROM [panel].[__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261013050730_AddSchemaComments'
)
BEGIN
    DECLARE @description AS sql_variant;
    SET @description = N'عکس فوری کاربر (JSON): شناسه، نام، کد کاربر، نام و کد شعبه';
    EXEC sp_addextendedproperty 'MS_Description', @description, 'SCHEMA', N'panel', 'TABLE', N'LoanBatches', 'COLUMN', N'Registrar';
END;
GO

IF NOT EXISTS (
    SELECT * FROM [panel].[__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261013050730_AddSchemaComments'
)
BEGIN
    DECLARE @description AS sql_variant;
    SET @description = N'طرح';
    EXEC sp_addextendedproperty 'MS_Description', @description, 'SCHEMA', N'panel', 'TABLE', N'LoanBatches', 'COLUMN', N'PlanId';
END;
GO

IF NOT EXISTS (
    SELECT * FROM [panel].[__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261013050730_AddSchemaComments'
)
BEGIN
    DECLARE @description AS sql_variant;
    SET @description = N'۱ مبلغ یکسان، ۲ مبلغ جدا برای هر نفر';
    EXEC sp_addextendedproperty 'MS_Description', @description, 'SCHEMA', N'panel', 'TABLE', N'LoanBatches', 'COLUMN', N'Mode';
END;
GO

IF NOT EXISTS (
    SELECT * FROM [panel].[__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261013050730_AddSchemaComments'
)
BEGIN
    DECLARE @description AS sql_variant;
    SET @description = N'توضیح';
    EXEC sp_addextendedproperty 'MS_Description', @description, 'SCHEMA', N'panel', 'TABLE', N'LoanBatches', 'COLUMN', N'Description';
END;
GO

IF NOT EXISTS (
    SELECT * FROM [panel].[__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261013050730_AddSchemaComments'
)
BEGIN
    DECLARE @description AS sql_variant;
    SET @description = N'زمان ثبت';
    EXEC sp_addextendedproperty 'MS_Description', @description, 'SCHEMA', N'panel', 'TABLE', N'LoanBatches', 'COLUMN', N'CreatedAt';
END;
GO

IF NOT EXISTS (
    SELECT * FROM [panel].[__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261013050730_AddSchemaComments'
)
BEGIN
    DECLARE @description AS sql_variant;
    SET @description = N'شرکت';
    EXEC sp_addextendedproperty 'MS_Description', @description, 'SCHEMA', N'panel', 'TABLE', N'LoanBatches', 'COLUMN', N'CompanyId';
END;
GO

IF NOT EXISTS (
    SELECT * FROM [panel].[__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261013050730_AddSchemaComments'
)
BEGIN
    DECLARE @description AS sql_variant;
    SET @description = N'کپی شعبهٔ شرکت';
    EXEC sp_addextendedproperty 'MS_Description', @description, 'SCHEMA', N'panel', 'TABLE', N'LoanBatches', 'COLUMN', N'BranchCode';
END;
GO

IF NOT EXISTS (
    SELECT * FROM [panel].[__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261013050730_AddSchemaComments'
)
BEGIN
    DECLARE @description AS sql_variant;
    SET @description = N'تاریخ مبنا (فقط یک ماه گذشته)';
    EXEC sp_addextendedproperty 'MS_Description', @description, 'SCHEMA', N'panel', 'TABLE', N'LoanBatches', 'COLUMN', N'BaseDate';
END;
GO

IF NOT EXISTS (
    SELECT * FROM [panel].[__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261013050730_AddSchemaComments'
)
BEGIN
    DECLARE @description AS sql_variant;
    SET @description = N'شناسهٔ ثبت';
    EXEC sp_addextendedproperty 'MS_Description', @description, 'SCHEMA', N'panel', 'TABLE', N'LoanBatches', 'COLUMN', N'Id';
END;
GO

IF NOT EXISTS (
    SELECT * FROM [panel].[__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261013050730_AddSchemaComments'
)
BEGIN
    DECLARE @description AS sql_variant;
    SET @description = N'بارگذارکننده';
    EXEC sp_addextendedproperty 'MS_Description', @description, 'SCHEMA', N'panel', 'TABLE', N'Files', 'COLUMN', N'UploadedBy';
END;
GO

IF NOT EXISTS (
    SELECT * FROM [panel].[__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261013050730_AddSchemaComments'
)
BEGIN
    DECLARE @description AS sql_variant;
    SET @description = N'زمان بارگذاری';
    EXEC sp_addextendedproperty 'MS_Description', @description, 'SCHEMA', N'panel', 'TABLE', N'Files', 'COLUMN', N'UploadedAt';
END;
GO

IF NOT EXISTS (
    SELECT * FROM [panel].[__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261013050730_AddSchemaComments'
)
BEGIN
    DECLARE @description AS sql_variant;
    SET @description = N'کلید Minio: facility-panel/yyyy/MM/…';
    EXEC sp_addextendedproperty 'MS_Description', @description, 'SCHEMA', N'panel', 'TABLE', N'Files', 'COLUMN', N'StorageKey';
END;
GO

IF NOT EXISTS (
    SELECT * FROM [panel].[__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261013050730_AddSchemaComments'
)
BEGIN
    DECLARE @description AS sql_variant;
    SET @description = N'حجم (بایت)';
    EXEC sp_addextendedproperty 'MS_Description', @description, 'SCHEMA', N'panel', 'TABLE', N'Files', 'COLUMN', N'Size';
END;
GO

IF NOT EXISTS (
    SELECT * FROM [panel].[__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261013050730_AddSchemaComments'
)
BEGIN
    DECLARE @description AS sql_variant;
    SET @description = N'نام اصلی';
    EXEC sp_addextendedproperty 'MS_Description', @description, 'SCHEMA', N'panel', 'TABLE', N'Files', 'COLUMN', N'FileName';
END;
GO

IF NOT EXISTS (
    SELECT * FROM [panel].[__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261013050730_AddSchemaComments'
)
BEGIN
    DECLARE @description AS sql_variant;
    SET @description = N'نوع فایل';
    EXEC sp_addextendedproperty 'MS_Description', @description, 'SCHEMA', N'panel', 'TABLE', N'Files', 'COLUMN', N'ContentType';
END;
GO

IF NOT EXISTS (
    SELECT * FROM [panel].[__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261013050730_AddSchemaComments'
)
BEGIN
    DECLARE @description AS sql_variant;
    SET @description = N'شناسه';
    EXEC sp_addextendedproperty 'MS_Description', @description, 'SCHEMA', N'panel', 'TABLE', N'Files', 'COLUMN', N'Id';
END;
GO

IF NOT EXISTS (
    SELECT * FROM [panel].[__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261013050730_AddSchemaComments'
)
BEGIN
    DECLARE @description AS sql_variant;
    SET @description = N'طرح';
    EXEC sp_addextendedproperty 'MS_Description', @description, 'SCHEMA', N'panel', 'TABLE', N'CompanyPlans', 'COLUMN', N'PlanId';
END;
GO

IF NOT EXISTS (
    SELECT * FROM [panel].[__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261013050730_AddSchemaComments'
)
BEGIN
    DECLARE @description AS sql_variant;
    SET @description = N'شرکت';
    EXEC sp_addextendedproperty 'MS_Description', @description, 'SCHEMA', N'panel', 'TABLE', N'CompanyPlans', 'COLUMN', N'CompanyId';
END;
GO

IF NOT EXISTS (
    SELECT * FROM [panel].[__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261013050730_AddSchemaComments'
)
BEGIN
    DECLARE @description AS sql_variant;
    SET @description = N'عنوان دلیل در لحظهٔ لغو';
    EXEC sp_addextendedproperty 'MS_Description', @description, 'SCHEMA', N'panel', 'TABLE', N'CompanyCancellations', 'COLUMN', N'ReasonTitle';
END;
GO

IF NOT EXISTS (
    SELECT * FROM [panel].[__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261013050730_AddSchemaComments'
)
BEGIN
    DECLARE @description AS sql_variant;
    SET @description = N'دلیل از فهرست (نوع ۱)';
    EXEC sp_addextendedproperty 'MS_Description', @description, 'SCHEMA', N'panel', 'TABLE', N'CompanyCancellations', 'COLUMN', N'ReasonId';
END;
GO

IF NOT EXISTS (
    SELECT * FROM [panel].[__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261013050730_AddSchemaComments'
)
BEGIN
    DECLARE @description AS sql_variant;
    SET @description = N'فایل نامهٔ لغو (الزامی)';
    EXEC sp_addextendedproperty 'MS_Description', @description, 'SCHEMA', N'panel', 'TABLE', N'CompanyCancellations', 'COLUMN', N'LetterFileId';
END;
GO

IF NOT EXISTS (
    SELECT * FROM [panel].[__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261013050730_AddSchemaComments'
)
BEGIN
    DECLARE @description AS sql_variant;
    SET @description = N'توضیح (۵ تا ۱۰۰۰)';
    EXEC sp_addextendedproperty 'MS_Description', @description, 'SCHEMA', N'panel', 'TABLE', N'CompanyCancellations', 'COLUMN', N'Description';
END;
GO

IF NOT EXISTS (
    SELECT * FROM [panel].[__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261013050730_AddSchemaComments'
)
BEGIN
    DECLARE @description AS sql_variant;
    SET @description = N'عکس فوری کاربر (JSON): شناسه، نام، کد کاربر، نام و کد شعبه';
    EXEC sp_addextendedproperty 'MS_Description', @description, 'SCHEMA', N'panel', 'TABLE', N'CompanyCancellations', 'COLUMN', N'CancelledBy';
END;
GO

IF NOT EXISTS (
    SELECT * FROM [panel].[__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261013050730_AddSchemaComments'
)
BEGIN
    DECLARE @description AS sql_variant;
    SET @description = N'زمان ثبت لغو';
    EXEC sp_addextendedproperty 'MS_Description', @description, 'SCHEMA', N'panel', 'TABLE', N'CompanyCancellations', 'COLUMN', N'CancelledAt';
END;
GO

IF NOT EXISTS (
    SELECT * FROM [panel].[__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261013050730_AddSchemaComments'
)
BEGIN
    DECLARE @description AS sql_variant;
    SET @description = N'تاریخ لغو (۱۴۰۵/۰۱/۰۱ تا امروز)';
    EXEC sp_addextendedproperty 'MS_Description', @description, 'SCHEMA', N'panel', 'TABLE', N'CompanyCancellations', 'COLUMN', N'CancelDate';
END;
GO

IF NOT EXISTS (
    SELECT * FROM [panel].[__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261013050730_AddSchemaComments'
)
BEGIN
    DECLARE @description AS sql_variant;
    SET @description = N'شرکت (۱ به ۰..۱)';
    EXEC sp_addextendedproperty 'MS_Description', @description, 'SCHEMA', N'panel', 'TABLE', N'CompanyCancellations', 'COLUMN', N'CompanyId';
END;
GO

IF NOT EXISTS (
    SELECT * FROM [panel].[__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261013050730_AddSchemaComments'
)
BEGIN
    DECLARE @description AS sql_variant;
    SET @description = N'نسخهٔ همزمانی؛ با هر تغییر +۱ (در API: rowVersion)';
    EXEC sp_addextendedproperty 'MS_Description', @description, 'SCHEMA', N'panel', 'TABLE', N'Companies', 'COLUMN', N'Version';
END;
GO

IF NOT EXISTS (
    SELECT * FROM [panel].[__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261013050730_AddSchemaComments'
)
BEGIN
    DECLARE @description AS sql_variant;
    SET @description = N'۱ فعال، ۲ لغو قرارداد، ۳ منقضی، ۴ حذف‌شده';
    EXEC sp_addextendedproperty 'MS_Description', @description, 'SCHEMA', N'panel', 'TABLE', N'Companies', 'COLUMN', N'Status';
END;
GO

IF NOT EXISTS (
    SELECT * FROM [panel].[__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261013050730_AddSchemaComments'
)
BEGIN
    DECLARE @description AS sql_variant;
    SET @description = N'طرح کلان';
    EXEC sp_addextendedproperty 'MS_Description', @description, 'SCHEMA', N'panel', 'TABLE', N'Companies', 'COLUMN', N'SchemeId';
END;
GO

IF NOT EXISTS (
    SELECT * FROM [panel].[__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261013050730_AddSchemaComments'
)
BEGIN
    DECLARE @description AS sql_variant;
    SET @description = N'حقوق پرسنل از بانک سپه؟';
    EXEC sp_addextendedproperty 'MS_Description', @description, 'SCHEMA', N'panel', 'TABLE', N'Companies', 'COLUMN', N'SalaryFromSepah';
END;
GO

IF NOT EXISTS (
    SELECT * FROM [panel].[__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261013050730_AddSchemaComments'
)
BEGIN
    DECLARE @description AS sql_variant;
    SET @description = N'عکس فوری کاربر (JSON): شناسه، نام، کد کاربر، نام و کد شعبه';
    EXEC sp_addextendedproperty 'MS_Description', @description, 'SCHEMA', N'panel', 'TABLE', N'Companies', 'COLUMN', N'Registrar';
END;
GO

IF NOT EXISTS (
    SELECT * FROM [panel].[__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261013050730_AddSchemaComments'
)
BEGIN
    DECLARE @description AS sql_variant;
    SET @description = N'شناسهٔ ملی ۱۱ رقمی؛ بین شرکت‌های حذف‌نشده یکتا';
    EXEC sp_addextendedproperty 'MS_Description', @description, 'SCHEMA', N'panel', 'TABLE', N'Companies', 'COLUMN', N'NationalId';
END;
GO

IF NOT EXISTS (
    SELECT * FROM [panel].[__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261013050730_AddSchemaComments'
)
BEGIN
    DECLARE @description AS sql_variant;
    SET @description = N'نام شرکت (فارسی، ۳ تا ۱۰۰)';
    EXEC sp_addextendedproperty 'MS_Description', @description, 'SCHEMA', N'panel', 'TABLE', N'Companies', 'COLUMN', N'Name';
END;
GO

IF NOT EXISTS (
    SELECT * FROM [panel].[__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261013050730_AddSchemaComments'
)
BEGIN
    DECLARE @description AS sql_variant;
    SET @description = N'زمان حذف نرم';
    EXEC sp_addextendedproperty 'MS_Description', @description, 'SCHEMA', N'panel', 'TABLE', N'Companies', 'COLUMN', N'DeletedAt';
END;
GO

IF NOT EXISTS (
    SELECT * FROM [panel].[__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261013050730_AddSchemaComments'
)
BEGIN
    DECLARE @description AS sql_variant;
    SET @description = N'توضیح حذف';
    EXEC sp_addextendedproperty 'MS_Description', @description, 'SCHEMA', N'panel', 'TABLE', N'Companies', 'COLUMN', N'DeleteDescription';
END;
GO

IF NOT EXISTS (
    SELECT * FROM [panel].[__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261013050730_AddSchemaComments'
)
BEGIN
    DECLARE @description AS sql_variant;
    SET @description = N'زمان ثبت';
    EXEC sp_addextendedproperty 'MS_Description', @description, 'SCHEMA', N'panel', 'TABLE', N'Companies', 'COLUMN', N'CreatedAt';
END;
GO

IF NOT EXISTS (
    SELECT * FROM [panel].[__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261013050730_AddSchemaComments'
)
BEGIN
    DECLARE @description AS sql_variant;
    SET @description = N'تاریخ انعقاد قرارداد';
    EXEC sp_addextendedproperty 'MS_Description', @description, 'SCHEMA', N'panel', 'TABLE', N'Companies', 'COLUMN', N'ContractDate';
END;
GO

IF NOT EXISTS (
    SELECT * FROM [panel].[__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261013050730_AddSchemaComments'
)
BEGIN
    DECLARE @description AS sql_variant;
    SET @description = N'شعبهٔ شرکت (دامنهٔ کاربر شعبه)';
    EXEC sp_addextendedproperty 'MS_Description', @description, 'SCHEMA', N'panel', 'TABLE', N'Companies', 'COLUMN', N'BranchCode';
END;
GO

IF NOT EXISTS (
    SELECT * FROM [panel].[__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261013050730_AddSchemaComments'
)
BEGIN
    DECLARE @description AS sql_variant;
    SET @description = N'نسخهٔ جدای جدول میانگین‌ها';
    EXEC sp_addextendedproperty 'MS_Description', @description, 'SCHEMA', N'panel', 'TABLE', N'Companies', 'COLUMN', N'BalancesVersion';
END;
GO

IF NOT EXISTS (
    SELECT * FROM [panel].[__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261013050730_AddSchemaComments'
)
BEGIN
    DECLARE @description AS sql_variant;
    SET @description = N'شمارهٔ تفاهم‌نامه';
    EXEC sp_addextendedproperty 'MS_Description', @description, 'SCHEMA', N'panel', 'TABLE', N'Companies', 'COLUMN', N'AgreementNumber';
END;
GO

IF NOT EXISTS (
    SELECT * FROM [panel].[__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261013050730_AddSchemaComments'
)
BEGIN
    DECLARE @description AS sql_variant;
    SET @description = N'فایل تفاهم‌نامه';
    EXEC sp_addextendedproperty 'MS_Description', @description, 'SCHEMA', N'panel', 'TABLE', N'Companies', 'COLUMN', N'AgreementFileId';
END;
GO

IF NOT EXISTS (
    SELECT * FROM [panel].[__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261013050730_AddSchemaComments'
)
BEGIN
    DECLARE @description AS sql_variant;
    SET @description = N'افتتاح حساب سپرده؛ روزِ ماهش = روز مجاز انتقال اعتبار';
    EXEC sp_addextendedproperty 'MS_Description', @description, 'SCHEMA', N'panel', 'TABLE', N'Companies', 'COLUMN', N'AccountOpeningDate';
END;
GO

IF NOT EXISTS (
    SELECT * FROM [panel].[__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261013050730_AddSchemaComments'
)
BEGIN
    DECLARE @description AS sql_variant;
    SET @description = N'شمارهٔ حساب جاری (۸ تا ۲۴ رقم)';
    EXEC sp_addextendedproperty 'MS_Description', @description, 'SCHEMA', N'panel', 'TABLE', N'Companies', 'COLUMN', N'AccountNumber';
END;
GO

IF NOT EXISTS (
    SELECT * FROM [panel].[__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261013050730_AddSchemaComments'
)
BEGIN
    DECLARE @description AS sql_variant;
    SET @description = N'شناسهٔ شرکت (در Identity = ScopeId دامنهٔ شرکت)';
    EXEC sp_addextendedproperty 'MS_Description', @description, 'SCHEMA', N'panel', 'TABLE', N'Companies', 'COLUMN', N'Id';
END;
GO

IF NOT EXISTS (
    SELECT * FROM [panel].[__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261013050730_AddSchemaComments'
)
BEGIN
    DECLARE @description AS sql_variant;
    SET @description = N'کاربر پنل در Identity (بدون FK؛ سرویس دیگر)';
    EXEC sp_addextendedproperty 'MS_Description', @description, 'SCHEMA', N'panel', 'TABLE', N'ChangeLogs', 'COLUMN', N'UserId';
END;
GO

IF NOT EXISTS (
    SELECT * FROM [panel].[__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261013050730_AddSchemaComments'
)
BEGIN
    DECLARE @description AS sql_variant;
    SET @description = N'عکس فوری کاربر (JSON): شناسه، نام، کد کاربر، نام و کد شعبه';
    EXEC sp_addextendedproperty 'MS_Description', @description, 'SCHEMA', N'panel', 'TABLE', N'ChangeLogs', 'COLUMN', N'User';
END;
GO

IF NOT EXISTS (
    SELECT * FROM [panel].[__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261013050730_AddSchemaComments'
)
BEGIN
    DECLARE @description AS sql_variant;
    SET @description = N'زمان بازگردانی';
    EXEC sp_addextendedproperty 'MS_Description', @description, 'SCHEMA', N'panel', 'TABLE', N'ChangeLogs', 'COLUMN', N'RestoredAt';
END;
GO

IF NOT EXISTS (
    SELECT * FROM [panel].[__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261013050730_AddSchemaComments'
)
BEGIN
    DECLARE @description AS sql_variant;
    SET @description = N'دادهٔ لازم برای بازگردانی';
    EXEC sp_addextendedproperty 'MS_Description', @description, 'SCHEMA', N'panel', 'TABLE', N'ChangeLogs', 'COLUMN', N'RestorePayload';
END;
GO

IF NOT EXISTS (
    SELECT * FROM [panel].[__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261013050730_AddSchemaComments'
)
BEGIN
    DECLARE @description AS sql_variant;
    SET @description = N'balances یا personnel-delete';
    EXEC sp_addextendedproperty 'MS_Description', @description, 'SCHEMA', N'panel', 'TABLE', N'ChangeLogs', 'COLUMN', N'RestoreKind';
END;
GO

IF NOT EXISTS (
    SELECT * FROM [panel].[__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261013050730_AddSchemaComments'
)
BEGIN
    DECLARE @description AS sql_variant;
    SET @description = N'جزئیات';
    EXEC sp_addextendedproperty 'MS_Description', @description, 'SCHEMA', N'panel', 'TABLE', N'ChangeLogs', 'COLUMN', N'Details';
END;
GO

IF NOT EXISTS (
    SELECT * FROM [panel].[__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261013050730_AddSchemaComments'
)
BEGIN
    DECLARE @description AS sql_variant;
    SET @description = N'شرکت؛ null = تغییر اطلاعات پایه';
    EXEC sp_addextendedproperty 'MS_Description', @description, 'SCHEMA', N'panel', 'TABLE', N'ChangeLogs', 'COLUMN', N'CompanyId';
END;
GO

IF NOT EXISTS (
    SELECT * FROM [panel].[__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261013050730_AddSchemaComments'
)
BEGIN
    DECLARE @description AS sql_variant;
    SET @description = N'۱ شرکت، ۲ پرسنل، ۳ وام، ۴ کاربر';
    EXEC sp_addextendedproperty 'MS_Description', @description, 'SCHEMA', N'panel', 'TABLE', N'ChangeLogs', 'COLUMN', N'Category';
END;
GO

IF NOT EXISTS (
    SELECT * FROM [panel].[__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261013050730_AddSchemaComments'
)
BEGIN
    DECLARE @description AS sql_variant;
    SET @description = N'شعبهٔ شرکت (فیلتر دامنه)';
    EXEC sp_addextendedproperty 'MS_Description', @description, 'SCHEMA', N'panel', 'TABLE', N'ChangeLogs', 'COLUMN', N'BranchCode';
END;
GO

IF NOT EXISTS (
    SELECT * FROM [panel].[__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261013050730_AddSchemaComments'
)
BEGIN
    DECLARE @description AS sql_variant;
    SET @description = N'مقدار قبلی (JSON)';
    EXEC sp_addextendedproperty 'MS_Description', @description, 'SCHEMA', N'panel', 'TABLE', N'ChangeLogs', 'COLUMN', N'Before';
END;
GO

IF NOT EXISTS (
    SELECT * FROM [panel].[__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261013050730_AddSchemaComments'
)
BEGIN
    DECLARE @description AS sql_variant;
    SET @description = N'زمان';
    EXEC sp_addextendedproperty 'MS_Description', @description, 'SCHEMA', N'panel', 'TABLE', N'ChangeLogs', 'COLUMN', N'At';
END;
GO

IF NOT EXISTS (
    SELECT * FROM [panel].[__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261013050730_AddSchemaComments'
)
BEGIN
    DECLARE @description AS sql_variant;
    SET @description = N'مقدار جدید (JSON)';
    EXEC sp_addextendedproperty 'MS_Description', @description, 'SCHEMA', N'panel', 'TABLE', N'ChangeLogs', 'COLUMN', N'After';
END;
GO

IF NOT EXISTS (
    SELECT * FROM [panel].[__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261013050730_AddSchemaComments'
)
BEGIN
    DECLARE @description AS sql_variant;
    SET @description = N'عملیات';
    EXEC sp_addextendedproperty 'MS_Description', @description, 'SCHEMA', N'panel', 'TABLE', N'ChangeLogs', 'COLUMN', N'Action';
END;
GO

IF NOT EXISTS (
    SELECT * FROM [panel].[__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261013050730_AddSchemaComments'
)
BEGIN
    DECLARE @description AS sql_variant;
    SET @description = N'شناسه';
    EXEC sp_addextendedproperty 'MS_Description', @description, 'SCHEMA', N'panel', 'TABLE', N'ChangeLogs', 'COLUMN', N'Id';
END;
GO

IF NOT EXISTS (
    SELECT * FROM [panel].[__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261013050730_AddSchemaComments'
)
BEGIN
    DECLARE @description AS sql_variant;
    SET @description = N'ماه مربوط (اختیاری)';
    EXEC sp_addextendedproperty 'MS_Description', @description, 'SCHEMA', N'panel', 'TABLE', N'BalanceDocuments', 'COLUMN', N'Month';
END;
GO

IF NOT EXISTS (
    SELECT * FROM [panel].[__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261013050730_AddSchemaComments'
)
BEGIN
    DECLARE @description AS sql_variant;
    SET @description = N'فایل';
    EXEC sp_addextendedproperty 'MS_Description', @description, 'SCHEMA', N'panel', 'TABLE', N'BalanceDocuments', 'COLUMN', N'FileId';
END;
GO

IF NOT EXISTS (
    SELECT * FROM [panel].[__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261013050730_AddSchemaComments'
)
BEGIN
    DECLARE @description AS sql_variant;
    SET @description = N'نوع مستند (دلیل نوع ۶: بنکو، نامه، …)';
    EXEC sp_addextendedproperty 'MS_Description', @description, 'SCHEMA', N'panel', 'TABLE', N'BalanceDocuments', 'COLUMN', N'DocumentTypeId';
END;
GO

IF NOT EXISTS (
    SELECT * FROM [panel].[__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261013050730_AddSchemaComments'
)
BEGIN
    DECLARE @description AS sql_variant;
    SET @description = N'توضیح';
    EXEC sp_addextendedproperty 'MS_Description', @description, 'SCHEMA', N'panel', 'TABLE', N'BalanceDocuments', 'COLUMN', N'Description';
END;
GO

IF NOT EXISTS (
    SELECT * FROM [panel].[__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261013050730_AddSchemaComments'
)
BEGIN
    DECLARE @description AS sql_variant;
    SET @description = N'شرکت';
    EXEC sp_addextendedproperty 'MS_Description', @description, 'SCHEMA', N'panel', 'TABLE', N'BalanceDocuments', 'COLUMN', N'CompanyId';
END;
GO

IF NOT EXISTS (
    SELECT * FROM [panel].[__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261013050730_AddSchemaComments'
)
BEGIN
    DECLARE @description AS sql_variant;
    SET @description = N'شناسه';
    EXEC sp_addextendedproperty 'MS_Description', @description, 'SCHEMA', N'panel', 'TABLE', N'BalanceDocuments', 'COLUMN', N'Id';
END;
GO

IF NOT EXISTS (
    SELECT * FROM [panel].[__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261013050730_AddSchemaComments'
)
BEGIN
    INSERT INTO [panel].[__EFMigrationsHistory] ([MigrationId], [ProductVersion])
    VALUES (N'20261013050730_AddSchemaComments', N'8.0.10');
END;
GO

COMMIT;
GO
