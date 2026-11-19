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
