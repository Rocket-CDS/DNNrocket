﻿# RocketComm

This project deals with client communication between a RocketCDS client and a RocketCDS server.  

RocketComm.dll and Simplisity.dll can be referenced by a .Net application and offers a communication link with RocketCDS.

**Things you should know before reading**
<table>
<tr>
    <td><b>RocketCDS</b></td>
    <td>The online Central Data Service.  This is the delivery system for content using a web API.</td>
</tr>
<tr>
    <td><b>system</b></td>
    <td>The CDS is made of multiple applications doing different things.  Each application is called a "system"</td>
</tr>
<tr>
    <td><b>command</b></td>
    <td>The API "command" that tells RocketCDS to do something.</td>
</tr>
</table>


#### Server to Server

RocketComm is designed to d a server to server call to a RocketCDS application.  


#### remoteParams

The "remoteParams" data is an XML string.  This data is used by RocketCDS, some data is optional, different systems required different parameters.

**Example (Minimum):**
```
<genxml>
  <remote>
    <engineurl>http://dev2.toasted2.com</engineurl>
    <securitykey>Gw2Rub30QEKDSdgsiouCmigKSumAP3---8</securitykey>
  </remote>
</genxml>

```
**Example (Real):**
```
<genxml>
  <remote>
    <apiurl>/Desktopmodules/CDSviewerDNN/apihandler.ashx</apiurl>
    <moduleref>uuzCPWBEu4GGtwsZVe3w407</moduleref>
    <culturecode><![CDATA[fr-FR]]></culturecode>
    <browsersessionid><![CDATA[4ce02d83-0224-4a98-9615-a0228dad5c7f-1645145190124]]></browsersessionid>
    <browserid><![CDATA[f755495f-d923-4b8e-8ecc-f4629693e409-1638609443941]]></browserid>
    <serviceref>3GB5wUm33EGsu57FGGeSA</serviceref>
    <systemkey>rocketcatalog</systemkey>
    <engineurl>http://dev2.toasted2.com</engineurl>
    <securitykey>Gw2Rub30QEKDSdgsiouCmigKSumAP3---8</securitykey>
    <securitykeyedit>k8M46skgkkaWQN8scb5WugJyumAP3---8</securitykeyedit>
    <culturecodeedit><![CDATA[en-US]]></culturecodeedit>
    <org><![CDATA[RocketAppThemes-W3-CSS]]></org>
    <urlparams>
      <TabId><![CDATA[39]]></TabId>
      <language><![CDATA[fr-FR]]></language>
    </urlparams>
    <pageurl><![CDATA[http://dev.testinstall.site/fr-fr/cdsviewer]]></pageurl>
    <searchtext><![CDATA[]]></searchtext>
    <checkboxfilter325><![CDATA[false]]></checkboxfilter325>
    <checkboxfilter330><![CDATA[false]]></checkboxfilter330>
    <pagesize><![CDATA[8]]></pagesize>
    <orderbyref><![CDATA[]]></orderbyref>
  </remote>
</genxml>
```

As you can see from the above XML.  The only required field is

<table>
<tr>
    <td><b>engineurl</b></td>
    <td>This is the URL of the RocketCDS</td>
</tr>
<tr>
    <td><b>securitykey</b></td>
    <td>This key is generated by the RocketCDS system and is used to allow connections to secure commands. (System dependant, but most systems will not work without the securitykey.) </td>
</tr>
</table>

Of course just using the API URL will do nothing.  It needs to have a command (cmd) parameters to get anything out of the CDS.  The command is passed to RocketComm via:  
```
CallRedirect(string cmd)
```
As you can see from the **real** example, most systems want more data.  Each system may require different data.  To understand what data is required by a system you must look at the system API documentation.  

Most of the data passed to the RocketCDS is session data, page, pagesize, searchtext, etc.  
This session data is kept on the browser in local storage and a cookie, by simplisity.js.  In all cases this cookie is read and the values added to the XML.  
A pageurl is also added, so any links automatically created by the CDS always point to the correct page.  
The Query String paramaters are also added to the XML in the "genxml/remote/urlparams/*" xpath.
Lastly it is impostant to pass the current culturecode to the CDS, so any output is generated in the correct langauge.  (NOTE: there is both a "culturecode" and a "culturecodeedit")

*The method in RocketComm to call the CDS is:*
```
CallRedirect(string cmd, string jsonPost = "", string jsonParams = "", string systemKey = "")
```

### JsonPost (optional)
A json string generated by simplisity.js and posted via the "s-post" command attributes (https://www.simplisityjs.org/)

### JsonParams (optional)
A json string generated by simplisity.js and posted via the "s-fields" command attributes.  Often this will contain the session data and some other fields. (https://www.simplisityjs.org/)

### SystemKey  (optional)

If the system you wish to call is different form the remote data default (In the XML), you can pass the systemkey required.


#### Dependancies:

Simplsity.dll - Simplisity is used by RocketComm to easier deal with XML.  It contains some powerful XML methods, but its use could be avoided.

