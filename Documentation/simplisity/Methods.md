# Methods

Simplisity has a few methods which are used to make things happen.

```
$(document).simplisityStartUp(string apiurl)
```
This is the main activation for Simplisity. It searches the webpage for any HTML elements with the `simplisity_panel` class. Simplisity will then process the command attributes attached to the HTML element.

Options can also be passed: `{systemkey: 'dnnrocket', activatepanel: true}`.

**systemkey**: You can pass a default systemkey to use if none are specified on the `simplisity_panel`. This stops the need to specify the systemkey on each `simplisity_panel`.  
**activatepanel**: Activate the `simplisity_panels`. Default: `true`  
**debug**: Puts the JS in debug mode. The browser console log will contain data to help debugging. Default: `false`

*Example:*
```
$(document).simplisityStartUp('/Desktopmodules/dnnrocket/api/rocket/action', { systemkey: 'rocketexample' });
```

Each panel can be activated individually if required.
```
$('#mycontainer').activateSimplisityPanel();
```

Each `simplisity_panel` can be called individually.
```
$('#mycontainer').getSimplisity(string s-cmdurl, string s-cmd, string s-fields, string s-after)
```
This method will do an individual post to the server API and return data to the selected element.

```
simplisity_callserver(element, cmdurl, returncontainer, reload)
```
The JavaScript function to post to the server can also be called directly. This works by passing the HTML element; the command attributes on the element tell the Simplisity code what to do.

If `simplisity_callserver` has already been called, the last command URL used will be activated. Therefore this function can be called with the minimum of parameters (i.e. only the HTML element).

> **WARNING:** When used with `simplisity_panel` on the same page, multiple calls may be made to the server. This can lead to unexpected results and race conditions.

*Examples*
```
simplisity_callserver(this, '/Desktopmodules/dnnrocket/api/rocket/action', '#myreturn', false)
```
Minimum call parameters
```
simplisity_callserver(this)
```
