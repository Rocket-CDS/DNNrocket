# Partial and Shared Templates
Templates can be inject within other templates to allow partial and shared templates.  
There are 2 methods to do this.  

## Render Token
The template is written as a normal templates and then injected into another template by using a razor token to compile and include it.  

```
public IEncodedString RenderTemplate(string razorTemplateName, AppThemeRocketApiLimpet appThemeSystem, SimplisityRazor model, bool cacheOff = false)
public IEncodedString RenderTemplate(string razorTemplateName, AppThemeSystemLimpet appThemeSystem, SimplisityRazor model, bool cacheOff = false)
public IEncodedString RenderTemplate(string razorTemplateName, AppThemeDNNrocketLimpet appThemeSystem, SimplisityRazor model, bool cacheOff = false)
public IEncodedString RenderTemplate(string razorTemplateName, AppThemeLimpet appTheme, SimplisityRazor model, bool cacheOff = false)
public IEncodedString RenderTemplate(string razorTemplateName, string moduleRef, AppThemeLimpet appTheme, SimplisityRazor model, bool cacheOff = false)
public IEncodedString RenderTemplate(string razorTemplate, SimplisityRazor model, bool debugMode = false)
```
This is the easiest method.  The  razor template is built as normal, compiled on call and included.    
However, this type of inclusion uses compile on call, which can lead to multiple compiles for a single template and slower performace. 

**NOTE: Circular calls will crash processing.**

## INJECT Token Replacement
This is a replacement method, so the template injects the sub-template code and then only compiles once.  This gives better performace than the Render Token method.  
```
[INJECT:<AppTheme object key>,<template name>]
```
Example:
```
[INJECT:appthemesystem,AppThemeFields.cshtml]
```
Because this method does not require the top section of a razor template (Data definition) it does require a little more thought to get it working.  
The sub-template is built in the normal way, so intelli-sense work.  But a replacement token is added to the template so only the text AFTER the token is included in the parent template.

The token for spliting the template is case sensitive:
```
<!--inject-->
```
If no inject  token is found the first "<div" (lower case) is used.

Names for the AppTheme object key can differ.  The AppTheme object key can be anything, it depends what the developer has called it.  
However, the convension is the following names.  
```
apptheme
appthemesystem

appthemedirectory
appthemedirectorydefault
appthemedefault
appthemeview
appthemeplugin
```
*You will need to look at the system being used for the true name.*  

Example template call:
```
@inherits RocketPortal.Components.RocketPortalTokens<Simplisity.SimplisityRazor>
@AddProcessData("resourcepath", "/DesktopModules/DNNrocket/api/App_LocalResources/")
<link rel="stylesheet" href="/DesktopModules/DNNrocket/API/Themes/config-w3/1.0/css/dnnrocket-theme.css">
<div class="w3-container w3-center w3-margin">
    INJECT TEST
</div>
[INJECT:appthemesystem,injecttest1.cshtml]
```
Example sub-template 1:
```
@inherits RocketPortal.Components.RocketPortalTokens<Simplisity.SimplisityRazor>
@AddProcessData("resourcepath", "/DesktopModules/DNNrocket/api/App_LocalResources/")
<!--inject-->
<link rel="stylesheet" href="/DesktopModules/DNNrocket/API/Themes/config-w3/1.0/css/dnnrocket-theme.css">
<div class="w3-container w3-center w3-margin">
    INJECT TEST 1
</div>
```
Only the text after the inject token is included.  
It is expected that all data objects required will be included in the calling template.  

Example sub-template 2:
```
@inherits DNNrocketAPI.render.DNNrocketTokens<Simplisity.SimplisityRazor>
@AddProcessData("resourcepath", "/DesktopModules/DNNrocket/images/App_LocalResources/")
@{
    var appTheme = (AppThemeLimpet)Model.GetDataObject("apptheme");
}
<!--inject-->
@{
    if (appTheme != null)
    {
        var x = "Example";
    }
}
<div class="w3-container w3-center w3-margin">
    INJECT TEST 1
</div>
```
The above example shows how razor code can be included in the sub-template.  
Circular references to sub-templates will display an error on the output page.    
Templates that do not exist will show the inject token on the output page.  

