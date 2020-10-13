CREATE TABLE TenantSettings (
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[TeamsTenantId] [nvarchar](max) NULL,
	[HarvestAccountId] [nvarchar](max) NULL,
	[HarvestUrl] [nvarchar](max) NULL,
	[DevOpsAccessToken] [nvarchar](max) NULL,
	[DevOpsOrganization] [nvarchar](max) NULL,
	[HarvestAccessToken] [nvarchar](max) NULL,
	[HarvestAccessTokenExpiryDate] [datetimeoffset](7) NULL,
	[HarvestRefreshToken] [nvarchar](max) NULL,
	[SlackBotToken] [nvarchar](max) NULL,
	[PlatformId] [int] NULL,
	[TeamsRegistrationCode] [nvarchar](max) NULL,
	[IsTeamsRegistered] [bit] NOT NULL Default 0,
	[ServiceUrl] [nvarchar](max) NULL,
	[TeamsServiceUrl] [nvarchar](max) NULL
	)