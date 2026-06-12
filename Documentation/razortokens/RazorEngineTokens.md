<style>
	details.clean-accordion {
		background-color: #fff; /* Changed to white */
		border: 1px solid #ddd;
		border-radius: 5px;
		margin-bottom: 0.5em;
		overflow: hidden;
	}
	details.clean-accordion summary {
		font-weight: 600;
		padding: 0.6em 1em; /* Reduced padding */
		cursor: pointer;
		background-color: #f5f5f5;
		border-bottom: 1px solid #ddd;
		transition: background-color 0.2s;
		list-style: none;
		display: block;
	}
	details.clean-accordion summary::-webkit-details-marker {
		display: none;
	}
	details.clean-accordion[open] > summary {
		background-color: #e9e9e9;
	}
	details.clean-accordion summary:hover {
		background-color: #e1e1e1;
	}
	details.clean-accordion .token-details {
		padding: 0.8em 1em 1em 2em; /* Reduced padding, kept indent */
		font-size: 0.9em;
	}
	details.clean-accordion .token-details p {
		margin-top: 0;
	}
	details.clean-accordion .token-details pre {
		white-space: pre-wrap;
	}
</style>

<details class="clean-accordion">
	<summary>AddProcessData</summary>
	<div class="token-details">
		<p><strong>Description:</strong> Adds metadata to the current rendering process. This data can be used by other tokens or within the same template. Returns an empty string.</p>
		<strong>Signature</strong>
		<pre><code>public IEncodedString AddProcessData(String metaType, String metaValue)</code></pre>
		<strong>Example</strong>
		<pre><code>@AddProcessData("mykey", "myvalue")</code></pre>
	</div>
</details>
<details class="clean-accordion">
	<summary>AddPreProcessData</summary>
	<div class="token-details">
		<p><strong>Description:</strong> Adds metadata to a specific cache list before the Razor template is rendered. This allows module code to use this data (e.g., for database queries) before rendering. It requires a unique template name and module ID to create a specific cache key.</p>
		<strong>Signature</strong>
		<pre><code>public IEncodedString AddPreProcessData(String metaKey, String metaValue, String templateFullName, String moduleId)</code></pre>
		<strong>Example</strong>
		<pre><code>@AddPreProcessData("sortfield", "name", "MyTheme.list.cshtml", "123")</code></pre>
	</div>
</details>
<details class="clean-accordion">
	<summary>AddCssLinkHeader</summary>
	<div class="token-details">
		<p><strong>Description:</strong> Generates a &lt;link&gt; tag to include a CSS file in the HTML header.</p>
		<strong>Signature</strong>
		<pre><code>public IEncodedString AddCssLinkHeader(string cssRelPath)</code></pre>
		<strong>Example</strong>
		<pre><code>@AddCssLinkHeader("/DesktopModules/MyModule/style.css")</code></pre>
	</div>
</details>
<details class="clean-accordion">
	<summary>AddJsScriptHeader</summary>
	<div class="token-details">
		<p><strong>Description:</strong> Generates a &lt;script&gt; tag to include a JavaScript file in the HTML header.</p>
		<strong>Signature</strong>
		<pre><code>public IEncodedString AddJsScriptHeader(string jsRelPath)</code></pre>
		<strong>Example</strong>
		<pre><code>@AddJsScriptHeader("/DesktopModules/MyModule/script.js")</code></pre>
	</div>
</details>
<details class="clean-accordion">
	<summary>HiddenField</summary>
	<div class="token-details">
		<p><strong>Description:</strong> Renders a hidden input field bound to a SimplisityInfo data model.</p>
		<strong>Signature</strong>
		<pre><code>public IEncodedString HiddenField(SimplisityInfo info, String xpath, String attributes = "", String defaultValue = "", bool localized = false, int row = 0, string listname = "")</code></pre>
		<strong>Example</strong>
		<pre><code>@HiddenField(Model.Info, "genxml/hidden/mydata")</code></pre>
	</div>
</details>
<details class="clean-accordion">
	<summary>TextBox</summary>
	<div class="token-details">
		<p><strong>Description:</strong> Renders a text input field bound to a SimplisityInfo data model.</p>
		<strong>Signature</strong>
		<pre><code>public IEncodedString TextBox(SimplisityInfo info, String xpath, String attributes = "", String defaultValue = "", bool localized = false, int row = 0, string listname = "", string type = "text")</code></pre>
		<strong>Example</strong>
		<pre><code>@TextBox(Model.Info, "genxml/textbox/firstname")</code></pre>
	</div>
</details>
<details class="clean-accordion">
	<summary>TextBoxDate</summary>
	<div class="token-details">
		<p><strong>Description:</strong> Renders a date input field (type='date') bound to a SimplisityInfo data model. The value is formatted as 'yyyy-MM-dd'.</p>
		<strong>Signature</strong>
		<pre><code>public IEncodedString TextBoxDate(SimplisityInfo info, String xpath, String attributes = "", String defaultValue = "", bool localized = false, int row = 0, string listname = "")</code></pre>
		<strong>Example</strong>
		<pre><code>@TextBoxDate(Model.Info, "genxml/date/startdate")</code></pre>
	</div>
</details>
<details class="clean-accordion">
	<summary>TextArea</summary>
	<div class="token-details">
		<p><strong>Description:</strong> Renders a textarea field bound to a SimplisityInfo data model.</p>
		<strong>Signature</strong>
		<pre><code>public IEncodedString TextArea(SimplisityInfo info, String xpath, String attributes = "", String defaultValue = "", bool localized = false, int row = 0, string listname = "")</code></pre>
		<strong>Example</strong>
		<pre><code>@TextArea(Model.Info, "genxml/textbox/description")</code></pre>
	</div>
</details>
<details class="clean-accordion">
	<summary>CheckBox</summary>
	<div class="token-details">
		<p><strong>Description:</strong> Renders a single checkbox with a label, bound to a boolean value in a SimplisityInfo data model.</p>
		<strong>Signature</strong>
		<pre><code>public IEncodedString CheckBox(SimplisityInfo info, String xpath, String text, String attributes = "", Boolean defaultValue = false, bool localized = false, int row = 0, string listname = "")</code></pre>
		<strong>Example</strong>
		<pre><code>@CheckBox(Model.Info, "genxml/checkbox/isactive", "Is Active")</code></pre>
	</div>
</details>
<details class="clean-accordion">
	<summary>CheckBoxList</summary>
	<div class="token-details">
		<p><strong>Description:</strong> Renders a list of checkboxes from a dictionary or comma-separated strings. Each checkbox corresponds to a sub-node in the SimplisityInfo data model.</p>
		<strong>Signature</strong>
		<pre><code>public IEncodedString CheckBoxList(SimplisityInfo info, string xpath, Dictionary&lt;string, string&gt; dataDictionary, string attributes = "", bool defaultValue = false, bool localized = false, int row = 0, string listname = "")</code></pre>
		<strong>Example</strong>
		<pre><code>@CheckBoxList(Model.Info, "genxml/checkboxlist/options", myDictionary)</code></pre>
	</div>
</details>
<details class="clean-accordion">
	<summary>RadioButtonList</summary>
	<div class="token-details">
		<p><strong>Description:</strong> Renders a list of radio buttons from a dictionary or comma-separated strings, bound to a single field in the SimplisityInfo data model.</p>
		<strong>Signature</strong>
		<pre><code>public IEncodedString RadioButtonList(SimplisityInfo info, string xpath, Dictionary&lt;string, string&gt; dataDictionary, string attributes = "", string defaultValue = "", string labelattributes = "", bool localized = false, int row = 0, string listname = "", string inputclass = "")</code></pre>
		<strong>Example</strong>
		<pre><code>@RadioButtonList(Model.Info, "genxml/radio/selection", myDictionary)</code></pre>
	</div>
</details>
<details class="clean-accordion">
	<summary>DropDownList</summary>
	<div class="token-details">
		<p><strong>Description:</strong> Renders a dropdown list (select) from a dictionary or comma-separated strings, bound to a single field in the SimplisityInfo data model.</p>
		<strong>Signature</strong>
		<pre><code>public IEncodedString DropDownList(SimplisityInfo info, String xpath, Dictionary&lt;string, string&gt; dataDictionary, String attributes = "", String defaultValue = "", bool localized = false, int row = 0, string listname = "")</code></pre>
		<strong>Example</strong>
		<pre><code>@DropDownList(Model.Info, "genxml/dropdown/category", myDictionary)</code></pre>
	</div>
</details>
<details class="clean-accordion">
	<summary>SortableListIndex</summary>
	<div class="token-details">
		<p><strong>Description:</strong> Outputs hidden fields required for a sortable list to correctly process the sort order. This includes a unique item reference and the current row index.</p>
		<strong>Signature</strong>
		<pre><code>public IEncodedString SortableListIndex(SimplisityInfo info, int row)</code></pre>
		<strong>Example</strong>
		<pre><code>@SortableListIndex(Model.Info, i)</code></pre>
	</div>
</details>
<details class="clean-accordion">
	<summary>EmailOf</summary>
	<div class="token-details">
		<p><strong>Description:</strong> Formats an email address from the data model as a 'mailto:' link.</p>
		<strong>Signature</strong>
		<pre><code>public IEncodedString EmailOf(SimplisityInfo info, String xpath, string subject = "", string visibleText = "")</code></pre>
		<strong>Example</strong>
		<pre><code>@EmailOf(Model.Info, "genxml/textbox/email", "Inquiry")</code></pre>
	</div>
</details>
<details class="clean-accordion">
	<summary>HtmlOf</summary>
	<div class="token-details">
		<p><strong>Description:</strong> HTML-decodes a string from the data model or a direct string, rendering it as raw HTML.</p>
		<strong>Signature</strong>
		<pre><code>public IEncodedString HtmlOf(SimplisityInfo info, String xpath)</code></pre>
		<strong>Example</strong>
		<pre><code>@HtmlOf(Model.Info, "genxml/richtext/content")</code></pre>
	</div>
</details>
<details class="clean-accordion">
	<summary>DateOf</summary>
	<div class="token-details">
		<p><strong>Description:</strong> Formats a date from the data model or a DateTime object into a string using a specific culture and format.</p>
		<strong>Signature</strong>
		<pre><code>public IEncodedString DateOf(SimplisityInfo info, String xpath, String cultureCode, String format = "d")</code></pre>
		<strong>Example</strong>
		<pre><code>@DateOf(Model.Info, "genxml/date/publishdate", "en-GB", "D")</code></pre>
	</div>
</details>
<details class="clean-accordion">
	<summary>Succinct</summary>
	<div class="token-details">
		<p><strong>Description:</strong> Shortens a string to a specified length and appends '...' if it was truncated.</p>
		<strong>Signature</strong>
		<pre><code>public IEncodedString Succinct(string value, int size, bool showdots = true)</code></pre>
		<strong>Example</strong>
		<pre><code>@Succinct(Model.Get("summary"), 100)</code></pre>
	</div>
</details>
<details class="clean-accordion">
	<summary>BreakOf</summary>
	<div class="token-details">
		<p><strong>Description:</strong> Converts newline characters in a string to &lt;br/&gt; tags and HTML-encodes the content.</p>
		<strong>Signature</strong>
		<pre><code>public IEncodedString BreakOf(SimplisityInfo info, String xpath)</code></pre>
		<strong>Example</strong>
		<pre><code>@BreakOf(Model.Info, "genxml/textbox/multilinetext")</code></pre>
	</div>
</details>
<details class="clean-accordion">
	<summary>CheckBoxListOf</summary>
	<div class="token-details">
		<p><strong>Description:</strong> Displays a formatted list (ul/li) of the selected items from a checkbox list.</p>
		<strong>Signature</strong>
		<pre><code>public IEncodedString CheckBoxListOf(SimplisityInfo info, String xpath, String datavalue, String datatext, String attributes = "")</code></pre>
		<strong>Example</strong>
		<pre><code>@CheckBoxListOf(Model.Info, "genxml/checkboxlist/options", "1,2,3", "Option 1,Option 2,Option 3")</code></pre>
	</div>
</details>
<details class="clean-accordion">
	<summary>FileSelectList</summary>
	<div class="token-details">
		<p><strong>Description:</strong> Renders a dropdown list of files from a specified directory.</p>
		<strong>Signature</strong>
		<pre><code>public IEncodedString FileSelectList(SimplisityInfo info, String xpath, String mappathRootFolder, String attributes = "", Boolean allowEmpty = true, bool localized = false)</code></pre>
		<strong>Example</strong>
		<pre><code>@FileSelectList(Model.Info, "genxml/dropdown/templatefile", Server.MapPath("~/MyTemplates"))</code></pre>
	</div>
</details>
<details class="clean-accordion">
	<summary>FolderSelectList</summary>
	<div class="token-details">
		<p><strong>Description:</strong> Renders a dropdown list of subdirectories from a specified directory.</p>
		<strong>Signature</strong>
		<pre><code>public IEncodedString FolderSelectList(SimplisityInfo info, String xpath, String mappathRootFolder, String attributes = "", Boolean allowEmpty = true, bool localized = false)</code></pre>
		<strong>Example</strong>
		<pre><code>@FolderSelectList(Model.Info, "genxml/dropdown/themefolder", Server.MapPath("~/MyThemes"))</code></pre>
	</div>
</details>
<details class="clean-accordion">
	<summary>SFields</summary>
	<div class="token-details">
		<p><strong>Description:</strong> Generates a 's-fields' HTML attribute containing a JSON object from a series of key-value pairs. This is used for client-side scripting with Simplisity.</p>
		<strong>Signature</strong>
		<pre><code>public IEncodedString SFields(params string[] sFields)</code></pre>
		<strong>Example</strong>
		<pre><code>@SFields("key1", "value1", "key2", "value2")</code></pre>
	</div>
</details>
<details class="clean-accordion">
	<summary>SecuritySiteKey</summary>
	<div class="token-details">
		<p><strong>Description:</strong> Renders a hidden element containing the current session's site key. This can be used for security validation in client-side calls.</p>
		<strong>Signature</strong>
		<pre><code>public IEncodedString SecuritySiteKey(SessionParams sessionParams)</code></pre>
		<strong>Example</strong>
		<pre><code>@SecuritySiteKey(Model.SessionParams)</code></pre>
	</div>
</details>
