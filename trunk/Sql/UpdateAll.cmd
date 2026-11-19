SET RESULT_FILE=.UpdateAll.sql
IF EXIST .UpdateAll.sql       del /Q .UpdateAll.sql
@ECHO GO >%RESULT_FILE%

@IF EXIST AutoChanges.sql      copy /A %RESULT_FILE% + AutoChanges.sql     %RESULT_FILE%
@IF EXIST AutoChangesGuid.sql  copy /A %RESULT_FILE% + AutoChangesGuid.sql %RESULT_FILE%

@CALL Update.cmd
IF EXIST %RESULT_FILE%         copy /A %RESULT_FILE% + .Update.sql         %RESULT_FILE%

CALL Dictionaries.cmd
IF EXIST .Dictionaries.sql    copy /A %RESULT_FILE% + .Dictionaries.sql   %RESULT_FILE%

FOR /R %%F IN ("Jobs\*.sql") DO @CALL AppendScript.cmd %%F %RESULT_FILE%
REM copy /A %RESULT_FILE% + Jobs\*.sql %RESULT_FILE%

                FOR /R %%F IN ("ContentScript\*.sql")    DO @CALL AppendScript.cmd %%F %RESULT_FILE%
IF NOT ""=="%1" FOR /R %%F IN ("ContentScript\%1\*.sql") DO @CALL AppendScript.cmd %%F %RESULT_FILE%
REM START notepad %RESULT_FILE%