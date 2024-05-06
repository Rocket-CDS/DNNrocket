# OpenAI ChatGpt

RocketCDS supports "GPT-3.5 Turbo".  TextArea and RichText can have a AI button added into the template to use the ChatGPT API system.  

**The ChatGPT API Key must be entered in the Global Rocket Settings.** 
To get your key you'll need an account on https://openai.com/

**NOTE: Login to the API login to get the API key https://platform.openai.com/ **

The razor token @ChatGPT can be used to add a button to display the ChatGPT form.

```
ChatGPT(string textId, string sourceTextId = "")
```
*textId = The input ID*  
*sourceTextId = source ID of input elemnt to use as the default question (optional)*  

Example of Rich Text (CKEditor)
```
<div class='w3-col m12 w3-padding' style='min-width: 200px;'>
    <label>@ResourceKey("HtmlContent.heading")</label>		&nbsp;@EditFlag(sessionParams)
    @TextBox(info, "genxml/lang/genxml/textbox/title", " class='w3-input w3-border' autocomplete='off' ", "", true, 0)
</div>

<div id="html" class='w3-row sectionname' style="display:none">
    <div class='w3-col m12 w3-padding'>
        @EditFlag(sessionParams) &nbsp; @ChatGPT("richtext", "title")
        <div class='w3-col m12'>
            @CKEditor4(info, "genxml/lang/genxml/textbox/richtext", true)
        </div>
        <div class='w3-col' style='width:0px;height:600px;'></div>
    </div>
</div>
```
