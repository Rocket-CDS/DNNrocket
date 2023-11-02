# Dependancy

Dependancies are CSS or JS files that are required for the theme to function.  The dependancy system stops duplicate CSS and JS files being injected.  
Dependancies ONLY work on the Front View of the website.  For Admin View use "AdminFirstHeader.cshtml" or "AdminLastHeader.cshtml" templates.

**Tokens**  

\{domainurl} = Protocol and domain URL of RocketAPI.  
\{appthemefolder} = Relitive URL of the from view AppTheme.  
\{appthemesystemfolder} = Relitive URL of the system.  
\{jquery} = Injects Jquery into the view page.  

Example:
```
<genxml>
	<deps list="true">
		<genxml>
			<ctrltype><![CDATA[js]]></ctrltype>
			<url><![CDATA[{jquery}]]></url>
		</genxml>
		<genxml>
			<ctrltype><![CDATA[css]]></ctrltype>
			<url><![CDATA[/DesktopModules/DNNrocket/css/w3.css]]></url>
		</genxml>
		<genxml>
			<ctrltype><![CDATA[css]]></ctrltype>
			<url><![CDATA[{domainurl}{appthemefolder}/css/HtmlContent.css]]></url>
		</genxml>
	</deps>
</genxml>
```
