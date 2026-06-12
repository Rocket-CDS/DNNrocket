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
	<summary>AssignDataModel</summary>
	<div class="token-details">
		<p><strong>Description:</strong> Assigns the data model for Razor, making the template easier to build by populating various data properties like articleData, appTheme, moduleData, etc., from the SimplisityRazor model.</p>
		<strong>Signature</strong>
		<pre><code>public string AssignDataModel(SimplisityRazor sModel)</code></pre>
		<strong>Example</strong>
		<pre><code>@{ AssignDataModel(Model); }</code></pre>
	</div>
</details>

<details class="clean-accordion">
	<summary>RowKey</summary>
	<div class="token-details">
		<p><strong>Description:</strong> Generates the necessary hidden fields for a row's unique key ('rowkey' and 'rowkeylang') and a unique entity ID ('eid'). A row MUST have a rowkey to be saved to the database.</p>
		<strong>Signature</strong>
		<pre><code>public IEncodedString RowKey(SimplisityInfo info)</code></pre>
		<strong>Example</strong>
		<pre><code>@RowKey(Model.Info)</code></pre>
	</div>
</details>

<details class="clean-accordion">
	<summary>CheckBoxRowIsHidden</summary>
	<div class="token-details">
		<p><strong>Description:</strong> Creates a checkbox for the 'IsHidden' property of a row, allowing a row to be marked as hidden.</p>
		<strong>Signature</strong>
		<pre><code>public IEncodedString CheckBoxRowIsHidden(SimplisityInfo rowData)</code></pre>
		<strong>Example</strong>
		<pre><code>@CheckBoxRowIsHidden(Model.Info)</code></pre>
	</div>
</details>

<details class="clean-accordion">
	<summary>TextBoxRowTitle</summary>
	<div class="token-details">
		<p><strong>Description:</strong> Creates a standard textbox for a row's title using the XPath 'genxml/lang/genxml/textbox/title'.</p>
		<strong>Signature</strong>
		<pre><code>public IEncodedString TextBoxRowTitle(SimplisityInfo rowData)</code></pre>
		<strong>Example</strong>
		<pre><code>@TextBoxRowTitle(Model.Info)</code></pre>
	</div>
</details>

<details class="clean-accordion">
	<summary>ChatGPT</summary>
	<div class="token-details">
		<p><strong>Description:</strong> Renders a button to open a ChatGPT modal for generating text. Requires a ChatGPT API key in the global settings. The generated text will populate the field specified by 'textId'.</p>
		<strong>Signature</strong>
		<pre><code>public IEncodedString ChatGPT(string textId, string sourceTextId = "")</code></pre>
		<strong>Example</strong>
		<pre><code>@ChatGPT("mytextarea", "mysourcetextbox")</code></pre>
	</div>
</details>

<details class="clean-accordion">
	<summary>DeepL</summary>
	<div class="token-details">
		<p><strong>Description:</strong> Renders a button to open a DeepL translation modal. Requires a DeepL API key in the global settings and more than one portal language to be enabled. The translated text will populate the field specified by 'textId'.</p>
		<strong>Signature</strong>
		<pre><code>public IEncodedString DeepL(string textId, string sourceTextId = "", string cultureCode = "")</code></pre>
		<strong>Example</strong>
		<pre><code>@DeepL("translatedtext", "originaltext")</code></pre>
	</div>
</details>

<details class="clean-accordion">
	<summary>StylePadding</summary>
	<div class="token-details">
		<p><strong>Description:</strong> Generates an inline CSS padding style string based on module settings. It reads 'leftpadding', 'rightpadding', 'toppadding', and 'bottompadding' settings and creates corresponding CSS properties.</p>
		<strong>Signature</strong>
		<pre><code>public string StylePadding()</code></pre>
		<strong>Example</strong>
		<pre><code>&lt;div style="@StylePadding()"&gt;...&lt;/div&gt;</code></pre>
	</div>
</details>
