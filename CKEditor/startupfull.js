// use {id} to replace the control id in the script.

 var editorvar{id} = '';

 $(document).ready(function () {

     editorvar{id} = CKEDITOR.replace('editor{id}', {
         extraPlugins: 'justify',
     });

     editorvar{id}.unbind('change');
     editorvar{id}.on('change', function (event) {  
         var value = editorvar{id}.getData();
         $('#{id}').val(value);
     });
});
 