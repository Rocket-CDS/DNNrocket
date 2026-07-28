# Utility Functions

Simplisity has some general functions which can be used.

##### `simplisity_encode(value)`
>Encode data into decimal format. This is used if you need to ensure special characters do not affect operation. `"decimalchar.decimalchar.decimalchar...."`

##### `simplisity_decode(value)`
>Decode a value from decimal encode format back to a normal string.

##### `simplisity_getCookieValue(cookiename)`
>Get a cookie value.

##### `simplisity_setCookieValue(cookiename, cookievalue)`
>Set a cookie value.

##### `simplisity_deleteCookie(cookiename)`
>Delete a cookie.

##### `simplisity_getSessionField(fieldname)`
>Get a session variable from a cookie. (Session vars persist across posts.)

##### `simplisity_setSessionField(fieldname, value)`
>Set a session variable to a cookie. (Session vars persist across posts.)

##### `simplisity_setParamField(fieldkey, fieldvalue)`
>Sets a value to the `"simplisity_params"` field, to be passed to the server.

##### `simplisity_getParamField(fieldkey)`
>Gets a value from the `"simplisity_params"` field.

##### `simplisity_getField(sfields, fieldkey)`
>Gets a value from the `sfield` JSON passed to it.

##### `simplisity_lastmenuindex()`
>Get the index of the last menu item clicked.

##### `simplisity_emptyrecyclebin(recyclebin)`
>Removes ALL elements from a recycle bin.

##### `simplisity_getpostjson(elementselector)`
>Returns a JSON string of all child inputs.

##### `simplisity_getlistjson(elementselector)`
>Returns a JSON string of all child inputs for a list.
