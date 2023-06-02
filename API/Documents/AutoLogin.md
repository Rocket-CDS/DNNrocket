# AutoLogin

### Introduction
This module allows a user from 1 portal to login automatically to another portal if there user exists on the new portal.  

The new portal MUST exist in the same DNN installation.  

### Installation

Move assembly "AutoLogin" to bin folder and then add to the web.config in "configuration/modules" of the installation.

```
      <add name="AutoLogin" type="DNNrocketAPI.Components.AutoLogin, DNNrocketAPI" preCondition="managedHandler" />
```
### Login Method
Firstly a user with the same username as the current portal must exist on the new/next portal. 
The portal url must be entered with a query stirng parameter of "autologin".  The value of this param will be the coded name of a temporay file found on the file system of the website (not accessible to the public).  
This file is created by the redirect login command and will contain the login information.  Each file can only be used once and has a timeout of 30 seconds.  

### Error messages and debuging
All output can be found in the RocketLog log file for that day. (\Portals\_default\RocketLogs\*)


