Deployment manual
--------------------

Changes in the App.Config (or web.config)
1. Fix paths in <nlog> section
2. Add your connection string in <connectionStrings>, give it your name, ex. EurekaStrokeEntities
3. Fix UploaderPath path in <appSettings>
4. Specify the name of your connection string, ex. EurekaStrokeEntities in section <applicationSettings>    <Core.Properties.Settings>


Execution
-------------

If you run upload and receive message that file is too big, then you can run this task
with a separate PendingUploadsRunner utility.

Run PendingUploadsRunner, it will pickup all pending tasks and parse big files

The file is considered as too big for online mode if number of lines exceeds the threshold max number.
This threshold max number of lines is a confugurable value (now 10). See MaxLinesInOnlineMode setting.

