REM  copy /A Views\*.sql + Functions\*.sql + Procedures\*.sql + Triggers\*.sql .Update.sql

SETLOCAL
SET RESULT_FILE=.Update.sql
@ECHO GO >%RESULT_FILE%
FOR /R %%F IN ("Views\*.sql")   DO @CALL AppendScript.cmd %%F %RESULT_FILE%
FOR /R %%F IN ("Functions\*.sql")  DO @CALL AppendScript.cmd %%F %RESULT_FILE%
FOR /R %%F IN ("Procedures\*.sql") DO @CALL AppendScript.cmd %%F %RESULT_FILE%
FOR /R %%F IN ("Triggers\*.sql")   DO @CALL AppendScript.cmd %%F %RESULT_FILE%
ECHO %SCRIPT_FILE%
REM START NOTEPAD %RESULT_FILE%
ENDLOCAL