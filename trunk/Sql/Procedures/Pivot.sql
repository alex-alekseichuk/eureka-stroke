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

