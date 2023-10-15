# DNN Search
The DNN search can be used by rocket modules.  The AppTheme used contains the field infomation required by the search functionality.  


## RocketContent

The id of the fields required are added by using a hidden field and the field id.  
This can be a CSV list if multiple fields are required on the search.  
This functionality uses the  "ToDictionary()" method of simplsity, so duplicate field ids on the same data will not work and shoudl be avoided in the AppTheme.  

The search field information should be in the "ThemeSetings.cshtml" template of the AppTheme.   

### ThemeSetings.cshtml (example)
```
<!-- Search Field Data -->
@HiddenField(new SimplisityInfo(),"genxml/settings/searchtitle","","headertitle")
@HiddenField(new SimplisityInfo(),"genxml/settings/searchbody","","title,richtext")
@HiddenField(new SimplisityInfo(),"genxml/settings/searchdescription","","headertext")

```
### AdminRow.cshtml (example)
```
    <div class="w3-col m6">
        <label>@ResourceKey("DNNrocket.moduletitle")</label>&nbsp;@EditFlag(sessionParams)
        @TextBox(info, "genxml/lang/genxml/header/headertitle", "class='w3-input w3-border'", "", true)
    </div>
    
    <div class="w3-row-padding w3-section">
        <div class="w3-row-padding">
            <label>@ResourceKey("DNNrocket.text")</label>&nbsp;@EditFlag(sessionParams)
            @TextBox(info, "genxml/lang/genxml/header/headertext", "class='w3-input w3-border'", "", true)
        </div>
    </div>

    <div class="w3-row-padding w3-section">
        <div class="w3-row-padding">
            <label>@ResourceKey("DNNrocket.heading")</label>&nbsp;@EditFlag(sessionParams)
            @TextBox(info, "genxml/lang/genxml/textbox/title", " id='title' class='w3-input w3-border' autocomplete='off' ", "", true, 0)
        </div>
    </div>

    <div id="html" class='w3-row sectionname' style="display:none">
        <div class='w3-col m12 w3-padding'>
            @EditFlag(sessionParams)
            <div class='w3-col m12'>
                @CKEditor4(info, "genxml/lang/genxml/textbox/richtext", true)
            </div>
            <div class='w3-col' style='width:0px;height:600px;'></div>
        </div>
    </div>
```