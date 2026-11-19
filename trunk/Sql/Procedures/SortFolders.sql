IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[SortFolders]') AND type IN (N'P', N'PC'))
DROP PROCEDURE [dbo].[SortFolders]
GO


CREATE PROCEDURE [dbo].[SortFolders]
AS
BEGIN

;WITH RowIndexes AS (
	select ID, [Name], row_number() over(order by [Name]) AS RowIndex from  Folder
)
UPDATE Folder SET [Rank]=RowIndexes.RowIndex FROM Folder JOIN RowIndexes ON RowIndexes.ID = Folder.ID;

END

GO

