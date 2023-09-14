<script>

$(document).ready(function () {

	CKEDITOR.replace('{id}', {
		customConfig: '/DesktopModules/DNNrocket/API/Themes/config-w3/1.0/js/ckeditor4config1.js',
		extraPlugins: 'justify'
    });

	CKEDITOR.instances.{id}.on('key', function () {
		$('#{id}').val(CKEDITOR.instances.{id}.getData());
		$('#isdirty').val('true');
	});

	CKEDITOR.instances.{id}.on('change', function () {
		$('#{id}').val(CKEDITOR.instances.{id}.getData());
		$('#isdirty').val('true');
	});

});

</script>
