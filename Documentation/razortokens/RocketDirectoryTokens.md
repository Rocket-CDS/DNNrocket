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
		<pre><code>@{ AssignDataModel(Model); }</code></pre>
	</div>
</details>
<details class="clean-accordion">
	<summary>TextBoxMoney</summary>
	<div class="token-details">
		<p><strong>Description:</strong> Renders a textbox for currency input. The value is formatted according to the portal's currency settings.</p>
		<strong>Signature</strong>
		<pre><code>public IEncodedString TextBoxMoney(int portalId, string systemKey, string cultureCode, SimplisityInfo info, String xpath, String attributes = "", String defaultValue = "", bool localized = false, int row = 0, string listname = "", string type = "text")</code></pre>
		<strong>Example</strong>
		<pre><code>@TextBoxMoney(0, "rocketdirectoryapi", "en-US", Model.Info, "genxml/textbox/price")</code></pre>
	</div>
</details>
<details class="clean-accordion">
	<summary>InterfaceNameResourceKey</summary>
	<div class="token-details">
		<p><strong>Description:</strong> Gets the localized name for a RocketInterface from the resource files. It searches in the system's resources first, then the interface's template resources.</p>
		<strong>Signature</strong>
		<pre><code>public IEncodedString InterfaceNameResourceKey(RocketInterface rocketInterface, SystemLimpet systemData, String lang = "", string resxFileName = "SideMenu")</code></pre>
		<strong>Example</strong>
		<pre><code>@InterfaceNameResourceKey(myInterface, systemData)</code></pre>
	</div>
</details>
<details class="clean-accordion">
	<summary>FilterGroupCheckBox</summary>
	<div class="token-details">
		<p><strong>Description:</strong> Renders a checkbox for a property group filter, typically used in theme settings to enable or disable filter groups.</p>
		<strong>Signature</strong>
		<pre><code>public IEncodedString FilterGroupCheckBox(SimplisityInfo info, string groupId, string textName)</code></pre>
		<strong>Example</strong>
		<pre><code>@FilterGroupCheckBox(Model.Info, "group1", "Group 1 Filters")</code></pre>
	</div>
</details>
<details class="clean-accordion">
	<summary>FilterCheckBox</summary>
	<div class="token-details">
		<p><strong>Description:</strong> Renders a filter checkbox for the public-facing view. When changed, it updates a session field and triggers a JavaScript function to refresh the article list.</p>
		<strong>Signature</strong>
		<pre><code>public IEncodedString FilterCheckBox(string checkboxId, string textName, string sreturn, bool value, string cssClass = "", string attributes = "")</code></pre>
		<strong>Example</strong>
		<pre><code>@FilterCheckBox("filter-color-red", "Red", "#articlelist", false)</code></pre>
	</div>
</details>
<details class="clean-accordion">
	<summary>FilterJsApiCall</summary>
	<div class="token-details">
		<p><strong>Description:</strong> Renders the JavaScript function 'callFilterArticleList' which calls the remote API to refresh the list of articles based on the current filter selections.</p>
		<strong>Signature</strong>
		<pre><code>public IEncodedString FilterJsApiCall(ModuleContentLimpet moduleData, SessionParams sessionParams, string templateName = "articlelist.cshtml")</code></pre>
		<strong>Example</strong>
		<pre><code>@FilterJsApiCall(moduleData, sessionParams)</code></pre>
	</div>
</details>
<details class="clean-accordion">
	<summary>FilterClearButton</summary>
	<div class="token-details">
		<p><strong>Description:</strong> Renders a button that clears all active filters by unchecking all filter checkboxes and refreshing the article list.</p>
		<strong>Signature</strong>
		<pre><code>public IEncodedString FilterClearButton(string textName, string sreturn)</code></pre>
		<strong>Example</strong>
		<pre><code>@FilterClearButton("Clear All", "#articlelist")</code></pre>
	</div>
</details>
<details class="clean-accordion">
	<summary>TagButtonClear</summary>
	<div class="token-details">
		<p><strong>Description:</strong> Renders a button to clear the active tag filter. It is initially hidden and appears when a tag is selected.</p>
		<strong>Signature</strong>
		<pre><code>public IEncodedString TagButtonClear(string textName, SessionParams sessionParams, string displayClass = "rocket-tagbutton")</code></pre>
		<strong>Example</strong>
		<pre><code>@TagButtonClear("Clear Tag", sessionParams)</code></pre>
	</div>
</details>
<details class="clean-accordion">
	<summary>TagButton</summary>
	<div class="token-details">
		<p><strong>Description:</strong> Renders a clickable tag button. When clicked, it sets the 'rocketpropertyidtag' session field and refreshes the article list to show items with that tag.</p>
		<strong>Signature</strong>
		<pre><code>public IEncodedString TagButton(int propertyid, string textName, SessionParams sessionParams, string displayClass = "rocket-tagbutton", string selectedClass = "rocket-tagbuttonOn")</code></pre>
		<strong>Example</strong>
		<pre><code>@TagButton(123, "Featured", sessionParams)</code></pre>
	</div>
</details>
<details class="clean-accordion">
	<summary>TagJsApiCall</summary>
	<div class="token-details">
		<p><strong>Description:</strong> Renders the JavaScript function 'callTagArticleList' which calls the remote API to refresh the list of articles based on the selected tag.</p>
		<strong>Signature</strong>
		<pre><code>public IEncodedString TagJsApiCall(ModuleContentLimpet moduleData, string sreturn, SessionParams sessionParams, string templateName = "articlelist.cshtml")</code></pre>
		<strong>Example</strong>
		<pre><code>@TagJsApiCall(moduleData, "#articlelist", sessionParams)</code></pre>
	</div>
</details>
<details class="clean-accordion">
	<summary>DateJsApiCall</summary>
	<div class="token-details">
		<p><strong>Description:</strong> Renders the JavaScript function 'doDateSearchReload' which calls the remote API to refresh the list of articles based on a selected date range.</p>
		<strong>Signature</strong>
		<pre><code>public IEncodedString DateJsApiCall(ModuleContentLimpet moduleData, string sreturn, SessionParams sessionParams, string templateName = "articlelist.cshtml")</code></pre>
		<strong>Example</strong>
		<pre><code>@DateJsApiCall(moduleData, "#articlelist", sessionParams)</code></pre>
	</div>
</details>
<details class="clean-accordion">
	<summary>ListUrl</summary>
	<div class="token-details">
		<p><strong>Description:</strong> Builds a friendly URL to a list page, optionally including category information.</p>
		<strong>Signature</strong>
		<pre><code>public IEncodedString ListUrl(int listpageid, CategoryLimpet categoryData, string[] urlparams = null)</code></pre>
		<strong>Example</strong>
		<pre><code>@ListUrl(100, myCategory)</code></pre>
	</div>
</details>
<details class="clean-accordion">
	<summary>DetailUrl</summary>
	<div class="token-details">
		<p><strong>Description:</strong> Builds a friendly URL to a detail page for a specific article, including the article title and ID for SEO and routing.</p>
		<strong>Signature</strong>
		<pre><code>public IEncodedString DetailUrl(int detailpageid, ArticleLimpet articleData, string[] urlparams = null)</code></pre>
		<strong>Example</strong>
		<pre><code>@DetailUrl(101, myArticle)</code></pre>
	</div>
</details>
<details class="clean-accordion">
	<summary>RssUrl</summary>
	<div class="token-details">
		<p><strong>Description:</strong> Generates a URL for an RSS feed based on a command, date range, and optional SQL index.</p>
		<strong>Signature</strong>
		<pre><code>public IEncodedString RssUrl(int portalId, string cmd, int yearDate, int monthDate, int numberOfMonths = 1, string sqlidx = "")</code></pre>
		<strong>Example</strong>
		<pre><code>@RssUrl(0, "getfeed", 2023, 1, 12)</code></pre>
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
