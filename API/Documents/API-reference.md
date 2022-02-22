# API reference for RocketCDS

RocketCDS is an application that delivers content via web API.

#### RocketCDS Systems
RocketCDS can do many functions like:  e-commerce, html display, catalog systems, admin systems, etc.  
Each System is a plugin application that works with RocketCDS to deliver content.  You can think of each system as an independant application.  Each system uses the CDS endpoint, but usually doesn't know about any other system.



##### Data POST Interface:

<table>
<tr>
    <th>Key</th>
    <th>Description</th>
    <th>format</th>
</tr>
<tr>
    <td>context.Request.Form["cmd"]</td>
    <td>Command to be executed on RocketCDS.</td>
    <td>Json String</td>
</tr>
<tr>
    <td>context.Request.Form["inputjson"]</td>
    <td>Used as a data post.  Simplisity.js created a Json string from the HTML page input fields.</td>
    <td>Json String</td>
</tr>
<tr>
    <td>context.Request.Form["paramjson"]</td>
    <td>Used as a parameter post.  Simplisity.js created a Json string from command attributes and sessions values.</td>
    <td>Text (Base64)</td>
</tr>
<tr>
    <td>systemkey</td>
    <td>This gets added to the RocketCDS Endpoint URL, so the CDS can call the correct system.</td>
    <td>Text (Base64)</td>
</tr>
<tr>
    <td>Security Code</td>
    <td>The "Security Code" that was used to register the plugin should be posted.  RocketCDS expects the "Security Code" to be returned in base64 and char decimal encoding.</td>
    <td>Text (Base64 - char decimal encoding)</td>
</tr>
</table>

##### Data Response Interface:

The data is returned as the body, with extra data returned in the header Array.

<table>

<tr>
    <th>Key</th>
    <th>Description</th>
    <th>format</th>
</tr>

<tr>
    <td>remote-firstheader</td>
    <td>HTML text injected in the FIRST posiion of the header</td>
    <td>Text (Base64)</td>
</tr>

<tr>
    <td>remote-lastheader</td>
    <td>HTML text injected in the LAST posiion of the header</td>
    <td>Text (Base64)</td>
</tr>

<tr>
    <td>remote-seoheader</td>
    <td>XML format for SEO data. (See "SEO Data")</td>
    <td>XML (Base64)</td>
</tr>

<tr>
    <td>remote-return</td>
    <td>HTML that is returned as the default data response body.  (The ResponseStream is used for legacy reasons)</td>
    <td>Text (UTF8)</td>
</tr>

<tr>
    <td>remote-cache</td>
    <td>A flag to indicate if the client module can cache data.</td>
    <td>Bool (Base64)</td>
</tr>

<tr>
    <td>remote-json</td>
    <td>Json string returned for client side data objects.</td>
    <td>Text (Base64)</td>
</tr>

<tr>
    <td>remote-settingsxml</td>
    <td>An XML string of settings.  Each system may have a different set of XML settings.</td>
    <td>XML (Base64)</td>
</tr>

</table>
 

##### Default commands:
Each system has many "cmd" parameters, but there are a number of standard ones that all systems can use.  
<table>

<tr>
    <th>cmd</th>
    <th>Description</th>
</tr>

<tr>
    <td>"remote_publicview"</td>
    <td>The standard public display.</td>
</tr>
<tr>
    <td>"remote_edit"</td>
    <td>Returns the edit page for data entry.</td>
</tr>
<tr>
    <td>"remote_settings"</td>
    <td>Returns the settings fields for the plugin.</td>
</tr>
<tr>
    <td>"remote_editoption"</td>
    <td>Some systems can have direct Edit of the record data.  This cmd identifies if they do. (Returns a string boolean)</td>
</tr>
<tr>
    <td>"dataclients_register"</td>
    <td>This cmd registers a data client.  When a plugin is attached to the RocketCDS we should register a "Data Cient".  RocketCDS expects the "Security Code" to be returned in base64 and char decimal encoding.</td>
</tr>
</table>



##### SEO Data:

**remote-seoheader** SEO data is returned as a standard XML format.  
NOTE: SEO data return is optional.
```
<item>
	<genxml>
		<title><![CDATA[]]></title>
		<description><![CDATA[]]></description>
		<keywords><![CDATA[]]></keywords>
	</genxml>
</item>
```