# Debug API

The API input can be exported to the filesystem when command are written.  This is used to get a Json formatted data file.  

In the session params pass the "debugapi" parameter.

```
        simplisity_setParamField("debugapi", true);
```

This can also be set in the "s-fields" of the element.

```
s-fields='{"debugapi":"true"}'
```

The output json can be found in the "/Portals/_default/RocketLogs/" folder with a file name of the command.  
There will be 2 files, 1 for the param data "paramjson" and 1 for the post data "inputjson".

# log4net
Debug Message can be output from the server code by using the LogUtils class.
```
LogUtils.LogSystem("ERROR GetGroupCache(string groupid) : " + ex.Message);
```

Output messages are output to the log4net log file.  
**The Logging must be activated in the Global settings of RocketCDS**  
Usually this file is set to only output errors, to view rocket log you must set the "log4net/root/level" setting to "INFO" or "ALL".  
```
<?xml version="1.0" encoding="utf-8" ?>
<log4net>
  <appender name="RollingFile" type="log4net.Appender.RollingFileAppender">
    <file value="Portals/_default/Logs/" />
    <datePattern value="yyyy.MM.dd'.log.resources'" />
    <rollingStyle value="Date" />
    <staticLogFileName value="false" />
    <appendToFile value="true" />
    <maximumFileSize value="10MB" />
    <maxSizeRollBackups value="5" />
    <lockingModel type="log4net.Appender.FileAppender+MinimalLock"/>
    <layout type="log4net.Layout.PatternLayout">
      <conversionPattern value="%d{yyyy-MM-dd HH:mm:ss.fffzzz} [%property{log4net:HostName}][D:%property{appdomain}][T:%thread][%level] %logger - %m%n" />
    </layout>
  </appender>
  <root>
    <level value="INFO" />
    <appender-ref ref="RollingFile" />
  </root>
</log4net>

```
# Error Logging
RocketrCDS can output errors to the standard DNN Event Log.
```
LogUtils.LogException(ex);
```

