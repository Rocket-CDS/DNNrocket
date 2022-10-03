# Debug API

The API input can be exported to the filesystem when command are written.  This is used to get a Json formatted data file.  

In the session params pass the "debugapi" parameter.

```
        simplisity_setParamField("debugapi", true);
```

This can also be set in the "s-fields" of the element.

```
s-fields='{"debugapi":"true"}'
```

The output json can be found in the "/Portals/_default/RocketLogs/" folder with a file name of the command.  
There will be 2 files, 1 for the param data "paramjson" and 1 for the post data "inputjson".

