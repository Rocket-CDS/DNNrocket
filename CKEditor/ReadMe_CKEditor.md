# CKEditor setup

CKEditor code is not included in DNNrocket.  You must use the CDN or include in your system.

There has been a special build made.

The current CDN for default build (it may need to be upgraded)
```
<script src="https://cdn.jsdelivr.net/gh/leedavi/CKEditorBuilds@1.0.0/ckeditorbase1.js"></script>
```

The build is made by using the online builder (https://ckeditor.com/ckeditor-5/online-builder/) and hosted on GitHub.

https://github.com/leedavi/CKEditorBuilds/tree/v1.0.0

Only compatible with CKEditor5.

Once the CKEditor CDN is inclulded you can use the razor token to activate the editor on a textarea.
```
@CKEditor(info, "genxml/lang/genxml/textbox/categoryrichtext", "scriptClassic.html", true)
```

The config html is saved in **/DesktopModule/DNNrocket/CKeditor**.  This is the default folder that will be used.  

A full config html rel path can be passed to the razor token.

Becuase the CKEditor uses a element as a canvas, the sourcecode created by the editor needs to be moved into a input field before saving.  This is be done by using JS on the simplisityJS command "s-before" save button.

```
<div class="w3-col m2 w3-button simplisity_click"
s-before="richtextidSave"
s-return=".articlepopup"
s-cmd="categoryadmin_save"
s-post="#datacategorysection"
s-fields='{"categoryid":"@info.ItemID"}'>
@ButtonText(ButtonTypes.save)
</div>
```

The JS function is found in the "scriptClassic.html" file.

The JS function name is made from the textarea ID and the word "Save".  

"{textareaid}Save"

Thefore the s-before simplisity command attribute must match this.




---

*NOTE: DNNrocket also supports Quill editor.  This is a fully opensource projet, easier to install and simplier.  But is not as complete as CKEditor5.*
```
@EditorQuill(info, "genxml/lang/genxml/textbox/categoryrichtext", true)
```
