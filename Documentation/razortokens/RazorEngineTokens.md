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
	<summary>AddCssLinkHeader</summary>
	<div class="token-details">
		<p><strong>Description:</strong> Adds a CSS link to the page header. Generates a &lt;link&gt; tag to include a CSS file in the HTML header.</p>
		<strong>Signature</strong>
		<pre><code>public IEncodedString AddCssLinkHeader(string cssRelPath)</code></pre>
		<strong>Example</strong>
		<pre><code>@AddCssLinkHeader(cssRelPath)</code></pre>
	</div>
</details>
<details class="clean-accordion">
	<summary>AddJsScriptHeader</summary>
	<div class="token-details">
		<p><strong>Description:</strong> Generates a &lt;script&gt; tag to include a JavaScript file in the HTML header.</p>
		<strong>Signature</strong>
		<pre><code>public IEncodedString AddJsScriptHeader(string jsRelPath)</code></pre>
		<strong>Example</strong>
		<pre><code>@AddJsScriptHeader(jsRelPath)</code></pre>
	</div>
</details>
<details class="clean-accordion">
	<summary>AddPreProcessData</summary>
	<div class="token-details">
		<p><strong>Description:</strong> This method add the meta data to a specific cache list, so the we can use that data in the module code, before the razor template is rendered. This allows us to use the metadata token to add data selection information, like search filters and sort before we get the data from the DB. Adds a key-value pair to the pre-process data dictionary for later use. Adds metadata to a specific cache list before the Razor template is rendered. This allows module code to use this data (e.g., for database queries) before rendering. It requires a unique template name and module ID to create a specific cache key.</p>
		<strong>Signature</strong>
		<pre><code>public IEncodedString AddPreProcessData(String metaKey, String metaValue,String templateFullName,String moduleId)</code></pre>
		<strong>Example</strong>
		<pre><code>@AddPreProcessData(metaKey, metaValue, templateFullName, moduleId)</code></pre>
	</div>
</details>
<details class="clean-accordion">
	<summary>AddProcessData</summary>
	<div class="token-details">
		<p><strong>Description:</strong> Adds a key-value pair to the process data dictionary for later use. Adds metadata to the current rendering process. This data can be used by other tokens or within the same template. Returns an empty string.</p>
		<strong>Signature</strong>
		<pre><code>public IEncodedString AddProcessData(String metaType, String metaValue)</code></pre>
		<strong>Example</strong>
		<pre><code>@AddProcessData(metaType, metaValue)</code></pre>
		<strong>Signature</strong>
		<pre><code>public IEncodedString AddProcessData(String metaKey, String metaValue, String templateFullName)</code></pre>
		<strong>Example</strong>
		<pre><code>@AddProcessData(metaKey, metaValue, templateFullName)</code></pre>
	</div>
</details>
<details class="clean-accordion">
	<summary>BreakOf</summary>
	<div class="token-details">
		<p><strong>Description:</strong> Converts newline characters in a string to &lt;br/&gt; tags and HTML-encodes the content.</p>
		<strong>Signature</strong>
		<pre><code>public IEncodedString BreakOf(SimplisityInfo info, String xpath)</code></pre>
		<strong>Example</strong>
		<pre><code>@BreakOf(info, xpath)</code></pre>
		<strong>Signature</strong>
		<pre><code>public IEncodedString BreakOf(IEncodedString strIn)</code></pre>
		<strong>Example</strong>
		<pre><code>@BreakOf(strIn)</code></pre>
		<strong>Signature</strong>
		<pre><code>public IEncodedString BreakOf(String strIn)</code></pre>
		<strong>Example</strong>
		<pre><code>@BreakOf(strIn)</code></pre>
	</div>
</details>
<details class="clean-accordion">
	<summary>CheckBox</summary>
	<div class="token-details">
		<p><strong>Description:</strong> Renders a checkbox input field. Renders a single checkbox with a label, bound to a boolean value in a SimplisityInfo data model.</p>
		<strong>Signature</strong>
		<pre><code>public IEncodedString CheckBox(SimplisityInfo info, String xpath,String text, String attributes = &quot;&quot;, Boolean defaultValue = false, bool localized = false, int row = 0, string listname = &quot;&quot;)</code></pre>
		<strong>Example</strong>
		<pre><code>@CheckBox(info, xpath, text, attributes, defaultValue, localized, row, listname)</code></pre>
	</div>
</details>
<details class="clean-accordion">
	<summary>CheckBoxList</summary>
	<div class="token-details">
		<p><strong>Description:</strong> Renders a list of checkboxes from a dictionary or comma-separated strings. Each checkbox corresponds to a sub-node in the SimplisityInfo data model.</p>
		<strong>Signature</strong>
		<pre><code>public IEncodedString CheckBoxList(SimplisityInfo info, string xpath, Dictionary&lt;string, string&gt; dataDictionary, string attributes = &quot;&quot;, bool defaultValue = false, bool localized = false, int row = 0, string listname = &quot;&quot;)</code></pre>
		<strong>Example</strong>
		<pre><code>@CheckBoxList(info, xpath, dataDictionary, attributes, defaultValue, localized, row, listname)</code></pre>
		<strong>Signature</strong>
		<pre><code>public IEncodedString CheckBoxList(SimplisityInfo info, string xpath, string datavalue, string datatext, string attributes = &quot;&quot;, bool defaultValue = false, bool localized = false, int row = 0, string listname = &quot;&quot;)</code></pre>
		<strong>Example</strong>
		<pre><code>@CheckBoxList(info, xpath, datavalue, datatext, attributes, defaultValue, localized, row, listname)</code></pre>
	</div>
</details>
<details class="clean-accordion">
	<summary>CheckBoxListOf</summary>
	<div class="token-details">
		<p><strong>Description:</strong> Displays a formatted list (ul/li) of the selected items from a checkbox list.</p>
		<strong>Signature</strong>
		<pre><code>public IEncodedString CheckBoxListOf(SimplisityInfo info, String xpath, String datavalue, String datatext, String attributes = &quot;&quot;)</code></pre>
		<strong>Example</strong>
		<pre><code>@CheckBoxListOf(info, xpath, datavalue, datatext, attributes)</code></pre>
	</div>
</details>
<details class="clean-accordion">
	<summary>DateOf</summary>
	<div class="token-details">
		<p><strong>Description:</strong> Formats a date from the data model or a DateTime object into a string using a specific culture and format.</p>
		<strong>Signature</strong>
		<pre><code>public IEncodedString DateOf(SimplisityInfo info, String xpath, String cultureCode, String format = &quot;d&quot;)</code></pre>
		<strong>Example</strong>
		<pre><code>@DateOf(info, xpath, cultureCode, format)</code></pre>
		<strong>Signature</strong>
		<pre><code>public IEncodedString DateOf(SimplisityInfo info, String xpath, bool displayEmpty, String cultureCode, String format = &quot;d&quot;)</code></pre>
		<strong>Example</strong>
		<pre><code>@DateOf(info, xpath, displayEmpty, cultureCode, format)</code></pre>
		<strong>Signature</strong>
		<pre><code>public IEncodedString DateOf(DateTime dateTime, String cultureCode, String format = &quot;g&quot;)</code></pre>
		<strong>Example</strong>
		<pre><code>@DateOf(dateTime, cultureCode, format)</code></pre>
	</div>
</details>
<details class="clean-accordion">
	<summary>DropDownList</summary>
	<div class="token-details">
		<p><strong>Description:</strong> Renders a dropdown list from a key-value dictionary.</p>
		<strong>Signature</strong>
		<pre><code>public IEncodedString DropDownList(SimplisityInfo info, String xpath, Dictionary&lt;string,string&gt; dataDictionary, String attributes = &quot;&quot;, String defaultValue = &quot;&quot;, bool localized = false, int row = 0, string listname = &quot;&quot;)</code></pre>
		<strong>Example</strong>
		<pre><code>@DropDownList(info, xpath, dataDictionary, attributes, defaultValue, localized, row, listname)</code></pre>
		<strong>Signature</strong>
		<pre><code>public IEncodedString DropDownList(SimplisityInfo info, String xpath, String datavalue, String datatext, String attributes = &quot;&quot;, String defaultValue = &quot;&quot;, bool localized = false, int row = 0, string listname = &quot;&quot;)</code></pre>
		<strong>Example</strong>
		<pre><code>@DropDownList(info, xpath, datavalue, datatext, attributes, defaultValue, localized, row, listname)</code></pre>
	</div>
</details>
<details class="clean-accordion">
	<summary>EmailOf</summary>
	<div class="token-details">
		<p><strong>Description:</strong> Formats an email address from the data model as a &#39;mailto:&#39; link.</p>
		<strong>Signature</strong>
		<pre><code>public IEncodedString EmailOf(SimplisityInfo info, String xpath, string subject = &quot;&quot;, string visibleText = &quot;&quot;)</code></pre>
		<strong>Example</strong>
		<pre><code>@EmailOf(info, xpath, subject, visibleText)</code></pre>
	</div>
</details>
<details class="clean-accordion">
	<summary>FileSelectList</summary>
	<div class="token-details">
		<p><strong>Description:</strong> Renders a dropdown list of files from a specified directory.</p>
		<strong>Signature</strong>
		<pre><code>public IEncodedString FileSelectList(string selectedfilename, String mappathRootFolder, String attributes = &quot;&quot;, Boolean allowEmpty = true)</code></pre>
		<strong>Example</strong>
		<pre><code>@FileSelectList(selectedfilename, mappathRootFolder, attributes, allowEmpty)</code></pre>
		<strong>Signature</strong>
		<pre><code>public IEncodedString FileSelectList(SimplisityInfo info, String xpath, String mappathRootFolder, String attributes = &quot;&quot;, Boolean allowEmpty = true, bool localized = false)</code></pre>
		<strong>Example</strong>
		<pre><code>@FileSelectList(info, xpath, mappathRootFolder, attributes, allowEmpty, localized)</code></pre>
	</div>
</details>
<details class="clean-accordion">
	<summary>FolderSelectList</summary>
	<div class="token-details">
		<p><strong>Description:</strong> Renders a dropdown list of subdirectories from a specified directory. Renders a dropdown list of subdirectories from a specified directory.</p>
		<strong>Signature</strong>
		<pre><code>public IEncodedString FolderSelectList(SimplisityInfo info, String xpath, String mappathRootFolder, String attributes = &quot;&quot;, Boolean allowEmpty = true, bool localized = false)</code></pre>
		<strong>Example</strong>
		<pre><code>@FolderSelectList(info, xpath, mappathRootFolder, attributes, allowEmpty, localized)</code></pre>
	</div>
</details>
<details class="clean-accordion">
	<summary>getChecked</summary>
	<div class="token-details">
		<strong>Signature</strong>
		<pre><code>public String getChecked(SimplisityInfo info, String xpath, Boolean defaultValue)</code></pre>
		<strong>Example</strong>
		<pre><code>@getChecked(info, xpath, defaultValue)</code></pre>
	</div>
</details>
<details class="clean-accordion">
	<summary>getIdFromXpath</summary>
	<div class="token-details">
		<strong>Signature</strong>
		<pre><code>public String getIdFromXpath(String xpath, int row, string listname)</code></pre>
		<strong>Example</strong>
		<pre><code>@getIdFromXpath(xpath, row, listname)</code></pre>
	</div>
</details>
<details class="clean-accordion">
	<summary>getUpdateAttr</summary>
	<div class="token-details">
		<strong>Signature</strong>
		<pre><code>public String getUpdateAttr(String xpath, String attributes, bool localized)</code></pre>
		<strong>Example</strong>
		<pre><code>@getUpdateAttr(xpath, attributes, localized)</code></pre>
	</div>
</details>
<details class="clean-accordion">
	<summary>HiddenField</summary>
	<div class="token-details">
		<p><strong>Description:</strong> Renders a hidden input field. Renders a hidden input field bound to a SimplisityInfo data model.</p>
		<strong>Signature</strong>
		<pre><code>public IEncodedString HiddenField(SimplisityInfo info, String xpath, String attributes = &quot;&quot;, String defaultValue = &quot;&quot;, bool localized = false, int row = 0, string listname = &quot;&quot;)</code></pre>
		<strong>Example</strong>
		<pre><code>@HiddenField(info, xpath, attributes, defaultValue, localized, row, listname)</code></pre>
	</div>
</details>
<details class="clean-accordion">
	<summary>HtmlOf</summary>
	<div class="token-details">
		<p><strong>Description:</strong> HTML-decodes a string from the data model or a direct string, rendering it as raw HTML.</p>
		<strong>Signature</strong>
		<pre><code>public IEncodedString HtmlOf(SimplisityInfo info, String xpath)</code></pre>
		<strong>Example</strong>
		<pre><code>@HtmlOf(info, xpath)</code></pre>
		<strong>Signature</strong>
		<pre><code>public IEncodedString HtmlOf(String htmlString)</code></pre>
		<strong>Example</strong>
		<pre><code>@HtmlOf(htmlString)</code></pre>
	</div>
</details>
<details class="clean-accordion">
	<summary>RadioButtonList</summary>
	<div class="token-details">
		<p><strong>Description:</strong> Renders a list of radio buttons from a dictionary or comma-separated strings, bound to a single field in the SimplisityInfo data model.</p>
		<strong>Signature</strong>
		<pre><code>public IEncodedString RadioButtonList(SimplisityInfo info, string xpath, Dictionary&lt;string, string&gt; dataDictionary, string attributes = &quot;&quot;, string defaultValue = &quot;&quot;, string labelattributes = &quot;&quot;, bool localized = false, int row = 0, string listname = &quot;&quot;, string inputclass = &quot;&quot;)</code></pre>
		<strong>Example</strong>
		<pre><code>@RadioButtonList(info, xpath, dataDictionary, attributes, defaultValue, labelattributes, localized, row, listname, inputclass)</code></pre>
		<strong>Signature</strong>
		<pre><code>public IEncodedString RadioButtonList(SimplisityInfo info, string xpath, string datavalue, string datatext, string attributes = &quot;&quot;, string defaultValue = &quot;&quot;,string labelattributes = &quot;&quot;, bool localized = false, int row = 0, string listname = &quot;&quot;, string inputclass = &quot;&quot;)</code></pre>
		<strong>Example</strong>
		<pre><code>@RadioButtonList(info, xpath, datavalue, datatext, attributes, defaultValue, labelattributes, localized, row, listname, inputclass)</code></pre>
	</div>
</details>
<details class="clean-accordion">
	<summary>SecuritySiteKey</summary>
	<div class="token-details">
		<p><strong>Description:</strong> This token is used to place a siteKey onto the return template. This key can then be checked by the client module to confirm a valid template has been returned.</p>
		<strong>Signature</strong>
		<pre><code>public IEncodedString SecuritySiteKey(SessionParams sessionParams)</code></pre>
		<strong>Example</strong>
		<pre><code>@SecuritySiteKey(sessionParams)</code></pre>
	</div>
</details>
<details class="clean-accordion">
	<summary>SFields</summary>
	<div class="token-details">
		<p><strong>Description:</strong> Renders a hidden input containing a base64 encoded string of a dictionary, used for secure field submission. Generates a &#39;s-fields&#39; HTML attribute containing a JSON object from a series of key-value pairs. This is used for client-side scripting with Simplisity.</p>
		<strong>Signature</strong>
		<pre><code>public IEncodedString SFields(params string[] sFields)</code></pre>
		<strong>Example</strong>
		<pre><code>@SFields(sFields)</code></pre>
	</div>
</details>
<details class="clean-accordion">
	<summary>SortableListIndex</summary>
	<div class="token-details">
		<p><strong>Description:</strong> outputs the index fields required for a list, so we can process a sort order correctly. Outputs hidden fields required for a sortable list to correctly process the sort order. This includes a unique item reference and the current row index.</p>
		<strong>Signature</strong>
		<pre><code>public IEncodedString SortableListIndex(SimplisityInfo info, int row)</code></pre>
		<strong>Example</strong>
		<pre><code>@SortableListIndex(info, row)</code></pre>
	</div>
</details>
<details class="clean-accordion">
	<summary>Succinct</summary>
	<div class="token-details">
		<p><strong>Description:</strong> Succinct shortens your text to a specified size, and then dots to the end. Shortens a string to a specified length and appends &#39;...&#39; if it was truncated.</p>
		<strong>Signature</strong>
		<pre><code>public IEncodedString Succinct(string value, int size, bool showdots = true)</code></pre>
		<strong>Example</strong>
		<pre><code>@Succinct(value, size, showdots)</code></pre>
	</div>
</details>
<details class="clean-accordion">
	<summary>TextArea</summary>
	<div class="token-details">
		<p><strong>Description:</strong> Renders a textarea field. Renders a textarea field bound to a SimplisityInfo data model.</p>
		<strong>Signature</strong>
		<pre><code>public IEncodedString TextArea(SimplisityInfo info, String xpath, String attributes = &quot;&quot;, String defaultValue = &quot;&quot;, bool localized = false, int row = 0, string listname = &quot;&quot;)</code></pre>
		<strong>Example</strong>
		<pre><code>@TextArea(info, xpath, attributes, defaultValue, localized, row, listname)</code></pre>
	</div>
</details>
<details class="clean-accordion">
	<summary>TextBox</summary>
	<div class="token-details">
		<p><strong>Description:</strong> Renders a text input field. Renders a text input field bound to a SimplisityInfo data model.</p>
		<strong>Signature</strong>
		<pre><code>public IEncodedString TextBox(SimplisityInfo info, String xpath, String attributes = &quot;&quot;, String defaultValue = &quot;&quot;, bool localized = false, int row = 0, string listname = &quot;&quot;, string type = &quot;text&quot;)</code></pre>
		<strong>Example</strong>
		<pre><code>@TextBox(info, xpath, attributes, defaultValue, localized, row, listname, type)</code></pre>
	</div>
</details>
<details class="clean-accordion">
	<summary>TextBoxDate</summary>
	<div class="token-details">
		<p><strong>Description:</strong> Renders a date input field (type=&#39;date&#39;) bound to a SimplisityInfo data model. The value is formatted as &#39;yyyy-MM-dd&#39;.</p>
		<strong>Signature</strong>
		<pre><code>public IEncodedString TextBoxDate(SimplisityInfo info, String xpath, String attributes = &quot;&quot;, String defaultValue = &quot;&quot;, bool localized = false, int row = 0, string listname = &quot;&quot;)</code></pre>
		<strong>Example</strong>
		<pre><code>@TextBoxDate(info, xpath, attributes, defaultValue, localized, row, listname)</code></pre>
	</div>
</details>
