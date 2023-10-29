# Field Data
Hidden fields are created by simplisity on page load, these are to keep track of data that often needs passing to the server.  

##### ```simplisity_systemkey```
>This is set by the "simplisity_startup" function (see below). It keeps track of the default systemkey being used by the page.

```
$(document).ready(function () {
$(document).simplisityStartUp('/My API call url', { systemkey: 'amylisbusiness'});
});
```  

##### ```simplisity_loader```
>This is a an element with a class which will be displayed when a call to the server is made. By default this is "overlayclass: 'w3-overlay'" but this call can be overwritten on the "simplisity_startup" function. If required it can be used with JQuery or JS.

```
$('#simplisity_loader').show();  
```  

##### ```simplisity_fileuploadlist```
>This field keeps a list of the fields which need to be uploaded. They are then passed and processed server side.

##### ```simplisity_params```
>List of the params that need to be passed to server side (use 'simplisity_setParamField' function). These are found on the server in the "paramjson" form fields.
This field will also save the "activevalue" paramater. The "activevalue" is the current element value. It can be used by server side code:

```
paramInfo.GetXmlProperty("genxml/hidden/activevalue")
```  

##### ```simplisity_searchfields```
>List of the search fields and data that needs to be passed to server side for a search operation. These are passed in the "paramjson" form fields.

##### ```simplisity_cmdurl```
>The current page API url endpoint. All commands that do not have a 'cmdurl' (API endpoint) specified will use this as the API endpoint. This is set by the "$(document).simplisityStartUp(string apiurl)" method, which will be on the starting page.

