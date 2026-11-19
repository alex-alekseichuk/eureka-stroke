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

