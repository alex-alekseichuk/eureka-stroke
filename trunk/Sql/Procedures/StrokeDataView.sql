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
