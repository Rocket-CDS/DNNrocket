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
		<pre><code>@AddProcessDataResx(appTheme, true)</code></pre>
	</div>
</details>
<details class="clean-accordion">
	<summary>DropDownLanguageList</summary>
	<div class="token-details">
		<p><strong>Description:</strong> Renders a dropdown list of enabled languages for the portal, with flags.</p>
		<strong>Signature</strong>
		<pre><code>public IEncodedString DropDownLanguageList(SimplisityInfo info, String xpath, String attributes = "", String defaultValue = "", bool localized = false, int row = 0, string listname = "")</code></pre>
		<strong>Example</strong>
		<pre><code>@DropDownLanguageList(Model.Info, "genxml/dropdown/language")</code></pre>
	</div>
</details>
<details class="clean-accordion">
	<summary>DropDownCurrencyList</summary>
	<div class="token-details">
		<p><strong>Description:</strong> Renders a dropdown list of available currencies.</p>
		<strong>Signature</strong>
		<pre><code>public IEncodedString DropDownCurrencyList(SimplisityInfo info, String xpath, String attributes = "", String defaultValue = "", bool localized = false, int row = 0, string listname = "")</code></pre>
		<strong>Example</strong>
		<pre><code>@DropDownCurrencyList(Model.Info, "genxml/dropdown/currency")</code></pre>
	</div>
</details>
<details class="clean-accordion">
	<summary>DropDownCultureCodeList</summary>
	<div class="token-details">
		<p><strong>Description:</strong> Renders a dropdown list of culture codes for the portal.</p>
		<strong>Signature</strong>
		<pre><code>public IEncodedString DropDownCultureCodeList(SimplisityInfo info, String xpath, String attributes = "", String defaultValue = "", bool localized = false, int row = 0, string listname = "")</code></pre>
		<strong>Example</strong>
		<pre><code>@DropDownCultureCodeList(Model.Info, "genxml/dropdown/culturecode")</code></pre>
	</div>
</details>
<details class="clean-accordion">
	<summary>DropDownCountryCodeList</summary>
	<div class="token-details">
		<p><strong>Description:</strong> Renders a dropdown list of country codes.</p>
		<strong>Signature</strong>
		<pre><code>public IEncodedString DropDownCountryCodeList(SimplisityInfo info, String xpath, String attributes = "", String defaultValue = "", bool localized = false, int row = 0, string listname = "")</code></pre>
		<strong>Example</strong>
		<pre><code>@DropDownCountryCodeList(Model.Info, "genxml/dropdown/countrycode")</code></pre>
	</div>
</details>
<details class="clean-accordion">
	<summary>DropDownSystemKeyList</summary>
	<div class="token-details">
		<p><strong>Description:</strong> Renders a dropdown list of active system keys.</p>
		<strong>Signature</strong>
		<pre><code>public IEncodedString DropDownSystemKeyList(SimplisityInfo info, String xpath, String attributes = "", String defaultValue = "", bool localized = false, int row = 0, string listname = "")</code></pre>
		<strong>Example</strong>
		<pre><code>@DropDownSystemKeyList(Model.Info, "genxml/dropdown/systemkey")</code></pre>
	</div>
</details>
<details class="clean-accordion">
	<summary>ResourceCSV</summary>
	<div class="token-details">
		<p><strong>Description:</strong> Output CSV list of resx values. Example: @ResourceCSV("RocketIntra", "test1,test2,test3")</p>
		<strong>Signature</strong>
		<pre><code>public IEncodedString ResourceCSV(String resourceFileKey, string keyListCSV, string lang = "", string resourceExtension = "Text")</code></pre>
		<strong>Example</strong>
		<pre><code>@ResourceCSV("MyResources", "key1,key2,key3")</code></pre>
	</div>
</details>
<details class="clean-accordion">
	<summary>ButtonTextIcon</summary>
	<div class="token-details">
		<p><strong>Description:</strong> Renders a button with text followed by an icon, based on a button type.</p>
		<strong>Signature</strong>
		<pre><code>public IEncodedString ButtonTextIcon(ButtonTypes buttontype, String lang = "")</code></pre>
		<strong>Example</strong>
		<pre><code>@ButtonTextIcon(ButtonTypes.Save)</code></pre>
	</div>
</details>
<details class="clean-accordion">
	<summary>ButtonIconText</summary>
	<div class="token-details">
		<p><strong>Description:</strong> Renders a button with an icon followed by text, based on a button type.</p>
		<strong>Signature</strong>
		<pre><code>public IEncodedString ButtonIconText(ButtonTypes buttontype, String lang = "")</code></pre>
		<strong>Example</strong>
		<pre><code>@ButtonIconText(ButtonTypes.Save)</code></pre>
	</div>
</details>
<details class="clean-accordion">
	<summary>ButtonText</summary>
	<div class="token-details">
		<p><strong>Description:</strong> Renders a button with an icon followed by text.</p>
		<strong>Signature</strong>
		<pre><code>public IEncodedString ButtonText(ButtonTypes buttontype, String lang = "")</code></pre>
		<strong>Example</strong>
		<pre><code>@ButtonText(ButtonTypes.Delete)</code></pre>
	</div>
</details>
<details class="clean-accordion">
	<summary>ButtonIcon</summary>
	<div class="token-details">
		<p><strong>Description:</strong> Renders a button with only an icon, using the button text as the title attribute for accessibility.</p>
		<strong>Signature</strong>
		<pre><code>public IEncodedString ButtonIcon(ButtonTypes buttontype, String lang = "")</code></pre>
		<strong>Example</strong>
		<pre><code>@ButtonIcon(ButtonTypes.Edit)</code></pre>
	</div>
</details>
<details class="clean-accordion">
	<summary>ResourceKeyMod</summary>
	<div class="token-details">
		<p><strong>Description:</strong> Gets a resource string, automatically prepending the key with a module reference and an underscore.</p>
		<strong>Signature</strong>
		<pre><code>public IEncodedString ResourceKeyMod(String moduleRef, String resourceFileKey, String lang = "", String resourceExtension = "Text")</code></pre>
		<strong>Example</strong>
		<pre><code>@ResourceKeyMod("MyModRef", "MyKey")</code></pre>
	</div>
</details>
<details class="clean-accordion">
	<summary>ResourceKey</summary>
	<div class="token-details">
		<p><strong>Description:</strong> Gets a resource string from the resource paths previously added via AddProcessDataResx.</p>
		<strong>Signature</strong>
		<pre><code>public IEncodedString ResourceKey(String resourceFileKey, String lang = "", String resourceExtension = "Text")</code></pre>
		<strong>Example</strong>
		<pre><code>@ResourceKey("WelcomeMessage")</code></pre>
	</div>
</details>
<details class="clean-accordion">
	<summary>ResourceKeyJS</summary>
	<div class="token-details">
		<p><strong>Description:</strong> Gets a resource string and escapes single quotes for safe use within JavaScript code.</p>
		<strong>Signature</strong>
		<pre><code>public IEncodedString ResourceKeyJS(String resourceFileKey, String lang = "", String resourceExtension = "Text")</code></pre>
		<strong>Example</strong>
		<pre><code>var message = '@ResourceKeyJS("AlertMessage")';</code></pre>
	</div>
</details>
<details class="clean-accordion">
	<summary>RenderLanguageSelector</summary>
	<div class="token-details">
		<p><strong>Description:</strong> Renders a language selector component with a dictionary for selector fields.</p>
		<strong>Signature</strong>
		<pre><code>public IEncodedString RenderLanguageSelector(string scmd, Dictionary&lt;string, string&gt; sfieldDict, AppThemeSystemLimpet appThemeSystem, SimplisityRazor model)</code></pre>
		<strong>Example</strong>
		<pre><code>@RenderLanguageSelector("changelang", new Dictionary&lt;string, string&gt;(), appThemeSystem, Model)</code></pre>
	</div>
</details>
<details class="clean-accordion">
	<summary>RenderRemoteLanguageSelector</summary>
	<div class="token-details">
		<p><strong>Description:</strong> Renders a remote language selector component.</p>
		<strong>Signature</strong>
		<pre><code>public IEncodedString RenderRemoteLanguageSelector(string scmd, string sfields, AppThemeSystemLimpet appThemeSystem, SimplisityRazor model)</code></pre>
		<strong>Example</strong>
		<pre><code>@RenderRemoteLanguageSelector("changelang", "{}", appThemeSystem, Model)</code></pre>
	</div>
</details>
<details class="clean-accordion">
	<summary>RenderTemplate</summary>
	<div class="token-details">
		<p><strong>Description:</strong> Renders a Razor template string with the given model.</p>
		<strong>Signature</strong>
		<pre><code>public IEncodedString RenderTemplate(string razorTemplate, SimplisityRazor model, bool debugMode = false)</code></pre>
		<strong>Example</strong>
		<pre><code>@RenderTemplate("&lt;div&gt;Hello @Model.Get('name')&lt;/div&gt;", myModel)</code></pre>
	</div>
</details>
<details class="clean-accordion">
	<summary>RenderPlugin</summary>
	<div class="token-details">
		<p><strong>Description:</strong> Renders a plugin based on its registered interface key. The 'systemdata' object must be available in the model.</p>
		<strong>Signature</strong>
		<pre><code>public IEncodedString RenderPlugin(string interfaceKey, string cmd, SimplisityRazor model)</code></pre>
		<strong>Example</strong>
		<pre><code>@RenderPlugin("myplugin", "showdetails", Model)</code></pre>
	</div>
</details>
<details class="clean-accordion">
	<summary>RenderXml</summary>
	<div class="token-details">
		<p><strong>Description:</strong> Renders a display of the XML model from a SimplisityInfo object for debugging purposes.</p>
		<strong>Signature</strong>
		<pre><code>public IEncodedString RenderXml(SimplisityInfo info, string xmlidx = "")</code></pre>
		<strong>Example</strong>
		<pre><code>@RenderXml(Model.Info)</code></pre>
	</div>
</details>
<details class="clean-accordion">
	<summary>RenderImageSelect</summary>
	<div class="token-details">
		<p><strong>Description:</strong> Renders an image selection interface.</p>
		<strong>Signature</strong>
		<pre><code>public IEncodedString RenderImageSelect(string systemKey, string imageFolderRel, bool singleselect = true, bool autoreturn = false)</code></pre>
		<strong>Example</strong>
		<pre><code>@RenderImageSelect("mysystem", "/Portals/0/Images")</code></pre>
	</div>
</details>
<details class="clean-accordion">
	<summary>RenderDocumentSelect</summary>
	<div class="token-details">
		<p><strong>Description:</strong> Renders a document selection interface.</p>
		<strong>Signature</strong>
		<pre><code>public IEncodedString RenderDocumentSelect(string systemKey, string docFolderRel, bool singleselect = true, bool autoreturn = false)</code></pre>
		<strong>Example</strong>
		<pre><code>@RenderDocumentSelect("mysystem", "/Portals/0/Documents")</code></pre>
	</div>
</details>
<details class="clean-accordion">
	<summary>TranslationLock</summary>
	<div class="token-details">
		<p><strong>Description:</strong> Renders a lock/unlock icon for managing the translation state of a field. It includes a hidden checkbox to store the state.</p>
		<strong>Signature</strong>
		<pre><code>public IEncodedString TranslationLock(SimplisityInfo info, string xpath, bool active = true, int row = 0)</code></pre>
		<strong>Example</strong>
		<pre><code>@TranslationLock(Model.Info, "genxml/textbox/title")</code></pre>
	</div>
</details>
<details class="clean-accordion">
	<summary>Translate</summary>
	<div class="token-details">
		<p><strong>Description:</strong> Renders a translation icon that can be clicked to trigger a translation action for a specific field.</p>
		<strong>Signature</strong>
		<pre><code>public IEncodedString Translate(SimplisityInfo info, string xpath, bool active = true, int row = 0)</code></pre>
		<strong>Example</strong>
		<pre><code>@Translate(Model.Info, "genxml/textbox/summary")</code></pre>
	</div>
</details>
<details class="clean-accordion">
	<summary>TranslationKeyUp</summary>
	<div class="token-details">
		<p><strong>Description:</strong> Generates an 'onkeyup' HTML attribute. When the user types in a field, this script will automatically set the corresponding translation lock to 'locked'.</p>
		<strong>Signature</strong>
		<pre><code>public IEncodedString TranslationKeyUp(string fieldId, bool active = true, int row = 0)</code></pre>
		<strong>Example</strong>
		<pre><code>&lt;input type='text' @TranslationKeyUp("title") /&gt;</code></pre>
	</div>
</details>
<details class="clean-accordion">
	<summary>EditFlag</summary>
	<div class="token-details">
		<p><strong>Description:</strong> Displays the flag image for the current editing culture code from session parameters.</p>
		<strong>Signature</strong>
		<pre><code>public IEncodedString EditFlag(SessionParams sessionParams, string classvalues = "")</code></pre>
		<strong>Example</strong>
		<pre><code>@EditFlag(Model.SessionParams, "my-flag-class")</code></pre>
	</div>
</details>
<details class="clean-accordion">
	<summary>DisplayFlag</summary>
	<div class="token-details">
		<p><strong>Description:</strong> Displays a flag image for a given culture code, if the image file exists.</p>
		<strong>Signature</strong>
		<pre><code>public IEncodedString DisplayFlag(string cultureCode, string classvalues = "")</code></pre>
		<strong>Example</strong>
		<pre><code>@DisplayFlag("fr-FR")</code></pre>
	</div>
</details>
<details class="clean-accordion">
	<summary>DisplayEngineFlag</summary>
	<div class="token-details">
		<p><strong>Description:</strong> Displays a flag image from a remote engine URL for a given culture code.</p>
		<strong>Signature</strong>
		<pre><code>public IEncodedString DisplayEngineFlag(string engineUrl, string cultureCode, string classvalues = "")</code></pre>
		<strong>Example</strong>
		<pre><code>@DisplayEngineFlag("https://myothersite.com", "de-DE")</code></pre>
	</div>
</details>
<details class="clean-accordion">
	<summary>ImageUrl</summary>
	<div class="token-details">
		<p><strong>Description:</strong> Display Thumbnail Image. Creates and returns a URL for a resized version of an image. Supports various output formats and cropping. By default, PNGs remain PNGs, and other formats are converted to WEBP. The cache holds a lock on the image file, so use DNNrocketUtils.ClearThumbnailLock() before deleting the original image.</p>
		<strong>Signature</strong>
		<pre><code>public IEncodedString ImageUrl(string engineUrl, string imgRelPath, int width, int height, string imgType, bool cropCenter)</code></pre>
		<strong>Example</strong>
		<pre><code>@ImageUrl("", "/Portals/0/my-image.jpg", 200, 200, "webp", true)</code></pre>
	</div>
</details>
<details class="clean-accordion">
	<summary>InjectHiddenFieldData</summary>
	<div class="token-details">
		<p><strong>Description:</strong> Renders all nodes under 'genxml/hidden/*' as hidden input fields in the HTML. This is useful for passing data from the model to client-side scripts.</p>
		<strong>Signature</strong>
		<pre><code>public IEncodedString InjectHiddenFieldData(SimplisityInfo sInfo)</code></pre>
		<strong>Example</strong>
		<pre><code>@InjectHiddenFieldData(Model.Info)</code></pre>
	</div>
</details>
<details class="clean-accordion">
	<summary>CKEditor4legacy</summary>
	<div class="token-details">
		<p><strong>Description:</strong> Legacy CKEditor 4 implementation. Consider using @Editor() instead.</p>
		<strong>Signature</strong>
		<pre><code>public IEncodedString CKEditor4legacy(SimplisityInfo info, string xpath, bool localized = false, int row = 0, string listname = "", string langauge = "", bool coded = false, string filename = "ckeditor4startup1.js")</code></pre>
		<strong>Example</strong>
		<pre><code>@CKEditor4legacy(Model.Info, "genxml/richtext/content")</code></pre>
	</div>
</details>
<details class="clean-accordion">
	<summary>Editor</summary>
	<div class="token-details">
		<p><strong>Description:</strong> Renders a rich text editor (defaulting to Jodit). The specific editor template can be configured in the portal settings or specified directly.</p>
		<strong>Signature</strong>
		<pre><code>public IEncodedString Editor(SimplisityInfo info, string xpath, SimplisityRazor model, int row = 0, string listname = "", string editorRazorTemplate = "EditorJoditDefault.cshtml")</code></pre>
		<strong>Example</strong>
		<pre><code>@Editor(Model.Info, "genxml/richtext/content", Model)</code></pre>
	</div>
</details>
<details class="clean-accordion">
	<summary>LinkInternalUrl</summary>
	<div class="token-details">
		<p><strong>Description:</strong> Generates a URL for an internal DNN page (tab) with a specific culture code and optional extra parameters.</p>
		<strong>Signature</strong>
		<pre><code>public IEncodedString LinkInternalUrl(int portalid, int tabid, string cultureCode, PortalSettings portalSettings = null, string[] extraparams = null)</code></pre>
		<strong>Example</strong>
		<pre><code>@LinkInternalUrl(0, 55, "en-US")</code></pre>
	</div>
</details>
<details class="clean-accordion">
	<summary>TabSelectListOnTabId</summary>
	<div class="token-details">
		<p><strong>Description:</strong> Renders a dropdown list of portal tabs (pages), structured as a tree. The value of each option is the TabId.</p>
		<strong>Signature</strong>
		<pre><code>public IEncodedString TabSelectListOnTabId(SimplisityInfo info, String xpath, String attributes = "", Boolean allowEmpty = true, bool localized = false, int row = 0, string listname = "", bool showAllTabs = false)</code></pre>
		<strong>Example</strong>
		<pre><code>@TabSelectListOnTabId(Model.Info, "genxml/dropdown/pagelink")</code></pre>
	</div>
</details>
<details class="clean-accordion">
	<summary>GetTabUrlByGuid</summary>
	<div class="token-details">
		<p><strong>Description:</strong> Gets the URL for a tab by its unique GUID.</p>
		<strong>Signature</strong>
		<pre><code>public IEncodedString GetTabUrlByGuid(String tabguid)</code></pre>
		<strong>Example</strong>
		<pre><code>@GetTabUrlByGuid("a1b2c3d4-e5f6-7890-1234-567890abcdef")</code></pre>
	</div>
</details>
<details class="clean-accordion">
	<summary>LinkPageURL</summary>
	<div class="token-details">
		<p><strong>Description:</strong> Creates an anchor tag linking to an internal DNN page. The tab ID is read from a SimplisityInfo field.</p>
		<strong>Signature</strong>
		<pre><code>public IEncodedString LinkPageURL(SimplisityInfo info, string xpath, bool openInNewWindow = true, string text = "", string attributes = "")</code></pre>
		<strong>Example</strong>
		<pre><code>@LinkPageURL(Model.Info, "genxml/data/linkedpageid", text: "Read More")</code></pre>
	</div>
</details>
<details class="clean-accordion">
	<summary>LinkURL</summary>
	<div class="token-details">
		<p><strong>Description:</strong> Creates an anchor tag for a URL stored in a SimplisityInfo field. Automatically handles adding 'https://' if missing.</p>
		<strong>Signature</strong>
		<pre><code>public IEncodedString LinkURL(SimplisityInfo info, string xpath, bool openInNewWindow = true, string text = "", string attributes = "")</code></pre>
		<strong>Example</strong>
		<pre><code>@LinkURL(Model.Info, "genxml/textbox/websiteurl", true, "Visit Website")</code></pre>
	</div>
</details>
<details class="clean-accordion">
	<summary>DataSourceList</summary>
	<div class="token-details">
		<p><strong>Description:</strong> Renders a dropdown list of data sources (MODULEPARAMS) for a given system key.</p>
		<strong>Signature</strong>
		<pre><code>public IEncodedString DataSourceList(SimplisityInfo info, int systemkey, string xpath, string attributes = "", bool allowEmpty = true, bool localized = false)</code></pre>
		<strong>Example</strong>
		<pre><code>@DataSourceList(Model.Info, 1, "genxml/dropdown/datasource")</code></pre>
	</div>
</details>
<details class="clean-accordion">
	<summary>GetTreeTabList</summary>
	<div class="token-details">
		<p><strong>Description:</strong> Generates a tree-structured HTML list of portal tabs with checkboxes for selection.</p>
		<strong>Signature</strong>
		<pre><code>public IEncodedString GetTreeTabList(int portalId, List&lt;int&gt; selectedTabIdList, string treeviewId, string lang = "", string attributes = "", bool showAllTabs = false)</code></pre>
		<strong>Example</strong>
		<pre><code>@GetTreeTabList(0, new List&lt;int&gt;(), "mytree")</code></pre>
	</div>
</details>
<details class="clean-accordion">
	<summary>ModSelectList</summary>
	<div class="token-details">
		<p><strong>Description:</strong> Renders a dropdown list of modules for a given portal, showing module references.</p>
		<strong>Signature</strong>
		<pre><code>public IEncodedString ModSelectList(SimplisityInfo info, String xpath, int portalId, String attributes = "", bool addEmpty = true)</code></pre>
		<strong>Example</strong>
		<pre><code>@ModSelectList(Model.Info, "genxml/dropdown/moduleid", 0)</code></pre>
	</div>
</details>
<details class="clean-accordion">
	<summary>CheckBoxRowECOMode</summary>
	<div class="token-details">
		<p><strong>Description:</strong> Creates a checkbox for ECOMode in the settings of a module.</p>
		<strong>Signature</strong>
		<pre><code>public IEncodedString CheckBoxRowECOMode(SimplisityInfo rowData, bool defaultValue = true)</code></pre>
		<strong>Example</strong>
		<pre><code>@CheckBoxRowECOMode(Model.Info)</code></pre>
	</div>
</details>
