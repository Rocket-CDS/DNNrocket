# Event Interface
We can create an event interface to do functionality when the API call is made.
## How it works
The event providers are called by the DNNrocket API, specifically in the “DNNrocketAPI.ApiControllers.RocketController” class.  

When an API call is made there are 2 event methods trigered.  The "BeforeEvent" and the "AfterEvent".

## Defining the Event Provider
The Event Interface is defined in the "providerdata" section of the "system.rules" file for the system or plugin.  

Example
```
	  <genxml>
		  <textbox>
			  <interfacekey>events-rocketblogapi</interfacekey>
			  <namespaceclass>RocketBlogAPI.Components.Events</namespaceclass>
			  <assembly>RocketBlogAPI</assembly>
			  <interfaceicon></interfaceicon>
			  <defaultcommand></defaultcommand>
			  <relpath>/DesktopModules/DNNrocketModules/RocketBlogAPI</relpath>
		  </textbox>
		  <providertype>eventprovider</providertype>
		  <dropdownlist>
			  <group></group>
		  </dropdownlist>
		  <checkbox>
			  <onmenu>false</onmenu>
			  <active>true</active>
		  </checkbox>
		  <radio>
			  <securityrolesadministrators>1</securityrolesadministrators>
			  <securityrolesmanager>0</securityrolesmanager>
			  <securityroleseditor>0</securityroleseditor>
			  <securityrolesclienteditor>0</securityrolesclienteditor>
			  <securityrolesregisteredusers>0</securityrolesregisteredusers>
			  <securityrolessubscribers>0</securityrolessubscribers>
			  <securityrolesall>0</securityrolesall>
		  </radio>
	  </genxml>	  

```
NOTE: The "interfacekey" should be unique.

## Event Provider Code
The Event provider class should inherit "DNNRocket.IEventAction".  

Basic Class
```
    public class Events : IEventAction
    {
        public Dictionary<string, object> BeforeEvent(string paramCmd, SystemLimpet systemData, SimplisityInfo interfaceInfo, SimplisityInfo postInfo, SimplisityInfo paramInfo, string langRequired = "")
        {
            var rtn = new Dictionary<string, object>();
            return rtn;
        }
        public Dictionary<string, object> AfterEvent(string paramCmd, SystemLimpet systemData, SimplisityInfo interfaceInfo, SimplisityInfo postInfo, SimplisityInfo paramInfo, string langRequired = "")
        {
            var rtn = new Dictionary<string, object>();
            return rtn;
        }
    }

```

The Methods will be activated when an API call is made. The methods can return a data dictionary that will be used by the API call or altered for the API return.  

## BeforeEvent
The BeforeEvent will be activated before any other action of the API call. 
The data passed from the client will be passed to this method before any API call is done.  This allows the option to alter the data before an API call is made.  
Function of the RocketController to call the Before Event.
```
var rtnDictInfo = DNNrocketUtils.EventProviderBefore(paramCmd, systemData, postInfo, paramInfo, "");
if (rtnDictInfo.ContainsKey("post")) postInfo = (SimplisityInfo)rtnDictInfo["post"];
if (rtnDictInfo.ContainsKey("param")) paramInfo = (SimplisityInfo)rtnDictInfo["param"];
```
The return dictionary will overwrite the "post" or "param" data, ready to be passed to the API call.   
NOTE: No data needs to be returned if no input data needs to be changed..

## After Event
The AfterEvent will be activated after the API has done it's work.  The return of the API can be altered or overwritten in this method.  

Function of the RocketController to call the After Event.
```
var returnDictionaryAfterEvent = DNNrocketUtils.EventProviderAfter(paramCmd, systemData, postInfo, paramInfo, "");
if (returnDictionaryAfterEvent.ContainsKey("outputhtml")) strOut = (string)returnDictionaryAfterEvent["outputhtml"];
if (returnDictionaryAfterEvent.ContainsKey("outputjson")) jsonReturn = returnDictionaryAfterEvent["outputjson"];
if (returnDictionaryAfterEvent.ContainsKey("outputxml")) xmlReturn = returnDictionaryAfterEvent["outputxml"];
```
The return dictionary can return 3 types of output.  If any are created by the Event provider they will overwrite the API call data.  
NOTE: A return dictionary does not need to return any data if no output needs to be chnaged.

In the below example, the database is changed and a new return "outputhtml" is returned by the API.
```
public Dictionary<string, object> AfterEvent(string paramCmd, SystemLimpet systemData, SimplisityInfo interfaceInfo, SimplisityInfo postInfo, SimplisityInfo paramInfo, string langRequired = "")
{
    var rtn = new Dictionary<string, object>();
    if (paramCmd == "articleadmin_savedata")
    {
        var articleId = paramInfo.GetXmlPropertyInt("genxml/hidden/articleid");
        if (articleId > 0)
        {
            var portalid = paramInfo.GetXmlPropertyInt("genxml/hidden/portalid");
            if (portalid == 0) portalid = PortalUtils.GetCurrentPortalId();
            var cultureCode = DNNrocketUtils.GetCurrentCulture();
            var articleData = new ArticleLimpet(portalid, articleId, cultureCode, systemData.SystemKey);
            if (articleData.Exists)
            {
                var catalogSettings = new CatalogSettingsLimpet(portalid, cultureCode, systemData.SystemKey);
                if (catalogSettings.Info.GetXmlPropertyBool("genxml/checkbox/hidefuturedates"))
                {
                    // Hide any future articles.  (The scheudler will display them on the correct date)
                    if (articleData.Info.GetXmlPropertyDate("genxml/textbox/publisheddate").Date > DateTime.Now.Date)
                    {
                        if (articleData.Info.GetXmlPropertyBool("genxml/checkbox/autopublish"))
                        {
                            articleData.Info.SetXmlProperty("genxml/checkbox/hidden", "true");
                            articleData.Update();

                            var sessionParams = new SessionParams(paramInfo);
                            var strOut = "";
                            var dataObject = new DataObjectLimpet(portalid, sessionParams.ModuleRef, sessionParams, systemData.SystemKey);

                            dataObject.SetDataObject("articledata", articleData);

                            var razorTempl = dataObject.AppTheme.GetTemplate("admindetail.cshtml");
                            var pr = RenderRazorUtils.RazorProcessData(razorTempl, articleData, dataObject.DataObjects, dataObject.Settings, sessionParams, true);
                            if (pr.ErrorMsg != "")
                                strOut = pr.ErrorMsg;
                            else
                                strOut = pr.RenderedText;
                            rtn.Add("outputhtml", strOut);
                        }
                    }
                }
            }
        }
    }
    return rtn;
}
```