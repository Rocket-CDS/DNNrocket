# AutoLogin

### Introduction
This module allows a user from 1 portal to login automatically to another portal if their user exists on the new portal.  

**The "login" portal MUST exist in the same DNN installation.**  

### Installation
Add to the web.config in "configuration/modules" of the installation.

```
      <add name="AutoLogin" type="DNNrocketAPI.Components.AutoLogin, DNNrocketAPI" preCondition="managedHandler" />
```
### Login Method
Firstly a user with the same username as the current portal must exist on the new/next portal.  
The portal url must be entered with a query stirng parameter of "autologin".  The value of this param will be a temporay record by [GUIDKey] found in the [DNNrocketTemp] table.  
This record is created by the redirect login command and will contain the login information.  
Each file can only be used once and has a timeout.  

### Error messages and debuging
All output can be found in the log file for that day. (\Portals\_default\Logs\*)  The log system will need to be in INFO mode, change the log config: DotNetNuke.log4net.config  



