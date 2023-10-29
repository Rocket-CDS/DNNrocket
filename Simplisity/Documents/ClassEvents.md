# Class Events
Most operations for Simplisity are setup by applying a class or id onto a html element. Simplisity uses these class or id as instructions on what needs to be done.  

## Element Id

##### ```simplisity_startpanel```
>Identifies a html START container to use as a simplisity panel. This will be the default panel and activated when "simplisityStartUp" is called. This must be the ID of the element.  

## Class Name
##### ```simplisity_panel```
>Identifies a html container to use as a simplisity panel. This will be activated when "simplisityStartUp" is called.
##### ```simplisity_click```
>Apply a Simplisity click event onto an element.
##### ```simplisity_confirmclick```
>Apply a Simplisity click event onto an element, but ask for a confirmation popup. use "s-confirm" attribute to specify a message.
##### ```simplisity_change```
>Apply a Simplisity change event onto an element. This is usually used for dropdownlist, so something can be populated on selection.
##### ```simplisity_removelistitem```
>Remove item from html li list
##### ```simplisity_removetablerow```
>Remove row from html table
##### ```simplisity_itemundo```
>Undo Remove item from html li list
##### ```simplisity_searchfield	```
>Identifies fields used for searching. Simplisity will make a cookie from the fields identified, the data will then be sent to the search by being added automatically to the "s-field" param.
##### ```simplisity_fileupload	```
>Identifies a file upload. This class should be on the input field with type='file'
##### ```simplisity_filedownload	```
>Identifies a download file link. This class should be on the <a> element and should have the "s-fields" and "s-cmd" attributes.
##### ```simplisity_fadeout	```
>Any element with this event will fade out in 2 seconds, after the return from the server. Often used to display messages after a save and then fade them out.