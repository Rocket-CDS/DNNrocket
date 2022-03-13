# JQuery Validation

JQuery validation can be added and used as a normal website.

*See Documentation:* [jQuery Validation Plugin | Form validation with jQuery](https://jqueryvalidation.org/)

#### Getting Started

The base html page (*usually admin.html*) or the razor header template needs to include the link to the JS script.

Either use the CDN on the JQuery Validation page or add Rocket version (could be an old version)

```html
<script src="/DesktopModules/DNNrocket/js/jquery.validate.min.js"></script>
<script src="/DesktopModules/DNNrocket/js/additional-methods.min.js"></script>
```
English is the default message language, if you have other languages you must also add the localization js.

We need to test which langauge is being used so we can add the correct message js.  Therefore we must add these files using razor, or hardcode is we know which langauges.

```html
@if (DNNrocketUtils.GetCurrentLanguageCode() != "en")
{
    <script type="text/javascript" src="/DesktopModules/DNNrocket/js/localization/messages_@(DNNrocketUtils.GetCurrentLanguageCode()).js"></script>
}
```

This works if injected in the body. Usually with admin panels the SideMenu.cshtml appears on all admin pages and can be used for this.

#### Form required

A form element is required for the validation to work.  On the admin panel this form can be added to wrap the simplisity_startpanel.  

```html
<form id="admin_form">
<div id="simplisity_startpanel" class="simplisity_panel" s-cmd="dashboard_get">
    <div class="">
        <i class='fa fa-spinner fa-spin w3-display-middle' style='font-size:48px'>				</i>
    </div>
</div>
</form>
```
On normal front office webpages, the form may need to be added.

#### Input Fields

The input fields MUST have a unique "name" attribute.  Otherwise only the first input will be validated.

```html
<input class="w3-input w3-border" type="password" s-xpath="genxml/managerpassword" id="managerpassword" name='managerpassword' required>
```

#### Trigger Validation

Becuase we are using simplisityJS we do not have a "submit form" event for the ajax.  We therefore use the **s-before** and **s-stop** attributes of simplisity.

##### Button 

```html
<div class="w3-button w3-green w3-padding-8 w3-right simplisity_click portalcreatebutton " s-before="validatepopupdetail" s-cmd="portal_create" s-reload="false" s-post="#portaldetail">Create</div>

```

Notice the s-before calls a function "validatepopupdetail".

```js
function validatepopupdetail() {
    var form = $("#rocketecommerceadmin_form");
    form.validate({
        errorClass: "w3-text-red",
        highlight: function (element, errorClass) {
            $(element).removeClass(errorClass);
        }
    });
    if (form.valid()) {
        $('#portaldetail').hide();
    }
    else {
        $('.portalcreatebutton').attr('s-stop', 'stop');
    }
}
```

**Notice:** *$('.portalcreatebutton').attr('s-stop', 'stop');*  This stops the ajax from triggering the command to the server.