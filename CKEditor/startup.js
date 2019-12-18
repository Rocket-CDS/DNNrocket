// use {id} to replace the control id in the script.

 var editorvar{id} = '';

 $(document).ready(function () {

     editorvar{id} = CKEDITOR.replace('editor{id}', {
         customConfig: '/DesktopModules/DNNrocket/ckeditor/limitedconfig.js',
         contentsCss: ['/DesktopModules/DNNrocket/css/w3.css',{cssfilelist}],
         extraPlugins: 'justify',
     });

     editorvar{id}.on('change', function (event) {  
	    var value = editorvar{id}.getData();
         $('#{id}').val(value);
     });

 });
 