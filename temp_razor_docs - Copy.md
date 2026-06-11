# RocketCDS Razor Tokens and Static Methods

This document provides a comprehensive reference for the Razor tokens and static utility methods available within the RocketCDS framework.

---
## Simplisity.RazorEngineTokens

The `RazorEngineTokens` class is the base class for all Razor tokens in Simplisity. It provides a set of common methods for generating HTML controls and manipulating data within Razor templates.

---

## Token: `AddProcessData`

Adds metadata to the current rendering process. This data can be used by other tokens or the calling module.

---

### Signature

```csharp
@AddProcessData(String metaType, String metaValue)
```

---

### Parameters

| Parameter   | Type   | Description                       | Default / Optional |
| :---------- | :----- | :-------------------------------- | :----------------- |
| `metaType`  | string | The key of the metadata.          | **Required**       |
| `metaValue` | string | The value of the metadata.        | **Required**       |

---

### Returns

*   An `IEncodedString` containing an empty string. The primary purpose is to add data to the process, not to render output.

---

### Example

```razor
@AddProcessData("mycustomkey", "mycustomvalue")
```

---

<details>
<summary>Machine-Readable Metadata (JSON)</summary>

```json
{
  "name": "AddProcessData",
  "type": "token",
  "summary": "Adds metadata to the current rendering process. This data can be used by other tokens or the calling module.",
  "returns": {
	"type": "IEncodedString",
	"description": "Returns an IEncodedString containing an empty string. The primary purpose is to add data to the process, not to render output."
  },
  "parameters": [
	{
	  "name": "metaType",
	  "type": "string",
	  "description": "The key of the metadata.",
	  "required": true
	},
	{
	  "name": "metaValue",
	  "type": "string",
	  "description": "The value of the metadata.",
	  "required": true
	}
  ]
}
```

</details>

---

## Token: `AddPreProcessData`

Adds metadata to a specific cache list that can be used by the module before the Razor template is rendered. This is useful for passing data from a template to the underlying module logic.

---

### Signature

```csharp
@AddPreProcessData(String metaKey, String metaValue, String templateFullName, String moduleId)
```

---

### Parameters

| Parameter          | Type   | Description                                                                                             | Default / Optional |
| :----------------- | :----- | :------------------------------------------------------------------------------------------------------ | :----------------- |
| `metaKey`          | string | The key of the metadata.                                                                                | **Required**       |
| `metaValue`        | string | The value of the metadata.                                                                              | **Required**       |
| `templateFullName` | string | The cache key, typically in the format `{theme}.{templatename}.{templateExtension}`.                      | **Required**       |
| `moduleId`         | string | The ID of the module to identify individual modules, ensuring data is stored for the correct instance. | **Required**       |

---

### Returns

*   An `IEncodedString` containing an empty string.

---

### Example

```razor
@AddPreProcessData("onload", "myfunction()", "mytheme.mytemplate.cshtml", "123")
```

---

<details>
<summary>Machine-Readable Metadata (JSON)</summary>

```json
{
  "name": "AddPreProcessData",
  "type": "token",
  "summary": "Adds metadata to a specific cache list that can be used by the module before the Razor template is rendered.",
  "returns": {
	"type": "IEncodedString",
	"description": "Returns an IEncodedString containing an empty string."
  },
  "parameters": [
	{
	  "name": "metaKey",
	  "type": "string",
	  "description": "The key of the metadata.",
	  "required": true
	},
	{
	  "name": "metaValue",
	  "type": "string",
	  "description": "The value of the metadata.",
	  "required": true
	},
	{
	  "name": "templateFullName",
	  "type": "string",
	  "description": "The cache key, typically in the format `{theme}.{templatename}.{templateExtension}`.",
	  "required": true
	},
	{
	  "name": "moduleId",
	  "type": "string",
	  "description": "The ID of the module to identify individual modules.",
	  "required": true
	}
  ]
}
```

</details>

---

## Token: `HiddenField`

Renders an HTML hidden input field, typically used to store data that needs to be submitted with a form but not visible to the user.

---

### Signature

```csharp
@HiddenField(SimplisityInfo info, string xpath, string attributes = "", string defaultValue = "", bool localized = false, int row = 0, string listname = "")
```

---

### Parameters

| Parameter      | Type          | Description                                                                                             | Default / Optional |
| :------------- | :------------ | :------------------------------------------------------------------------------------------------------ | :----------------- |
| `info`         | SimplisityInfo| The data object containing the value.                                                                   | **Required**       |
| `xpath`        | string        | The XPath expression to select the value from the `info` object.                                        | **Required**       |
| `attributes`   | string        | Additional HTML attributes to apply to the `<input>` tag.                                               | Optional           |
| `defaultValue` | string        | The value to use if the XPath expression does not find a value.                                         | Optional           |
| `localized`    | bool          | If `true`, retrieves the value from the language-specific section of the data.                          | `false`            |
| `row`          | int           | The row index, used for creating unique IDs in list-based scenarios.                                    | `0`                |
| `listname`     | string        | The name of the list, used for creating unique IDs in list-based scenarios.                             | `""`               |

---

### Returns

*   An `IEncodedString` containing the HTML for the `<input type="hidden">` element.

---

### Example

```razor
<!-- Simple hidden field -->
@HiddenField(Model.info, "genxml/hidden/itemid")

<!-- Hidden field with a default value and custom attribute -->
@HiddenField(Model.info, "genxml/config/sortkey", "class='sort-key'", "default-sort")
```

---

<details>
<summary>Machine-Readable Metadata (JSON)</summary>

```json
{
  "name": "HiddenField",
  "type": "token",
  "summary": "Renders an HTML hidden input field.",
  "returns": {
	"type": "IEncodedString",
	"description": "An IEncodedString containing the HTML for the <input type=\"hidden\"> element."
  },
  "parameters": [
	{
	  "name": "info",
	  "type": "SimplisityInfo",
	  "description": "The data object containing the value.",
	  "required": true
	},
	{
	  "name": "xpath",
	  "type": "string",
	  "description": "The XPath expression to select the value from the info object.",
	  "required": true
	},
	{
	  "name": "attributes",
	  "type": "string",
	  "description": "Additional HTML attributes to apply to the <input> tag.",
	  "required": false,
	  "default": ""
	},
	{
	  "name": "defaultValue",
	  "type": "string",
	  "description": "The value to use if the XPath expression does not find a value.",
	  "required": false,
	  "default": ""
	},
	{
	  "name": "localized",
	  "type": "bool",
	  "description": "If true, retrieves the value from the language-specific section of the data.",
	  "required": false,
	  "default": "false"
	},
	{
	  "name": "row",
	  "type": "int",
	  "description": "The row index, used for creating unique IDs in list-based scenarios.",
	  "required": false,
	  "default": "0"
	},
	{
	  "name": "listname",
	  "type": "string",
	  "description": "The name of the list, used for creating unique IDs in list-based scenarios.",
	  "required": false,
	  "default": ""
	}
  ]
}
```

</details>

---

## Token: `TextBox`

Renders an HTML text input field.

---

### Signature

```csharp
@TextBox(SimplisityInfo info, string xpath, string attributes = "", string defaultValue = "", bool localized = false, int row = 0, string listname = "", string type = "text")
```

---

### Parameters

| Parameter      | Type          | Description                                                                                             | Default / Optional |
| :------------- | :------------ | :------------------------------------------------------------------------------------------------------ | :----------------- |
| `info`         | SimplisityInfo| The data object containing the value.                                                                   | **Required**       |
| `xpath`        | string        | The XPath expression to select the value from the `info` object.                                        | **Required**       |
| `attributes`   | string        | Additional HTML attributes to apply to the `<input>` tag.                                               | Optional           |
| `defaultValue` | string        | The value to use if the XPath expression does not find a value.                                         | Optional           |
| `localized`    | bool          | If `true`, retrieves the value from the language-specific section of the data.                          | `false`            |
| `row`          | int           | The row index, used for creating unique IDs in list-based scenarios.                                    | `0`                |
| `listname`     | string        | The name of the list, used for creating unique IDs in list-based scenarios.                             | `""`               |
| `type`         | string        | The `type` attribute of the input (e.g., 'text', 'password', 'email').                                  | `"text"`           |

---

### Returns

*   An `IEncodedString` containing the HTML for the `<input>` element.

---

### Example

```razor
<!-- Standard text box -->
@TextBox(Model.info, "genxml/textbox/firstname", "class='w3-input w3-border'")

<!-- Password field -->
@TextBox(Model.info, "genxml/textbox/password", "class='w3-input'", type: "password")
```

---

<details>
<summary>Machine-Readable Metadata (JSON)</summary>

```json
{
  "name": "TextBox",
  "type": "token",
  "summary": "Renders an HTML text input field.",
  "returns": {
	"type": "IEncodedString",
	"description": "An IEncodedString containing the HTML for the <input> element."
  },
  "parameters": [
	{
	  "name": "info",
	  "type": "SimplisityInfo",
	  "description": "The data object containing the value.",
	  "required": true
	},
	{
	  "name": "xpath",
	  "type": "string",
	  "description": "The XPath expression to select the value from the info object.",
	  "required": true
	},
	{
	  "name": "attributes",
	  "type": "string",
	  "description": "Additional HTML attributes to apply to the <input> tag.",
	  "required": false,
	  "default": ""
	},
	{
	  "name": "defaultValue",
	  "type": "string",
	  "description": "The value to use if the XPath expression does not find a value.",
	  "required": false,
	  "default": ""
	},
	{
	  "name": "localized",
	  "type": "bool",
	  "description": "If true, retrieves the value from the language-specific section of the data.",
	  "required": false,
	  "default": "false"
	},
	{
	  "name": "row",
	  "type": "int",
	  "description": "The row index, used for creating unique IDs in list-based scenarios.",
	  "required": false,
	  "default": "0"
	},
	{
	  "name": "listname",
	  "type": "string",
	  "description": "The name of the list, used for creating unique IDs in list-based scenarios.",
	  "required": false,
	  "default": ""
	},
	{
	  "name": "type",
	  "type": "string",
		"description": "The `type` attribute of the input (e.g., 'text', 'password', 'email').",
			"required": false,
			"default": "text"
		  }
		]
	  }
	  ```

	  </details>

	  ---

	  ## Token: `TextBoxDate`

	  Renders an HTML date input field.

	  ---

	  ### Signature

	  ```csharp
	  @TextBoxDate(SimplisityInfo info, String xpath, String attributes = "", String defaultValue = "", bool localized = false, int row = 0, string listname = "")
	  ```

	  ---

	  ### Parameters

	  | Parameter      | Type          | Description                                                                                             | Default / Optional |
	  | :------------- | :------------ | :------------------------------------------------------------------------------------------------------ | :----------------- |
	  | `info`         | SimplisityInfo| The data object containing the value.                                                                   | **Required**       |
	  | `xpath`        | string        | The XPath expression to select the value from the `info` object.                                        | **Required**       |
	  | `attributes`   | string        | Additional HTML attributes to apply to the `<input>` tag.                                               | Optional           |
	  | `defaultValue` | string        | The value to use if the XPath expression does not find a value.                                         | Optional           |
	  | `localized`    | bool          | If `true`, retrieves the value from the language-specific section of the data.                          | `false`            |
	  | `row`          | int           | The row index, used for creating unique IDs in list-based scenarios.                                    | `0`                |
	  | `listname`     | string        | The name of the list, used for creating unique IDs in list-based scenarios.                             | `""`               |

	  ---

	  ### Returns

	  *   An `IEncodedString` containing the HTML for the `<input type="date">` element.

	  ---

	  ### Example

	  ```razor
	  @TextBoxDate(Model.info, "genxml/data/startdate", "class='w3-input w3-border'")
	  ```

	  ---

	  <details>
	  <summary>Machine-Readable Metadata (JSON)</summary>

	  ```json
	  {
		"name": "TextBoxDate",
		"type": "token",
		"summary": "Renders an HTML date input field.",
		"returns": {
		  "type": "IEncodedString",
		  "description": "An IEncodedString containing the HTML for the <input type=\"date\"> element."
		},
		"parameters": [
		  { "name": "info", "type": "SimplisityInfo", "description": "The data object containing the value.", "required": true },
		  { "name": "xpath", "type": "string", "description": "The XPath expression to select the value from the info object.", "required": true },
		  { "name": "attributes", "type": "string", "description": "Additional HTML attributes to apply to the <input> tag.", "required": false, "default": "" },
		  { "name": "defaultValue", "type": "string", "description": "The value to use if the XPath expression does not find a value.", "required": false, "default": "" },
		  { "name": "localized", "type": "bool", "description": "If true, retrieves the value from the language-specific section of the data.", "required": false, "default": "false" },
		  { "name": "row", "type": "int", "description": "The row index, used for creating unique IDs in list-based scenarios.", "required": false, "default": "0" },
		  { "name": "listname", "type": "string", "description": "The name of the list, used for creating unique IDs in list-based scenarios.", "required": false, "default": "" }
		]
	  }
	  ```
	  </details>

	  ---

	  ## Token: `TextArea`

	  Renders an HTML textarea element.

	  ---

	  ### Signature

	  ```csharp
	  @TextArea(SimplisityInfo info, String xpath, String attributes = "", String defaultValue = "", bool localized = false, int row = 0, string listname = "")
	  ```

	  ---

	  ### Parameters

	  | Parameter      | Type          | Description                                                                                             | Default / Optional |
	  | :------------- | :------------ | :------------------------------------------------------------------------------------------------------ | :----------------- |
	  | `info`         | SimplisityInfo| The data object containing the value.                                                                   | **Required**       |
	  | `xpath`        | string        | The XPath expression to select the value from the `info` object.                                        | **Required**       |
	  | `attributes`   | string        | Additional HTML attributes to apply to the `<textarea>` tag.                                            | Optional           |
	  | `defaultValue` | string        | The value to use if the XPath expression does not find a value.                                         | Optional           |
	  | `localized`    | bool          | If `true`, retrieves the value from the language-specific section of the data.                          | `false`            |
	  | `row`          | int           | The row index, used for creating unique IDs in list-based scenarios.                                    | `0`                |
	  | `listname`     | string        | The name of the list, used for creating unique IDs in list-based scenarios.                             | `""`               |

	  ---

	  ### Returns

	  *   An `IEncodedString` containing the HTML for the `<textarea>` element.

	  ---

	  ### Example

	  ```razor
	  @TextArea(Model.info, "genxml/data/description", "class='w3-input w3-border' rows='4'")
	  ```

	  ---

	  <details>
	  <summary>Machine-Readable Metadata (JSON)</summary>

	  ```json
	  {
		"name": "TextArea",
		"type": "token",
		"summary": "Renders an HTML textarea element.",
		"returns": {
		  "type": "IEncodedString",
		  "description": "An IEncodedString containing the HTML for the <textarea> element."
		},
		"parameters": [
		  { "name": "info", "type": "SimplisityInfo", "description": "The data object containing the value.", "required": true },
		  { "name": "xpath", "type": "string", "description": "The XPath expression to select the value from the info object.", "required": true },
		  { "name": "attributes", "type": "string", "description": "Additional HTML attributes to apply to the <textarea> tag.", "required": false, "default": "" },
		  { "name": "defaultValue", "type": "string", "description": "The value to use if the XPath expression does not find a value.", "required": false, "default": "" },
		  { "name": "localized", "type": "bool", "description": "If true, retrieves the value from the language-specific section of the data.", "required": false, "default": "false" },
		  { "name": "row", "type": "int", "description": "The row index, used for creating unique IDs in list-based scenarios.", "required": false, "default": "0" },
		  { "name": "listname", "type": "string", "description": "The name of the list, used for creating unique IDs in list-based scenarios.", "required": false, "default": "" }
		]
	  }
	  ```
	  </details>

	  ---

	  ## Token: `CheckBox`

	  Renders an HTML checkbox input field with a label.

	  ---

	  ### Signature

	  ```csharp
	  @CheckBox(SimplisityInfo info, String xpath, String text, String attributes = "", Boolean defaultValue = false, bool localized = false, int row = 0, string listname = "")
	  ```

	  ---

	  ### Parameters

	  | Parameter      | Type          | Description                                                                                             | Default / Optional |
	  | :------------- | :------------ | :------------------------------------------------------------------------------------------------------ | :----------------- |
	  | `info`         | SimplisityInfo| The data object containing the value.                                                                   | **Required**       |
	  | `xpath`        | string        | The XPath expression to select the value from the `info` object.                                        | **Required**       |
	  | `text`         | string        | The text to display in the label for the checkbox.                                                      | **Required**       |
	  | `attributes`   | string        | Additional HTML attributes to apply to the `<input>` tag.                                               | Optional           |
	  | `defaultValue` | bool          | The default checked state if the XPath expression does not find a value.                                | `false`            |
	  | `localized`    | bool          | If `true`, retrieves the value from the language-specific section of the data.                          | `false`            |
	  | `row`          | int           | The row index, used for creating unique IDs in list-based scenarios.                                    | `0`                |
	  | `listname`     | string        | The name of the list, used for creating unique IDs in list-based scenarios.                             | `""`               |

	  ---

	  ### Returns

	  *   An `IEncodedString` containing the HTML for the `<input type="checkbox">` and `<label>` elements.

	  ---

	  ### Example

	  ```razor
	  @CheckBox(Model.info, "genxml/data/isactive", "Is Active", "class='w3-check'")
	  ```

	  ---

	  <details>
	  <summary>Machine-Readable Metadata (JSON)</summary>

	  ```json
	  {
		  "name": "CheckBox",
		  "type": "token",
		  "summary": "Renders an HTML checkbox input field with a label.",
		  "returns": {
			"type": "IEncodedString",
			"description": "An IEncodedString containing the HTML for the <input type=\"checkbox\"> and <label> elements."
		  },
		  "parameters": [
			{ "name": "info", "type": "SimplisityInfo", "description": "The data object containing the value.", "required": true },
			{ "name": "xpath", "type": "string", "description": "The XPath expression to select the value from the info object.", "required": true },
			{ "name": "text", "type": "string", "description": "The text to display in the label for the checkbox.", "required": true },
			{ "name": "attributes", "type": "string", "description": "Additional HTML attributes to apply to the <input> tag.", "required": false, "default": "" },
			{ "name": "defaultValue", "type": "bool", "description": "The default checked state if the XPath expression does not find a value.", "required": false, "default": "false" },
			{ "name": "localized", "type": "bool", "description": "If true, retrieves the value from the language-specific section of the data.", "required": false, "default": "false" },
			{ "name": "row", "type": "int", "description": "The row index, used for creating unique IDs in list-based scenarios.", "required": false, "default": "0" },
			{ "name": "listname", "type": "string", "description": "The name of the list, used for creating unique IDs in list-based scenarios.", "required": false, "default": "" }
		  ]
		}
		```
		</details>

		---

		## Token: `CheckBoxList`

		Renders a list of checkboxes from a data source.

		---

		### Signature

		```csharp
		// From Dictionary
		@CheckBoxList(SimplisityInfo info, string xpath, Dictionary<string, string> dataDictionary, string attributes = "", bool defaultValue = false, bool localized = false, int row = 0, string listname = "")

		// From comma-separated strings
		@CheckBoxList(SimplisityInfo info, string xpath, string datavalue, string datatext, string attributes = "", bool defaultValue = false, bool localized = false, int row = 0, string listname = "")
		```

		---

		### Parameters

		| Parameter        | Type                    | Description                                                                                             | Default / Optional |
		| :--------------- | :---------------------- | :------------------------------------------------------------------------------------------------------ | :----------------- |
		| `info`           | SimplisityInfo          | The data object containing the selected values.                                                         | **Required**       |
		| `xpath`          | string                  | The base XPath expression to store the selected values.                                                 | **Required**       |
		| `dataDictionary` | Dictionary<string,string> | A dictionary where keys are checkbox values and values are display text.                                | **Required** (Overload 1) |
		| `datavalue`      | string                  | A comma-separated string of checkbox values.                                                            | **Required** (Overload 2) |
		| `datatext`       | string                  | A comma-separated string of checkbox display texts.                                                     | **Required** (Overload 2) |
		| `attributes`     | string                  | Additional HTML attributes to apply to the container `<div>`.                                           | Optional           |
		| `defaultValue`   | bool                    | The default checked state for items if no value is found.                                               | `false`            |
		| `localized`      | bool                    | If `true`, retrieves values from the language-specific section of the data.                             | `false`            |
		| `row`            | int                     | The row index, used for creating unique IDs in list-based scenarios.                                    | `0`                |
		| `listname`       | string                  | The name of the list, used for creating unique IDs in list-based scenarios.                             | `""`               |

		---

		### Returns

		*   An `IEncodedString` containing the HTML for the `<div>` containing the checkbox list.

		---

		### Example

		```razor
		@{
			var options = new Dictionary<string, string> { { "opt1", "Option 1" }, { "opt2", "Option 2" } };
		}
		@CheckBoxList(Model.info, "genxml/data/options", options, "class='w3-container'")

		@CheckBoxList(Model.info, "genxml/data/options", "opt1,opt2", "Option 1,Option 2", "class='w3-container'")
		```

		---

		<details>
		<summary>Machine-Readable Metadata (JSON)</summary>

		```json
		{
		  "name": "CheckBoxList",
		  "type": "token",
		  "summary": "Renders a list of checkboxes from a data source.",
		  "returns": {
			"type": "IEncodedString",
			"description": "An IEncodedString containing the HTML for the <div> containing the checkbox list."
		  },
		  "parameters": [
			{ "name": "info", "type": "SimplisityInfo", "description": "The data object containing the selected values.", "required": true },
			{ "name": "xpath", "type": "string", "description": "The base XPath expression to store the selected values.", "required": true },
			{ "name": "dataDictionary", "type": "Dictionary<string,string>", "description": "A dictionary where keys are checkbox values and values are display text.", "required": true, "overload": "1" },
			{ "name": "datavalue", "type": "string", "description": "A comma-separated string of checkbox values.", "required": true, "overload": "2" },
			{ "name": "datatext", "type": "string", "description": "A comma-separated string of checkbox display texts.", "required": true, "overload": "2" },
			{ "name": "attributes", "type": "string", "description": "Additional HTML attributes to apply to the container <div>.", "required": false, "default": "" },
			{ "name": "defaultValue", "type": "bool", "description": "The default checked state for items if no value is found.", "required": false, "default": "false" },
			{ "name": "localized", "type": "bool", "description": "If true, retrieves values from the language-specific section of the data.", "required": false, "default": "false" },
			{ "name": "row", "type": "int", "description": "The row index, used for creating unique IDs in list-based scenarios.", "required": false, "default": "0" },
			{ "name": "listname", "type": "string", "description": "The name of the list, used for creating unique IDs in list-based scenarios.", "required": false, "default": "" }
		  ]
		}
		```
		</details>

---

## Token: `RadioButtonList`

Renders a list of radio buttons from a data source.

---

### Signature

```csharp
// From Dictionary
@RadioButtonList(SimplisityInfo info, string xpath, Dictionary<string, string> dataDictionary, string attributes = "", string defaultValue = "", string labelattributes = "", bool localized = false, int row = 0, string listname = "", string inputclass = "")

// From comma-separated strings
@RadioButtonList(SimplisityInfo info, string xpath, string datavalue, string datatext, string attributes = "", string defaultValue = "",string labelattributes = "", bool localized = false, int row = 0, string listname = "", string inputclass = "")
```

---

### Parameters

| Parameter        | Type                    | Description                                                                                             | Default / Optional |
| :--------------- | :---------------------- | :------------------------------------------------------------------------------------------------------ | :----------------- |
| `info`           | SimplisityInfo          | The data object containing the selected value.                                                          | **Required**       |
| `xpath`          | string                  | The XPath expression to store the selected value.                                                       | **Required**       |
| `dataDictionary` | Dictionary<string,string> | A dictionary where keys are radio button values and values are display text.                            | **Required** (Overload 1) |
| `datavalue`      | string                  | A comma-separated string of radio button values.                                                        | **Required** (Overload 2) |
| `datatext`       | string                  | A comma-separated string of radio button display texts.                                                 | **Required** (Overload 2) |
| `attributes`     | string                  | Additional HTML attributes to apply to the container `<div>`.                                           | Optional           |
| `defaultValue`   | string                  | The default selected value if no value is found.                                                        | Optional           |
| `labelattributes`| string                  | Additional HTML attributes to apply to the `<label>` tags.                                              | Optional           |
| `localized`      | bool                    | If `true`, retrieves the value from the language-specific section of the data.                          | `false`            |
| `row`            | int                     | The row index, used for creating unique IDs in list-based scenarios.                                    | `0`                |
| `listname`       | string                  | The name of the list, used for creating unique IDs in list-based scenarios.                             | `""`               |
| `inputclass`     | string                  | CSS class to apply to each `<input>` element.                                                           | Optional           |

---

### Returns

*   An `IEncodedString` containing the HTML for the `<div>` containing the radio button list.

---

### Example

```razor
@{
	var genders = new Dictionary<string, string> { { "m", "Male" }, { "f", "Female" } };
}
@RadioButtonList(Model.info, "genxml/data/gender", genders, "class='w3-container'")

@RadioButtonList(Model.info, "genxml/data/gender", "m,f", "Male,Female", "class='w3-container'")
```

---

<details>
<summary>Machine-Readable Metadata (JSON)</summary>

```json
{
  "name": "RadioButtonList",
  "type": "token",
  "summary": "Renders a list of radio buttons from a data source.",
  "returns": {
	"type": "IEncodedString",
	"description": "An IEncodedString containing the HTML for the <div> containing the radio button list."
  },
  "parameters": [
	{ "name": "info", "type": "SimplisityInfo", "description": "The data object containing the selected value.", "required": true },
	{ "name": "xpath", "type": "string", "description": "The XPath expression to store the selected value.", "required": true },
	{ "name": "dataDictionary", "type": "Dictionary<string,string>", "description": "A dictionary where keys are radio button values and values are display text.", "required": true, "overload": "1" },
	{ "name": "datavalue", "type": "string", "description": "A comma-separated string of radio button values.", "required": true, "overload": "2" },
	{ "name": "datatext", "type": "string", "description": "A comma-separated string of radio button display texts.", "required": true, "overload": "2" },
	{ "name": "attributes", "type": "string", "description": "Additional HTML attributes to apply to the container <div>.", "required": false, "default": "" },
	{ "name": "defaultValue", "type": "string", "description": "The default selected value if no value is found.", "required": false, "default": "" },
	{ "name": "labelattributes", "type": "string", "description": "Additional HTML attributes to apply to the <label> tags.", "required": false, "default": "" },
	{ "name": "localized", "type": "bool", "description": "If true, retrieves the value from the language-specific section of the data.", "required": false, "default": "false" },
	{ "name": "row", "type": "int", "description": "The row index, used for creating unique IDs in list-based scenarios.", "required": false, "default": "0" },
	{ "name": "listname", "type": "string", "description": "The name of the list, used for creating unique IDs in list-based scenarios.", "required": false, "default": "" },
	{ "name": "inputclass", "type": "string", "description": "CSS class to apply to each <input> element.", "required": false, "default": "" }
  ]
}
```
</details>

---

## Token: `DropDownList`

Renders an HTML dropdown list (select element) from a data source.

---

### Signature

```csharp
// From Dictionary
@DropDownList(SimplisityInfo info, String xpath, Dictionary<string,string> dataDictionary, String attributes = "", String defaultValue = "", bool localized = false, int row = 0, string listname = "")

// From comma-separated strings
@DropDownList(SimplisityInfo info, String xpath, String datavalue, String datatext, String attributes = "", String defaultValue = "", bool localized = false, int row = 0, string listname = "")
```

---

### Parameters

| Parameter        | Type                    | Description                                                                                             | Default / Optional |
| :--------------- | :---------------------- | :------------------------------------------------------------------------------------------------------ | :----------------- |
| `info`           | SimplisityInfo          | The data object containing the selected value.                                                          | **Required**       |
| `xpath`          | string                  | The XPath expression to store the selected value.                                                       | **Required**       |
| `dataDictionary` | Dictionary<string,string> | A dictionary where keys are option values and values are option display text.                           | **Required** (Overload 1) |
| `datavalue`      | string                  | A comma-separated string of option values.                                                              | **Required** (Overload 2) |
| `datatext`       | string                  | A comma-separated string of option display texts.                                                       | **Required** (Overload 2) |
| `attributes`     | string                  | Additional HTML attributes to apply to the `<select>` element.                                          | Optional           |
| `defaultValue`   | string                  | The default selected value if no value is found.                                                        | Optional           |
| `localized`      | bool                    | If `true`, retrieves the value from the language-specific section of the data.                          | `false`            |
| `row`            | int                     | The row index, used for creating unique IDs in list-based scenarios.                                    | `0`                |
| `listname`       | string                  | The name of the list, used for creating unique IDs in list-based scenarios.                             | `""`               |

---

### Returns

*   An `IEncodedString` containing the HTML for the `<select>` element.

---

### Example

```razor
@{
	var countries = new Dictionary<string, string> { { "US", "United States" }, { "CA", "Canada" } };
}
@DropDownList(Model.info, "genxml/data/country", countries, "class='w3-select'")

@DropDownList(Model.info, "genxml/data/country", "US,CA", "United States,Canada", "class='w3-select'")
```

---

<details>
<summary>Machine-Readable Metadata (JSON)</summary>

```json
{
  "name": "DropDownList",
  "type": "token",
  "summary": "Renders an HTML dropdown list (select element) from a data source.",
  "returns": {
	"type": "IEncodedString",
	"description": "An IEncodedString containing the HTML for the <select> element."
  },
  "parameters": [
	{ "name": "info", "type": "SimplisityInfo", "description": "The data object containing the selected value.", "required": true },
	{ "name": "xpath", "type": "string", "description": "The XPath expression to store the selected value.", "required": true },
	{ "name": "dataDictionary", "type": "Dictionary<string,string>", "description": "A dictionary where keys are option values and values are option display text.", "required": true, "overload": "1" },
	{ "name": "datavalue", "type": "string", "description": "A comma-separated string of option values.", "required": true, "overload": "2" },
	{ "name": "datatext", "type": "string", "description": "A comma-separated string of option display texts.", "required": true, "overload": "2" },
	{ "name": "attributes", "type": "string", "description": "Additional HTML attributes to apply to the <select> element.", "required": false, "default": "" },
	{ "name": "defaultValue", "type": "string", "description": "The default selected value if no value is found.", "required": false, "default": "" },
	{ "name": "localized", "type": "bool", "description": "If true, retrieves the value from the language-specific section of the data.", "required": false, "default": "false" },
	{ "name": "row", "type": "int", "description": "The row index, used for creating unique IDs in list-based scenarios.", "required": false, "default": "0" },
	{ "name": "listname", "type": "string", "description": "The name of the list, used for creating unique IDs in list-based scenarios.", "required": false, "default": "" }
  ]
}
```
</details>

---

## Token: `SortableListIndex`

Outputs the hidden input fields required to correctly process the sort order of a list.

---

### Signature

```csharp
@SortableListIndex(SimplisityInfo info, int row)
```

---

### Parameters

| Parameter | Type           | Description                                   | Default / Optional |
| :-------- | :------------- | :-------------------------------------------- | :----------------- |
| `info`    | SimplisityInfo | The data object for the list item.            | **Required**       |
| `row`     | int            | The current row index of the item in the list. | **Required**       |

---

### Returns

*   An `IEncodedString` containing the HTML for the hidden input fields.

---

### Example

```razor
@foreach (var item in Model.List)
{
	<div class="list-item">
		@SortableListIndex(item, item.GetXmlPropertyInt("genxml/hidden/sortableindex"))
		<span>@item.GetXmlProperty("genxml/data/name")</span>
	</div>
}
```

---

<details>
<summary>Machine-Readable Metadata (JSON)</summary>

```json
{
  "name": "SortableListIndex",
  "type": "token",
  "summary": "Outputs the hidden input fields required to correctly process the sort order of a list.",
  "returns": {
	"type": "IEncodedString",
	"description": "An IEncodedString containing the HTML for the hidden input fields."
  },
  "parameters": [
	{ "name": "info", "type": "SimplisityInfo", "description": "The data object for the list item.", "required": true },
	{ "name": "row", "type": "int", "description": "The current row index of the item in the list.", "required": true }
  ]
}
```
</details>

---

## Token: `EmailOf`

Formats an email address into a `mailto:` link.

---

### Signature

```csharp
@EmailOf(SimplisityInfo info, String xpath, string subject = "", string visibleText = "")
```

---

### Parameters

| Parameter   | Type           | Description                                                     | Default / Optional |
| :---------- | :------------- | :-------------------------------------------------------------- | :----------------- |
| `info`      | SimplisityInfo | The data object containing the email address.                   | **Required**       |
| `xpath`     | string         | The XPath expression to select the email address from the `info` object. | **Required**       |
| `subject`   | string         | The subject line for the email.                                 | Optional           |
| `visibleText`| string         | The visible text for the link. If empty, the email address is used. | Optional           |

---

### Returns

*   An `IEncodedString` containing the HTML `<a>` tag for the `mailto:` link.

---

### Example

```razor
@EmailOf(Model.info, "genxml/data/contactemail", "Inquiry", "Contact Us")
```

---

<details>
<summary>Machine-Readable Metadata (JSON)</summary>

```json
{
  "name": "EmailOf",
  "type": "token",
  "summary": "Formats an email address into a mailto: link.",
  "returns": {
	"type": "IEncodedString",
	"description": "An IEncodedString containing the HTML <a> tag for the mailto: link."
  },
  "parameters": [
	{ "name": "info", "type": "SimplisityInfo", "description": "The data object containing the email address.", "required": true },
	{ "name": "xpath", "type": "string", "description": "The XPath expression to select the email address from the info object.", "required": true },
	{ "name": "subject", "type": "string", "description": "The subject line for the email.", "required": false, "default": "" },
	{ "name": "visibleText", "type": "string", "description": "The visible text for the link. If empty, the email address is used.", "required": false, "default": "" }
  ]
}
```
</details>

---

## Token: `HtmlOf`

Decodes an HTML string, rendering the raw HTML.

---

### Signature

```csharp
// From SimplisityInfo
@HtmlOf(SimplisityInfo info, String xpath)

// From string
@HtmlOf(String htmlString)
```

---

### Parameters

| Parameter    | Type           | Description                                                     | Default / Optional |
| :----------- | :------------- | :-------------------------------------------------------------- | :----------------- |
| `info`       | SimplisityInfo | The data object containing the HTML string.                     | **Required** (Overload 1) |
| `xpath`      | string         | The XPath expression to select the HTML string from the `info` object. | **Required** (Overload 1) |
| `htmlString` | string         | The HTML string to decode.                                      | **Required** (Overload 2) |

---

### Returns

*   An `IEncodedString` containing the decoded HTML.

---

### Example

```razor
@HtmlOf(Model.info, "genxml/data/htmldescription")

@HtmlOf("&lt;h1&gt;Title&lt;/h1&gt;")
```

---

<details>
<summary>Machine-Readable Metadata (JSON)</summary>

```json
{
  "name": "HtmlOf",
  "type": "token",
  "summary": "Decodes an HTML string, rendering the raw HTML.",
  "returns": {
	"type": "IEncodedString",
	"description": "An IEncodedString containing the decoded HTML."
  },
  "parameters": [
	{ "name": "info", "type": "SimplisityInfo", "description": "The data object containing the HTML string.", "required": true, "overload": "1" },
	{ "name": "xpath", "type": "string", "description": "The XPath expression to select the HTML string from the info object.", "required": true, "overload": "1" },
	{ "name": "htmlString", "type": "string", "description": "The HTML string to decode.", "required": true, "overload": "2" }
  ]
}
```
</details>

---

## Token: `DateOf`

Formats a date value into a string based on a culture and format.

---

### Signature

```csharp
// From SimplisityInfo
@DateOf(SimplisityInfo info, String xpath, String cultureCode, String format = "d")
@DateOf(SimplisityInfo info, String xpath, bool displayEmpty, String cultureCode, String format = "d")

// From DateTime
@DateOf(DateTime dateTime, String cultureCode, String format = "g")
```

---

### Parameters

| Parameter      | Type           | Description                                                                 | Default / Optional |
| :------------- | :------------- | :-------------------------------------------------------------------------- | :----------------- |
| `info`         | SimplisityInfo | The data object containing the date value.                                  | **Required** (Overload 1, 2) |
| `xpath`        | string         | The XPath expression to select the date value from the `info` object.       | **Required** (Overload 1, 2) |
| `displayEmpty` | bool           | If `true`, returns an empty string if the date is not set.                  | `false` (Overload 2) |
| `cultureCode`  | string         | The culture code to use for formatting (e.g., "en-US", "fr-FR").            | **Required**       |
| `format`       | string         | The date format string (e.g., "d", "D", "yyyy-MM-dd").                      | Optional           |
| `dateTime`     | DateTime       | The DateTime object to format.                                              | **Required** (Overload 3) |

---

### Returns

*   An `IEncodedString` containing the formatted date string.

---

### Example

```razor
@DateOf(Model.info, "genxml/data/publishdate", "en-US", "D")

@DateOf(DateTime.Now, "en-GB", "g")
```

---

<details>
<summary>Machine-Readable Metadata (JSON)</summary>

```json
{
  "name": "DateOf",
  "type": "token",
  "summary": "Formats a date value into a string based on a culture and format.",
  "returns": {
	"type": "IEncodedString",
	"description": "An IEncodedString containing the formatted date string."
  },
  "parameters": [
	{ "name": "info", "type": "SimplisityInfo", "description": "The data object containing the date value.", "required": true, "overload": "1,2" },
	{ "name": "xpath", "type": "string", "description": "The XPath expression to select the date value from the info object.", "required": true, "overload": "1,2" },
	{ "name": "displayEmpty", "type": "bool", "description": "If true, returns an empty string if the date is not set.", "required": false, "overload": "2", "default": "false" },
	{ "name": "cultureCode", "type": "string", "description": "The culture code to use for formatting (e.g., \"en-US\", \"fr-FR\").", "required": true },
	{ "name": "format", "type": "string", "description": "The date format string (e.g., \"d\", \"D\", \"yyyy-MM-dd\").", "required": false, "default": "d or g" },
	{ "name": "dateTime", "type": "DateTime", "description": "The DateTime object to format.", "required": true, "overload": "3" }
  ]
}
```
</details>

---

## Token: `Succinct`

Shortens a string to a specified length and appends "..." if the string is longer than the specified size.

---

### Signature

```csharp
@Succinct(string value, int size, bool showdots = true)
```

---

### Parameters

| Parameter | Type    | Description                                       | Default / Optional |
| :-------- | :------ | :------------------------------------------------ | :----------------- |
| `value`   | string  | The string to shorten.                            | **Required**       |
| `size`    | int     | The maximum length of the returned string.        | **Required**       |
| `showdots`| bool    | If `true`, appends "..." to the truncated string. | `true`             |

---

### Returns

*   An `IEncodedString` containing the shortened string.

---

### Example

```razor
@Succinct("This is a very long string.", 10)
// Output: This is a...
```

---

<details>
<summary>Machine-Readable Metadata (JSON)</summary>

```json
{
  "name": "Succinct",
  "type": "token",
  "summary": "Shortens a string to a specified length and appends \"...\" if the string is longer than the specified size.",
  "returns": {
	"type": "IEncodedString",
	"description": "An IEncodedString containing the shortened string."
  },
  "parameters": [
	{ "name": "value", "type": "string", "description": "The string to shorten.", "required": true },
	{ "name": "size", "type": "int", "description": "The maximum length of the returned string.", "required": true },
	{ "name": "showdots", "type": "bool", "description": "If true, appends \"...\" to the truncated string.", "required": false, "default": "true" }
  ]
}
```
</details>

---

## Token: `BreakOf`

Replaces newline characters in a string with HTML `<br/>` tags.

---

### Signature

```csharp
// From SimplisityInfo
@BreakOf(SimplisityInfo info, String xpath)

// From IEncodedString
@BreakOf(IEncodedString strIn)

// From string
@BreakOf(String strIn)
```

---

### Parameters

| Parameter | Type             | Description                                                     | Default / Optional |
| :-------- | :--------------- | :-------------------------------------------------------------- | :----------------- |
| `info`    | SimplisityInfo   | The data object containing the string.                          | **Required** (Overload 1) |
| `xpath`   | string           | The XPath expression to select the string from the `info` object. | **Required** (Overload 1) |
| `strIn`   | IEncodedString or string | The input string.                                       | **Required** (Overload 2, 3) |

---

### Returns

*   An `IEncodedString` with newlines converted to `<br/>` tags.

---

### Example

```razor
@BreakOf("Line 1\nLine 2")
```

---

<details>
<summary>Machine-Readable Metadata (JSON)</summary>

```json
{
  "name": "BreakOf",
  "type": "token",
  "summary": "Replaces newline characters in a string with HTML <br/> tags.",
  "returns": {
	"type": "IEncodedString",
	"description": "An IEncodedString with newlines converted to <br/> tags."
  },
  "parameters": [
	{ "name": "info", "type": "SimplisityInfo", "description": "The data object containing the string.", "required": true, "overload": "1" },
	{ "name": "xpath", "type": "string", "description": "The XPath expression to select the string from the info object.", "required": true, "overload": "1" },
	{ "name": "strIn", "type": "IEncodedString or string", "description": "The input string.", "required": true, "overload": "2,3" }
  ]
}
```
</details>

---

## Token: `CheckBoxListOf`

Renders a `<ul>` list of the selected items from a checkbox list.

---

### Signature

```csharp
@CheckBoxListOf(SimplisityInfo info, String xpath, String datavalue, String datatext, String attributes = "")
```

---

### Parameters

| Parameter  | Type           | Description                                                              | Default / Optional |
| :--------- | :------------- | :----------------------------------------------------------------------- | :----------------- |
| `info`     | SimplisityInfo | The data object containing the selected values.                          | **Required**       |
| `xpath`    | string         | The base XPath expression where the selected values are stored.          | **Required**       |
| `datavalue`| string         | A comma-separated string of all possible checkbox values.                | **Required**       |
| `datatext` | string         | A comma-separated string of all possible checkbox display texts.         | **Required**       |
| `attributes`| string         | Additional HTML attributes to apply to the `<ul>` element.               | Optional           |

---

### Returns

*   An `IEncodedString` containing the HTML for the `<ul>` list.

---

### Example

```razor
@CheckBoxListOf(Model.info, "genxml/data/options", "opt1,opt2", "Option 1,Option 2", "class='w3-ul'")
```

---

<details>
<summary>Machine-Readable Metadata (JSON)</summary>

```json
{
  "name": "CheckBoxListOf",
  "type": "token",
  "summary": "Renders a <ul> list of the selected items from a checkbox list.",
  "returns": {
	"type": "IEncodedString",
	"description": "An IEncodedString containing the HTML for the <ul> list."
  },
  "parameters": [
	{ "name": "info", "type": "SimplisityInfo", "description": "The data object containing the selected values.", "required": true },
	{ "name": "xpath", "type": "string", "description": "The base XPath expression where the selected values are stored.", "required": true },
	{ "name": "datavalue", "type": "string", "description": "A comma-separated string of all possible checkbox values.", "required": true },
	{ "name": "datatext", "type": "string", "description": "A comma-separated string of all possible checkbox display texts.", "required": true },
	{ "name": "attributes", "type": "string", "description": "Additional HTML attributes to apply to the <ul> element.", "required": false, "default": "" }
  ]
}
```
</details>

---

## Token: `FileSelectList`

Renders a dropdown list of files from a specified directory.

---

### Signature

```csharp
// From selected filename
@FileSelectList(string selectedfilename, String mappathRootFolder, String attributes = "", Boolean allowEmpty = true)

// From SimplisityInfo
@FileSelectList(SimplisityInfo info, String xpath, String mappathRootFolder, String attributes = "", Boolean allowEmpty = true, bool localized = false)
```

---

### Parameters

| Parameter         | Type           | Description                                                              | Default / Optional |
| :---------------- | :------------- | :----------------------------------------------------------------------- | :----------------- |
| `selectedfilename`| string         | The filename to pre-select in the list.                                  | **Required** (Overload 1) |
| `mappathRootFolder`| string         | The physical path to the directory containing the files.                 | **Required**       |
| `attributes`      | string         | Additional HTML attributes to apply to the `<select>` element.           | Optional           |
| `allowEmpty`      | bool           | If `true`, includes an empty option in the list.                         | `true`             |
| `info`            | SimplisityInfo | The data object containing the selected value.                           | **Required** (Overload 2) |
| `xpath`           | string         | The XPath expression to select and store the value in the `info` object. | **Required** (Overload 2) |
| `localized`       | bool           | If `true`, uses the language-specific section of the data.               | `false`            |

---

### Returns

*   An `IEncodedString` containing the HTML for the `<select>` element.

---

### Example

```razor
@FileSelectList("default.jpg", Server.MapPath("/images"), "class='w3-select'")

@FileSelectList(Model.info, "genxml/data/image", Server.MapPath("/images"))
```

---

<details>
<summary>Machine-Readable Metadata (JSON)</summary>

```json
{
  "name": "FileSelectList",
  "type": "token",
  "summary": "Renders a dropdown list of files from a specified directory.",
  "returns": {
	"type": "IEncodedString",
	"description": "An IEncodedString containing the HTML for the <select> element."
  },
  "parameters": [
	{ "name": "selectedfilename", "type": "string", "description": "The filename to pre-select in the list.", "required": true, "overload": "1" },
	{ "name": "mappathRootFolder", "type": "string", "description": "The physical path to the directory containing the files.", "required": true },
	{ "name": "attributes", "type": "string", "description": "Additional HTML attributes to apply to the <select> element.", "required": false, "default": "" },
	{ "name": "allowEmpty", "type": "bool", "description": "If true, includes an empty option in the list.", "required": false, "default": "true" },
	{ "name": "info", "type": "SimplisityInfo", "description": "The data object containing the selected value.", "required": true, "overload": "2" },
	{ "name": "xpath", "type": "string", "description": "The XPath expression to select and store the value in the info object.", "required": true, "overload": "2" },
	{ "name": "localized", "type": "bool", "description": "If true, uses the language-specific section of the data.", "required": false, "default": "false" }
  ]
}
```
</details>

---

## Token: `FolderSelectList`

Renders a dropdown list of sub-folders from a specified directory.

---

### Signature

```csharp
@FolderSelectList(SimplisityInfo info, String xpath, String mappathRootFolder, String attributes = "", Boolean allowEmpty = true, bool localized = false)
```

---

### Parameters

| Parameter         | Type           | Description                                                              | Default / Optional |
| :---------------- | :------------- | :----------------------------------------------------------------------- | :----------------- |
| `info`            | SimplisityInfo | The data object containing the selected value.                           | **Required**       |
| `xpath`           | string         | The XPath expression to select and store the value in the `info` object. | **Required**       |
| `mappathRootFolder`| string         | The physical path to the directory containing the sub-folders.           | **Required**       |
| `attributes`      | string         | Additional HTML attributes to apply to the `<select>` element.           | Optional           |
| `allowEmpty`      | bool           | If `true`, includes an empty option in the list.                         | `true`             |
| `localized`       | bool           | If `true`, uses the language-specific section of the data.               | `false`            |

---

### Returns

*   An `IEncodedString` containing the HTML for the `<select>` element.

---

### Example

```razor
@FolderSelectList(Model.info, "genxml/data/category", Server.MapPath("/categories"))
```

---

<details>
<summary>Machine-Readable Metadata (JSON)</summary>

```json
{
  "name": "FolderSelectList",
  "type": "token",
  "summary": "Renders a dropdown list of sub-folders from a specified directory.",
  "returns": {
	"type": "IEncodedString",
	"description": "An IEncodedString containing the HTML for the <select> element."
  },
  "parameters": [
	{ "name": "info", "type": "SimplisityInfo", "description": "The data object containing the selected value.", "required": true },
	{ "name": "xpath", "type": "string", "description": "The XPath expression to select and store the value in the info object.", "required": true },
	{ "name": "mappathRootFolder", "type": "string", "description": "The physical path to the directory containing the sub-folders.", "required": true },
	{ "name": "attributes", "type": "string", "description": "Additional HTML attributes to apply to the <select> element.", "required": false, "default": "" },
	{ "name": "allowEmpty", "type": "bool", "description": "If true, includes an empty option in the list.", "required": false, "default": "true" },
	{ "name": "localized", "type": "bool", "description": "If true, uses the language-specific section of the data.", "required": false, "default": "false" }
  ]
}
```
</details>

---
## DNNrocketAPI.render.DNNrocketTokens

Extends `Simplisity.RazorEngineTokens` with DNN-specific functionality, including resource localization, DNN entity-based controls, and image handling.

---

## Token: `AddProcessDataResx`

Adds resource file paths to the processing data, allowing `ResourceKey` tokens to resolve localization strings.

---

### Signature

```csharp
@AddProcessDataResx(AppThemeLimpet appTheme, bool includeAPIresx = false)
```

---

### Parameters

| Parameter      | Type           | Description                                                              | Default / Optional |
| :------------- | :------------- | :----------------------------------------------------------------------- | :----------------- |
| `appTheme`     | AppThemeLimpet | The application theme object, which provides the paths to the resource files. | **Required**       |
| `includeAPIresx`| bool           | If `true`, includes the DNNrocket API's resource file path.              | `false`            |

---

### Returns

*   An `IEncodedString` containing an empty string.

---

### Example

```razor
@AddProcessDataResx(Model.AppTheme)
```

---

<details>
<summary>Machine-Readable Metadata (JSON)</summary>

```json
{
  "name": "AddProcessDataResx",
  "type": "token",
  "summary": "Adds resource file paths to the processing data, allowing ResourceKey tokens to resolve localization strings.",
  "returns": {
	"type": "IEncodedString",
	"description": "An IEncodedString containing an empty string."
  },
  "parameters": [
	{ "name": "appTheme", "type": "AppThemeLimpet", "description": "The application theme object, which provides the paths to the resource files.", "required": true },
	{ "name": "includeAPIresx", "type": "bool", "description": "If true, includes the DNNrocket API's resource file path.", "required": false, "default": "false" }
  ]
}
```
</details>

---

## Token: `DropDownLanguageList`

Renders a dropdown list of enabled languages for the portal, showing flags and language names.

---

### Signature

```csharp
@DropDownLanguageList(SimplisityInfo info, String xpath, String attributes = "", String defaultValue = "", bool localized = false, int row = 0, string listname = "")
```

---

### Parameters

| Parameter      | Type          | Description                                                              | Default / Optional |
| :------------- | :------------ | :----------------------------------------------------------------------- | :----------------- |
| `info`         | SimplisityInfo| The data object containing the selected value.                           | **Required**       |
| `xpath`        | string        | The XPath expression to select and store the value in the `info` object. | **Required**       |
| `attributes`   | string        | Additional HTML attributes to apply to the `<select>` element.           | Optional           |
| `defaultValue` | string        | The default selected value if no value is found.                         | Optional           |
| `localized`    | bool          | If `true`, uses the language-specific section of the data.               | `false`            |
| `row`          | int           | The row index, used for creating unique IDs in list-based scenarios.     | `0`                |
| `listname`     | string        | The name of the list, used for creating unique IDs in list-based scenarios.| `""`               |

---

### Returns

*   An `IEncodedString` containing the HTML for the language dropdown list.

---

### Example

```razor
@DropDownLanguageList(Model.info, "genxml/data/language", "class='w3-select'")
```

---

<details>
<summary>Machine-Readable Metadata (JSON)</summary>

```json
{
  "name": "DropDownLanguageList",
  "type": "token",
  "summary": "Renders a dropdown list of enabled languages for the portal, showing flags and language names.",
  "returns": {
	"type": "IEncodedString",
	"description": "An IEncodedString containing the HTML for the language dropdown list."
  },
  "parameters": [
	{ "name": "info", "type": "SimplisityInfo", "description": "The data object containing the selected value.", "required": true },
	{ "name": "xpath", "type": "string", "description": "The XPath expression to select and store the value in the info object.", "required": true },
	{ "name": "attributes", "type": "string", "description": "Additional HTML attributes to apply to the <select> element.", "required": false, "default": "" },
	{ "name": "defaultValue", "type": "string", "description": "The default selected value if no value is found.", "required": false, "default": "" },
	{ "name": "localized", "type": "bool", "description": "If true, uses the language-specific section of the data.", "required": false, "default": "false" },
	{ "name": "row", "type": "int", "description": "The row index, used for creating unique IDs in list-based scenarios.", "required": false, "default": "0" },
	{ "name": "listname", "type": "string", "description": "The name of the list, used for creating unique IDs in list-based scenarios.", "required": false, "default": "" }
  ]
}
```
</details>

---

## Token: `DropDownCurrencyList`

Renders a dropdown list of available currencies.

---

### Signature

```csharp
@DropDownCurrencyList(SimplisityInfo info, String xpath, String attributes = "", String defaultValue = "", bool localized = false, int row = 0, string listname = "")
```

---

### Parameters

| Parameter      | Type          | Description                                                              | Default / Optional |
| :------------- | :------------ | :----------------------------------------------------------------------- | :----------------- |
| `info`         | SimplisityInfo| The data object containing the selected value.                           | **Required**       |
| `xpath`        | string        | The XPath expression to select and store the value in the `info` object. | **Required**       |
| `attributes`   | string        | Additional HTML attributes to apply to the `<select>` element.           | Optional           |
| `defaultValue` | string        | The default selected value if no value is found.                         | Optional           |
| `localized`    | bool          | If `true`, uses the language-specific section of the data.               | `false`            |
| `row`          | int           | The row index, used for creating unique IDs in list-based scenarios.     | `0`                |
| `listname`     | string        | The name of the list, used for creating unique IDs in list-based scenarios.| `""`               |

---

### Returns

*   An `IEncodedString` containing the HTML for the currency dropdown list.

---

### Example

```razor
@DropDownCurrencyList(Model.info, "genxml/data/currency", "class='w3-select'")
```

---

<details>
<summary>Machine-Readable Metadata (JSON)</summary>

```json
{
  "name": "DropDownCurrencyList",
  "type": "token",
  "summary": "Renders a dropdown list of available currencies.",
  "returns": {
	"type": "IEncodedString",
	"description": "An IEncodedString containing the HTML for the currency dropdown list."
  },
  "parameters": [
	{ "name": "info", "type": "SimplisityInfo", "description": "The data object containing the selected value.", "required": true },
	{ "name": "xpath", "type": "string", "description": "The XPath expression to select and store the value in the info object.", "required": true },
	{ "name": "attributes", "type": "string", "description": "Additional HTML attributes to apply to the <select> element.", "required": false, "default": "" },
	{ "name": "defaultValue", "type": "string", "description": "The default selected value if no value is found.", "required": false, "default": "" },
	{ "name": "localized", "type": "bool", "description": "If true, uses the language-specific section of the data.", "required": false, "default": "false" },
	{ "name": "row", "type": "int", "description": "The row index, used for creating unique IDs in list-based scenarios.", "required": false, "default": "0" },
	{ "name": "listname", "type": "string", "description": "The name of the list, used for creating unique IDs in list-based scenarios.", "required": false, "default": "" }
  ]
}
```
</details>

---

## Token: `DropDownCultureCodeList`

Renders a dropdown list of culture codes available in the portal.

---

### Signature

```csharp
@DropDownCultureCodeList(SimplisityInfo info, String xpath, String attributes = "", String defaultValue = "", bool localized = false, int row = 0, string listname = "")
```

---

### Parameters

| Parameter      | Type          | Description                                                              | Default / Optional |
| :------------- | :------------ | :----------------------------------------------------------------------- | :----------------- |
| `info`         | SimplisityInfo| The data object containing the selected value.                           | **Required**       |
| `xpath`        | string        | The XPath expression to select and store the value in the `info` object. | **Required**       |
| `attributes`   | string        | Additional HTML attributes to apply to the `<select>` element.           | Optional           |
| `defaultValue` | string        | The default selected value if no value is found.                         | Optional           |
| `localized`    | bool          | If `true`, uses the language-specific section of the data.               | `false`            |
| `row`          | int           | The row index, used for creating unique IDs in list-based scenarios.     | `0`                |
| `listname`     | string        | The name of the list, used for creating unique IDs in list-based scenarios.| `""`               |

---

### Returns

*   An `IEncodedString` containing the HTML for the culture code dropdown list.

---

### Example

```razor
@DropDownCultureCodeList(Model.info, "genxml/data/culturecode", "class='w3-select'")
```

---

<details>
<summary>Machine-Readable Metadata (JSON)</summary>

```json
{
  "name": "DropDownCultureCodeList",
  "type": "token",
  "summary": "Renders a dropdown list of culture codes available in the portal.",
  "returns": {
	"type": "IEncodedString",
	"description": "An IEncodedString containing the HTML for the culture code dropdown list."
  },
  "parameters": [
	{ "name": "info", "type": "SimplisityInfo", "description": "The data object containing the selected value.", "required": true },
	{ "name": "xpath", "type": "string", "description": "The XPath expression to select and store the value in the info object.", "required": true },
	{ "name": "attributes", "type": "string", "description": "Additional HTML attributes to apply to the <select> element.", "required": false, "default": "" },
	{ "name": "defaultValue", "type": "string", "description": "The default selected value if no value is found.", "required": false, "default": "" },
	{ "name": "localized", "type": "bool", "description": "If true, uses the language-specific section of the data.", "required": false, "default": "false" },
	{ "name": "row", "type": "int", "description": "The row index, used for creating unique IDs in list-based scenarios.", "required": false, "default": "0" },
	{ "name": "listname", "type": "string", "description": "The name of the list, used for creating unique IDs in list-based scenarios.", "required": false, "default": "" }
  ]
}
```
</details>

---

## Token: `DropDownCountryCodeList`

Renders a dropdown list of country codes.

---

### Signature

```csharp
@DropDownCountryCodeList(SimplisityInfo info, String xpath, String attributes = "", String defaultValue = "", bool localized = false, int row = 0, string listname = "")
```

---

### Parameters

| Parameter      | Type          | Description                                                              | Default / Optional |
| :------------- | :------------ | :----------------------------------------------------------------------- | :----------------- |
| `info`         | SimplisityInfo| The data object containing the selected value.                           | **Required**       |
| `xpath`        | string        | The XPath expression to select and store the value in the `info` object. | **Required**       |
| `attributes`   | string        | Additional HTML attributes to apply to the `<select>` element.           | Optional           |
| `defaultValue` | string        | The default selected value if no value is found.                         | Optional           |
| `localized`    | bool          | If `true`, uses the language-specific section of the data.               | `false`            |
| `row`          | int           | The row index, used for creating unique IDs in list-based scenarios.     | `0`                |
| `listname`     | string        | The name of the list, used for creating unique IDs in list-based scenarios.| `""`               |

---

### Returns

*   An `IEncodedString` containing the HTML for the country code dropdown list.

---

### Example

```razor
@DropDownCountryCodeList(Model.info, "genxml/data/countrycode", "class='w3-select'")
```

---

<details>
<summary>Machine-Readable Metadata (JSON)</summary>

```json
{
  "name": "DropDownCountryCodeList",
  "type": "token",
  "summary": "Renders a dropdown list of country codes.",
  "returns": {
	"type": "IEncodedString",
	"description": "An IEncodedString containing the HTML for the country code dropdown list."
  },
  "parameters": [
	{ "name": "info", "type": "SimplisityInfo", "description": "The data object containing the selected value.", "required": true },
	{ "name": "xpath", "type": "string", "description": "The XPath expression to select and store the value in the info object.", "required": true },
	{ "name": "attributes", "type": "string", "description": "Additional HTML attributes to apply to the <select> element.", "required": false, "default": "" },
	{ "name": "defaultValue", "type": "string", "description": "The default selected value if no value is found.", "required": false, "default": "" },
	{ "name": "localized", "type": "bool", "description": "If true, uses the language-specific section of the data.", "required": false, "default": "false" },
	{ "name": "row", "type": "int", "description": "The row index, used for creating unique IDs in list-based scenarios.", "required": false, "default": "0" },
	{ "name": "listname", "type": "string", "description": "The name of the list, used for creating unique IDs in list-based scenarios.", "required": false, "default": "" }
  ]
}
```
</details>

---

## Token: `DropDownSystemKeyList`

Renders a dropdown list of active system keys.

---

### Signature

```csharp
@DropDownSystemKeyList(SimplisityInfo info, String xpath, String attributes = "", String defaultValue = "", bool localized = false, int row = 0, string listname = "")
```

---

### Parameters

| Parameter      | Type          | Description                                                              | Default / Optional |
| :------------- | :------------ | :----------------------------------------------------------------------- | :----------------- |
| `info`         | SimplisityInfo| The data object containing the selected value.                           | **Required**       |
| `xpath`        | string        | The XPath expression to select and store the value in the `info` object. | **Required**       |
| `attributes`   | string        | Additional HTML attributes to apply to the `<select>` element.           | Optional           |
| `defaultValue` | string        | The default selected value if no value is found.                         | Optional           |
| `localized`    | bool          | If `true`, uses the language-specific section of the data.               | `false`            |
| `row`          | int           | The row index, used for creating unique IDs in list-based scenarios.     | `0`                |
| `listname`     | string        | The name of the list, used for creating unique IDs in list-based scenarios.| `""`               |

---

### Returns

*   An `IEncodedString` containing the HTML for the system key dropdown list.

---

### Example

```razor
@DropDownSystemKeyList(Model.info, "genxml/data/systemkey", "class='w3-select'")
```

---

<details>
<summary>Machine-Readable Metadata (JSON)</summary>

```json
{
  "name": "DropDownSystemKeyList",
  "type": "token",
  "summary": "Renders a dropdown list of active system keys.",
  "returns": {
	"type": "IEncodedString",
	"description": "An IEncodedString containing the HTML for the system key dropdown list."
  },
  "parameters": [
	{ "name": "info", "type": "SimplisityInfo", "description": "The data object containing the selected value.", "required": true },
	{ "name": "xpath", "type": "string", "description": "The XPath expression to select and store the value in the info object.", "required": true },
	{ "name": "attributes", "type": "string", "description": "Additional HTML attributes to apply to the <select> element.", "required": false, "default": "" },
	{ "name": "defaultValue", "type": "string", "description": "The default selected value if no value is found.", "required": false, "default": "" },
	{ "name": "localized", "type": "bool", "description": "If true, uses the language-specific section of the data.", "required": false, "default": "false" },
	{ "name": "row", "type": "int", "description": "The row index, used for creating unique IDs in list-based scenarios.", "required": false, "default": "0" },
	{ "name": "listname", "type": "string", "description": "The name of the list, used for creating unique IDs in list-based scenarios.", "required": false, "default": "" }
  ]
}
```
</details>

---

## Token: `ResourceCSV`

Retrieves a comma-separated list of localized resource strings.

---

### Signature

```csharp
@ResourceCSV(String resourceFileKey, string keyListCSV, string lang = "", string resourceExtension = "Text")
```

---

### Parameters

| Parameter         | Type   | Description                                                              | Default / Optional |
| :---------------- | :----- | :----------------------------------------------------------------------- | :----------------- |
| `resourceFileKey` | string | The base name of the resource file.                                      | **Required**       |
| `keyListCSV`      | string | A comma-separated list of resource keys to retrieve.                     | **Required**       |
| `lang`            | string | The language code for localization. If empty, the current culture is used. | Optional           |
| `resourceExtension`| string | The extension of the resource file (e.g., "Text", "Icon").               | `"Text"`           |

---

### Returns

*   An `IEncodedString` containing the comma-separated localized strings.

---

### Example

```razor
@ResourceCSV("MyResources", "key1,key2,key3")
```

---

<details>
<summary>Machine-Readable Metadata (JSON)</summary>

```json
{
  "name": "ResourceCSV",
  "type": "token",
  "summary": "Retrieves a comma-separated list of localized resource strings.",
  "returns": {
	"type": "IEncodedString",
	"description": "An IEncodedString containing the comma-separated localized strings."
  },
  "parameters": [
	{ "name": "resourceFileKey", "type": "string", "description": "The base name of the resource file.", "required": true },
	{ "name": "keyListCSV", "type": "string", "description": "A comma-separated list of resource keys to retrieve.", "required": true },
	{ "name": "lang", "type": "string", "description": "The language code for localization. If empty, the current culture is used.", "required": false, "default": "" },
	{ "name": "resourceExtension", "type": "string", "description": "The extension of the resource file (e.g., \"Text\", \"Icon\").", "required": false, "default": "Text" }
  ]
}
```
</details>

---

## Token: `ButtonTextIcon`

Renders a button with text followed by an icon, using localized resources.

---

### Signature

```csharp
@ButtonTextIcon(ButtonTypes buttontype, String lang = "")
```

---

### Parameters

| Parameter   | Type        | Description                                                              | Default / Optional |
| :---------- | :---------- | :----------------------------------------------------------------------- | :----------------- |
| `buttontype`| ButtonTypes | The type of button to render (e.g., `ButtonTypes.Save`, `ButtonTypes.Delete`). | **Required**       |
| `lang`      | string      | The language code for localization. If empty, the current culture is used. | Optional           |

---

### Returns

*   An `IEncodedString` containing the HTML for the button content.

---

### Example

```razor
<button>@ButtonTextIcon(ButtonTypes.Save)</button>
```

---

<details>
<summary>Machine-Readable Metadata (JSON)</summary>

```json
{
  "name": "ButtonTextIcon",
  "type": "token",
  "summary": "Renders a button with text followed by an icon, using localized resources.",
  "returns": {
	"type": "IEncodedString",
	"description": "An IEncodedString containing the HTML for the button content."
  },
  "parameters": [
	{ "name": "buttontype", "type": "ButtonTypes", "description": "The type of button to render (e.g., ButtonTypes.Save, ButtonTypes.Delete).", "required": true },
	{ "name": "lang", "type": "string", "description": "The language code for localization. If empty, the current culture is used.", "required": false, "default": "" }
  ]
}
```
</details>

---

## Token: `ButtonIconText`

Renders a button with an icon followed by text, using localized resources. This is an alias for `ButtonText`.

---

### Signature

```csharp
@ButtonIconText(ButtonTypes buttontype, String lang = "")
```

---

### Parameters

| Parameter   | Type        | Description                                                              | Default / Optional |
| :---------- | :---------- | :----------------------------------------------------------------------- | :----------------- |
| `buttontype`| ButtonTypes | The type of button to render (e.g., `ButtonTypes.Save`, `ButtonTypes.Delete`). | **Required**       |
| `lang`      | string      | The language code for localization. If empty, the current culture is used. | Optional           |

---

### Returns

*   An `IEncodedString` containing the HTML for the button content.

---

### Example

```razor
<button>@ButtonIconText(ButtonTypes.Save)</button>
```

---

<details>
<summary>Machine-Readable Metadata (JSON)</summary>

```json
{
  "name": "ButtonIconText",
  "type": "token",
  "summary": "Renders a button with an icon followed by text, using localized resources. This is an alias for ButtonText.",
  "returns": {
	"type": "IEncodedString",
	"description": "An IEncodedString containing the HTML for the button content."
  },
  "parameters": [
	{ "name": "buttontype", "type": "ButtonTypes", "description": "The type of button to render (e.g., ButtonTypes.Save, ButtonTypes.Delete).", "required": true },
	{ "name": "lang", "type": "string", "description": "The language code for localization. If empty, the current culture is used.", "required": false, "default": "" }
  ]
}
```
</details>

---

## Token: `ButtonText`

Renders a button with an icon followed by text, using localized resources.

---

### Signature

```csharp
@ButtonText(ButtonTypes buttontype, String lang = "")
```

---

### Parameters

| Parameter   | Type        | Description                                                              | Default / Optional |
| :---------- | :---------- | :----------------------------------------------------------------------- | :----------------- |
| `buttontype`| ButtonTypes | The type of button to render (e.g., `ButtonTypes.Save`, `ButtonTypes.Delete`). | **Required**       |
| `lang`      | string      | The language code for localization. If empty, the current culture is used. | Optional           |

---

### Returns

*   An `IEncodedString` containing the HTML for the button content.

---

### Example

```razor
<button>@ButtonText(ButtonTypes.Cancel)</button>
```

---

<details>
<summary>Machine-Readable Metadata (JSON)</summary>

```json
{
  "name": "ButtonText",
  "type": "token",
  "summary": "Renders a button with an icon followed by text, using localized resources.",
  "returns": {
	"type": "IEncodedString",
	"description": "An IEncodedString containing the HTML for the button content."
  },
  "parameters": [
	{ "name": "buttontype", "type": "ButtonTypes", "description": "The type of button to render (e.g., ButtonTypes.Save, ButtonTypes.Delete).", "required": true },
	{ "name": "lang", "type": "string", "description": "The language code for localization. If empty, the current culture is used.", "required": false, "default": "" }
  ]
}
```
</details>

---

## Token: `ButtonIcon`

Renders a button with only an icon, using a localized tooltip for the text.

---

### Signature

```csharp
@ButtonIcon(ButtonTypes buttontype, String lang = "")
```

---

### Parameters

| Parameter   | Type        | Description                                                              | Default / Optional |
| :---------- | :---------- | :----------------------------------------------------------------------- | :----------------- |
| `buttontype`| ButtonTypes | The type of button to render (e.g., `ButtonTypes.Save`, `ButtonTypes.Delete`). | **Required**       |
| `lang`      | string      | The language code for localization. If empty, the current culture is used. | Optional           |

---

### Returns

*   An `IEncodedString` containing the HTML for the icon.

---

### Example

```razor
<button>@ButtonIcon(ButtonTypes.Delete)</button>
```

---

<details>
<summary>Machine-Readable Metadata (JSON)</summary>

```json
{
  "name": "ButtonIcon",
  "type": "token",
  "summary": "Renders a button with only an icon, using a localized tooltip for the text.",
  "returns": {
	"type": "IEncodedString",
	"description": "An IEncodedString containing the HTML for the icon."
  },
  "parameters": [
	{ "name": "buttontype", "type": "ButtonTypes", "description": "The type of button to render (e.g., ButtonTypes.Save, ButtonTypes.Delete).", "required": true },
	{ "name": "lang", "type": "string", "description": "The language code for localization. If empty, the current culture is used.", "required": false, "default": "" }
  ]
}
```
</details>

---

## Token: `ResourceKeyMod`

Retrieves a localized resource string for a specific module.

---

### Signature

```csharp
@ResourceKeyMod(String moduleRef, String resourceFileKey, String lang = "", String resourceExtension = "Text")
```

---

### Parameters

| Parameter         | Type   | Description                                                              | Default / Optional |
| :---------------- | :----- | :----------------------------------------------------------------------- | :----------------- |
| `moduleRef`       | string | The reference for the module (e.g., "RocketBlog").                       | **Required**       |
| `resourceFileKey` | string | The key of the resource string.                                          | **Required**       |
| `lang`            | string | The language code for localization. If empty, the current culture is used. | Optional           |
| `resourceExtension`| string | The extension of the resource file (e.g., "Text", "Icon").               | `"Text"`           |

---

### Returns

*   An `IEncodedString` containing the localized string.

---

### Example

```razor
@ResourceKeyMod("RocketBlog", "blog.title")
```

---

<details>
<summary>Machine-Readable Metadata (JSON)</summary>

```json
{
  "name": "ResourceKeyMod",
  "type": "token",
  "summary": "Retrieves a localized resource string for a specific module.",
  "returns": {
	"type": "IEncodedString",
	"description": "An IEncodedString containing the localized string."
  },
  "parameters": [
	{ "name": "moduleRef", "type": "string", "description": "The reference for the module (e.g., \"RocketBlog\").", "required": true },
	{ "name": "resourceFileKey", "type": "string", "description": "The key of the resource string.", "required": true },
	{ "name": "lang", "type": "string", "description": "The language code for localization. If empty, the current culture is used.", "required": false, "default": "" },
	{ "name": "resourceExtension", "type": "string", "description": "The extension of the resource file (e.g., \"Text\", \"Icon\").", "required": false, "default": "Text" }
  ]
}
```
</details>

---

## Token: `ResourceKey`

Retrieves a localized resource string.

---

### Signature

```csharp
@ResourceKey(String resourceFileKey, String lang = "", String resourceExtension = "Text")
```

---

### Parameters

| Parameter         | Type   | Description                                                              | Default / Optional |
| :---------------- | :----- | :----------------------------------------------------------------------- | :----------------- |
| `resourceFileKey` | string | The key of the resource string.                                          | **Required**       |
| `lang`            | string | The language code for localization. If empty, the current culture is used. | Optional           |
| `resourceExtension`| string | The extension of the resource file (e.g., "Text", "Icon").               | `"Text"`           |

---

### Returns

*   An `IEncodedString` containing the localized string.

---

### Example

```razor
@ResourceKey("DNNrocket.save")
```

---

<details>
<summary>Machine-Readable Metadata (JSON)</summary>

```json
{
  "name": "ResourceKey",
  "type": "token",
  "summary": "Retrieves a localized resource string.",
  "returns": {
	"type": "IEncodedString",
	"description": "An IEncodedString containing the localized string."
  },
  "parameters": [
	{ "name": "resourceFileKey", "type": "string", "description": "The key of the resource string.", "required": true },
	{ "name": "lang", "type": "string", "description": "The language code for localization. If empty, the current culture is used.", "required": false, "default": "" },
	{ "name": "resourceExtension", "type": "string", "description": "The extension of the resource file (e.g., \"Text\", \"Icon\").", "required": false, "default": "Text" }
  ]
}
```
</details>

---

## Token: `ResourceKeyJS`

Retrieves a localized resource string, escaped for use in JavaScript.

---

### Signature

```csharp
@ResourceKeyJS(String resourceFileKey, String lang = "", String resourceExtension = "Text")
```

---

### Parameters

| Parameter         | Type   | Description                                                              | Default / Optional |
| :---------------- | :----- | :----------------------------------------------------------------------- | :----------------- |
| `resourceFileKey` | string | The key of the resource string.                                          | **Required**       |
| `lang`            | string | The language code for localization. If empty, the current culture is used. | Optional           |
| `resourceExtension`| string | The extension of the resource file (e.g., "Text", "Icon").               | `"Text"`           |

---

### Returns

*   An `IEncodedString` containing the JavaScript-escaped localized string.

---

### Example

```javascript
var saveText = '@ResourceKeyJS("DNNrocket.save")';
```

---

<details>
<summary>Machine-Readable Metadata (JSON)</summary>

```json
{
  "name": "ResourceKeyJS",
  "type": "token",
  "summary": "Retrieves a localized resource string, escaped for use in JavaScript.",
  "returns": {
	"type": "IEncodedString",
	"description": "An IEncodedString containing the JavaScript-escaped localized string."
  },
  "parameters": [
	{ "name": "resourceFileKey", "type": "string", "description": "The key of the resource string.", "required": true },
	{ "name": "lang", "type": "string", "description": "The language code for localization. If empty, the current culture is used.", "required": false, "default": "" },
	{ "name": "resourceExtension", "type": "string", "description": "The extension of the resource file (e.g., \"Text\", \"Icon\").", "required": false, "default": "Text" }
  ]
}
```
</details>

---

## Token: `RenderTemplate`

Renders a Razor template with a given model.

---

### Signature

```csharp
@RenderTemplate(string razorTemplateName, AppThemeLimpet appTheme, SimplisityRazor model, bool cacheOff = false)
@RenderTemplate(string razorTemplate, SimplisityRazor model, bool debugMode = false)
```

---

### Parameters

| Parameter         | Type           | Description                                                              | Default / Optional |
| :---------------- | :------------- | :----------------------------------------------------------------------- | :----------------- |
| `razorTemplateName`| string         | The name of the template file to render.                                 | **Required** (Overload 1) |
| `appTheme`        | AppThemeLimpet | The application theme, used to locate the template.                      | **Required** (Overload 1) |
| `model`           | SimplisityRazor| The data model to pass to the template.                                  | **Required**       |
| `cacheOff`        | bool           | If `true`, disables caching for the template.                            | `false`            |
| `razorTemplate`   | string         | The full string content of the Razor template.                           | **Required** (Overload 2) |
| `debugMode`       | bool           | If `true`, enables debug mode for rendering.                             | `false`            |

---

### Returns

*   An `IEncodedString` containing the rendered HTML output.

---

### Example

```razor
@RenderTemplate("MyView.cshtml", Model.AppTheme, Model)

@{
	var templateString = "<h1>Hello, @Model.Get(\"username\")</h1>";
}
@RenderTemplate(templateString, Model)
```

---

<details>
<summary>Machine-Readable Metadata (JSON)</summary>

```json
{
  "name": "RenderTemplate",
  "type": "token",
  "summary": "Renders a Razor template with a given model.",
  "returns": {
	"type": "IEncodedString",
	"description": "An IEncodedString containing the rendered HTML output."
  },
  "parameters": [
	{ "name": "razorTemplateName", "type": "string", "description": "The name of the template file to render.", "required": true, "overload": "1" },
	{ "name": "appTheme", "type": "AppThemeLimpet", "description": "The application theme, used to locate the template.", "required": true, "overload": "1" },
	{ "name": "model", "type": "SimplisityRazor", "description": "The data model to pass to the template.", "required": true },
	{ "name": "cacheOff", "type": "bool", "description": "If true, disables caching for the template.", "required": false, "default": "false" },
	{ "name": "razorTemplate", "type": "string", "description": "The full string content of the Razor template.", "required": true, "overload": "2" },
	{ "name": "debugMode", "type": "bool", "description": "If true, enables debug mode for rendering.", "required": false, "default": "false" }
  ]
}
```
</details>

---

## Token: `RenderXml`

Renders the XML model of a `SimplisityInfo` object for debugging purposes.

---

### Signature

```csharp
@RenderXml(SimplisityInfo info, string xmlidx = "")
```

---

### Parameters

| Parameter | Type           | Description                                                     | Default / Optional |
| :-------- | :------------- | :-------------------------------------------------------------- | :----------------- |
| `info`    | SimplisityInfo | The data object whose XML model will be rendered.               | **Required**       |
| `xmlidx`  | string         | An identifier for the XML display, used for multiple instances. | Optional           |

---

### Returns

*   An `IEncodedString` containing the HTML for the XML display.

---

### Example

```razor
@RenderXml(Model.info)
```

---

<details>
<summary>Machine-Readable Metadata (JSON)</summary>

```json
{
  "name": "RenderXml",
  "type": "token",
  "summary": "Renders the XML model of a SimplisityInfo object for debugging purposes.",
  "returns": {
	"type": "IEncodedString",
	"description": "An IEncodedString containing the HTML for the XML display."
  },
  "parameters": [
	{ "name": "info", "type": "SimplisityInfo", "description": "The data object whose XML model will be rendered.", "required": true },
	{ "name": "xmlidx", "type": "string", "description": "An identifier for the XML display, used for multiple instances.", "required": false, "default": "" }
  ]
}
```
</details>

---

## Token: `RenderImageSelect`

Renders a button that opens the image selection dialog.

---

### Signature

```csharp
@RenderImageSelect(string systemKey, string imageFolderRel, bool singleselect = true, bool autoreturn = false)
```

---

### Parameters

| Parameter      | Type   | Description                                                              | Default / Optional |
| :------------- | :----- | :----------------------------------------------------------------------- | :----------------- |
| `systemKey`    | string | The system key for the module instance.                                  | **Required**       |
| `imageFolderRel`| string | The relative path to the folder where images are stored.                 | **Required**       |
| `singleselect` | bool   | If `true`, allows only a single image to be selected.                    | `true`             |
| `autoreturn`   | bool   | If `true`, automatically returns the selection without user confirmation. | `false`            |

---

### Returns

*   An `IEncodedString` containing the HTML for the image select button.

---

### Example

```razor
@RenderImageSelect("MySystem", "/Portals/0/Images")
```

---

<details>
<summary>Machine-Readable Metadata (JSON)</summary>

```json
{
  "name": "RenderImageSelect",
  "type": "token",
  "summary": "Renders a button that opens the image selection dialog.",
  "returns": {
	"type": "IEncodedString",
	"description": "An IEncodedString containing the HTML for the image select button."
  },
  "parameters": [
	{ "name": "systemKey", "type": "string", "description": "The system key for the module instance.", "required": true },
	{ "name": "imageFolderRel", "type": "string", "description": "The relative path to the folder where images are stored.", "required": true },
	{ "name": "singleselect", "type": "bool", "description": "If true, allows only a single image to be selected.", "required": false, "default": "true" },
	{ "name": "autoreturn", "type": "bool", "description": "If true, automatically returns the selection without user confirmation.", "required": false, "default": "false" }
  ]
}
```
</details>

---

## Token: `RenderDocumentSelect`

Renders a button that opens the document selection dialog.

---

### Signature

```csharp
@RenderDocumentSelect(string systemKey, string docFolderRel, bool singleselect = true, bool autoreturn = false)
```

---

### Parameters

| Parameter      | Type   | Description                                                              | Default / Optional |
| :------------- | :----- | :----------------------------------------------------------------------- | :----------------- |
| `systemKey`    | string | The system key for the module instance.                                  | **Required**       |
| `docFolderRel` | string | The relative path to the folder where documents are stored.              | **Required**       |
| `singleselect` | bool   | If `true`, allows only a single document to be selected.                 | `true`             |
| `autoreturn`   | bool   | If `true`, automatically returns the selection without user confirmation. | `false`            |

---

### Returns

*   An `IEncodedString` containing the HTML for the document select button.

---

### Example

```razor
@RenderDocumentSelect("MySystem", "/Portals/0/Documents")
```

---

<details>
<summary>Machine-Readable Metadata (JSON)</summary>

```json
{
  "name": "RenderDocumentSelect",
  "type": "token",
  "summary": "Renders a button that opens the document selection dialog.",
  "returns": {
	"type": "IEncodedString",
	"description": "An IEncodedString containing the HTML for the document select button."
  },
  "parameters": [
	{ "name": "systemKey", "type": "string", "description": "The system key for the module instance.", "required": true },
	{ "name": "docFolderRel", "type": "string", "description": "The relative path to the folder where documents are stored.", "required": true },
	{ "name": "singleselect", "type": "bool", "description": "If true, allows only a single document to be selected.", "required": false, "default": "true" },
	{ "name": "autoreturn", "type": "bool", "description": "If true, automatically returns the selection without user confirmation.", "required": false, "default": "false" }
  ]
}
```
</details>

---

## Token: `TranslationLock`

Renders a lock/unlock icon to manage the translation state of a field.

---

### Signature

```csharp
@TranslationLock(SimplisityInfo info, string xpath, bool active = true, int row = 0)
```

---

### Parameters

| Parameter | Type           | Description                                                              | Default / Optional |
| :-------- | :------------- | :----------------------------------------------------------------------- | :----------------- |
| `info`    | SimplisityInfo | The data object containing the translation lock state.                   | **Required**       |
| `xpath`   | string         | The XPath expression for the field. The lock state is stored at `xpath + "-lock"`. | **Required**       |
| `active`  | bool           | If `false`, the lock is disabled and only shows the current state.       | `true`             |
| `row`     | int            | The row index, used for creating unique IDs in list-based scenarios.     | `0`                |

---

### Returns

*   An `IEncodedString` containing the HTML for the lock/unlock icons and the hidden checkbox.

---

### Example

```razor
@TranslationLock(Model.info, "genxml/lang/textbox/title")
```

---

<details>
<summary>Machine-Readable Metadata (JSON)</summary>

```json
{
  "name": "TranslationLock",
  "type": "token",
  "summary": "Renders a lock/unlock icon to manage the translation state of a field.",
  "returns": {
	"type": "IEncodedString",
	"description": "An IEncodedString containing the HTML for the lock/unlock icons and the hidden checkbox."
  },
  "parameters": [
	{ "name": "info", "type": "SimplisityInfo", "description": "The data object containing the translation lock state.", "required": true },
	{ "name": "xpath", "type": "string", "description": "The XPath expression for the field. The lock state is stored at `xpath + \"-lock\"`.", "required": true },
	{ "name": "active", "type": "bool", "description": "If false, the lock is disabled and only shows the current state.", "required": false, "default": "true" },
	{ "name": "row", "type": "int", "description": "The row index, used for creating unique IDs in list-based scenarios.", "required": false, "default": "0" }
  ]
}
```
</details>

---

## Token: `Translate`

Renders an icon that triggers the translation process for a field.

---

### Signature

```csharp
@Translate(SimplisityInfo info, string xpath, bool active = true, int row = 0)
```

---

### Parameters

| Parameter | Type           | Description                                                              | Default / Optional |
| :-------- | :------------- | :----------------------------------------------------------------------- | :----------------- |
| `info`    | SimplisityInfo | The data object.                                                         | **Required**       |
| `xpath`   | string         | The XPath expression for the field to be translated.                     | **Required**       |
| `active`  | bool           | If `false`, the translate icon is not rendered.                          | `true`             |
| `row`     | int            | The row index, used for creating unique IDs in list-based scenarios.     | `0`                |

---

### Returns

*   An `IEncodedString` containing the HTML for the translate icon.

---

### Example

```razor
@Translate(Model.info, "genxml/lang/textbox/title")
```

---

<details>
<summary>Machine-Readable Metadata (JSON)</summary>

```json
{
  "name": "Translate",
  "type": "token",
  "summary": "Renders an icon that triggers the translation process for a field.",
  "returns": {
	"type": "IEncodedString",
	"description": "An IEncodedString containing the HTML for the translate icon."
  },
  "parameters": [
	{ "name": "info", "type": "SimplisityInfo", "description": "The data object.", "required": true },
	{ "name": "xpath", "type": "string", "description": "The XPath expression for the field to be translated.", "required": true },
	{ "name": "active", "type": "bool", "description": "If false, the translate icon is not rendered.", "required": false, "default": "true" },
	{ "name": "row", "type": "int", "description": "The row index, used for creating unique IDs in list-based scenarios.", "required": false, "default": "0" }
  ]
}
```
</details>

---

## Token: `TranslationKeyUp`

Adds a JavaScript `onkeyup` event handler to an input field to automatically lock the translation when the user types.

---

### Signature

```csharp
@TranslationKeyUp(string fieldId, bool active = true, int row = 0)
```

---

### Parameters

| Parameter | Type   | Description                                                              | Default / Optional |
| :-------- | :----- | :----------------------------------------------------------------------- | :----------------- |
| `fieldId` | string | The base ID of the input field.                                          | **Required**       |
| `active`  | bool   | If `false`, no event handler is rendered.                                | `true`             |
| `row`     | int    | The row index, used for creating unique IDs in list-based scenarios.     | `0`                |

---

### Returns

*   An `IEncodedString` containing the `onkeyup` attribute.

---

### Example

```razor
<input type="text" @TranslationKeyUp("title", Model.GetBool("editlanguage"), 1)>
```

---

<details>
<summary>Machine-Readable Metadata (JSON)</summary>

```json
{
  "name": "TranslationKeyUp",
  "type": "token",
  "summary": "Adds a JavaScript onkeyup event handler to an input field to automatically lock the translation when the user types.",
  "returns": {
	"type": "IEncodedString",
	"description": "An IEncodedString containing the onkeyup attribute."
  },
  "parameters": [
	{ "name": "fieldId", "type": "string", "description": "The base ID of the input field.", "required": true },
	{ "name": "active", "type": "bool", "description": "If false, no event handler is rendered.", "required": false, "default": "true" },
	{ "name": "row", "type": "int", "description": "The row index, used for creating unique IDs in list-based scenarios.", "required": false, "default": "0" }
  ]
}
```
</details>

---

## Token: `EditFlag`

Displays the flag icon for the current editing language.

---

### Signature

```csharp
@EditFlag(SessionParams sessionParams, string classvalues = "")
```

---

### Parameters

| Parameter     | Type          | Description                                  | Default / Optional |
| :------------ | :------------ | :------------------------------------------- | :----------------- |
| `sessionParams`| SessionParams | The session parameters object containing the edit culture code. | **Required**       |
| `classvalues` | string        | CSS classes to apply to the `<img>` element. | Optional           |

---

### Returns

*   An `IEncodedString` containing the HTML for the flag `<img>` tag.

---

### Example

```razor
<span>@EditFlag(Model.SessionParams, "w3-circle")</span>
```

---

<details>
<summary>Machine-Readable Metadata (JSON)</summary>

```json
{
  "name": "EditFlag",
  "type": "token",
  "summary": "Displays the flag icon for the current editing language.",
  "returns": {
	"type": "IEncodedString",
	"description": "An IEncodedString containing the HTML for the flag <img> tag."
  },
  "parameters": [
	{ "name": "sessionParams", "type": "SessionParams", "description": "The session parameters object containing the edit culture code.", "required": true },
	{ "name": "classvalues", "type": "string", "description": "CSS classes to apply to the <img> element.", "required": false, "default": "" }
  ]
}
```
</details>

---

## Token: `DisplayFlag`

Displays the flag icon for a specified culture code.

---

### Signature

```csharp
@DisplayFlag(string cultureCode, string classvalues = "")
```

---

### Parameters

| Parameter     | Type   | Description                                  | Default / Optional |
| :------------ | :----- | :------------------------------------------- | :----------------- |
| `cultureCode` | string | The culture code for the flag to display.    | **Required**       |
| `classvalues` | string | CSS classes to apply to the `<img>` element. | Optional           |

---

### Returns

*   An `IEncodedString` containing the HTML for the flag `<img>` tag, or an empty string if the flag image does not exist.

---

### Example

```razor
@DisplayFlag("fr-FR", "w3-margin-right")
```

---

<details>
<summary>Machine-Readable Metadata (JSON)</summary>

```json
{
  "name": "DisplayFlag",
  "type": "token",
  "summary": "Displays the flag icon for a specified culture code.",
  "returns": {
	"type": "IEncodedString",
	"description": "An IEncodedString containing the HTML for the flag <img> tag, or an empty string if the flag image does not exist."
  },
  "parameters": [
	{ "name": "cultureCode", "type": "string", "description": "The culture code for the flag to display.", "required": true },
	{ "name": "classvalues", "type": "string", "description": "CSS classes to apply to the <img> element.", "required": false, "default": "" }
  ]
}
```
</details>

---

## Token: `DisplayEngineFlag`

Displays the flag icon for a specified culture code, using a full engine URL.

---

### Signature

```csharp
@DisplayEngineFlag(string engineUrl, string cultureCode, string classvalues = "")
```

---

### Parameters

| Parameter     | Type   | Description                                  | Default / Optional |
| :------------ | :----- | :------------------------------------------- | :----------------- |
| `engineUrl`   | string | The base URL of the application engine.      | **Required**       |
| `cultureCode` | string | The culture code for the flag to display.    | **Required**       |
| `classvalues` | string | CSS classes to apply to the `<img>` element. | Optional           |

---

### Returns

*   An `IEncodedString` containing the HTML for the flag `<img>` tag with a full URL, or an empty string if the flag image does not exist.

---

### Example

```razor
@DisplayEngineFlag("https://my-site.com", "de-DE")
```

---

<details>
<summary>Machine-Readable Metadata (JSON)</summary>

```json
{
  "name": "DisplayEngineFlag",
  "type": "token",
  "summary": "Displays the flag icon for a specified culture code, using a full engine URL.",
  "returns": {
	"type": "IEncodedString",
	"description": "An IEncodedString containing the HTML for the flag <img> tag with a full URL, or an empty string if the flag image does not exist."
  },
  "parameters": [
	{ "name": "engineUrl", "type": "string", "description": "The base URL of the application engine.", "required": true },
	{ "name": "cultureCode", "type": "string", "description": "The culture code for the flag to display.", "required": true },
	{ "name": "classvalues", "type": "string", "description": "CSS classes to apply to the <img> element.", "required": false, "default": "" }
  ]
}
```
</details>

---

## Token: `ImageUrl`

Generates a URL for an image, with options for resizing and format conversion.

---

### Signature

```csharp
@ImageUrl(string url, int width = 0, int height = 0, string imgType = "", bool cropCenter = true)
@ImageUrl(string engineUrl, string imgRelPath, int width, int height, string imgType, bool cropCenter)
```

---

### Parameters

| Parameter    | Type   | Description                                                              | Default / Optional |
| :----------- | :----- | :----------------------------------------------------------------------- | :----------------- |
| `url`        | string | The relative path to the image.                                          | **Required** (Overload 1) |
| `width`      | int    | The desired width of the image.                                          | `0`                |
| `height`     | int    | The desired height of the image.                                         | `0`                |
| `imgType`    | string | The output image format (e.g., "png", "jpg", "webp"). Defaults to "webp" unless the source is a PNG. | Optional           |
| `cropCenter` | bool   | If `true`, crops the image from the center to fit the dimensions.        | `true`             |
| `engineUrl`  | string | The base URL of the application engine.                                  | **Required** (Overload 2) |
| `imgRelPath` | string | The relative path to the image.                                          | **Required** (Overload 2) |

---

### Returns

*   An `IEncodedString` containing the URL to the (potentially resized) image.

---

### Example

```razor
<img src="@ImageUrl("/images/my-photo.jpg", 100, 100)">
```

---

<details>
<summary>Machine-Readable Metadata (JSON)</summary>

```json
{
  "name": "ImageUrl",
  "type": "token",
  "summary": "Generates a URL for an image, with options for resizing and format conversion.",
  "returns": {
	"type": "IEncodedString",
	"description": "An IEncodedString containing the URL to the (potentially resized) image."
  },
  "parameters": [
	{ "name": "url", "type": "string", "description": "The relative path to the image.", "required": true, "overload": "1" },
	{ "name": "width", "type": "int", "description": "The desired width of the image.", "required": false, "default": "0" },
	{ "name": "height", "type": "int", "description": "The desired height of the image.", "required": false, "default": "0" },
	{ "name": "imgType", "type": "string", "description": "The output image format (e.g., \"png\", \"jpg\", \"webp\"). Defaults to \"webp\" unless the source is a PNG.", "required": false, "default": "" },
	{ "name": "cropCenter", "type": "bool", "description": "If true, crops the image from the center to fit the dimensions.", "required": false, "default": "true" },
	{ "name": "engineUrl", "type": "string", "description": "The base URL of the application engine.", "required": true, "overload": "2" },
	{ "name": "imgRelPath", "type": "string", "description": "The relative path to the image.", "required": true, "overload": "2" }
  ]
}
```
</details>

---

## Token: `InjectHiddenFieldData`

Renders hidden input fields for all data under the `genxml/hidden/*` path in a `SimplisityInfo` object.

---

### Signature

```csharp
@InjectHiddenFieldData(SimplisityInfo sInfo)
```

---

### Parameters

| Parameter | Type           | Description                                  | Default / Optional |
| :-------- | :------------- | :------------------------------------------- | :----------------- |
| `sInfo`   | SimplisityInfo | The data object containing the hidden fields. | **Required**       |

---

### Returns

*   An `IEncodedString` containing the HTML for all hidden input fields.

---

### Example

```razor
<form>
	@InjectHiddenFieldData(Model.info)
</form>
```

---

<details>
<summary>Machine-Readable Metadata (JSON)</summary>

```json
{
  "name": "InjectHiddenFieldData",
  "type": "token",
  "summary": "Renders hidden input fields for all data under the `genxml/hidden/*` path in a `SimplisityInfo` object.",
  "returns": {
	"type": "IEncodedString",
	"description": "An IEncodedString containing the HTML for all hidden input fields."
  },
  "parameters": [
	{ "name": "sInfo", "type": "SimplisityInfo", "description": "The data object containing the hidden fields.", "required": true }
  ]
}
```
</details>

---

## Token: `FileSelectList`

Renders a dropdown list of files from a specified directory.

---

### Signature

```csharp
// From selected filename
@FileSelectList(string selectedfilename, String mappathRootFolder, String attributes = "", Boolean allowEmpty = true)

// From SimplisityInfo
@FileSelectList(SimplisityInfo info, String xpath, String mappathRootFolder, String attributes = "", Boolean allowEmpty = true, bool localized = false)
```

---

### Parameters

| Parameter         | Type           | Description                                                              | Default / Optional |
| :---------------- | :------------- | :----------------------------------------------------------------------- | :----------------- |
| `selectedfilename`| string         | The filename to pre-select in the list.                                  | **Required** (Overload 1) |
| `mappathRootFolder`| string         | The physical path to the directory containing the files.                 | **Required**       |
| `attributes`      | string         | Additional HTML attributes to apply to the `<select>` element.           | Optional           |
| `allowEmpty`      | bool           | If `true`, includes an empty option in the list.                         | `true`             |
| `info`            | SimplisityInfo | The data object containing the selected value.                           | **Required** (Overload 2) |
| `xpath`           | string         | The XPath expression to select and store the value in the `info` object. | **Required** (Overload 2) |
| `localized`       | bool           | If `true`, uses the language-specific section of the data.               | `false`            |

---

### Returns

*   An `IEncodedString` containing the HTML for the `<select>` element.

---

### Example

```razor
@FileSelectList("default.jpg", Server.MapPath("/images"), "class='w3-select'")

@FileSelectList(Model.info, "genxml/data/image", Server.MapPath("/images"))
```

---

<details>
<summary>Machine-Readable Metadata (JSON)</summary>

```json
{
  "name": "FileSelectList",
  "type": "token",
  "summary": "Renders a dropdown list of files from a specified directory.",
  "returns": {
	"type": "IEncodedString",
	"description": "An IEncodedString containing the HTML for the <select> element."
  },
  "parameters": [
	{ "name": "selectedfilename", "type": "string", "description": "The filename to pre-select in the list.", "required": true, "overload": "1" },
	{ "name": "mappathRootFolder", "type": "string", "description": "The physical path to the directory containing the files.", "required": true },
	{ "name": "attributes", "type": "string", "description": "Additional HTML attributes to apply to the <select> element.", "required": false, "default": "" },
	{ "name": "allowEmpty", "type": "bool", "description": "If true, includes an empty option in the list.", "required": false, "default": "true" },
	{ "name": "info", "type": "SimplisityInfo", "description": "The data object containing the selected value.", "required": true, "overload": "2" },
	{ "name": "xpath", "type": "string", "description": "The XPath expression to select and store the value in the info object.", "required": true, "overload": "2" },
	{ "name": "localized", "type": "bool", "description": "If true, uses the language-specific section of the data.", "required": false, "default": "false" }
  ]
}
```
</details>

---

## Token: `FolderSelectList`

Renders a dropdown list of sub-folders from a specified directory.

---

### Signature

```csharp
@FolderSelectList(SimplisityInfo info, String xpath, String mappathRootFolder, String attributes = "", Boolean allowEmpty = true, bool localized = false)
```

---

### Parameters

| Parameter         | Type           | Description                                                              | Default / Optional |
| :---------------- | :------------- | :----------------------------------------------------------------------- | :----------------- |
| `info`            | SimplisityInfo | The data object containing the selected value.                           | **Required**       |
| `xpath`           | string         | The XPath expression to select and store the value in the `info` object. | **Required**       |
| `mappathRootFolder`| string         | The physical path to the directory containing the sub-folders.           | **Required**       |
| `attributes`      | string         | Additional HTML attributes to apply to the `<select>` element.           | Optional           |
| `allowEmpty`      | bool           | If `true`, includes an empty option in the list.                         | `true`             |
| `localized`       | bool           | If `true`, uses the language-specific section of the data.               | `false`            |

---

### Returns

*   An `IEncodedString` containing the HTML for the `<select>` element.

---

### Example

```razor
@FolderSelectList(Model.info, "genxml/data/category", Server.MapPath("/categories"))
```

---

<details>
<summary>Machine-Readable Metadata (JSON)</summary>

```json
{
  "name": "FolderSelectList",
  "type": "token",
  "summary": "Renders a dropdown list of sub-folders from a specified directory.",
  "returns": {
	"type": "IEncodedString",
	"description": "An IEncodedString containing the HTML for the <select> element."
  },
  "parameters": [
	{ "name": "info", "type": "SimplisityInfo", "description": "The data object containing the selected value.", "required": true },
	{ "name": "xpath", "type": "string", "description": "The XPath expression to select and store the value in the info object.", "required": true },
	{ "name": "mappathRootFolder", "type": "string", "description": "The physical path to the directory containing the sub-folders.", "required": true },
	{ "name": "attributes", "type": "string", "description": "Additional HTML attributes to apply to the <select> element.", "required": false, "default": "" },
	{ "name": "allowEmpty", "type": "bool", "description": "If true, includes an empty option in the list.", "required": false, "default": "true" },
	{ "name": "localized", "type": "bool", "description": "If true, uses the language-specific section of the data.", "required": false, "default": "false" }
  ]
}
```
</details>
