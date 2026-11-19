/****** Object:  UserDefinedFunction [dbo].[udf_GetDataDictionary]    Script Date: 04/15/2010 00:39:47 ******/
IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[udf_GetDataDictionary]') AND type in (N'FN', N'IF', N'TF', N'FS', N'FT'))
	DROP FUNCTION [dbo].[udf_GetDataDictionary]
GO

/****** Object:  UserDefinedFunction [dbo].[udf_GetDataDictionary]    Script Date: 04/15/2010 00:39:47 ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE FUNCTION [dbo].[udf_GetDataDictionary] (
)
RETURNS TABLE
AS
RETURN (
    select
	e.ID as EurekaFieldID,
	e.[Name] as EurekaFieldName,
	e.[Description] as EurekaFieldDescription,
    e.FieldType,
    case e.FieldType
            when 0 then 'String'
            when 1 then 'Integer'
            when 2 then 'Boolean'
            when 3 then 'Decimal'
            when 4 then 'DateTime'
            when 5 then 'SingleSelect'
            when 5 then 'MultipleSelect'
            else 'Unknown'
    end as FieldTypeName,
    
    ev.[Value] as [EurekaValue],
    ev.[Description] as [EurekaValueDescription]

    from EurekaField e
        
    left outer join EurekaValue ev
    on e.ID = ev.EurekaFieldID
    
		
)

GO
