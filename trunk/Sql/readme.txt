1) Open folder .\Sql\

2) Run UpdateAll.cmd - it will create .UpdateAll.sql

3) Run script .UpdateAll.sql using SQL Server Managmet Studio or sqlcmd (Example: sqlcmd -b -I -U gdg -P *** -S (local)\ -d gdg_database -i ".UpdateAll.sql" -o "Logfile.txt")
Do not forget to select the correct database name!!!
SQL user must have SQLAgentUserRole, SQLAgentReaderRole, SQLAgentOperatorRole permissions on MSDB DB for Jobs creation/update
	
At last, if possible (it may take some time and take some server resources), run the script optimize database: Maintenance\rebuildIndexStatistics.sql

