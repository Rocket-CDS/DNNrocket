# AppTheme Dependancies

AppThemes often have data and files that they are dependant on.  There is a file in the AppTheme that can define those dependancies.  

It is found in the subfolder "dep", in XML format, with an extension of ".dep".  
```
/DesktopModules/RocketThemes/#AppThemeProjectName#/#systemkey#.#AppThemeName#/#version#/dep
```
And has different sections.  Not all sections are required for some AppThemes or systems.

## Dependancy File Section

Dependancies are CSS or JS files that are required for the theme to function.  The dependancy system stops duplicate CSS and JS files being injected.  
Dependancies ONLY work on the Front View of the website.  For Admin View use "AdminFirstHeader.cshtml" or "AdminLastHeader.cshtml" templates.

### ECO Mode
ECO Mode is a module level flag setting that stops the dependancy system from injecting any dependancies with "ecofriendly" set to false.  

**Tokens**  

\{domainurl} = Protocol and domain URL of RocketAPI.  
\{appthemefolder} = Relitive URL of the from view AppTheme.  
\{appthemesystemfolder} = Relitive URL of the system.  
\{jquery} = Injects Jquery into the view page.  

*Example:*
```
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
```

## Module Templates
A ModuleTemplate is part of an Apptheme.  It defines what templates and commands that can be used on a Module at startup.  

The XML contains meta data that is used by the system, there is no UI in the apptheme editor, this data is static to the AppTheme.  

*Example:*
```
<moduletemplates list="true">
	<genxml>
		<file>view.cshtml></file>
		<name>List View></name>
		<cmd>list></cmd>
	</genxml>
	<genxml>
		<file>hozcatmenu2lvl.cshtml></file>
		<name>Horizontal Category Menu (2 levels)></name>
		<cmd>catmenu></cmd>			
	</genxml>
</moduletemplates>
```

file = File name of the template.  
name = Friendly name.  
cmd = The data command that will be used.  

## Admin Panel Interfaces

An AppTheme for "rocketdirectoryapi" system (or a wrapper system) can select which options are available in the Admin Panel of the system.  

By default options are shown, if you want to hide options then you need to have a "adminpanelinterfacekeys" section in the dependancy file of the AppTheme.  

there are 5 options that can be shown.
- Articel Admin
- Category Admin
- Property Admin
- Settings Admin
- Portal System Admin  (Host only)

The superuser will always see all admin options.

All activated plugins will be shown on the Admin Panel.

Setting the show node to "False" will hide the option.

*Example:*
```
<adminpanelinterfacekeys list="true">
	<genxml>
		<interfacekey>articleadmin</interfacekey>
		<show>true</show>
	</genxml>
	<genxml>
		<interfacekey>categoryadmin</interfacekey>
		<show>true</show>
	</genxml>
	<genxml>
		<interfacekey>propertyadmin</interfacekey>
		<show>true</show>
	</genxml>
	<genxml>
		<interfacekey>settingsadmin</interfacekey>
		<show>true</show>
	</genxml>
	<genxml>
		<interfacekey>rocketdirectoryadmin</interfacekey>
		<show>true</show>
	</genxml>
</adminpanelinterfacekeys>
```
### QueryParams
With the directory system you may have a list and detail structure.  
The detail should contain SEO data in the header.  The SEO data is read by using a URL parameter, this paramater is defined in the dependacies file.  Saving the directory settings will also update the Page data so the Meta.ascx can capture the detail data with an ItemId.  
```
<queryparams list="true">
	<genxml>
		<queryparam>articleid</queryparam>
		<tablename>rocketdirectoryapi</tablename>
	</genxml>
</queryparams>
```
queryparam = The URL param that will be looked for.  The value of which is used to read records form the Database.  
tablename = The name of the table that will be used.  Usually "rocketdirectoryapi" or "rocketecommerceapi".

The data record must have some default field names that will be used for SEO.  

metatitle = "genxml/lang/genxml/textbox/seotitle" or "genxml/lang/genxml/textbox/articlename"  
metadescription = "genxml/lang/genxml/textbox/seodescription" or "genxml/lang/genxml/textbox/articlesummary"  
metatagwords = "genxml/lang/genxml/textbox/seokeyword"  

It will also look for the first image called "imagepatharticleimage".

These field names are he default names used in the Shared Templates.  If you are not using the shared templates you must use the same names to make the SEO header work.  

```
_articleTable = paramDict.Value;
articleid = Convert.ToInt32(strParam);
_dataRecordTemp = objCtrl.GetInfo(articleid, DNNrocketUtils.GetCurrentCulture(), _articleTable);
if (_dataRecordTemp != null)
{
    if (_dataRecordTemp.GetXmlProperty("genxml/lang/genxml/textbox/seotitle") != "")
        _metatitle = _dataRecordTemp.GetXmlProperty("genxml/lang/genxml/textbox/seotitle");
    if (_metatitle == "") _metatitle = _dataRecordTemp.GetXmlProperty("genxml/lang/genxml/textbox/articlename");

    if (_dataRecordTemp.GetXmlProperty("genxml/lang/genxml/textbox/seodescription") != "")
        _metadescription = _dataRecordTemp.GetXmlProperty("genxml/lang/genxml/textbox/seodescription");
    if (_metadescription == "") _metadescription = _dataRecordTemp.GetXmlProperty("genxml/lang/genxml/textbox/articlesummary");

    if (_dataRecordTemp.GetXmlProperty("genxml/lang/genxml/textbox/seokeyword") != "")
        _metatagwords = _dataRecordTemp.GetXmlProperty("genxml/lang/genxml/textbox/seokeyword");

    var portalContentRec = DNNrocketUtils.GetPortalContentRecByRefId(_dataRecordTemp.PortalId, _dataRecordTemp.GetXmlProperty("genxml/systemkey"), _articleTable);
    _articleDefaultTabId = portalContentRec.GetXmlPropertyInt("genxml/detailpage");
    if (_articleDefaultTabId == 0) _articleDefaultTabId = PortalSettings.ActiveTab.TabID;

    string[] urlparams = { "articleid", articleid.ToString(), DNNrocketUtils.UrlFriendly(_metatitle) };
    var ogurl = DNNrocketUtils.NavigateURL(_articleDefaultTabId, _dataRecordTemp.Lang, urlparams);

    metaList.Add(BuildMeta("", "og:type", "article"));
    metaList.Add(BuildMeta("", "og:title", _metatitle.Truncate(150).Replace("\"", "")));
    metaList.Add(BuildMeta("", "og:description", _metadescription.Truncate(250).Replace("\"", "")));
    metaList.Add(BuildMeta("", "og:url", ogurl));
    var imgRelPath = _dataRecordTemp.GetXmlProperty("genxml/imagelist/genxml[1]/hidden/imagepatharticleimage").ToString();
    if (imgRelPath != "") metaList.Add(BuildMeta("", "og:image", Request.Url.GetLeftPart(UriPartial.Authority).TrimEnd('/') + "/" + imgRelPath.TrimStart('/')));

    articleMeta = true;
}
```


## Example of a full dependancy file

*Example:*
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
	<moduletemplates list="true">
		<genxml>
			<file>view.cshtml></file>
			<name>List View></name>
			<cmd>list></cmd>
		</genxml>
		<genxml>
			<file>hozcatmenu2lvl.cshtml></file>
			<name>Horizontal Category Menu (2 levels)></name>
			<cmd>catmenu></cmd>			
		</genxml>
	</moduletemplates>
	<adminpanelinterfacekeys list="true">
		<genxml>
			<interfacekey>articleadmin</interfacekey>
			<show>true</show>
		</genxml>
		<genxml>
			<interfacekey>categoryadmin</interfacekey>
			<show>true</show>
		</genxml>
		<genxml>
			<interfacekey>propertyadmin</interfacekey>
			<show>true</show>
		</genxml>
		<genxml>
			<interfacekey>settingsadmin</interfacekey>
			<show>true</show>
		</genxml>
		<genxml>
			<interfacekey>rocketdirectoryadmin</interfacekey>
			<show>true</show>
		</genxml>
	</adminpanelinterfacekeys>
	<queryparams list="true">
		<genxml>
			<queryparam>articleid</queryparam>
			<tablename>rocketdirectoryapi</tablename>
		</genxml>
	</queryparams>
</genxml>


```

