
drop TABLE [dbo].[AuditLog]
go

drop table UploadLogStrokeData
go


IF EXISTS (SELECT * FROM dbo.sysobjects WHERE id = OBJECT_ID(N'[FieldValueDefinition]') AND OBJECTPROPERTY(id, N'IsUserTable') = 1)
	drop table FieldValueDefinition
go

IF EXISTS (SELECT * FROM dbo.sysobjects WHERE id = OBJECT_ID(N'[FieldValueDefinitionMap]') AND OBJECTPROPERTY(id, N'IsUserTable') = 1)
	drop table FieldValueDefinitionMap
go

drop table BusinessRuleCheckResults 
go

drop TABLE [dbo].[StrokeFieldValue] 
go

drop TABLE [dbo].[StrokeData]
go

IF EXISTS (SELECT * FROM dbo.sysobjects WHERE id = OBJECT_ID(N'[FieldDefinition]') AND OBJECTPROPERTY(id, N'IsUserTable') = 1)
	drop table FieldDefinition
go

IF EXISTS (SELECT * FROM dbo.sysobjects WHERE id = OBJECT_ID(N'[FieldDefinitionMap]') AND OBJECTPROPERTY(id, N'IsUserTable') = 1)
	drop table FieldDefinitionMap
go

-----------

drop table AlienValue
go

drop table EurekaValue
go




drop TABLE [dbo].[UploadLog]
go



drop table AlienField
go

drop table EurekaField
go

drop table Facility
go



drop table Format
go

drop table VersionGuid
go








