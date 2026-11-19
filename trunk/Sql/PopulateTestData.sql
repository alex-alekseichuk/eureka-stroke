
-- Format
SET IDENTITY_INSERT [dbo].[Format] ON
GO

INSERT INTO [dbo].[Format] ([ID], [Name], [IsNative], [KeyFields], [Status])
VALUES (3, N'alien1', 0, NULL, 0)
GO

if (not exists (select * from [dbo].[Format] where IsNative = 1) )
	INSERT INTO [dbo].[Format] ([ID], [Name], [IsNative], [KeyFields], [Status])
	VALUES (1, N'EurekaTest', 1, N'PatientID,AdmissionDate', 1)
GO

SET IDENTITY_INSERT [dbo].[Format] OFF
GO

-- Facility
SET IDENTITY_INSERT [dbo].[Facility] ON
GO


INSERT INTO [dbo].[Facility] ([ID], [Name], [FormatID])
VALUES (3, N'facility1', 3)
GO

SET IDENTITY_INSERT [dbo].[Facility] OFF
GO

-- Eureka Field
SET IDENTITY_INSERT [dbo].[EurekaField] ON
GO

INSERT INTO [dbo].[EurekaField] ([ID], [Name], [FormatID], [FieldType], [Description], [IsMandatory], [CreatedBy], [LastModifiedBy], [CreatedDate], [LastModifiedDate], [IsNew])
VALUES (348, N'PatientID', 1, 0, N'Eureka PatientID', 0, NULL, NULL, NULL, NULL, 1)
GO

INSERT INTO [dbo].[EurekaField] ([ID], [Name], [FormatID], [FieldType], [Description], [IsMandatory], [CreatedBy], [LastModifiedBy], [CreatedDate], [LastModifiedDate], [IsNew])
VALUES (349, N'AdmissionDate', 1, 4, N'Eureka Admission date', 0, NULL, NULL, NULL, NULL, 1)
GO

INSERT INTO [dbo].[EurekaField] ([ID], [Name], [FormatID], [FieldType], [Description], [IsMandatory], [CreatedBy], [LastModifiedBy], [CreatedDate], [LastModifiedDate], [IsNew])
VALUES (350, N'E.String', 1, 0, N'Eureka String', 0, NULL, NULL, NULL, NULL, 1)
GO

INSERT INTO [dbo].[EurekaField] ([ID], [Name], [FormatID], [FieldType], [Description], [IsMandatory], [CreatedBy], [LastModifiedBy], [CreatedDate], [LastModifiedDate], [IsNew])
VALUES (351, N'DateTime', 1, 4, N'Eureka datetime', 0, NULL, NULL, NULL, NULL, 1)
GO

INSERT INTO [dbo].[EurekaField] ([ID], [Name], [FormatID], [FieldType], [Description], [IsMandatory], [CreatedBy], [LastModifiedBy], [CreatedDate], [LastModifiedDate], [IsNew])
VALUES (352, N'StaticSingle', 1, 5, N'Eureka StaticSingle', 0, NULL, NULL, NULL, NULL, 1)
GO

SET IDENTITY_INSERT [dbo].[EurekaField] OFF
GO


-- AlienField
SET IDENTITY_INSERT [dbo].[AlienField] ON
GO

INSERT INTO [dbo].[AlienField] ([ID], [Name], [EurekaFieldID], [FormatID], [RegularExpression], [CreatedBy], [LastModifiedBy], [CreatedDate], [LastModifiedDate])
VALUES (348, N'A.PatientID', 348, 3, N'', NULL, NULL, NULL, NULL)
GO

INSERT INTO [dbo].[AlienField] ([ID], [Name], [EurekaFieldID], [FormatID], [RegularExpression], [CreatedBy], [LastModifiedBy], [CreatedDate], [LastModifiedDate])
VALUES (349, N'A.AdmissionDate', 349, 3, N'', NULL, NULL, NULL, NULL)
GO

INSERT INTO [dbo].[AlienField] ([ID], [Name], [EurekaFieldID], [FormatID], [RegularExpression], [CreatedBy], [LastModifiedBy], [CreatedDate], [LastModifiedDate])
VALUES (350, N'A.String', 350, 3, N'', NULL, NULL, NULL, NULL)
GO

INSERT INTO [dbo].[AlienField] ([ID], [Name], [EurekaFieldID], [FormatID], [RegularExpression], [CreatedBy], [LastModifiedBy], [CreatedDate], [LastModifiedDate])
VALUES (351, N'A.DateTime', 351, 3, N'', NULL, NULL, NULL, NULL)
GO

INSERT INTO [dbo].[AlienField] ([ID], [Name], [EurekaFieldID], [FormatID], [RegularExpression], [CreatedBy], [LastModifiedBy], [CreatedDate], [LastModifiedDate])
VALUES (352, N'A.StaticSingle', 352, 3, N'', NULL, NULL, NULL, NULL)
GO

SET IDENTITY_INSERT [dbo].[AlienField] OFF
GO

-- Eureka Value
SET IDENTITY_INSERT [dbo].[EurekaValue] ON
GO

INSERT INTO [dbo].[EurekaValue] ([ID], [EurekaFieldID], [Value], [Description], [CreatedBy], [LastModifiedBy], [CreatedDate], [LastModifiedDate])
VALUES (640, 352, N'1', N'eureka value 1', NULL, NULL, NULL, NULL)
GO

INSERT INTO [dbo].[EurekaValue] ([ID], [EurekaFieldID], [Value], [Description], [CreatedBy], [LastModifiedBy], [CreatedDate], [LastModifiedDate])
VALUES (641, 352, N'2', N'eureka value 2', NULL, NULL, NULL, NULL)
GO

INSERT INTO [dbo].[EurekaValue] ([ID], [EurekaFieldID], [Value], [Description], [CreatedBy], [LastModifiedBy], [CreatedDate], [LastModifiedDate])
VALUES (642, 352, N'3', N'eureka value 3', NULL, NULL, NULL, NULL)
GO

INSERT INTO [dbo].[EurekaValue] ([ID], [EurekaFieldID], [Value], [Description], [CreatedBy], [LastModifiedBy], [CreatedDate], [LastModifiedDate])
VALUES (643, 352, N'4', N'eureka value 4', NULL, NULL, NULL, NULL)
GO

SET IDENTITY_INSERT [dbo].[EurekaValue] OFF
GO

-- Alien Value
SET IDENTITY_INSERT [dbo].[AlienValue] ON
GO

INSERT INTO [dbo].[AlienValue] ([ID], [EurekaFieldID], [FormatID], [Value], [EurekaValueID], [CreatedBy], [LastModifiedBy], [CreatedDate], [LastModifiedDate], [Description])
VALUES (640, 352, 3, N'11', 640, NULL, NULL, NULL, NULL, N'alien value 1')
GO

INSERT INTO [dbo].[AlienValue] ([ID], [EurekaFieldID], [FormatID], [Value], [EurekaValueID], [CreatedBy], [LastModifiedBy], [CreatedDate], [LastModifiedDate], [Description])
VALUES (641, 352, 3, N'12', 641, NULL, NULL, NULL, NULL, N'alien value 2')
GO

INSERT INTO [dbo].[AlienValue] ([ID], [EurekaFieldID], [FormatID], [Value], [EurekaValueID], [CreatedBy], [LastModifiedBy], [CreatedDate], [LastModifiedDate], [Description])
VALUES (642, 352, 3, N'13', 642, NULL, NULL, NULL, NULL, N'alien value 3')
GO

INSERT INTO [dbo].[AlienValue] ([ID], [EurekaFieldID], [FormatID], [Value], [EurekaValueID], [CreatedBy], [LastModifiedBy], [CreatedDate], [LastModifiedDate], [Description])
VALUES (643, 352, 3, N'14', 643, NULL, NULL, NULL, NULL, N'alien value 4')
GO

SET IDENTITY_INSERT [dbo].[AlienValue] OFF
GO







