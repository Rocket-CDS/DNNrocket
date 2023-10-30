# ModuleTemplate  (modt)

A ModuleTemplate is part of an Apptheme.  It defines what templates and commands that can be used on a Module.

It is defined in the Apptheme (e.g. "/AppThemeName/1.0/modt/filename.modt") with a ".modt" file extension and is selected in the module settings.

The file contains meta data that is used by the system, there is no UI in the apptheme editor, this data is static to the AppTheme.

```
<genxml>
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
</genxml>
```

file = File name of the template.  
name = Friendly name.  
cmd = The data command that will be used.  


