# RocketForms Razor Snippets

Snippets for building AppTheme templates for **RocketForms**.  
Copy and paste into your own `AppThemes` folder to get started.

---

## Folder Structure

A minimal AppTheme only needs `View.cshtml` and `ThemeSettings.cshtml`. Add the optional override templates as needed.

```
rocketforms.{YourThemeName}/
  1.0/
    default/
      View.cshtml               ← the form (front-end)  [required]
      ThemeSettings.cshtml      ← module-level settings UI  [required]
      DelayFormButton.cshtml    ← custom submit button appearance  [optional override]
      PinForm.cshtml            ← verification step before submit  [optional override]
      InvalidMessage.cshtml     ← shown when pin verification fails  [optional override]
      SentMessage.cshtml        ← shown after successful submission  [optional override]
      SentErrMessage.cshtml     ← shown when email fails to send  [optional override]
      EmailForm.cshtml          ← HTML body of the notification email  [optional override]
    css/
    dep/
      {YourThemeName}.dep
    resx/
      {YourThemeName}.resx
```

> The system templates (send button, messages, email body, verification) are all provided by the module and work out of the box. Placing a file of the same name in your AppTheme `default/` folder overrides the built-in version.

---

## How a Form Submission Works

1. The user fills in the form fields in `View.cshtml`.
2. If `PinForm.cshtml` is included, a verification step is shown first.
3. `@DelayFormButton(Model, "#myform" + moduleData.ModuleId)` renders a submit button **after a delay** (bot protection).
4. On click, the Simplisity framework POSTs all fields inside the `#myform{moduleId}` container to the `rocketforms_publicpostform` API command.
5. The posted data is saved to disk as an XML file (never to the DB — prevents SQL injection).
6. The system sends an email using the `EmailForm.cshtml` template as the email body.
7. On success, the form container is replaced with `SentMessage.cshtml`. On failure, `SentErrMessage.cshtml` is shown.

---

## The `@DelayFormButton` Token — Explained

```razor
@DelayFormButton(Model, "#contactform" + moduleData.ModuleId)
```

| Argument | Description |
|---|---|
| `Model` | The current Razor model — passed so the button template can access module settings. |
| `"#contactform" + moduleData.ModuleId` | jQuery selector of the **div that wraps your form fields**. The framework POSTs every input inside this element. The `moduleId` suffix makes it unique when multiple form modules are on the same page. |
| `millisec` *(optional, default 1200)* | Milliseconds to wait before the button becomes visible. |
| `template` *(optional, default `"DelayFormButton.cshtml"`)* | Name of the Razor template to use for the button's appearance. |

**Why the delay?** Automated bots submit forms immediately on page load. By withholding the submit button for ~1.2 seconds, bots that don't execute JavaScript, or that submit before the button appears, are silently ignored — no CAPTCHA required.

**What it renders:** A `<div>` placeholder that the button is injected into via JavaScript after the delay. The button fires `rocketforms_publicpostform`, serialises every input inside the form `id` selector, and replaces the form container with the `SentMessage.cshtml` (or `SentErrMessage.cshtml`) response.

---

## Example Empty View.cshtml

The bare-bones starting point. The outer `<div>` id must match the selector passed to `@DelayFormButton`.

```razor
@inherits RocketForms.Components.RocketFormsTokens<Simplisity.SimplisityRazor>
@using DNNrocketAPI.Components;
@using RocketForms.Components;
@using Simplisity;
@AssignDataModel(Model)
<!--inject-->
@{
    var info = new SimplisityInfo();
}

<div id="contactform@(moduleData.ModuleId)" class="w3-row">
    <div class="w3-container">

        <div class="w3-row w3-padding">
            <label>Name</label>
            @TextBox(info, "genxml/textbox/name", "class='w3-input w3-border'")
        </div>

        <div class="w3-row w3-padding">
            <label>Email *</label>
            @TextBox(info, "genxml/textbox/email", "class='w3-input w3-border' name='email' required email", "", false, 0, "", "email")
        </div>

        <div class="w3-row w3-padding">
            <label>Message</label>
            @TextArea(info, "genxml/textbox/message", "class='w3-input w3-border' rows='6'")
        </div>

    </div>

    <div class="w3-row w3-center w3-large">
        @DelayFormButton(Model, "#contactform" + moduleData.ModuleId)
    </div>

</div>
```

> **Important:** `var info = new SimplisityInfo()` creates an **empty** info object. Form fields capture new user input, so there is no pre-existing data to load.  
> The `email` field must have `name='email'` so the built-in jQuery validation rule can find it.

---

## Form Field Types

All fields use the same empty `info` object. The `genxml/textbox/` prefix is the standard path.

```razor
@{
    var info = new SimplisityInfo();
}

@* single-line text *@
@TextBox(info, "genxml/textbox/name", "class='w3-input w3-border'")

@* email (with browser & jQuery validation) *@
@TextBox(info, "genxml/textbox/email", "class='w3-input w3-border' name='email' required", "", false, 0, "", "email")

@* phone *@
@TextBox(info, "genxml/textbox/tel", "class='w3-input w3-border' type='tel'")

@* multi-line text *@
@TextArea(info, "genxml/textbox/message", "class='w3-input w3-border' rows='6'")

@* date picker *@
@TextBox(info, "genxml/textbox/appointmentdate", "class='w3-input w3-border'", "", false, 0, "", "date")

@* dropdown *@
@DropDownList(info, "genxml/select/topic", "General,Support,Sales,Other", "class='w3-input w3-border'", "General")

@* checkbox *@
@CheckBox(info, "genxml/checkbox/newsletter", "Subscribe to newsletter", "class='w3-check'")

@* hidden value passed through with the form (e.g. to prefix the email subject) *@
@HiddenField(infoempty, "genxml/hidden/emailsubjectprefix", "", "Contact Form")
```

---

## Localized Resource Keys

Use `@ResourceKey("RocketForms.key")` to load strings from the module's `.resx` file or the AppTheme's own `.resx`.

```razor
<label>@ResourceKey("RocketForms.name")</label>
<label>@ResourceKey("RocketForms.email")</label>
<label>@ResourceKey("RocketForms.tel")</label>
<label>@ResourceKey("RocketForms.company")</label>
<label>@ResourceKey("RocketForms.subject")</label>
<label>@ResourceKey("RocketForms.message")</label>
<small>@ResourceKey("RocketForms.compulsory")</small>
```

Add your own keys to the AppTheme `.resx` and they are picked up automatically.

---

## Reading Module Settings in View.cshtml

Settings configured in `ThemeSettings.cshtml` are read via `moduleData`.

```razor
@{
    var backgroundcolor = moduleData.GetSetting("backgroundcolor");
    var paddingClass    = moduleData.GetSetting("padding");
    var cssClass        = moduleData.GetSetting("cssclass");
    var headingText     = moduleData.GetSetting("heading" + sessionParams.CultureCode);
    var buttonColor     = moduleData.GetSetting("buttoncolor");
    var stylepadding    = StylePadding();
}

<div class="w3-section @backgroundcolor @cssClass" style="@stylepadding">
    @if (headingText != "")
    {
        <h2>@headingText</h2>
    }
    @* form fields here *@
</div>
```

---

## Example ThemeSettings.cshtml

All field xPaths **must** use the `genxml/settings/` prefix.  
Use `sessionParams.CultureCode` suffix for per-language text settings.

```razor
@inherits RocketForms.Components.RocketFormsTokens<Simplisity.SimplisityRazor>
@using DNNrocketAPI.Components;
@using RocketForms.Components;
@using Simplisity;
@AssignDataModel(Model)
<!--inject-->
@{
    var info = new SimplisityInfo(moduleData.Record);
}

@foreach (var lang in DNNrocketUtils.GetCultureCodeList())
{
    <div class="w3-row w3-padding">
        <label>Heading (@lang) @DisplayFlag(lang)</label>
        @TextBox(info, "genxml/settings/heading" + lang, "class='w3-input w3-border'")
    </div>
}

<div class="w3-third w3-padding">
    <label>Button Color</label>
    @DropDownList(info, "genxml/settings/buttoncolor", W3Utils.W3colors(), "class='w3-input w3-border'", "normal")
</div>

<div class="w3-third w3-padding">
    <label>Background Color</label>
    @DropDownList(info, "genxml/settings/backgroundcolor", W3Utils.W3colors(), "class='w3-input w3-border'", "normal")
</div>

<div class="w3-third w3-padding">
    <label>Padding</label>
    @RadioButtonList(info, "genxml/settings/padding", "w3-padding-small,w3-padding,w3-padding-large", "Small,Medium,Large", "class=''", "w3-padding")
</div>

<div class="w3-row w3-padding">
    <label>Top Padding</label>
    @TextBox(info, "genxml/settings/toppadding", "class='w3-input' min='0' max='320' step='8'", "8", false, 0, "", "range")
</div>

<div class="w3-row w3-padding">
    <label>Bottom Padding</label>
    @TextBox(info, "genxml/settings/bottompadding", "class='w3-input' min='0' max='320' step='8'", "8", false, 0, "", "range")
</div>

<div class="w3-third w3-padding">
    <label>CSS Class</label>
    @TextBox(info, "genxml/settings/cssclass", "class='w3-input w3-border'", "")
</div>
```

---

## Overriding the Send Button Appearance

To customise the button, create a `DelayFormButton.cshtml` in your AppTheme's `default/` folder. The system uses yours instead of the built-in one.

```razor
@inherits RocketForms.Components.RocketFormsTokens<Simplisity.SimplisityRazor>
@using RocketForms.Components;
@using Simplisity;
@AssigDataModel(Model)
@{
    var buttonColor = moduleData.GetSetting("buttoncolor");
}

<div id="buttondiv@(moduleData.ModuleId)" class="rocket-postbutton simplisity_panel" style="min-height:44px;">
    <div class="w3-button w3-theme-action @buttonColor">Sending…</div>
</div>
@HiddenField(infoempty, "genxml/hidden/moduleid", "", moduleData.ModuleId.ToString())

<script>
    $(document).ready(function () {
        setTimeout(function () {
            var btn = '<div id="sendemail@(moduleData.ModuleId)" class="isbutton w3-button w3-theme-action @buttonColor a-dovalidate simplisity_click" '
                + 's-before="validateFom@(moduleData.ModuleId)" '
                + 's-return="#buttondiv@(moduleData.ModuleId)" '
                + 's-cmdurl="/Desktopmodules/dnnrocket/api/rocket/action" '
                + 's-cmd="rocketforms_publicpostform" '
                + 's-post="@(Model.GetSetting("spost"))" >'
                + '@ResourceKey("RocketForms.send")</div>';
            $('#buttondiv@(moduleData.ModuleId)').html(btn);
            $('@(Model.GetSetting("spost"))').activateSimplisityPanel();
        }, 1200);
    });
    function validateFom@(moduleData.ModuleId)() {
        var form = $('#Form');
        form.validate({
            rules: { email: { required: true, email: true } },
            errorPlacement: function () { return false; },
            invalidHandler: function (e, v) {
                $('input').removeClass('w3-pale-red w3-border-red');
                for (var i in v.errorMap) { $('#' + i).addClass('w3-pale-red w3-border-red'); }
            }
        });
        if (!form.valid()) { $('.a-dovalidate').attr('s-stop', 'stop'); }
    }
</script>
```

---

## Overriding the Success / Error Messages

Create `SentMessage.cshtml` and/or `SentErrMessage.cshtml` in your AppTheme `default/` folder to replace the built-in messages. The success message text is set in the module's admin panel (stored as `emailsentmessage{cultureCode}`).

```razor
@* SentMessage.cshtml *@
@inherits RocketForms.Components.RocketFormsTokens<Simplisity.SimplisityRazor>
@using RocketForms.Components;
@using Simplisity;
@AssigDataModel(Model)
<!--inject-->
<div class="w3-panel w3-green w3-padding">
    @HtmlOf(moduleData.GetSetting("emailsentmessage" + sessionParams.CultureCode))
</div>
```

```razor
@* SentErrMessage.cshtml *@
@inherits RocketForms.Components.RocketFormsTokens<Simplisity.SimplisityRazor>
@using RocketForms.Components;
@using Simplisity;
@AssigDataModel(Model)
<!--inject-->
<div class="w3-panel w3-red w3-padding">
    <span onclick="$('.simplisity_loader').show();window.location.reload(true);" style="cursor:pointer;float:right;">✕</span>
    @HtmlOf(moduleData.GetSetting("emailfailmessage" + sessionParams.CultureCode))
</div>
```

---

## The EmailForm.cshtml Template

`EmailForm.cshtml` is the HTML body of the notification email sent after each successful form submission. The **built-in version is generic** — it loops through every posted field and renders it as a labelled key/value list. This works for any form without any customisation.

### How the generic template works

- The email subject is taken from the module admin setting `subject`.
- Every field inside `genxml/data/` is printed as `Label: Value`, one per line.
- Field labels are resolved from the AppTheme `.resx` file using the field name as the key — so if you add a field `genxml/textbox/fromaddress`, add `fromaddress` as a key in your `.resx` and the email will use that label.
- Date fields (any field whose name contains `date`) are automatically formatted using the `emaillanguage` setting from the module admin panel.
- Internal system fields (`moduleid`, `emailsubjectprefix`, `browsersessionid`, etc.) are automatically excluded from the email body.

### Adding an AppTheme `.resx` key for a field

In your AppTheme `resx/{YourThemeName}.resx`:

```xml
<data name="fromaddress" xml:space="preserve">
    <value>Departure address</value>
</data>
<data name="destaddress" xml:space="preserve">
    <value>Destination address</value>
</data>
```

### Overriding EmailForm.cshtml for a bespoke email layout

Create `EmailForm.cshtml` in your AppTheme `default/` folder to take full control of the email design — add your logo, brand colours, custom layout, and pick exactly which fields to show and in what order.

```razor
@inherits RocketForms.Components.RocketFormsTokens<Simplisity.SimplisityRazor>
@using DNNrocketAPI;
@using Simplisity;
@using RocketForms.Components;
@using DNNrocketAPI.Components;
@AssigDataModel(Model)
<!--inject-->
@{
    var sRec = (SimplisityRecord)Model.GetDataObject("formdata");
}

<style>
body { margin: 0; padding: 40px; font-family: system-ui, sans-serif; font-size: 16px; color: #222; }
h2   { font-size: 24px; color: #003366; }
.field-label { font-weight: bold; }
</style>

<h2>New enquiry — @moduleData.GetSetting("subject")</h2>

<p>
    <span class="field-label">Name:</span> @sRec.GetXmlProperty("genxml/textbox/name")<br />
    <span class="field-label">Email:</span> @sRec.GetXmlProperty("genxml/textbox/email")<br />
    <span class="field-label">Tel:</span> @sRec.GetXmlProperty("genxml/textbox/tel")<br />
</p>

<hr />
<p><span class="field-label">Message:</span></p>
<p>@HtmlOf(sRec.GetXmlProperty("genxml/textbox/message"))</p>
```

> `Model.GetDataObject("formdata")` returns a `SimplisityRecord` containing every field that was posted. Use `GetXmlProperty("genxml/textbox/{fieldname}")` to read individual fields.

---

## The PinForm.cshtml Template

`PinForm.cshtml` adds an extra human-verification step that is displayed **before the main form fields become active**. It is a lightweight, CAPTCHA-free way to reduce automated submissions. The default system template provides a simple implementation and can be overridden by placing your own `PinForm.cshtml` in the AppTheme `default/` folder.

When the user completes the verification step, the module reloads the form area using `s-cmd="rocketforms_public2"`, which returns the normal `View.cshtml` content and enables the delayed submit button.

### Minimal PinForm.cshtml override

```razor
@inherits RocketForms.Components.RocketFormsTokens<Simplisity.SimplisityRazor>
@using DNNrocketAPI.Components;
@using Simplisity;
@using RocketForms.Components;
@AssigDataModel(Model)
<!--inject-->

<div class="w3-card-4 w3-white w3-round-large w3-padding" style="max-width:400px; margin:2em auto;">
    <h4 class="w3-center w3-text-grey">@ResourceKey("PinForm.securityverification")</h4>
    <div class="w3-margin-top">
        <label class="w3-text-dark-grey">
            <input class="w3-check w3-margin-right simplisity_change"
                   s-cmd="rocketforms_public2"
                   @SFields("moduleid", moduleData.ModuleId.ToString())
                   s-return="#buttondiv@(moduleData.ModuleId)"
                   s-cmdurl="/Desktopmodules/dnnrocket/api/rocket/action"
                   type="checkbox"
                   name="robotCheck"
                   id="robotCheck">
            <span>@ResourceKey("PinForm.notrobot")</span>
        </label>
    </div>
</div>
<script>
    $(document).ready(function() {
        simplisity_setParamField("pinform", new Date().toISOString());
    });
</script>
```

**Key points:**

- The `s-cmd="rocketforms_public2"` attribute on the checkbox triggers the reload when the user checks it.
- `@SFields("moduleid", moduleData.ModuleId.ToString())` passes the module id back so the correct module refreshes.
- `simplisity_setParamField("pinform", ...)` stamps a timestamp that the server can use to validate the interaction.
- `s-return="#buttondiv@(moduleData.ModuleId)"` replaces the button placeholder area with the activated form.

### InvalidMessage.cshtml

If the verification step fails or an unexpected condition is detected, `InvalidMessage.cshtml` is shown instead of the form. Override it in the AppTheme to match your site style.

```razor
@inherits RocketForms.Components.RocketFormsTokens<Simplisity.SimplisityRazor>
@using RocketForms.Components;
@using Simplisity;
@AssigDataModel(Model)
<!--inject-->
<div class="w3-panel w3-red w3-padding w3-round">
    <span onclick="$('.simplisity_loader').show();window.location.reload(true);"
          style="cursor:pointer;float:right;">✕</span>
    @HtmlOf(moduleData.GetSetting("emailfailmessage" + sessionParams.CultureCode))
</div>
```

### AppTheme `.resx` keys for PinForm and InvalidMessage

Add these keys to your AppTheme resx files to localise the pin form and error message:

```xml
<!-- PinForm.resx -->
<data name="PinForm.securityverification" xml:space="preserve">
    <value>Security Verification</value>
</data>
<data name="PinForm.notrobot" xml:space="preserve">
    <value>I'm not a robot</value>
</data>
<data name="PinForm.pleaseverify" xml:space="preserve">
    <value>Please complete the verification</value>
</data>

<!-- InvalidMessage.resx -->
<data name="InvalidMessage.errorheading" xml:space="preserve">
    <value>Verification failed</value>
</data>
<data name="InvalidMessage.tryagainbutton" xml:space="preserve">
    <value>Try again</value>
</data>
```

---

## System Templates You Can Override

Place any of these in your AppTheme `default/` folder to override the built-in version:

| Template | Purpose |
|---|---|
| `DelayFormButton.cshtml` | The submit button (delayed display) |
| `PinForm.cshtml` | Human-verification step shown before the submit button |
| `InvalidMessage.cshtml` | Shown when the verification step fails |
| `SentMessage.cshtml` | Shown after successful submission |
| `SentErrMessage.cshtml` | Shown when the email fails to send |
| `EmailForm.cshtml` | The HTML body of the notification email |

---

## Quick Reference

| What | Code |
|---|---|
| Inherits line | `@inherits RocketForms.Components.RocketFormsTokens<Simplisity.SimplisityRazor>` |
| Assign data model | `@AssignDataModel(Model)` |
| Empty info for form fields | `var info = new SimplisityInfo()` |
| Info from module record (settings) | `var info = new SimplisityInfo(moduleData.Record)` |
| Text input | `@TextBox(info, "genxml/textbox/field", "class='w3-input w3-border'")` |
| Email input | `@TextBox(info, "genxml/textbox/email", "... name='email' required", "", false, 0, "", "email")` |
| Textarea | `@TextArea(info, "genxml/textbox/field", "class='w3-input w3-border' rows='6'")` |
| Dropdown | `@DropDownList(info, "genxml/select/field", "A,B,C", "class='w3-input w3-border'", "A")` |
| Checkbox | `@CheckBox(info, "genxml/checkbox/field", "Label", "class='w3-check'")` |
| Hidden value in post | `@HiddenField(infoempty, "genxml/hidden/emailsubjectprefix", "", "My Subject")` |
| Delayed submit button | `@DelayFormButton(Model, "#myform" + moduleData.ModuleId)` |
| Custom delay (ms) | `@DelayFormButton(Model, "#myform" + moduleData.ModuleId, 2000)` |
| Custom button template | `@DelayFormButton(Model, "#myform" + moduleData.ModuleId, 1200, "MyButton.cshtml")` |
| Pin verification cmd | `s-cmd="rocketforms_public2"` on the verification input |
| Pin timestamp field | `simplisity_setParamField("pinform", new Date().toISOString())` |
| Posted form data object | `(SimplisityRecord)Model.GetDataObject("formdata")` |
| Read a posted field | `sRec.GetXmlProperty("genxml/textbox/fieldname")` |
| Resource string | `@ResourceKey("RocketForms.name")` |
| Module setting | `moduleData.GetSetting("fieldname")` |
| Module setting (int) | `moduleData.GetSettingInt("fieldname")` |
| Inline padding style | `StylePadding()` |
| Success message text | `moduleData.GetSetting("emailsentmessage" + sessionParams.CultureCode)` |
| Error message text | `moduleData.GetSetting("emailfailmessage" + sessionParams.CultureCode)` |
| Rendered HTML setting | `@HtmlOf(moduleData.GetSetting("emailsentmessage" + sessionParams.CultureCode))` |
| Language flag | `@DisplayFlag(lang)` |
| Per-language setting key | `"genxml/settings/heading" + lang` |
