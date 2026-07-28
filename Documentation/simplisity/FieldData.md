# Field Data

Hidden fields are created by Simplisity on page load to keep track of data that often needs to be passed to the server.

##### `simplisity_systemkey`
>This is set by the `simplisity_startup` function (see below). It keeps track of the default systemkey being used by the page.

```
$(document).ready(function () {
	$(document).simplisityStartUp('/My API call url', { systemkey: 'amylisbusiness' });
});
```
<br/>

##### `simplisity_loader`
>This is an element with a class that will be displayed when a call to the server is made. By default this is `overlayclass: 'w3-overlay'`, but this can be overridden in the `simplisity_startup` function. If required it can be used with jQuery or JS.

```
$('.simplisity_loader').show();
```
<br/>

##### `simplisity_fileuploadlist`
>This field keeps a list of the fields which need to be uploaded. They are then passed and processed server-side.

##### `simplisity_params`
>List of the params that need to be passed to the server (use the `simplisity_setParamField` function). These are found on the server in the `paramjson` form fields.
>
>This field will also save the `activevalue` parameter. The `activevalue` is the current element value. It can be used by server-side code:

```
paramInfo.GetXmlProperty("genxml/hidden/activevalue")
```
<br/>

##### `simplisity_sessionfield`
>Input fields identified with this class will persist the value to the `paramjson` fields and repopulate the input fields on reload.

##### `simplisity_cmdurl`
>The current page API URL endpoint. All commands that do not have a `cmdurl` (API endpoint) specified will use this as the API endpoint. This is set by the `$(document).simplisityStartUp(string apiurl)` method, which will be on the starting page.
