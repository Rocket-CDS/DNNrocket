# Command Attributes

Simplisty uses element attributes to do the required processing and to pass/return data to the server side API.  

>s-cmdurl  
s-cmd  
s-fields  
s-itemid  
s-post  
s-list  
s-return  
s-append  
s-before  
s-after  
s-hideloader  
s-recylebin  
s-removelist  
s-confirm  
s-regexpr  
s-maxfilesize  
s-dropdownlist  
s-index  
s-update  
s-datatype  
s-reload  
s-stop  
s-xpath  


### ```s-cmdurl```  
>Server side API url. This is saved to a cookie when used. If you are using "simplisityStartUp" then this should be defined in there, however from a module which is not using simplisity panels you may need to define this on the element. It MUST be defined or any ajax calls will not work and may simply result is a reload of the page.

### ```s-cmd```  
>API command.

### ```s-fields```  
>Paramater fields that can be passed to the API. This is a json string in the format of '{"name":"value","name":"value","name":"value"}'. Any number of parameters can be passed.

### ```s-itemid```  
>Identify records in list. Used for sorting and selecting.

### ```s-post```  
>The JQuery selector of an element that will be passed to the server. Simplisity takes ALL input fields in the selected html element and posts to the server, which can then process the data. If s-fields and s-post are defined both sets of data are passed to the server.

### ```s-list```  
>The JQuery selector of repeating data which will be past to server as a list. The value can be a csv of list classes used on the page, each row should have the class assigned to the parent/enclosing element. If you want to save a list to the DB then this should be on the save button, it will then be passed to the server along with the s-post element. It can be read by using <SimplisityInfo>.GetList(list name) and is returned as a List<SimplisityInfo>.  

### ```s-return```  
>The JQuery selector of the return element. Any data returned from the server will be added/replace to this html element content. If not specified, this will default to "#simplisity_startpanel"  

### ```s-append```  
>Specifies that any return data will be added to the s-return element. ('true' or 'false')  

### ```s-before```  
>The name of a js function to be activated before the API call is made.  

### ```s-after```  
>The name of a js function to be activated after the API call is made.  

### ```s-hideloader```  
>Hide the client side loader after the server API call. Default = 'true'. This is used when you need to stop flicker because of multiple calls before processing is completed.  

### ```s-recylebin```  
>Specify the recycle bin when a list element or table row is removed. This must be on both the remove button and the undo button.  

### ```s-removelist```  
>The JQuery selector of the removed element. This is used to move the list element into and out of the recycle bin. It should be placed on the removeitem button and the undo button. This is the li[s-index] or tr[s-index] parent of the removed button.  

### ```s-confirm```  
>Confirm message of "simplisity_confirmclick"  

### ```s-regexpr```  
>Regular Expression used to limit file type upload. This is used only on the "simplisity_fileupload" class element. If not specified the default is used.    
```
'/(\.|\/)(gif|jpe?g|png|pdf)$/i'
```

### ```s-maxfilesize```  
>Used to limit file size on upload. This is used only on the "simplisity_fileupload" class element. If not specified the default is used. 5000000  

### ```s-dropdownlist```  
>Used to link a dropdown to another field. If used the server will return a JSON object. This can be used to populated the dropdownlist when an element changes. The JSON return format shoud be   
```
{listkey:['1','2','3'],listvalue:['1','2','3']}
```

### ```s-index```  
>This is an attribute which is automatically added to elements in a list, so Simplisity can identify them.  

### ```s-update```  
>INPUT FIELD: This attribute is optional for all input fields. There are 3 possible values: s-update="save" the server will deal with the value as a normal non-localized value. This is the same as if no s-update attribute was added. s-update="lang" the server will deal with the value as a localized value. s-update="ignore" the value will be ignored.  

### ```s-datatype```  
>INPUT FIELD: Datatype of the field data. "email","date","double","html","coded". All other data type are dealt with as string.  

### ```s-reload```  
>'true' or 'false'. By default an image or document upload will trigger a reload of the page. In some situations this is not required (when ajax return updates the page), so the reload can be stopped by using s-reload='false'. Default is 'false'.  

### ```s-stop```  
>Stop Call. If you want to stop the call to the server you can create an attribute with a value of 'stop' on the call element, using jQuery or JS. This is usually used for validation, where the s-before function has found a problem and wants to stop the call to the server. Simplisity will clear the s-stop when the call is cancelled. 
```
$('.myselectionclass').attr('s-stop','stop')
```

### ```s-xpath```  
>Sets the xpath for data in the XML. This is usually only used if you have a html input which saves data back to the server.  
