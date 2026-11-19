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





