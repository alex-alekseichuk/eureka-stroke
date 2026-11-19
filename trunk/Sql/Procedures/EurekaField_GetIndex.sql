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
