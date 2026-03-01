CREATE SCHEMA dbo;

CREATE TABLE [HRM-Demo-1].dbo.Banners (
	Id int IDENTITY(1,1) NOT NULL,
	Place nvarchar(MAX) COLLATE SQL_Latin1_General_CP1_CI_AS NOT NULL,
	[Type] nvarchar(MAX) COLLATE SQL_Latin1_General_CP1_CI_AS NOT NULL,
	[Image] nvarchar(MAX) COLLATE SQL_Latin1_General_CP1_CI_AS NOT NULL,
	Title nvarchar(MAX) COLLATE SQL_Latin1_General_CP1_CI_AS NOT NULL,
	Description nvarchar(MAX) COLLATE SQL_Latin1_General_CP1_CI_AS NULL,
	Alt nvarchar(MAX) COLLATE SQL_Latin1_General_CP1_CI_AS NULL,
	CtaTitle nvarchar(MAX) COLLATE SQL_Latin1_General_CP1_CI_AS NULL,
	LinkTo nvarchar(MAX) COLLATE SQL_Latin1_General_CP1_CI_AS NULL,
	Properties nvarchar(MAX) COLLATE SQL_Latin1_General_CP1_CI_AS NULL,
	IsBlank bit NOT NULL,
	Priority int NOT NULL,
	Expired datetime2 NOT NULL,
	CreatedBy int NULL,
	CreatedName nvarchar(MAX) COLLATE SQL_Latin1_General_CP1_CI_AS NULL,
	CreatedAt datetime2 NULL,
	UpdatedBy int NULL,
	UpdatedName nvarchar(MAX) COLLATE SQL_Latin1_General_CP1_CI_AS NULL,
	UpdatedAt datetime2 NULL,
	IsDeleted bit NULL,
	CONSTRAINT PK_Banners PRIMARY KEY (Id)
);


-- [HRM-Demo-1].dbo.Companies definition

-- Drop table

-- DROP TABLE [HRM-Demo-1].dbo.Companies;

CREATE TABLE [HRM-Demo-1].dbo.Companies (
	Id int IDENTITY(1,1) NOT NULL,
	FullName nvarchar(MAX) COLLATE SQL_Latin1_General_CP1_CI_AS NOT NULL,
	Abbreviation nvarchar(MAX) COLLATE SQL_Latin1_General_CP1_CI_AS NULL,
	TaxCode nvarchar(MAX) COLLATE SQL_Latin1_General_CP1_CI_AS NOT NULL,
	CompanyCode nvarchar(MAX) COLLATE SQL_Latin1_General_CP1_CI_AS NOT NULL,
	IncorporationDate datetime2 NULL,
	LogoPath nvarchar(MAX) COLLATE SQL_Latin1_General_CP1_CI_AS NULL,
	BusinessRegistrationCode nvarchar(MAX) COLLATE SQL_Latin1_General_CP1_CI_AS NULL,
	BusinessRegistrationDate datetime2 NULL,
	IssuingAuthority nvarchar(MAX) COLLATE SQL_Latin1_General_CP1_CI_AS NULL,
	LegalRepresentative nvarchar(MAX) COLLATE SQL_Latin1_General_CP1_CI_AS NULL,
	LegalRepresentativeTitle nvarchar(MAX) COLLATE SQL_Latin1_General_CP1_CI_AS NULL,
	Address nvarchar(MAX) COLLATE SQL_Latin1_General_CP1_CI_AS NULL,
	PhoneNumber nvarchar(MAX) COLLATE SQL_Latin1_General_CP1_CI_AS NULL,
	Fax nvarchar(MAX) COLLATE SQL_Latin1_General_CP1_CI_AS NULL,
	Email nvarchar(MAX) COLLATE SQL_Latin1_General_CP1_CI_AS NULL,
	Website nvarchar(MAX) COLLATE SQL_Latin1_General_CP1_CI_AS NULL,
	CreatedBy int NULL,
	CreatedName nvarchar(MAX) COLLATE SQL_Latin1_General_CP1_CI_AS NULL,
	CreatedAt datetime2 NULL,
	UpdatedBy int NULL,
	UpdatedName nvarchar(MAX) COLLATE SQL_Latin1_General_CP1_CI_AS NULL,
	UpdatedAt datetime2 NULL,
	IsDeleted bit NULL,
	CONSTRAINT PK_Companies PRIMARY KEY (Id)
);


-- [HRM-Demo-1].dbo.ContractDurations definition

-- Drop table

-- DROP TABLE [HRM-Demo-1].dbo.ContractDurations;

CREATE TABLE [HRM-Demo-1].dbo.ContractDurations (
	Id int IDENTITY(1,1) NOT NULL,
	Duration nvarchar(MAX) COLLATE SQL_Latin1_General_CP1_CI_AS NULL,
	ContractDurationStatus bit NULL,
	CreatedBy int NULL,
	CreatedName nvarchar(MAX) COLLATE SQL_Latin1_General_CP1_CI_AS NULL,
	CreatedAt datetime2 NULL,
	UpdatedBy int NULL,
	UpdatedName nvarchar(MAX) COLLATE SQL_Latin1_General_CP1_CI_AS NULL,
	UpdatedAt datetime2 NULL,
	IsDeleted bit NULL,
	CONSTRAINT PK_ContractDurations PRIMARY KEY (Id)
);


-- [HRM-Demo-1].dbo.ContractTypes definition

-- Drop table

-- DROP TABLE [HRM-Demo-1].dbo.ContractTypes;

CREATE TABLE [HRM-Demo-1].dbo.ContractTypes (
	Id int IDENTITY(1,1) NOT NULL,
	Name nvarchar(MAX) COLLATE SQL_Latin1_General_CP1_CI_AS NULL,
	ContractTypeStatus bit NULL,
	CreatedBy int NULL,
	CreatedName nvarchar(MAX) COLLATE SQL_Latin1_General_CP1_CI_AS NULL,
	CreatedAt datetime2 NULL,
	UpdatedBy int NULL,
	UpdatedName nvarchar(MAX) COLLATE SQL_Latin1_General_CP1_CI_AS NULL,
	UpdatedAt datetime2 NULL,
	IsDeleted bit NULL,
	CONSTRAINT PK_ContractTypes PRIMARY KEY (Id)
);


-- [HRM-Demo-1].dbo.Countries definition

-- Drop table

-- DROP TABLE [HRM-Demo-1].dbo.Countries;

CREATE TABLE [HRM-Demo-1].dbo.Countries (
	Id int IDENTITY(1,1) NOT NULL,
	Code nvarchar(MAX) COLLATE SQL_Latin1_General_CP1_CI_AS NULL,
	Name nvarchar(MAX) COLLATE SQL_Latin1_General_CP1_CI_AS NULL,
	Status int NULL,
	IsDeleted int NULL,
	Version int NULL,
	CONSTRAINT PK_Countries PRIMARY KEY (Id)
);


-- [HRM-Demo-1].dbo.DepartmentRoles definition

-- Drop table

-- DROP TABLE [HRM-Demo-1].dbo.DepartmentRoles;

CREATE TABLE [HRM-Demo-1].dbo.DepartmentRoles (
	Id int IDENTITY(1,1) NOT NULL,
	Name nvarchar(MAX) COLLATE SQL_Latin1_General_CP1_CI_AS NOT NULL,
	DisplayName nvarchar(MAX) COLLATE SQL_Latin1_General_CP1_CI_AS NOT NULL,
	Description nvarchar(MAX) COLLATE SQL_Latin1_General_CP1_CI_AS NULL,
	CONSTRAINT PK_DepartmentRoles PRIMARY KEY (Id)
);


-- [HRM-Demo-1].dbo.GroupPositions definition

-- Drop table

-- DROP TABLE [HRM-Demo-1].dbo.GroupPositions;

CREATE TABLE [HRM-Demo-1].dbo.GroupPositions (
	Id int IDENTITY(1,1) NOT NULL,
	GroupPositionName nvarchar(MAX) COLLATE SQL_Latin1_General_CP1_CI_AS NULL,
	CreatedBy int NULL,
	CreatedName nvarchar(MAX) COLLATE SQL_Latin1_General_CP1_CI_AS NULL,
	CreatedAt datetime2 NULL,
	UpdatedBy int NULL,
	UpdatedName nvarchar(MAX) COLLATE SQL_Latin1_General_CP1_CI_AS NULL,
	UpdatedAt datetime2 NULL,
	IsDeleted bit NULL,
	CONSTRAINT PK_GroupPositions PRIMARY KEY (Id)
);


-- [HRM-Demo-1].dbo.NatureOfLabor definition

-- Drop table

-- DROP TABLE [HRM-Demo-1].dbo.NatureOfLabor;

CREATE TABLE [HRM-Demo-1].dbo.NatureOfLabor (
	Id int IDENTITY(1,1) NOT NULL,
	Name nvarchar(MAX) COLLATE SQL_Latin1_General_CP1_CI_AS NULL,
	CreatedBy int NULL,
	CreatedName nvarchar(MAX) COLLATE SQL_Latin1_General_CP1_CI_AS NULL,
	CreatedAt datetime2 NULL,
	UpdatedBy int NULL,
	UpdatedName nvarchar(MAX) COLLATE SQL_Latin1_General_CP1_CI_AS NULL,
	UpdatedAt datetime2 NULL,
	IsDeleted bit NULL,
	CONSTRAINT PK_NatureOfLabor PRIMARY KEY (Id)
);


-- [HRM-Demo-1].dbo.PrefixConfigs definition

-- Drop table

-- DROP TABLE [HRM-Demo-1].dbo.PrefixConfigs;

CREATE TABLE [HRM-Demo-1].dbo.PrefixConfigs (
	Id int IDENTITY(1,1) NOT NULL,
	[Key] nvarchar(MAX) COLLATE SQL_Latin1_General_CP1_CI_AS NOT NULL,
	Value nvarchar(MAX) COLLATE SQL_Latin1_General_CP1_CI_AS NOT NULL,
	CreatedBy int NULL,
	CreatedName nvarchar(MAX) COLLATE SQL_Latin1_General_CP1_CI_AS NULL,
	CreatedAt datetime2 NULL,
	UpdatedBy int NULL,
	UpdatedName nvarchar(MAX) COLLATE SQL_Latin1_General_CP1_CI_AS NULL,
	UpdatedAt datetime2 NULL,
	IsDeleted bit NULL,
	CONSTRAINT PK_PrefixConfigs PRIMARY KEY (Id)
);


-- [HRM-Demo-1].dbo.ProjectRoles definition

-- Drop table

-- DROP TABLE [HRM-Demo-1].dbo.ProjectRoles;

CREATE TABLE [HRM-Demo-1].dbo.ProjectRoles (
	Id int IDENTITY(1,1) NOT NULL,
	Name nvarchar(MAX) COLLATE SQL_Latin1_General_CP1_CI_AS NOT NULL,
	DisplayName nvarchar(MAX) COLLATE SQL_Latin1_General_CP1_CI_AS NOT NULL,
	Description nvarchar(MAX) COLLATE SQL_Latin1_General_CP1_CI_AS NULL,
	CONSTRAINT PK_ProjectRoles PRIMARY KEY (Id)
);


-- [HRM-Demo-1].dbo.RepeatWorks definition

-- Drop table

-- DROP TABLE [HRM-Demo-1].dbo.RepeatWorks;

CREATE TABLE [HRM-Demo-1].dbo.RepeatWorks (
	Id int IDENTITY(1,1) NOT NULL,
	IsRepeat bit NULL,
	StartDate datetime2 NULL,
	EndDate datetime2 NOT NULL,
	RepeatNumberDay int NULL,
	IsMonday bit NULL,
	IsTuesday bit NULL,
	IsWednesday bit NULL,
	IsThursday bit NULL,
	IsFriday bit NULL,
	IsSaturday bit NULL,
	IsSunday bit NULL,
	RepeatWorkType int NOT NULL,
	RepeatCycle int NULL,
	InDayOfWeek int NULL,
	InDayOfMonth int NULL,
	RepeatHour time NULL,
	CreatedBy int NULL,
	CreatedName nvarchar(MAX) COLLATE SQL_Latin1_General_CP1_CI_AS NULL,
	CreatedAt datetime2 NULL,
	UpdatedBy int NULL,
	UpdatedName nvarchar(MAX) COLLATE SQL_Latin1_General_CP1_CI_AS NULL,
	UpdatedAt datetime2 NULL,
	IsDeleted bit NULL,
	CONSTRAINT PK_RepeatWorks PRIMARY KEY (Id)
);


-- [HRM-Demo-1].dbo.Roles definition

-- Drop table

-- DROP TABLE [HRM-Demo-1].dbo.Roles;

CREATE TABLE [HRM-Demo-1].dbo.Roles (
	Id int IDENTITY(1,1) NOT NULL,
	Description nvarchar(MAX) COLLATE SQL_Latin1_General_CP1_CI_AS NULL,
	CreatedBy int NULL,
	CreatedAt datetime2 NULL,
	UpdatedBy int NULL,
	UpdatedAt datetime2 NULL,
	DeletedBy int NULL,
	DeletedAt datetime2 NULL,
	Name nvarchar(256) COLLATE SQL_Latin1_General_CP1_CI_AS NULL,
	NormalizedName nvarchar(256) COLLATE SQL_Latin1_General_CP1_CI_AS NULL,
	ConcurrencyStamp nvarchar(MAX) COLLATE SQL_Latin1_General_CP1_CI_AS NULL,
	CONSTRAINT PK_Roles PRIMARY KEY (Id)
);
 CREATE UNIQUE NONCLUSTERED INDEX RoleNameIndex ON HRM-Demo-1.dbo.Roles (  NormalizedName ASC  )  
	 WHERE  ([NormalizedName] IS NOT NULL)
	 WITH (  PAD_INDEX = OFF ,FILLFACTOR = 100  ,SORT_IN_TEMPDB = OFF , IGNORE_DUP_KEY = OFF , STATISTICS_NORECOMPUTE = OFF , ONLINE = OFF , ALLOW_ROW_LOCKS = ON , ALLOW_PAGE_LOCKS = ON  )
	 ON [PRIMARY ] ;


-- [HRM-Demo-1].dbo.ScheduleJobs definition

-- Drop table

-- DROP TABLE [HRM-Demo-1].dbo.ScheduleJobs;

CREATE TABLE [HRM-Demo-1].dbo.ScheduleJobs (
	Id int IDENTITY(1,1) NOT NULL,
	WorkId int NOT NULL,
	JobId nvarchar(MAX) COLLATE SQL_Latin1_General_CP1_CI_AS NOT NULL,
	JobType nvarchar(MAX) COLLATE SQL_Latin1_General_CP1_CI_AS NOT NULL,
	CreatedBy int NULL,
	CreatedName nvarchar(MAX) COLLATE SQL_Latin1_General_CP1_CI_AS NULL,
	CreatedAt datetime2 NULL,
	UpdatedBy int NULL,
	UpdatedName nvarchar(MAX) COLLATE SQL_Latin1_General_CP1_CI_AS NULL,
	UpdatedAt datetime2 NULL,
	IsDeleted bit NULL,
	CONSTRAINT PK_ScheduleJobs PRIMARY KEY (Id)
);


-- [HRM-Demo-1].dbo.StaffTitles definition

-- Drop table

-- DROP TABLE [HRM-Demo-1].dbo.StaffTitles;

CREATE TABLE [HRM-Demo-1].dbo.StaffTitles (
	Id int IDENTITY(1,1) NOT NULL,
	StaffTitleName nvarchar(MAX) COLLATE SQL_Latin1_General_CP1_CI_AS NOT NULL,
	CreatedBy int NULL,
	CreatedName nvarchar(MAX) COLLATE SQL_Latin1_General_CP1_CI_AS NULL,
	CreatedAt datetime2 NULL,
	UpdatedBy int NULL,
	UpdatedName nvarchar(MAX) COLLATE SQL_Latin1_General_CP1_CI_AS NULL,
	UpdatedAt datetime2 NULL,
	IsDeleted bit NULL,
	CONSTRAINT PK_StaffTitles PRIMARY KEY (Id)
);


-- [HRM-Demo-1].dbo.StandardWorkNumbers definition

-- Drop table

-- DROP TABLE [HRM-Demo-1].dbo.StandardWorkNumbers;

CREATE TABLE [HRM-Demo-1].dbo.StandardWorkNumbers (
	Id int IDENTITY(1,1) NOT NULL,
	CreatedBy int NULL,
	CreatedName nvarchar(MAX) COLLATE SQL_Latin1_General_CP1_CI_AS NULL,
	CreatedAt datetime2 NULL,
	UpdatedBy int NULL,
	UpdatedName nvarchar(MAX) COLLATE SQL_Latin1_General_CP1_CI_AS NULL,
	UpdatedAt datetime2 NULL,
	IsDeleted bit NULL,
	CONSTRAINT PK_StandardWorkNumbers PRIMARY KEY (Id)
);


-- [HRM-Demo-1].dbo.Tags definition

-- Drop table

-- DROP TABLE [HRM-Demo-1].dbo.Tags;

CREATE TABLE [HRM-Demo-1].dbo.Tags (
	Id int IDENTITY(1,1) NOT NULL,
	Name nvarchar(MAX) COLLATE SQL_Latin1_General_CP1_CI_AS NULL,
	Color nvarchar(MAX) COLLATE SQL_Latin1_General_CP1_CI_AS NULL,
	CreatedBy int NULL,
	CreatedName nvarchar(MAX) COLLATE SQL_Latin1_General_CP1_CI_AS NULL,
	CreatedAt datetime2 NULL,
	UpdatedBy int NULL,
	UpdatedName nvarchar(MAX) COLLATE SQL_Latin1_General_CP1_CI_AS NULL,
	UpdatedAt datetime2 NULL,
	IsDeleted bit NULL,
	CONSTRAINT PK_Tags PRIMARY KEY (Id)
);


-- [HRM-Demo-1].dbo.WorkingForms definition

-- Drop table

-- DROP TABLE [HRM-Demo-1].dbo.WorkingForms;

CREATE TABLE [HRM-Demo-1].dbo.WorkingForms (
	Id int IDENTITY(1,1) NOT NULL,
	Form nvarchar(MAX) COLLATE SQL_Latin1_General_CP1_CI_AS NULL,
	WorkingFormStatus bit NULL,
	CreatedBy int NULL,
	CreatedName nvarchar(MAX) COLLATE SQL_Latin1_General_CP1_CI_AS NULL,
	CreatedAt datetime2 NULL,
	UpdatedBy int NULL,
	UpdatedName nvarchar(MAX) COLLATE SQL_Latin1_General_CP1_CI_AS NULL,
	UpdatedAt datetime2 NULL,
	IsDeleted bit NULL,
	CONSTRAINT PK_WorkingForms PRIMARY KEY (Id)
);


-- [HRM-Demo-1].dbo.[__EFMigrationsHistory] definition

-- Drop table

-- DROP TABLE [HRM-Demo-1].dbo.[__EFMigrationsHistory];

CREATE TABLE [HRM-Demo-1].dbo.[__EFMigrationsHistory] (
	MigrationId nvarchar(150) COLLATE SQL_Latin1_General_CP1_CI_AS NOT NULL,
	ProductVersion nvarchar(32) COLLATE SQL_Latin1_General_CP1_CI_AS NOT NULL,
	CONSTRAINT PK___EFMigrationsHistory PRIMARY KEY (MigrationId)
);


-- [HRM-Demo-1].dbo.sysdiagrams definition

-- Drop table

-- DROP TABLE [HRM-Demo-1].dbo.sysdiagrams;

CREATE TABLE [HRM-Demo-1].dbo.sysdiagrams (
	name sysname COLLATE SQL_Latin1_General_CP1_CI_AS NOT NULL,
	principal_id int NOT NULL,
	diagram_id int IDENTITY(1,1) NOT NULL,
	version int NULL,
	definition varbinary(MAX) NULL,
	CONSTRAINT PK__sysdiagr__C2B05B611323F589 PRIMARY KEY (diagram_id),
	CONSTRAINT UK_principal_name UNIQUE (principal_id,name)
);


-- [HRM-Demo-1].dbo.Cities definition

-- Drop table

-- DROP TABLE [HRM-Demo-1].dbo.Cities;

CREATE TABLE [HRM-Demo-1].dbo.Cities (
	Id int IDENTITY(1,1) NOT NULL,
	Code nvarchar(MAX) COLLATE SQL_Latin1_General_CP1_CI_AS NULL,
	Name nvarchar(MAX) COLLATE SQL_Latin1_General_CP1_CI_AS NULL,
	CountryId int NULL,
	Status int NULL,
	IsDeleted int NULL,
	Version int NULL,
	CreatedBy int NULL,
	CreatedName nvarchar(MAX) COLLATE SQL_Latin1_General_CP1_CI_AS NULL,
	CreatedAt datetime2 NULL,
	UpdatedBy int NULL,
	UpdatedName nvarchar(MAX) COLLATE SQL_Latin1_General_CP1_CI_AS NULL,
	UpdatedAt datetime2 NULL,
	CONSTRAINT PK_Cities PRIMARY KEY (Id),
	CONSTRAINT FK_Cities_Countries_CountryId FOREIGN KEY (CountryId) REFERENCES [HRM-Demo-1].dbo.Countries(Id)
);
 CREATE NONCLUSTERED INDEX IX_Cities_CountryId ON HRM-Demo-1.dbo.Cities (  CountryId ASC  )  
	 WITH (  PAD_INDEX = OFF ,FILLFACTOR = 100  ,SORT_IN_TEMPDB = OFF , IGNORE_DUP_KEY = OFF , STATISTICS_NORECOMPUTE = OFF , ONLINE = OFF , ALLOW_ROW_LOCKS = ON , ALLOW_PAGE_LOCKS = ON  )
	 ON [PRIMARY ] ;


-- [HRM-Demo-1].dbo.Districts definition

-- Drop table

-- DROP TABLE [HRM-Demo-1].dbo.Districts;

CREATE TABLE [HRM-Demo-1].dbo.Districts (
	Id int IDENTITY(1,1) NOT NULL,
	Code nvarchar(MAX) COLLATE SQL_Latin1_General_CP1_CI_AS NULL,
	Name nvarchar(MAX) COLLATE SQL_Latin1_General_CP1_CI_AS NULL,
	CityId int NULL,
	CityName nvarchar(MAX) COLLATE SQL_Latin1_General_CP1_CI_AS NULL,
	Status int NULL,
	IsDeleted int NULL,
	Version int NULL,
	CONSTRAINT PK_Districts PRIMARY KEY (Id),
	CONSTRAINT FK_Districts_Cities_CityId FOREIGN KEY (CityId) REFERENCES [HRM-Demo-1].dbo.Cities(Id)
);
 CREATE NONCLUSTERED INDEX IX_Districts_CityId ON HRM-Demo-1.dbo.Districts (  CityId ASC  )  
	 WITH (  PAD_INDEX = OFF ,FILLFACTOR = 100  ,SORT_IN_TEMPDB = OFF , IGNORE_DUP_KEY = OFF , STATISTICS_NORECOMPUTE = OFF , ONLINE = OFF , ALLOW_ROW_LOCKS = ON , ALLOW_PAGE_LOCKS = ON  )
	 ON [PRIMARY ] ;


-- [HRM-Demo-1].dbo.OrganizationTypes definition

-- Drop table

-- DROP TABLE [HRM-Demo-1].dbo.OrganizationTypes;

CREATE TABLE [HRM-Demo-1].dbo.OrganizationTypes (
	Id int IDENTITY(1,1) NOT NULL,
	CompanyId int NOT NULL,
	OrganizationTypeName nvarchar(MAX) COLLATE SQL_Latin1_General_CP1_CI_AS NOT NULL,
	CreatedBy int NULL,
	CreatedName nvarchar(MAX) COLLATE SQL_Latin1_General_CP1_CI_AS NULL,
	CreatedAt datetime2 NULL,
	UpdatedBy int NULL,
	UpdatedName nvarchar(MAX) COLLATE SQL_Latin1_General_CP1_CI_AS NULL,
	UpdatedAt datetime2 NULL,
	IsDeleted bit NULL,
	CONSTRAINT PK_OrganizationTypes PRIMARY KEY (Id),
	CONSTRAINT FK_OrganizationTypes_Companies_CompanyId FOREIGN KEY (CompanyId) REFERENCES [HRM-Demo-1].dbo.Companies(Id) ON DELETE CASCADE
);
 CREATE NONCLUSTERED INDEX IX_OrganizationTypes_CompanyId ON HRM-Demo-1.dbo.OrganizationTypes (  CompanyId ASC  )  
	 WITH (  PAD_INDEX = OFF ,FILLFACTOR = 100  ,SORT_IN_TEMPDB = OFF , IGNORE_DUP_KEY = OFF , STATISTICS_NORECOMPUTE = OFF , ONLINE = OFF , ALLOW_ROW_LOCKS = ON , ALLOW_PAGE_LOCKS = ON  )
	 ON [PRIMARY ] ;


-- [HRM-Demo-1].dbo.Organizations definition

-- Drop table

-- DROP TABLE [HRM-Demo-1].dbo.Organizations;

CREATE TABLE [HRM-Demo-1].dbo.Organizations (
	Id int IDENTITY(1,1) NOT NULL,
	OrganizationEnum int NOT NULL,
	OrganizationCode nvarchar(MAX) COLLATE SQL_Latin1_General_CP1_CI_AS NULL,
	OrganizationName nvarchar(MAX) COLLATE SQL_Latin1_General_CP1_CI_AS NULL,
	Abbreviation nvarchar(MAX) COLLATE SQL_Latin1_General_CP1_CI_AS NULL,
	CompanyId int NULL,
	[Rank] int NULL,
	OrganizationTypeId int NULL,
	OrganizationTypeName nvarchar(MAX) COLLATE SQL_Latin1_General_CP1_CI_AS NULL,
	OrganizatioParentId int NULL,
	OrganizationDescription nvarchar(MAX) COLLATE SQL_Latin1_General_CP1_CI_AS NULL,
	BusinessRegistrationCode nvarchar(MAX) COLLATE SQL_Latin1_General_CP1_CI_AS NULL,
	BusinessRegistrationDate datetime2 NULL,
	IssuingAuthority nvarchar(MAX) COLLATE SQL_Latin1_General_CP1_CI_AS NULL,
	OrganizationAddress nvarchar(MAX) COLLATE SQL_Latin1_General_CP1_CI_AS NULL,
	OrganizationLeadersName nvarchar(MAX) COLLATE SQL_Latin1_General_CP1_CI_AS NULL,
	OrganizationStatus bit NULL,
	CreatedBy int NULL,
	CreatedName nvarchar(MAX) COLLATE SQL_Latin1_General_CP1_CI_AS NULL,
	CreatedAt datetime2 NULL,
	UpdatedBy int NULL,
	UpdatedName nvarchar(MAX) COLLATE SQL_Latin1_General_CP1_CI_AS NULL,
	UpdatedAt datetime2 NULL,
	IsDeleted bit NULL,
	CONSTRAINT PK_Organizations PRIMARY KEY (Id),
	CONSTRAINT FK_Organizations_Companies_CompanyId FOREIGN KEY (CompanyId) REFERENCES [HRM-Demo-1].dbo.Companies(Id),
	CONSTRAINT FK_Organizations_OrganizationTypes_OrganizationTypeId FOREIGN KEY (OrganizationTypeId) REFERENCES [HRM-Demo-1].dbo.OrganizationTypes(Id),
	CONSTRAINT FK_Organizations_Organizations_OrganizatioParentId FOREIGN KEY (OrganizatioParentId) REFERENCES [HRM-Demo-1].dbo.Organizations(Id)
);
 CREATE NONCLUSTERED INDEX IX_Organizations_CompanyId ON HRM-Demo-1.dbo.Organizations (  CompanyId ASC  )  
	 WITH (  PAD_INDEX = OFF ,FILLFACTOR = 100  ,SORT_IN_TEMPDB = OFF , IGNORE_DUP_KEY = OFF , STATISTICS_NORECOMPUTE = OFF , ONLINE = OFF , ALLOW_ROW_LOCKS = ON , ALLOW_PAGE_LOCKS = ON  )
	 ON [PRIMARY ] ;
 CREATE NONCLUSTERED INDEX IX_Organizations_OrganizatioParentId ON HRM-Demo-1.dbo.Organizations (  OrganizatioParentId ASC  )  
	 WITH (  PAD_INDEX = OFF ,FILLFACTOR = 100  ,SORT_IN_TEMPDB = OFF , IGNORE_DUP_KEY = OFF , STATISTICS_NORECOMPUTE = OFF , ONLINE = OFF , ALLOW_ROW_LOCKS = ON , ALLOW_PAGE_LOCKS = ON  )
	 ON [PRIMARY ] ;
 CREATE NONCLUSTERED INDEX IX_Organizations_OrganizationTypeId ON HRM-Demo-1.dbo.Organizations (  OrganizationTypeId ASC  )  
	 WITH (  PAD_INDEX = OFF ,FILLFACTOR = 100  ,SORT_IN_TEMPDB = OFF , IGNORE_DUP_KEY = OFF , STATISTICS_NORECOMPUTE = OFF , ONLINE = OFF , ALLOW_ROW_LOCKS = ON , ALLOW_PAGE_LOCKS = ON  )
	 ON [PRIMARY ] ;


-- [HRM-Demo-1].dbo.Payrolls definition

-- Drop table

-- DROP TABLE [HRM-Demo-1].dbo.Payrolls;

CREATE TABLE [HRM-Demo-1].dbo.Payrolls (
	Id int IDENTITY(1,1) NOT NULL,
	OrganizationId int NULL,
	PayrollName nvarchar(MAX) COLLATE SQL_Latin1_General_CP1_CI_AS NULL,
	PayrollStatus int NOT NULL,
	CreatedBy int NULL,
	CreatedName nvarchar(MAX) COLLATE SQL_Latin1_General_CP1_CI_AS NULL,
	CreatedAt datetime2 NULL,
	UpdatedBy int NULL,
	UpdatedName nvarchar(MAX) COLLATE SQL_Latin1_General_CP1_CI_AS NULL,
	UpdatedAt datetime2 NULL,
	IsDeleted bit NULL,
	PayrollConfirmationStatus int DEFAULT 0 NOT NULL,
	CONSTRAINT PK_Payrolls PRIMARY KEY (Id),
	CONSTRAINT FK_Payrolls_Organizations_OrganizationId FOREIGN KEY (OrganizationId) REFERENCES [HRM-Demo-1].dbo.Organizations(Id)
);
 CREATE NONCLUSTERED INDEX IX_Payrolls_OrganizationId ON HRM-Demo-1.dbo.Payrolls (  OrganizationId ASC  )  
	 WITH (  PAD_INDEX = OFF ,FILLFACTOR = 100  ,SORT_IN_TEMPDB = OFF , IGNORE_DUP_KEY = OFF , STATISTICS_NORECOMPUTE = OFF , ONLINE = OFF , ALLOW_ROW_LOCKS = ON , ALLOW_PAGE_LOCKS = ON  )
	 ON [PRIMARY ] ;


-- [HRM-Demo-1].dbo.Permissions definition

-- Drop table

-- DROP TABLE [HRM-Demo-1].dbo.Permissions;

CREATE TABLE [HRM-Demo-1].dbo.Permissions (
	Id int IDENTITY(1,1) NOT NULL,
	Name nvarchar(MAX) COLLATE SQL_Latin1_General_CP1_CI_AS NOT NULL,
	DisplayName nvarchar(MAX) COLLATE SQL_Latin1_General_CP1_CI_AS NOT NULL,
	ParentPermissionId int NULL,
	Description nvarchar(MAX) COLLATE SQL_Latin1_General_CP1_CI_AS NULL,
	CreatedBy int NULL,
	CreatedName nvarchar(MAX) COLLATE SQL_Latin1_General_CP1_CI_AS NULL,
	CreatedAt datetime2 NULL,
	UpdatedBy int NULL,
	UpdatedName nvarchar(MAX) COLLATE SQL_Latin1_General_CP1_CI_AS NULL,
	UpdatedAt datetime2 NULL,
	IsDeleted bit NULL,
	[Section] int NULL,
	CONSTRAINT PK_Permissions PRIMARY KEY (Id),
	CONSTRAINT FK_Permissions_Permissions_ParentPermissionId FOREIGN KEY (ParentPermissionId) REFERENCES [HRM-Demo-1].dbo.Permissions(Id)
);
 CREATE NONCLUSTERED INDEX IX_Permissions_ParentPermissionId ON HRM-Demo-1.dbo.Permissions (  ParentPermissionId ASC  )  
	 WITH (  PAD_INDEX = OFF ,FILLFACTOR = 100  ,SORT_IN_TEMPDB = OFF , IGNORE_DUP_KEY = OFF , STATISTICS_NORECOMPUTE = OFF , ONLINE = OFF , ALLOW_ROW_LOCKS = ON , ALLOW_PAGE_LOCKS = ON  )
	 ON [PRIMARY ] ;


-- [HRM-Demo-1].dbo.RoleClaims definition

-- Drop table

-- DROP TABLE [HRM-Demo-1].dbo.RoleClaims;

CREATE TABLE [HRM-Demo-1].dbo.RoleClaims (
	Id int IDENTITY(1,1) NOT NULL,
	RoleId int NOT NULL,
	ClaimType nvarchar(MAX) COLLATE SQL_Latin1_General_CP1_CI_AS NULL,
	ClaimValue nvarchar(MAX) COLLATE SQL_Latin1_General_CP1_CI_AS NULL,
	CONSTRAINT PK_RoleClaims PRIMARY KEY (Id),
	CONSTRAINT FK_RoleClaims_Roles_RoleId FOREIGN KEY (RoleId) REFERENCES [HRM-Demo-1].dbo.Roles(Id) ON DELETE CASCADE
);
 CREATE NONCLUSTERED INDEX IX_RoleClaims_RoleId ON HRM-Demo-1.dbo.RoleClaims (  RoleId ASC  )  
	 WITH (  PAD_INDEX = OFF ,FILLFACTOR = 100  ,SORT_IN_TEMPDB = OFF , IGNORE_DUP_KEY = OFF , STATISTICS_NORECOMPUTE = OFF , ONLINE = OFF , ALLOW_ROW_LOCKS = ON , ALLOW_PAGE_LOCKS = ON  )
	 ON [PRIMARY ] ;


-- [HRM-Demo-1].dbo.RolePermissions definition

-- Drop table

-- DROP TABLE [HRM-Demo-1].dbo.RolePermissions;

CREATE TABLE [HRM-Demo-1].dbo.RolePermissions (
	Id int IDENTITY(1,1) NOT NULL,
	RoleId int NOT NULL,
	PermissionId int NOT NULL,
	CONSTRAINT PK_RolePermissions PRIMARY KEY (Id),
	CONSTRAINT FK_RolePermissions_Permissions_PermissionId FOREIGN KEY (PermissionId) REFERENCES [HRM-Demo-1].dbo.Permissions(Id) ON DELETE CASCADE,
	CONSTRAINT FK_RolePermissions_Roles_RoleId FOREIGN KEY (RoleId) REFERENCES [HRM-Demo-1].dbo.Roles(Id) ON DELETE CASCADE
);
 CREATE NONCLUSTERED INDEX IX_RolePermissions_PermissionId ON HRM-Demo-1.dbo.RolePermissions (  PermissionId ASC  )  
	 WITH (  PAD_INDEX = OFF ,FILLFACTOR = 100  ,SORT_IN_TEMPDB = OFF , IGNORE_DUP_KEY = OFF , STATISTICS_NORECOMPUTE = OFF , ONLINE = OFF , ALLOW_ROW_LOCKS = ON , ALLOW_PAGE_LOCKS = ON  )
	 ON [PRIMARY ] ;
 CREATE NONCLUSTERED INDEX IX_RolePermissions_RoleId ON HRM-Demo-1.dbo.RolePermissions (  RoleId ASC  )  
	 WITH (  PAD_INDEX = OFF ,FILLFACTOR = 100  ,SORT_IN_TEMPDB = OFF , IGNORE_DUP_KEY = OFF , STATISTICS_NORECOMPUTE = OFF , ONLINE = OFF , ALLOW_ROW_LOCKS = ON , ALLOW_PAGE_LOCKS = ON  )
	 ON [PRIMARY ] ;


-- [HRM-Demo-1].dbo.SalaryComponents definition

-- Drop table

-- DROP TABLE [HRM-Demo-1].dbo.SalaryComponents;

CREATE TABLE [HRM-Demo-1].dbo.SalaryComponents (
	Id int IDENTITY(1,1) NOT NULL,
	OrganizationId int NULL,
	ComponentName nvarchar(MAX) COLLATE SQL_Latin1_General_CP1_CI_AS NULL,
	ComponentCode nvarchar(MAX) COLLATE SQL_Latin1_General_CP1_CI_AS NULL,
	Nature int NOT NULL,
	Characteristic int NOT NULL,
	ValueFormula nvarchar(MAX) COLLATE SQL_Latin1_General_CP1_CI_AS NULL,
	CalcType int DEFAULT 0 NOT NULL,
	BaseSource int NULL,
	FixedAmount decimal(18,2) NULL,
	UnitAmount decimal(18,2) NULL,
	RatePercent decimal(18,2) NULL,
	CapAmount decimal(18,2) NULL,
	Description nvarchar(MAX) COLLATE SQL_Latin1_General_CP1_CI_AS NULL,
	CreatedBy int NULL,
	CreatedName nvarchar(MAX) COLLATE SQL_Latin1_General_CP1_CI_AS NULL,
	CreatedAt datetime2 NULL,
	UpdatedBy int NULL,
	UpdatedName nvarchar(MAX) COLLATE SQL_Latin1_General_CP1_CI_AS NULL,
	UpdatedAt datetime2 NULL,
	IsDeleted bit NULL,
	Status int DEFAULT 0 NOT NULL,
	Piority int NULL,
	CONSTRAINT PK_SalaryComponents PRIMARY KEY (Id),
	CONSTRAINT FK_SalaryComponents_Organizations_OrganizationId FOREIGN KEY (OrganizationId) REFERENCES [HRM-Demo-1].dbo.Organizations(Id)
);
 CREATE NONCLUSTERED INDEX IX_SalaryComponents_OrganizationId ON HRM-Demo-1.dbo.SalaryComponents (  OrganizationId ASC  )  
	 WITH (  PAD_INDEX = OFF ,FILLFACTOR = 100  ,SORT_IN_TEMPDB = OFF , IGNORE_DUP_KEY = OFF , STATISTICS_NORECOMPUTE = OFF , ONLINE = OFF , ALLOW_ROW_LOCKS = ON , ALLOW_PAGE_LOCKS = ON  )
	 ON [PRIMARY ] ;


-- [HRM-Demo-1].dbo.ShiftCatalogs definition

-- Drop table

-- DROP TABLE [HRM-Demo-1].dbo.ShiftCatalogs;

CREATE TABLE [HRM-Demo-1].dbo.ShiftCatalogs (
	Id int IDENTITY(1,1) NOT NULL,
	OrganizationId int NULL,
	Code nvarchar(MAX) COLLATE SQL_Latin1_General_CP1_CI_AS NULL,
	Name nvarchar(MAX) COLLATE SQL_Latin1_General_CP1_CI_AS NULL,
	StartTime time NULL,
	EndTime time NULL,
	IsTimeChecked bit NULL,
	StartTimeIn time NULL,
	EndTimeIn time NULL,
	IsBreak bit NULL,
	StartTimeOut time NULL,
	EndTimeOut time NULL,
	TakeABreak bit NULL,
	StartTakeABreak time NULL,
	EndTakeABreak time NULL,
	WorkingHours float NULL,
	WorkingDays int NULL,
	RegularMultiplier float NULL,
	HolidayMultiplier float NULL,
	LeaveDaysMultiplier float NULL,
	DeductIfNoStartTime bit NULL,
	DeductIfNoEndTime bit NULL,
	AllowEarlyLeave bit NULL,
	AllowedEarlyLeaveMinutes int NULL,
	AllowLateArrival bit NULL,
	AllowedLateArrivalMinutes int NULL,
	AllowOvertime bit NULL,
	CreatedBy int NULL,
	CreatedName nvarchar(MAX) COLLATE SQL_Latin1_General_CP1_CI_AS NULL,
	CreatedAt datetime2 NULL,
	UpdatedBy int NULL,
	UpdatedName nvarchar(MAX) COLLATE SQL_Latin1_General_CP1_CI_AS NULL,
	UpdatedAt datetime2 NULL,
	IsDeleted bit NULL,
	CONSTRAINT PK_ShiftCatalogs PRIMARY KEY (Id),
	CONSTRAINT FK_ShiftCatalogs_Organizations_OrganizationId FOREIGN KEY (OrganizationId) REFERENCES [HRM-Demo-1].dbo.Organizations(Id)
);
 CREATE NONCLUSTERED INDEX IX_ShiftCatalogs_OrganizationId ON HRM-Demo-1.dbo.ShiftCatalogs (  OrganizationId ASC  )  
	 WITH (  PAD_INDEX = OFF ,FILLFACTOR = 100  ,SORT_IN_TEMPDB = OFF , IGNORE_DUP_KEY = OFF , STATISTICS_NORECOMPUTE = OFF , ONLINE = OFF , ALLOW_ROW_LOCKS = ON , ALLOW_PAGE_LOCKS = ON  )
	 ON [PRIMARY ] ;


-- [HRM-Demo-1].dbo.ShiftWorks definition

-- Drop table

-- DROP TABLE [HRM-Demo-1].dbo.ShiftWorks;

CREATE TABLE [HRM-Demo-1].dbo.ShiftWorks (
	Id int IDENTITY(1,1) NOT NULL,
	ShiftTableName nvarchar(MAX) COLLATE SQL_Latin1_General_CP1_CI_AS NULL,
	ShiftCatalogId int NULL,
	OrganizationId int NULL,
	StartDate datetime2 NULL,
	EndDate datetime2 NULL,
	RecurrenceType int NULL,
	RecurrenceCount int NULL,
	IsMonday bit NULL,
	IsTuesday bit NULL,
	IsWednesday bit NULL,
	IsThursday bit NULL,
	IsFriday bit NULL,
	IsSaturday bit NULL,
	IsSunday bit NULL,
	ApplyObject int NULL,
	CreatedBy int NULL,
	CreatedName nvarchar(MAX) COLLATE SQL_Latin1_General_CP1_CI_AS NULL,
	CreatedAt datetime2 NULL,
	UpdatedBy int NULL,
	UpdatedName nvarchar(MAX) COLLATE SQL_Latin1_General_CP1_CI_AS NULL,
	UpdatedAt datetime2 NULL,
	IsDeleted bit NULL,
	TotalWork int NULL,
	CONSTRAINT PK_ShiftWorks PRIMARY KEY (Id),
	CONSTRAINT FK_ShiftWorks_Organizations_OrganizationId FOREIGN KEY (OrganizationId) REFERENCES [HRM-Demo-1].dbo.Organizations(Id),
	CONSTRAINT FK_ShiftWorks_ShiftCatalogs_ShiftCatalogId FOREIGN KEY (ShiftCatalogId) REFERENCES [HRM-Demo-1].dbo.ShiftCatalogs(Id)
);
 CREATE NONCLUSTERED INDEX IX_ShiftWorks_OrganizationId ON HRM-Demo-1.dbo.ShiftWorks (  OrganizationId ASC  )  
	 WITH (  PAD_INDEX = OFF ,FILLFACTOR = 100  ,SORT_IN_TEMPDB = OFF , IGNORE_DUP_KEY = OFF , STATISTICS_NORECOMPUTE = OFF , ONLINE = OFF , ALLOW_ROW_LOCKS = ON , ALLOW_PAGE_LOCKS = ON  )
	 ON [PRIMARY ] ;
 CREATE NONCLUSTERED INDEX IX_ShiftWorks_ShiftCatalogId ON HRM-Demo-1.dbo.ShiftWorks (  ShiftCatalogId ASC  )  
	 WITH (  PAD_INDEX = OFF ,FILLFACTOR = 100  ,SORT_IN_TEMPDB = OFF , IGNORE_DUP_KEY = OFF , STATISTICS_NORECOMPUTE = OFF , ONLINE = OFF , ALLOW_ROW_LOCKS = ON , ALLOW_PAGE_LOCKS = ON  )
	 ON [PRIMARY ] ;


-- [HRM-Demo-1].dbo.StaffPositions definition

-- Drop table

-- DROP TABLE [HRM-Demo-1].dbo.StaffPositions;

CREATE TABLE [HRM-Demo-1].dbo.StaffPositions (
	Id int IDENTITY(1,1) NOT NULL,
	PositionCode nvarchar(MAX) COLLATE SQL_Latin1_General_CP1_CI_AS NOT NULL,
	PositionName nvarchar(MAX) COLLATE SQL_Latin1_General_CP1_CI_AS NOT NULL,
	GroupPositionId int NULL,
	StaffTitleId int NULL,
	StaffPositionStatus bit NULL,
	CreatedBy int NULL,
	CreatedName nvarchar(MAX) COLLATE SQL_Latin1_General_CP1_CI_AS NULL,
	CreatedAt datetime2 NULL,
	UpdatedBy int NULL,
	UpdatedName nvarchar(MAX) COLLATE SQL_Latin1_General_CP1_CI_AS NULL,
	UpdatedAt datetime2 NULL,
	IsDeleted bit NULL,
	CONSTRAINT PK_StaffPositions PRIMARY KEY (Id),
	CONSTRAINT FK_StaffPositions_GroupPositions_GroupPositionId FOREIGN KEY (GroupPositionId) REFERENCES [HRM-Demo-1].dbo.GroupPositions(Id),
	CONSTRAINT FK_StaffPositions_StaffTitles_StaffTitleId FOREIGN KEY (StaffTitleId) REFERENCES [HRM-Demo-1].dbo.StaffTitles(Id)
);
 CREATE NONCLUSTERED INDEX IX_StaffPositions_GroupPositionId ON HRM-Demo-1.dbo.StaffPositions (  GroupPositionId ASC  )  
	 WITH (  PAD_INDEX = OFF ,FILLFACTOR = 100  ,SORT_IN_TEMPDB = OFF , IGNORE_DUP_KEY = OFF , STATISTICS_NORECOMPUTE = OFF , ONLINE = OFF , ALLOW_ROW_LOCKS = ON , ALLOW_PAGE_LOCKS = ON  )
	 ON [PRIMARY ] ;
 CREATE NONCLUSTERED INDEX IX_StaffPositions_StaffTitleId ON HRM-Demo-1.dbo.StaffPositions (  StaffTitleId ASC  )  
	 WITH (  PAD_INDEX = OFF ,FILLFACTOR = 100  ,SORT_IN_TEMPDB = OFF , IGNORE_DUP_KEY = OFF , STATISTICS_NORECOMPUTE = OFF , ONLINE = OFF , ALLOW_ROW_LOCKS = ON , ALLOW_PAGE_LOCKS = ON  )
	 ON [PRIMARY ] ;


-- [HRM-Demo-1].dbo.SummaryTimesheetNames definition

-- Drop table

-- DROP TABLE [HRM-Demo-1].dbo.SummaryTimesheetNames;

CREATE TABLE [HRM-Demo-1].dbo.SummaryTimesheetNames (
	Id int IDENTITY(1,1) NOT NULL,
	OrganizationId int NULL,
	TimekeepingSheetName nvarchar(MAX) COLLATE SQL_Latin1_General_CP1_CI_AS NULL,
	TimekeepingMethod int NULL,
	CreatedBy int NULL,
	CreatedName nvarchar(MAX) COLLATE SQL_Latin1_General_CP1_CI_AS NULL,
	CreatedAt datetime2 NULL,
	UpdatedBy int NULL,
	UpdatedName nvarchar(MAX) COLLATE SQL_Latin1_General_CP1_CI_AS NULL,
	UpdatedAt datetime2 NULL,
	IsDeleted bit NULL,
	CONSTRAINT PK_SummaryTimesheetNames PRIMARY KEY (Id),
	CONSTRAINT FK_SummaryTimesheetNames_Organizations_OrganizationId FOREIGN KEY (OrganizationId) REFERENCES [HRM-Demo-1].dbo.Organizations(Id)
);
 CREATE NONCLUSTERED INDEX IX_SummaryTimesheetNames_OrganizationId ON HRM-Demo-1.dbo.SummaryTimesheetNames (  OrganizationId ASC  )  
	 WITH (  PAD_INDEX = OFF ,FILLFACTOR = 100  ,SORT_IN_TEMPDB = OFF , IGNORE_DUP_KEY = OFF , STATISTICS_NORECOMPUTE = OFF , ONLINE = OFF , ALLOW_ROW_LOCKS = ON , ALLOW_PAGE_LOCKS = ON  )
	 ON [PRIMARY ] ;


-- [HRM-Demo-1].dbo.TimekeepingLocations definition

-- Drop table

-- DROP TABLE [HRM-Demo-1].dbo.TimekeepingLocations;

CREATE TABLE [HRM-Demo-1].dbo.TimekeepingLocations (
	Id int IDENTITY(1,1) NOT NULL,
	OrganizationId int NULL,
	Name nvarchar(MAX) COLLATE SQL_Latin1_General_CP1_CI_AS NULL,
	Latitude nvarchar(MAX) COLLATE SQL_Latin1_General_CP1_CI_AS NOT NULL,
	Longitude nvarchar(MAX) COLLATE SQL_Latin1_General_CP1_CI_AS NOT NULL,
	AllowableRadius float NOT NULL,
	CreatedBy int NULL,
	CreatedName nvarchar(MAX) COLLATE SQL_Latin1_General_CP1_CI_AS NULL,
	CreatedAt datetime2 NULL,
	UpdatedBy int NULL,
	UpdatedName nvarchar(MAX) COLLATE SQL_Latin1_General_CP1_CI_AS NULL,
	UpdatedAt datetime2 NULL,
	IsDeleted bit NULL,
	CONSTRAINT PK_TimekeepingLocations PRIMARY KEY (Id),
	CONSTRAINT FK_TimekeepingLocations_Organizations_OrganizationId FOREIGN KEY (OrganizationId) REFERENCES [HRM-Demo-1].dbo.Organizations(Id)
);
 CREATE NONCLUSTERED INDEX IX_TimekeepingLocations_OrganizationId ON HRM-Demo-1.dbo.TimekeepingLocations (  OrganizationId ASC  )  
	 WITH (  PAD_INDEX = OFF ,FILLFACTOR = 100  ,SORT_IN_TEMPDB = OFF , IGNORE_DUP_KEY = OFF , STATISTICS_NORECOMPUTE = OFF , ONLINE = OFF , ALLOW_ROW_LOCKS = ON , ALLOW_PAGE_LOCKS = ON  )
	 ON [PRIMARY ] ;


-- [HRM-Demo-1].dbo.TimekeepingRegulations definition

-- Drop table

-- DROP TABLE [HRM-Demo-1].dbo.TimekeepingRegulations;

CREATE TABLE [HRM-Demo-1].dbo.TimekeepingRegulations (
	Id int IDENTITY(1,1) NOT NULL,
	OrganizationId int NULL,
	AllowEmployeesToRegisterForShifts bit NULL,
	AllowDailyTimekeepingDetail bit NULL,
	AllowTrackingWorkHoursOnTimekeepingSheet bit NULL,
	AllowTimekeepingOutsideScheduledShifts bit NULL,
	AllowShiftBasedTimekeeping bit NULL,
	PartTimePayrollType int NULL,
	CreatedBy int NULL,
	CreatedName nvarchar(MAX) COLLATE SQL_Latin1_General_CP1_CI_AS NULL,
	CreatedAt datetime2 NULL,
	UpdatedBy int NULL,
	UpdatedName nvarchar(MAX) COLLATE SQL_Latin1_General_CP1_CI_AS NULL,
	UpdatedAt datetime2 NULL,
	IsDeleted bit NULL,
	CONSTRAINT PK_TimekeepingRegulations PRIMARY KEY (Id),
	CONSTRAINT FK_TimekeepingRegulations_Organizations_OrganizationId FOREIGN KEY (OrganizationId) REFERENCES [HRM-Demo-1].dbo.Organizations(Id)
);
 CREATE NONCLUSTERED INDEX IX_TimekeepingRegulations_OrganizationId ON HRM-Demo-1.dbo.TimekeepingRegulations (  OrganizationId ASC  )  
	 WITH (  PAD_INDEX = OFF ,FILLFACTOR = 100  ,SORT_IN_TEMPDB = OFF , IGNORE_DUP_KEY = OFF , STATISTICS_NORECOMPUTE = OFF , ONLINE = OFF , ALLOW_ROW_LOCKS = ON , ALLOW_PAGE_LOCKS = ON  )
	 ON [PRIMARY ] ;


-- [HRM-Demo-1].dbo.TimekeepingSettings definition

-- Drop table

-- DROP TABLE [HRM-Demo-1].dbo.TimekeepingSettings;

CREATE TABLE [HRM-Demo-1].dbo.TimekeepingSettings (
	Id int IDENTITY(1,1) NOT NULL,
	OrganizationId int NULL,
	AllowApplication int NULL,
	CreatedBy int NULL,
	CreatedName nvarchar(MAX) COLLATE SQL_Latin1_General_CP1_CI_AS NULL,
	CreatedAt datetime2 NULL,
	UpdatedBy int NULL,
	UpdatedName nvarchar(MAX) COLLATE SQL_Latin1_General_CP1_CI_AS NULL,
	UpdatedAt datetime2 NULL,
	IsDeleted bit NULL,
	TimekeepingType int NULL,
	CONSTRAINT PK_TimekeepingSettings PRIMARY KEY (Id),
	CONSTRAINT FK_TimekeepingSettings_Organizations_OrganizationId FOREIGN KEY (OrganizationId) REFERENCES [HRM-Demo-1].dbo.Organizations(Id)
);
 CREATE NONCLUSTERED INDEX IX_TimekeepingSettings_OrganizationId ON HRM-Demo-1.dbo.TimekeepingSettings (  OrganizationId ASC  )  
	 WITH (  PAD_INDEX = OFF ,FILLFACTOR = 100  ,SORT_IN_TEMPDB = OFF , IGNORE_DUP_KEY = OFF , STATISTICS_NORECOMPUTE = OFF , ONLINE = OFF , ALLOW_ROW_LOCKS = ON , ALLOW_PAGE_LOCKS = ON  )
	 ON [PRIMARY ] ;


-- [HRM-Demo-1].dbo.TypeOfLeaves definition

-- Drop table

-- DROP TABLE [HRM-Demo-1].dbo.TypeOfLeaves;

CREATE TABLE [HRM-Demo-1].dbo.TypeOfLeaves (
	Id int IDENTITY(1,1) NOT NULL,
	OrganizationId int NULL,
	Name nvarchar(MAX) COLLATE SQL_Latin1_General_CP1_CI_AS NULL,
	SalaryRate decimal(18,2) NULL,
	MaximumNumberOfDayOff float NULL,
	Note nvarchar(MAX) COLLATE SQL_Latin1_General_CP1_CI_AS NULL,
	ApplyObject int NOT NULL,
	CreatedBy int NULL,
	CreatedName nvarchar(MAX) COLLATE SQL_Latin1_General_CP1_CI_AS NULL,
	CreatedAt datetime2 NULL,
	UpdatedBy int NULL,
	UpdatedName nvarchar(MAX) COLLATE SQL_Latin1_General_CP1_CI_AS NULL,
	UpdatedAt datetime2 NULL,
	IsDeleted bit NULL,
	CONSTRAINT PK_TypeOfLeaves PRIMARY KEY (Id),
	CONSTRAINT FK_TypeOfLeaves_Organizations_OrganizationId FOREIGN KEY (OrganizationId) REFERENCES [HRM-Demo-1].dbo.Organizations(Id)
);
 CREATE NONCLUSTERED INDEX IX_TypeOfLeaves_OrganizationId ON HRM-Demo-1.dbo.TypeOfLeaves (  OrganizationId ASC  )  
	 WITH (  PAD_INDEX = OFF ,FILLFACTOR = 100  ,SORT_IN_TEMPDB = OFF , IGNORE_DUP_KEY = OFF , STATISTICS_NORECOMPUTE = OFF , ONLINE = OFF , ALLOW_ROW_LOCKS = ON , ALLOW_PAGE_LOCKS = ON  )
	 ON [PRIMARY ] ;


-- [HRM-Demo-1].dbo.Wards definition

-- Drop table

-- DROP TABLE [HRM-Demo-1].dbo.Wards;

CREATE TABLE [HRM-Demo-1].dbo.Wards (
	Id int IDENTITY(1,1) NOT NULL,
	Code nvarchar(MAX) COLLATE SQL_Latin1_General_CP1_CI_AS NULL,
	Name nvarchar(MAX) COLLATE SQL_Latin1_General_CP1_CI_AS NULL,
	DistrictId int NULL,
	Status int NULL,
	IsDeleted int NULL,
	Version int NULL,
	CONSTRAINT PK_Wards PRIMARY KEY (Id),
	CONSTRAINT FK_Wards_Districts_DistrictId FOREIGN KEY (DistrictId) REFERENCES [HRM-Demo-1].dbo.Districts(Id)
);
 CREATE NONCLUSTERED INDEX IX_Wards_DistrictId ON HRM-Demo-1.dbo.Wards (  DistrictId ASC  )  
	 WITH (  PAD_INDEX = OFF ,FILLFACTOR = 100  ,SORT_IN_TEMPDB = OFF , IGNORE_DUP_KEY = OFF , STATISTICS_NORECOMPUTE = OFF , ONLINE = OFF , ALLOW_ROW_LOCKS = ON , ALLOW_PAGE_LOCKS = ON  )
	 ON [PRIMARY ] ;


-- [HRM-Demo-1].dbo.ApplyOrganizations definition

-- Drop table

-- DROP TABLE [HRM-Demo-1].dbo.ApplyOrganizations;

CREATE TABLE [HRM-Demo-1].dbo.ApplyOrganizations (
	Id int IDENTITY(1,1) NOT NULL,
	TimekeepingSettingId int NULL,
	OrganizationId int NULL,
	TimekeepingLocationOption int NOT NULL,
	RequireFaceVerification bit NULL,
	RequireDocumentAttachment bit NULL,
	RequireManagerApproval bit NULL,
	CreatedBy int NULL,
	CreatedName nvarchar(MAX) COLLATE SQL_Latin1_General_CP1_CI_AS NULL,
	CreatedAt datetime2 NULL,
	UpdatedBy int NULL,
	UpdatedName nvarchar(MAX) COLLATE SQL_Latin1_General_CP1_CI_AS NULL,
	UpdatedAt datetime2 NULL,
	IsDeleted bit NULL,
	TimekeepingType int NULL,
	TimekeepingLocationId int NULL,
	CONSTRAINT PK_ApplyOrganizations PRIMARY KEY (Id),
	CONSTRAINT FK_ApplyOrganizations_Organizations_OrganizationId FOREIGN KEY (OrganizationId) REFERENCES [HRM-Demo-1].dbo.Organizations(Id),
	CONSTRAINT FK_ApplyOrganizations_TimekeepingLocations_TimekeepingLocationId FOREIGN KEY (TimekeepingLocationId) REFERENCES [HRM-Demo-1].dbo.TimekeepingLocations(Id),
	CONSTRAINT FK_ApplyOrganizations_TimekeepingSettings_TimekeepingSettingId FOREIGN KEY (TimekeepingSettingId) REFERENCES [HRM-Demo-1].dbo.TimekeepingSettings(Id)
);
 CREATE NONCLUSTERED INDEX IX_ApplyOrganizations_OrganizationId ON HRM-Demo-1.dbo.ApplyOrganizations (  OrganizationId ASC  )  
	 WITH (  PAD_INDEX = OFF ,FILLFACTOR = 100  ,SORT_IN_TEMPDB = OFF , IGNORE_DUP_KEY = OFF , STATISTICS_NORECOMPUTE = OFF , ONLINE = OFF , ALLOW_ROW_LOCKS = ON , ALLOW_PAGE_LOCKS = ON  )
	 ON [PRIMARY ] ;
 CREATE NONCLUSTERED INDEX IX_ApplyOrganizations_TimekeepingLocationId ON HRM-Demo-1.dbo.ApplyOrganizations (  TimekeepingLocationId ASC  )  
	 WITH (  PAD_INDEX = OFF ,FILLFACTOR = 100  ,SORT_IN_TEMPDB = OFF , IGNORE_DUP_KEY = OFF , STATISTICS_NORECOMPUTE = OFF , ONLINE = OFF , ALLOW_ROW_LOCKS = ON , ALLOW_PAGE_LOCKS = ON  )
	 ON [PRIMARY ] ;
 CREATE NONCLUSTERED INDEX IX_ApplyOrganizations_TimekeepingSettingId ON HRM-Demo-1.dbo.ApplyOrganizations (  TimekeepingSettingId ASC  )  
	 WITH (  PAD_INDEX = OFF ,FILLFACTOR = 100  ,SORT_IN_TEMPDB = OFF , IGNORE_DUP_KEY = OFF , STATISTICS_NORECOMPUTE = OFF , ONLINE = OFF , ALLOW_ROW_LOCKS = ON , ALLOW_PAGE_LOCKS = ON  )
	 ON [PRIMARY ] ;


-- [HRM-Demo-1].dbo.Departments definition

-- Drop table

-- DROP TABLE [HRM-Demo-1].dbo.Departments;

CREATE TABLE [HRM-Demo-1].dbo.Departments (
	Id int IDENTITY(1,1) NOT NULL,
	OrganizationId int NULL,
	Name nvarchar(MAX) COLLATE SQL_Latin1_General_CP1_CI_AS NULL,
	Description nvarchar(MAX) COLLATE SQL_Latin1_General_CP1_CI_AS NULL,
	CreatedBy int NULL,
	CreatedName nvarchar(MAX) COLLATE SQL_Latin1_General_CP1_CI_AS NULL,
	CreatedAt datetime2 NULL,
	UpdatedBy int NULL,
	UpdatedName nvarchar(MAX) COLLATE SQL_Latin1_General_CP1_CI_AS NULL,
	UpdatedAt datetime2 NULL,
	IsDeleted bit NULL,
	CONSTRAINT PK_Departments PRIMARY KEY (Id),
	CONSTRAINT FK_Departments_Organizations_OrganizationId FOREIGN KEY (OrganizationId) REFERENCES [HRM-Demo-1].dbo.Organizations(Id)
);
 CREATE NONCLUSTERED INDEX IX_Departments_OrganizationId ON HRM-Demo-1.dbo.Departments (  OrganizationId ASC  )  
	 WITH (  PAD_INDEX = OFF ,FILLFACTOR = 100  ,SORT_IN_TEMPDB = OFF , IGNORE_DUP_KEY = OFF , STATISTICS_NORECOMPUTE = OFF , ONLINE = OFF , ALLOW_ROW_LOCKS = ON , ALLOW_PAGE_LOCKS = ON  )
	 ON [PRIMARY ] ;


-- [HRM-Demo-1].dbo.DetailTimesheetNames definition

-- Drop table

-- DROP TABLE [HRM-Demo-1].dbo.DetailTimesheetNames;

CREATE TABLE [HRM-Demo-1].dbo.DetailTimesheetNames (
	Id int IDENTITY(1,1) NOT NULL,
	OrganizationId int NULL,
	TimekeepingSheetName nvarchar(MAX) COLLATE SQL_Latin1_General_CP1_CI_AS NULL,
	StartDate datetime2 NULL,
	EndDate datetime2 NULL,
	TimekeepingMethod int NULL,
	CreatedBy int NULL,
	CreatedName nvarchar(MAX) COLLATE SQL_Latin1_General_CP1_CI_AS NULL,
	CreatedAt datetime2 NULL,
	UpdatedBy int NULL,
	UpdatedName nvarchar(MAX) COLLATE SQL_Latin1_General_CP1_CI_AS NULL,
	UpdatedAt datetime2 NULL,
	IsDeleted bit NULL,
	IsLock bit NULL,
	CONSTRAINT PK_DetailTimesheetNames PRIMARY KEY (Id),
	CONSTRAINT FK_DetailTimesheetNames_Organizations_OrganizationId FOREIGN KEY (OrganizationId) REFERENCES [HRM-Demo-1].dbo.Organizations(Id)
);
 CREATE NONCLUSTERED INDEX IX_DetailTimesheetNames_OrganizationId ON HRM-Demo-1.dbo.DetailTimesheetNames (  OrganizationId ASC  )  
	 WITH (  PAD_INDEX = OFF ,FILLFACTOR = 100  ,SORT_IN_TEMPDB = OFF , IGNORE_DUP_KEY = OFF , STATISTICS_NORECOMPUTE = OFF , ONLINE = OFF , ALLOW_ROW_LOCKS = ON , ALLOW_PAGE_LOCKS = ON  )
	 ON [PRIMARY ] ;


-- [HRM-Demo-1].dbo.Employees definition

-- Drop table

-- DROP TABLE [HRM-Demo-1].dbo.Employees;

CREATE TABLE [HRM-Demo-1].dbo.Employees (
	Id int IDENTITY(1,1) NOT NULL,
	CompanyId int NULL,
	EmployeeCode nvarchar(MAX) COLLATE SQL_Latin1_General_CP1_CI_AS NOT NULL,
	LastName nvarchar(MAX) COLLATE SQL_Latin1_General_CP1_CI_AS NOT NULL,
	FirstName nvarchar(MAX) COLLATE SQL_Latin1_General_CP1_CI_AS NOT NULL,
	DateOfBirth datetime2 NULL,
	AvatarUrl nvarchar(MAX) COLLATE SQL_Latin1_General_CP1_CI_AS NULL,
	Address nvarchar(MAX) COLLATE SQL_Latin1_General_CP1_CI_AS NULL,
	PhoneNumber nvarchar(MAX) COLLATE SQL_Latin1_General_CP1_CI_AS NULL,
	PersonalEmail nvarchar(MAX) COLLATE SQL_Latin1_General_CP1_CI_AS NULL,
	Sex int NULL,
	StaffPositionId int NULL,
	StaffTitleId int NULL,
	WorkingStatus int NOT NULL,
	ProbationDate datetime2 NULL,
	OfficialDate datetime2 NULL,
	WorkPhoneNumber nvarchar(MAX) COLLATE SQL_Latin1_General_CP1_CI_AS NULL,
	CompanyEmail nvarchar(MAX) COLLATE SQL_Latin1_General_CP1_CI_AS NULL,
	AccountEmail nvarchar(MAX) COLLATE SQL_Latin1_General_CP1_CI_AS NOT NULL,
	AccountStatus int NOT NULL,
	LeaveJobDate datetime2 NULL,
	ManagerDirectId int NULL,
	ManagerIndirectId int NULL,
	EmployeeApproveId int NULL,
	CreatedBy int NULL,
	CreatedName nvarchar(MAX) COLLATE SQL_Latin1_General_CP1_CI_AS NULL,
	CreatedAt datetime2 NULL,
	UpdatedBy int NULL,
	UpdatedName nvarchar(MAX) COLLATE SQL_Latin1_General_CP1_CI_AS NULL,
	UpdatedAt datetime2 NULL,
	IsDeleted bit NULL,
	City nvarchar(MAX) COLLATE SQL_Latin1_General_CP1_CI_AS NULL,
	CityId int NULL,
	District nvarchar(MAX) COLLATE SQL_Latin1_General_CP1_CI_AS NULL,
	DistrictId int NULL,
	Nation nvarchar(MAX) COLLATE SQL_Latin1_General_CP1_CI_AS NULL,
	NationId int NULL,
	Street nvarchar(MAX) COLLATE SQL_Latin1_General_CP1_CI_AS NULL,
	StreetId int NULL,
	Ward nvarchar(MAX) COLLATE SQL_Latin1_General_CP1_CI_AS NULL,
	WardId int NULL,
	OrganizationId int NULL,
	CONSTRAINT PK_Employees PRIMARY KEY (Id),
	CONSTRAINT FK_Employees_Companies_CompanyId FOREIGN KEY (CompanyId) REFERENCES [HRM-Demo-1].dbo.Companies(Id),
	CONSTRAINT FK_Employees_Employees_EmployeeApproveId FOREIGN KEY (EmployeeApproveId) REFERENCES [HRM-Demo-1].dbo.Employees(Id),
	CONSTRAINT FK_Employees_Employees_ManagerDirectId FOREIGN KEY (ManagerDirectId) REFERENCES [HRM-Demo-1].dbo.Employees(Id),
	CONSTRAINT FK_Employees_Employees_ManagerIndirectId FOREIGN KEY (ManagerIndirectId) REFERENCES [HRM-Demo-1].dbo.Employees(Id),
	CONSTRAINT FK_Employees_Organizations_OrganizationId FOREIGN KEY (OrganizationId) REFERENCES [HRM-Demo-1].dbo.Organizations(Id),
	CONSTRAINT FK_Employees_StaffPositions_StaffPositionId FOREIGN KEY (StaffPositionId) REFERENCES [HRM-Demo-1].dbo.StaffPositions(Id),
	CONSTRAINT FK_Employees_StaffTitles_StaffTitleId FOREIGN KEY (StaffTitleId) REFERENCES [HRM-Demo-1].dbo.StaffTitles(Id)
);
 CREATE NONCLUSTERED INDEX IX_Employees_CompanyId ON HRM-Demo-1.dbo.Employees (  CompanyId ASC  )  
	 WITH (  PAD_INDEX = OFF ,FILLFACTOR = 100  ,SORT_IN_TEMPDB = OFF , IGNORE_DUP_KEY = OFF , STATISTICS_NORECOMPUTE = OFF , ONLINE = OFF , ALLOW_ROW_LOCKS = ON , ALLOW_PAGE_LOCKS = ON  )
	 ON [PRIMARY ] ;
 CREATE NONCLUSTERED INDEX IX_Employees_EmployeeApproveId ON HRM-Demo-1.dbo.Employees (  EmployeeApproveId ASC  )  
	 WITH (  PAD_INDEX = OFF ,FILLFACTOR = 100  ,SORT_IN_TEMPDB = OFF , IGNORE_DUP_KEY = OFF , STATISTICS_NORECOMPUTE = OFF , ONLINE = OFF , ALLOW_ROW_LOCKS = ON , ALLOW_PAGE_LOCKS = ON  )
	 ON [PRIMARY ] ;
 CREATE NONCLUSTERED INDEX IX_Employees_ManagerDirectId ON HRM-Demo-1.dbo.Employees (  ManagerDirectId ASC  )  
	 WITH (  PAD_INDEX = OFF ,FILLFACTOR = 100  ,SORT_IN_TEMPDB = OFF , IGNORE_DUP_KEY = OFF , STATISTICS_NORECOMPUTE = OFF , ONLINE = OFF , ALLOW_ROW_LOCKS = ON , ALLOW_PAGE_LOCKS = ON  )
	 ON [PRIMARY ] ;
 CREATE NONCLUSTERED INDEX IX_Employees_ManagerIndirectId ON HRM-Demo-1.dbo.Employees (  ManagerIndirectId ASC  )  
	 WITH (  PAD_INDEX = OFF ,FILLFACTOR = 100  ,SORT_IN_TEMPDB = OFF , IGNORE_DUP_KEY = OFF , STATISTICS_NORECOMPUTE = OFF , ONLINE = OFF , ALLOW_ROW_LOCKS = ON , ALLOW_PAGE_LOCKS = ON  )
	 ON [PRIMARY ] ;
 CREATE NONCLUSTERED INDEX IX_Employees_OrganizationId ON HRM-Demo-1.dbo.Employees (  OrganizationId ASC  )  
	 WITH (  PAD_INDEX = OFF ,FILLFACTOR = 100  ,SORT_IN_TEMPDB = OFF , IGNORE_DUP_KEY = OFF , STATISTICS_NORECOMPUTE = OFF , ONLINE = OFF , ALLOW_ROW_LOCKS = ON , ALLOW_PAGE_LOCKS = ON  )
	 ON [PRIMARY ] ;
 CREATE NONCLUSTERED INDEX IX_Employees_StaffPositionId ON HRM-Demo-1.dbo.Employees (  StaffPositionId ASC  )  
	 WITH (  PAD_INDEX = OFF ,FILLFACTOR = 100  ,SORT_IN_TEMPDB = OFF , IGNORE_DUP_KEY = OFF , STATISTICS_NORECOMPUTE = OFF , ONLINE = OFF , ALLOW_ROW_LOCKS = ON , ALLOW_PAGE_LOCKS = ON  )
	 ON [PRIMARY ] ;
 CREATE NONCLUSTERED INDEX IX_Employees_StaffTitleId ON HRM-Demo-1.dbo.Employees (  StaffTitleId ASC  )  
	 WITH (  PAD_INDEX = OFF ,FILLFACTOR = 100  ,SORT_IN_TEMPDB = OFF , IGNORE_DUP_KEY = OFF , STATISTICS_NORECOMPUTE = OFF , ONLINE = OFF , ALLOW_ROW_LOCKS = ON , ALLOW_PAGE_LOCKS = ON  )
	 ON [PRIMARY ] ;


-- [HRM-Demo-1].dbo.GeneralLeaveRegulations definition

-- Drop table

-- DROP TABLE [HRM-Demo-1].dbo.GeneralLeaveRegulations;

CREATE TABLE [HRM-Demo-1].dbo.GeneralLeaveRegulations (
	Id int IDENTITY(1,1) NOT NULL,
	OrganizationId int NULL,
	AdmissionDay int NULL,
	MonthlyLeaveAccrual int NULL,
	LeaveCalculationStartPoint int NULL,
	SeniorityMonths int NOT NULL,
	LeaveCalculationForPartialMonth int NOT NULL,
	NumberOfDaysOff int NULL,
	CreatedBy int NULL,
	CreatedName nvarchar(MAX) COLLATE SQL_Latin1_General_CP1_CI_AS NULL,
	CreatedAt datetime2 NULL,
	UpdatedBy int NULL,
	UpdatedName nvarchar(MAX) COLLATE SQL_Latin1_General_CP1_CI_AS NULL,
	UpdatedAt datetime2 NULL,
	IsDeleted bit NULL,
	CONSTRAINT PK_GeneralLeaveRegulations PRIMARY KEY (Id),
	CONSTRAINT FK_GeneralLeaveRegulations_Organizations_OrganizationId FOREIGN KEY (OrganizationId) REFERENCES [HRM-Demo-1].dbo.Organizations(Id)
);
 CREATE NONCLUSTERED INDEX IX_GeneralLeaveRegulations_OrganizationId ON HRM-Demo-1.dbo.GeneralLeaveRegulations (  OrganizationId ASC  )  
	 WITH (  PAD_INDEX = OFF ,FILLFACTOR = 100  ,SORT_IN_TEMPDB = OFF , IGNORE_DUP_KEY = OFF , STATISTICS_NORECOMPUTE = OFF , ONLINE = OFF , ALLOW_ROW_LOCKS = ON , ALLOW_PAGE_LOCKS = ON  )
	 ON [PRIMARY ] ;


-- [HRM-Demo-1].dbo.Holidays definition

-- Drop table

-- DROP TABLE [HRM-Demo-1].dbo.Holidays;

CREATE TABLE [HRM-Demo-1].dbo.Holidays (
	Id int IDENTITY(1,1) NOT NULL,
	OrganizationId int NULL,
	Name nvarchar(MAX) COLLATE SQL_Latin1_General_CP1_CI_AS NULL,
	IsACompensatoryDayOff bit NULL,
	FromDate datetime2 NOT NULL,
	ToDate datetime2 NOT NULL,
	Note nvarchar(MAX) COLLATE SQL_Latin1_General_CP1_CI_AS NULL,
	ApplyObject int NOT NULL,
	CreatedBy int NULL,
	CreatedName nvarchar(MAX) COLLATE SQL_Latin1_General_CP1_CI_AS NULL,
	CreatedAt datetime2 NULL,
	UpdatedBy int NULL,
	UpdatedName nvarchar(MAX) COLLATE SQL_Latin1_General_CP1_CI_AS NULL,
	UpdatedAt datetime2 NULL,
	IsDeleted bit NULL,
	CONSTRAINT PK_Holidays PRIMARY KEY (Id),
	CONSTRAINT FK_Holidays_Organizations_OrganizationId FOREIGN KEY (OrganizationId) REFERENCES [HRM-Demo-1].dbo.Organizations(Id)
);
 CREATE NONCLUSTERED INDEX IX_Holidays_OrganizationId ON HRM-Demo-1.dbo.Holidays (  OrganizationId ASC  )  
	 WITH (  PAD_INDEX = OFF ,FILLFACTOR = 100  ,SORT_IN_TEMPDB = OFF , IGNORE_DUP_KEY = OFF , STATISTICS_NORECOMPUTE = OFF , ONLINE = OFF , ALLOW_ROW_LOCKS = ON , ALLOW_PAGE_LOCKS = ON  )
	 ON [PRIMARY ] ;


-- [HRM-Demo-1].dbo.InfoJobs definition

-- Drop table

-- DROP TABLE [HRM-Demo-1].dbo.InfoJobs;

CREATE TABLE [HRM-Demo-1].dbo.InfoJobs (
	Id int IDENTITY(1,1) NOT NULL,
	EmployeeId int NULL,
	OrganizationId int NULL,
	StaffPositioId int NULL,
	StaffTitleId int NULL,
	WorkingStatus int NOT NULL,
	TimeKeepingId int NULL,
	NatureOfLaborId int NULL,
	NatureOfLaborName nvarchar(MAX) COLLATE SQL_Latin1_General_CP1_CI_AS NULL,
	WorkingArea nvarchar(MAX) COLLATE SQL_Latin1_General_CP1_CI_AS NULL,
	WorkingLocation nvarchar(MAX) COLLATE SQL_Latin1_General_CP1_CI_AS NULL,
	ContractTypeId int NULL,
	InternshipStartDate datetime2 NULL,
	ProbationStartDate datetime2 NULL,
	OfficialStartDate datetime2 NULL,
	Seniority float NULL,
	RetiredDate datetime2 NULL,
	ReasonGroupQuitJob nvarchar(MAX) COLLATE SQL_Latin1_General_CP1_CI_AS NULL,
	ReasonQuitJob nvarchar(MAX) COLLATE SQL_Latin1_General_CP1_CI_AS NULL,
	QuitJobDate datetime2 NULL,
	OpinionContribute nvarchar(MAX) COLLATE SQL_Latin1_General_CP1_CI_AS NULL,
	SalaryLevel decimal(18,2) NULL,
	BasicSalary decimal(18,2) NULL,
	InsuranceSalary decimal(18,2) NULL,
	TotalSalary decimal(18,2) NULL,
	BankAccountNumber nvarchar(MAX) COLLATE SQL_Latin1_General_CP1_CI_AS NULL,
	BankName nvarchar(MAX) COLLATE SQL_Latin1_General_CP1_CI_AS NULL,
	CreatedBy int NULL,
	CreatedName nvarchar(MAX) COLLATE SQL_Latin1_General_CP1_CI_AS NULL,
	CreatedAt datetime2 NULL,
	UpdatedBy int NULL,
	UpdatedName nvarchar(MAX) COLLATE SQL_Latin1_General_CP1_CI_AS NULL,
	UpdatedAt datetime2 NULL,
	IsDeleted bit NULL,
	CONSTRAINT PK_InfoJobs PRIMARY KEY (Id),
	CONSTRAINT FK_InfoJobs_ContractTypes_ContractTypeId FOREIGN KEY (ContractTypeId) REFERENCES [HRM-Demo-1].dbo.ContractTypes(Id),
	CONSTRAINT FK_InfoJobs_Employees_EmployeeId FOREIGN KEY (EmployeeId) REFERENCES [HRM-Demo-1].dbo.Employees(Id),
	CONSTRAINT FK_InfoJobs_NatureOfLabor_NatureOfLaborId FOREIGN KEY (NatureOfLaborId) REFERENCES [HRM-Demo-1].dbo.NatureOfLabor(Id)
);
 CREATE NONCLUSTERED INDEX IX_InfoJobs_ContractTypeId ON HRM-Demo-1.dbo.InfoJobs (  ContractTypeId ASC  )  
	 WITH (  PAD_INDEX = OFF ,FILLFACTOR = 100  ,SORT_IN_TEMPDB = OFF , IGNORE_DUP_KEY = OFF , STATISTICS_NORECOMPUTE = OFF , ONLINE = OFF , ALLOW_ROW_LOCKS = ON , ALLOW_PAGE_LOCKS = ON  )
	 ON [PRIMARY ] ;
 CREATE UNIQUE NONCLUSTERED INDEX IX_InfoJobs_EmployeeId ON HRM-Demo-1.dbo.InfoJobs (  EmployeeId ASC  )  
	 WHERE  ([EmployeeId] IS NOT NULL)
	 WITH (  PAD_INDEX = OFF ,FILLFACTOR = 100  ,SORT_IN_TEMPDB = OFF , IGNORE_DUP_KEY = OFF , STATISTICS_NORECOMPUTE = OFF , ONLINE = OFF , ALLOW_ROW_LOCKS = ON , ALLOW_PAGE_LOCKS = ON  )
	 ON [PRIMARY ] ;
 CREATE NONCLUSTERED INDEX IX_InfoJobs_NatureOfLaborId ON HRM-Demo-1.dbo.InfoJobs (  NatureOfLaborId ASC  )  
	 WITH (  PAD_INDEX = OFF ,FILLFACTOR = 100  ,SORT_IN_TEMPDB = OFF , IGNORE_DUP_KEY = OFF , STATISTICS_NORECOMPUTE = OFF , ONLINE = OFF , ALLOW_ROW_LOCKS = ON , ALLOW_PAGE_LOCKS = ON  )
	 ON [PRIMARY ] ;


-- [HRM-Demo-1].dbo.KpiTables definition

-- Drop table

-- DROP TABLE [HRM-Demo-1].dbo.KpiTables;

CREATE TABLE [HRM-Demo-1].dbo.KpiTables (
	Id int IDENTITY(1,1) NOT NULL,
	NameKpiTable nvarchar(MAX) COLLATE SQL_Latin1_General_CP1_CI_AS NULL,
	OrganizationId int NULL,
	ToDate datetime2 NULL,
	StaffPositionId int NULL,
	CreatedBy int NULL,
	CreatedName nvarchar(MAX) COLLATE SQL_Latin1_General_CP1_CI_AS NULL,
	CreatedAt datetime2 NULL,
	UpdatedBy int NULL,
	UpdatedName nvarchar(MAX) COLLATE SQL_Latin1_General_CP1_CI_AS NULL,
	UpdatedAt datetime2 NULL,
	IsDeleted bit NULL,
	FromDate datetime2 NULL,
	CONSTRAINT PK_KpiTables PRIMARY KEY (Id),
	CONSTRAINT FK_KpiTables_Organizations_OrganizationId FOREIGN KEY (OrganizationId) REFERENCES [HRM-Demo-1].dbo.Organizations(Id)
);
 CREATE NONCLUSTERED INDEX IX_KpiTables_OrganizationId ON HRM-Demo-1.dbo.KpiTables (  OrganizationId ASC  )  
	 WITH (  PAD_INDEX = OFF ,FILLFACTOR = 100  ,SORT_IN_TEMPDB = OFF , IGNORE_DUP_KEY = OFF , STATISTICS_NORECOMPUTE = OFF , ONLINE = OFF , ALLOW_ROW_LOCKS = ON , ALLOW_PAGE_LOCKS = ON  )
	 ON [PRIMARY ] ;


-- [HRM-Demo-1].dbo.LeaveApplications definition

-- Drop table

-- DROP TABLE [HRM-Demo-1].dbo.LeaveApplications;

CREATE TABLE [HRM-Demo-1].dbo.LeaveApplications (
	Id int IDENTITY(1,1) NOT NULL,
	EmployeeId int NULL,
	StartDate datetime2 NULL,
	EndDate datetime2 NULL,
	NumberOfDays float NULL,
	TypeOfLeaveId int NULL,
	SalaryPercentage decimal(18,2) NULL,
	ReasonForLeave nvarchar(MAX) COLLATE SQL_Latin1_General_CP1_CI_AS NULL,
	Note nvarchar(MAX) COLLATE SQL_Latin1_General_CP1_CI_AS NULL,
	Status int NULL,
	CreatedBy int NULL,
	CreatedName nvarchar(MAX) COLLATE SQL_Latin1_General_CP1_CI_AS NULL,
	CreatedAt datetime2 NULL,
	UpdatedBy int NULL,
	UpdatedName nvarchar(MAX) COLLATE SQL_Latin1_General_CP1_CI_AS NULL,
	UpdatedAt datetime2 NULL,
	IsDeleted bit NULL,
	OrganizationId int NULL,
	ApproverNote nvarchar(MAX) COLLATE SQL_Latin1_General_CP1_CI_AS NULL,
	OnPaidLeaveStatus int NULL,
	CONSTRAINT PK_LeaveApplications PRIMARY KEY (Id),
	CONSTRAINT FK_LeaveApplications_Employees_EmployeeId FOREIGN KEY (EmployeeId) REFERENCES [HRM-Demo-1].dbo.Employees(Id),
	CONSTRAINT FK_LeaveApplications_Organizations_OrganizationId FOREIGN KEY (OrganizationId) REFERENCES [HRM-Demo-1].dbo.Organizations(Id),
	CONSTRAINT FK_LeaveApplications_TypeOfLeaves_TypeOfLeaveId FOREIGN KEY (TypeOfLeaveId) REFERENCES [HRM-Demo-1].dbo.TypeOfLeaves(Id)
);
 CREATE NONCLUSTERED INDEX IX_LeaveApplications_EmployeeId ON HRM-Demo-1.dbo.LeaveApplications (  EmployeeId ASC  )  
	 WITH (  PAD_INDEX = OFF ,FILLFACTOR = 100  ,SORT_IN_TEMPDB = OFF , IGNORE_DUP_KEY = OFF , STATISTICS_NORECOMPUTE = OFF , ONLINE = OFF , ALLOW_ROW_LOCKS = ON , ALLOW_PAGE_LOCKS = ON  )
	 ON [PRIMARY ] ;
 CREATE NONCLUSTERED INDEX IX_LeaveApplications_OrganizationId ON HRM-Demo-1.dbo.LeaveApplications (  OrganizationId ASC  )  
	 WITH (  PAD_INDEX = OFF ,FILLFACTOR = 100  ,SORT_IN_TEMPDB = OFF , IGNORE_DUP_KEY = OFF , STATISTICS_NORECOMPUTE = OFF , ONLINE = OFF , ALLOW_ROW_LOCKS = ON , ALLOW_PAGE_LOCKS = ON  )
	 ON [PRIMARY ] ;
 CREATE NONCLUSTERED INDEX IX_LeaveApplications_TypeOfLeaveId ON HRM-Demo-1.dbo.LeaveApplications (  TypeOfLeaveId ASC  )  
	 WITH (  PAD_INDEX = OFF ,FILLFACTOR = 100  ,SORT_IN_TEMPDB = OFF , IGNORE_DUP_KEY = OFF , STATISTICS_NORECOMPUTE = OFF , ONLINE = OFF , ALLOW_ROW_LOCKS = ON , ALLOW_PAGE_LOCKS = ON  )
	 ON [PRIMARY ] ;


-- [HRM-Demo-1].dbo.LeavePermissions definition

-- Drop table

-- DROP TABLE [HRM-Demo-1].dbo.LeavePermissions;

CREATE TABLE [HRM-Demo-1].dbo.LeavePermissions (
	Id int IDENTITY(1,1) NOT NULL,
	ContractId int NULL,
	LeaveApplicationId int NULL,
	EmployeeId int NOT NULL,
	NumerOfLeave float NOT NULL,
	[Date] datetime2 NOT NULL,
	LeavePerrmissionStatus int NOT NULL,
	CreatedBy int NULL,
	CreatedName nvarchar(MAX) COLLATE SQL_Latin1_General_CP1_CI_AS NULL,
	CreatedAt datetime2 NULL,
	UpdatedBy int NULL,
	UpdatedName nvarchar(MAX) COLLATE SQL_Latin1_General_CP1_CI_AS NULL,
	UpdatedAt datetime2 NULL,
	IsDeleted bit NULL,
	CONSTRAINT PK_LeavePermissions PRIMARY KEY (Id),
	CONSTRAINT FK_LeavePermissions_Employees_EmployeeId FOREIGN KEY (EmployeeId) REFERENCES [HRM-Demo-1].dbo.Employees(Id) ON DELETE CASCADE,
	CONSTRAINT FK_LeavePermissions_LeaveApplications_LeaveApplicationId FOREIGN KEY (LeaveApplicationId) REFERENCES [HRM-Demo-1].dbo.LeaveApplications(Id)
);
 CREATE UNIQUE NONCLUSTERED INDEX IX_LeavePermissions_EmployeeId ON HRM-Demo-1.dbo.LeavePermissions (  EmployeeId ASC  )  
	 WITH (  PAD_INDEX = OFF ,FILLFACTOR = 100  ,SORT_IN_TEMPDB = OFF , IGNORE_DUP_KEY = OFF , STATISTICS_NORECOMPUTE = OFF , ONLINE = OFF , ALLOW_ROW_LOCKS = ON , ALLOW_PAGE_LOCKS = ON  )
	 ON [PRIMARY ] ;
 CREATE NONCLUSTERED INDEX IX_LeavePermissions_LeaveApplicationId ON HRM-Demo-1.dbo.LeavePermissions (  LeaveApplicationId ASC  )  
	 WITH (  PAD_INDEX = OFF ,FILLFACTOR = 100  ,SORT_IN_TEMPDB = OFF , IGNORE_DUP_KEY = OFF , STATISTICS_NORECOMPUTE = OFF , ONLINE = OFF , ALLOW_ROW_LOCKS = ON , ALLOW_PAGE_LOCKS = ON  )
	 ON [PRIMARY ] ;


-- [HRM-Demo-1].dbo.OrganizationLeaders definition

-- Drop table

-- DROP TABLE [HRM-Demo-1].dbo.OrganizationLeaders;

CREATE TABLE [HRM-Demo-1].dbo.OrganizationLeaders (
	OrganizationId int NOT NULL,
	EmployeeId int NOT NULL,
	OrganizationLeaderType int NOT NULL,
	CONSTRAINT PK_OrganizationLeaders PRIMARY KEY (OrganizationId,EmployeeId),
	CONSTRAINT FK_OrganizationLeaders_Employees_EmployeeId FOREIGN KEY (EmployeeId) REFERENCES [HRM-Demo-1].dbo.Employees(Id),
	CONSTRAINT FK_OrganizationLeaders_Organizations_OrganizationId FOREIGN KEY (OrganizationId) REFERENCES [HRM-Demo-1].dbo.Organizations(Id)
);
 CREATE NONCLUSTERED INDEX IX_OrganizationLeaders_EmployeeId ON HRM-Demo-1.dbo.OrganizationLeaders (  EmployeeId ASC  )  
	 WITH (  PAD_INDEX = OFF ,FILLFACTOR = 100  ,SORT_IN_TEMPDB = OFF , IGNORE_DUP_KEY = OFF , STATISTICS_NORECOMPUTE = OFF , ONLINE = OFF , ALLOW_ROW_LOCKS = ON , ALLOW_PAGE_LOCKS = ON  )
	 ON [PRIMARY ] ;


-- [HRM-Demo-1].dbo.OrganizationPositions definition

-- Drop table

-- DROP TABLE [HRM-Demo-1].dbo.OrganizationPositions;

CREATE TABLE [HRM-Demo-1].dbo.OrganizationPositions (
	StaffPositionId int NOT NULL,
	OrganizationId int NOT NULL,
	CONSTRAINT PK_OrganizationPositions PRIMARY KEY (StaffPositionId,OrganizationId),
	CONSTRAINT FK_OrganizationPositions_Organizations_OrganizationId FOREIGN KEY (OrganizationId) REFERENCES [HRM-Demo-1].dbo.Organizations(Id) ON DELETE CASCADE,
	CONSTRAINT FK_OrganizationPositions_StaffPositions_StaffPositionId FOREIGN KEY (StaffPositionId) REFERENCES [HRM-Demo-1].dbo.StaffPositions(Id) ON DELETE CASCADE
);
 CREATE NONCLUSTERED INDEX IX_OrganizationPositions_OrganizationId ON HRM-Demo-1.dbo.OrganizationPositions (  OrganizationId ASC  )  
	 WITH (  PAD_INDEX = OFF ,FILLFACTOR = 100  ,SORT_IN_TEMPDB = OFF , IGNORE_DUP_KEY = OFF , STATISTICS_NORECOMPUTE = OFF , ONLINE = OFF , ALLOW_ROW_LOCKS = ON , ALLOW_PAGE_LOCKS = ON  )
	 ON [PRIMARY ] ;


-- [HRM-Demo-1].dbo.PayrollStaffPositions definition

-- Drop table

-- DROP TABLE [HRM-Demo-1].dbo.PayrollStaffPositions;

CREATE TABLE [HRM-Demo-1].dbo.PayrollStaffPositions (
	PayrollId int NOT NULL,
	StaffPositionId int NOT NULL,
	CONSTRAINT PK_PayrollStaffPositions PRIMARY KEY (PayrollId,StaffPositionId),
	CONSTRAINT FK_PayrollStaffPositions_Payrolls_PayrollId FOREIGN KEY (PayrollId) REFERENCES [HRM-Demo-1].dbo.Payrolls(Id) ON DELETE CASCADE,
	CONSTRAINT FK_PayrollStaffPositions_StaffPositions_StaffPositionId FOREIGN KEY (StaffPositionId) REFERENCES [HRM-Demo-1].dbo.StaffPositions(Id) ON DELETE CASCADE
);
 CREATE NONCLUSTERED INDEX IX_PayrollStaffPositions_StaffPositionId ON HRM-Demo-1.dbo.PayrollStaffPositions (  StaffPositionId ASC  )  
	 WITH (  PAD_INDEX = OFF ,FILLFACTOR = 100  ,SORT_IN_TEMPDB = OFF , IGNORE_DUP_KEY = OFF , STATISTICS_NORECOMPUTE = OFF , ONLINE = OFF , ALLOW_ROW_LOCKS = ON , ALLOW_PAGE_LOCKS = ON  )
	 ON [PRIMARY ] ;


-- [HRM-Demo-1].dbo.PayrollSummaryTimesheets definition

-- Drop table

-- DROP TABLE [HRM-Demo-1].dbo.PayrollSummaryTimesheets;

CREATE TABLE [HRM-Demo-1].dbo.PayrollSummaryTimesheets (
	PayrollId int NOT NULL,
	SummaryTimesheetNameId int NOT NULL,
	CONSTRAINT PK_PayrollSummaryTimesheets PRIMARY KEY (PayrollId,SummaryTimesheetNameId),
	CONSTRAINT FK_PayrollSummaryTimesheets_Payrolls_PayrollId FOREIGN KEY (PayrollId) REFERENCES [HRM-Demo-1].dbo.Payrolls(Id) ON DELETE CASCADE,
	CONSTRAINT FK_PayrollSummaryTimesheets_SummaryTimesheetNames_SummaryTimesheetNameId FOREIGN KEY (SummaryTimesheetNameId) REFERENCES [HRM-Demo-1].dbo.SummaryTimesheetNames(Id) ON DELETE CASCADE
);
 CREATE NONCLUSTERED INDEX IX_PayrollSummaryTimesheets_SummaryTimesheetNameId ON HRM-Demo-1.dbo.PayrollSummaryTimesheets (  SummaryTimesheetNameId ASC  )  
	 WITH (  PAD_INDEX = OFF ,FILLFACTOR = 100  ,SORT_IN_TEMPDB = OFF , IGNORE_DUP_KEY = OFF , STATISTICS_NORECOMPUTE = OFF , ONLINE = OFF , ALLOW_ROW_LOCKS = ON , ALLOW_PAGE_LOCKS = ON  )
	 ON [PRIMARY ] ;


-- [HRM-Demo-1].dbo.ProfileInfos definition

-- Drop table

-- DROP TABLE [HRM-Demo-1].dbo.ProfileInfos;

CREATE TABLE [HRM-Demo-1].dbo.ProfileInfos (
	Id int IDENTITY(1,1) NOT NULL,
	EmployeeId int NULL,
	ProfileCode nvarchar(MAX) COLLATE SQL_Latin1_General_CP1_CI_AS NULL,
	AnotherName nvarchar(MAX) COLLATE SQL_Latin1_General_CP1_CI_AS NULL,
	BornLocation nvarchar(MAX) COLLATE SQL_Latin1_General_CP1_CI_AS NULL,
	OriginalLocation nvarchar(MAX) COLLATE SQL_Latin1_General_CP1_CI_AS NULL,
	MarriageStatus nvarchar(MAX) COLLATE SQL_Latin1_General_CP1_CI_AS NULL,
	PersonalTaxNumber nvarchar(MAX) COLLATE SQL_Latin1_General_CP1_CI_AS NULL,
	TypeFamily nvarchar(MAX) COLLATE SQL_Latin1_General_CP1_CI_AS NULL,
	TypePersonal nvarchar(MAX) COLLATE SQL_Latin1_General_CP1_CI_AS NULL,
	Tripe nvarchar(MAX) COLLATE SQL_Latin1_General_CP1_CI_AS NULL,
	Religion nvarchar(MAX) COLLATE SQL_Latin1_General_CP1_CI_AS NULL,
	Nation nvarchar(MAX) COLLATE SQL_Latin1_General_CP1_CI_AS NULL,
	TypePaper nvarchar(MAX) COLLATE SQL_Latin1_General_CP1_CI_AS NULL,
	PaperNumber nvarchar(MAX) COLLATE SQL_Latin1_General_CP1_CI_AS NULL,
	PaperProvideDate datetime2 NULL,
	PaperProvideLocation nvarchar(MAX) COLLATE SQL_Latin1_General_CP1_CI_AS NULL,
	ExpirePaperDate datetime2 NULL,
	PassportNumber nvarchar(MAX) COLLATE SQL_Latin1_General_CP1_CI_AS NULL,
	PassportProvideDate datetime2 NULL,
	PassportProvideLocation nvarchar(MAX) COLLATE SQL_Latin1_General_CP1_CI_AS NULL,
	ExpirePassportDate datetime2 NULL,
	CultureLevel nvarchar(MAX) COLLATE SQL_Latin1_General_CP1_CI_AS NULL,
	EducationLevel nvarchar(MAX) COLLATE SQL_Latin1_General_CP1_CI_AS NULL,
	EducationTraningLocation nvarchar(MAX) COLLATE SQL_Latin1_General_CP1_CI_AS NULL,
	Faculty nvarchar(MAX) COLLATE SQL_Latin1_General_CP1_CI_AS NULL,
	Specialized nvarchar(MAX) COLLATE SQL_Latin1_General_CP1_CI_AS NULL,
	GraduateDate nvarchar(MAX) COLLATE SQL_Latin1_General_CP1_CI_AS NULL,
	GraduationClassification nvarchar(MAX) COLLATE SQL_Latin1_General_CP1_CI_AS NULL,
	CreatedBy int NULL,
	CreatedName nvarchar(MAX) COLLATE SQL_Latin1_General_CP1_CI_AS NULL,
	CreatedAt datetime2 NULL,
	UpdatedBy int NULL,
	UpdatedName nvarchar(MAX) COLLATE SQL_Latin1_General_CP1_CI_AS NULL,
	UpdatedAt datetime2 NULL,
	IsDeleted bit NULL,
	CONSTRAINT PK_ProfileInfos PRIMARY KEY (Id),
	CONSTRAINT FK_ProfileInfos_Employees_EmployeeId FOREIGN KEY (EmployeeId) REFERENCES [HRM-Demo-1].dbo.Employees(Id)
);
 CREATE UNIQUE NONCLUSTERED INDEX IX_ProfileInfos_EmployeeId ON HRM-Demo-1.dbo.ProfileInfos (  EmployeeId ASC  )  
	 WHERE  ([EmployeeId] IS NOT NULL)
	 WITH (  PAD_INDEX = OFF ,FILLFACTOR = 100  ,SORT_IN_TEMPDB = OFF , IGNORE_DUP_KEY = OFF , STATISTICS_NORECOMPUTE = OFF , ONLINE = OFF , ALLOW_ROW_LOCKS = ON , ALLOW_PAGE_LOCKS = ON  )
	 ON [PRIMARY ] ;


-- [HRM-Demo-1].dbo.Projects definition

-- Drop table

-- DROP TABLE [HRM-Demo-1].dbo.Projects;

CREATE TABLE [HRM-Demo-1].dbo.Projects (
	Id int IDENTITY(1,1) NOT NULL,
	DepartmentId int NULL,
	Name nvarchar(MAX) COLLATE SQL_Latin1_General_CP1_CI_AS NULL,
	Description nvarchar(MAX) COLLATE SQL_Latin1_General_CP1_CI_AS NULL,
	ProjectType int NOT NULL,
	StartDate datetime2 NULL,
	EndDate datetime2 NULL,
	CreatedBy int NULL,
	CreatedName nvarchar(MAX) COLLATE SQL_Latin1_General_CP1_CI_AS NULL,
	CreatedAt datetime2 NULL,
	UpdatedBy int NULL,
	UpdatedName nvarchar(MAX) COLLATE SQL_Latin1_General_CP1_CI_AS NULL,
	UpdatedAt datetime2 NULL,
	IsDeleted bit NULL,
	OrganizationId int NULL,
	CONSTRAINT PK_Projects PRIMARY KEY (Id),
	CONSTRAINT FK_Projects_Departments_DepartmentId FOREIGN KEY (DepartmentId) REFERENCES [HRM-Demo-1].dbo.Departments(Id)
);
 CREATE NONCLUSTERED INDEX IX_Projects_DepartmentId ON HRM-Demo-1.dbo.Projects (  DepartmentId ASC  )  
	 WITH (  PAD_INDEX = OFF ,FILLFACTOR = 100  ,SORT_IN_TEMPDB = OFF , IGNORE_DUP_KEY = OFF , STATISTICS_NORECOMPUTE = OFF , ONLINE = OFF , ALLOW_ROW_LOCKS = ON , ALLOW_PAGE_LOCKS = ON  )
	 ON [PRIMARY ] ;


-- [HRM-Demo-1].dbo.ResignationLetters definition

-- Drop table

-- DROP TABLE [HRM-Demo-1].dbo.ResignationLetters;

CREATE TABLE [HRM-Demo-1].dbo.ResignationLetters (
	Id int IDENTITY(1,1) NOT NULL,
	CreatedBy int NULL,
	CreatedName nvarchar(MAX) COLLATE SQL_Latin1_General_CP1_CI_AS NULL,
	CreatedAt datetime2 NULL,
	UpdatedBy int NULL,
	UpdatedName nvarchar(MAX) COLLATE SQL_Latin1_General_CP1_CI_AS NULL,
	UpdatedAt datetime2 NULL,
	IsDeleted bit NULL,
	EmployeeId int NULL,
	CONSTRAINT PK_ResignationLetters PRIMARY KEY (Id),
	CONSTRAINT FK_ResignationLetters_Employees_EmployeeId FOREIGN KEY (EmployeeId) REFERENCES [HRM-Demo-1].dbo.Employees(Id)
);
 CREATE NONCLUSTERED INDEX IX_ResignationLetters_EmployeeId ON HRM-Demo-1.dbo.ResignationLetters (  EmployeeId ASC  )  
	 WITH (  PAD_INDEX = OFF ,FILLFACTOR = 100  ,SORT_IN_TEMPDB = OFF , IGNORE_DUP_KEY = OFF , STATISTICS_NORECOMPUTE = OFF , ONLINE = OFF , ALLOW_ROW_LOCKS = ON , ALLOW_PAGE_LOCKS = ON  )
	 ON [PRIMARY ] ;


-- [HRM-Demo-1].dbo.SummaryTimesheetNameDetailTimesheetNames definition

-- Drop table

-- DROP TABLE [HRM-Demo-1].dbo.SummaryTimesheetNameDetailTimesheetNames;

CREATE TABLE [HRM-Demo-1].dbo.SummaryTimesheetNameDetailTimesheetNames (
	SummaryTimesheetNameId int NOT NULL,
	DetailTimesheetNameId int NOT NULL,
	CONSTRAINT PK_SummaryTimesheetNameDetailTimesheetNames PRIMARY KEY (SummaryTimesheetNameId,DetailTimesheetNameId),
	CONSTRAINT FK_SummaryTimesheetNameDetailTimesheetNames_DetailTimesheetNames_DetailTimesheetNameId FOREIGN KEY (DetailTimesheetNameId) REFERENCES [HRM-Demo-1].dbo.DetailTimesheetNames(Id) ON DELETE CASCADE,
	CONSTRAINT FK_SummaryTimesheetNameDetailTimesheetNames_SummaryTimesheetNames_SummaryTimesheetNameId FOREIGN KEY (SummaryTimesheetNameId) REFERENCES [HRM-Demo-1].dbo.SummaryTimesheetNames(Id) ON DELETE CASCADE
);
 CREATE NONCLUSTERED INDEX IX_SummaryTimesheetNameDetailTimesheetNames_DetailTimesheetNameId ON HRM-Demo-1.dbo.SummaryTimesheetNameDetailTimesheetNames (  DetailTimesheetNameId ASC  )  
	 WITH (  PAD_INDEX = OFF ,FILLFACTOR = 100  ,SORT_IN_TEMPDB = OFF , IGNORE_DUP_KEY = OFF , STATISTICS_NORECOMPUTE = OFF , ONLINE = OFF , ALLOW_ROW_LOCKS = ON , ALLOW_PAGE_LOCKS = ON  )
	 ON [PRIMARY ] ;


-- [HRM-Demo-1].dbo.SummaryTimesheetNameEmployeeConfirms definition

-- Drop table

-- DROP TABLE [HRM-Demo-1].dbo.SummaryTimesheetNameEmployeeConfirms;

CREATE TABLE [HRM-Demo-1].dbo.SummaryTimesheetNameEmployeeConfirms (
	Id int IDENTITY(1,1) NOT NULL,
	SummaryTimesheetNameId int NULL,
	EmployeeId int NULL,
	Status int NULL,
	Note nvarchar(MAX) COLLATE SQL_Latin1_General_CP1_CI_AS NULL,
	[Date] datetime2 NULL,
	CreatedBy int NULL,
	CreatedName nvarchar(MAX) COLLATE SQL_Latin1_General_CP1_CI_AS NULL,
	CreatedAt datetime2 NULL,
	UpdatedBy int NULL,
	UpdatedName nvarchar(MAX) COLLATE SQL_Latin1_General_CP1_CI_AS NULL,
	UpdatedAt datetime2 NULL,
	IsDeleted bit NULL,
	CONSTRAINT PK_SummaryTimesheetNameEmployeeConfirms PRIMARY KEY (Id),
	CONSTRAINT FK_SummaryTimesheetNameEmployeeConfirms_Employees_EmployeeId FOREIGN KEY (EmployeeId) REFERENCES [HRM-Demo-1].dbo.Employees(Id),
	CONSTRAINT FK_SummaryTimesheetNameEmployeeConfirms_SummaryTimesheetNames_SummaryTimesheetNameId FOREIGN KEY (SummaryTimesheetNameId) REFERENCES [HRM-Demo-1].dbo.SummaryTimesheetNames(Id)
);
 CREATE NONCLUSTERED INDEX IX_SummaryTimesheetNameEmployeeConfirms_EmployeeId ON HRM-Demo-1.dbo.SummaryTimesheetNameEmployeeConfirms (  EmployeeId ASC  )  
	 WITH (  PAD_INDEX = OFF ,FILLFACTOR = 100  ,SORT_IN_TEMPDB = OFF , IGNORE_DUP_KEY = OFF , STATISTICS_NORECOMPUTE = OFF , ONLINE = OFF , ALLOW_ROW_LOCKS = ON , ALLOW_PAGE_LOCKS = ON  )
	 ON [PRIMARY ] ;
 CREATE NONCLUSTERED INDEX IX_SummaryTimesheetNameEmployeeConfirms_SummaryTimesheetNameId ON HRM-Demo-1.dbo.SummaryTimesheetNameEmployeeConfirms (  SummaryTimesheetNameId ASC  )  
	 WITH (  PAD_INDEX = OFF ,FILLFACTOR = 100  ,SORT_IN_TEMPDB = OFF , IGNORE_DUP_KEY = OFF , STATISTICS_NORECOMPUTE = OFF , ONLINE = OFF , ALLOW_ROW_LOCKS = ON , ALLOW_PAGE_LOCKS = ON  )
	 ON [PRIMARY ] ;


-- [HRM-Demo-1].dbo.SummaryTimesheetNameStaffPositions definition

-- Drop table

-- DROP TABLE [HRM-Demo-1].dbo.SummaryTimesheetNameStaffPositions;

CREATE TABLE [HRM-Demo-1].dbo.SummaryTimesheetNameStaffPositions (
	StaffPositionId int NOT NULL,
	SummaryTimesheetNameId int NOT NULL,
	CONSTRAINT PK_SummaryTimesheetNameStaffPositions PRIMARY KEY (SummaryTimesheetNameId,StaffPositionId),
	CONSTRAINT FK_SummaryTimesheetNameStaffPositions_StaffPositions_StaffPositionId FOREIGN KEY (StaffPositionId) REFERENCES [HRM-Demo-1].dbo.StaffPositions(Id) ON DELETE CASCADE,
	CONSTRAINT FK_SummaryTimesheetNameStaffPositions_SummaryTimesheetNames_SummaryTimesheetNameId FOREIGN KEY (SummaryTimesheetNameId) REFERENCES [HRM-Demo-1].dbo.SummaryTimesheetNames(Id) ON DELETE CASCADE
);
 CREATE NONCLUSTERED INDEX IX_SummaryTimesheetNameStaffPositions_StaffPositionId ON HRM-Demo-1.dbo.SummaryTimesheetNameStaffPositions (  StaffPositionId ASC  )  
	 WITH (  PAD_INDEX = OFF ,FILLFACTOR = 100  ,SORT_IN_TEMPDB = OFF , IGNORE_DUP_KEY = OFF , STATISTICS_NORECOMPUTE = OFF , ONLINE = OFF , ALLOW_ROW_LOCKS = ON , ALLOW_PAGE_LOCKS = ON  )
	 ON [PRIMARY ] ;


-- [HRM-Demo-1].dbo.Timesheets definition

-- Drop table

-- DROP TABLE [HRM-Demo-1].dbo.Timesheets;

CREATE TABLE [HRM-Demo-1].dbo.Timesheets (
	Id int IDENTITY(1,1) NOT NULL,
	ShiftWorkId int NULL,
	EmployeeId int NULL,
	[Date] datetime2 NULL,
	StartTime time NULL,
	EndTime time NULL,
	NumberOfWorkingHour float NULL,
	CreatedBy int NULL,
	CreatedName nvarchar(MAX) COLLATE SQL_Latin1_General_CP1_CI_AS NULL,
	CreatedAt datetime2 NULL,
	UpdatedBy int NULL,
	UpdatedName nvarchar(MAX) COLLATE SQL_Latin1_General_CP1_CI_AS NULL,
	UpdatedAt datetime2 NULL,
	IsDeleted bit NULL,
	TimekeepingType int NULL,
	EarlyLeaveDuration float NULL,
	LateDuration float NULL,
	TimeKeepingLeaveStatus int DEFAULT 0 NOT NULL,
	CONSTRAINT PK_Timesheets PRIMARY KEY (Id),
	CONSTRAINT FK_Timesheets_Employees_EmployeeId FOREIGN KEY (EmployeeId) REFERENCES [HRM-Demo-1].dbo.Employees(Id),
	CONSTRAINT FK_Timesheets_ShiftWorks_ShiftWorkId FOREIGN KEY (ShiftWorkId) REFERENCES [HRM-Demo-1].dbo.ShiftWorks(Id)
);
 CREATE NONCLUSTERED INDEX IX_Timesheets_EmployeeId ON HRM-Demo-1.dbo.Timesheets (  EmployeeId ASC  )  
	 WITH (  PAD_INDEX = OFF ,FILLFACTOR = 100  ,SORT_IN_TEMPDB = OFF , IGNORE_DUP_KEY = OFF , STATISTICS_NORECOMPUTE = OFF , ONLINE = OFF , ALLOW_ROW_LOCKS = ON , ALLOW_PAGE_LOCKS = ON  )
	 ON [PRIMARY ] ;
 CREATE NONCLUSTERED INDEX IX_Timesheets_ShiftWorkId ON HRM-Demo-1.dbo.Timesheets (  ShiftWorkId ASC  )  
	 WITH (  PAD_INDEX = OFF ,FILLFACTOR = 100  ,SORT_IN_TEMPDB = OFF , IGNORE_DUP_KEY = OFF , STATISTICS_NORECOMPUTE = OFF , ONLINE = OFF , ALLOW_ROW_LOCKS = ON , ALLOW_PAGE_LOCKS = ON  )
	 ON [PRIMARY ] ;


-- [HRM-Demo-1].dbo.TypeOfLeaveEmployees definition

-- Drop table

-- DROP TABLE [HRM-Demo-1].dbo.TypeOfLeaveEmployees;

CREATE TABLE [HRM-Demo-1].dbo.TypeOfLeaveEmployees (
	Id int IDENTITY(1,1) NOT NULL,
	TypeOfLeaveId int NULL,
	EmployeeId int NULL,
	DaysRemaining float NULL,
	[Year] int NULL,
	CreatedBy int NULL,
	CreatedName nvarchar(MAX) COLLATE SQL_Latin1_General_CP1_CI_AS NULL,
	CreatedAt datetime2 NULL,
	UpdatedBy int NULL,
	UpdatedName nvarchar(MAX) COLLATE SQL_Latin1_General_CP1_CI_AS NULL,
	UpdatedAt datetime2 NULL,
	IsDeleted bit NULL,
	CONSTRAINT PK_TypeOfLeaveEmployees PRIMARY KEY (Id),
	CONSTRAINT FK_TypeOfLeaveEmployees_Employees_EmployeeId FOREIGN KEY (EmployeeId) REFERENCES [HRM-Demo-1].dbo.Employees(Id),
	CONSTRAINT FK_TypeOfLeaveEmployees_TypeOfLeaves_TypeOfLeaveId FOREIGN KEY (TypeOfLeaveId) REFERENCES [HRM-Demo-1].dbo.TypeOfLeaves(Id)
);
 CREATE NONCLUSTERED INDEX IX_TypeOfLeaveEmployees_EmployeeId ON HRM-Demo-1.dbo.TypeOfLeaveEmployees (  EmployeeId ASC  )  
	 WITH (  PAD_INDEX = OFF ,FILLFACTOR = 100  ,SORT_IN_TEMPDB = OFF , IGNORE_DUP_KEY = OFF , STATISTICS_NORECOMPUTE = OFF , ONLINE = OFF , ALLOW_ROW_LOCKS = ON , ALLOW_PAGE_LOCKS = ON  )
	 ON [PRIMARY ] ;
 CREATE NONCLUSTERED INDEX IX_TypeOfLeaveEmployees_TypeOfLeaveId ON HRM-Demo-1.dbo.TypeOfLeaveEmployees (  TypeOfLeaveId ASC  )  
	 WITH (  PAD_INDEX = OFF ,FILLFACTOR = 100  ,SORT_IN_TEMPDB = OFF , IGNORE_DUP_KEY = OFF , STATISTICS_NORECOMPUTE = OFF , ONLINE = OFF , ALLOW_ROW_LOCKS = ON , ALLOW_PAGE_LOCKS = ON  )
	 ON [PRIMARY ] ;


-- [HRM-Demo-1].dbo.UserConnections definition

-- Drop table

-- DROP TABLE [HRM-Demo-1].dbo.UserConnections;

CREATE TABLE [HRM-Demo-1].dbo.UserConnections (
	Id int IDENTITY(1,1) NOT NULL,
	EmployeeId int NOT NULL,
	ConnectionId nvarchar(MAX) COLLATE SQL_Latin1_General_CP1_CI_AS NOT NULL,
	CreatedBy int NULL,
	CreatedName nvarchar(MAX) COLLATE SQL_Latin1_General_CP1_CI_AS NULL,
	CreatedAt datetime2 NULL,
	UpdatedBy int NULL,
	UpdatedName nvarchar(MAX) COLLATE SQL_Latin1_General_CP1_CI_AS NULL,
	UpdatedAt datetime2 NULL,
	IsDeleted bit NULL,
	DeviceInfo nvarchar(MAX) COLLATE SQL_Latin1_General_CP1_CI_AS DEFAULT N'' NOT NULL,
	Host nvarchar(MAX) COLLATE SQL_Latin1_General_CP1_CI_AS DEFAULT N'' NOT NULL,
	IpAddress nvarchar(MAX) COLLATE SQL_Latin1_General_CP1_CI_AS DEFAULT N'' NOT NULL,
	UserAgent nvarchar(MAX) COLLATE SQL_Latin1_General_CP1_CI_AS DEFAULT N'' NOT NULL,
	UserName nvarchar(MAX) COLLATE SQL_Latin1_General_CP1_CI_AS DEFAULT N'' NOT NULL,
	CONSTRAINT PK_UserConnections PRIMARY KEY (Id),
	CONSTRAINT FK_UserConnections_Employees_EmployeeId FOREIGN KEY (EmployeeId) REFERENCES [HRM-Demo-1].dbo.Employees(Id) ON DELETE CASCADE
);
 CREATE NONCLUSTERED INDEX IX_UserConnections_EmployeeId ON HRM-Demo-1.dbo.UserConnections (  EmployeeId ASC  )  
	 WITH (  PAD_INDEX = OFF ,FILLFACTOR = 100  ,SORT_IN_TEMPDB = OFF , IGNORE_DUP_KEY = OFF , STATISTICS_NORECOMPUTE = OFF , ONLINE = OFF , ALLOW_ROW_LOCKS = ON , ALLOW_PAGE_LOCKS = ON  )
	 ON [PRIMARY ] ;


-- [HRM-Demo-1].dbo.Users definition

-- Drop table

-- DROP TABLE [HRM-Demo-1].dbo.Users;

CREATE TABLE [HRM-Demo-1].dbo.Users (
	Id int IDENTITY(1,1) NOT NULL,
	Name nvarchar(MAX) COLLATE SQL_Latin1_General_CP1_CI_AS NULL,
	AvatarUrl nvarchar(MAX) COLLATE SQL_Latin1_General_CP1_CI_AS NULL,
	Sex int NULL,
	Address nvarchar(MAX) COLLATE SQL_Latin1_General_CP1_CI_AS NULL,
	BirthDay datetime2 NULL,
	Status bit NULL,
	IsDeleted bit NULL,
	RefreshToken nvarchar(MAX) COLLATE SQL_Latin1_General_CP1_CI_AS NULL,
	IsRefreshToken bit NULL,
	IsActivated bit NOT NULL,
	ActivationCode nvarchar(MAX) COLLATE SQL_Latin1_General_CP1_CI_AS NULL,
	ActivationExpiry datetime2 NULL,
	IsLockAccount bit NULL,
	EmployeeId int NULL,
	RefreshTokenExpiryTime datetime2 NULL,
	UserName nvarchar(256) COLLATE SQL_Latin1_General_CP1_CI_AS NULL,
	NormalizedUserName nvarchar(256) COLLATE SQL_Latin1_General_CP1_CI_AS NULL,
	Email nvarchar(256) COLLATE SQL_Latin1_General_CP1_CI_AS NULL,
	NormalizedEmail nvarchar(256) COLLATE SQL_Latin1_General_CP1_CI_AS NULL,
	EmailConfirmed bit NOT NULL,
	PasswordHash nvarchar(MAX) COLLATE SQL_Latin1_General_CP1_CI_AS NULL,
	SecurityStamp nvarchar(MAX) COLLATE SQL_Latin1_General_CP1_CI_AS NULL,
	ConcurrencyStamp nvarchar(MAX) COLLATE SQL_Latin1_General_CP1_CI_AS NULL,
	PhoneNumber nvarchar(MAX) COLLATE SQL_Latin1_General_CP1_CI_AS NULL,
	PhoneNumberConfirmed bit NOT NULL,
	TwoFactorEnabled bit NOT NULL,
	LockoutEnd datetimeoffset NULL,
	LockoutEnabled bit NOT NULL,
	AccessFailedCount int NOT NULL,
	CreatedAt datetime2 NULL,
	UpdatedAt datetime2 NULL,
	CONSTRAINT PK_Users PRIMARY KEY (Id),
	CONSTRAINT FK_Users_Employees_EmployeeId FOREIGN KEY (EmployeeId) REFERENCES [HRM-Demo-1].dbo.Employees(Id)
);
 CREATE NONCLUSTERED INDEX EmailIndex ON HRM-Demo-1.dbo.Users (  NormalizedEmail ASC  )  
	 WITH (  PAD_INDEX = OFF ,FILLFACTOR = 100  ,SORT_IN_TEMPDB = OFF , IGNORE_DUP_KEY = OFF , STATISTICS_NORECOMPUTE = OFF , ONLINE = OFF , ALLOW_ROW_LOCKS = ON , ALLOW_PAGE_LOCKS = ON  )
	 ON [PRIMARY ] ;
 CREATE NONCLUSTERED INDEX IX_Users_EmployeeId ON HRM-Demo-1.dbo.Users (  EmployeeId ASC  )  
	 WITH (  PAD_INDEX = OFF ,FILLFACTOR = 100  ,SORT_IN_TEMPDB = OFF , IGNORE_DUP_KEY = OFF , STATISTICS_NORECOMPUTE = OFF , ONLINE = OFF , ALLOW_ROW_LOCKS = ON , ALLOW_PAGE_LOCKS = ON  )
	 ON [PRIMARY ] ;
 CREATE UNIQUE NONCLUSTERED INDEX UserNameIndex ON HRM-Demo-1.dbo.Users (  NormalizedUserName ASC  )  
	 WHERE  ([NormalizedUserName] IS NOT NULL)
	 WITH (  PAD_INDEX = OFF ,FILLFACTOR = 100  ,SORT_IN_TEMPDB = OFF , IGNORE_DUP_KEY = OFF , STATISTICS_NORECOMPUTE = OFF , ONLINE = OFF , ALLOW_ROW_LOCKS = ON , ALLOW_PAGE_LOCKS = ON  )
	 ON [PRIMARY ] ;


-- [HRM-Demo-1].dbo.WorkFactors definition

-- Drop table

-- DROP TABLE [HRM-Demo-1].dbo.WorkFactors;

CREATE TABLE [HRM-Demo-1].dbo.WorkFactors (
	Id int IDENTITY(1,1) NOT NULL,
	HolidayId int NULL,
	[Year] int NULL,
	Factor decimal(18,2) NULL,
	IsFixed bit NOT NULL,
	CreatedBy int NULL,
	CreatedName nvarchar(MAX) COLLATE SQL_Latin1_General_CP1_CI_AS NULL,
	CreatedAt datetime2 NULL,
	UpdatedBy int NULL,
	UpdatedName nvarchar(MAX) COLLATE SQL_Latin1_General_CP1_CI_AS NULL,
	UpdatedAt datetime2 NULL,
	IsDeleted bit NULL,
	CONSTRAINT PK_WorkFactors PRIMARY KEY (Id),
	CONSTRAINT FK_WorkFactors_Holidays_HolidayId FOREIGN KEY (HolidayId) REFERENCES [HRM-Demo-1].dbo.Holidays(Id)
);
 CREATE NONCLUSTERED INDEX IX_WorkFactors_HolidayId ON HRM-Demo-1.dbo.WorkFactors (  HolidayId ASC  )  
	 WITH (  PAD_INDEX = OFF ,FILLFACTOR = 100  ,SORT_IN_TEMPDB = OFF , IGNORE_DUP_KEY = OFF , STATISTICS_NORECOMPUTE = OFF , ONLINE = OFF , ALLOW_ROW_LOCKS = ON , ALLOW_PAGE_LOCKS = ON  )
	 ON [PRIMARY ] ;


-- [HRM-Demo-1].dbo.ApplyEmployeeTimekeepingSettings definition

-- Drop table

-- DROP TABLE [HRM-Demo-1].dbo.ApplyEmployeeTimekeepingSettings;

CREATE TABLE [HRM-Demo-1].dbo.ApplyEmployeeTimekeepingSettings (
	Id int IDENTITY(1,1) NOT NULL,
	ApplyOrganizationId int NOT NULL,
	EmployeeId int NOT NULL,
	CreatedBy int NULL,
	CreatedName nvarchar(MAX) COLLATE SQL_Latin1_General_CP1_CI_AS NULL,
	CreatedAt datetime2 NULL,
	UpdatedBy int NULL,
	UpdatedName nvarchar(MAX) COLLATE SQL_Latin1_General_CP1_CI_AS NULL,
	UpdatedAt datetime2 NULL,
	IsDeleted bit NULL,
	CONSTRAINT PK_ApplyEmployeeTimekeepingSettings PRIMARY KEY (Id),
	CONSTRAINT FK_ApplyEmployeeTimekeepingSettings_ApplyOrganizations_ApplyOrganizationId FOREIGN KEY (ApplyOrganizationId) REFERENCES [HRM-Demo-1].dbo.ApplyOrganizations(Id) ON DELETE CASCADE,
	CONSTRAINT FK_ApplyEmployeeTimekeepingSettings_Employees_EmployeeId FOREIGN KEY (EmployeeId) REFERENCES [HRM-Demo-1].dbo.Employees(Id) ON DELETE CASCADE
);
 CREATE NONCLUSTERED INDEX IX_ApplyEmployeeTimekeepingSettings_ApplyOrganizationId ON HRM-Demo-1.dbo.ApplyEmployeeTimekeepingSettings (  ApplyOrganizationId ASC  )  
	 WITH (  PAD_INDEX = OFF ,FILLFACTOR = 100  ,SORT_IN_TEMPDB = OFF , IGNORE_DUP_KEY = OFF , STATISTICS_NORECOMPUTE = OFF , ONLINE = OFF , ALLOW_ROW_LOCKS = ON , ALLOW_PAGE_LOCKS = ON  )
	 ON [PRIMARY ] ;
 CREATE NONCLUSTERED INDEX IX_ApplyEmployeeTimekeepingSettings_EmployeeId ON HRM-Demo-1.dbo.ApplyEmployeeTimekeepingSettings (  EmployeeId ASC  )  
	 WITH (  PAD_INDEX = OFF ,FILLFACTOR = 100  ,SORT_IN_TEMPDB = OFF , IGNORE_DUP_KEY = OFF , STATISTICS_NORECOMPUTE = OFF , ONLINE = OFF , ALLOW_ROW_LOCKS = ON , ALLOW_PAGE_LOCKS = ON  )
	 ON [PRIMARY ] ;


-- [HRM-Demo-1].dbo.CheckInCheckOutApplications definition

-- Drop table

-- DROP TABLE [HRM-Demo-1].dbo.CheckInCheckOutApplications;

CREATE TABLE [HRM-Demo-1].dbo.CheckInCheckOutApplications (
	Id int IDENTITY(1,1) NOT NULL,
	EmployeeId int NOT NULL,
	ApproverId int NOT NULL,
	[Date] datetime2 NOT NULL,
	CheckType int NOT NULL,
	CheckInCheckOutStatus int NOT NULL,
	ShiftCatalogId int NOT NULL,
	Reason nvarchar(MAX) COLLATE SQL_Latin1_General_CP1_CI_AS NOT NULL,
	Description nvarchar(MAX) COLLATE SQL_Latin1_General_CP1_CI_AS NULL,
	CreatedBy int NULL,
	CreatedName nvarchar(MAX) COLLATE SQL_Latin1_General_CP1_CI_AS NULL,
	CreatedAt datetime2 NULL,
	UpdatedBy int NULL,
	UpdatedName nvarchar(MAX) COLLATE SQL_Latin1_General_CP1_CI_AS NULL,
	UpdatedAt datetime2 NULL,
	IsDeleted bit NULL,
	TimeCheckIn time NULL,
	TimeCheckOut time NULL,
	CONSTRAINT PK_CheckInCheckOutApplications PRIMARY KEY (Id),
	CONSTRAINT FK_CheckInCheckOutApplications_Employees_ApproverId FOREIGN KEY (ApproverId) REFERENCES [HRM-Demo-1].dbo.Employees(Id),
	CONSTRAINT FK_CheckInCheckOutApplications_Employees_EmployeeId FOREIGN KEY (EmployeeId) REFERENCES [HRM-Demo-1].dbo.Employees(Id),
	CONSTRAINT FK_CheckInCheckOutApplications_ShiftCatalogs_ShiftCatalogId FOREIGN KEY (ShiftCatalogId) REFERENCES [HRM-Demo-1].dbo.ShiftCatalogs(Id) ON DELETE CASCADE
);
 CREATE NONCLUSTERED INDEX IX_CheckInCheckOutApplications_ApproverId ON HRM-Demo-1.dbo.CheckInCheckOutApplications (  ApproverId ASC  )  
	 WITH (  PAD_INDEX = OFF ,FILLFACTOR = 100  ,SORT_IN_TEMPDB = OFF , IGNORE_DUP_KEY = OFF , STATISTICS_NORECOMPUTE = OFF , ONLINE = OFF , ALLOW_ROW_LOCKS = ON , ALLOW_PAGE_LOCKS = ON  )
	 ON [PRIMARY ] ;
 CREATE NONCLUSTERED INDEX IX_CheckInCheckOutApplications_EmployeeId ON HRM-Demo-1.dbo.CheckInCheckOutApplications (  EmployeeId ASC  )  
	 WITH (  PAD_INDEX = OFF ,FILLFACTOR = 100  ,SORT_IN_TEMPDB = OFF , IGNORE_DUP_KEY = OFF , STATISTICS_NORECOMPUTE = OFF , ONLINE = OFF , ALLOW_ROW_LOCKS = ON , ALLOW_PAGE_LOCKS = ON  )
	 ON [PRIMARY ] ;
 CREATE NONCLUSTERED INDEX IX_CheckInCheckOutApplications_ShiftCatalogId ON HRM-Demo-1.dbo.CheckInCheckOutApplications (  ShiftCatalogId ASC  )  
	 WITH (  PAD_INDEX = OFF ,FILLFACTOR = 100  ,SORT_IN_TEMPDB = OFF , IGNORE_DUP_KEY = OFF , STATISTICS_NORECOMPUTE = OFF , ONLINE = OFF , ALLOW_ROW_LOCKS = ON , ALLOW_PAGE_LOCKS = ON  )
	 ON [PRIMARY ] ;


-- [HRM-Demo-1].dbo.Comments definition

-- Drop table

-- DROP TABLE [HRM-Demo-1].dbo.Comments;

CREATE TABLE [HRM-Demo-1].dbo.Comments (
	Id int IDENTITY(1,1) NOT NULL,
	WorkId int NOT NULL,
	Content nvarchar(MAX) COLLATE SQL_Latin1_General_CP1_CI_AS NOT NULL,
	Attachment nvarchar(MAX) COLLATE SQL_Latin1_General_CP1_CI_AS NULL,
	CommentsCount int NULL,
	EmployeeId int NULL,
	EmployeeName nvarchar(MAX) COLLATE SQL_Latin1_General_CP1_CI_AS NULL,
	ParentCommentId int NULL,
	CreatedBy int NULL,
	CreatedName nvarchar(MAX) COLLATE SQL_Latin1_General_CP1_CI_AS NULL,
	CreatedAt datetime2 NULL,
	UpdatedBy int NULL,
	UpdatedName nvarchar(MAX) COLLATE SQL_Latin1_General_CP1_CI_AS NULL,
	UpdatedAt datetime2 NULL,
	IsDeleted bit NULL,
	CONSTRAINT PK_Comments PRIMARY KEY (Id),
	CONSTRAINT FK_Comments_Employees_EmployeeId FOREIGN KEY (EmployeeId) REFERENCES [HRM-Demo-1].dbo.Employees(Id)
);
 CREATE NONCLUSTERED INDEX IX_Comments_EmployeeId ON HRM-Demo-1.dbo.Comments (  EmployeeId ASC  )  
	 WITH (  PAD_INDEX = OFF ,FILLFACTOR = 100  ,SORT_IN_TEMPDB = OFF , IGNORE_DUP_KEY = OFF , STATISTICS_NORECOMPUTE = OFF , ONLINE = OFF , ALLOW_ROW_LOCKS = ON , ALLOW_PAGE_LOCKS = ON  )
	 ON [PRIMARY ] ;


-- [HRM-Demo-1].dbo.ContactInfos definition

-- Drop table

-- DROP TABLE [HRM-Demo-1].dbo.ContactInfos;

CREATE TABLE [HRM-Demo-1].dbo.ContactInfos (
	Id int IDENTITY(1,1) NOT NULL,
	EmployeeId int NULL,
	AnotherPhoneNumber nvarchar(MAX) COLLATE SQL_Latin1_General_CP1_CI_AS NULL,
	OrganizationPhone nvarchar(MAX) COLLATE SQL_Latin1_General_CP1_CI_AS NULL,
	PhonePersonHome nvarchar(MAX) COLLATE SQL_Latin1_General_CP1_CI_AS NULL,
	OrganizationEmail nvarchar(MAX) COLLATE SQL_Latin1_General_CP1_CI_AS NULL,
	AnotherEmail nvarchar(MAX) COLLATE SQL_Latin1_General_CP1_CI_AS NULL,
	Skype nvarchar(MAX) COLLATE SQL_Latin1_General_CP1_CI_AS NULL,
	Facebook nvarchar(MAX) COLLATE SQL_Latin1_General_CP1_CI_AS NULL,
	PersonEmail nvarchar(MAX) COLLATE SQL_Latin1_General_CP1_CI_AS NULL,
	NationId int NULL,
	Nation nvarchar(MAX) COLLATE SQL_Latin1_General_CP1_CI_AS NULL,
	CityId int NULL,
	City nvarchar(MAX) COLLATE SQL_Latin1_General_CP1_CI_AS NULL,
	DistrictId int NULL,
	District nvarchar(MAX) COLLATE SQL_Latin1_General_CP1_CI_AS NULL,
	WardId int NULL,
	Ward nvarchar(MAX) COLLATE SQL_Latin1_General_CP1_CI_AS NULL,
	StreetId int NULL,
	Street nvarchar(MAX) COLLATE SQL_Latin1_General_CP1_CI_AS NULL,
	Address nvarchar(MAX) COLLATE SQL_Latin1_General_CP1_CI_AS NULL,
	HomeNumber nvarchar(MAX) COLLATE SQL_Latin1_General_CP1_CI_AS NULL,
	FamilyNumber nvarchar(MAX) COLLATE SQL_Latin1_General_CP1_CI_AS NULL,
	HomeRegistrationNumber nvarchar(MAX) COLLATE SQL_Latin1_General_CP1_CI_AS NULL,
	IsMaster bit NULL,
	ResidenceAddress nvarchar(MAX) COLLATE SQL_Latin1_General_CP1_CI_AS NULL,
	IsAtResidenceAddress bit NULL,
	CurrentNationId int NULL,
	CurrentNation nvarchar(MAX) COLLATE SQL_Latin1_General_CP1_CI_AS NULL,
	CurrentCityId int NULL,
	CurrentCity nvarchar(MAX) COLLATE SQL_Latin1_General_CP1_CI_AS NULL,
	CurrentDistrictId int NULL,
	CurrentDistrict nvarchar(MAX) COLLATE SQL_Latin1_General_CP1_CI_AS NULL,
	CurrentWardId int NULL,
	CurrentWard nvarchar(MAX) COLLATE SQL_Latin1_General_CP1_CI_AS NULL,
	CurrentStreetId int NULL,
	CurrentStreet nvarchar(MAX) COLLATE SQL_Latin1_General_CP1_CI_AS NULL,
	CurrentHomeNumberId int NULL,
	CurrentHomeNumber nvarchar(MAX) COLLATE SQL_Latin1_General_CP1_CI_AS NULL,
	CurrentAddresss nvarchar(MAX) COLLATE SQL_Latin1_General_CP1_CI_AS NULL,
	FullName nvarchar(MAX) COLLATE SQL_Latin1_General_CP1_CI_AS NULL,
	RelationshipId int NULL,
	RelationshipName nvarchar(MAX) COLLATE SQL_Latin1_General_CP1_CI_AS NULL,
	PhoneNumberEmergency nvarchar(MAX) COLLATE SQL_Latin1_General_CP1_CI_AS NULL,
	AddressEmergency nvarchar(MAX) COLLATE SQL_Latin1_General_CP1_CI_AS NULL,
	HomePhoneEmergency nvarchar(MAX) COLLATE SQL_Latin1_General_CP1_CI_AS NULL,
	EmailEmergency nvarchar(MAX) COLLATE SQL_Latin1_General_CP1_CI_AS NULL,
	InsuranceDate datetime2 NULL,
	InsuranceContributionRate decimal(18,2) NULL,
	HealthInsuranceNumber nvarchar(MAX) COLLATE SQL_Latin1_General_CP1_CI_AS NULL,
	SocialInsuranceNumber nvarchar(MAX) COLLATE SQL_Latin1_General_CP1_CI_AS NULL,
	HealthInsuranceCardNumber nvarchar(MAX) COLLATE SQL_Latin1_General_CP1_CI_AS NULL,
	SocialSecurityNumber nvarchar(MAX) COLLATE SQL_Latin1_General_CP1_CI_AS NULL,
	IsSyndicate bit NULL,
	IssuranceStatus int NOT NULL,
	CityProvideId int NULL,
	CityProvide nvarchar(MAX) COLLATE SQL_Latin1_General_CP1_CI_AS NULL,
	CityProvideCode nvarchar(MAX) COLLATE SQL_Latin1_General_CP1_CI_AS NULL,
	HeathcareRegistractionCode nvarchar(MAX) COLLATE SQL_Latin1_General_CP1_CI_AS NULL,
	HeathcareRegistractionLocation nvarchar(MAX) COLLATE SQL_Latin1_General_CP1_CI_AS NULL,
	HeathcareRegistracionNumber nvarchar(MAX) COLLATE SQL_Latin1_General_CP1_CI_AS NULL,
	CreatedBy int NULL,
	CreatedName nvarchar(MAX) COLLATE SQL_Latin1_General_CP1_CI_AS NULL,
	CreatedAt datetime2 NULL,
	UpdatedBy int NULL,
	UpdatedName nvarchar(MAX) COLLATE SQL_Latin1_General_CP1_CI_AS NULL,
	UpdatedAt datetime2 NULL,
	IsDeleted bit NULL,
	CONSTRAINT PK_ContactInfos PRIMARY KEY (Id),
	CONSTRAINT FK_ContactInfos_Employees_EmployeeId FOREIGN KEY (EmployeeId) REFERENCES [HRM-Demo-1].dbo.Employees(Id)
);
 CREATE UNIQUE NONCLUSTERED INDEX IX_ContactInfos_EmployeeId ON HRM-Demo-1.dbo.ContactInfos (  EmployeeId ASC  )  
	 WHERE  ([EmployeeId] IS NOT NULL)
	 WITH (  PAD_INDEX = OFF ,FILLFACTOR = 100  ,SORT_IN_TEMPDB = OFF , IGNORE_DUP_KEY = OFF , STATISTICS_NORECOMPUTE = OFF , ONLINE = OFF , ALLOW_ROW_LOCKS = ON , ALLOW_PAGE_LOCKS = ON  )
	 ON [PRIMARY ] ;


-- [HRM-Demo-1].dbo.Contracts definition

-- Drop table

-- DROP TABLE [HRM-Demo-1].dbo.Contracts;

CREATE TABLE [HRM-Demo-1].dbo.Contracts (
	Id int IDENTITY(1,1) NOT NULL,
	EmployeeId int NULL,
	NameEmployee nvarchar(MAX) COLLATE SQL_Latin1_General_CP1_CI_AS NULL,
	CodeEmployee nvarchar(MAX) COLLATE SQL_Latin1_General_CP1_CI_AS NULL,
	Code nvarchar(MAX) COLLATE SQL_Latin1_General_CP1_CI_AS NULL,
	UnitId int NULL,
	[Position] nvarchar(MAX) COLLATE SQL_Latin1_General_CP1_CI_AS NULL,
	SigningDate datetime2 NULL,
	ContractName nvarchar(MAX) COLLATE SQL_Latin1_General_CP1_CI_AS NULL,
	ContractTypeId int NULL,
	ContractDurationId int NULL,
	WorkingFormId int NULL,
	EffectiveDate datetime2 NULL,
	ExpiryDate datetime2 NULL,
	SalaryAmount decimal(18,2) NULL,
	SalaryInsurance decimal(18,2) NULL,
	SalaryRate int NULL,
	CompanyRepresentativeSigningId int NULL,
	CompanyRepresentativeSigning nvarchar(MAX) COLLATE SQL_Latin1_General_CP1_CI_AS NULL,
	Representative nvarchar(MAX) COLLATE SQL_Latin1_General_CP1_CI_AS NULL,
	Attachment nvarchar(MAX) COLLATE SQL_Latin1_General_CP1_CI_AS NULL,
	Note nvarchar(MAX) COLLATE SQL_Latin1_General_CP1_CI_AS NULL,
	SignStatus int NOT NULL,
	ExpiredStatus bit NULL,
	NatureOfLaborId int NULL,
	CreatedBy int NULL,
	CreatedName nvarchar(MAX) COLLATE SQL_Latin1_General_CP1_CI_AS NULL,
	CreatedAt datetime2 NULL,
	UpdatedBy int NULL,
	UpdatedName nvarchar(MAX) COLLATE SQL_Latin1_General_CP1_CI_AS NULL,
	UpdatedAt datetime2 NULL,
	IsDeleted bit NULL,
	ContractTypeStatus int DEFAULT 0 NOT NULL,
	KpiSalary float NULL,
	RecurringAddNumberLeaveJobId nvarchar(MAX) COLLATE SQL_Latin1_General_CP1_CI_AS NULL,
	ScheduleBeforeStartHandleLeaveDayRecurringJobId nvarchar(MAX) COLLATE SQL_Latin1_General_CP1_CI_AS NULL,
	ScheduleUpdateExpireStatusJobId nvarchar(MAX) COLLATE SQL_Latin1_General_CP1_CI_AS NULL,
	UnionFee float NULL,
	CONSTRAINT PK_Contracts PRIMARY KEY (Id),
	CONSTRAINT FK_Contracts_ContractDurations_ContractDurationId FOREIGN KEY (ContractDurationId) REFERENCES [HRM-Demo-1].dbo.ContractDurations(Id),
	CONSTRAINT FK_Contracts_ContractTypes_ContractTypeId FOREIGN KEY (ContractTypeId) REFERENCES [HRM-Demo-1].dbo.ContractTypes(Id),
	CONSTRAINT FK_Contracts_Employees_EmployeeId FOREIGN KEY (EmployeeId) REFERENCES [HRM-Demo-1].dbo.Employees(Id),
	CONSTRAINT FK_Contracts_NatureOfLabor_NatureOfLaborId FOREIGN KEY (NatureOfLaborId) REFERENCES [HRM-Demo-1].dbo.NatureOfLabor(Id),
	CONSTRAINT FK_Contracts_Organizations_UnitId FOREIGN KEY (UnitId) REFERENCES [HRM-Demo-1].dbo.Organizations(Id),
	CONSTRAINT FK_Contracts_WorkingForms_WorkingFormId FOREIGN KEY (WorkingFormId) REFERENCES [HRM-Demo-1].dbo.WorkingForms(Id)
);
 CREATE NONCLUSTERED INDEX IX_Contracts_ContractDurationId ON HRM-Demo-1.dbo.Contracts (  ContractDurationId ASC  )  
	 WITH (  PAD_INDEX = OFF ,FILLFACTOR = 100  ,SORT_IN_TEMPDB = OFF , IGNORE_DUP_KEY = OFF , STATISTICS_NORECOMPUTE = OFF , ONLINE = OFF , ALLOW_ROW_LOCKS = ON , ALLOW_PAGE_LOCKS = ON  )
	 ON [PRIMARY ] ;
 CREATE NONCLUSTERED INDEX IX_Contracts_ContractTypeId ON HRM-Demo-1.dbo.Contracts (  ContractTypeId ASC  )  
	 WITH (  PAD_INDEX = OFF ,FILLFACTOR = 100  ,SORT_IN_TEMPDB = OFF , IGNORE_DUP_KEY = OFF , STATISTICS_NORECOMPUTE = OFF , ONLINE = OFF , ALLOW_ROW_LOCKS = ON , ALLOW_PAGE_LOCKS = ON  )
	 ON [PRIMARY ] ;
 CREATE NONCLUSTERED INDEX IX_Contracts_EmployeeId ON HRM-Demo-1.dbo.Contracts (  EmployeeId ASC  )  
	 WITH (  PAD_INDEX = OFF ,FILLFACTOR = 100  ,SORT_IN_TEMPDB = OFF , IGNORE_DUP_KEY = OFF , STATISTICS_NORECOMPUTE = OFF , ONLINE = OFF , ALLOW_ROW_LOCKS = ON , ALLOW_PAGE_LOCKS = ON  )
	 ON [PRIMARY ] ;
 CREATE NONCLUSTERED INDEX IX_Contracts_NatureOfLaborId ON HRM-Demo-1.dbo.Contracts (  NatureOfLaborId ASC  )  
	 WITH (  PAD_INDEX = OFF ,FILLFACTOR = 100  ,SORT_IN_TEMPDB = OFF , IGNORE_DUP_KEY = OFF , STATISTICS_NORECOMPUTE = OFF , ONLINE = OFF , ALLOW_ROW_LOCKS = ON , ALLOW_PAGE_LOCKS = ON  )
	 ON [PRIMARY ] ;
 CREATE NONCLUSTERED INDEX IX_Contracts_UnitId ON HRM-Demo-1.dbo.Contracts (  UnitId ASC  )  
	 WITH (  PAD_INDEX = OFF ,FILLFACTOR = 100  ,SORT_IN_TEMPDB = OFF , IGNORE_DUP_KEY = OFF , STATISTICS_NORECOMPUTE = OFF , ONLINE = OFF , ALLOW_ROW_LOCKS = ON , ALLOW_PAGE_LOCKS = ON  )
	 ON [PRIMARY ] ;
 CREATE NONCLUSTERED INDEX IX_Contracts_WorkingFormId ON HRM-Demo-1.dbo.Contracts (  WorkingFormId ASC  )  
	 WITH (  PAD_INDEX = OFF ,FILLFACTOR = 100  ,SORT_IN_TEMPDB = OFF , IGNORE_DUP_KEY = OFF , STATISTICS_NORECOMPUTE = OFF , ONLINE = OFF , ALLOW_ROW_LOCKS = ON , ALLOW_PAGE_LOCKS = ON  )
	 ON [PRIMARY ] ;


-- [HRM-Demo-1].dbo.Deductions definition

-- Drop table

-- DROP TABLE [HRM-Demo-1].dbo.Deductions;

CREATE TABLE [HRM-Demo-1].dbo.Deductions (
	Id int IDENTITY(1,1) NOT NULL,
	DeducationName nvarchar(MAX) COLLATE SQL_Latin1_General_CP1_CI_AS NULL,
	StandardType nvarchar(MAX) COLLATE SQL_Latin1_General_CP1_CI_AS NULL,
	StandardValue decimal(18,2) NULL,
	TypeValue nvarchar(MAX) COLLATE SQL_Latin1_General_CP1_CI_AS NULL,
	Value decimal(18,2) NULL,
	Note nvarchar(MAX) COLLATE SQL_Latin1_General_CP1_CI_AS NULL,
	EmployeeId int NULL,
	CreatedBy int NULL,
	CreatedName nvarchar(MAX) COLLATE SQL_Latin1_General_CP1_CI_AS NULL,
	CreatedAt datetime2 NULL,
	UpdatedBy int NULL,
	UpdatedName nvarchar(MAX) COLLATE SQL_Latin1_General_CP1_CI_AS NULL,
	UpdatedAt datetime2 NULL,
	IsDeleted bit NULL,
	CONSTRAINT PK_Deductions PRIMARY KEY (Id),
	CONSTRAINT FK_Deductions_Employees_EmployeeId FOREIGN KEY (EmployeeId) REFERENCES [HRM-Demo-1].dbo.Employees(Id)
);
 CREATE NONCLUSTERED INDEX IX_Deductions_EmployeeId ON HRM-Demo-1.dbo.Deductions (  EmployeeId ASC  )  
	 WITH (  PAD_INDEX = OFF ,FILLFACTOR = 100  ,SORT_IN_TEMPDB = OFF , IGNORE_DUP_KEY = OFF , STATISTICS_NORECOMPUTE = OFF , ONLINE = OFF , ALLOW_ROW_LOCKS = ON , ALLOW_PAGE_LOCKS = ON  )
	 ON [PRIMARY ] ;


-- [HRM-Demo-1].dbo.Delegations definition

-- Drop table

-- DROP TABLE [HRM-Demo-1].dbo.Delegations;

CREATE TABLE [HRM-Demo-1].dbo.Delegations (
	Id int IDENTITY(1,1) NOT NULL,
	EmployeeDelegationId int NULL,
	StartDate datetime2 NULL,
	EndDate datetime2 NULL,
	CreatedBy int NULL,
	CreatedName nvarchar(MAX) COLLATE SQL_Latin1_General_CP1_CI_AS NULL,
	CreatedAt datetime2 NULL,
	UpdatedBy int NULL,
	UpdatedName nvarchar(MAX) COLLATE SQL_Latin1_General_CP1_CI_AS NULL,
	UpdatedAt datetime2 NULL,
	IsDeleted bit NULL,
	CONSTRAINT PK_Delegations PRIMARY KEY (Id),
	CONSTRAINT FK_Delegations_Employees_EmployeeDelegationId FOREIGN KEY (EmployeeDelegationId) REFERENCES [HRM-Demo-1].dbo.Employees(Id)
);
 CREATE NONCLUSTERED INDEX IX_Delegations_EmployeeDelegationId ON HRM-Demo-1.dbo.Delegations (  EmployeeDelegationId ASC  )  
	 WITH (  PAD_INDEX = OFF ,FILLFACTOR = 100  ,SORT_IN_TEMPDB = OFF , IGNORE_DUP_KEY = OFF , STATISTICS_NORECOMPUTE = OFF , ONLINE = OFF , ALLOW_ROW_LOCKS = ON , ALLOW_PAGE_LOCKS = ON  )
	 ON [PRIMARY ] ;


-- [HRM-Demo-1].dbo.DepartmentEmployees definition

-- Drop table

-- DROP TABLE [HRM-Demo-1].dbo.DepartmentEmployees;

CREATE TABLE [HRM-Demo-1].dbo.DepartmentEmployees (
	EmployeeId int NOT NULL,
	DepartmentId int NOT NULL,
	DepartmentRoleId int NOT NULL,
	CONSTRAINT PK_DepartmentEmployees PRIMARY KEY (DepartmentId,EmployeeId),
	CONSTRAINT FK_DepartmentEmployees_DepartmentRoles_DepartmentRoleId FOREIGN KEY (DepartmentRoleId) REFERENCES [HRM-Demo-1].dbo.DepartmentRoles(Id) ON DELETE CASCADE,
	CONSTRAINT FK_DepartmentEmployees_Departments_DepartmentId FOREIGN KEY (DepartmentId) REFERENCES [HRM-Demo-1].dbo.Departments(Id) ON DELETE CASCADE,
	CONSTRAINT FK_DepartmentEmployees_Employees_EmployeeId FOREIGN KEY (EmployeeId) REFERENCES [HRM-Demo-1].dbo.Employees(Id) ON DELETE CASCADE
);
 CREATE NONCLUSTERED INDEX IX_DepartmentEmployees_DepartmentRoleId ON HRM-Demo-1.dbo.DepartmentEmployees (  DepartmentRoleId ASC  )  
	 WITH (  PAD_INDEX = OFF ,FILLFACTOR = 100  ,SORT_IN_TEMPDB = OFF , IGNORE_DUP_KEY = OFF , STATISTICS_NORECOMPUTE = OFF , ONLINE = OFF , ALLOW_ROW_LOCKS = ON , ALLOW_PAGE_LOCKS = ON  )
	 ON [PRIMARY ] ;
 CREATE NONCLUSTERED INDEX IX_DepartmentEmployees_EmployeeId ON HRM-Demo-1.dbo.DepartmentEmployees (  EmployeeId ASC  )  
	 WITH (  PAD_INDEX = OFF ,FILLFACTOR = 100  ,SORT_IN_TEMPDB = OFF , IGNORE_DUP_KEY = OFF , STATISTICS_NORECOMPUTE = OFF , ONLINE = OFF , ALLOW_ROW_LOCKS = ON , ALLOW_PAGE_LOCKS = ON  )
	 ON [PRIMARY ] ;


-- [HRM-Demo-1].dbo.DetailTimesheetNameStaffPositions definition

-- Drop table

-- DROP TABLE [HRM-Demo-1].dbo.DetailTimesheetNameStaffPositions;

CREATE TABLE [HRM-Demo-1].dbo.DetailTimesheetNameStaffPositions (
	StaffPositionId int NOT NULL,
	DetailTimesheetNameId int NOT NULL,
	CONSTRAINT PK_DetailTimesheetNameStaffPositions PRIMARY KEY (StaffPositionId,DetailTimesheetNameId),
	CONSTRAINT FK_DetailTimesheetNameStaffPositions_DetailTimesheetNames_DetailTimesheetNameId FOREIGN KEY (DetailTimesheetNameId) REFERENCES [HRM-Demo-1].dbo.DetailTimesheetNames(Id) ON DELETE CASCADE,
	CONSTRAINT FK_DetailTimesheetNameStaffPositions_StaffPositions_StaffPositionId FOREIGN KEY (StaffPositionId) REFERENCES [HRM-Demo-1].dbo.StaffPositions(Id) ON DELETE CASCADE
);
 CREATE NONCLUSTERED INDEX IX_DetailTimesheetNameStaffPositions_DetailTimesheetNameId ON HRM-Demo-1.dbo.DetailTimesheetNameStaffPositions (  DetailTimesheetNameId ASC  )  
	 WITH (  PAD_INDEX = OFF ,FILLFACTOR = 100  ,SORT_IN_TEMPDB = OFF , IGNORE_DUP_KEY = OFF , STATISTICS_NORECOMPUTE = OFF , ONLINE = OFF , ALLOW_ROW_LOCKS = ON , ALLOW_PAGE_LOCKS = ON  )
	 ON [PRIMARY ] ;


-- [HRM-Demo-1].dbo.GroupWorks definition

-- Drop table

-- DROP TABLE [HRM-Demo-1].dbo.GroupWorks;

CREATE TABLE [HRM-Demo-1].dbo.GroupWorks (
	Id int IDENTITY(1,1) NOT NULL,
	Name nvarchar(MAX) COLLATE SQL_Latin1_General_CP1_CI_AS NULL,
	Color nvarchar(MAX) COLLATE SQL_Latin1_General_CP1_CI_AS NULL,
	CreatedBy int NULL,
	CreatedName nvarchar(MAX) COLLATE SQL_Latin1_General_CP1_CI_AS NULL,
	CreatedAt datetime2 NULL,
	UpdatedBy int NULL,
	UpdatedName nvarchar(MAX) COLLATE SQL_Latin1_General_CP1_CI_AS NULL,
	UpdatedAt datetime2 NULL,
	IsDeleted bit NULL,
	ProjectId int NULL,
	CONSTRAINT PK_GroupWorks PRIMARY KEY (Id),
	CONSTRAINT FK_GroupWorks_Projects_ProjectId FOREIGN KEY (ProjectId) REFERENCES [HRM-Demo-1].dbo.Projects(Id)
);
 CREATE NONCLUSTERED INDEX IX_GroupWorks_ProjectId ON HRM-Demo-1.dbo.GroupWorks (  ProjectId ASC  )  
	 WITH (  PAD_INDEX = OFF ,FILLFACTOR = 100  ,SORT_IN_TEMPDB = OFF , IGNORE_DUP_KEY = OFF , STATISTICS_NORECOMPUTE = OFF , ONLINE = OFF , ALLOW_ROW_LOCKS = ON , ALLOW_PAGE_LOCKS = ON  )
	 ON [PRIMARY ] ;


-- [HRM-Demo-1].dbo.KpiTableDetails definition

-- Drop table

-- DROP TABLE [HRM-Demo-1].dbo.KpiTableDetails;

CREATE TABLE [HRM-Demo-1].dbo.KpiTableDetails (
	Id int IDENTITY(1,1) NOT NULL,
	KpiTableId int NULL,
	EmployeeId int NULL,
	EmployeeCode nvarchar(MAX) COLLATE SQL_Latin1_General_CP1_CI_AS NULL,
	EmployeeName nvarchar(MAX) COLLATE SQL_Latin1_General_CP1_CI_AS NULL,
	CompletionRate float NULL,
	Bonus float NULL,
	Revenue decimal(18,2) NULL,
	IsCommissionManual bit NULL,
	CommissionManualAmount decimal(18,2) NULL,
	CreatedBy int NULL,
	CreatedName nvarchar(MAX) COLLATE SQL_Latin1_General_CP1_CI_AS NULL,
	CreatedAt datetime2 NULL,
	UpdatedBy int NULL,
	UpdatedName nvarchar(MAX) COLLATE SQL_Latin1_General_CP1_CI_AS NULL,
	UpdatedAt datetime2 NULL,
	IsDeleted bit NULL,
	CONSTRAINT PK_KpiTableDetails PRIMARY KEY (Id),
	CONSTRAINT FK_KpiTableDetails_Employees_EmployeeId FOREIGN KEY (EmployeeId) REFERENCES [HRM-Demo-1].dbo.Employees(Id),
	CONSTRAINT FK_KpiTableDetails_KpiTables_KpiTableId FOREIGN KEY (KpiTableId) REFERENCES [HRM-Demo-1].dbo.KpiTables(Id)
);
 CREATE NONCLUSTERED INDEX IX_KpiTableDetails_EmployeeId ON HRM-Demo-1.dbo.KpiTableDetails (  EmployeeId ASC  )  
	 WITH (  PAD_INDEX = OFF ,FILLFACTOR = 100  ,SORT_IN_TEMPDB = OFF , IGNORE_DUP_KEY = OFF , STATISTICS_NORECOMPUTE = OFF , ONLINE = OFF , ALLOW_ROW_LOCKS = ON , ALLOW_PAGE_LOCKS = ON  )
	 ON [PRIMARY ] ;
 CREATE NONCLUSTERED INDEX IX_KpiTableDetails_KpiTableId ON HRM-Demo-1.dbo.KpiTableDetails (  KpiTableId ASC  )  
	 WITH (  PAD_INDEX = OFF ,FILLFACTOR = 100  ,SORT_IN_TEMPDB = OFF , IGNORE_DUP_KEY = OFF , STATISTICS_NORECOMPUTE = OFF , ONLINE = OFF , ALLOW_ROW_LOCKS = ON , ALLOW_PAGE_LOCKS = ON  )
	 ON [PRIMARY ] ;


-- [HRM-Demo-1].dbo.KpiTablePositions definition

-- Drop table

-- DROP TABLE [HRM-Demo-1].dbo.KpiTablePositions;

CREATE TABLE [HRM-Demo-1].dbo.KpiTablePositions (
	StaffPositionId int NOT NULL,
	KpiTableId int NOT NULL,
	CONSTRAINT PK_KpiTablePositions PRIMARY KEY (StaffPositionId,KpiTableId),
	CONSTRAINT FK_KpiTablePositions_KpiTables_KpiTableId FOREIGN KEY (KpiTableId) REFERENCES [HRM-Demo-1].dbo.KpiTables(Id) ON DELETE CASCADE,
	CONSTRAINT FK_KpiTablePositions_StaffPositions_StaffPositionId FOREIGN KEY (StaffPositionId) REFERENCES [HRM-Demo-1].dbo.StaffPositions(Id) ON DELETE CASCADE
);
 CREATE NONCLUSTERED INDEX IX_KpiTablePositions_KpiTableId ON HRM-Demo-1.dbo.KpiTablePositions (  KpiTableId ASC  )  
	 WITH (  PAD_INDEX = OFF ,FILLFACTOR = 100  ,SORT_IN_TEMPDB = OFF , IGNORE_DUP_KEY = OFF , STATISTICS_NORECOMPUTE = OFF , ONLINE = OFF , ALLOW_ROW_LOCKS = ON , ALLOW_PAGE_LOCKS = ON  )
	 ON [PRIMARY ] ;


-- [HRM-Demo-1].dbo.LeaveApplicationApprovers definition

-- Drop table

-- DROP TABLE [HRM-Demo-1].dbo.LeaveApplicationApprovers;

CREATE TABLE [HRM-Demo-1].dbo.LeaveApplicationApprovers (
	LeaveApplicationId int NOT NULL,
	ApproverId int NOT NULL,
	CONSTRAINT PK_LeaveApplicationApprovers PRIMARY KEY (ApproverId,LeaveApplicationId),
	CONSTRAINT FK_LeaveApplicationApprovers_Employees_ApproverId FOREIGN KEY (ApproverId) REFERENCES [HRM-Demo-1].dbo.Employees(Id) ON DELETE CASCADE,
	CONSTRAINT FK_LeaveApplicationApprovers_LeaveApplications_LeaveApplicationId FOREIGN KEY (LeaveApplicationId) REFERENCES [HRM-Demo-1].dbo.LeaveApplications(Id) ON DELETE CASCADE
);
 CREATE NONCLUSTERED INDEX IX_LeaveApplicationApprovers_LeaveApplicationId ON HRM-Demo-1.dbo.LeaveApplicationApprovers (  LeaveApplicationId ASC  )  
	 WITH (  PAD_INDEX = OFF ,FILLFACTOR = 100  ,SORT_IN_TEMPDB = OFF , IGNORE_DUP_KEY = OFF , STATISTICS_NORECOMPUTE = OFF , ONLINE = OFF , ALLOW_ROW_LOCKS = ON , ALLOW_PAGE_LOCKS = ON  )
	 ON [PRIMARY ] ;


-- [HRM-Demo-1].dbo.LeaveApplicationRelatedPeople definition

-- Drop table

-- DROP TABLE [HRM-Demo-1].dbo.LeaveApplicationRelatedPeople;

CREATE TABLE [HRM-Demo-1].dbo.LeaveApplicationRelatedPeople (
	LeaveApplicationId int NOT NULL,
	RelatedPersonId int NOT NULL,
	CONSTRAINT PK_LeaveApplicationRelatedPeople PRIMARY KEY (RelatedPersonId,LeaveApplicationId),
	CONSTRAINT FK_LeaveApplicationRelatedPeople_Employees_RelatedPersonId FOREIGN KEY (RelatedPersonId) REFERENCES [HRM-Demo-1].dbo.Employees(Id) ON DELETE CASCADE,
	CONSTRAINT FK_LeaveApplicationRelatedPeople_LeaveApplications_LeaveApplicationId FOREIGN KEY (LeaveApplicationId) REFERENCES [HRM-Demo-1].dbo.LeaveApplications(Id) ON DELETE CASCADE
);
 CREATE NONCLUSTERED INDEX IX_LeaveApplicationRelatedPeople_LeaveApplicationId ON HRM-Demo-1.dbo.LeaveApplicationRelatedPeople (  LeaveApplicationId ASC  )  
	 WITH (  PAD_INDEX = OFF ,FILLFACTOR = 100  ,SORT_IN_TEMPDB = OFF , IGNORE_DUP_KEY = OFF , STATISTICS_NORECOMPUTE = OFF , ONLINE = OFF , ALLOW_ROW_LOCKS = ON , ALLOW_PAGE_LOCKS = ON  )
	 ON [PRIMARY ] ;


-- [HRM-Demo-1].dbo.LeaveApplicationReplacements definition

-- Drop table

-- DROP TABLE [HRM-Demo-1].dbo.LeaveApplicationReplacements;

CREATE TABLE [HRM-Demo-1].dbo.LeaveApplicationReplacements (
	LeaveApplicationId int NOT NULL,
	ReplacementId int NOT NULL,
	CONSTRAINT PK_LeaveApplicationReplacements PRIMARY KEY (ReplacementId,LeaveApplicationId),
	CONSTRAINT FK_LeaveApplicationReplacements_Employees_ReplacementId FOREIGN KEY (ReplacementId) REFERENCES [HRM-Demo-1].dbo.Employees(Id) ON DELETE CASCADE,
	CONSTRAINT FK_LeaveApplicationReplacements_LeaveApplications_LeaveApplicationId FOREIGN KEY (LeaveApplicationId) REFERENCES [HRM-Demo-1].dbo.LeaveApplications(Id) ON DELETE CASCADE
);
 CREATE NONCLUSTERED INDEX IX_LeaveApplicationReplacements_LeaveApplicationId ON HRM-Demo-1.dbo.LeaveApplicationReplacements (  LeaveApplicationId ASC  )  
	 WITH (  PAD_INDEX = OFF ,FILLFACTOR = 100  ,SORT_IN_TEMPDB = OFF , IGNORE_DUP_KEY = OFF , STATISTICS_NORECOMPUTE = OFF , ONLINE = OFF , ALLOW_ROW_LOCKS = ON , ALLOW_PAGE_LOCKS = ON  )
	 ON [PRIMARY ] ;


-- [HRM-Demo-1].dbo.PayrollDetails definition

-- Drop table

-- DROP TABLE [HRM-Demo-1].dbo.PayrollDetails;

CREATE TABLE [HRM-Demo-1].dbo.PayrollDetails (
	Id int IDENTITY(1,1) NOT NULL,
	PayrollId int NULL,
	EmployeeId int NULL,
	ContractId int NULL,
	BaseSalary decimal(18,2) NULL,
	StandardWorkDays int NULL,
	ActualWorkDays float NULL,
	ReceivedSalary decimal(18,2) NULL,
	KPI decimal(18,2) NULL,
	KpiPercentage decimal(18,2) NULL,
	KpiSalary decimal(18,2) NULL,
	Bonus decimal(18,2) NULL,
	SalaryRate decimal(18,2) NULL,
	AllowanceMealTravel decimal(18,2) NULL,
	ParkingAmount decimal(18,2) NULL,
	OvertimeAmount decimal(18,2) NULL,
	BhxhAmount decimal(18,2) NULL,
	Revenue decimal(18,2) NULL,
	CommissionRate decimal(9,6) NULL,
	CommissionAmount decimal(18,2) NULL,
	TotalSalary decimal(18,2) NULL,
	TotalReceivedSalary decimal(18,2) NULL,
	ConfirmationStatus int NOT NULL,
	CreatedBy int NULL,
	CreatedName nvarchar(MAX) COLLATE SQL_Latin1_General_CP1_CI_AS NULL,
	CreatedAt datetime2 NULL,
	UpdatedBy int NULL,
	UpdatedName nvarchar(MAX) COLLATE SQL_Latin1_General_CP1_CI_AS NULL,
	UpdatedAt datetime2 NULL,
	IsDeleted bit NULL,
	ContractTypeStatus int DEFAULT 0 NOT NULL,
	Department nvarchar(MAX) COLLATE SQL_Latin1_General_CP1_CI_AS NULL,
	EmployeeCode nvarchar(MAX) COLLATE SQL_Latin1_General_CP1_CI_AS NULL,
	FullName nvarchar(MAX) COLLATE SQL_Latin1_General_CP1_CI_AS NULL,
	OrganizationId int NULL,
	ResponseDeadline datetime2 NULL,
	ConfirmationDate datetime2 NULL,
	SocialInsurance float NULL,
	UnionFee float NULL,
	ContractTypeId int NULL,
	TotalAllowance decimal(18,2) NULL,
	TotalDeduction decimal(18,2) NULL,
	CONSTRAINT PK_PayrollDetails PRIMARY KEY (Id),
	CONSTRAINT FK_PayrollDetails_Contracts_ContractId FOREIGN KEY (ContractId) REFERENCES [HRM-Demo-1].dbo.Contracts(Id),
	CONSTRAINT FK_PayrollDetails_Employees_EmployeeId FOREIGN KEY (EmployeeId) REFERENCES [HRM-Demo-1].dbo.Employees(Id),
	CONSTRAINT FK_PayrollDetails_Organizations_OrganizationId FOREIGN KEY (OrganizationId) REFERENCES [HRM-Demo-1].dbo.Organizations(Id),
	CONSTRAINT FK_PayrollDetails_Payrolls_PayrollId FOREIGN KEY (PayrollId) REFERENCES [HRM-Demo-1].dbo.Payrolls(Id)
);
 CREATE NONCLUSTERED INDEX IX_PayrollDetails_ContractId ON HRM-Demo-1.dbo.PayrollDetails (  ContractId ASC  )  
	 WITH (  PAD_INDEX = OFF ,FILLFACTOR = 100  ,SORT_IN_TEMPDB = OFF , IGNORE_DUP_KEY = OFF , STATISTICS_NORECOMPUTE = OFF , ONLINE = OFF , ALLOW_ROW_LOCKS = ON , ALLOW_PAGE_LOCKS = ON  )
	 ON [PRIMARY ] ;
 CREATE NONCLUSTERED INDEX IX_PayrollDetails_EmployeeId ON HRM-Demo-1.dbo.PayrollDetails (  EmployeeId ASC  )  
	 WITH (  PAD_INDEX = OFF ,FILLFACTOR = 100  ,SORT_IN_TEMPDB = OFF , IGNORE_DUP_KEY = OFF , STATISTICS_NORECOMPUTE = OFF , ONLINE = OFF , ALLOW_ROW_LOCKS = ON , ALLOW_PAGE_LOCKS = ON  )
	 ON [PRIMARY ] ;
 CREATE NONCLUSTERED INDEX IX_PayrollDetails_OrganizationId ON HRM-Demo-1.dbo.PayrollDetails (  OrganizationId ASC  )  
	 WITH (  PAD_INDEX = OFF ,FILLFACTOR = 100  ,SORT_IN_TEMPDB = OFF , IGNORE_DUP_KEY = OFF , STATISTICS_NORECOMPUTE = OFF , ONLINE = OFF , ALLOW_ROW_LOCKS = ON , ALLOW_PAGE_LOCKS = ON  )
	 ON [PRIMARY ] ;
 CREATE NONCLUSTERED INDEX IX_PayrollDetails_PayrollId ON HRM-Demo-1.dbo.PayrollDetails (  PayrollId ASC  )  
	 WITH (  PAD_INDEX = OFF ,FILLFACTOR = 100  ,SORT_IN_TEMPDB = OFF , IGNORE_DUP_KEY = OFF , STATISTICS_NORECOMPUTE = OFF , ONLINE = OFF , ALLOW_ROW_LOCKS = ON , ALLOW_PAGE_LOCKS = ON  )
	 ON [PRIMARY ] ;


-- [HRM-Demo-1].dbo.PayrollInquiries definition

-- Drop table

-- DROP TABLE [HRM-Demo-1].dbo.PayrollInquiries;

CREATE TABLE [HRM-Demo-1].dbo.PayrollInquiries (
	Id int IDENTITY(1,1) NOT NULL,
	PayrollDetailId int NULL,
	Content nvarchar(MAX) COLLATE SQL_Latin1_General_CP1_CI_AS NULL,
	Status int NOT NULL,
	CreatedBy int NULL,
	CreatedName nvarchar(MAX) COLLATE SQL_Latin1_General_CP1_CI_AS NULL,
	CreatedAt datetime2 NULL,
	UpdatedBy int NULL,
	UpdatedName nvarchar(MAX) COLLATE SQL_Latin1_General_CP1_CI_AS NULL,
	UpdatedAt datetime2 NULL,
	IsDeleted bit NULL,
	CONSTRAINT PK_PayrollInquiries PRIMARY KEY (Id),
	CONSTRAINT FK_PayrollInquiries_PayrollDetails_PayrollDetailId FOREIGN KEY (PayrollDetailId) REFERENCES [HRM-Demo-1].dbo.PayrollDetails(Id)
);
 CREATE NONCLUSTERED INDEX IX_PayrollInquiries_PayrollDetailId ON HRM-Demo-1.dbo.PayrollInquiries (  PayrollDetailId ASC  )  
	 WITH (  PAD_INDEX = OFF ,FILLFACTOR = 100  ,SORT_IN_TEMPDB = OFF , IGNORE_DUP_KEY = OFF , STATISTICS_NORECOMPUTE = OFF , ONLINE = OFF , ALLOW_ROW_LOCKS = ON , ALLOW_PAGE_LOCKS = ON  )
	 ON [PRIMARY ] ;


-- [HRM-Demo-1].dbo.ProjectEmployees definition

-- Drop table

-- DROP TABLE [HRM-Demo-1].dbo.ProjectEmployees;

CREATE TABLE [HRM-Demo-1].dbo.ProjectEmployees (
	ProjectId int NOT NULL,
	EmployeeId int NOT NULL,
	ProjectRoleId int NOT NULL,
	CONSTRAINT PK_ProjectEmployees PRIMARY KEY (ProjectId,EmployeeId),
	CONSTRAINT FK_ProjectEmployees_Employees_EmployeeId FOREIGN KEY (EmployeeId) REFERENCES [HRM-Demo-1].dbo.Employees(Id) ON DELETE CASCADE,
	CONSTRAINT FK_ProjectEmployees_ProjectRoles_ProjectRoleId FOREIGN KEY (ProjectRoleId) REFERENCES [HRM-Demo-1].dbo.ProjectRoles(Id) ON DELETE CASCADE,
	CONSTRAINT FK_ProjectEmployees_Projects_ProjectId FOREIGN KEY (ProjectId) REFERENCES [HRM-Demo-1].dbo.Projects(Id) ON DELETE CASCADE
);
 CREATE NONCLUSTERED INDEX IX_ProjectEmployees_EmployeeId ON HRM-Demo-1.dbo.ProjectEmployees (  EmployeeId ASC  )  
	 WITH (  PAD_INDEX = OFF ,FILLFACTOR = 100  ,SORT_IN_TEMPDB = OFF , IGNORE_DUP_KEY = OFF , STATISTICS_NORECOMPUTE = OFF , ONLINE = OFF , ALLOW_ROW_LOCKS = ON , ALLOW_PAGE_LOCKS = ON  )
	 ON [PRIMARY ] ;
 CREATE NONCLUSTERED INDEX IX_ProjectEmployees_ProjectRoleId ON HRM-Demo-1.dbo.ProjectEmployees (  ProjectRoleId ASC  )  
	 WITH (  PAD_INDEX = OFF ,FILLFACTOR = 100  ,SORT_IN_TEMPDB = OFF , IGNORE_DUP_KEY = OFF , STATISTICS_NORECOMPUTE = OFF , ONLINE = OFF , ALLOW_ROW_LOCKS = ON , ALLOW_PAGE_LOCKS = ON  )
	 ON [PRIMARY ] ;


-- [HRM-Demo-1].dbo.UserClaims definition

-- Drop table

-- DROP TABLE [HRM-Demo-1].dbo.UserClaims;

CREATE TABLE [HRM-Demo-1].dbo.UserClaims (
	Id int IDENTITY(1,1) NOT NULL,
	UserId int NOT NULL,
	ClaimType nvarchar(MAX) COLLATE SQL_Latin1_General_CP1_CI_AS NULL,
	ClaimValue nvarchar(MAX) COLLATE SQL_Latin1_General_CP1_CI_AS NULL,
	CONSTRAINT PK_UserClaims PRIMARY KEY (Id),
	CONSTRAINT FK_UserClaims_Users_UserId FOREIGN KEY (UserId) REFERENCES [HRM-Demo-1].dbo.Users(Id) ON DELETE CASCADE
);
 CREATE NONCLUSTERED INDEX IX_UserClaims_UserId ON HRM-Demo-1.dbo.UserClaims (  UserId ASC  )  
	 WITH (  PAD_INDEX = OFF ,FILLFACTOR = 100  ,SORT_IN_TEMPDB = OFF , IGNORE_DUP_KEY = OFF , STATISTICS_NORECOMPUTE = OFF , ONLINE = OFF , ALLOW_ROW_LOCKS = ON , ALLOW_PAGE_LOCKS = ON  )
	 ON [PRIMARY ] ;


-- [HRM-Demo-1].dbo.UserLogins definition

-- Drop table

-- DROP TABLE [HRM-Demo-1].dbo.UserLogins;

CREATE TABLE [HRM-Demo-1].dbo.UserLogins (
	UserId int NOT NULL,
	LoginProvider nvarchar(MAX) COLLATE SQL_Latin1_General_CP1_CI_AS NULL,
	ProviderKey nvarchar(MAX) COLLATE SQL_Latin1_General_CP1_CI_AS NULL,
	ProviderDisplayName nvarchar(MAX) COLLATE SQL_Latin1_General_CP1_CI_AS NULL,
	CONSTRAINT PK_UserLogins PRIMARY KEY (UserId),
	CONSTRAINT FK_UserLogins_Users_UserId FOREIGN KEY (UserId) REFERENCES [HRM-Demo-1].dbo.Users(Id) ON DELETE CASCADE
);


-- [HRM-Demo-1].dbo.UserRoles definition

-- Drop table

-- DROP TABLE [HRM-Demo-1].dbo.UserRoles;

CREATE TABLE [HRM-Demo-1].dbo.UserRoles (
	UserId int NOT NULL,
	RoleId int NOT NULL,
	Discriminator nvarchar(21) COLLATE SQL_Latin1_General_CP1_CI_AS NOT NULL,
	RoleId1 int NULL,
	UserId1 int NULL,
	CONSTRAINT PK_UserRoles PRIMARY KEY (RoleId,UserId),
	CONSTRAINT FK_UserRoles_Roles_RoleId FOREIGN KEY (RoleId) REFERENCES [HRM-Demo-1].dbo.Roles(Id) ON DELETE CASCADE,
	CONSTRAINT FK_UserRoles_Roles_RoleId1 FOREIGN KEY (RoleId1) REFERENCES [HRM-Demo-1].dbo.Roles(Id),
	CONSTRAINT FK_UserRoles_Users_UserId FOREIGN KEY (UserId) REFERENCES [HRM-Demo-1].dbo.Users(Id) ON DELETE CASCADE,
	CONSTRAINT FK_UserRoles_Users_UserId1 FOREIGN KEY (UserId1) REFERENCES [HRM-Demo-1].dbo.Users(Id)
);
 CREATE NONCLUSTERED INDEX IX_UserRoles_RoleId1 ON HRM-Demo-1.dbo.UserRoles (  RoleId1 ASC  )  
	 WITH (  PAD_INDEX = OFF ,FILLFACTOR = 100  ,SORT_IN_TEMPDB = OFF , IGNORE_DUP_KEY = OFF , STATISTICS_NORECOMPUTE = OFF , ONLINE = OFF , ALLOW_ROW_LOCKS = ON , ALLOW_PAGE_LOCKS = ON  )
	 ON [PRIMARY ] ;
 CREATE NONCLUSTERED INDEX IX_UserRoles_UserId ON HRM-Demo-1.dbo.UserRoles (  UserId ASC  )  
	 WITH (  PAD_INDEX = OFF ,FILLFACTOR = 100  ,SORT_IN_TEMPDB = OFF , IGNORE_DUP_KEY = OFF , STATISTICS_NORECOMPUTE = OFF , ONLINE = OFF , ALLOW_ROW_LOCKS = ON , ALLOW_PAGE_LOCKS = ON  )
	 ON [PRIMARY ] ;
 CREATE NONCLUSTERED INDEX IX_UserRoles_UserId1 ON HRM-Demo-1.dbo.UserRoles (  UserId1 ASC  )  
	 WITH (  PAD_INDEX = OFF ,FILLFACTOR = 100  ,SORT_IN_TEMPDB = OFF , IGNORE_DUP_KEY = OFF , STATISTICS_NORECOMPUTE = OFF , ONLINE = OFF , ALLOW_ROW_LOCKS = ON , ALLOW_PAGE_LOCKS = ON  )
	 ON [PRIMARY ] ;


-- [HRM-Demo-1].dbo.UserTokens definition

-- Drop table

-- DROP TABLE [HRM-Demo-1].dbo.UserTokens;

CREATE TABLE [HRM-Demo-1].dbo.UserTokens (
	UserId int NOT NULL,
	LoginProvider nvarchar(MAX) COLLATE SQL_Latin1_General_CP1_CI_AS NULL,
	Name nvarchar(MAX) COLLATE SQL_Latin1_General_CP1_CI_AS NULL,
	Value nvarchar(MAX) COLLATE SQL_Latin1_General_CP1_CI_AS NULL,
	CONSTRAINT PK_UserTokens PRIMARY KEY (UserId),
	CONSTRAINT FK_UserTokens_Users_UserId FOREIGN KEY (UserId) REFERENCES [HRM-Demo-1].dbo.Users(Id) ON DELETE CASCADE
);


-- [HRM-Demo-1].dbo.Works definition

-- Drop table

-- DROP TABLE [HRM-Demo-1].dbo.Works;

CREATE TABLE [HRM-Demo-1].dbo.Works (
	Id int IDENTITY(1,1) NOT NULL,
	ReporterId int NULL,
	ProjectId int NULL,
	GroupWorkId int NULL,
	Name nvarchar(MAX) COLLATE SQL_Latin1_General_CP1_CI_AS NULL,
	Description nvarchar(MAX) COLLATE SQL_Latin1_General_CP1_CI_AS NULL,
	StartTime datetime2 NULL,
	DueDate datetime2 NULL,
	CreatedBy int NULL,
	CreatedName nvarchar(MAX) COLLATE SQL_Latin1_General_CP1_CI_AS NULL,
	CreatedAt datetime2 NULL,
	UpdatedBy int NULL,
	UpdatedName nvarchar(MAX) COLLATE SQL_Latin1_General_CP1_CI_AS NULL,
	UpdatedAt datetime2 NULL,
	IsDeleted bit NULL,
	FilePath nvarchar(MAX) COLLATE SQL_Latin1_General_CP1_CI_AS NULL,
	IsPin bit DEFAULT CONVERT([bit],(0)) NOT NULL,
	WorkPiority int DEFAULT 0 NOT NULL,
	ExecutorId int NULL,
	WorkStatus int DEFAULT 0 NOT NULL,
	CONSTRAINT PK_Works PRIMARY KEY (Id),
	CONSTRAINT FK_Works_Employees_ExecutorId FOREIGN KEY (ExecutorId) REFERENCES [HRM-Demo-1].dbo.Employees(Id),
	CONSTRAINT FK_Works_Employees_ReporterId FOREIGN KEY (ReporterId) REFERENCES [HRM-Demo-1].dbo.Employees(Id),
	CONSTRAINT FK_Works_GroupWorks_GroupWorkId FOREIGN KEY (GroupWorkId) REFERENCES [HRM-Demo-1].dbo.GroupWorks(Id)
);
 CREATE NONCLUSTERED INDEX IX_Works_ExecutorId ON HRM-Demo-1.dbo.Works (  ExecutorId ASC  )  
	 WITH (  PAD_INDEX = OFF ,FILLFACTOR = 100  ,SORT_IN_TEMPDB = OFF , IGNORE_DUP_KEY = OFF , STATISTICS_NORECOMPUTE = OFF , ONLINE = OFF , ALLOW_ROW_LOCKS = ON , ALLOW_PAGE_LOCKS = ON  )
	 ON [PRIMARY ] ;
 CREATE NONCLUSTERED INDEX IX_Works_GroupWorkId ON HRM-Demo-1.dbo.Works (  GroupWorkId ASC  )  
	 WITH (  PAD_INDEX = OFF ,FILLFACTOR = 100  ,SORT_IN_TEMPDB = OFF , IGNORE_DUP_KEY = OFF , STATISTICS_NORECOMPUTE = OFF , ONLINE = OFF , ALLOW_ROW_LOCKS = ON , ALLOW_PAGE_LOCKS = ON  )
	 ON [PRIMARY ] ;
 CREATE NONCLUSTERED INDEX IX_Works_ReporterId ON HRM-Demo-1.dbo.Works (  ReporterId ASC  )  
	 WITH (  PAD_INDEX = OFF ,FILLFACTOR = 100  ,SORT_IN_TEMPDB = OFF , IGNORE_DUP_KEY = OFF , STATISTICS_NORECOMPUTE = OFF , ONLINE = OFF , ALLOW_ROW_LOCKS = ON , ALLOW_PAGE_LOCKS = ON  )
	 ON [PRIMARY ] ;


-- [HRM-Demo-1].dbo.Allowances definition

-- Drop table

-- DROP TABLE [HRM-Demo-1].dbo.Allowances;

CREATE TABLE [HRM-Demo-1].dbo.Allowances (
	Id int IDENTITY(1,1) NOT NULL,
	AllowNameId int NULL,
	AllowanceName nvarchar(MAX) COLLATE SQL_Latin1_General_CP1_CI_AS NULL,
	StandardTypeId int NULL,
	StandardType nvarchar(MAX) COLLATE SQL_Latin1_General_CP1_CI_AS NULL,
	StandardValue decimal(18,2) NULL,
	TypeValueId int NULL,
	TypeValue nvarchar(MAX) COLLATE SQL_Latin1_General_CP1_CI_AS NULL,
	Value decimal(18,2) NULL,
	Note nvarchar(MAX) COLLATE SQL_Latin1_General_CP1_CI_AS NULL,
	ContractId int NOT NULL,
	CreatedBy int NULL,
	CreatedName nvarchar(MAX) COLLATE SQL_Latin1_General_CP1_CI_AS NULL,
	CreatedAt datetime2 NULL,
	UpdatedBy int NULL,
	UpdatedName nvarchar(MAX) COLLATE SQL_Latin1_General_CP1_CI_AS NULL,
	UpdatedAt datetime2 NULL,
	IsDeleted bit NULL,
	CONSTRAINT PK_Allowances PRIMARY KEY (Id),
	CONSTRAINT FK_Allowances_Contracts_ContractId FOREIGN KEY (ContractId) REFERENCES [HRM-Demo-1].dbo.Contracts(Id) ON DELETE CASCADE
);
 CREATE NONCLUSTERED INDEX IX_Allowances_ContractId ON HRM-Demo-1.dbo.Allowances (  ContractId ASC  )  
	 WITH (  PAD_INDEX = OFF ,FILLFACTOR = 100  ,SORT_IN_TEMPDB = OFF , IGNORE_DUP_KEY = OFF , STATISTICS_NORECOMPUTE = OFF , ONLINE = OFF , ALLOW_ROW_LOCKS = ON , ALLOW_PAGE_LOCKS = ON  )
	 ON [PRIMARY ] ;


-- [HRM-Demo-1].dbo.Approvals definition

-- Drop table

-- DROP TABLE [HRM-Demo-1].dbo.Approvals;

CREATE TABLE [HRM-Demo-1].dbo.Approvals (
	Id int IDENTITY(1,1) NOT NULL,
	ApproveDate datetime2 NULL,
	FileUrl nvarchar(MAX) COLLATE SQL_Latin1_General_CP1_CI_AS NULL,
	Description nvarchar(MAX) COLLATE SQL_Latin1_General_CP1_CI_AS NULL,
	IsApprove bit NULL,
	RejectReason nvarchar(MAX) COLLATE SQL_Latin1_General_CP1_CI_AS NULL,
	CreatedBy int NULL,
	CreatedName nvarchar(MAX) COLLATE SQL_Latin1_General_CP1_CI_AS NULL,
	CreatedAt datetime2 NULL,
	UpdatedBy int NULL,
	UpdatedName nvarchar(MAX) COLLATE SQL_Latin1_General_CP1_CI_AS NULL,
	UpdatedAt datetime2 NULL,
	IsDeleted bit NULL,
	WorkId int NULL,
	CONSTRAINT PK_Approvals PRIMARY KEY (Id),
	CONSTRAINT FK_Approvals_Works_WorkId FOREIGN KEY (WorkId) REFERENCES [HRM-Demo-1].dbo.Works(Id)
);
 CREATE NONCLUSTERED INDEX IX_Approvals_WorkId ON HRM-Demo-1.dbo.Approvals (  WorkId ASC  )  
	 WITH (  PAD_INDEX = OFF ,FILLFACTOR = 100  ,SORT_IN_TEMPDB = OFF , IGNORE_DUP_KEY = OFF , STATISTICS_NORECOMPUTE = OFF , ONLINE = OFF , ALLOW_ROW_LOCKS = ON , ALLOW_PAGE_LOCKS = ON  )
	 ON [PRIMARY ] ;


-- [HRM-Demo-1].dbo.CheckLists definition

-- Drop table

-- DROP TABLE [HRM-Demo-1].dbo.CheckLists;

CREATE TABLE [HRM-Demo-1].dbo.CheckLists (
	Id int IDENTITY(1,1) NOT NULL,
	WorkId int NULL,
	Name nvarchar(MAX) COLLATE SQL_Latin1_General_CP1_CI_AS NULL,
	IsDone bit NULL,
	CreatedBy int NULL,
	CreatedName nvarchar(MAX) COLLATE SQL_Latin1_General_CP1_CI_AS NULL,
	CreatedAt datetime2 NULL,
	UpdatedBy int NULL,
	UpdatedName nvarchar(MAX) COLLATE SQL_Latin1_General_CP1_CI_AS NULL,
	UpdatedAt datetime2 NULL,
	IsDeleted bit NULL,
	CONSTRAINT PK_CheckLists PRIMARY KEY (Id),
	CONSTRAINT FK_CheckLists_Works_WorkId FOREIGN KEY (WorkId) REFERENCES [HRM-Demo-1].dbo.Works(Id)
);
 CREATE NONCLUSTERED INDEX IX_CheckLists_WorkId ON HRM-Demo-1.dbo.CheckLists (  WorkId ASC  )  
	 WITH (  PAD_INDEX = OFF ,FILLFACTOR = 100  ,SORT_IN_TEMPDB = OFF , IGNORE_DUP_KEY = OFF , STATISTICS_NORECOMPUTE = OFF , ONLINE = OFF , ALLOW_ROW_LOCKS = ON , ALLOW_PAGE_LOCKS = ON  )
	 ON [PRIMARY ] ;


-- [HRM-Demo-1].dbo.DelegationEmployees definition

-- Drop table

-- DROP TABLE [HRM-Demo-1].dbo.DelegationEmployees;

CREATE TABLE [HRM-Demo-1].dbo.DelegationEmployees (
	EmployeeId int NOT NULL,
	DelegationId int NOT NULL,
	CONSTRAINT PK_DelegationEmployees PRIMARY KEY (DelegationId,EmployeeId),
	CONSTRAINT FK_DelegationEmployees_Delegations_DelegationId FOREIGN KEY (DelegationId) REFERENCES [HRM-Demo-1].dbo.Delegations(Id) ON DELETE CASCADE,
	CONSTRAINT FK_DelegationEmployees_Employees_EmployeeId FOREIGN KEY (EmployeeId) REFERENCES [HRM-Demo-1].dbo.Employees(Id) ON DELETE CASCADE
);
 CREATE NONCLUSTERED INDEX IX_DelegationEmployees_EmployeeId ON HRM-Demo-1.dbo.DelegationEmployees (  EmployeeId ASC  )  
	 WITH (  PAD_INDEX = OFF ,FILLFACTOR = 100  ,SORT_IN_TEMPDB = OFF , IGNORE_DUP_KEY = OFF , STATISTICS_NORECOMPUTE = OFF , ONLINE = OFF , ALLOW_ROW_LOCKS = ON , ALLOW_PAGE_LOCKS = ON  )
	 ON [PRIMARY ] ;


-- [HRM-Demo-1].dbo.DelegationProjects definition

-- Drop table

-- DROP TABLE [HRM-Demo-1].dbo.DelegationProjects;

CREATE TABLE [HRM-Demo-1].dbo.DelegationProjects (
	ProjectId int NOT NULL,
	DelegationId int NOT NULL,
	CONSTRAINT PK_DelegationProjects PRIMARY KEY (DelegationId,ProjectId),
	CONSTRAINT FK_DelegationProjects_Delegations_DelegationId FOREIGN KEY (DelegationId) REFERENCES [HRM-Demo-1].dbo.Delegations(Id) ON DELETE CASCADE,
	CONSTRAINT FK_DelegationProjects_Projects_ProjectId FOREIGN KEY (ProjectId) REFERENCES [HRM-Demo-1].dbo.Projects(Id) ON DELETE CASCADE
);
 CREATE NONCLUSTERED INDEX IX_DelegationProjects_ProjectId ON HRM-Demo-1.dbo.DelegationProjects (  ProjectId ASC  )  
	 WITH (  PAD_INDEX = OFF ,FILLFACTOR = 100  ,SORT_IN_TEMPDB = OFF , IGNORE_DUP_KEY = OFF , STATISTICS_NORECOMPUTE = OFF , ONLINE = OFF , ALLOW_ROW_LOCKS = ON , ALLOW_PAGE_LOCKS = ON  )
	 ON [PRIMARY ] ;


-- [HRM-Demo-1].dbo.RemindWorks definition

-- Drop table

-- DROP TABLE [HRM-Demo-1].dbo.RemindWorks;

CREATE TABLE [HRM-Demo-1].dbo.RemindWorks (
	Id int IDENTITY(1,1) NOT NULL,
	WorkId int NOT NULL,
	RemindWorkType int NOT NULL,
	TimeRemindStart float NULL,
	TimeRemindEnd float NULL,
	CreatedBy int NULL,
	CreatedName nvarchar(MAX) COLLATE SQL_Latin1_General_CP1_CI_AS NULL,
	CreatedAt datetime2 NULL,
	UpdatedBy int NULL,
	UpdatedName nvarchar(MAX) COLLATE SQL_Latin1_General_CP1_CI_AS NULL,
	UpdatedAt datetime2 NULL,
	IsDeleted bit NULL,
	CONSTRAINT PK_RemindWorks PRIMARY KEY (Id),
	CONSTRAINT FK_RemindWorks_Works_WorkId FOREIGN KEY (WorkId) REFERENCES [HRM-Demo-1].dbo.Works(Id) ON DELETE CASCADE
);
 CREATE UNIQUE NONCLUSTERED INDEX IX_RemindWorks_WorkId ON HRM-Demo-1.dbo.RemindWorks (  WorkId ASC  )  
	 WITH (  PAD_INDEX = OFF ,FILLFACTOR = 100  ,SORT_IN_TEMPDB = OFF , IGNORE_DUP_KEY = OFF , STATISTICS_NORECOMPUTE = OFF , ONLINE = OFF , ALLOW_ROW_LOCKS = ON , ALLOW_PAGE_LOCKS = ON  )
	 ON [PRIMARY ] ;


-- [HRM-Demo-1].dbo.SubWork definition

-- Drop table

-- DROP TABLE [HRM-Demo-1].dbo.SubWork;

CREATE TABLE [HRM-Demo-1].dbo.SubWork (
	Id int IDENTITY(1,1) NOT NULL,
	WorkId int NOT NULL,
	Name nvarchar(MAX) COLLATE SQL_Latin1_General_CP1_CI_AS NULL,
	AssignEmployeeId int NOT NULL,
	DueDate datetime2 NULL,
	IsFinish bit NOT NULL,
	CreatedBy int NULL,
	CreatedName nvarchar(MAX) COLLATE SQL_Latin1_General_CP1_CI_AS NULL,
	CreatedAt datetime2 NULL,
	UpdatedBy int NULL,
	UpdatedName nvarchar(MAX) COLLATE SQL_Latin1_General_CP1_CI_AS NULL,
	UpdatedAt datetime2 NULL,
	IsDeleted bit NULL,
	StartDate datetime2 NULL,
	CONSTRAINT PK_SubWork PRIMARY KEY (Id),
	CONSTRAINT FK_SubWork_Employees_AssignEmployeeId FOREIGN KEY (AssignEmployeeId) REFERENCES [HRM-Demo-1].dbo.Employees(Id) ON DELETE CASCADE,
	CONSTRAINT FK_SubWork_Works_WorkId FOREIGN KEY (WorkId) REFERENCES [HRM-Demo-1].dbo.Works(Id) ON DELETE CASCADE
);
 CREATE NONCLUSTERED INDEX IX_SubWork_AssignEmployeeId ON HRM-Demo-1.dbo.SubWork (  AssignEmployeeId ASC  )  
	 WITH (  PAD_INDEX = OFF ,FILLFACTOR = 100  ,SORT_IN_TEMPDB = OFF , IGNORE_DUP_KEY = OFF , STATISTICS_NORECOMPUTE = OFF , ONLINE = OFF , ALLOW_ROW_LOCKS = ON , ALLOW_PAGE_LOCKS = ON  )
	 ON [PRIMARY ] ;
 CREATE NONCLUSTERED INDEX IX_SubWork_WorkId ON HRM-Demo-1.dbo.SubWork (  WorkId ASC  )  
	 WITH (  PAD_INDEX = OFF ,FILLFACTOR = 100  ,SORT_IN_TEMPDB = OFF , IGNORE_DUP_KEY = OFF , STATISTICS_NORECOMPUTE = OFF , ONLINE = OFF , ALLOW_ROW_LOCKS = ON , ALLOW_PAGE_LOCKS = ON  )
	 ON [PRIMARY ] ;


-- [HRM-Demo-1].dbo.TagsWork definition

-- Drop table

-- DROP TABLE [HRM-Demo-1].dbo.TagsWork;

CREATE TABLE [HRM-Demo-1].dbo.TagsWork (
	TagId int NOT NULL,
	WorkId int NOT NULL,
	CONSTRAINT PK_TagsWork PRIMARY KEY (WorkId,TagId),
	CONSTRAINT FK_TagsWork_Tags_TagId FOREIGN KEY (TagId) REFERENCES [HRM-Demo-1].dbo.Tags(Id) ON DELETE CASCADE,
	CONSTRAINT FK_TagsWork_Works_WorkId FOREIGN KEY (WorkId) REFERENCES [HRM-Demo-1].dbo.Works(Id) ON DELETE CASCADE
);
 CREATE NONCLUSTERED INDEX IX_TagsWork_TagId ON HRM-Demo-1.dbo.TagsWork (  TagId ASC  )  
	 WITH (  PAD_INDEX = OFF ,FILLFACTOR = 100  ,SORT_IN_TEMPDB = OFF , IGNORE_DUP_KEY = OFF , STATISTICS_NORECOMPUTE = OFF , ONLINE = OFF , ALLOW_ROW_LOCKS = ON , ALLOW_PAGE_LOCKS = ON  )
	 ON [PRIMARY ] ;


-- [HRM-Demo-1].dbo.WorkAssignments definition

-- Drop table

-- DROP TABLE [HRM-Demo-1].dbo.WorkAssignments;

CREATE TABLE [HRM-Demo-1].dbo.WorkAssignments (
	WorkId int NOT NULL,
	AssigneeId int NOT NULL,
	CONSTRAINT PK_WorkAssignments PRIMARY KEY (WorkId,AssigneeId),
	CONSTRAINT FK_WorkAssignments_Employees_AssigneeId FOREIGN KEY (AssigneeId) REFERENCES [HRM-Demo-1].dbo.Employees(Id) ON DELETE CASCADE,
	CONSTRAINT FK_WorkAssignments_Works_WorkId FOREIGN KEY (WorkId) REFERENCES [HRM-Demo-1].dbo.Works(Id) ON DELETE CASCADE
);
 CREATE NONCLUSTERED INDEX IX_WorkAssignments_AssigneeId ON HRM-Demo-1.dbo.WorkAssignments (  AssigneeId ASC  )  
	 WITH (  PAD_INDEX = OFF ,FILLFACTOR = 100  ,SORT_IN_TEMPDB = OFF , IGNORE_DUP_KEY = OFF , STATISTICS_NORECOMPUTE = OFF , ONLINE = OFF , ALLOW_ROW_LOCKS = ON , ALLOW_PAGE_LOCKS = ON  )
	 ON [PRIMARY ] ;


-- [HRM-Demo-1].dbo.ApprovalEmployees definition

-- Drop table

-- DROP TABLE [HRM-Demo-1].dbo.ApprovalEmployees;

CREATE TABLE [HRM-Demo-1].dbo.ApprovalEmployees (
	Id int IDENTITY(1,1) NOT NULL,
	Description nvarchar(MAX) COLLATE SQL_Latin1_General_CP1_CI_AS NULL,
	IsApprove bit NULL,
	RejectReason nvarchar(MAX) COLLATE SQL_Latin1_General_CP1_CI_AS NULL,
	EmployeeId int NULL,
	ApprovalId int NULL,
	CreatedBy int NULL,
	CreatedName nvarchar(MAX) COLLATE SQL_Latin1_General_CP1_CI_AS NULL,
	CreatedAt datetime2 NULL,
	UpdatedBy int NULL,
	UpdatedName nvarchar(MAX) COLLATE SQL_Latin1_General_CP1_CI_AS NULL,
	UpdatedAt datetime2 NULL,
	IsDeleted bit NULL,
	CONSTRAINT PK_ApprovalEmployees PRIMARY KEY (Id),
	CONSTRAINT FK_ApprovalEmployees_Approvals_ApprovalId FOREIGN KEY (ApprovalId) REFERENCES [HRM-Demo-1].dbo.Approvals(Id),
	CONSTRAINT FK_ApprovalEmployees_Employees_EmployeeId FOREIGN KEY (EmployeeId) REFERENCES [HRM-Demo-1].dbo.Employees(Id)
);
 CREATE NONCLUSTERED INDEX IX_ApprovalEmployees_ApprovalId ON HRM-Demo-1.dbo.ApprovalEmployees (  ApprovalId ASC  )  
	 WITH (  PAD_INDEX = OFF ,FILLFACTOR = 100  ,SORT_IN_TEMPDB = OFF , IGNORE_DUP_KEY = OFF , STATISTICS_NORECOMPUTE = OFF , ONLINE = OFF , ALLOW_ROW_LOCKS = ON , ALLOW_PAGE_LOCKS = ON  )
	 ON [PRIMARY ] ;
 CREATE NONCLUSTERED INDEX IX_ApprovalEmployees_EmployeeId ON HRM-Demo-1.dbo.ApprovalEmployees (  EmployeeId ASC  )  
	 WITH (  PAD_INDEX = OFF ,FILLFACTOR = 100  ,SORT_IN_TEMPDB = OFF , IGNORE_DUP_KEY = OFF , STATISTICS_NORECOMPUTE = OFF , ONLINE = OFF , ALLOW_ROW_LOCKS = ON , ALLOW_PAGE_LOCKS = ON  )
	 ON [PRIMARY ] ;


-- [HRM-Demo-1].dbo.Notifications definition

-- Drop table

-- DROP TABLE [HRM-Demo-1].dbo.Notifications;

CREATE TABLE [HRM-Demo-1].dbo.Notifications (
	Id int IDENTITY(1,1) NOT NULL,
	EmployeeReceiveId int NULL,
	EmployeeSendId int NULL,
	Url nvarchar(MAX) COLLATE SQL_Latin1_General_CP1_CI_AS NULL,
	ContractId int NULL,
	WorkId int NULL,
	RemindWorkId int NULL,
	NotificationType int NOT NULL,
	Content nvarchar(MAX) COLLATE SQL_Latin1_General_CP1_CI_AS NULL,
	CreatedBy int NULL,
	CreatedName nvarchar(MAX) COLLATE SQL_Latin1_General_CP1_CI_AS NULL,
	CreatedAt datetime2 NULL,
	UpdatedBy int NULL,
	UpdatedName nvarchar(MAX) COLLATE SQL_Latin1_General_CP1_CI_AS NULL,
	UpdatedAt datetime2 NULL,
	IsDeleted bit NULL,
	IsRead bit DEFAULT CONVERT([bit],(0)) NOT NULL,
	CheckInCheckOutId int NULL,
	CONSTRAINT PK_Notifications PRIMARY KEY (Id),
	CONSTRAINT FK_Notifications_CheckInCheckOutApplications_CheckInCheckOutId FOREIGN KEY (CheckInCheckOutId) REFERENCES [HRM-Demo-1].dbo.CheckInCheckOutApplications(Id),
	CONSTRAINT FK_Notifications_Contracts_ContractId FOREIGN KEY (ContractId) REFERENCES [HRM-Demo-1].dbo.Contracts(Id),
	CONSTRAINT FK_Notifications_Employees_EmployeeReceiveId FOREIGN KEY (EmployeeReceiveId) REFERENCES [HRM-Demo-1].dbo.Employees(Id),
	CONSTRAINT FK_Notifications_Employees_EmployeeSendId FOREIGN KEY (EmployeeSendId) REFERENCES [HRM-Demo-1].dbo.Employees(Id),
	CONSTRAINT FK_Notifications_RemindWorks_RemindWorkId FOREIGN KEY (RemindWorkId) REFERENCES [HRM-Demo-1].dbo.RemindWorks(Id),
	CONSTRAINT FK_Notifications_Works_WorkId FOREIGN KEY (WorkId) REFERENCES [HRM-Demo-1].dbo.Works(Id)
);
 CREATE NONCLUSTERED INDEX IX_Notifications_CheckInCheckOutId ON HRM-Demo-1.dbo.Notifications (  CheckInCheckOutId ASC  )  
	 WITH (  PAD_INDEX = OFF ,FILLFACTOR = 100  ,SORT_IN_TEMPDB = OFF , IGNORE_DUP_KEY = OFF , STATISTICS_NORECOMPUTE = OFF , ONLINE = OFF , ALLOW_ROW_LOCKS = ON , ALLOW_PAGE_LOCKS = ON  )
	 ON [PRIMARY ] ;
 CREATE NONCLUSTERED INDEX IX_Notifications_ContractId ON HRM-Demo-1.dbo.Notifications (  ContractId ASC  )  
	 WITH (  PAD_INDEX = OFF ,FILLFACTOR = 100  ,SORT_IN_TEMPDB = OFF , IGNORE_DUP_KEY = OFF , STATISTICS_NORECOMPUTE = OFF , ONLINE = OFF , ALLOW_ROW_LOCKS = ON , ALLOW_PAGE_LOCKS = ON  )
	 ON [PRIMARY ] ;
 CREATE NONCLUSTERED INDEX IX_Notifications_EmployeeReceiveId ON HRM-Demo-1.dbo.Notifications (  EmployeeReceiveId ASC  )  
	 WITH (  PAD_INDEX = OFF ,FILLFACTOR = 100  ,SORT_IN_TEMPDB = OFF , IGNORE_DUP_KEY = OFF , STATISTICS_NORECOMPUTE = OFF , ONLINE = OFF , ALLOW_ROW_LOCKS = ON , ALLOW_PAGE_LOCKS = ON  )
	 ON [PRIMARY ] ;
 CREATE NONCLUSTERED INDEX IX_Notifications_EmployeeSendId ON HRM-Demo-1.dbo.Notifications (  EmployeeSendId ASC  )  
	 WITH (  PAD_INDEX = OFF ,FILLFACTOR = 100  ,SORT_IN_TEMPDB = OFF , IGNORE_DUP_KEY = OFF , STATISTICS_NORECOMPUTE = OFF , ONLINE = OFF , ALLOW_ROW_LOCKS = ON , ALLOW_PAGE_LOCKS = ON  )
	 ON [PRIMARY ] ;
 CREATE NONCLUSTERED INDEX IX_Notifications_RemindWorkId ON HRM-Demo-1.dbo.Notifications (  RemindWorkId ASC  )  
	 WITH (  PAD_INDEX = OFF ,FILLFACTOR = 100  ,SORT_IN_TEMPDB = OFF , IGNORE_DUP_KEY = OFF , STATISTICS_NORECOMPUTE = OFF , ONLINE = OFF , ALLOW_ROW_LOCKS = ON , ALLOW_PAGE_LOCKS = ON  )
	 ON [PRIMARY ] ;
 CREATE NONCLUSTERED INDEX IX_Notifications_WorkId ON HRM-Demo-1.dbo.Notifications (  WorkId ASC  )  
	 WITH (  PAD_INDEX = OFF ,FILLFACTOR = 100  ,SORT_IN_TEMPDB = OFF , IGNORE_DUP_KEY = OFF , STATISTICS_NORECOMPUTE = OFF , ONLINE = OFF , ALLOW_ROW_LOCKS = ON , ALLOW_PAGE_LOCKS = ON  )
	 ON [PRIMARY ] ;