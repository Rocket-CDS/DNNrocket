# System Defaults
Default systems can be activated on the creation of a portal. 

Currently The RocketContentAPI system is automatically activated on creation of a portal.  Other systems can also be setup to be activated.

## 1. Get the Default Data
The default setup needs to be created.  The best way to do this is to setup a system in the RocketPortal and then get the "postnfo" data on the save event.  This can be done by putting a break on the paramcmd "rocketsystem_save".  

Once the data has been placed into an editor all the hidden fields can be removed.

Example from RocketIntra...
```
<genxml>
	<hidden>
		<systemkey><![CDATA[rocketintra]]></systemkey>
	</hidden>
	<defaultplugin><![CDATA[rocketintramenu]]></defaultplugin>
	<textbox>
		<htmllogo>
			<![CDATA[<div class='w3-blue w3-center w3-padding'  style="height:48px;">
<span class="material-icons">
restaurant_menu
</span>
Menu
</div>]]>
		</htmllogo>
	</textbox>
	<active><![CDATA[true]]></active>
	<emailon><![CDATA[false]]></emailon>
	<debugmode><![CDATA[false]]></debugmode>
	<checkbox>
		<active><![CDATA[true]]></active>
	</checkbox>
	<plugins list="true">
		<genxml>
			<hidden>
				<pluginkey><![CDATA[dashboard]]></pluginkey>
			</hidden>
			<checkbox>
				<active><![CDATA[false]]></active>
			</checkbox>
		</genxml>
		<genxml>
			<hidden>
				<pluginkey><![CDATA[rocketintramenu]]></pluginkey>
			</hidden>
			<checkbox>
				<active><![CDATA[true]]></active>
			</checkbox>
		</genxml>
	</plugins>
	<lang>
		<genxml>
			<plugins list="true">
				<genxml />
				<genxml />
			</plugins>
		</genxml>
	</lang>
</genxml>

```

**IMPORTANT: Save this XML in a file called "SystemInit.rules" in "/DesktopModules/DNNrocketModules/-systemkey-/Installation/SystemInit.rules"**

## 2 Create Activate
In the system code a paramcmd will need to be created (if it does not exist).

```
case "rocketintra_activate":
    strOut = RocketSystemSave();
    break;

```
This code will call the same save system code method.  Usually "RocketSystemSave()".  

The creation process will look for a file called "SystemInit.rules", it it exists the XML data will be sent to the "rocketintra_activate" command.  This command will save the pre-defined data in the XML file.  

## 3 Add the SystemKey
Once the above has been done you can add the required systemkey to the list of default systems to be automatically activated.  
The list of auto activate systems is in the "/DesktopModules/DNNrocket/RocketPortal/WebsiteBuilds/SystemDefaults.rules" file.

Default list...
```
<genxml>
	<systems list="true">
		<genxml>
			<systemkey>rocketcontentapi</systemkey>
		</genxml>
	</systems>
</genxml>
```
List with RocketIntra added...
```
<genxml>
	<systems list="true">
		<genxml>
			<systemkey>rocketcontentapi</systemkey>
		</genxml>
		<genxml>
			<systemkey>rocketintra</systemkey>
		</genxml>
	</systems>
</genxml>
```
With Langauges added...
```
<genxml>
	<systems list="true">
		<genxml>
			<systemkey>rocketcontentapi</systemkey>
		</genxml>
		<genxml>
			<systemkey>rocketintra</systemkey>
		</genxml>
	</systems>
	<languages>
		<genxml>
			<culturecode>en-US</culturecode>
		</genxml>
		<genxml>
			<culturecode>fr-FR</culturecode>
		</genxml>
		<genxml>
			<culturecode>en-GB</culturecode>
		</genxml>
		<genxml>
			<culturecode>it-IT</culturecode>
		</genxml>
		<genxml>
			<culturecode>es-ES</culturecode>
		</genxml>
	</languages>
</genxml>

```










