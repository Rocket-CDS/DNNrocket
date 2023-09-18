# File Download

## Simple public download
Documents can be downloaded by using a simple html link or javascript.  The same as in any webpage.

HTML
```
<a href="myfile.pdf">Download Brochure</a>
```
JS
```
  <script>
    function actiondownload() {
      window.location ="<URL>)";
    }
  </script>
```

### RocketDirectoryAPI
File downloads from RocketDirectory have a default document field name.  (But any field name can be used n the template)  
```
@foreach (var doc in articleData.GetDocs())
{
    <a href="@doc.RelPath">
        @doc.Name
    </a>
}
```
## Restricted download
Simplsity.js can download files from an API call.  This allows server side tests to check if the users has security access to download.

Example:
```
@foreach (var doc in articleData.GetDocs())
{
    <a class="panel-block simplisity_filedownload" s-cmd="remote_publicdownload" s-fields='{"articleid":"@(articleData.ArticleId.ToString())","dockey":"@(doc.DocKey)"}'>
        <i class="fas fa-file-pdf fa-fw" aria-hidden="true"></i> @doc.Name
    </a>
}
```

The simplsity class event of "simplisity_filedownload" creates a URL to download from the API.

*NOTE: This example is from RocketDirectory and must include "articleid" and "dockey".*

## Important
To make Simplisity file downloads work, you MUST implement the link within a html element within a “simplisity_panel” class.  
Activate simplisity by the JS call to “StartUp” or a “single panel”.
```
$(document).simplisityStartUp('/Desktopmodules/dnnrocket/api/rocket/action', { "systemkey": "rocketdirectoryapi"});
```

# Server Side
Because the s-fields are added to the url as parameters you can therefore access those values on server-side code by using the “genxml/urlparams/***” xpath from the paramInfo SimplsityInfo class.  

## Return dictionary keys on server code

The download can be done from a file or from a document in memory.

### From File
```
rtnDic.Add("filenamepath", DNNrocketUtils.MapPath(articleDoc.RelPath));  
rtnDic.Add("downloadname", articleDoc.Name);
```

### From Memory Data
(Recommended with only small files [< 10MB], but may work with larger files if the server is powerful)  
```
rtnDic.Add("downloadfiledata", "String Data");
rtnDic.Add("downloadname", articleDoc.Name);
```

# RocketDirectory

The default server side code in RocketDirectory checks is the user is authorised.  If not, no download is done.  

This functionality is controlled by the "secure documents" checkbox in the Admin Settings for the system.  Uploads will be given a secure name that cannot be downloaded by normal html links. (This assumes the IIS default settings for files.)  

This functionlity stops and documents from being referenced by any search engine.  
*NOTE: The "secure document" must be turned on before upload if you required secure documents.* 

If public downloads are required for the entire site, you can uncheck the "secure documents" checkbox.

You must use a **s-cmd="remote_publicdownload"** to get this functionality.

If extra security is required then a plugin will need to be created.


Example:
```
private Dictionary<string, object> DownloadArticleFile()
{
    var rtnDic = new Dictionary<string, object>();
    var strId = GeneralUtils.DeCode(_paramInfo.GetXmlProperty("genxml/urlparams/articleid"));
    var articleId = 0;
    if (GeneralUtils.IsNumeric(strId) && (UserUtils.IsAuthorised() || !_dataObject.PortalContent.SecureUpload))
    {
        articleId = Convert.ToInt32(strId);
        var articleData = GetActiveArticle(articleId);
        articleData.ClearCache(); // reset data to clear cached name.
        articleData = GetActiveArticle(articleId);

        var dockey = GeneralUtils.DeCode(_paramInfo.GetXmlProperty("genxml/urlparams/dockey"));
        var articleDoc = articleData.GetDoc(dockey);
        rtnDic.Add("filenamepath", DNNrocketUtils.MapPath(articleDoc.RelPath));
        rtnDic.Add("downloadname", articleDoc.DownloadName);
    }
    return rtnDic;
}
```
## InterfaceKey
An interfacekey must also exist in the "system.rules" file.  The interfacekey name must match the command. Usually "remote", for public access with the security being done server side o the download API method.   

Example:
```
      <genxml>
        <textbox>
          <interfacekey>remote</interfacekey>
          <namespaceclass>RocketDirectoryAPI.API.StartConnect</namespaceclass>
          <providernamespaceclass></providernamespaceclass>
          <assembly>RocketDirectoryAPI</assembly>
          <interfaceicon></interfaceicon>
          <defaultcommand></defaultcommand>
          <relpath>/DesktopModules/DNNrocketModules/RocketDirectoryAPI</relpath>
        </textbox>
        <providertype></providertype>
        <dropdownlist>
          <group></group>
        </dropdownlist>
        <checkbox>
          <onmenu>false</onmenu>
          <active>true</active>
        </checkbox>
        <radio>
          <securityrolesadministrators>1</securityrolesadministrators>
          <securityrolesmanager>1</securityrolesmanager>
          <securityroleseditor>1</securityroleseditor>
          <securityrolesclienteditor>1</securityrolesclienteditor>
          <securityrolesregisteredusers>1</securityrolesregisteredusers>
          <securityrolessubscribers>1</securityrolessubscribers>
          <securityrolesall>1</securityrolesall>
        </radio>
      </genxml>

```
