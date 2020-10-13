CREATE TABLE Tenant (
  Id int IDENTITY(1,1) NOT NULL,
  [Name] nvarchar(max) NOT NULL,
  [IsActive] bit NOT NULL Default 1,
  TenantConnectionString nvarchar(max) NULL,
  TeamsRegistrationCode nvarchar(max) NULL
  )
