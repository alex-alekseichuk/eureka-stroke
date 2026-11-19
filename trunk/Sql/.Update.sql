GO 
--D:\Projects\med\EurekaStroke\trunk\Sql\Views\StrokeUser.sql 
PRINT 'Processing file: D:\Projects\med\EurekaStroke\trunk\Sql\Views\StrokeUser.sql...'  
  
GO  
  
IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[StrokeUser]') AND type IN (N'V'))
    DROP VIEW [dbo].[StrokeUser]
GO


CREATE VIEW StrokeUser
AS
select 1 as ID, 'Tester 1' as UserName
union all
select 2 as ID, 'Tester 2' as UserName
union all
select 3 as ID, 'Tester 3' as UserName
union all
select 4 as ID, 'Tester 4' as UserName
union all
select 5 as ID, 'Tester 5' as UserName;

GO

  
GO  
 
--D:\Projects\med\EurekaStroke\trunk\Sql\Functions\fnGetIDasTable.sql 
PRINT 'Processing file: D:\Projects\med\EurekaStroke\trunk\Sql\Functions\fnGetIDasTable.sql...'  
  
GO  
  
/*
DECLARE @s as varchar(max);
SET @s = '2,5,10';
select id from dbo.fnGetIDasTable(@s, ',');
*/

IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[fnGetIDasTable]') AND type in (N'FN', N'IF', N'TF', N'FS', N'FT'))
    DROP FUNCTION [dbo].[fnGetIDasTable]
GO

set ANSI_NULLS ON
set QUOTED_IDENTIFIER ON
GO

CREATE FUNCTION [dbo].[fnGetIDasTable] (
    @ids varchar(8000), 
    @separator varchar(20)
) RETURNS @retTable TABLE(ID int)
BEGIN
    DECLARE	@firstComma int
    IF (@ids IS NOT NULL)
    BEGIN
	    WHILE @ids != ''
	    BEGIN
		    SET @firstComma = CHARINDEX(@separator, @ids)
		    IF @firstComma = 0
		    BEGIN
			    INSERT INTO @retTable 
                    VALUES (@ids)
			    RETURN
		    END
		    INSERT INTO @retTable 
                VALUES (CONVERT(int, SUBSTRING(@ids, 1, @firstComma - 1)))
		    SET @ids = SUBSTRING(@ids, @firstComma + 1, LEN(@ids))
	    END
    END
    RETURN
END
  
GO  
 
--D:\Projects\med\EurekaStroke\trunk\Sql\Functions\udf_GetChangesReport.sql 
PRINT 'Processing file: D:\Projects\med\EurekaStroke\trunk\Sql\Functions\udf_GetChangesReport.sql...'  
  
GO  
  
IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[udf_GetChangesReport]') AND type in (N'FN', N'IF', N'TF', N'FS', N'FT'))
DROP FUNCTION [dbo].[udf_GetChangesReport]
GO

SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO


CREATE FUNCTION [dbo].[udf_GetChangesReport] (
@uploadLogID int = null
)
RETURNS TABLE
AS
RETURN (
    select 
	fac.[Name] as FacilityName, f.[Name] as FormatName,
	u.OriginalFilename as [filename], u.StartedDate as UploadDate,
	d.PatientID, d.AdmissionDate,
	a.RowNumber, a.[Action], 
        case a.[Action]
            when 0 then 'Create'
            when 1 then 'Update'
            when 2 then 'Delete'
            else 'Unknown'
        end as ActionName,
        a.ActionDate,
        a.ActionUser, a.ErrorMessage, a.FieldName,
        a.OldValue,
        a.NextValue, a.[Status],

        case a.[Status]
            when 2 then 'Error'
            when 0 then 'Success'
            else 'Unknown'
        end as StatusName

    from AuditLog a
    inner join UploadLog u
    on a.UploadLogID = u.ID
    inner join Facility fac
    on u.FacilityID = fac.ID
    inner join [Format] f
    on u.FormatID = f.ID

    inner join StrokeFieldValue v
    on a.FieldValueID = v.ID
    inner join StrokeData d
    on v.StrokeDataID = d.ID
  
    where a.[Status]=0
        and a.[Action]=1
        and a.ItemType=0
        and (a.UploadLogID = @uploadLogID or 
        @uploadLogID is null or @uploadLogID = 0)
)


GO


  
GO  
 
--D:\Projects\med\EurekaStroke\trunk\Sql\Functions\udf_GetDataDictionary.sql 
PRINT 'Processing file: D:\Projects\med\EurekaStroke\trunk\Sql\Functions\udf_GetDataDictionary.sql...'  
  
GO  
  
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
  
GO  
 
--D:\Projects\med\EurekaStroke\trunk\Sql\Functions\udf_GetErrorReport.sql 
PRINT 'Processing file: D:\Projects\med\EurekaStroke\trunk\Sql\Functions\udf_GetErrorReport.sql...'  
  
GO  
  
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


  
GO  
 
--D:\Projects\med\EurekaStroke\trunk\Sql\Functions\_dropObsoleteFunctions.sql 
PRINT 'Processing file: D:\Projects\med\EurekaStroke\trunk\Sql\Functions\_dropObsoleteFunctions.sql...'  
  
GO  
  
IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[fn_ObsoleteFunction]') AND type in (N'FN', N'IF', N'TF', N'FS', N'FT'))
DROP FUNCTION [dbo].[fn_ObsoleteFunction]
GO

  
GO  
 
--D:\Projects\med\EurekaStroke\trunk\Sql\Procedures\EurekaField_GetIndex.sql 
PRINT 'Processing file: D:\Projects\med\EurekaStroke\trunk\Sql\Procedures\EurekaField_GetIndex.sql...'  
  
GO  
  
IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[EurekaField_GetIndex]') AND type in (N'P', N'PC'))
DROP PROCEDURE [dbo].[EurekaField_GetIndex]
GO

CREATE PROCEDURE [dbo].[EurekaField_GetIndex] 
	@ID int
AS
	;WITH RowIndexes AS (
		select ID, row_number() over(order by [Name]) AS RowIndex from  EurekaField
	)
	SELECT CAST(RowIndex AS int) FROM RowIndexes WHERE ID = @ID;
GO
  
GO  
 
--D:\Projects\med\EurekaStroke\trunk\Sql\Procedures\Pivot.sql 
PRINT 'Processing file: D:\Projects\med\EurekaStroke\trunk\Sql\Procedures\Pivot.sql...'  
  
GO  
  
IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[PivotStrokeData]') AND type IN (N'P', N'PC'))
DROP PROCEDURE [dbo].[PivotStrokeData]
GO


CREATE PROCEDURE [dbo].[PivotStrokeData]
AS
BEGIN

DECLARE @listCol VARCHAR(max)
DECLARE @query VARCHAR(max)
SELECT  @listCol = STUFF(( SELECT DISTINCT
                                '],[' + ltrim(f.Name)
                        FROM    StrokeDataView v
                        inner join EurekaField f
                        on v.FieldID = f.ID
                        
                        ORDER BY '],[' + ltrim(f.Name)
                        FOR XML PATH('')
                                    ), 1, 2, '') + ']'
                                    
print @listCol
 
SET @query =
'
With src as (
	SELECT d.ID, 
      --d.PatientID as PatID, d.AdmissionDate as AdmDate, 
			v.MultiValue as FieldValue, f.Name as FieldName
       FROM    
					StrokeData d
					inner join StrokeDataView v
					on d.ID = v.StrokeDataID
                    inner join EurekaField f
                    on v.FieldID = f.ID
		    --where v.Status = 0

)
SELECT * FROM src
PIVOT (Min(FieldValue) FOR FieldName
IN ('+@listCol+')) AS pvt'

print @query
 
EXECUTE (@query)

END

GO

  
GO  
 
--D:\Projects\med\EurekaStroke\trunk\Sql\Procedures\PivotFacilitiesData.sql 
PRINT 'Processing file: D:\Projects\med\EurekaStroke\trunk\Sql\Procedures\PivotFacilitiesData.sql...'  
  
GO  
  
IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[PivotFacilitiesData]') AND type IN (N'P', N'PC'))
DROP PROCEDURE [dbo].[PivotFacilitiesData]
GO

CREATE PROCEDURE [dbo].[PivotFacilitiesData]
	@facilitiesIDs varchar(1024),
	@maxrows int = null
AS
BEGIN
DECLARE @listCol VARCHAR(max)
DECLARE @query VARCHAR(max)
SELECT  @listCol = STUFF(( SELECT DISTINCT
                                '],[' + ltrim(f.Name)
                        FROM    StrokeDataView v
                        inner join EurekaField f on v.FieldID = f.ID
						inner join UploadLogStrokeData ulsd on v.StrokeDataID = ulsd.StrokeDataID
						inner join UploadLog ul on ul.ID = ulsd.UploadLogID
                        inner join dbo.fnGetIDasTable(@facilitiesIDs, ',') AS fs ON fs.[id]=ul.FacilityID
                        ORDER BY '],[' + ltrim(f.Name)
                        FOR XML PATH('')
                                    ), 1, 2, '') + ']'
                                    
--print @listCol

IF @maxrows IS NULL
	SET @query = 'SELECT';
ELSE
	SET @query = 'SELECT TOP (' + cast(@maxrows as varchar(32)) + ')';
SET @query = @query + ' * FROM
      (
      SELECT ulsd.StrokeDataID as ID, 
			v.MultiValue as FieldValue, f.Name as FieldName
       FROM    
				UploadLog ul
				inner join UploadLogStrokeData ulsd on ul.ID = ulsd.UploadLogID
				inner join StrokeDataView v on v.StrokeDataID = ulsd.StrokeDataID 
				inner join EurekaField f on v.FieldID = f.ID
		    where ul.FacilityID IN (' + @facilitiesIDs + ')
        ) src
PIVOT (Min(FieldValue) FOR FieldName
IN ('+@listCol+')) AS pvt'

--print @query
 
EXECUTE (@query)

END

GO





  
GO  
 
--D:\Projects\med\EurekaStroke\trunk\Sql\Procedures\PivotFacilityData.sql 
PRINT 'Processing file: D:\Projects\med\EurekaStroke\trunk\Sql\Procedures\PivotFacilityData.sql...'  
  
GO  
  
IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[PivotFacilityData]') AND type IN (N'P', N'PC'))
DROP PROCEDURE [dbo].[PivotFacilityData]
GO

CREATE PROCEDURE [dbo].[PivotFacilityData]
	@facilityID int
AS
BEGIN

DECLARE @listCol VARCHAR(max)
DECLARE @query VARCHAR(max)
SELECT  @listCol = STUFF(( SELECT DISTINCT
                                '],[' + ltrim(f.Name)
                        FROM    StrokeDataView v
                        inner join EurekaField f on v.FieldID = f.ID
						inner join UploadLogStrokeData ulsd on v.StrokeDataID = ulsd.StrokeDataID
						inner join UploadLog ul on ul.ID = ulsd.UploadLogID
						WHERE
							ul.FacilityID = @facilityID
							or @facilityID is null or @facilityID = 0
                        ORDER BY '],[' + ltrim(f.Name)
                        FOR XML PATH('')
                                    ), 1, 2, '') + ']'
                                    
--print @listCol
 
SET @query =
'SELECT * FROM
      (
      SELECT ulsd.StrokeDataID as ID, 
			v.MultiValue as FieldValue, f.Name as FieldName
       FROM    
				UploadLog ul
				inner join UploadLogStrokeData ulsd on ul.ID = ulsd.UploadLogID
				inner join StrokeDataView v on v.StrokeDataID = ulsd.StrokeDataID 
				inner join EurekaField f on v.FieldID = f.ID
		    where ul.FacilityID = ' + cast(@facilityID as varchar(64)) + '
        ) src
PIVOT (Min(FieldValue) FOR FieldName
IN ('+@listCol+')) AS pvt'

--print @query
 
EXECUTE (@query)

END

GO





  
GO  
 
--D:\Projects\med\EurekaStroke\trunk\Sql\Procedures\PivotReportData.sql 
PRINT 'Processing file: D:\Projects\med\EurekaStroke\trunk\Sql\Procedures\PivotReportData.sql...'  
  
GO  
  
IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[GetPivotReportData]') AND type IN (N'P', N'PC'))
DROP PROCEDURE [dbo].[GetPivotReportData]
GO

CREATE PROCEDURE [dbo].[GetPivotReportData]
	@userID int,
	@reportingSource varchar(128),
	@maxrows int = null
AS
BEGIN
DECLARE @listCol VARCHAR(max)
DECLARE @query VARCHAR(max)

IF @reportingSource <> ''
BEGIN
	IF NOT EXISTS(SELECT * FROM UserReportingSource WHERE UserID=@userID AND ReportingSource=@reportingSource)
		RETURN;
	SELECT  @listCol = STUFF(( SELECT DISTINCT
                                '],[' + ltrim(f.Name)
                        FROM    StrokeDataView v
                        inner join EurekaField f on v.FieldID = f.ID
						inner join StrokeData sd on v.StrokeDataID = sd.ID AND sd.ReportingSource=@reportingSource
                        ORDER BY '],[' + ltrim(f.Name)
                        FOR XML PATH('')
                                    ), 1, 2, '') + ']'
END ELSE BEGIN
	SELECT  @listCol = STUFF(( SELECT DISTINCT
                                '],[' + t.Name
                        FROM (        
							SELECT ltrim(f.Name) as Name
		                    FROM    StrokeDataView v
			                inner join EurekaField f on v.FieldID = f.ID
				            inner join StrokeData sd on v.StrokeDataID = sd.ID
				            inner join UserReportingSource AS urs ON urs.ReportingSource=sd.ReportingSource AND urs.UserID=@userID
							UNION
							SELECT ltrim(f.Name) as Name
		                    FROM    StrokeDataView v
			                inner join EurekaField f on v.FieldID = f.ID
							inner join UploadLogStrokeData ulsd on v.StrokeDataID = ulsd.StrokeDataID
							inner join UploadLog ul on ul.ID = ulsd.UploadLogID
							inner join UserFacility AS ufs ON ufs.FacilityID=ul.FacilityID AND ufs.UserID=@userID
                        ) t
                        ORDER BY '],[' + t.Name
                        FOR XML PATH('')
                                    ), 1, 2, '') + ']'
END

--print @listCol

IF @maxrows IS NULL
	SET @query = 'SELECT';
ELSE
	SET @query = 'SELECT TOP (' + cast(@maxrows as varchar(32)) + ')';

IF @reportingSource <> ''
	SET @query = @query + ' * FROM
		  (
		  SELECT v.StrokeDataID as ID,
				v.MultiValue as FieldValue, f.Name as FieldName
		   FROM
					StrokeDataView v
					inner join EurekaField f on v.FieldID = f.ID
					inner join StrokeData sd on v.StrokeDataID = sd.ID AND sd.ReportingSource=' + @reportingSource + '
			) src
	PIVOT (Min(FieldValue) FOR FieldName
	IN ('+@listCol+')) AS pvt'
ELSE
	SET @query = @query + ' * FROM
		  (
		  SELECT v.StrokeDataID as ID, 
				v.MultiValue as FieldValue, f.Name as FieldName
		   FROM    
					StrokeDataView v
					inner join EurekaField f on v.FieldID = f.ID
					inner join (
						SELECT sd.ID AS StrokeDataID
						FROM StrokeData sd 
							inner join UserReportingSource AS urs ON urs.ReportingSource=sd.ReportingSource AND urs.UserID=' + cast(@userID as varchar(32)) + '
						UNION
						SELECT sd.ID AS StrokeDataID
						FROM UploadLog ul
							inner join UploadLogStrokeData ulsd on ul.ID = ulsd.UploadLogID
							inner join StrokeData sd on ulsd.StrokeDataID = sd.ID
							inner join UserFacility AS ufs ON ufs.FacilityID=ul.FacilityID AND ufs.UserID=' + cast(@userID as varchar(32)) + '
					) t ON t.StrokeDataID=v.StrokeDataID
					
			) src
	PIVOT (Min(FieldValue) FOR FieldName
	IN ('+@listCol+')) AS pvt'

--print @query

EXECUTE (@query)

END

GO
  
GO  
 
--D:\Projects\med\EurekaStroke\trunk\Sql\Procedures\StrokeDataView.sql 
PRINT 'Processing file: D:\Projects\med\EurekaStroke\trunk\Sql\Procedures\StrokeDataView.sql...'  
  
GO  
  
/****** Object:  View [dbo].[StrokeDataView]    Script Date: 04/22/2010 00:59:51 ******/
IF  EXISTS (SELECT * FROM sys.views WHERE object_id = OBJECT_ID(N'[dbo].[StrokeDataView]'))
	DROP VIEW [dbo].[StrokeDataView]
GO


/****** Object:  View [dbo].[StrokeDataView]    Script Date: 04/22/2010 00:59:51 ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

	CREATE VIEW [dbo].[StrokeDataView]
	AS
	SELECT     StrokeDataID, FieldID, 
	STUFF((SELECT     ':' + v1.StringValue AS [text()]
        FROM         StrokeFieldValue v1
        WHERE     v1.StrokeDataID = v2.StrokeDataID AND v1.FieldID = v2.FieldID 
	AND v1.[Status] = 0
        ORDER BY v1.StringValue FOR xml PATH('')), 1, 1, '') AS MultiValue
	FROM         StrokeFieldValue v2
	GROUP BY StrokeDataID, FieldID

GO
  
GO  
 
--D:\Projects\med\EurekaStroke\trunk\Sql\Procedures\_dropObsoleteSP.sql 
PRINT 'Processing file: D:\Projects\med\EurekaStroke\trunk\Sql\Procedures\_dropObsoleteSP.sql...'  
  
GO  
  
IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[ObsoleteProcedure]') AND type in (N'P', N'PC'))
DROP PROCEDURE [dbo].[ObsoleteProcedure]
GO

  
GO  
 
