IF EXIST .Dictionaries.sql       del /Q .Dictionaries.sql
REM copy /A Dictionaries\*.sql .Dictionaries.sql
FOR /R %%F IN ("Dictionaries\*.sql")  DO @CALL AppendScript.cmd %%F .Dictionaries.sql
