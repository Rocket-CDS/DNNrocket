# Methods
Simplisty has a few methods which are called to make things happen.

```
$(document).simplisityStartUp(string apiurl)
```
This is the main activation for Simplisity. It searches the webpage for any html elements with "simplisity_panel" class. Simplisity will then process the command attributes attached to the html element.

Options can also be passed. {systemkey: 'dnnrocket',activatepanel: true, overlayclass: 'w3-overlay'}.

**systemkey**: You can pass a default systemkey to use if non are specifiecd on the simplisity_panel. This stops the need to specify the systemkey on each simplisity_panel.  
**activatepanel**: Activate the simplsity_panels. Default:true  
**overlayclass**: Defines a overlayclass to use, for when the server is called. Default: w3-overlay  
**debug**: Puts the JS in debug mode. The browser console log will contain data to help debuging.  
*Example:*
```
$(document).simplisityStartUp('/Desktopmodules/dnnrocket/api/rocket/action', { systemkey: 'rocketexample', usehistory: true, overlayclass:'w3-overlay w3-theme' });
```
```
$('#mycontainer').activateSimplisityPanel();
```

Each "simplisity_panel" can be called individually.
```
$('#mycontainer').getSimplisity(string s-cmdurl, string s-cmd, string s-fields,string s-after)
```
This method will do an individual post to the server API and a return of data to the selected element.
```
simplisity_callserver(element, cmdurl, returncontainer, reload)
```
The JavaScript function to post to the server can also be called directly. This works by passing the html element and the command attributes on the element tell the simplisity code what to do.

If we have already called simplisity server then the last command url used will be activate. Therefore we can call this function with the minimum of parameters. i.e. only the html element.

WARNING!! When used with simplisity_panel on the same page, multiple calls may be made to the server. This can lead to unexpected results and race conditions.

*Examples*
```
simplisity_callserver(this, '/Desktopmodules/dnnrocket/api/rocket/action', '#myreturn', false)
```
Minimum call parameters
```
simplisity_callserver(this)
```