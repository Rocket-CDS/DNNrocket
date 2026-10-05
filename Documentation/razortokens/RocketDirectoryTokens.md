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
		<p><strong>Description:</strong> Assigns the data model for Razor, making the template easier to build by populating various data properties like appTheme, moduleData, articleData, etc., from the SimplisityRazor model.</p>
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
	<summary>DateJsApiCall</summary>
	<div class="token-details">
		<p><strong>Description:</strong> Dates the js API call. JS function: doDateSearchReload cmd:remote_publiclist Renders the JavaScript function &#39;doDateSearchReload&#39; which calls the remote API to refresh the list of articles based on a selected date range.</p>
		<strong>Signature</strong>
		<pre><code>public IEncodedString DateJsApiCall(ModuleContentLimpet moduleData, string sreturn, SessionParams sessionParams, string templateName = &quot;articlelist.cshtml&quot;)</code></pre>
		<strong>Example</strong>
		<pre><code>@DateJsApiCall(moduleData, sreturn, sessionParams, templateName)</code></pre>
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
	<summary>DetailUrl</summary>
	<div class="token-details">
		<p><strong>Description:</strong> Builds the Detail URL. Builds a friendly URL to a detail page for a specific article, including the article title and ID for SEO and routing.</p>
		<strong>Signature</strong>
		<pre><code>public IEncodedString DetailUrl(int detailpageid, ArticleLimpet articleData, string[] urlparams = null)</code></pre>
		<strong>Example</strong>
		<pre><code>@DetailUrl(detailpageid, articleData, urlparams)</code></pre>
		<strong>Signature</strong>
		<pre><code>public IEncodedString DetailUrl(int detailpageid, ArticleLimpet articleData, CategoryLimpet categoryData, string[] urlparams = null)</code></pre>
		<strong>Example</strong>
		<pre><code>@DetailUrl(detailpageid, articleData, categoryData, urlparams)</code></pre>
	</div>
</details>
<details class="clean-accordion">
	<summary>FilterCheckBox</summary>
	<div class="token-details">
		<p><strong>Description:</strong> Filters the CheckBox on the &quot;Filters&quot; website view. Renders a filter checkbox for the public-facing view. When changed, it updates a session field and triggers a JavaScript function to refresh the article list.</p>
		<strong>Signature</strong>
		<pre><code>public IEncodedString FilterCheckBox(string checkboxId, string textName, string sreturn, bool value, string cssClass = &quot;&quot;, string attributes = &quot;&quot;)</code></pre>
		<strong>Example</strong>
		<pre><code>@FilterCheckBox(checkboxId, textName, sreturn, value, cssClass, attributes)</code></pre>
	</div>
</details>
<details class="clean-accordion">
	<summary>FilterClearButton</summary>
	<div class="token-details">
		<p><strong>Description:</strong> Renders a button that clears all active filters by unchecking all filter checkboxes and refreshing the article list.</p>
		<strong>Signature</strong>
		<pre><code>public IEncodedString FilterClearButton(string textName, string sreturn)</code></pre>
		<strong>Example</strong>
		<pre><code>@FilterClearButton(textName, sreturn)</code></pre>
	</div>
</details>
<details class="clean-accordion">
	<summary>FilterGroupCheckBox</summary>
	<div class="token-details">
		<p><strong>Description:</strong> CheckBox for a group filter. (Used in the ThemeSettings for selecting which group filters to use.) Renders a checkbox for a property group filter, typically used in theme settings to enable or disable filter groups.</p>
		<strong>Signature</strong>
		<pre><code>public IEncodedString FilterGroupCheckBox(SimplisityInfo info, string groupId, string textName)</code></pre>
		<strong>Example</strong>
		<pre><code>@FilterGroupCheckBox(info, groupId, textName)</code></pre>
		<strong>Signature</strong>
		<pre><code>public IEncodedString FilterGroupCheckBox(SimplisityInfo info, CatalogSettingsLimpet catalogSettings)</code></pre>
		<strong>Example</strong>
		<pre><code>@FilterGroupCheckBox(info, catalogSettings)</code></pre>
	</div>
</details>
<details class="clean-accordion">
	<summary>FilterJsApiCall</summary>
	<div class="token-details">
		<p><strong>Description:</strong> Adds the JS for calling the filter API. JS:callArticleList() cmd:remote_publiclist Renders the JavaScript function &#39;callFilterArticleList&#39; which calls the remote API to refresh the list of articles based on the current filter selections.</p>
		<strong>Signature</strong>
		<pre><code>public IEncodedString FilterJsApiCall(ModuleContentLimpet moduleData, SessionParams sessionParams, string templateName = &quot;articlelist.cshtml&quot;)</code></pre>
		<strong>Example</strong>
		<pre><code>@FilterJsApiCall(moduleData, sessionParams, templateName)</code></pre>
	</div>
</details>
<details class="clean-accordion">
	<summary>InterfaceNameResourceKey</summary>
	<div class="token-details">
		<p><strong>Description:</strong> Gets the interfaces name from the resource file. Gets the localized name for a RocketInterface from the resource files. It searches in the system&#39;s resources first, then the interface&#39;s template resources.</p>
		<strong>Signature</strong>
		<pre><code>public IEncodedString InterfaceNameResourceKey(RocketInterface rocketInterface, SystemLimpet systemData, String lang = &quot;&quot;, string resxFileName = &quot;SideMenu&quot;)</code></pre>
		<strong>Example</strong>
		<pre><code>@InterfaceNameResourceKey(rocketInterface, systemData, lang, resxFileName)</code></pre>
	</div>
</details>
<details class="clean-accordion">
	<summary>ListUrl</summary>
	<div class="token-details">
		<p><strong>Description:</strong> Builds the List URL. Builds a friendly URL to a list page, optionally including category information.</p>
		<strong>Signature</strong>
		<pre><code>public IEncodedString ListUrl(int listpageid, CategoryLimpet categoryData, string[] urlparams = null)</code></pre>
		<strong>Example</strong>
		<pre><code>@ListUrl(listpageid, categoryData, urlparams)</code></pre>
		<strong>Signature</strong>
		<pre><code>public IEncodedString ListUrl(int listpageid, string[] urlparams = null)</code></pre>
		<strong>Example</strong>
		<pre><code>@ListUrl(listpageid, urlparams)</code></pre>
	</div>
</details>
<details class="clean-accordion">
	<summary>RssUrl</summary>
	<div class="token-details">
		<p><strong>Description:</strong> Generates a URL for an RSS feed based on a command, date range, and optional SQL index.</p>
		<strong>Signature</strong>
		<pre><code>public IEncodedString RssUrl(int portalId, string cmd, int yearDate, int monthDate, int numberOfMonths = 1, string sqlidx = &quot;&quot;)</code></pre>
		<strong>Example</strong>
		<pre><code>@RssUrl(portalId, cmd, yearDate, monthDate, numberOfMonths, sqlidx)</code></pre>
	</div>
</details>
<details class="clean-accordion">
	<summary>TagButton</summary>
	<div class="token-details">
		<p><strong>Description:</strong> Renders a clickable tag button. When clicked, it sets the &#39;rocketpropertyidtag&#39; session field and refreshes the article list to show items with that tag.</p>
		<strong>Signature</strong>
		<pre><code>public IEncodedString TagButton(int propertyid, string textName, SessionParams sessionParams, string displayClass = &quot;rocket-tagbutton&quot;, string selectedClass = &quot;rocket-tagbuttonOn&quot;)</code></pre>
		<strong>Example</strong>
		<pre><code>@TagButton(propertyid, textName, sessionParams, displayClass, selectedClass)</code></pre>
	</div>
</details>
<details class="clean-accordion">
	<summary>TagButtonClear</summary>
	<div class="token-details">
		<p><strong>Description:</strong> Tags the button. Renders a button to clear the active tag filter. It is initially hidden and appears when a tag is selected.</p>
		<strong>Signature</strong>
		<pre><code>public IEncodedString TagButtonClear(string textName, SessionParams sessionParams, string displayClass = &quot;rocket-tagbutton&quot;)</code></pre>
		<strong>Example</strong>
		<pre><code>@TagButtonClear(textName, sessionParams, displayClass)</code></pre>
	</div>
</details>
<details class="clean-accordion">
	<summary>TagJsApiCall</summary>
	<div class="token-details">
		<p><strong>Description:</strong> Adds the JS for calling the filter API. JS:callTagArticleList() cmd:remote_publiclist</p>
		<strong>Signature</strong>
		<pre><code>public IEncodedString TagJsApiCall(ModuleContentLimpet moduleData, string sreturn, SessionParams sessionParams, string templateName = &quot;articlelist.cshtml&quot;)</code></pre>
		<strong>Example</strong>
		<pre><code>@TagJsApiCall(moduleData, sreturn, sessionParams, templateName)</code></pre>
		<strong>Signature</strong>
		<pre><code>public IEncodedString TagJsApiCall(ModuleContentLimpet moduleData, string sreturn, SessionParams sessionParams, string displayClass, string selectedClass, string templateName)</code></pre>
		<strong>Example</strong>
		<pre><code>@TagJsApiCall(moduleData, sreturn, sessionParams, displayClass, selectedClass, templateName)</code></pre>
	</div>
</details>
<details class="clean-accordion">
	<summary>TextBoxMoney</summary>
	<div class="token-details">
		<p><strong>Description:</strong> Renders a textbox for currency input. The value is formatted according to the portal&#39;s currency settings.</p>
		<strong>Signature</strong>
		<pre><code>public IEncodedString TextBoxMoney(int portalId, string systemKey, string cultureCode, SimplisityInfo info, String xpath, String attributes = &quot;&quot;, String defaultValue = &quot;&quot;, bool localized = false, int row = 0, string listname = &quot;&quot;, string type = &quot;text&quot;)</code></pre>
		<strong>Example</strong>
		<pre><code>@TextBoxMoney(portalId, systemKey, cultureCode, info, xpath, attributes, defaultValue, localized, row, listname, type)</code></pre>
	</div>
</details>
