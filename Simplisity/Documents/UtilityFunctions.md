# Utility Functions
Simplisity has some general functions which can be used.  

##### ```simplisity_encode(value)```
>Encode data into decimal format, this is used if you need to ensure special chars do not effect operation. "decimalchar.decimalchar.decimalchar...."

##### ```simplisity_decode(value)```
>Decode value from decimal encode format back to normal string.

##### ```simplisity_getCookieValue(cookiename)```
>Get Cookie

##### ```simplisity_setCookieValue(cookiename,cookievalue)```
>Set Cookie

##### ```simplisity_setParamField(fieldkey, fieldvalue)```
>Sets a value to the "###### simplisity_params" field, to be passed to the server.

##### ```simplisity_getParamField(fieldkey)```
>Gets a value from the "###### simplisity_params" field

##### ```simplisity_getField(sfields, fieldkey)```
>Gets a value from the sfield json passed to it.

##### ```simplisity_lastmenuindex()```
>Get the index of the last menu item clicked.

##### ```simplisity_emptyrecyclebin(recyclebin)```
>Removes ALL elements from a recycle bin.

##### ```simplisity_getpostjson(elementselector)```
>Returns a json string of all child inputs.

##### ```simplisity_getlistjson(elementselector)```
>Returns a json string of all child inputs for a list.  


