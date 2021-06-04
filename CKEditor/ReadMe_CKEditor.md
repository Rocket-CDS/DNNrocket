# CKEditor setup

CKEditor code is not included in DNNrocket.  You must use the CDN or include in your system.

The current CDN  (it may need to be upgraded)
```
<script src="https://cdn.ckeditor.com/ckeditor5/27.1.0/classic/ckeditor.js"></script>
```

Be care to use CKEditor5.

Once the CKEditor CDN is inclulded you can use the razor token to activate the editor on a textarea.
```
@CKEditor(info, "genxml/lang/genxml/textbox/categoryrichtext", "scriptClassic.html", true)
```

The config html is saved in **/DesktopModule/DNNrocket/CKeditor**.  This is the default folder that will be used.  

A full config html rel path can be passed to the razor token.

If a custom build CKEditor is required the editor can be added to the system install and the required full html config path passed to the razor token.

Becuase the CKEditor uses a element as a canvas, the sourcecode created by the editor needs to be moved into a input field before saving.  This can be done by using JS on the simplisityJS command "s-before" save button.

```
<div class="w3-col m2 w3-button simplisity_click"
s-before="saveRichText"
s-return=".articlepopup"
s-cmd="categoryadmin_save"
s-post="#datacategorysection"
s-fields='{"categoryid":"@info.ItemID"}'>
@ButtonText(ButtonTypes.save)
</div>
```

The JS function:
```
    function saveRichText() {
        $('#datacategorysection').append($('#richtext')); // we need to move the richtext to the save data area.
        categoryrichtextSave(); // Copy editor data to the textarea (This function is defined by the config html and rendered by the razor token)
    }
```

---

*NOTE: DNNrocket also supports Quill editor.  This is a fully opensource projet, easier to install, simplier and is the prefered editor.  But is not as complete as CKEditor5.*
```
@EditorQuill(info, "genxml/lang/genxml/textbox/categoryrichtext", true)
```
