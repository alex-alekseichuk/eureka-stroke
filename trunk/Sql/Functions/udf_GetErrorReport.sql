IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[udf_GetErrorReport]') AND type in (N'FN', N'IF', N'TF', N'FS', N'FT'))
	DROP FUNCTION [dbo].[udf_GetErrorReport]
GO


/****** Object:  UserDefinedFunction [dbo].[udf_GetErrorReport]    Script Date: 04/17/2010 22:50:02 ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO



CREATE FUNCTION [dbo].[udf_GetErrorReport] (
@uploadLogID int = null
)
RETURNS TABLE
AS
RETURN (
    select
		fac.[Name] as FacilityName, f.[Name] as FormatName,
		u.OriginalFilename as [filename], u.StartedDate as UploadDate,
		isnull(d.PatientID,'N/A') as PatientID, 
		isnull(d.AdmissionDate,a.ActionDate) as AdmissionDate,   
        a.RowNumber, 
        a.[Action],

        case a.[Action]
            when 0 then 'Create'
            when 1 then 'Update'
            when 2 then 'Delete'
            else 'Unknown'
        end as ActionName

        ,a.ActionDate,
        a.ActionUser, a.ErrorMessage, a.FieldName,
        a.OldValue,
        a.NextValue, a.[Status],
        case a.[Status]
            when 2 then 'Error'
            else 'Unknown'
        end as StatusName

    from AuditLog a
	inner join UploadLog u
	on a.UploadLogID = u.ID
	inner join Facility fac
	on u.FacilityID = fac.ID
	inner join [Format] f
	on u.FormatID = f.ID
	left outer join StrokeFieldValue v
	on a.FieldValueID = v.ID
	left outer join StrokeData d
	on v.StrokeDataID = d.ID
  
    where a.[Status]<>0
        and a.ItemType=0
        and (a.UploadLogID = @uploadLogID or 
        @uploadLogID is null or 
        @uploadLogID = 0)
)


GO


