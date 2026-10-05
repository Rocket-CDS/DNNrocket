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
	<summary>AddProcessDataResx</summary>
	<div class="token-details">
		<p><strong>Description:</strong> Adds resource paths to the process data for later use by resource key tokens. Can include portal-specific, app-theme-specific, and optionally the core API resx paths.</p>
		<strong>Signature</strong>
		<pre><code>public IEncodedString AddProcessDataResx(AppThemeLimpet appTheme, bool includeAPIresx = false)</code></pre>
		<strong>Example</strong>
		<pre><code>@AddProcessDataResx(appTheme, includeAPIresx)</code></pre>
	</div>
</details>
<details class="clean-accordion">
	<summary>ButtonIcon</summary>
	<div class="token-details">
		<p><strong>Description:</strong> Renders a button with only an icon, using the button text as the title attribute for accessibility.</p>
		<strong>Signature</strong>
		<pre><code>public IEncodedString ButtonIcon(ButtonTypes buttontype, String lang = &quot;&quot;)</code></pre>
		<strong>Example</strong>
		<pre><code>@ButtonIcon(buttontype, lang)</code></pre>
	</div>
</details>
<details class="clean-accordion">
	<summary>ButtonIconText</summary>
	<div class="token-details">
		<p><strong>Description:</strong> Renders a button with an icon followed by text, based on a button type.</p>
		<strong>Signature</strong>
		<pre><code>public IEncodedString ButtonIconText(ButtonTypes buttontype, String lang = &quot;&quot;)</code></pre>
		<strong>Example</strong>
		<pre><code>@ButtonIconText(buttontype, lang)</code></pre>
	</div>
</details>
<details class="clean-accordion">
	<summary>ButtonText</summary>
	<div class="token-details">
		<p><strong>Description:</strong> Renders a button with an icon followed by text.</p>
		<strong>Signature</strong>
		<pre><code>public IEncodedString ButtonText(ButtonTypes buttontype, String lang = &quot;&quot;)</code></pre>
		<strong>Example</strong>
		<pre><code>@ButtonText(buttontype, lang)</code></pre>
	</div>
</details>
<details class="clean-accordion">
	<summary>ButtonTextIcon</summary>
	<div class="token-details">
		<p><strong>Description:</strong> Renders a button with text followed by an icon, based on a button type.</p>
		<strong>Signature</strong>
		<pre><code>public IEncodedString ButtonTextIcon(ButtonTypes buttontype, String lang = &quot;&quot;)</code></pre>
		<strong>Example</strong>
		<pre><code>@ButtonTextIcon(buttontype, lang)</code></pre>
	</div>
</details>
<details class="clean-accordion">
	<summary>CheckBoxRowECOMode</summary>
	<div class="token-details">
		<p><strong>Description:</strong> Creates a checkbox for ECOMode in the settings of a module. Creates a checkbox for ECOMode in the settings of a module.</p>
		<strong>Signature</strong>
		<pre><code>public IEncodedString CheckBoxRowECOMode(SimplisityInfo rowData, bool defaultValue = true)</code></pre>
		<strong>Example</strong>
		<pre><code>@CheckBoxRowECOMode(rowData, defaultValue)</code></pre>
	</div>
</details>
<details class="clean-accordion">
	<summary>CKEditor4legacy</summary>
	<div class="token-details">
		<p><strong>Description:</strong> Legacy CKEditor 4 implementation. Consider using @Editor() instead. Legacy CKEditor 4 implementation. Consider using @Editor() instead.</p>
		<strong>Signature</strong>
		<pre><code>public IEncodedString CKEditor4legacy(SimplisityInfo info, string xpath, bool localized = false, int row = 0, string listname = &quot;&quot;, string langauge = &quot;&quot;, bool coded = false, string filename = &quot;ckeditor4startup1.js&quot;)</code></pre>
		<strong>Example</strong>
		<pre><code>@CKEditor4legacy(info, xpath, localized, row, listname, langauge, coded, filename)</code></pre>
	</div>
</details>
<details class="clean-accordion">
	<summary>DataSourceList</summary>
	<div class="token-details">
		<p><strong>Description:</strong> Renders a dropdown list of data sources (MODULEPARAMS) for a given system key.</p>
		<strong>Signature</strong>
		<pre><code>public IEncodedString DataSourceList(SimplisityInfo info, int systemkey, string xpath, string attributes = &quot;&quot;, bool allowEmpty = true, bool localized = false)</code></pre>
		<strong>Example</strong>
		<pre><code>@DataSourceList(info, systemkey, xpath, attributes, allowEmpty, localized)</code></pre>
	</div>
</details>
<details class="clean-accordion">
	<summary>DisplayEngineFlag</summary>
	<div class="token-details">
		<p><strong>Description:</strong> Displays a flag image from a remote engine URL for a given culture code.</p>
		<strong>Signature</strong>
		<pre><code>public IEncodedString DisplayEngineFlag(string engineUrl, string cultureCode, string classvalues = &quot;&quot;)</code></pre>
		<strong>Example</strong>
		<pre><code>@DisplayEngineFlag(engineUrl, cultureCode, classvalues)</code></pre>
	</div>
</details>
<details class="clean-accordion">
	<summary>DisplayFlag</summary>
	<div class="token-details">
		<p><strong>Description:</strong> Displays a flag image for a given culture code, if the image file exists.</p>
		<strong>Signature</strong>
		<pre><code>public IEncodedString DisplayFlag(string cultureCode, string classvalues = &quot;&quot;)</code></pre>
		<strong>Example</strong>
		<pre><code>@DisplayFlag(cultureCode, classvalues)</code></pre>
	</div>
</details>
<details class="clean-accordion">
	<summary>DropDownCountryCodeList</summary>
	<div class="token-details">
		<p><strong>Description:</strong> Renders a dropdown list of country codes.</p>
		<strong>Signature</strong>
		<pre><code>public IEncodedString DropDownCountryCodeList(SimplisityInfo info, String xpath, String attributes = &quot;&quot;, String defaultValue = &quot;&quot;, bool localized = false, int row = 0, string listname = &quot;&quot;)</code></pre>
		<strong>Example</strong>
		<pre><code>@DropDownCountryCodeList(info, xpath, attributes, defaultValue, localized, row, listname)</code></pre>
	</div>
</details>
<details class="clean-accordion">
	<summary>DropDownCultureCodeList</summary>
	<div class="token-details">
		<p><strong>Description:</strong> Renders a dropdown list of culture codes for the portal.</p>
		<strong>Signature</strong>
		<pre><code>public IEncodedString DropDownCultureCodeList(SimplisityInfo info, String xpath, String attributes = &quot;&quot;, String defaultValue = &quot;&quot;, bool localized = false, int row = 0, string listname = &quot;&quot;)</code></pre>
		<strong>Example</strong>
		<pre><code>@DropDownCultureCodeList(info, xpath, attributes, defaultValue, localized, row, listname)</code></pre>
	</div>
</details>
<details class="clean-accordion">
	<summary>DropDownCurrencyList</summary>
	<div class="token-details">
		<p><strong>Description:</strong> Renders a dropdown list of available currencies.</p>
		<strong>Signature</strong>
		<pre><code>public IEncodedString DropDownCurrencyList(SimplisityInfo info, String xpath, String attributes = &quot;&quot;, String defaultValue = &quot;&quot;, bool localized = false, int row = 0, string listname = &quot;&quot;)</code></pre>
		<strong>Example</strong>
		<pre><code>@DropDownCurrencyList(info, xpath, attributes, defaultValue, localized, row, listname)</code></pre>
	</div>
</details>
<details class="clean-accordion">
	<summary>DropDownLanguageList</summary>
	<div class="token-details">
		<p><strong>Description:</strong> Renders a dropdown list of enabled languages for the portal, with flags.</p>
		<strong>Signature</strong>
		<pre><code>public IEncodedString DropDownLanguageList(SimplisityInfo info, String xpath, String attributes = &quot;&quot;, String defaultValue = &quot;&quot;, bool localized = false, int row = 0, string listname = &quot;&quot;)</code></pre>
		<strong>Example</strong>
		<pre><code>@DropDownLanguageList(info, xpath, attributes, defaultValue, localized, row, listname)</code></pre>
	</div>
</details>
<details class="clean-accordion">
	<summary>DropDownSystemKeyList</summary>
	<div class="token-details">
		<p><strong>Description:</strong> Renders a dropdown list of active system keys.</p>
		<strong>Signature</strong>
		<pre><code>public IEncodedString DropDownSystemKeyList(SimplisityInfo info, String xpath, String attributes = &quot;&quot;, String defaultValue = &quot;&quot;, bool localized = false, int row = 0, string listname = &quot;&quot;)</code></pre>
		<strong>Example</strong>
		<pre><code>@DropDownSystemKeyList(info, xpath, attributes, defaultValue, localized, row, listname)</code></pre>
	</div>
</details>
<details class="clean-accordion">
	<summary>EditFlag</summary>
	<div class="token-details">
		<p><strong>Description:</strong> Displays the flag image for the current editing culture code from session parameters.</p>
		<strong>Signature</strong>
		<pre><code>public IEncodedString EditFlag(SessionParams sessionParams, string classvalues = &quot;&quot;)</code></pre>
		<strong>Example</strong>
		<pre><code>@EditFlag(sessionParams, classvalues)</code></pre>
	</div>
</details>
<details class="clean-accordion">
	<summary>Editor</summary>
	<div class="token-details">
		<p><strong>Description:</strong> Renders a rich text editor (defaulting to Jodit). The specific editor template can be configured in the portal settings or specified directly.</p>
		<strong>Signature</strong>
		<pre><code>public IEncodedString Editor(SimplisityInfo info, string xpath, SimplisityRazor model, int row = 0, string listname = &quot;&quot;, string editorRazorTemplate = &quot;EditorJoditDefault.cshtml&quot;)</code></pre>
		<strong>Example</strong>
		<pre><code>@Editor(info, xpath, model, row, listname, editorRazorTemplate)</code></pre>
	</div>
</details>
<details class="clean-accordion">
	<summary>GetTabUrlByGuid</summary>
	<div class="token-details">
		<p><strong>Description:</strong> Gets the URL for a tab by its unique GUID.</p>
		<strong>Signature</strong>
		<pre><code>public IEncodedString GetTabUrlByGuid(String tabguid)</code></pre>
		<strong>Example</strong>
		<pre><code>@GetTabUrlByGuid(tabguid)</code></pre>
		<strong>Signature</strong>
		<pre><code>public IEncodedString GetTabUrlByGuid(SimplisityInfo info, String xpath)</code></pre>
		<strong>Example</strong>
		<pre><code>@GetTabUrlByGuid(info, xpath)</code></pre>
	</div>
</details>
<details class="clean-accordion">
	<summary>GetTreeTabList</summary>
	<div class="token-details">
		<p><strong>Description:</strong> Generates a tree-structured HTML list of portal tabs with checkboxes for selection.</p>
		<strong>Signature</strong>
		<pre><code>public IEncodedString GetTreeTabList(int portalId, List&lt;int&gt; selectedTabIdList, string treeviewId, string lang = &quot;&quot;, string attributes = &quot;&quot;, bool showAllTabs = false)</code></pre>
		<strong>Example</strong>
		<pre><code>@GetTreeTabList(portalId, selectedTabIdList, treeviewId, lang, attributes, showAllTabs)</code></pre>
	</div>
</details>
<details class="clean-accordion">
	<summary>ImageUrl</summary>
	<div class="token-details">
		<p><strong>Description:</strong> Display Thumbnail Image  - DEFAULT RULE: PNG will always output as PNG all other supported image formats will output as WEBP. note: The default rule can be overwritten is the &quot;imgtype&quot; is passed. default rule set by FMC on 9/1/2024  - IMPORTANT: If you need to delete the image file you MUST remove the cache first. The cache holds a link to the locked image file and must be disposed. use: DNNrocketUtils.ClearThumbnailLock(); Display Thumbnail Image. Creates and returns a URL for a resized version of an image. Supports various output formats and cropping. By default, PNGs remain PNGs, and other formats are converted to WEBP. The cache holds a lock on the image file, so use DNNrocketUtils.ClearThumbnailLock() before deleting the original image.</p>
		<strong>Signature</strong>
		<pre><code>public IEncodedString ImageUrl(string url, int width = 0, int height = 0)</code></pre>
		<strong>Example</strong>
		<pre><code>@ImageUrl(url, width, height)</code></pre>
		<strong>Signature</strong>
		<pre><code>public IEncodedString ImageUrl(string url, int width = 0, int height = 0, string imgType = &quot;&quot;)</code></pre>
		<strong>Example</strong>
		<pre><code>@ImageUrl(url, width, height, imgType)</code></pre>
		<strong>Signature</strong>
		<pre><code>public IEncodedString ImageUrl(string url, int width = 0, int height = 0, string imgType = &quot;&quot;, bool cropCenter = true)</code></pre>
		<strong>Example</strong>
		<pre><code>@ImageUrl(url, width, height, imgType, cropCenter)</code></pre>
		<strong>Signature</strong>
		<pre><code>public IEncodedString ImageUrl(string engineUrl, string imgRelPath, int width, int height, string imgType, bool cropCenter)</code></pre>
		<strong>Example</strong>
		<pre><code>@ImageUrl(engineUrl, imgRelPath, width, height, imgType, cropCenter)</code></pre>
	</div>
</details>
<details class="clean-accordion">
	<summary>InjectHiddenFieldData</summary>
	<div class="token-details">
		<p><strong>Description:</strong> Add all genxml/hidden/* fields to the template. Renders all nodes under &#39;genxml/hidden/*&#39; as hidden input fields in the HTML. This is useful for passing data from the model to client-side scripts.</p>
		<strong>Signature</strong>
		<pre><code>public IEncodedString InjectHiddenFieldData(SimplisityInfo sInfo)</code></pre>
		<strong>Example</strong>
		<pre><code>@InjectHiddenFieldData(sInfo)</code></pre>
	</div>
</details>
<details class="clean-accordion">
	<summary>LinkInternalUrl</summary>
	<div class="token-details">
		<p><strong>Description:</strong> Generates a URL for an internal DNN page (tab) with a specific culture code and optional extra parameters. Generates a URL for an internal DNN page (tab) with a specific culture code and optional extra parameters.</p>
		<strong>Signature</strong>
		<pre><code>public IEncodedString LinkInternalUrl(int portalid, int tabid, string cultureCode, PortalSettings portalSettings = null, string[] extraparams = null)</code></pre>
		<strong>Example</strong>
		<pre><code>@LinkInternalUrl(portalid, tabid, cultureCode, portalSettings, extraparams)</code></pre>
	</div>
</details>
<details class="clean-accordion">
	<summary>LinkPageURL</summary>
	<div class="token-details">
		<p><strong>Description:</strong> Creates an anchor tag linking to an internal DNN page. The tab ID is read from a SimplisityInfo field.</p>
		<strong>Signature</strong>
		<pre><code>public IEncodedString LinkPageURL(SimplisityInfo info, string xpath, bool openInNewWindow = true, string text = &quot;&quot;, string attributes = &quot;&quot;)</code></pre>
		<strong>Example</strong>
		<pre><code>@LinkPageURL(info, xpath, openInNewWindow, text, attributes)</code></pre>
	</div>
</details>
<details class="clean-accordion">
	<summary>LinkURL</summary>
	<div class="token-details">
		<p><strong>Description:</strong> Creates an anchor tag for a URL stored in a SimplisityInfo field. Automatically handles adding &#39;https://&#39; if missing.</p>
		<strong>Signature</strong>
		<pre><code>public IEncodedString LinkURL(SimplisityInfo info, string xpath, bool openInNewWindow = true, string text = &quot;&quot;, string attributes = &quot;&quot;)</code></pre>
		<strong>Example</strong>
		<pre><code>@LinkURL(info, xpath, openInNewWindow, text, attributes)</code></pre>
	</div>
</details>
<details class="clean-accordion">
	<summary>ModSelectList</summary>
	<div class="token-details">
		<p><strong>Description:</strong> Renders a dropdown list of modules for a given portal, showing module references.</p>
		<strong>Signature</strong>
		<pre><code>public IEncodedString ModSelectList(SimplisityInfo info, String xpath, int portalId, String attributes = &quot;&quot;, bool addEmpty = true )</code></pre>
		<strong>Example</strong>
		<pre><code>@ModSelectList(info, xpath, portalId, attributes, addEmpty)</code></pre>
	</div>
</details>
<details class="clean-accordion">
	<summary>RenderDocumentSelect</summary>
	<div class="token-details">
		<p><strong>Description:</strong> Renders a document selection interface.</p>
		<strong>Signature</strong>
		<pre><code>public IEncodedString RenderDocumentSelect(string systemKey, string docFolderRel, bool singleselect = true, bool autoreturn = false)</code></pre>
		<strong>Example</strong>
		<pre><code>@RenderDocumentSelect(systemKey, docFolderRel, singleselect, autoreturn)</code></pre>
		<strong>Signature</strong>
		<pre><code>public IEncodedString RenderDocumentSelect(int portalId, int moduleid, string systemKey, bool singleselect = true, bool autoreturn = false)</code></pre>
		<strong>Example</strong>
		<pre><code>@RenderDocumentSelect(portalId, moduleid, systemKey, singleselect, autoreturn)</code></pre>
		<strong>Signature</strong>
		<pre><code>public IEncodedString RenderDocumentSelect(ModuleParams moduleParams, bool singleselect = true, bool autoreturn = false)</code></pre>
		<strong>Example</strong>
		<pre><code>@RenderDocumentSelect(moduleParams, singleselect, autoreturn)</code></pre>
	</div>
</details>
<details class="clean-accordion">
	<summary>RenderImageSelect</summary>
	<div class="token-details">
		<p><strong>Description:</strong> Renders an image selection interface.</p>
		<strong>Signature</strong>
		<pre><code>public IEncodedString RenderImageSelect(string systemKey, string imageFolderRel, bool singleselect = true, bool autoreturn = false)</code></pre>
		<strong>Example</strong>
		<pre><code>@RenderImageSelect(systemKey, imageFolderRel, singleselect, autoreturn)</code></pre>
		<strong>Signature</strong>
		<pre><code>public IEncodedString RenderImageSelect(int portalId, int moduleid, string systemKey, bool singleselect = true, bool autoreturn = false)</code></pre>
		<strong>Example</strong>
		<pre><code>@RenderImageSelect(portalId, moduleid, systemKey, singleselect, autoreturn)</code></pre>
		<strong>Signature</strong>
		<pre><code>public IEncodedString RenderImageSelect(ModuleParams moduleParams, bool singleselect = true, bool autoreturn = false)</code></pre>
		<strong>Example</strong>
		<pre><code>@RenderImageSelect(moduleParams, singleselect, autoreturn)</code></pre>
	</div>
</details>
<details class="clean-accordion">
	<summary>RenderLanguageSelector</summary>
	<div class="token-details">
		<p><strong>Description:</strong> Renders a language selector component with a dictionary for selector fields.</p>
		<strong>Signature</strong>
		<pre><code>public IEncodedString RenderLanguageSelector(string scmd, AppThemeSystemLimpet appThemeSystem, SimplisityRazor model)</code></pre>
		<strong>Example</strong>
		<pre><code>@RenderLanguageSelector(scmd, appThemeSystem, model)</code></pre>
		<strong>Signature</strong>
		<pre><code>public IEncodedString RenderLanguageSelector(string scmd, Dictionary&lt;string,string&gt; sfieldDict, AppThemeSystemLimpet appThemeSystem, SimplisityRazor model)</code></pre>
		<strong>Example</strong>
		<pre><code>@RenderLanguageSelector(scmd, sfieldDict, appThemeSystem, model)</code></pre>
	</div>
</details>
<details class="clean-accordion">
	<summary>RenderPlugin</summary>
	<div class="token-details">
		<p><strong>Description:</strong> Renders a plugin based on its registered interface key. The &#39;systemdata&#39; object must be available in the model.</p>
		<strong>Signature</strong>
		<pre><code>public IEncodedString RenderPlugin(string interfaceKey, string cmd, SimplisityRazor model)</code></pre>
		<strong>Example</strong>
		<pre><code>@RenderPlugin(interfaceKey, cmd, model)</code></pre>
	</div>
</details>
<details class="clean-accordion">
	<summary>RenderRemoteLanguageSelector</summary>
	<div class="token-details">
		<p><strong>Description:</strong> Renders a remote language selector component.</p>
		<strong>Signature</strong>
		<pre><code>public IEncodedString RenderRemoteLanguageSelector(string scmd, string sfields, AppThemeSystemLimpet appThemeSystem, SimplisityRazor model)</code></pre>
		<strong>Example</strong>
		<pre><code>@RenderRemoteLanguageSelector(scmd, sfields, appThemeSystem, model)</code></pre>
		<strong>Signature</strong>
		<pre><code>public IEncodedString RenderRemoteLanguageSelector(string scmd, string sfields, AppThemeDNNrocketLimpet appThemeDNNrocket, SimplisityRazor model)</code></pre>
		<strong>Example</strong>
		<pre><code>@RenderRemoteLanguageSelector(scmd, sfields, appThemeDNNrocket, model)</code></pre>
	</div>
</details>
<details class="clean-accordion">
	<summary>RenderTemplate</summary>
	<div class="token-details">
		<p><strong>Description:</strong> Renders a Razor template string with the given model. Renders a Razor template string with the given model.</p>
		<strong>Signature</strong>
		<pre><code>public IEncodedString RenderTemplate(string razorTemplateName, AppThemeRocketApiLimpet appThemeSystem, SimplisityRazor model, bool cacheOff = false)</code></pre>
		<strong>Example</strong>
		<pre><code>@RenderTemplate(razorTemplateName, appThemeSystem, model, cacheOff)</code></pre>
		<strong>Signature</strong>
		<pre><code>public IEncodedString RenderTemplate(string razorTemplateName, AppThemeSystemLimpet appThemeSystem, SimplisityRazor model, bool cacheOff = false)</code></pre>
		<strong>Example</strong>
		<pre><code>@RenderTemplate(razorTemplateName, appThemeSystem, model, cacheOff)</code></pre>
		<strong>Signature</strong>
		<pre><code>public IEncodedString RenderTemplate(string razorTemplateName, AppThemeDNNrocketLimpet appThemeSystem, SimplisityRazor model, bool cacheOff = false)</code></pre>
		<strong>Example</strong>
		<pre><code>@RenderTemplate(razorTemplateName, appThemeSystem, model, cacheOff)</code></pre>
		<strong>Signature</strong>
		<pre><code>public IEncodedString RenderTemplate(string razorTemplateName, AppThemeLimpet appTheme, SimplisityRazor model, bool cacheOff = false)</code></pre>
		<strong>Example</strong>
		<pre><code>@RenderTemplate(razorTemplateName, appTheme, model, cacheOff)</code></pre>
		<strong>Signature</strong>
		<pre><code>public IEncodedString RenderTemplate(string razorTemplateName, string moduleRef, AppThemeLimpet appTheme, SimplisityRazor model, bool cacheOff = false)</code></pre>
		<strong>Example</strong>
		<pre><code>@RenderTemplate(razorTemplateName, moduleRef, appTheme, model, cacheOff)</code></pre>
		<strong>Signature</strong>
		<pre><code>public IEncodedString RenderTemplate(string razorTemplate, SimplisityRazor model, bool debugMode = false)</code></pre>
		<strong>Example</strong>
		<pre><code>@RenderTemplate(razorTemplate, model, debugMode)</code></pre>
	</div>
</details>
<details class="clean-accordion">
	<summary>RenderXml</summary>
	<div class="token-details">
		<p><strong>Description:</strong> Renders a display of the XML model from a SimplisityInfo object for debugging purposes.</p>
		<strong>Signature</strong>
		<pre><code>public IEncodedString RenderXml(SimplisityInfo info, string xmlidx = &quot;&quot;)</code></pre>
		<strong>Example</strong>
		<pre><code>@RenderXml(info, xmlidx)</code></pre>
	</div>
</details>
<details class="clean-accordion">
	<summary>ResourceCSV</summary>
	<div class="token-details">
		<p><strong>Description:</strong> Output CSV list of resx values. &lt;br/&gt; @ResourceCSV(&quot;Resx File Name&quot;, &quot;csv list of resx file keys&quot;) Example:&lt;br/&gt; @ResourceCSV(&quot;RocketIntra&quot;, &quot;test1,test2,test3&quot;) Output CSV list of resx values. Example: @ResourceCSV(&quot;RocketIntra&quot;, &quot;test1,test2,test3&quot;)</p>
		<strong>Signature</strong>
		<pre><code>public IEncodedString ResourceCSV(String resourceFileKey, string keyListCSV, string lang = &quot;&quot;, string resourceExtension = &quot;Text&quot;)</code></pre>
		<strong>Example</strong>
		<pre><code>@ResourceCSV(resourceFileKey, keyListCSV, lang, resourceExtension)</code></pre>
	</div>
</details>
<details class="clean-accordion">
	<summary>ResourceKey</summary>
	<div class="token-details">
		<p><strong>Description:</strong> Gets a resource string from the resource paths previously added via AddProcessDataResx.</p>
		<strong>Signature</strong>
		<pre><code>public IEncodedString ResourceKey(String resourceFileKey, String lang = &quot;&quot;, String resourceExtension = &quot;Text&quot;)</code></pre>
		<strong>Example</strong>
		<pre><code>@ResourceKey(resourceFileKey, lang, resourceExtension)</code></pre>
	</div>
</details>
<details class="clean-accordion">
	<summary>ResourceKeyJS</summary>
	<div class="token-details">
		<p><strong>Description:</strong> Gets a resource string and escapes single quotes for safe use within JavaScript code.</p>
		<strong>Signature</strong>
		<pre><code>public IEncodedString ResourceKeyJS(String resourceFileKey, String lang = &quot;&quot;, String resourceExtension = &quot;Text&quot;)</code></pre>
		<strong>Example</strong>
		<pre><code>@ResourceKeyJS(resourceFileKey, lang, resourceExtension)</code></pre>
	</div>
</details>
<details class="clean-accordion">
	<summary>ResourceKeyMod</summary>
	<div class="token-details">
		<p><strong>Description:</strong> Gets a resource string, automatically prepending the key with a module reference and an underscore.</p>
		<strong>Signature</strong>
		<pre><code>public IEncodedString ResourceKeyMod(String moduleRef, String resourceFileKey, String lang = &quot;&quot;, String resourceExtension = &quot;Text&quot;)</code></pre>
		<strong>Example</strong>
		<pre><code>@ResourceKeyMod(moduleRef, resourceFileKey, lang, resourceExtension)</code></pre>
	</div>
</details>
<details class="clean-accordion">
	<summary>TabSelectListOnTabId</summary>
	<div class="token-details">
		<p><strong>Description:</strong> Renders a dropdown list of portal tabs (pages), structured as a tree. The value of each option is the TabId.</p>
		<strong>Signature</strong>
		<pre><code>public IEncodedString TabSelectListOnTabId(SimplisityInfo info, String xpath, String attributes = &quot;&quot;, Boolean allowEmpty = true, bool localized = false, int row = 0, string listname = &quot;&quot;, bool showAllTabs = false)</code></pre>
		<strong>Example</strong>
		<pre><code>@TabSelectListOnTabId(info, xpath, attributes, allowEmpty, localized, row, listname, showAllTabs)</code></pre>
	</div>
</details>
<details class="clean-accordion">
	<summary>Translate</summary>
	<div class="token-details">
		<p><strong>Description:</strong> Renders a translation icon that can be clicked to trigger a translation action for a specific field.</p>
		<strong>Signature</strong>
		<pre><code>public IEncodedString Translate(SimplisityInfo info, string xpath, bool active = true, int row = 0)</code></pre>
		<strong>Example</strong>
		<pre><code>@Translate(info, xpath, active, row)</code></pre>
	</div>
</details>
<details class="clean-accordion">
	<summary>TranslationKeyUp</summary>
	<div class="token-details">
		<p><strong>Description:</strong> Generates an &#39;onkeyup&#39; HTML attribute. When the user types in a field, this script will automatically set the corresponding translation lock to &#39;locked&#39;.</p>
		<strong>Signature</strong>
		<pre><code>public IEncodedString TranslationKeyUp(string fieldId, bool active = true, int row = 0)</code></pre>
		<strong>Example</strong>
		<pre><code>@TranslationKeyUp(fieldId, active, row)</code></pre>
	</div>
</details>
<details class="clean-accordion">
	<summary>TranslationLock</summary>
	<div class="token-details">
		<p><strong>Description:</strong> Renders a lock/unlock icon for managing the translation state of a field. It includes a hidden checkbox to store the state.</p>
		<strong>Signature</strong>
		<pre><code>public IEncodedString TranslationLock(SimplisityInfo info, string xpath, bool active = true, int row = 0)</code></pre>
		<strong>Example</strong>
		<pre><code>@TranslationLock(info, xpath, active, row)</code></pre>
	</div>
</details>
