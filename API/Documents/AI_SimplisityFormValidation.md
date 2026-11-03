# Form Validation Pattern — Simplisity (DNNrocket) + jQuery Validate Bridge

## Purpose
This document describes a reusable pattern for adding **jQuery Validate** to any
form field group controlled by the DNNrocket `simplisity_click` framework (used in
`RocketIntraCRM`, `RocketIntra`, and related DNNrocket modules). It is written so
an AI assistant can apply the same pattern consistently to any future form,
whether hosted inside DNN or a standalone page.

---

## Background: How `simplisity_click` Triggers Validation

DNNrocket's client-side framework (`simplisity.js`) drives server actions via a
clickable element carrying data attributes:

```html
<div class="button simplisity_click a-validateform"
     s-cmd="some_command"
     s-before="validateForm"
     s-post="#targetContainer">
    Submit
</div>
```

| Attribute | Purpose |
|---|---|
| `s-cmd` | Server-side command to invoke via AJAX. |
| `s-before` | Name of a JS function called **before** the AJAX request fires. |
| `s-post` | CSS selector of the container whose fields are serialized and posted. |
| `s-stop` | If set to `"stop"` on the element, `simplisity.js` **aborts** the request instead of firing `s-cmd`. Absence (or any other value) allows the post to proceed. |

The important architectural point:

> **`simplisity.js` only knows about `s-before` and `s-stop`. It has no built-in
> validation logic of its own.** Validation itself must be done by whatever
> function is named in `s-before`. That function's only job (from
> `simplisity.js`'s point of view) is to set or clear `s-stop` on the clickable
> element(s) before the post happens.

This means **jQuery Validate (or any other validation library) works completely
normally** — it does not need to know it is being invoked by `simplisity.js`.
The only "glue" code required is a small bridge function that:

1. Asks jQuery Validate "is this valid?"
2. Translates the answer into `s-stop` being present or absent.

---

## The Bridge Function Pattern

```javascript
function validateForm() {
    if ($('#field1, #field2, #field3').valid()) {
        $('.a-validateform').removeAttr('s-stop');
    } else {
        $('.a-validateform').attr('s-stop', 'stop');
    }
}
```

- Replace `#field1, #field2, #field3` with the actual field selector(s) belonging
  to **this specific form/module** — not the whole page.
- Replace `.a-validateform` with whatever class/selector identifies the
  clickable submit element(s) for this form (there may be more than one, e.g.
  conditional buttons rendered via `@if/else`).

---

## Initializing jQuery Validate Safely

### Case A — Standalone page / SPA (you own the `<form>` tag)
Wrap the fields in your own `<form>` and initialize normally:

```html
<form id="myForm">
    <input id="name" name="name" />
    <input id="email" name="email" />
</form>
```

```javascript
$(document).ready(function () {
    $('#myForm').validate({
        rules: {
            name: { required: true, maxlength: 50 },
            email: { required: true, email: true }
        },
        errorPlacement: function (error, element) {
            error.insertAfter(element);
        }
    });
});

function validateForm() {
    if ($('#myForm').valid()) {
        $('.a-validateform').removeAttr('s-stop');
    } else {
        $('.a-validateform').attr('s-stop', 'stop');
    }
}
```

### Case B — Inside DNN (the page already has a single `<form id="Form" runat="server">`)
**Do not nest a second `<form>` tag** — DNN owns the page-level form and nesting
forms is invalid HTML. Instead:

1. Attach the plugin to the existing `#Form`, guarded so multiple
   modules/templates on the same page don't re-initialize it:

   ```javascript
   if (!$('#Form').data('validator')) {
       $('#Form').validate();
   }
   ```

2. Add rules to **only this module's fields**, using `rules('add', ...)` so each
   module can contribute its own rules independently without clobbering others:

   ```javascript
   $('#name').rules('add', { required: true, maxlength: 50 });
   $('#manageremail').rules('add', { email: true });
   ```

3. Validate **only this module's fields** when checking — never call
   `$('#Form').valid()` directly, since that would validate every field on the
   entire DNN page (including unrelated modules):

   ```javascript
   function validateForm() {
       if ($('#name, #manageremail').valid()) {
           $('.a-validateform').removeAttr('s-stop');
       } else {
           $('.a-validateform').attr('s-stop', 'stop');
       }
   }
   ```

---

## Checklist for AI When Applying This Pattern

When asked to "add validation" (or similar) to a form in this codebase or a
similar `simplisity_click`-driven form, follow these steps:

1. **Identify the submit trigger element(s).**
   Look for `class="... simplisity_click ..."` with `s-cmd`, `s-before`,
   `s-post` attributes. Note the function name in `s-before` (create it if it
   doesn't exist) and the class/selector used to mark the button(s), e.g.
   `.a-validateform`.

2. **Identify whether a `<form>` tag is available or must be reused.**
   - If the page is a standalone/non-DNN page: create or use a dedicated
     `<form>` wrapping just this module's fields.
   - If the page is DNN-hosted: reuse the existing `#Form` (or whatever the
     host page's form id is), guard initialization with
     `if (!$('#selector').data('validator'))`, and add rules per-field with
     `rules('add', ...)` instead of a single `rules: {...}` map at init time.

3. **Scope the `.valid()` check to this module's own fields only.**
   Never call `.valid()` on the entire shared host form — always validate a
   specific selector/subset belonging to the current form/module.

4. **Write the bridge function** (`validateForm()` or whatever name is used in
   `s-before`) to toggle `s-stop` based on the `.valid()` result, targeting the
   correct submit button class/selector.

5. **Do not alter the validation library's own behavior.**
   Standard jQuery Validate options (`rules`, `messages`, `required`,
   `email`, `maxlength`, custom methods, `highlight`/`unhighlight`,
   `errorPlacement`) all apply exactly as documented upstream. The only
   DNNrocket-specific addition is the `s-stop` bridge — everything else is
   plain jQuery Validate.

6. **If the field helpers emit their own markup** (e.g. a Razor `@TextBox()`
   helper), ensure the generated `id`/`name` attributes match what the
   validation rules reference. Prefer keying rules off `id` via
   `rules('add', ...)` if `name` attributes aren't guaranteed to be present or
   unique on the host page.

---

## Minimal End-to-End Example (DNN-hosted)

```html
<div id="myModuleFields">
    <input id="name" name="name" />
    <input id="manageremail" name="manageremail" />
</div>

<div class="button a-validateform simplisity_click"
     s-cmd="save_record"
     s-before="validateForm"
     s-post="#myModuleFields">
    Save
</div>

<script>
    function validateForm() {
        if ($('#name, #manageremail').valid()) {
            $('.a-validateform').removeAttr('s-stop');
        } else {
            $('.a-validateform').attr('s-stop', 'stop');
        }
    }

    $(document).ready(function () {
        if (!$('#Form').data('validator')) {
            $('#Form').validate();
        }
        $('#name').rules('add', { required: true, maxlength: 50 });
        $('#manageremail').rules('add', { email: true });
    });
</script>
```