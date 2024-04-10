# DeepL

RocketCDS supports language translation with DeepL.  TextArea and RichText can have a translate button added into the template to use the DeepL API system.  

**The DeepL API Key and URL must be entered in the Global Rocket Settings.** 
To get your key you'll need an account on https://www.deepl.com/en/pro-api  
*NOTE: The free version of DeepL is limited ot 1500 charecters.*  

The razor token @DeepL can be used to add a button to display the DeepL form.

```
DeepL(string textId, string sourceTextId = "", string cultureCode = "")
```
*textId = The input ID*  
*sourceTextId = source ID of input elemnt*  
*cultureCode = Culture code of translation*
Auto detection is used to detect the language of the text,

Example of Rich Text (CKEditor)
```
<div id="html" class='w3-row sectionname' style="display:none">
    <div class='w3-col m12 w3-padding'>
        @EditFlag(sessionParams)  &nbsp;@ChatGPT("richtext", "title") &nbsp;@DeepL("richtext", "richtext", sessionParams.CultureCodeEdit)
        <div class='w3-col m12'>
            @CKEditor4(info, "genxml/lang/genxml/textbox/richtext", true)
        </div>
        <div class='w3-col' style='width:0px;height:600px;'></div>
    </div>
</div>
```
*NOTE: RichText has the html elements removed before translation.*  
