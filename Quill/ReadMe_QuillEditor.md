#Quill Editor Setup

DNNrocket is compatible with the Quill editor. https://quilljs.com/

The Quill JS file is not included in DNNrocket.  You therefore must get a local version or use the CDN

```
    <!-- Include stylesheet -->
    <link href="https://cdn.quilljs.com/1.3.6/quill.snow.css" rel="stylesheet">
    <!-- Include the Quill library -->
    <script src="https://cdn.quilljs.com/1.3.6/quill.js"></script>
    <script src="/DesktopModules/DNNrocket/Quill/image-resize.min.js"></script>
```

The defualt install includes the 'snow' theme and an image resize. (See plugin 'image-resize.min.js')

The razor template only need to have the QuillEditor razor token included to activate and save the richtext.

```
@EditorQuill(info, "genxml/lang/genxml/textbox/categoryrichtext", "style='min-height:150px;width:100%'", "", true)
```

If you need to use specific plugins and css you can include a plugin script, which can provider bespoke functionality.  (The full config text is expected.)
```
@EditorQuill(info, "genxml/lang/genxml/textbox/categoryrichtext", "style='min-height:150px;width:100%'", portalCatalog.AppTheme.GetTemplate("quillconfig.html"), true)
```

The config JS is the basic configuration of Quill with a change event to save the richtext to the textarea which is passed to the server.

```
<script>
	$(document).ready(function () {


        var {elementid}quill = new Quill('#{elementid}quill', {
            theme: 'snow',
            modules: {
				imageResize: {
                    modules: ['Resize', 'DisplaySize']
                }
            }
		});

		
		{elementid}quill.on('text-change', function(delta, source) { 
			
		jQuery('#{elementid}').val({elementid}quill.root.innerHTML); 
		
		});
		
	});  
</script>
```

NOTE: The {elementid} token is used to identify the field and is replaced with the correct field id on rendering.

####CSS
The CSS for code sections is black and can be changed.
```
        .ql-snow .ql-editor pre.ql-syntax {
            background-color: #f8f8f2;
            color: #444;
            overflow: visible;
        }
```






