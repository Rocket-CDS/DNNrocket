# Change Language Code

The simplisityJS has a build-in cookies to deal with langauges.  

There are 2 cookies
- simplsity_language
- simplsity_editlanguage  

There are 2 types of language selectors.
- UI Selector. (simplsity_language) 
- Data Selector. (simplsity_editlanguage)

*There is a legancy cookie called "editlang", this may be used by some systems so it is valid.  The other legancy cookie is the "language", although this is legacy for simplisityJS it is still used in the DNN CMS.*
 
## User Language (UL)

The User langauge is the language the user uses to view the website.
Usually the "SideMenu.cshtml" contains the UI language selector for administration, but it can exist anywhere on the page.  It can be in any template.  

Example...
```
    <!-- Admin Menu -->
    <div class="w3-bottom w3-theme-l5" style="width:280px;">
        <div class="w3-bar-block ">
            <div class="w3-bar-item w3-button w3-padding-16  w3-center" id="adminbtn"><span class="material-icons">more_horiz</span></div>
        </div>
        <div class="w3-bar-block w3-section" style="display:none;" id="adminmenu">
            <div class="w3-row w3-padding-small">
                @foreach (var l in DNNrocketUtils.GetCultureCodeList())
                {
                    <div class="w3-button w3-padding-small w3-round w3-small uilanguage simplisity_click" s-cmd="changeculture" s-reload="true" s-return="#languagereturn" s-fields='{"selectedculturecode":"@l"}' style="cursor:pointer;" uilang='@l'>
                        <img class='' src='/DesktopModules/DNNrocket/API/images/flags/32/@(l).png' style="width:24px;" alt='@(l)' />
                    </div>
                }
            </div>
        </div>
    </div>

    <script>
    
        $(document).ready(function () {

            // Toggle Admin Menu
            $("#adminbtn").click(function () {
                $('#adminmenu').slideToggle();
            });

        });
    </script>

```

- **s-cmd="changeculture"** is a universal command in the "DNNrocketAPI/RocketController.cs" file, in the "ProcessAction()" method.   
- **s-fields='{"selectedculturecode":"@l"}'** The "selectedculturecode" data param is used to identify the language selected.  
- **s-reload="true"** reloads the page, so we see the new language.
- **s-return="#languagereturn"** returns the data (not relivant) to page area.  This area will usually not exist, it is normally only specified to stop the screen flashing.  


## Edit Language (EL)

The Edit langauge is the language the user is editing.
Usually the "LanguageChange.cshtml" or "TopBar.cshtml" contains the EL selector.  It can be in any template.  

Example...
```
    @foreach (var l in DNNrocketUtils.GetCultureCodeList())
    {
        var selectedcolor = "w3-theme-d3";
        if (sessionParams.CultureCodeEdit == l)
        {
            selectedcolor = "w3-theme-l5";
        }
        <div class="w3-button w3-round @(selectedcolor) editlanguage simplisity_click" s-cmd="changeeditculture" s-reload="true" s-return="#languagereturn" s-fields='{"selectedculturecode":"@l"}' style="cursor:pointer;" editlang='@l'>
            <img class='' src='/DesktopModules/DNNrocket/API/images/flags/24/@(l).png' style="" alt='@(l)' />
        </div>
    }

```
- **s-cmd="changeeditculture"** is a universal command in the "DNNrocketAPI/RocketController.cs" file, in the "ProcessAction()" method.   
- **s-fields='{"selectedculturecode":"@l"}'** The "selectedculturecode" data param is used to identify the language selected.  
- **s-reload="true"** reloads the page, so we see the new language.
- **s-return="#languagereturn"** returns the data (not relivant) to page area.  This area will usually not exist, it is normally only specified to stop the screen flashing.  

## How it works

- When a selector language is clicked a simplisity call to the server is made.  This call returns the cookie with the selected language.  
- The reload of the page will now use the new cookie.  
- Once the simplisityJS language cookie has been set, it will always be passed back to the server as a cookie.
- Because .Net standard cannot easily deal with the .Net Framework return the cookie value is added to the paramInfo simplisity class.  
- The selected languauge data is then used by each system.  *Usually in the "StartConnect.cs" file in the "InitCmd()" method.*

Example of System Code...
```
    // Assign Language
    if (_sessionParams.CultureCode == "") _sessionParams.CultureCode = DNNrocketUtils.GetCurrentCulture();
    if (_sessionParams.CultureCodeEdit == "") _sessionParams.CultureCodeEdit = DNNrocketUtils.GetEditCulture();
    DNNrocketUtils.SetCurrentCulture(_sessionParams.CultureCode);
    DNNrocketUtils.SetEditCulture(_sessionParams.CultureCodeEdit);

```

## Change Edit Language Without Reload

Reloading the page will reset the page to the first call of the page.  This is OK if we are using the tracking feature but often this is not used, so reloading the page loses the form you want to edit.  

The solution is to use a special command "scmdprocess" in the s-fields.  This is automatically populated using the "RenderLanguageSelector(string scmd, Dictionary<string,string> sfieldDict, AppThemeSystemLimpet appThemeSystem, SimplisityRazor model)" razor token.  

Call RenderLanguageSelector Example...
```
@{
    var sFieldDict = new Dictionary<string, string>();
    sFieldDict.Add("itemid", articleData.CategoryId.ToString());
}

@RenderLanguageSelector("rocketintramenu_detail", sFieldDict, appThemeSystem, Model)

```
The RenderLanguageSelector() methid needs to be called form each razor template that needs an edit language selector.  

A "[LANGUAGE]" token is added to the sfields that are generate, this needs to be replaced with the required langauge.

Example code in the "LanguageChange.cshtml" template...
```
    @foreach (var l in enabledlanguages)
    {
        var selectedcolor = "w3-theme-l3";
        if (sessionParams.CultureCodeEdit == l)
        {
            selectedcolor = "w3-theme-l5";
        }
        <div class="w3-button w3-padding-small w3-round @(selectedcolor) simplisity_click" s-cmd="@(Model.GetSetting("scmd"))" s-fields='@(Model.GetSetting("sfields").Replace("[LANGUAGE]", l))' language="@(l)"><img src="/DesktopModules/DNNrocket/API/images/flags/24/@(l).png" class="w3-round" /></div>
    }

```





