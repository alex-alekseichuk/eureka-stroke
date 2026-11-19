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
