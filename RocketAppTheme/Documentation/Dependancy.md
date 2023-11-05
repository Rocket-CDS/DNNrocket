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
			<ctrltype>js></ctrltype>
			<url>{jquery}</url>
			<ecofriendly>true</ecofriendly>
		</genxml>
		<genxml>
			<ctrltype>css></ctrltype>
			<url>/DesktopModules/DNNrocket/css/w3.css</url>
			<ecofriendly>true</ecofriendly>
		</genxml>
		<genxml>
			<ctrltype>css</ctrltype>
			<url>{domainurl}{appthemefolder}/css/HtmlContent.css></url>
			<ecofriendly>true</ecofriendly>
		</genxml>
	</deps>
</genxml>
```
