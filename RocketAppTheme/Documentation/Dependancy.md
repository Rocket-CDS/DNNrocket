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

### Do NOT inject with Skin
In some cases the CSS file is injected with the Skin.  In these cases we can choose to ignore the injection by using the name of the skin the the skinignore node.
```
<ignoreonskin>rocketw3,anotherskin></ignoreonskin>
```
This supports CSV list and will check if SkinSrc contains any of the skin names.  

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
		<ignoreonskin>rocketw3,anotherskin></ignoreonskin>
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
		<file>view.cshtml</file>
		<name>List View</name>
		<cmd>list</cmd>
	</genxml>
	<genxml>
		<file>hozcatmenu2lvl.cshtml</file>
		<name>Horizontal Category Menu (2 levels)</name>
		<cmd>catmenu</cmd>			
	</genxml>
</moduletemplates>
```

file = File name of the template.  
name = Friendly name.  
cmd = The data command that will be used.  

### cmd values RocketDirectory
listdetail = Display detai or list by using "articleid" param.  (default)  
list = Same as listdetail. (Deprecated)  
listonly = list data only   
detailonly = article data only  
catmenu = Category Data  
satellite = List data without populating it.  The call for data should be made in the razor template.  



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
## QueryParams
With the directory system you may have a list and detail structure.  

### Format
```
<queryparams list="true">
	<genxml>
		<queryparam>articleid</queryparam>
		<tablename>rocketdirectoryapi</tablename>
		<systemkey>rocketnewsapi</systemkey>
		<datatype>article</datatype>
	</genxml>
</queryparams>
```
*queryparam = The URL param that will be looked for.  The value of which is used to read records form the Database.*  
*tablename = The name of the table that will be used.  Usually "rocketdirectoryapi" or "rocketecommerceapi".*  
*systemkey = The systemkey for the queryparam.*

The QueryParams do 2 important actions when the detail page needs to be seen.  **SEO** and **Activation of the Detail Display**.  

#### Categories
The category article list query param is also defined in the dependancy file. 
```
<queryparams list="true">
	<genxml>
		<queryparam>catid</queryparam>
		<tablename>rocketdirectoryapi</tablename>
		<systemkey>rocketnewsapi</systemkey>
		<datatype>category</datatype>
	</genxml>
</queryparams>
```
This allow for each AppTheme to have it's own categoey menu injected into the DDRMenu.


### SEO
The detail should contain SEO data in the header.  The SEO data is read by using a URL parameter, this paramater is defined in the dependacies file.  Saving the directory settings will also update the Page data so the Meta.ascx can capture the detail data with an ItemId.  

The data record must have some default field names that will be used for SEO.  

metatitle = "genxml/lang/genxml/textbox/seotitle" or "genxml/lang/genxml/textbox/articlename"  
metadescription = "genxml/lang/genxml/textbox/seodescription" or "genxml/lang/genxml/textbox/articlesummary"  
metatagwords = "genxml/lang/genxml/textbox/seokeyword"  

*NOTE: Keywords are no longer used by search engines and do not have to be included.*

It will also look for the first image called "imagepatharticleimage" or "imagepathproductimage".

These field names are the default names used in the Shared Templates.  If you are not using the shared templates you must use the same names to make the SEO header work.  

### DDRMenu Provider
The categories can be added to the menu by the menu provider.  (See MenuManipulator documentation)

```
<menuprovider>
	<genxml>
		<assembly>RocketDirectoryAPI</assembly>
		<namespaceclass>RocketDirectoryAPI.Components.MenuDirectory</namespaceclass>
		<systemkey>rocketblogapi</systemkey>
	</genxml>
</menuprovider>
```


### Activation of the detail display
The detail page is displayed in a module by using the itemid in the URL.  The name of the query param for the itemid is defined in the dependacy file.   
The systemkey is also defined, so that only modules using the deifned systemkey are activated for detail.  
In some situation multiple systems/module may want to display the detail.  This can be done but it will affect the SEO, only the first detail SEO will be added to the page.  

## DNN search 
RocketContent can use the dependacy file to define what field data should be included in the DNN search.  
*NOTE: Other Rocket systems like RocketDirectory have deifned fields for the search and therefore does not use this section*
### Dependacy File Section (example)
```
<searchindex list="true">
	<genxml>
		<searchtitle>genxml/lang/genxml/textbox/title</searchtitle>
		<searchdescription>genxml/lang/genxml/textbox/richtext</searchdescription>
		<searchbody>genxml/lang/genxml/textbox/richtext</searchbody>
	</genxml>
</searchindex>
```
The dependacy section only has 1 entry, a CSV list can be used to concatinate multile fields.
### AdminRow.cshtml (example)
```
    <div class="w3-row-padding w3-section">
        <div class="w3-row-padding">
            <label>@ResourceKey("DNNrocket.heading")</label>&nbsp;@EditFlag(sessionParams)
            @TextBox(info, "genxml/lang/genxml/textbox/title", " id='title' class='w3-input w3-border' autocomplete='off' ", "", true, 0)
        </div>
    </div>

    <div id="html" class='w3-row sectionname' style="display:none">
        <div class='w3-col m12 w3-padding'>
            @EditFlag(sessionParams)
            <div class='w3-col m12'>
                @CKEditor4(info, "genxml/lang/genxml/textbox/richtext", true)
            </div>
            <div class='w3-col' style='width:0px;height:600px;'></div>
        </div>
    </div>
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
	<searchindex list="true">
		<genxml>
			<searchtitle>genxml/lang/genxml/textbox/title</searchtitle>
			<searchdescription>genxml/lang/genxml/textbox/richtext</searchdescription>
			<searchbody>genxml/lang/genxml/textbox/richtext</searchbody>
		</genxml>
	</searchindex>
</genxml>


```

