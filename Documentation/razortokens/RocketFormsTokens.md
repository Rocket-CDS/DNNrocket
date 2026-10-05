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
	<summary>AssigDataModel</summary>
	<div class="token-details">
		<strong>Signature</strong>
		<pre><code>public string AssigDataModel(SimplisityRazor sModel)</code></pre>
		<strong>Example</strong>
		<pre><code>@AssigDataModel(sModel)</code></pre>
	</div>
</details>
<details class="clean-accordion">
	<summary>AssignDataModel</summary>
	<div class="token-details">
		<p><strong>Description:</strong> Assigns the data model for Razor, making the template easier to build by populating various data properties like appTheme, moduleData, portalData, etc., from the SimplisityRazor model.</p>
		<strong>Signature</strong>
		<pre><code>public string AssignDataModel(SimplisityRazor sModel)</code></pre>
		<strong>Example</strong>
		<pre><code>@AssignDataModel(sModel)</code></pre>
	</div>
</details>
<details class="clean-accordion">
	<summary>DelayFormButton</summary>
	<div class="token-details">
		<p><strong>Description:</strong> Displays a &quot;Post&quot; button with delay.  This is to avoid unwanted robot posts. Renders a form submission button that appears after a specified delay. This is a security measure to help prevent automated bot submissions. It renders a specified Razor template for the button&#39;s appearance.</p>
		<strong>Signature</strong>
		<pre><code>public IEncodedString DelayFormButton(SimplisityRazor sModel, string spost, int millisec = 1200, string template = &quot;DelayFormButton.cshtml&quot;)</code></pre>
		<strong>Example</strong>
		<pre><code>@DelayFormButton(sModel, spost, millisec, template)</code></pre>
	</div>
</details>
<details class="clean-accordion">
	<summary>ResourceKey</summary>
	<div class="token-details">
		<p><strong>Description:</strong> Gets a localized resource string. It searches through a list of resource paths and file keys (including &#39;RocketForms&#39; and theme-specific resx files) to find the requested key.</p>
		<strong>Signature</strong>
		<pre><code>public string ResourceKey(AppThemeLimpet appTheme, String resourceKey, String lang = &quot;&quot;, String resourceExtension = &quot;Text&quot;)</code></pre>
		<strong>Example</strong>
		<pre><code>@ResourceKey(appTheme, resourceKey, lang, resourceExtension)</code></pre>
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
