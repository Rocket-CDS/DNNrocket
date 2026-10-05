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
		<p><strong>Description:</strong> Assigns the data model for razor, this makes the template easier to build. Assigns the data model for Razor, making the template easier to build by populating various data properties like articleData, appTheme, moduleData, etc., from the SimplisityRazor model.</p>
		<strong>Signature</strong>
		<pre><code>public string AssignDataModel(SimplisityRazor sModel)</code></pre>
		<strong>Example</strong>
		<pre><code>@AssignDataModel(sModel)</code></pre>
	</div>
</details>
<details class="clean-accordion">
	<summary>ChatGPT</summary>
	<div class="token-details">
		<p><strong>Description:</strong> Renders a button to open a ChatGPT modal for generating text. Requires a ChatGPT API key in the global settings. The generated text will populate the field specified by &#39;textId&#39;.</p>
		<strong>Signature</strong>
		<pre><code>public IEncodedString ChatGPT(string textId, string sourceTextId = &quot;&quot;)</code></pre>
		<strong>Example</strong>
		<pre><code>@ChatGPT(textId, sourceTextId)</code></pre>
	</div>
</details>
<details class="clean-accordion">
	<summary>CheckBoxRowIsHidden</summary>
	<div class="token-details">
		<p><strong>Description:</strong> Creates a checkbox for the IsHidden property of a row. Creates a checkbox for the &#39;IsHidden&#39; property of a row, allowing a row to be marked as hidden.</p>
		<strong>Signature</strong>
		<pre><code>public IEncodedString CheckBoxRowIsHidden(SimplisityInfo rowData)</code></pre>
		<strong>Example</strong>
		<pre><code>@CheckBoxRowIsHidden(rowData)</code></pre>
	</div>
</details>
<details class="clean-accordion">
	<summary>DeepL</summary>
	<div class="token-details">
		<p><strong>Description:</strong> Renders a button to open a DeepL translation modal. Requires a DeepL API key in the global settings and more than one portal language to be enabled. The translated text will populate the field specified by &#39;textId&#39;.</p>
		<strong>Signature</strong>
		<pre><code>public IEncodedString DeepL(string textId, string sourceTextId = &quot;&quot;, string cultureCode = &quot;&quot;)</code></pre>
		<strong>Example</strong>
		<pre><code>@DeepL(textId, sourceTextId, cultureCode)</code></pre>
	</div>
</details>
<details class="clean-accordion">
	<summary>RowKey</summary>
	<div class="token-details">
		<p><strong>Description:</strong> A row MUST have a rowkey to be saved to the DB.  This generates the rowkey. Generates the necessary hidden fields for a row&#39;s unique key (&#39;rowkey&#39; and &#39;rowkeylang&#39;) and a unique entity ID (&#39;eid&#39;). A row MUST have a rowkey to be saved to the database.</p>
		<strong>Signature</strong>
		<pre><code>public IEncodedString RowKey(SimplisityInfo info)</code></pre>
		<strong>Example</strong>
		<pre><code>@RowKey(info)</code></pre>
	</div>
</details>
<details class="clean-accordion">
	<summary>StylePadding</summary>
	<div class="token-details">
		<p><strong>Description:</strong> Standardized method and names to craete top,bottom,left,right padding on an element. Allows potion adjustment from module settings without change CSS files. field Id: leftpadding,rightpadding,toppadding,bottompadding Generates an inline CSS padding style string based on module settings. It reads &#39;leftpadding&#39;, &#39;rightpadding&#39;, &#39;toppadding&#39;, and &#39;bottompadding&#39; settings and creates corresponding CSS properties.</p>
		<strong>Signature</strong>
		<pre><code>public string StylePadding()</code></pre>
		<strong>Example</strong>
		<pre><code>@StylePadding()</code></pre>
	</div>
</details>
<details class="clean-accordion">
	<summary>TextBoxRowTitle</summary>
	<div class="token-details">
		<p><strong>Description:</strong> Creates a textbox for the title, using a standard xpath. Creates a standard textbox for a row&#39;s title using the XPath &#39;genxml/lang/genxml/textbox/title&#39;.</p>
		<strong>Signature</strong>
		<pre><code>public IEncodedString TextBoxRowTitle(SimplisityInfo rowData)</code></pre>
		<strong>Example</strong>
		<pre><code>@TextBoxRowTitle(rowData)</code></pre>
	</div>
</details>
