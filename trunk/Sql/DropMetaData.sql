
-- drop data

delete from [dbo].[AuditLog]
go

delete from UploadLogStrokeData
go

delete from [dbo].[StrokeFieldValue] 
go

delete from [dbo].[StrokeData]
go

delete from [dbo].[UploadLog]
go




-----------

-- drop meta data

delete from AlienValue
go

delete from EurekaValue
go



delete from AlienField
go

delete from EurekaField
go

delete from Facility
go


-- remove all alien formats
delete from Format where IsNative = 0
go









