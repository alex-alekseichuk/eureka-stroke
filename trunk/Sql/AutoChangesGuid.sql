--=======================================================================================
--  BEGIN INITIALIZE AutoChangesGuid.sql
--=======================================================================================

DECLARE @dbName varchar(MAX); SELECT @dbName = DB_NAME();
PRINT 'Applying AutoChangesGuid.sql against [' + @dbName + '] database';
IF (@dbName = 'master') RAISERROR('Error: An attempt to apply AutoChangesGuid.sql against [master] database', 20, 1) with log;
GO

IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[VersionGuid]') AND type in (N'U'))
	CREATE TABLE VersionGuid(
		[Guid] uniqueidentifier NOT NULL, CONSTRAINT PK_VersionGuid PRIMARY KEY NONCLUSTERED([Guid]),
		[dateMarked] datetime NOT NULL,
		[dateApplied] datetime NOT NULL,
		-- timestamp NOT NULL,
		[description] varchar(255) NOT NULL,
		 CONSTRAINT [UQ__VersionGuid_dateMarked] UNIQUE CLUSTERED 
		([dateMarked] ASC)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY],
		 CONSTRAINT [UQ__VersionGuid_description] UNIQUE NONCLUSTERED 
		([dateMarked] ASC) WITH( STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
	) ON [PRIMARY]
GO

IF (OBJECT_ID('tempdb..#VersionGuidTmp') IS NOT NULL)
	DROP TABLE #VersionGuidTmp;
CREATE TABLE #VersionGuidTmp(
	[Guid] uniqueidentifier NOT NULL,
	[dateMarked] datetime NOT NULL,
	[description] varchar(255) NOT NULL,
	[isError] bit NOT NULL DEFAULT 0,
) ON [PRIMARY]
GO

IF EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[sysVersionGuidPrepare]') AND type in (N'P', N'PC'))
DROP PROCEDURE [sysVersionGuidPrepare]
GO
CREATE PROCEDURE [sysVersionGuidPrepare] 
    @versionGuid uniqueidentifier
	,@dateMarked datetime
	,@description varchar(255)
AS
BEGIN
	DECLARE @options int, @oldNoCountValue bit; 
	SELECT @options = @@OPTIONS; IF ( (512 & @options) = 512) SET @oldNoCountValue = 1 ELSE SET NOCOUNT ON;

	IF EXISTS (SELECT * FROM #VersionGuidTmp)
		BEGIN
			UPDATE #VersionGuidTmp
			SET [isError] = 1;
			RAISERROR('Block is not apllied. See previous error.', 16, 1);
			RETURN -1;
		END

	INSERT INTO #VersionGuidTmp ([Guid], [dateMarked], [description],[isError]) 
		VALUES (@versionGuid, @dateMarked, @description, 0);

	IF @oldNoCountValue = 0 SET NOCOUNT OFF;
END
GO

IF EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[sysVersionGuidCommit]') AND type in (N'P', N'PC'))
DROP PROCEDURE [sysVersionGuidCommit]
GO
CREATE PROCEDURE [sysVersionGuidCommit]
AS
BEGIN
	DECLARE @options int, @oldNoCountValue bit; 
	SELECT @options = @@OPTIONS; IF ( (512 & @options) = 512) SET @oldNoCountValue = 1 ELSE SET NOCOUNT ON;

    DECLARE @versionGuid uniqueidentifier
			,@dateMarked datetime
			,@description varchar(255)
			,@isError bit;

	SELECT @versionGuid = [Guid]
		  ,@dateMarked = [dateMarked]
		  ,@description = [description]
		  ,@isError = [isError]
	FROM #VersionGuidTmp;

	IF @isError = 1
		RAISERROR('Commit is not allowed. See previous error.', 16, 1);

	DELETE #VersionGuidTmp;

    PRINT 'Actual auto-change [' + CAST(@versionGuid AS varchar(36)) + '] is apllied.';
	IF @oldNoCountValue = 0 SET NOCOUNT OFF;
END
GO

IF EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[sysVersionGuidLog]') AND type in (N'P', N'PC'))
DROP PROCEDURE [dbo].[sysVersionGuidLog]
GO
CREATE PROCEDURE [dbo].[sysVersionGuidLog] 
AS
BEGIN
    DECLARE @invalidReturn int; SET @invalidReturn = -1;
    DECLARE @dateStyle int; SET @dateStyle = 121;

    DECLARE @versionGuid uniqueidentifier
			,@dateMarked datetime
			,@description varchar(255)
			,@isError bit
			,@dateAlreadyApplied datetime
			,@dateMarkedAlreadyApplied datetime
			,@descriptionAlreadyApplied varchar(255);

	SELECT @versionGuid = [Guid]
		  ,@dateMarked = [dateMarked]
		  ,@description = [description]
		  ,@isError = [isError]
	FROM #VersionGuidTmp;

	IF @isError = 1
		BEGIN
	        RAISERROR( 'Block not applied! See previous error...', 16, 1);
			RETURN @invalidReturn;
		END

    DECLARE @options INT; 
    SELECT @options = @@OPTIONS;
    -- 16384 XACT_ABORT 'Rolls back a transaction if a Transact-SQL statement raises a run-time error.'
    IF (NOT (16384 & @options) = 16384)
    BEGIN
        PRINT '- AutoChangesGuid error 2: VersionGuid [' + CAST(@versionGuid AS varchar(36)) +
            '] cannot be applied since XACT_ABORT parameter is set OFF. Cannot continue the transaction.';
        RAISERROR('- AutoChangesGuid error 2: VersionGuid cannot be applied since XACT_ABORT parameter is set OFF. Cannot continue the transaction.', 16, 1);
        RETURN @invalidReturn;
    END

	IF @versionGuid IS NULL OR @versionGuid = '00000000-0000-0000-0000-000000000000'
		BEGIN
			PRINT '- AutoChangesGuid error 4: @versionGuid IS NULL. Cannot continue the transaction.';
			RAISERROR('- AutoChangesGuid error 4: @versionGuid IS NULL. Cannot continue the transaction.', 16, 1);
			RETURN @invalidReturn;
		END
	IF @dateMarked IS NULL OR @dateMarked = 0
		BEGIN
			PRINT '- AutoChangesGuid error 5: @dateMarked IS NULL. Cannot continue the transaction.';
			RAISERROR('- AutoChangesGuid error 5: @dateMarked IS NULL. Cannot continue the transaction.', 16, 1);
			RETURN @invalidReturn;
		END
	IF @description IS NULL OR @description = ''
		BEGIN
			PRINT '- AutoChangesGuid error 6: @description IS NULL. Cannot continue the transaction.';
			RAISERROR('- AutoChangesGuid error 6: @description IS NULL. Cannot continue the transaction.', 16, 1);
			RETURN @invalidReturn;
		END

    IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[VersionGuid]') AND type in (N'U'))
				RAISERROR('- AutoChangesGuid error - table [VersionGuid] not found', 16, 1);
    ELSE
		SELECT @dateAlreadyApplied = [dateApplied]
				,@dateMarkedAlreadyApplied = [dateMarked]
				,@descriptionAlreadyApplied = [description]
		FROM [VersionGuid]
		WHERE [Guid] = @versionGuid;

	IF (@dateAlreadyApplied IS NOT NULL)
		BEGIN
			PRINT '- VersionGuid [' + CAST(@versionGuid AS varchar(36)) + '] is already applied at ' 
				+ CONVERT(varchar, @dateAlreadyApplied, @dateStyle);
			IF (@descriptionAlreadyApplied != @description)
			BEGIN
				PRINT '- AutoChangesGuid error 3: VersionGuid [' + CAST(@versionGuid AS varchar(36)) + '] ['
					 + CONVERT(varchar(30),@dateMarked,121)	+ '] description ambiguity! ';
				PRINT '         applying: "' + @description + '"';
				PRINT '  already applied: "' + @descriptionAlreadyApplied + '"';
				RAISERROR('- AutoChangesGuid error 3', 16, 1);
			END
			IF (@dateMarkedAlreadyApplied != @dateMarked)
			BEGIN
				PRINT '- AutoChangesGuid error 2: VersionGuid [' + CAST(@versionGuid AS varchar(36)) + '] ['
					+ @description + '] dateMarked ambiguity! ';
				PRINT '         applying: "' + CONVERT(varchar, @dateMarked, @dateStyle) + '"';
				PRINT '  already applied: "' + CONVERT(varchar, @dateMarkedAlreadyApplied, @dateStyle) + '"';
				RAISERROR('- AutoChangesGuid error 2', 16, 1);
			END
			RETURN @invalidReturn;
		END
	ELSE
		BEGIN
			DECLARE @conflictingVersionGuid uniqueidentifier;

			SELECT TOP (1) @conflictingVersionGuid = [Guid]
			FROM [VersionGuid]
			WHERE @dateMarked = [dateMarked];

			IF @conflictingVersionGuid IS NOT NULL
			BEGIN
				PRINT '- AutoChangesGuid error 1: VersionGuid conflict: existing VersionGuid [' 
						+ CAST(@conflictingVersionGuid AS varchar(36)) + '] and applying VersionGuid [' 
						+ CAST(@versionGuid AS varchar(36)) + '] have the same dateMarked';
			   PRINT 'Cannot apply duplicating VersionGuid [' + CAST(@versionGuid AS varchar(36)) + '] [' + CONVERT(varchar(30),@dateMarked,121)
						+ '] [' + @description + ']';
				RAISERROR('- AutoChangesGuid error 1', 16, 1);
				RETURN @invalidReturn;
			END

			SELECT TOP (1) @conflictingVersionGuid = [Guid]
			FROM [VersionGuid]
			WHERE @description = [description];

			IF @conflictingVersionGuid IS NOT NULL
			BEGIN
				PRINT '- AutoChangesGuid error 1: VersionGuid conflict: existing VersionGuid [' 
						+ CAST(@conflictingVersionGuid AS varchar(36)) + '] and applying VersionGuid [' 
						+ CAST(@versionGuid AS varchar(36)) + '] have the same description';
			   PRINT 'Cannot apply duplicating VersionGuid [' + CAST(@versionGuid AS varchar(36)) + '] [' + CONVERT(varchar(30),@dateMarked,121)
						+ '] [' + @description + ']';
				RAISERROR('- AutoChangesGuid error 1', 16, 1);
				RETURN @invalidReturn;
			END

			SELECT @dateMarkedAlreadyApplied = MAX([dateMarked])
			FROM [VersionGuid];

			IF @dateMarked < @dateMarkedAlreadyApplied
				BEGIN
					PRINT 'AutoChangesGuid warning 1: VersionGuid [' + CAST(@versionGuid AS varchar(36)) + '] dateMarked is less than already applied! ';
					PRINT '         applying: "' + CONVERT(varchar, @dateMarked, @dateStyle) + '"';
					PRINT '  already applied: "' + CONVERT(varchar, @dateMarkedAlreadyApplied, @dateStyle) + '"';
				END

			DECLARE @dateApplied datetime; SET @dateApplied = GETDATE();

			PRINT '+ Applying VersionGuid [' + CAST(@versionGuid AS varchar(36)) + '] [' + CONVERT(varchar(30),@dateMarked,121)
					+ '] [' + @description + '] at '+ CONVERT(varchar, @dateApplied, @dateStyle);
 
            DECLARE @oldNoCountValue bit;
            SELECT @options = @@OPTIONS;
            -- 512 NOCOUNT 'Turns off the message returned at the end of each statement that states how many rows were affected.'
            IF ( (512 & @options) = 512) SET @oldNoCountValue = 1 ELSE SET NOCOUNT ON;
            INSERT INTO [VersionGuid] ([Guid], [dateMarked], [dateApplied], [description]) 
                VALUES (@versionGuid, @dateMarked, @dateApplied, @description);
            IF @oldNoCountValue = 0 SET NOCOUNT OFF;
		    PRINT 'Actual auto-change [' + CAST(@versionGuid AS varchar(36)) + '] is being applied...';
        END
    RETURN 0;
END
GO

/*********************************************************************************************
 ** Insert procedures/functions used in AutoChangesGuid (or integrate them to initial backup)  ***
 *********************************************************************************************/
--IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[TestFunction]') AND type in (N'FN', N'IF', N'TF', N'FS', N'FT'))
--DROP FUNCTION [dbo].[TestFunction]
--GO
--
--CREATE FUNCTION [dbo].[TestFunction](@param1 as int)
--RETURNS int
--AS
--BEGIN
--   RETURN @param1 * 2;
--END
--GO

-- GENERAL NOTES
-- An auto update script block should always start with "--(" and end with "--)"
-- Date format is EXACTLY 'dd MMM yyyy HH:mm:ss'
-- Do not forget to add linebreak after your script block both in the middle and in the end of this file
-- DO NOT SET SET XACT_ABORT OFF for proper script blocks work
SET XACT_ABORT ON
GO

--(
-- Initialization, don't edit and copy this block
DECLARE @ret int;
EXEC @ret = sysVersionGuidPrepare @versionGuid = '11111111-1111-1111-1111-111111111111',
	@dateMarked  = '01 Jan 2001 00:00:00', @description = 'alexa: VersionGuid autochange system initialization';
GO
DECLARE @ret int, @options int, @oldNoCountValue bit; 
BEGIN TRANSACTION;
EXEC @ret = sysVersionGuidLog;
IF (@ret = -1) 	BEGIN ROLLBACK TRANSACTION; SELECT @options = @@OPTIONS; IF ( (512 & @options) = 512) SET @oldNoCountValue = 1 ELSE SET NOCOUNT ON;	DELETE #VersionGuidTmp; IF @oldNoCountValue = 0 SET NOCOUNT OFF;	END
ELSE
	BEGIN
	 	EXEC sysVersionGuidCommit;
		COMMIT TRANSACTION;
END
GO
--) 11111111-1111-1111-1111-111111111111

--(
-- Select the expression below (not including leading -- ) and execute by press F5. Copy result Guid from Messages windows and 
-- paste it to initialize @versionGuid and @dateMarked of your block. Don't forget change @description
-- PRINT '''' + CAST(NEWID() as varchar(36)) + '''' + CAST(0x0D0A as varchar(2)) + '''' + CONVERT(varchar(20), GETDATE(), 113) + ''''

DECLARE @ret int;
EXEC @ret = sysVersionGuidPrepare @versionGuid = '44444444-4444-4444-4444-444444444444',
                                  @dateMarked  = '01 Jan 2001 00:00:15',
                                  @description = 'alexa: example Guid auto-change script block 4';
GO
DECLARE @ret int, @options int, @oldNoCountValue bit; 
BEGIN TRANSACTION;
EXEC @ret = sysVersionGuidLog; 
IF (@ret = -1) 	BEGIN ROLLBACK TRANSACTION; SELECT @options = @@OPTIONS; IF ( (512 & @options) = 512) SET @oldNoCountValue = 1 ELSE SET NOCOUNT ON;	DELETE #VersionGuidTmp; IF @oldNoCountValue = 0 SET NOCOUNT OFF;	END
ELSE BEGIN
--------------------------------------------------------------------------------------------------------------------------
    -- Insert actual new auto-changes here.
    -- Don''t forget to remove all GO statements from your change script fragment.
    -- Do not turn off XACT_ABORT.
    -- Split to several consecutive versions your long changes, or the changes that require some their parts committed before apllying their further statements.
    IF EXISTS (SELECT name FROM sysobjects WHERE name = 'Test_Versions_Script' AND type = 'U')
        DROP TABLE [dbo].[Test_Versions_Script];
--------------------------------------------------------------------------------------------------------------------------
	EXEC sysVersionGuidCommit;
	COMMIT TRANSACTION;
END
GO
--) 44444444-4444-4444-4444-444444444444


--(
-- Select the expression below (not including leading -- ) and execute by press F5. Copy result Guid from Messages windows and 
-- paste it to initialize @versionGuid and @dateMarked of your block. Don't forget change @description
-- PRINT '''' + CAST(NEWID() as varchar(36)) + '''' + CAST(0x0D0A as varchar(2)) + '''' + CONVERT(varchar(20), GETDATE(), 113) + ''''

DECLARE @ret int;
EXEC @ret = sysVersionGuidPrepare @versionGuid = 'DB955947-B5C1-4C53-9054-3434AAE49E8A',
                                  @dateMarked  = '01 Sep 2009 16:10:28',
                                  @description = 'alexa: added files from draft.sql';
GO
DECLARE @ret int, @options int, @oldNoCountValue bit; 
BEGIN TRANSACTION;
EXEC @ret = sysVersionGuidLog; 
IF (@ret = -1) 	BEGIN ROLLBACK TRANSACTION; SELECT @options = @@OPTIONS; IF ( (512 & @options) = 512) SET @oldNoCountValue = 1 ELSE SET NOCOUNT ON;	DELETE #VersionGuidTmp; IF @oldNoCountValue = 0 SET NOCOUNT OFF;	END
ELSE BEGIN
-----------------------------------------------------------------------------------------------------------------------------

CREATE TABLE [dbo].[AuditLog] (
  [ID] int IDENTITY(1, 1) NOT NULL,
  [FieldValueID] int NOT NULL,
  [Action] smallint NOT NULL,
  [ActionDate] int NOT NULL,
  [ActionUser] int NOT NULL,
  [OldValue] varchar(max)  NULL,
  [NextValue] varchar(max)  NULL,
  [Status] int NULL,
  CONSTRAINT PK_AuditLog PRIMARY KEY CLUSTERED (ID)
)
ON [PRIMARY];

CREATE TABLE [dbo].[StrokeFieldValue] (
  [ID] int IDENTITY(1, 1) NOT NULL,
  [StrokeDataID] int NOT NULL,
  [FieldID] int NOT NULL,
  [IntegerValue] int NULL,
  [BooleanValue] bit NULL,
  [StringValue] varchar(max)  NULL,
  [DateTimeValue] datetime NULL,
  [DecimalValue] decimal(19,6) NULL,
  [RawValue] varchar(max)  NULL,
  [CreatedDate] datetime NULL,
  [CreatedBy] int NULL,
  [LastModifiedBy] int NULL,
  [LastModifiedDate] datetime NULL,
  [Status] smallint NULL,
  CONSTRAINT PK_StrokeFieldValue PRIMARY KEY CLUSTERED (ID)
)
ON [PRIMARY];


EXEC sp_addextendedproperty 'MS_Description', N'0 - success
1 - failure', 'schema', 'dbo', 'table', 'StrokeFieldValue', 'column', 'Status'


CREATE TABLE [dbo].[StrokeData] (
  [ID] int IDENTITY(1, 1) NOT NULL,
  [UploadLogID] int NULL,
  [PatientID] varchar(256)  NOT NULL,
  [CreatedDate] datetime NULL,
  [CreatedBy] int NULL,
  [LastModifiedDate] datetime NULL,
  [LastModifiedBy] int NULL,
  [Status] smallint NULL,
  CONSTRAINT PK_StrokeData PRIMARY KEY CLUSTERED (ID)
)
ON [PRIMARY];

CREATE TABLE [dbo].[Facility] (
  [ID] int IDENTITY(1, 1) NOT NULL,
  [Name] varchar(max)  NOT NULL,
  [FormatID] int NULL,
  CONSTRAINT PK_Facility PRIMARY KEY CLUSTERED (ID)
)
ON [PRIMARY];

CREATE TABLE [dbo].[UploadLog] (
  [ID] int IDENTITY(1, 1) NOT NULL,
  [Filename] varchar(max)  NOT NULL,
  [Status] smallint NOT NULL,
  [ErrorCount] int NULL,
  [ChangesCount] int NULL,
  [TotalCount] int NULL,
  [StartedDate] datetime NOT NULL,
  [CompletionDate] datetime NULL,
  [StartedBy] int NULL,
  FacilityID int NOT NULL,
  CONSTRAINT PK_UploadLog PRIMARY KEY CLUSTERED (ID)
)
ON [PRIMARY];



CREATE TABLE [dbo].[Format] (
  [ID] int IDENTITY(1, 1) NOT NULL,
  [Name] varchar(max) NOT NULL,
  [IsNative] bit NULL,
  CONSTRAINT PK_Format PRIMARY KEY CLUSTERED (ID)
)
ON [PRIMARY];


--
-- Definition for foreign keys : 
--



ALTER TABLE [dbo].[Facility]
ADD CONSTRAINT [FK_Facility_Format] FOREIGN KEY ([FormatID]) 
  REFERENCES [dbo].[Format] ([ID]) 
  ON UPDATE NO ACTION
  ON DELETE NO ACTION;

ALTER TABLE [dbo].[UploadLog]
ADD CONSTRAINT [FK_UploadLog_FacilityID] FOREIGN KEY ([FacilityID]) 
  REFERENCES [dbo].[Facility] ([ID]) 
  ON UPDATE NO ACTION
  ON DELETE NO ACTION;

ALTER TABLE [dbo].[StrokeData]
ADD CONSTRAINT [FK_StrokeData_UploadLog] FOREIGN KEY ([UploadLogID]) 
  REFERENCES [dbo].[UploadLog] ([ID]) 
  ON UPDATE NO ACTION
  ON DELETE NO ACTION;

ALTER TABLE [dbo].[StrokeFieldValue]
ADD CONSTRAINT [FK_StrokeFieldValue_StrokeData] FOREIGN KEY ([StrokeDataID]) 
  REFERENCES [dbo].[StrokeData] ([ID]) 
  ON UPDATE NO ACTION
  ON DELETE NO ACTION;



ALTER TABLE [dbo].[AuditLog]
ADD CONSTRAINT [FK_AuditLog_StrokeFieldValue] FOREIGN KEY ([FieldValueID]) 
  REFERENCES [dbo].[StrokeFieldValue] ([ID]) 
  ON UPDATE NO ACTION
  ON DELETE NO ACTION;





-----------------------------------------------------------------------------------------------------------------------------------
	EXEC sysVersionGuidCommit;
	COMMIT TRANSACTION;
END
GO
--) DB955947-B5C1-4C53-9054-3434AAE49E8A



--(
-- Select the expression below (not including leading -- ) and execute by press F5. Copy result Guid from Messages windows and 
-- paste it to initialize @versionGuid and @dateMarked of your block. Don't forget change @description
-- PRINT '''' + CAST(NEWID() as varchar(36)) + '''' + CAST(0x0D0A as varchar(2)) + '''' + CONVERT(varchar(20), GETDATE(), 113) + ''''

DECLARE @ret int;
EXEC @ret = sysVersionGuidPrepare @versionGuid = '21B6DB1C-5FA3-4681-A05B-6AE68E26515F',
                                  @dateMarked  = '23 Jan 2010 16:55:31',
                                  @description = 'alexa: made some fields of UploadLog optional';
GO
DECLARE @ret int, @options int, @oldNoCountValue bit; 
BEGIN TRANSACTION;
EXEC @ret = sysVersionGuidLog; 
IF (@ret = -1) 	BEGIN ROLLBACK TRANSACTION; SELECT @options = @@OPTIONS; IF ( (512 & @options) = 512) SET @oldNoCountValue = 1 ELSE SET NOCOUNT ON;	DELETE #VersionGuidTmp; IF @oldNoCountValue = 0 SET NOCOUNT OFF;	END
ELSE BEGIN
--------------------------------------------------------------------------------------------------------------------------
ALTER TABLE UploadLog ALTER COLUMN Status int NULL;
ALTER TABLE UploadLog ALTER COLUMN StartedDate datetime NULL;
ALTER TABLE UploadLog Add FormatID int NULL;
--------------------------------------------------------------------------------------------------------------------------
	EXEC sysVersionGuidCommit;
	COMMIT TRANSACTION;
END
GO
--) '21B6DB1C-5FA3-4681-A05B-6AE68E26515F'

--(
-- Select the expression below (not including leading -- ) and execute by press F5. Copy result Guid from Messages windows and 
-- paste it to initialize @versionGuid and @dateMarked of your block. Don't forget change @description
-- PRINT '''' + CAST(NEWID() as varchar(36)) + '''' + CAST(0x0D0A as varchar(2)) + '''' + CONVERT(varchar(20), GETDATE(), 113) + ''''

DECLARE @ret int;
EXEC @ret = sysVersionGuidPrepare @versionGuid = 'FD41879E-A939-4AAB-BB99-4877BE589C9E',
                                  @dateMarked  = '26 Jan 2010 00:51:58',
                                  @description = 'alexl: AuditLog changes and UploadLog to StrokeData many-to-many';
GO
DECLARE @ret int, @options int, @oldNoCountValue bit; 
BEGIN TRANSACTION;
EXEC @ret = sysVersionGuidLog; 
IF (@ret = -1) 	BEGIN ROLLBACK TRANSACTION; SELECT @options = @@OPTIONS; IF ( (512 & @options) = 512) SET @oldNoCountValue = 1 ELSE SET NOCOUNT ON;	DELETE #VersionGuidTmp; IF @oldNoCountValue = 0 SET NOCOUNT OFF;	END
ELSE BEGIN
------------------------------------------------------------------------------------------------------------------------------------

ALTER TABLE [dbo].[AuditLog]
ALTER COLUMN [FieldValueID] int


ALTER TABLE [dbo].[AuditLog]
ADD [FieldName] varchar(512) NULL


ALTER TABLE [dbo].[AuditLog]
ADD [RowNumber] int NULL


ALTER TABLE [dbo].[AuditLog]
ADD [ErrorMessage] nvarchar(max) NULL


ALTER TABLE [dbo].[AuditLog]
ADD [UploadLogID] int NULL


ALTER TABLE [dbo].[AuditLog]
ADD CONSTRAINT [FK_AuditLog_UploadLog] FOREIGN KEY ([UploadLogID]) 
  REFERENCES [dbo].[UploadLog] ([ID]) 
  ON UPDATE NO ACTION
  ON DELETE NO ACTION


CREATE TABLE [dbo].[UploadLogStrokeData] (
  [UploadLogID] int NOT NULL,
  [StrokeDataID] int NOT NULL,
  PRIMARY KEY CLUSTERED ([UploadLogID], [StrokeDataID])
)


ALTER TABLE [dbo].[UploadLogStrokeData]
ADD CONSTRAINT [FK_UploadLog] FOREIGN KEY ([UploadLogID]) 
  REFERENCES [dbo].[UploadLog] ([ID]) 
  ON UPDATE NO ACTION
  ON DELETE NO ACTION


ALTER TABLE [dbo].[UploadLogStrokeData]
ADD CONSTRAINT [FK_StrokeData] FOREIGN KEY ([StrokeDataID]) 
  REFERENCES [dbo].[StrokeData] ([ID]) 
  ON UPDATE NO ACTION
  ON DELETE NO ACTION


ALTER TABLE [dbo].[StrokeData]
DROP CONSTRAINT [FK_StrokeData_UploadLog]


ALTER TABLE [dbo].[StrokeData]
DROP COLUMN [UploadLogID]


-------------------------------------------------------------------------------------------------------------------------------------
	EXEC sysVersionGuidCommit;
	COMMIT TRANSACTION;
END
GO
--) FD41879E-A939-4AAB-BB99-4877BE589C9E

--(
-- Select the expression below (not including leading -- ) and execute by press F5. Copy result Guid from Messages windows and 
-- paste it to initialize @versionGuid and @dateMarked of your block. Don't forget change @description
-- PRINT '''' + CAST(NEWID() as varchar(36)) + '''' + CAST(0x0D0A as varchar(2)) + '''' + CONVERT(varchar(20), GETDATE(), 113) + ''''


DECLARE @ret int;
EXEC @ret = sysVersionGuidPrepare @versionGuid = 'EE244ADD-1756-428B-91CD-4562ADB86843',
                                  @dateMarked  = '29 Jan 2010 20:23:19',
                                  @description = 'alexl: Format key fields';
GO
DECLARE @ret int, @options int, @oldNoCountValue bit; 
BEGIN TRANSACTION;
EXEC @ret = sysVersionGuidLog; 
IF (@ret = -1) 	BEGIN ROLLBACK TRANSACTION; SELECT @options = @@OPTIONS; IF ( (512 & @options) = 512) SET @oldNoCountValue = 1 ELSE SET NOCOUNT ON;	DELETE #VersionGuidTmp; IF @oldNoCountValue = 0 SET NOCOUNT OFF;	END
ELSE BEGIN
------------------------------------------------------------------------------------------------------------------------------------
ALTER TABLE [dbo].[Format]
ADD [KeyFields] varchar(1024) NULL


EXEC sp_addextendedproperty 'MS_Description', N'List of key field names', 'schema', 'dbo', 'table', 'Format', 'column', 'KeyFields'

ALTER TABLE [dbo].[StrokeData]
ADD [AdmissionDate] datetime NOT NULL


CREATE UNIQUE NONCLUSTERED INDEX [StrokeData_idx] ON [dbo].[StrokeData]
  ([PatientID], [AdmissionDate])
WITH (
  PAD_INDEX = OFF,
  IGNORE_DUP_KEY = ON,
  DROP_EXISTING = OFF,
  STATISTICS_NORECOMPUTE = OFF,
  SORT_IN_TEMPDB = OFF,
  ONLINE = OFF,
  ALLOW_ROW_LOCKS = ON,
  ALLOW_PAGE_LOCKS = ON)


-------------------------------------------------------------------------------------------------------------------------------------
	EXEC sysVersionGuidCommit;
	COMMIT TRANSACTION;
END
GO

--) EE244ADD-1756-428B-91CD-4562ADB86843


--(
-- Select the expression below (not including leading -- ) and execute by press F5. Copy result Guid from Messages windows and 
-- paste it to initialize @versionGuid and @dateMarked of your block. Don't forget change @description
-- PRINT '''' + CAST(NEWID() as varchar(36)) + '''' + CAST(0x0D0A as varchar(2)) + '''' + CONVERT(varchar(20), GETDATE(), 113) + ''''


DECLARE @ret int;
EXEC @ret = sysVersionGuidPrepare @versionGuid = '1AAFA5FF-12F7-4998-8CAE-6C38813025E4',
                                  @dateMarked  = '31 Jan 2010 12:44:16',
                                  @description = 'alexl: add ItemType field in AuditLog';
GO
DECLARE @ret int, @options int, @oldNoCountValue bit; 
BEGIN TRANSACTION;
EXEC @ret = sysVersionGuidLog; 
IF (@ret = -1) 	BEGIN ROLLBACK TRANSACTION; SELECT @options = @@OPTIONS; IF ( (512 & @options) = 512) SET @oldNoCountValue = 1 ELSE SET NOCOUNT ON;	DELETE #VersionGuidTmp; IF @oldNoCountValue = 0 SET NOCOUNT OFF;	END
ELSE BEGIN
------------------------------------------------------------------------------------------------------------------------------------
ALTER TABLE [dbo].[AuditLog]
ADD [ItemType] smallint NULL

EXEC sp_addextendedproperty 'MS_Description', N'The type of the item audited (StrokeValue, Meta data)', 'schema', 'dbo', 'table', 'AuditLog'



-------------------------------------------------------------------------------------------------------------------------------------
	EXEC sysVersionGuidCommit;
	COMMIT TRANSACTION;
END
GO

--) 1AAFA5FF-12F7-4998-8CAE-6C38813025E4


--(
-- Select the expression below (not including leading -- ) and execute by press F5. Copy result Guid from Messages windows and 
-- paste it to initialize @versionGuid and @dateMarked of your block. Don't forget change @description
-- PRINT '''' + CAST(NEWID() as varchar(36)) + '''' + CAST(0x0D0A as varchar(2)) + '''' + CONVERT(varchar(20), GETDATE(), 113) + ''''


DECLARE @ret int;
EXEC @ret = sysVersionGuidPrepare @versionGuid = '19257E9E-6F4A-4797-955C-03B294B6C4A5',
                                  @dateMarked  = '02 Feb 2010 15:10:02',
                                  @description = 'alexa: added CorrectionsCount and OriginalFilename to UploadLog';
GO
DECLARE @ret int, @options int, @oldNoCountValue bit; 
BEGIN TRANSACTION;
EXEC @ret = sysVersionGuidLog; 
IF (@ret = -1) 	BEGIN ROLLBACK TRANSACTION; SELECT @options = @@OPTIONS; IF ( (512 & @options) = 512) SET @oldNoCountValue = 1 ELSE SET NOCOUNT ON;	DELETE #VersionGuidTmp; IF @oldNoCountValue = 0 SET NOCOUNT OFF;	END
ELSE BEGIN
------------------------------------------------------------------------------------------------------------------------------------
ALTER TABLE [dbo].[UploadLog]
ADD [CorrectionsCount] int NULL

ALTER TABLE [dbo].[UploadLog]
ADD [OriginalFilename] nvarchar(1024) NULL
-------------------------------------------------------------------------------------------------------------------------------------
	EXEC sysVersionGuidCommit;
	COMMIT TRANSACTION;
END
GO
--) '19257E9E-6F4A-4797-955C-03B294B6C4A5'

--(
-- Select the expression below (not including leading -- ) and execute by press F5. Copy result Guid from Messages windows and 
-- paste it to initialize @versionGuid and @dateMarked of your block. Don't forget change @description
-- PRINT '''' + CAST(NEWID() as varchar(36)) + '''' + CAST(0x0D0A as varchar(2)) + '''' + CONVERT(varchar(20), GETDATE(), 113) + ''''


DECLARE @ret int;
EXEC @ret = sysVersionGuidPrepare @versionGuid = '19257E9E-6F4A-4797-955C-03B294B6C4A6',
                                  @dateMarked  = '03 Feb 2010 23:10:02',
                                  @description = 'alexl: fix AuditLog.ActionDate field';
GO
DECLARE @ret int, @options int, @oldNoCountValue bit; 
BEGIN TRANSACTION;
EXEC @ret = sysVersionGuidLog; 
IF (@ret = -1) 	BEGIN ROLLBACK TRANSACTION; SELECT @options = @@OPTIONS; IF ( (512 & @options) = 512) SET @oldNoCountValue = 1 ELSE SET NOCOUNT ON;	DELETE #VersionGuidTmp; IF @oldNoCountValue = 0 SET NOCOUNT OFF;	END
ELSE BEGIN
------------------------------------------------------------------------------------------------------------------------------------
ALTER TABLE [dbo].[AuditLog]
ALTER COLUMN [ActionDate] datetime NOT NULL
-------------------------------------------------------------------------------------------------------------------------------------
	EXEC sysVersionGuidCommit;
	COMMIT TRANSACTION;
END
GO
--) '19257E9E-6F4A-4797-955C-03B294B6C4A6'

--(
-- Select the expression below (not including leading -- ) and execute by press F5. Copy result Guid from Messages windows and 
-- paste it to initialize @versionGuid and @dateMarked of your block. Don't forget change @description
-- PRINT '''' + CAST(NEWID() as varchar(36)) + '''' + CAST(0x0D0A as varchar(2)) + '''' + CONVERT(varchar(20), GETDATE(), 113) + ''''
DECLARE @ret int;
EXEC @ret = sysVersionGuidPrepare @versionGuid = '2C26955F-ACE6-4AD8-90C9-52438BCA08FE',
                                  @dateMarked  = '06 Feb 2010 14:43:04',
                                  @description = 'alexa: ';
GO
DECLARE @ret int, @options int, @oldNoCountValue bit; 
BEGIN TRANSACTION;
EXEC @ret = sysVersionGuidLog; 
IF (@ret = -1) 	BEGIN ROLLBACK TRANSACTION; SELECT @options = @@OPTIONS; IF ( (512 & @options) = 512) SET @oldNoCountValue = 1 ELSE SET NOCOUNT ON;	DELETE #VersionGuidTmp; IF @oldNoCountValue = 0 SET NOCOUNT OFF;	END
ELSE BEGIN
------------------------------------------------------------------------------------------------------------------------------------
ALTER TABLE [dbo].[AuditLog]
ALTER COLUMN [ActionDate] datetime NOT NULL
-------------------------------------------------------------------------------------------------------------------------------------
	EXEC sysVersionGuidCommit;
	COMMIT TRANSACTION;
END
GO
--) '2C26955F-ACE6-4AD8-90C9-52438BCA08FE'


--(
-- Select the expression below (not including leading -- ) and execute by press F5. Copy result Guid from Messages windows and 
-- paste it to initialize @versionGuid and @dateMarked of your block. Don't forget change @description
-- PRINT '''' + CAST(NEWID() as varchar(36)) + '''' + CAST(0x0D0A as varchar(2)) + '''' + CONVERT(varchar(20), GETDATE(), 113) + ''''
DECLARE @ret int;
EXEC @ret = sysVersionGuidPrepare @versionGuid = 'F7CB615F-73E4-47DE-ACEF-159A8BCAE7EF',
                                  @dateMarked  = '23 Feb 2010 11:02:49',
                                  @description = 'alexa: added IsNew field to FieldDefinition for Eureka native fields';
GO
DECLARE @ret int, @options int, @oldNoCountValue bit; 
BEGIN TRANSACTION;
EXEC @ret = sysVersionGuidLog; 
IF (@ret = -1) 	BEGIN ROLLBACK TRANSACTION; SELECT @options = @@OPTIONS; IF ( (512 & @options) = 512) SET @oldNoCountValue = 1 ELSE SET NOCOUNT ON;	DELETE #VersionGuidTmp; IF @oldNoCountValue = 0 SET NOCOUNT OFF;	END
ELSE BEGIN
------------------------------------------------------------------------------------------------------------------------------------
--ALTER TABLE [dbo].[FieldDefinition] ADD [IsNew] bit NOT NULL DEFAULT(1);
-------------------------------------------------------------------------------------------------------------------------------------
	EXEC sysVersionGuidCommit;
	COMMIT TRANSACTION;
END
GO
--) 'F7CB615F-73E4-47DE-ACEF-159A8BCAE7EF'

--(
-- Select the expression below (not including leading -- ) and execute by press F5. Copy result Guid from Messages windows and 
-- paste it to initialize @versionGuid and @dateMarked of your block. Don't forget change @description
-- PRINT '''' + CAST(NEWID() as varchar(36)) + '''' + CAST(0x0D0A as varchar(2)) + '''' + CONVERT(varchar(20), GETDATE(), 113) + ''''
DECLARE @ret int;
EXEC @ret = sysVersionGuidPrepare @versionGuid = 'F7CB615F-73E4-47DE-ACEF-159A8BCAE7ED',
                                  @dateMarked  = '23 Feb 2010 11:02:50',
                                  @description = 'alexa: made all existing fields as not new';
GO
DECLARE @ret int, @options int, @oldNoCountValue bit; 
BEGIN TRANSACTION;
EXEC @ret = sysVersionGuidLog; 
IF (@ret = -1) 	BEGIN ROLLBACK TRANSACTION; SELECT @options = @@OPTIONS; IF ( (512 & @options) = 512) SET @oldNoCountValue = 1 ELSE SET NOCOUNT ON;	DELETE #VersionGuidTmp; IF @oldNoCountValue = 0 SET NOCOUNT OFF;	END
ELSE BEGIN
------------------------------------------------------------------------------------------------------------------------------------
--UPDATE [dbo].[FieldDefinition] SET [IsNew] = 0;
-------------------------------------------------------------------------------------------------------------------------------------
	EXEC sysVersionGuidCommit;
	COMMIT TRANSACTION;
END
GO
--) 'F7CB615F-73E4-47DE-ACEF-159A8BCAE7ED'

--(
-- Select the expression below (not including leading -- ) and execute by press F5. Copy result Guid from Messages windows and 
-- paste it to initialize @versionGuid and @dateMarked of your block. Don't forget change @description
-- PRINT '''' + CAST(NEWID() as varchar(36)) + '''' + CAST(0x0D0A as varchar(2)) + '''' + CONVERT(varchar(20), GETDATE(), 113) + ''''
DECLARE @ret int;
EXEC @ret = sysVersionGuidPrepare @versionGuid = '1628F2B7-A3F1-495D-BBFE-ECA0637C176B',
                                  @dateMarked  = '17 Mar 2010 16:13:56',
                                  @description = 'alexa: created new definition sctructure';
GO
DECLARE @ret int, @options int, @oldNoCountValue bit; 
BEGIN TRANSACTION;
EXEC @ret = sysVersionGuidLog; 
IF (@ret = -1) 	BEGIN ROLLBACK TRANSACTION; SELECT @options = @@OPTIONS; IF ( (512 & @options) = 512) SET @oldNoCountValue = 1 ELSE SET NOCOUNT ON;	DELETE #VersionGuidTmp; IF @oldNoCountValue = 0 SET NOCOUNT OFF;	END
ELSE BEGIN
------------------------------------------------------------------------------------------------------------------------------------
CREATE TABLE [dbo].[EurekaField] (
  [ID] int IDENTITY(1,1) NOT NULL,
  [Name] varchar(64)  NOT NULL,
  [FormatID] int NOT NULL,
  [FieldType] smallint NOT NULL,
  [Description] varchar(max) NULL,
  [IsMandatory] bit NOT NULL,
  [CreatedBy] int NULL,
  [LastModifiedBy] int NULL,
  [CreatedDate] datetime NULL,
  [LastModifiedDate] datetime NULL,
  CONSTRAINT PK_EurekaField PRIMARY KEY CLUSTERED (ID)
)
ON [PRIMARY];
ALTER TABLE [dbo].[EurekaField]
	ADD CONSTRAINT [FK_EurekaField_Format] FOREIGN KEY ([FormatID]) 
		REFERENCES [dbo].[Format] ([ID]) 
		ON UPDATE NO ACTION
		ON DELETE NO ACTION;

CREATE TABLE [dbo].[AlienField] (
  [ID] int IDENTITY(1,1) NOT NULL,
  [Name] varchar(64)  NOT NULL,
  [EurekaFieldID] int NOT NULL,
  [FormatID] int NOT NULL,
  [RegularExpression] varchar(64) NULL,
  [CreatedBy] int NULL,
  [LastModifiedBy] int NULL,
  [CreatedDate] datetime NULL,
  [LastModifiedDate] datetime NULL,
  CONSTRAINT PK_AlienField PRIMARY KEY CLUSTERED (ID)
)
ON [PRIMARY];
ALTER TABLE [dbo].[AlienField]
	ADD CONSTRAINT [FK_AlienField_Format] FOREIGN KEY ([FormatID]) 
		REFERENCES [dbo].[Format] ([ID]) 
		ON UPDATE NO ACTION
		ON DELETE NO ACTION;
ALTER TABLE [dbo].[AlienField]
	ADD CONSTRAINT [FK_AlienField_EurekaField] FOREIGN KEY ([EurekaFieldID]) 
		REFERENCES [dbo].[EurekaField] ([ID]) 
		ON UPDATE NO ACTION
		ON DELETE NO ACTION;

CREATE TABLE [dbo].[EurekaValue] (
  [ID] int IDENTITY(1,1) NOT NULL,
  [EurekaFieldID] int NOT NULL,
  [Value] varchar(64) NOT NULL,
  [Description] varchar(max) NULL,
  [CreatedBy] int NULL,
  [LastModifiedBy] int NULL,
  [CreatedDate] datetime NULL,
  [LastModifiedDate] datetime NULL,
  CONSTRAINT PK_EurekaValue PRIMARY KEY CLUSTERED (ID)
)
ON [PRIMARY];
ALTER TABLE [dbo].[EurekaValue]
	ADD CONSTRAINT [FK_EurekaValue_EurekaField] FOREIGN KEY ([EurekaFieldID]) 
		REFERENCES [dbo].[EurekaField] ([ID]) 
		ON UPDATE NO ACTION
		ON DELETE NO ACTION;

CREATE TABLE [dbo].[AlienValue] (
  [ID] int IDENTITY(1,1) NOT NULL,
  [EurekaFieldID] int NOT NULL,
  [FormatID] int NOT NULL,
  [Value] varchar(64) NOT NULL,
  [EurekaValueID] int NULL,
  [CreatedBy] int NULL,
  [LastModifiedBy] int NULL,
  [CreatedDate] datetime NULL,
  [LastModifiedDate] datetime NULL,
  [Description] nvarchar(max) NOT NULL, 
  CONSTRAINT PK_AlienValue PRIMARY KEY CLUSTERED (ID)
)
ON [PRIMARY];
ALTER TABLE [dbo].[AlienValue]
	ADD CONSTRAINT [FK_AlienValue_Format] FOREIGN KEY ([FormatID]) 
		REFERENCES [dbo].[Format] ([ID]) 
		ON UPDATE NO ACTION
		ON DELETE NO ACTION;
ALTER TABLE [dbo].[AlienValue]
	ADD CONSTRAINT [FK_AlienValue_EurekaField] FOREIGN KEY ([EurekaFieldID]) 
		REFERENCES [dbo].[EurekaField] ([ID]) 
		ON UPDATE NO ACTION
		ON DELETE NO ACTION;



-------------------------------------------------------------------------------------------------------------------------------------
	EXEC sysVersionGuidCommit;
	COMMIT TRANSACTION;
END
GO
--) '1628F2B7-A3F1-495D-BBFE-ECA0637C176B'



--(
-- Select the expression below (not including leading -- ) and execute by press F5. Copy result Guid from Messages windows and 
-- paste it to initialize @versionGuid and @dateMarked of your block. Don't forget change @description
-- PRINT '''' + CAST(NEWID() as varchar(36)) + '''' + CAST(0x0D0A as varchar(2)) + '''' + CONVERT(varchar(20), GETDATE(), 113) + ''''
DECLARE @ret int;
EXEC @ret = sysVersionGuidPrepare @versionGuid = '5BD5C470-355A-49ED-B93D-249F08E4A355',
                                  @dateMarked  = '17 Mar 2010 17:28:42',
                                  @description = 'alexa: link stroke data to new definitions structure instead of old one';
GO
DECLARE @ret int, @options int, @oldNoCountValue bit; 
BEGIN TRANSACTION;
EXEC @ret = sysVersionGuidLog; 
IF (@ret = -1) 	BEGIN ROLLBACK TRANSACTION; SELECT @options = @@OPTIONS; IF ( (512 & @options) = 512) SET @oldNoCountValue = 1 ELSE SET NOCOUNT ON;	DELETE #VersionGuidTmp; IF @oldNoCountValue = 0 SET NOCOUNT OFF;	END
ELSE BEGIN
------------------------------------------------------------------------------------------------------------------------------------
--ALTER TABLE StrokeFieldValue
	--DROP CONSTRAINT FK_StrokeFieldValue_FieldDefinition;
ALTER TABLE StrokeFieldValue
	ADD CONSTRAINT FK_StrokeFieldValue_EurekaField
	FOREIGN KEY ([FieldID]) 
		REFERENCES [dbo].[EurekaField] ([ID]) 
		ON UPDATE NO ACTION
		ON DELETE NO ACTION;
-------------------------------------------------------------------------------------------------------------------------------------
	EXEC sysVersionGuidCommit;
	COMMIT TRANSACTION;
END
GO
--) '5BD5C470-355A-49ED-B93D-249F08E4A355'



--(
-- Select the expression below (not including leading -- ) and execute by press F5. Copy result Guid from Messages windows and 
-- paste it to initialize @versionGuid and @dateMarked of your block. Don't forget change @description
-- PRINT '''' + CAST(NEWID() as varchar(36)) + '''' + CAST(0x0D0A as varchar(2)) + '''' + CONVERT(varchar(20), GETDATE(), 113) + ''''
DECLARE @ret int;
EXEC @ret = sysVersionGuidPrepare @versionGuid = '5BD5C470-355A-49ED-B93D-249F08E4A355',
                                  @dateMarked  = '17 Mar 2010 17:28:42',
                                  @description = 'alexa: link stroke data to new definitions structure instead of old one';
GO
DECLARE @ret int, @options int, @oldNoCountValue bit; 
BEGIN TRANSACTION;
EXEC @ret = sysVersionGuidLog; 
IF (@ret = -1) 	BEGIN ROLLBACK TRANSACTION; SELECT @options = @@OPTIONS; IF ( (512 & @options) = 512) SET @oldNoCountValue = 1 ELSE SET NOCOUNT ON;	DELETE #VersionGuidTmp; IF @oldNoCountValue = 0 SET NOCOUNT OFF;	END
ELSE BEGIN
------------------------------------------------------------------------------------------------------------------------------------
--ALTER TABLE StrokeFieldValue
	--DROP CONSTRAINT FK_StrokeFieldValue_FieldDefinition;
ALTER TABLE StrokeFieldValue
	ADD CONSTRAINT FK_StrokeFieldValue_EurekaField
	FOREIGN KEY ([FieldID]) 
		REFERENCES [dbo].[EurekaField] ([ID]) 
		ON UPDATE NO ACTION
		ON DELETE NO ACTION;
-------------------------------------------------------------------------------------------------------------------------------------
	EXEC sysVersionGuidCommit;
	COMMIT TRANSACTION;
END
GO
--) '5BD5C470-355A-49ED-B93D-249F08E4A355'


--(
-- Select the expression below (not including leading -- ) and execute by press F5. Copy result Guid from Messages windows and 
-- paste it to initialize @versionGuid and @dateMarked of your block. Don't forget change @description
-- PRINT '''' + CAST(NEWID() as varchar(36)) + '''' + CAST(0x0D0A as varchar(2)) + '''' + CONVERT(varchar(20), GETDATE(), 113) + ''''
DECLARE @ret int;
EXEC @ret = sysVersionGuidPrepare @versionGuid = 'B9DFF134-624A-4E72-9A62-EEF205DBDF01',
                                  @dateMarked  = '18 Mar 2010 17:12:18',
                                  @description = 'alexa: added one more optinal FK alienValue to eurekaValue';
GO
DECLARE @ret int, @options int, @oldNoCountValue bit; 
BEGIN TRANSACTION;
EXEC @ret = sysVersionGuidLog; 
IF (@ret = -1) 	BEGIN ROLLBACK TRANSACTION; SELECT @options = @@OPTIONS; IF ( (512 & @options) = 512) SET @oldNoCountValue = 1 ELSE SET NOCOUNT ON;	DELETE #VersionGuidTmp; IF @oldNoCountValue = 0 SET NOCOUNT OFF;	END
ELSE BEGIN
------------------------------------------------------------------------------------------------------------------------------------
ALTER TABLE AlienValue
	ADD CONSTRAINT FK_AlienValue_EurekaValue
	FOREIGN KEY ([EurekaValueID]) 
		REFERENCES [dbo].[EurekaValue] ([ID]) 
		ON UPDATE NO ACTION
		ON DELETE NO ACTION;
		
		Alter table UploadLog 
		ADD CONSTRAINT FK_UploadLog_Format
		FOREIGN KEY ([FormatID]) 
		REFERENCES [dbo].[Format] ([ID]) 
		ON UPDATE NO ACTION
		ON DELETE NO ACTION;
-------------------------------------------------------------------------------------------------------------------------------------
	EXEC sysVersionGuidCommit;
	COMMIT TRANSACTION;
END
GO
--) 'B9DFF134-624A-4E72-9A62-EEF205DBDF01'


--(
-- Select the expression below (not including leading -- ) and execute by press F5. Copy result Guid from Messages windows and 
-- paste it to initialize @versionGuid and @dateMarked of your block. Don't forget change @description
-- PRINT '''' + CAST(NEWID() as varchar(36)) + '''' + CAST(0x0D0A as varchar(2)) + '''' + CONVERT(varchar(20), GETDATE(), 113) + ''''
DECLARE @ret int;
EXEC @ret = sysVersionGuidPrepare @versionGuid = '74F172A7-E460-4EC6-ADC3-7A5560D808C9',
                                  @dateMarked  = '21 Mar 2010 13:24:18',
                                  @description = 'alexa: provided status for the format';
GO
DECLARE @ret int, @options int, @oldNoCountValue bit; 
BEGIN TRANSACTION;
EXEC @ret = sysVersionGuidLog; 
IF (@ret = -1) 	BEGIN ROLLBACK TRANSACTION; SELECT @options = @@OPTIONS; IF ( (512 & @options) = 512) SET @oldNoCountValue = 1 ELSE SET NOCOUNT ON;	DELETE #VersionGuidTmp; IF @oldNoCountValue = 0 SET NOCOUNT OFF;	END
ELSE BEGIN
------------------------------------------------------------------------------------------------------------------------------------
ALTER TABLE Format ADD Status smallint NULL CONSTRAINT DF_Format_Status DEFAULT(0);
-------------------------------------------------------------------------------------------------------------------------------------
	EXEC sysVersionGuidCommit;
	COMMIT TRANSACTION;
END
GO
--) '74F172A7-E460-4EC6-ADC3-7A5560D808C9'

--(
-- Select the expression below (not including leading -- ) and execute by press F5. Copy result Guid from Messages windows and 
-- paste it to initialize @versionGuid and @dateMarked of your block. Don't forget change @description
-- PRINT '''' + CAST(NEWID() as varchar(36)) + '''' + CAST(0x0D0A as varchar(2)) + '''' + CONVERT(varchar(20), GETDATE(), 113) + ''''
DECLARE @ret int;
EXEC @ret = sysVersionGuidPrepare @versionGuid = '2FA7015B-55CB-4BA2-842F-FEDD6EAB24AE',
                                  @dateMarked  = '21 Mar 2010 13:49:10',
                                  @description = 'alexa: set all formats as existing';
GO
DECLARE @ret int, @options int, @oldNoCountValue bit; 
BEGIN TRANSACTION;
EXEC @ret = sysVersionGuidLog; 
IF (@ret = -1) 	BEGIN ROLLBACK TRANSACTION; SELECT @options = @@OPTIONS; IF ( (512 & @options) = 512) SET @oldNoCountValue = 1 ELSE SET NOCOUNT ON;	DELETE #VersionGuidTmp; IF @oldNoCountValue = 0 SET NOCOUNT OFF;	END
ELSE BEGIN
------------------------------------------------------------------------------------------------------------------------------------
UPDATE Format SET Status=1;
-------------------------------------------------------------------------------------------------------------------------------------
	EXEC sysVersionGuidCommit;
	COMMIT TRANSACTION;
END
GO
--) '2FA7015B-55CB-4BA2-842F-FEDD6EAB24AE'



--(
-- Select the expression below (not including leading -- ) and execute by press F5. Copy result Guid from Messages windows and 
-- paste it to initialize @versionGuid and @dateMarked of your block. Don't forget change @description
-- PRINT '''' + CAST(NEWID() as varchar(36)) + '''' + CAST(0x0D0A as varchar(2)) + '''' + CONVERT(varchar(20), GETDATE(), 113) + ''''
DECLARE @ret int;
EXEC @ret = sysVersionGuidPrepare @versionGuid = 'D9D428C7-BC28-4D06-A1A8-76EFAABA82C1',
                                  @dateMarked  = '02 Apr 2010 08:15:39',
                                  @description = 'alexa: added IsNew field to FieldDefinition for Eureka native fields 2';
GO
DECLARE @ret int, @options int, @oldNoCountValue bit; 
BEGIN TRANSACTION;
EXEC @ret = sysVersionGuidLog; 
IF (@ret = -1) 	BEGIN ROLLBACK TRANSACTION; SELECT @options = @@OPTIONS; IF ( (512 & @options) = 512) SET @oldNoCountValue = 1 ELSE SET NOCOUNT ON;	DELETE #VersionGuidTmp; IF @oldNoCountValue = 0 SET NOCOUNT OFF;	END
ELSE BEGIN
------------------------------------------------------------------------------------------------------------------------------------
ALTER TABLE [dbo].[EurekaField] ADD [IsNew] bit NOT NULL DEFAULT(1);
-------------------------------------------------------------------------------------------------------------------------------------
	EXEC sysVersionGuidCommit;
	COMMIT TRANSACTION;
END
GO
--) 'D9D428C7-BC28-4D06-A1A8-76EFAABA82C1'

--(
-- Select the expression below (not including leading -- ) and execute by press F5. Copy result Guid from Messages windows and 
-- paste it to initialize @versionGuid and @dateMarked of your block. Don't forget change @description
-- PRINT '''' + CAST(NEWID() as varchar(36)) + '''' + CAST(0x0D0A as varchar(2)) + '''' + CONVERT(varchar(20), GETDATE(), 113) + ''''
DECLARE @ret int;
EXEC @ret = sysVersionGuidPrepare @versionGuid = 'AB8573CC-FE65-4F1C-8C12-7069EE489622',
                                  @dateMarked  = '02 Apr 2010 08:16:11',
                                  @description = 'alexa: made all existing fields as not new 2';
GO
DECLARE @ret int, @options int, @oldNoCountValue bit; 
BEGIN TRANSACTION;
EXEC @ret = sysVersionGuidLog; 
IF (@ret = -1) 	BEGIN ROLLBACK TRANSACTION; SELECT @options = @@OPTIONS; IF ( (512 & @options) = 512) SET @oldNoCountValue = 1 ELSE SET NOCOUNT ON;	DELETE #VersionGuidTmp; IF @oldNoCountValue = 0 SET NOCOUNT OFF;	END
ELSE BEGIN
------------------------------------------------------------------------------------------------------------------------------------
UPDATE [dbo].[EurekaField] SET [IsNew] = 0;
-------------------------------------------------------------------------------------------------------------------------------------
	EXEC sysVersionGuidCommit;
	COMMIT TRANSACTION;
END
GO
--) 'AB8573CC-FE65-4F1C-8C12-7069EE489622'



--(
-- Select the expression below (not including leading -- ) and execute by press F5. Copy result Guid from Messages windows and 
-- paste it to initialize @versionGuid and @dateMarked of your block. Don't forget change @description
-- PRINT '''' + CAST(NEWID() as varchar(36)) + '''' + CAST(0x0D0A as varchar(2)) + '''' + CONVERT(varchar(20), GETDATE(), 113) + ''''
DECLARE @ret int;
EXEC @ret = sysVersionGuidPrepare @versionGuid = 'EE27610E-E08E-4631-B3E6-D15D6A314463',
                                  @dateMarked  = '05 Apr 2010 21:57:35',
                                  @description = 'alexl: add indexes';
GO
DECLARE @ret int, @options int, @oldNoCountValue bit; 
BEGIN TRANSACTION;
EXEC @ret = sysVersionGuidLog; 
IF (@ret = -1) 	BEGIN ROLLBACK TRANSACTION; SELECT @options = @@OPTIONS; IF ( (512 & @options) = 512) SET @oldNoCountValue = 1 ELSE SET NOCOUNT ON;	DELETE #VersionGuidTmp; IF @oldNoCountValue = 0 SET NOCOUNT OFF;	END
ELSE BEGIN
------------------------------------------------------------------------------------------------------------------------------------

/****** Object:  Index [ix_StrokeFieldValue_Field]    Script Date: 04/05/2010 20:09:13 ******/
CREATE NONCLUSTERED INDEX [ix_StrokeFieldValue_Field] ON [dbo].[StrokeFieldValue] 
(
	[FieldID] ASC
)
INCLUDE ( [StringValue],
[Status]) WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, SORT_IN_TEMPDB = OFF, IGNORE_DUP_KEY = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]


/****** Object:  Index [ix_StrokeFieldValue_StrokeData]    Script Date: 04/05/2010 20:12:00 ******/
CREATE NONCLUSTERED INDEX [ix_StrokeFieldValue_StrokeData] ON [dbo].[StrokeFieldValue] 
(
	[StrokeDataID] ASC
)
INCLUDE ( [StringValue],
[Status]) WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, SORT_IN_TEMPDB = OFF, IGNORE_DUP_KEY = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]


/****** Object:  Index [ix_AlienField_EurekaField]    Script Date: 04/05/2010 20:14:30 ******/
CREATE NONCLUSTERED INDEX [ix_AlienField_EurekaField] ON [dbo].[AlienField] 
(
	[EurekaFieldID] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, SORT_IN_TEMPDB = OFF, IGNORE_DUP_KEY = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]


/****** Object:  Index [ix_AlienField_Format]    Script Date: 04/05/2010 20:14:45 ******/
CREATE NONCLUSTERED INDEX [ix_AlienField_Format] ON [dbo].[AlienField] 
(
	[FormatID] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, SORT_IN_TEMPDB = OFF, IGNORE_DUP_KEY = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]


CREATE NONCLUSTERED INDEX [ix_AlienValue_EurekaField] ON [dbo].[AlienValue] 
(
	[EurekaFieldID] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, SORT_IN_TEMPDB = OFF, IGNORE_DUP_KEY = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]

/****** Object:  Index [ix_AlienValue_EurekaValue]    Script Date: 04/05/2010 20:19:45 ******/
CREATE NONCLUSTERED INDEX [ix_AlienValue_EurekaValue] ON [dbo].[AlienValue] 
(
	[EurekaValueID] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, SORT_IN_TEMPDB = OFF, IGNORE_DUP_KEY = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]

/****** Object:  Index [ix_AlienValue_Format]    Script Date: 04/05/2010 20:20:04 ******/
CREATE NONCLUSTERED INDEX [ix_AlienValue_Format] ON [dbo].[AlienValue] 
(
	[FormatID] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, SORT_IN_TEMPDB = OFF, IGNORE_DUP_KEY = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]

/****** Object:  Index [ix_AuditLog_FieldValue]    Script Date: 04/05/2010 21:38:56 ******/
CREATE NONCLUSTERED INDEX [ix_AuditLog_FieldValue] ON [dbo].[AuditLog] 
(
	[FieldValueID] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, SORT_IN_TEMPDB = OFF, IGNORE_DUP_KEY = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]


/****** Object:  Index [ix_AuditLog_UploadLog]    Script Date: 04/05/2010 21:39:23 ******/
CREATE NONCLUSTERED INDEX [ix_AuditLog_UploadLog] ON [dbo].[AuditLog] 
(
	[UploadLogID] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, SORT_IN_TEMPDB = OFF, IGNORE_DUP_KEY = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]


/****** Object:  Index [ix_EurekaField_Format]    Script Date: 04/05/2010 21:40:30 ******/
CREATE NONCLUSTERED INDEX [ix_EurekaField_Format] ON [dbo].[EurekaField] 
(
	[FormatID] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, SORT_IN_TEMPDB = OFF, IGNORE_DUP_KEY = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]


/****** Object:  Index [ix_EurekaValue_EurekaField]    Script Date: 04/05/2010 21:42:42 ******/
CREATE NONCLUSTERED INDEX [ix_EurekaValue_EurekaField] ON [dbo].[EurekaValue] 
(
	[EurekaFieldID] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, SORT_IN_TEMPDB = OFF, IGNORE_DUP_KEY = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]


/****** Object:  Index [ix_Facility_Format]    Script Date: 04/05/2010 21:44:11 ******/
CREATE NONCLUSTERED INDEX [ix_Facility_Format] ON [dbo].[Facility] 
(
	[FormatID] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, SORT_IN_TEMPDB = OFF, IGNORE_DUP_KEY = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]

IF EXISTS(SELECT name FROM sysindexes WHERE name = 'StrokeData_idx')
	drop index StrokeData.StrokeData_idx;

/****** Object:  Index [ix_StrokeData_KeyFields]    Script Date: 04/05/2010 21:48:12 ******/
CREATE NONCLUSTERED INDEX [ix_StrokeData_KeyFields] ON [dbo].[StrokeData] 
(
	[PatientID] ASC,
	[AdmissionDate] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, SORT_IN_TEMPDB = OFF, IGNORE_DUP_KEY = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]

/****** Object:  Index [ix_StrokeData_Status]    Script Date: 04/05/2010 21:48:39 ******/
CREATE NONCLUSTERED INDEX [ix_StrokeData_Status] ON [dbo].[StrokeData] 
(
	[Status] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, SORT_IN_TEMPDB = OFF, IGNORE_DUP_KEY = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]



/****** Object:  Index [UploadLog_Facility]    Script Date: 04/05/2010 21:50:18 ******/
CREATE NONCLUSTERED INDEX [ix_UploadLog_Facility] ON [dbo].[UploadLog] 
(
	[FacilityID] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, SORT_IN_TEMPDB = OFF, IGNORE_DUP_KEY = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]

/****** Object:  Index [UploadLog_Format]    Script Date: 04/05/2010 21:51:16 ******/
CREATE NONCLUSTERED INDEX [ix_UploadLog_Format] ON [dbo].[UploadLog] 
(
	[FormatID] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, SORT_IN_TEMPDB = OFF, IGNORE_DUP_KEY = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]

/****** Object:  Index [UploadLogStrokeData_UploadLog]    Script Date: 04/05/2010 21:53:21 ******/
CREATE NONCLUSTERED INDEX [ix_UploadLogStrokeData_UploadLog] ON [dbo].[UploadLogStrokeData] 
(
	[UploadLogID] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, SORT_IN_TEMPDB = OFF, IGNORE_DUP_KEY = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]


/****** Object:  Index [UploadLogStrokeData_StrokeData]    Script Date: 04/05/2010 21:53:47 ******/
CREATE NONCLUSTERED INDEX [ix_UploadLogStrokeData_StrokeData] ON [dbo].[UploadLogStrokeData] 
(
	[StrokeDataID] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, SORT_IN_TEMPDB = OFF, IGNORE_DUP_KEY = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]


-------------------------------------------------------------------------------------------------------------------------------------
	EXEC sysVersionGuidCommit;
	COMMIT TRANSACTION;
END
GO
--) 'EE27610E-E08E-4631-B3E6-D15D6A314463'


--(
-- Select the expression below (not including leading -- ) and execute by press F5. Copy result Guid from Messages windows and 
-- paste it to initialize @versionGuid and @dateMarked of your block. Don't forget change @description
-- PRINT '''' + CAST(NEWID() as varchar(36)) + '''' + CAST(0x0D0A as varchar(2)) + '''' + CONVERT(varchar(20), GETDATE(), 113) + ''''
DECLARE @ret int;
EXEC @ret = sysVersionGuidPrepare @versionGuid = 'DB67839D-8463-4051-8EC2-91421FBD9F9A',
                                  @dateMarked  = '10 Apr 2010 17:12:24',
                                  @description = 'alexa: provided smaller size for text fields';
GO
DECLARE @ret int, @options int, @oldNoCountValue bit; 
BEGIN TRANSACTION;
EXEC @ret = sysVersionGuidLog; 
IF (@ret = -1) 	BEGIN ROLLBACK TRANSACTION; SELECT @options = @@OPTIONS; IF ( (512 & @options) = 512) SET @oldNoCountValue = 1 ELSE SET NOCOUNT ON;	DELETE #VersionGuidTmp; IF @oldNoCountValue = 0 SET NOCOUNT OFF;	END
ELSE BEGIN
------------------------------------------------------------------------------------------------------------------------------------
ALTER TABLE Format ALTER COLUMN [Name] varchar(100) NOT NULL;
ALTER TABLE Facility ALTER COLUMN [Name] varchar(100) NOT NULL;
ALTER TABLE UploadLog ALTER COLUMN [Filename] varchar(100) NOT NULL;
ALTER TABLE UploadLog ALTER COLUMN [OriginalFilename] varchar(100) NOT NULL;
-------------------------------------------------------------------------------------------------------------------------------------
	EXEC sysVersionGuidCommit;
	COMMIT TRANSACTION;
END
GO
--) 'DB67839D-8463-4051-8EC2-91421FBD9F9A'

--(
-- Select the expression below (not including leading -- ) and execute by press F5. Copy result Guid from Messages windows and 
-- paste it to initialize @versionGuid and @dateMarked of your block. Don't forget change @description
-- PRINT '''' + CAST(NEWID() as varchar(36)) + '''' + CAST(0x0D0A as varchar(2)) + '''' + CONVERT(varchar(20), GETDATE(), 113) + ''''
DECLARE @ret int;
EXEC @ret = sysVersionGuidPrepare @versionGuid = 'E02C3C32-5B50-4E0F-B5AD-01A161D44AB8',
                                  @dateMarked  = '6 May 2010 17:12:24',
                                  @description = 'alexl: add [BusinessRuleCheckResults]';
GO
DECLARE @ret int, @options int, @oldNoCountValue bit; 
BEGIN TRANSACTION;
EXEC @ret = sysVersionGuidLog; 
IF (@ret = -1) 	BEGIN ROLLBACK TRANSACTION; SELECT @options = @@OPTIONS; IF ( (512 & @options) = 512) SET @oldNoCountValue = 1 ELSE SET NOCOUNT ON;	DELETE #VersionGuidTmp; IF @oldNoCountValue = 0 SET NOCOUNT OFF;	END
ELSE BEGIN
------------------------------------------------------------------------------------------------------------------------------------
CREATE TABLE BusinessRuleCheckResults (
  [ID] int IDENTITY(1, 1) NOT NULL,
  [StrokeDataID] int NOT NULL,
  [RuleID] int NOT NULL,
  [RuleBreaker] bit NOT NULL,
  [ErrorMessage] varchar(1024) NOT NULL,
  PRIMARY KEY CLUSTERED ([ID]),
  CONSTRAINT [BusinessRuleCheckResults_fk] FOREIGN KEY ([StrokeDataID]) 
  REFERENCES StrokeData ([ID]) 
  ON UPDATE NO ACTION
  ON DELETE NO ACTION
)
ON [PRIMARY]
-------------------------------------------------------------------------------------------------------------------------------------
	EXEC sysVersionGuidCommit;
	COMMIT TRANSACTION;
END
GO
--) 'E02C3C32-5B50-4E0F-B5AD-01A161D44AB8'



--(
-- Select the expression below (not including leading -- ) and execute by press F5. Copy result Guid from Messages windows and 
-- paste it to initialize @versionGuid and @dateMarked of your block. Don't forget change @description
-- PRINT '''' + CAST(NEWID() as varchar(36)) + '''' + CAST(0x0D0A as varchar(2)) + '''' + CONVERT(varchar(20), GETDATE(), 113) + ''''
DECLARE @ret int;
EXEC @ret = sysVersionGuidPrepare @versionGuid = '93B88974-4DD2-4EB2-A3DF-CB8BF88D7996',
                                  @dateMarked  = '20 Jul 2010 14:05:02',
                                  @description = 'alexa: add [FormatRecordKeyIDs]';
GO
DECLARE @ret int, @options int, @oldNoCountValue bit; 
BEGIN TRANSACTION;
EXEC @ret = sysVersionGuidLog; 
IF (@ret = -1) 	BEGIN ROLLBACK TRANSACTION; SELECT @options = @@OPTIONS; IF ( (512 & @options) = 512) SET @oldNoCountValue = 1 ELSE SET NOCOUNT ON;	DELETE #VersionGuidTmp; IF @oldNoCountValue = 0 SET NOCOUNT OFF;	END
ELSE BEGIN
------------------------------------------------------------------------------------------------------------------------------------
--DROP TABLE KeyEurekaField;
CREATE TABLE KeyEurekaField (
  --[ID] int IDENTITY(1, 1) NOT NULL,
  [FormatID] int NOT NULL,
  [EurekaFieldID] int NOT NULL,
  PRIMARY KEY NONCLUSTERED ([FormatID], [EurekaFieldID]),
  CONSTRAINT [FK_FormatRecordKeyIDs_FormatID] 
	FOREIGN KEY ([FormatID]) 
	REFERENCES Format ([ID]) 
	ON UPDATE NO ACTION
	ON DELETE NO ACTION,
  CONSTRAINT [FK_FormatRecordKeyIDs_EurekaFieldID] 
	FOREIGN KEY ([EurekaFieldID]) 
	REFERENCES EurekaField ([ID]) 
	ON UPDATE NO ACTION
	ON DELETE NO ACTION
)
ON [PRIMARY]
-------------------------------------------------------------------------------------------------------------------------------------
	EXEC sysVersionGuidCommit;
	COMMIT TRANSACTION;
END
GO
--) '93B88974-4DD2-4EB2-A3DF-CB8BF88D7996'




--(
-- Select the expression below (not including leading -- ) and execute by press F5. Copy result Guid from Messages windows and 
-- paste it to initialize @versionGuid and @dateMarked of your block. Don't forget change @description
-- PRINT '''' + CAST(NEWID() as varchar(36)) + '''' + CAST(0x0D0A as varchar(2)) + '''' + CONVERT(varchar(20), GETDATE(), 113) + ''''
DECLARE @ret int;
EXEC @ret = sysVersionGuidPrepare @versionGuid = '863A0F3D-E3F3-417B-9334-0DACFAF0540F',
                                  @dateMarked  = '17 Aug 2010 12:11:20',
                                  @description = 'alexa: added custom reports objects';
GO
DECLARE @ret int, @options int, @oldNoCountValue bit; 
BEGIN TRANSACTION;
EXEC @ret = sysVersionGuidLog; 
IF (@ret = -1) 	BEGIN ROLLBACK TRANSACTION; SELECT @options = @@OPTIONS; IF ( (512 & @options) = 512) SET @oldNoCountValue = 1 ELSE SET NOCOUNT ON;	DELETE #VersionGuidTmp; IF @oldNoCountValue = 0 SET NOCOUNT OFF;	END
ELSE BEGIN
------------------------------------------------------------------------------------------------------------------------------------

CREATE TABLE Folder (
  [ID] int IDENTITY(1, 1) NOT NULL,
  [Name] varchar(128) NOT NULL,
  [Rank] int NOT NULL,
  [Enabled] bit NOT NULL,
  PRIMARY KEY NONCLUSTERED ([ID])
)
ON [PRIMARY];

CREATE TABLE Report (
  [ID] int IDENTITY(1, 1) NOT NULL,
  [Name] varchar(128) NOT NULL,
  [Description] varchar(max) NULL,
  [Enabled] bit NOT NULL,
  [FolderID] int NOT NULL,
  PRIMARY KEY NONCLUSTERED ([ID]),
  CONSTRAINT [FK_Report_FolderID] 
	FOREIGN KEY ([FolderID]) 
	REFERENCES Folder ([ID]) 
	ON UPDATE NO ACTION
	ON DELETE NO ACTION
)
ON [PRIMARY];

CREATE TABLE UserFolder (
  [FolderID] int NOT NULL,
  [UserID] int NOT NULL,
  PRIMARY KEY NONCLUSTERED ([FolderID], [UserID]),
  CONSTRAINT [FK_UserFolder_FolderID] 
	FOREIGN KEY ([FolderID]) 
	REFERENCES Folder ([ID]) 
	ON UPDATE NO ACTION
	ON DELETE NO ACTION
)
ON [PRIMARY];

CREATE TABLE UserReport (
  [ReportID] int NOT NULL,
  [UserID] int NOT NULL,
  PRIMARY KEY NONCLUSTERED ([ReportID], [UserID]),
  CONSTRAINT [FK_UserReport_ReportID] 
	FOREIGN KEY ([ReportID]) 
	REFERENCES Report ([ID]) 
	ON UPDATE NO ACTION
	ON DELETE NO ACTION
)
ON [PRIMARY];

CREATE TABLE UserFacility (
  [FacilityID] int NOT NULL,
  [UserID] int NOT NULL,
  PRIMARY KEY NONCLUSTERED ([FacilityID], [UserID]),
  CONSTRAINT [FK_UserFacility_FacilityID] 
	FOREIGN KEY ([FacilityID]) 
	REFERENCES Facility ([ID]) 
	ON UPDATE NO ACTION
	ON DELETE NO ACTION
)
ON [PRIMARY];

CREATE TABLE UserReportingSource (
  [ReportingSource] varchar(128) NOT NULL,
  [UserID] int NOT NULL,
  PRIMARY KEY NONCLUSTERED ([ReportingSource], [UserID])
)
ON [PRIMARY];

-------------------------------------------------------------------------------------------------------------------------------------
	EXEC sysVersionGuidCommit;
	COMMIT TRANSACTION;
END
GO
--) '863A0F3D-E3F3-417B-9334-0DACFAF0540F'



--(
-- Select the expression below (not including leading -- ) and execute by press F5. Copy result Guid from Messages windows and 
-- paste it to initialize @versionGuid and @dateMarked of your block. Don't forget change @description
-- PRINT '''' + CAST(NEWID() as varchar(36)) + '''' + CAST(0x0D0A as varchar(2)) + '''' + CONVERT(varchar(20), GETDATE(), 113) + ''''
DECLARE @ret int;
EXEC @ret = sysVersionGuidPrepare @versionGuid = '8D5CF35A-F90B-4E6B-B6DE-6395AE04D5FE',
                                  @dateMarked  = '20 Aug 2010 13:41:40',
                                  @description = 'alexa: added ReportingSource';
GO
DECLARE @ret int, @options int, @oldNoCountValue bit; 
BEGIN TRANSACTION;
EXEC @ret = sysVersionGuidLog; 
IF (@ret = -1) 	BEGIN ROLLBACK TRANSACTION; SELECT @options = @@OPTIONS; IF ( (512 & @options) = 512) SET @oldNoCountValue = 1 ELSE SET NOCOUNT ON;	DELETE #VersionGuidTmp; IF @oldNoCountValue = 0 SET NOCOUNT OFF;	END
ELSE BEGIN
------------------------------------------------------------------------------------------------------------------------------------
ALTER TABLE StrokeData ADD ReportingSource varchar(128) NULL;
-------------------------------------------------------------------------------------------------------------------------------------
	EXEC sysVersionGuidCommit;
	COMMIT TRANSACTION;
END
GO
--) '8D5CF35A-F90B-4E6B-B6DE-6395AE04D5FE'



--(
-- Select the expression below (not including leading -- ) and execute by press F5. Copy result Guid from Messages windows and 
-- paste it to initialize @versionGuid and @dateMarked of your block. Don't forget change @description
-- PRINT '''' + CAST(NEWID() as varchar(36)) + '''' + CAST(0x0D0A as varchar(2)) + '''' + CONVERT(varchar(20), GETDATE(), 113) + ''''
DECLARE @ret int;
EXEC @ret = sysVersionGuidPrepare @versionGuid = '38FB81D9-D10A-49AA-B1BF-3FE2D47084AF',
                                  @dateMarked  = '25 Oct 2010 10:10:00',
                                  @description = 'alexa: added Report.RSName';
GO
DECLARE @ret int, @options int, @oldNoCountValue bit; 
BEGIN TRANSACTION;
EXEC @ret = sysVersionGuidLog; 
IF (@ret = -1) 	BEGIN ROLLBACK TRANSACTION; SELECT @options = @@OPTIONS; IF ( (512 & @options) = 512) SET @oldNoCountValue = 1 ELSE SET NOCOUNT ON;	DELETE #VersionGuidTmp; IF @oldNoCountValue = 0 SET NOCOUNT OFF;	END
ELSE BEGIN
------------------------------------------------------------------------------------------------------------------------------------
ALTER TABLE Report ADD RSName varchar(128) NOT NULL CONSTRAINT Report_RSName_Uniq UNIQUE(RSName);
-------------------------------------------------------------------------------------------------------------------------------------
	EXEC sysVersionGuidCommit;
	COMMIT TRANSACTION;
END
GO
--) '38FB81D9-D10A-49AA-B1BF-3FE2D47084AF'





-----------------------------------------------------
-- !!!  insert new versions before this block  !!!!!!
-- !!!  INSERT NEW VERSIONS BEFORE THIS BLOCK  !!!!!!
-----------------------------------------------------
IF EXISTS(SELECT * FROM #VersionGuidTmp) PRINT 'Not all block applied! See previous error...';
PRINT 'Checking latest applied VersionGuid...';
DECLARE @result varchar(8000);
SELECT TOP (1) @result = 'Current latest VersionGuid: ' + CAST([Guid] AS varchar(36)) + '
        Date Marked:  ' + CONVERT(varchar(30),[dateMarked],121) + '
        Date Applied: ' + CONVERT(varchar(30),[dateApplied],121) + '
        Description:  ' + [description] + '
' FROM [dbo].[VersionGuid]
ORDER BY [dateApplied] DESC,[dateMarked] DESC;
PRINT @result;
IF (OBJECT_ID('tempdb..#VersionGuidTmp') IS NOT NULL)
	DROP TABLE #VersionGuidTmp;
GO

/******************************************************************
 ** Insert new auto-changes by the TEMPLATE BLOCK below ...     ***
 ** Just remove heading ---- in every line (as vertical block)  ***
 ******************************************************************/

--(
-- Select the expression below (not including leading -- ) and execute by press F5. Copy result Guid from Messages windows and 
-- paste it to initialize @versionGuid and @dateMarked of your block. Don't forget change @description
-- PRINT '''' + CAST(NEWID() as varchar(36)) + '''' + CAST(0x0D0A as varchar(2)) + '''' + CONVERT(varchar(20), GETDATE(), 113) + ''''

----DECLARE @ret int;
----EXEC @ret = sysVersionGuidPrepare @versionGuid = 'XXXXXXXX-XXXX-XXXX-XXXX-XXXXXXXXXXXX',
----                                  @dateMarked  = 'XX Jun 200X 00:00:00',
----                                  @description = 'alexa: example Guid auto-change script block';
----GO
----DECLARE @ret int, @options int, @oldNoCountValue bit; 
----BEGIN TRANSACTION;
----EXEC @ret = sysVersionGuidLog; 
----IF (@ret = -1) 	BEGIN ROLLBACK TRANSACTION; SELECT @options = @@OPTIONS; IF ( (512 & @options) = 512) SET @oldNoCountValue = 1 ELSE SET NOCOUNT ON;	DELETE #VersionGuidTmp; IF @oldNoCountValue = 0 SET NOCOUNT OFF;	END
----ELSE BEGIN
------------------------------------------------------------------------------------------------------------------------------------
----    -- Insert actual new auto-changes here.
----    -- Don''t forget to remove all GO statements from your change script fragment.
----    -- Do not turn off XACT_ABORT.
----    -- Split to several consecutive versions your long changes, or the changes that require some their parts committed before apllying their further statements.
-------------------------------------------------------------------------------------------------------------------------------------
----	EXEC sysVersionGuidCommit;
----	COMMIT TRANSACTION;
----END
----GO
--) XXXXXXXX-XXXX-XXXX-XXXX-XXXXXXXXXXXX

/***************************************************************************
 ** Remove here procedures/functions no more used after AutoChangesGuid  ***
 ***************************************************************************/

--IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[TestFunction]') AND type in (N'FN', N'IF', N'TF', N'FS', N'FT'))
--DROP FUNCTION [dbo].[TestFunction]
GO

