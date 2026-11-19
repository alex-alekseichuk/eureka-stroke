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





