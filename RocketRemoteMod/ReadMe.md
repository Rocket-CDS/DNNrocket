
# Running Mulitple RemoteModules

The templates for remote templates are usually designed to have only 1 remote module on the page.  
If you require mulitple modules on the page, you may need to alter the theme to have the moduleId to identify elements and JS functions.

Also, when a call is made to the remote API, we need to pass the correct remote settings params.
Because the parmas are sent by a simplisity field "remoteparams", each time we post the params to the API we need to pass the settings for that module.
we thereforw need a function to populate this "remoteparams" field.

*Example:*
`
    function sendRemoteParams@(moduleId)() {
        // send remoteParam back to the server, so we keep all the config data.
        // We place this in a function so if we have multiple remote modules, the correct data is set on the s-before.
        // FOR MULTIPLE MODULES: This function MUST be called on the "s-before" simplisty event, on each button call.
        // NOTE: This is not used on many templates that assume only 1 RemoteModule is added to the page.
        simplisity_setParamField('remotecall', 'true');
        simplisity_setParamField('remoteparams', '@(remoteParam.CompressedRemoteParam)');
    }
`

Notice the:  
`        simplisity_setParamField('remotecall', 'true'); `

The field "remotecall" needs to be passed to eh API, so the APi knows the call comes from an external source.  And will process accordingly. 

# Validation
Validation can be done by jQuery, which is required by SimplisityJS.  This function can be called by the *s-before="validatePayment"* attribute.
Notice if the validation fails, it places the "s-stop" attribute on the trigger API element.  
".dovalidate" in  this example, be sure to get the correct jQuery selector for the correct trigger element.

`
    function validatePayment() {

        var form = $("#Form");
        form.validate({
            rules: {
                email: {
                    required: true,
                    email: true
                },
            },
            errorPlacement: function () {
                return false;  // suppresses error message text
            },
            invalidHandler: function (e, validator) {
                $('input').removeClass('w3-pale-red');
                $('input').removeClass('w3-border-red');
                for (var i in validator.errorMap) {
                    $('#' + i).addClass('w3-pale-red');
                    $('#' + i).addClass('w3-border-red')
                }
            },
            amount: {
                required: true,
                digits: true
            },
        });
        if (!form.valid()) {
            $('.dovalidate').attr('s-stop', 'stop')
        }
    }
`