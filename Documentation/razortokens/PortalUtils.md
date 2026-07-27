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
	<summary>GetCurrentPortalId</summary>
	<div class="token-details">
		<p><strong>Description:</strong> Gets the ID of the currently active portal.</p>
		<strong>Signature</strong>
		<pre><code>public static int GetCurrentPortalId()</code></pre>
	</div>
</details>
<details class="clean-accordion">
	<summary>DeletePortal</summary>
	<div class="token-details">
		<p><strong>Description:</strong> Deletes a portal by its ID.</p>
		<strong>Signature</strong>
		<pre><code>public static void DeletePortal(int portalId)</code></pre>
	</div>
</details>
<details class="clean-accordion">
	<summary>CreatePortal</summary>
	<div class="token-details">
		<p><strong>Description:</strong> Creates a new portal with the specified name and alias. Returns the new portal ID.</p>
		<strong>Signature</strong>
		<pre><code>public static int CreatePortal(string portalName, string strPortalAlias, int userId = -1, string description = "NewPortal", string cultureCode = "en-US", bool useEmailAsUserName = true)</code></pre>
		<strong>Parameters</strong>
		<ul>
			<li><code>portalName</code> (string)</li>
			<li><code>strPortalAlias</code> (string)</li>
			<li><code>userId</code> (int) — optional, defaults to -1</li>
			<li><code>description</code> (string) — optional, defaults to "NewPortal"</li>
			<li><code>cultureCode</code> (string) — optional, defaults to "en-US"</li>
			<li><code>useEmailAsUserName</code> (bool) — optional, defaults to true</li>
		</ul>
	</div>
</details>
<details class="clean-accordion">
	<summary>BuildPortal</summary>
	<div class="token-details">
		<p><strong>Description:</strong> Builds a portal from a config file, associating it with the specified admin user.</p>
		<strong>Signature</strong>
		<pre><code>public static void BuildPortal(int portalid, int portalAdminUserId, string buildconfigfile)</code></pre>
		<strong>Parameters</strong>
		<ul>
			<li><code>portalid</code> (int)</li>
			<li><code>portalAdminUserId</code> (int)</li>
			<li><code>buildconfigfile</code> (string)</li>
		</ul>
	</div>
</details>
<details class="clean-accordion">
	<summary>BuildDefaultSystems</summary>
	<div class="token-details">
		<p><strong>Description:</strong> Builds and registers the default Rocket systems for a portal.</p>
		<strong>Signature</strong>
		<pre><code>public static void BuildDefaultSystems(int portalId)</code></pre>
		<strong>Parameters</strong>
		<ul>
			<li><code>portalId</code> (int)</li>
		</ul>
	</div>
</details>
<details class="clean-accordion">
	<summary>ActivateSystem</summary>
	<div class="token-details">
		<p><strong>Description:</strong> Activates a Rocket system on the specified portal.</p>
		<strong>Signature</strong>
		<pre><code>public static void ActivateSystem(int portalId, SystemLimpet systemData, SimplisityInfo postInfo = null)</code></pre>
		<strong>Parameters</strong>
		<ul>
			<li><code>portalId</code> (int)</li>
			<li><code>systemData</code> (SystemLimpet)</li>
			<li><code>postInfo</code> (SimplisityInfo) — optional</li>
		</ul>
	</div>
</details>
<details class="clean-accordion">
	<summary>GetPortals</summary>
	<div class="token-details">
		<p><strong>Description:</strong> Returns a list of all portal IDs in the DNN installation.</p>
		<strong>Signature</strong>
		<pre><code>public static List&lt;int&gt; GetPortals()</code></pre>
	</div>
</details>
<details class="clean-accordion">
	<summary>GetPortalId</summary>
	<div class="token-details">
		<p><strong>Description:</strong> Gets the portal ID of the current portal context.</p>
		<strong>Signature</strong>
		<pre><code>public static int GetPortalId()</code></pre>
	</div>
</details>
<details class="clean-accordion">
	<summary>GetPortalIdBySiteKey</summary>
	<div class="token-details">
		<p><strong>Description:</strong> Looks up a portal ID by its site key.</p>
		<strong>Signature</strong>
		<pre><code>public static int GetPortalIdBySiteKey(string siteKey)</code></pre>
		<strong>Parameters</strong>
		<ul>
			<li><code>siteKey</code> (string)</li>
		</ul>
	</div>
</details>
<details class="clean-accordion">
	<summary>GetPortal</summary>
	<div class="token-details">
		<p><strong>Description:</strong> Returns the DNN PortalInfo object for the specified portal ID.</p>
		<strong>Signature</strong>
		<pre><code>public static PortalInfo GetPortal(int portalId)</code></pre>
		<strong>Parameters</strong>
		<ul>
			<li><code>portalId</code> (int)</li>
		</ul>
	</div>
</details>
<details class="clean-accordion">
	<summary>GetPortalName</summary>
	<div class="token-details">
		<p><strong>Description:</strong> Returns the display name of the specified portal.</p>
		<strong>Signature</strong>
		<pre><code>public static String GetPortalName(int portalId)</code></pre>
		<strong>Parameters</strong>
		<ul>
			<li><code>portalId</code> (int)</li>
		</ul>
	</div>
</details>
<details class="clean-accordion">
	<summary>Registration</summary>
	<div class="token-details">
		<p><strong>Description:</strong> Sets the portal registration type. Values: NoRegistration = 0, PrivateRegistration = 1, PublicRegistration = 2, VerifiedRegistration = 3.</p>
		<strong>Signature</strong>
		<pre><code>public static void Registration(int portalId, int regType)</code></pre>
		<strong>Parameters</strong>
		<ul>
			<li><code>portalId</code> (int)</li>
			<li><code>regType</code> (int) — NoRegistration = 0, PrivateRegistration = 1, PublicRegistration = 2, VerifiedRegistration = 3</li>
		</ul>
	</div>
</details>
<details class="clean-accordion">
	<summary>EnablePopups</summary>
	<div class="token-details">
		<p><strong>Description:</strong> Enables or disables popup windows for the specified portal.</p>
		<strong>Signature</strong>
		<pre><code>public static void EnablePopups(int portalId, bool value)</code></pre>
		<strong>Parameters</strong>
		<ul>
			<li><code>portalId</code> (int)</li>
			<li><code>value</code> (bool)</li>
		</ul>
	</div>
</details>
<details class="clean-accordion">
	<summary>SSLSetup</summary>
	<div class="token-details">
		<p><strong>Description:</strong> Configures the SSL setting for the specified portal.</p>
		<strong>Signature</strong>
		<pre><code>public static void SSLSetup(int portalId, int value)</code></pre>
		<strong>Parameters</strong>
		<ul>
			<li><code>portalId</code> (int)</li>
			<li><code>value</code> (int)</li>
		</ul>
	</div>
</details>
<details class="clean-accordion">
	<summary>PageHeadTextUpdate</summary>
	<div class="token-details">
		<p><strong>Description:</strong> Replaces the page head text (custom HTML injected into the &lt;head&gt; tag) for the specified portal.</p>
		<strong>Signature</strong>
		<pre><code>public static void PageHeadTextUpdate(int portalId, string value)</code></pre>
		<strong>Parameters</strong>
		<ul>
			<li><code>portalId</code> (int)</li>
			<li><code>value</code> (string)</li>
		</ul>
	</div>
</details>
<details class="clean-accordion">
	<summary>PageHeadTextAppend</summary>
	<div class="token-details">
		<p><strong>Description:</strong> Appends text to the page head (custom HTML injected into the &lt;head&gt; tag) for the specified portal.</p>
		<strong>Signature</strong>
		<pre><code>public static void PageHeadTextAppend(int portalId, string value)</code></pre>
		<strong>Parameters</strong>
		<ul>
			<li><code>portalId</code> (int)</li>
			<li><code>value</code> (string)</li>
		</ul>
	</div>
</details>
<details class="clean-accordion">
	<summary>UpdatePortalSetting</summary>
	<div class="token-details">
		<p><strong>Description:</strong> Saves or updates a named portal setting value.</p>
		<strong>Signature</strong>
		<pre><code>public static void UpdatePortalSetting(int portalId, string settingName, string settingValue)</code></pre>
		<strong>Parameters</strong>
		<ul>
			<li><code>portalId</code> (int)</li>
			<li><code>settingName</code> (string)</li>
			<li><code>settingValue</code> (string)</li>
		</ul>
	</div>
</details>
<details class="clean-accordion">
	<summary>UseEmailAsUserName</summary>
	<div class="token-details">
		<p><strong>Description:</strong> Configures whether the portal uses email addresses as usernames.</p>
		<strong>Signature</strong>
		<pre><code>public static void UseEmailAsUserName(int portalId, bool value)</code></pre>
		<strong>Parameters</strong>
		<ul>
			<li><code>portalId</code> (int)</li>
			<li><code>value</code> (bool)</li>
		</ul>
	</div>
</details>
<details class="clean-accordion">
	<summary>GetPortalSettings (current portal)</summary>
	<div class="token-details">
		<p><strong>Description:</strong> Returns the DNN PortalSettings for the current portal context.</p>
		<strong>Signature</strong>
		<pre><code>public static PortalSettings GetPortalSettings()</code></pre>
	</div>
</details>
<details class="clean-accordion">
	<summary>GetPortalSettings (by portal ID)</summary>
	<div class="token-details">
		<p><strong>Description:</strong> Returns the DNN PortalSettings for the specified portal ID.</p>
		<strong>Signature</strong>
		<pre><code>public static PortalSettings GetPortalSettings(int portalId)</code></pre>
		<strong>Parameters</strong>
		<ul>
			<li><code>portalId</code> (int)</li>
		</ul>
	</div>
</details>
<details class="clean-accordion">
	<summary>EditorTemplate</summary>
	<div class="token-details">
		<p><strong>Description:</strong> Returns the configured editor template name for the current portal.</p>
		<strong>Signature</strong>
		<pre><code>public static string EditorTemplate()</code></pre>
	</div>
</details>
<details class="clean-accordion">
	<summary>LoginTabId (by portal ID)</summary>
	<div class="token-details">
		<p><strong>Description:</strong> Returns the tab (page) ID of the login page for the specified portal.</p>
		<strong>Signature</strong>
		<pre><code>public static int LoginTabId(int portalId)</code></pre>
		<strong>Parameters</strong>
		<ul>
			<li><code>portalId</code> (int)</li>
		</ul>
	</div>
</details>
<details class="clean-accordion">
	<summary>LoginTabId (current portal)</summary>
	<div class="token-details">
		<p><strong>Description:</strong> Returns the tab (page) ID of the login page for the current portal.</p>
		<strong>Signature</strong>
		<pre><code>public static int LoginTabId()</code></pre>
	</div>
</details>
<details class="clean-accordion">
	<summary>GetPortalByModuleID</summary>
	<div class="token-details">
		<p><strong>Description:</strong> Returns the portal ID that hosts the specified module.</p>
		<strong>Signature</strong>
		<pre><code>public static int GetPortalByModuleID(int moduleId)</code></pre>
		<strong>Parameters</strong>
		<ul>
			<li><code>moduleId</code> (int)</li>
		</ul>
	</div>
</details>
<details class="clean-accordion">
	<summary>GetAllPortalIds</summary>
	<div class="token-details">
		<p><strong>Description:</strong> Returns a list of all portal IDs across the entire DNN installation.</p>
		<strong>Signature</strong>
		<pre><code>public static List&lt;int&gt; GetAllPortalIds()</code></pre>
	</div>
</details>
<details class="clean-accordion">
	<summary>GetAllPortalRecords</summary>
	<div class="token-details">
		<p><strong>Description:</strong> Returns a list of SimplisityRecord objects representing all portals.</p>
		<strong>Signature</strong>
		<pre><code>public static List&lt;SimplisityRecord&gt; GetAllPortalRecords()</code></pre>
	</div>
</details>
<details class="clean-accordion">
	<summary>CreatePortalFolder</summary>
	<div class="token-details">
		<p><strong>Description:</strong> Creates a folder within the portal's file system using the DNN folder API.</p>
		<strong>Signature</strong>
		<pre><code>public static void CreatePortalFolder(DotNetNuke.Entities.Portals.PortalSettings PortalSettings, string FolderName)</code></pre>
		<strong>Parameters</strong>
		<ul>
			<li><code>PortalSettings</code> (DotNetNuke.Entities.Portals.PortalSettings)</li>
			<li><code>FolderName</code> (string)</li>
		</ul>
	</div>
</details>
<details class="clean-accordion">
	<summary>SetUserRegistration</summary>
	<div class="token-details">
		<p><strong>Description:</strong> Sets the user registration mode for the portal (numeric value).</p>
		<strong>Signature</strong>
		<pre><code>public static void SetUserRegistration(int portalId, int value)</code></pre>
		<strong>Parameters</strong>
		<ul>
			<li><code>portalId</code> (int)</li>
			<li><code>value</code> (int)</li>
		</ul>
	</div>
</details>
<details class="clean-accordion">
	<summary>GetUserRegistration</summary>
	<div class="token-details">
		<p><strong>Description:</strong> Returns the current user registration mode value for the portal.</p>
		<strong>Signature</strong>
		<pre><code>public static int GetUserRegistration(int portalId)</code></pre>
		<strong>Parameters</strong>
		<ul>
			<li><code>portalId</code> (int)</li>
		</ul>
	</div>
</details>
<details class="clean-accordion">
	<summary>GetCurrentPortalSettings</summary>
	<div class="token-details">
		<p><strong>Description:</strong> Returns the DNN PortalSettings for the currently active portal context.</p>
		<strong>Signature</strong>
		<pre><code>public static PortalSettings GetCurrentPortalSettings()</code></pre>
	</div>
</details>
<details class="clean-accordion">
	<summary>GetDomainFromUrl</summary>
	<div class="token-details">
		<p><strong>Description:</strong> Extracts the domain name from a full URL string.</p>
		<strong>Signature</strong>
		<pre><code>public static string GetDomainFromUrl(string url)</code></pre>
		<strong>Parameters</strong>
		<ul>
			<li><code>url</code> (string)</li>
		</ul>
	</div>
</details>
<details class="clean-accordion">
	<summary>GetPortalAliasesWithCultureCode</summary>
	<div class="token-details">
		<p><strong>Description:</strong> Returns a dictionary mapping portal aliases to their culture codes for the specified portal.</p>
		<strong>Signature</strong>
		<pre><code>public static Dictionary&lt;string, string&gt; GetPortalAliasesWithCultureCode(int portalId)</code></pre>
		<strong>Parameters</strong>
		<ul>
			<li><code>portalId</code> (int)</li>
		</ul>
	</div>
</details>
<details class="clean-accordion">
	<summary>GetPortalAliases</summary>
	<div class="token-details">
		<p><strong>Description:</strong> Returns a list of all portal aliases (hostnames) for the specified portal.</p>
		<strong>Signature</strong>
		<pre><code>public static List&lt;string&gt; GetPortalAliases(int portalId)</code></pre>
		<strong>Parameters</strong>
		<ul>
			<li><code>portalId</code> (int)</li>
		</ul>
	</div>
</details>
<details class="clean-accordion">
	<summary>DefaultPortalAlias (by portal ID)</summary>
	<div class="token-details">
		<p><strong>Description:</strong> Returns the default portal alias for the specified portal. Uses the current portal if portalId is -1.</p>
		<strong>Signature</strong>
		<pre><code>public static string DefaultPortalAlias(int portalId = -1)</code></pre>
		<strong>Parameters</strong>
		<ul>
			<li><code>portalId</code> (int) — optional, defaults to -1</li>
		</ul>
	</div>
</details>
<details class="clean-accordion">
	<summary>DefaultPortalAlias (by portal ID and culture)</summary>
	<div class="token-details">
		<p><strong>Description:</strong> Returns the default portal alias for the specified portal and culture code.</p>
		<strong>Signature</strong>
		<pre><code>public static string DefaultPortalAlias(int portalId, string cultureCode)</code></pre>
		<strong>Parameters</strong>
		<ul>
			<li><code>portalId</code> (int)</li>
			<li><code>cultureCode</code> (string)</li>
		</ul>
	</div>
</details>
<details class="clean-accordion">
	<summary>AddPortalAlias</summary>
	<div class="token-details">
		<p><strong>Description:</strong> Adds a new portal alias, optionally associated with a culture code.</p>
		<strong>Signature</strong>
		<pre><code>public static void AddPortalAlias(int portalId, string portalAlias, string cultureCode = "")</code></pre>
		<strong>Parameters</strong>
		<ul>
			<li><code>portalId</code> (int)</li>
			<li><code>portalAlias</code> (string)</li>
			<li><code>cultureCode</code> (string) — optional</li>
		</ul>
	</div>
</details>
<details class="clean-accordion">
	<summary>DeletePortalAlias</summary>
	<div class="token-details">
		<p><strong>Description:</strong> Removes a portal alias from the specified portal.</p>
		<strong>Signature</strong>
		<pre><code>public static void DeletePortalAlias(int portalId, string portalAlias)</code></pre>
		<strong>Parameters</strong>
		<ul>
			<li><code>portalId</code> (int)</li>
			<li><code>portalAlias</code> (string)</li>
		</ul>
	</div>
</details>
<details class="clean-accordion">
	<summary>SetPrimaryPortalAlias</summary>
	<div class="token-details">
		<p><strong>Description:</strong> Sets or unsets the primary flag on a portal alias.</p>
		<strong>Signature</strong>
		<pre><code>public static void SetPrimaryPortalAlias(int portalId, string portalAlias, bool isPrimary)</code></pre>
		<strong>Parameters</strong>
		<ul>
			<li><code>portalId</code> (int)</li>
			<li><code>portalAlias</code> (string)</li>
			<li><code>isPrimary</code> (bool)</li>
		</ul>
	</div>
</details>
<details class="clean-accordion">
	<summary>SetSearchTabId</summary>
	<div class="token-details">
		<p><strong>Description:</strong> Sets the tab (page) ID used as the search results page for the specified portal.</p>
		<strong>Signature</strong>
		<pre><code>public static void SetSearchTabId(int portalId, int tabId)</code></pre>
		<strong>Parameters</strong>
		<ul>
			<li><code>portalId</code> (int)</li>
			<li><code>tabId</code> (int)</li>
		</ul>
	</div>
</details>
<details class="clean-accordion">
	<summary>SetDefaultLanguage</summary>
	<div class="token-details">
		<p><strong>Description:</strong> Sets the default language (culture code) for the specified portal.</p>
		<strong>Signature</strong>
		<pre><code>public static void SetDefaultLanguage(int portalId, string cultureCode)</code></pre>
		<strong>Parameters</strong>
		<ul>
			<li><code>portalId</code> (int)</li>
			<li><code>cultureCode</code> (string)</li>
		</ul>
	</div>
</details>
<details class="clean-accordion">
	<summary>GetDefaultLanguage</summary>
	<div class="token-details">
		<p><strong>Description:</strong> Returns the default culture code (language) for the specified portal.</p>
		<strong>Signature</strong>
		<pre><code>public static string GetDefaultLanguage(int portalId)</code></pre>
		<strong>Parameters</strong>
		<ul>
			<li><code>portalId</code> (int)</li>
		</ul>
	</div>
</details>
<details class="clean-accordion">
	<summary>RootDomain</summary>
	<div class="token-details">
		<p><strong>Description:</strong> Returns the root domain (hostname only) for the specified portal. Uses the current portal if portalId is -1.</p>
		<strong>Signature</strong>
		<pre><code>public static string RootDomain(int portalId = -1)</code></pre>
		<strong>Parameters</strong>
		<ul>
			<li><code>portalId</code> (int) — optional, defaults to -1</li>
		</ul>
	</div>
</details>
<details class="clean-accordion">
	<summary>DomainSubUrl</summary>
	<div class="token-details">
		<p><strong>Description:</strong> Returns the sub-URL path component of the portal domain. Uses the current portal if portalId is -1.</p>
		<strong>Signature</strong>
		<pre><code>public static string DomainSubUrl(int portalId = -1)</code></pre>
		<strong>Parameters</strong>
		<ul>
			<li><code>portalId</code> (int) — optional, defaults to -1</li>
		</ul>
	</div>
</details>
<details class="clean-accordion">
	<summary>GetPortalAlias</summary>
	<div class="token-details">
		<p><strong>Description:</strong> Returns the portal alias for the specified language/culture. Uses the current portal if portalid is -1.</p>
		<strong>Signature</strong>
		<pre><code>public static string GetPortalAlias(string lang, int portalid = -1)</code></pre>
		<strong>Parameters</strong>
		<ul>
			<li><code>lang</code> (string)</li>
			<li><code>portalid</code> (int) — optional, defaults to -1</li>
		</ul>
	</div>
</details>
<details class="clean-accordion">
	<summary>GetPortalIdByAlias</summary>
	<div class="token-details">
		<p><strong>Description:</strong> Looks up the portal ID for a given portal alias string.</p>
		<strong>Signature</strong>
		<pre><code>public static int GetPortalIdByAlias(string portalAlias)</code></pre>
		<strong>Parameters</strong>
		<ul>
			<li><code>portalAlias</code> (string)</li>
		</ul>
	</div>
</details>
<details class="clean-accordion">
	<summary>HomeDNNrocketDirectoryMapPath</summary>
	<div class="token-details">
		<p><strong>Description:</strong> Returns the physical server path to the portal's DNNrocket home directory. Uses the current portal if portalId is -1.</p>
		<strong>Signature</strong>
		<pre><code>public static string HomeDNNrocketDirectoryMapPath(int portalId = -1)</code></pre>
		<strong>Parameters</strong>
		<ul>
			<li><code>portalId</code> (int) — optional, defaults to -1</li>
		</ul>
	</div>
</details>
<details class="clean-accordion">
	<summary>HomeDNNrocketDirectoryRel</summary>
	<div class="token-details">
		<p><strong>Description:</strong> Returns the relative URL path to the portal's DNNrocket home directory. Uses the current portal if portalId is -1.</p>
		<strong>Signature</strong>
		<pre><code>public static string HomeDNNrocketDirectoryRel(int portalId = -1)</code></pre>
		<strong>Parameters</strong>
		<ul>
			<li><code>portalId</code> (int) — optional, defaults to -1</li>
		</ul>
	</div>
</details>
<details class="clean-accordion">
	<summary>DNNrocketThemesDirectoryMapPath</summary>
	<div class="token-details">
		<p><strong>Description:</strong> Returns the physical server path to the portal's DNNrocket themes directory. Uses the current portal if portalId is -1.</p>
		<strong>Signature</strong>
		<pre><code>public static string DNNrocketThemesDirectoryMapPath(int portalId = -1)</code></pre>
		<strong>Parameters</strong>
		<ul>
			<li><code>portalId</code> (int) — optional, defaults to -1</li>
		</ul>
	</div>
</details>
<details class="clean-accordion">
	<summary>DNNrocketThemesDirectoryRel</summary>
	<div class="token-details">
		<p><strong>Description:</strong> Returns the relative URL path to the portal's DNNrocket themes directory. Uses the current portal if portalId is -1.</p>
		<strong>Signature</strong>
		<pre><code>public static string DNNrocketThemesDirectoryRel(int portalId = -1)</code></pre>
		<strong>Parameters</strong>
		<ul>
			<li><code>portalId</code> (int) — optional, defaults to -1</li>
		</ul>
	</div>
</details>
<details class="clean-accordion">
	<summary>TempDirectoryMapPath</summary>
	<div class="token-details">
		<p><strong>Description:</strong> Returns the physical server path to the portal's temp directory. Uses the current portal if portalId is -1.</p>
		<strong>Signature</strong>
		<pre><code>public static string TempDirectoryMapPath(int portalId = -1)</code></pre>
		<strong>Parameters</strong>
		<ul>
			<li><code>portalId</code> (int) — optional, defaults to -1</li>
		</ul>
	</div>
</details>
<details class="clean-accordion">
	<summary>BackUpDirectoryMapPath</summary>
	<div class="token-details">
		<p><strong>Description:</strong> Returns the physical server path to the portal's backup directory. Uses the current portal if portalId is -1.</p>
		<strong>Signature</strong>
		<pre><code>public static string BackUpDirectoryMapPath(int portalId = -1)</code></pre>
		<strong>Parameters</strong>
		<ul>
			<li><code>portalId</code> (int) — optional, defaults to -1</li>
		</ul>
	</div>
</details>
<details class="clean-accordion">
	<summary>TempDirectoryRel</summary>
	<div class="token-details">
		<p><strong>Description:</strong> Returns the relative URL path to the portal's temp directory. Uses the current portal if portalId is -1.</p>
		<strong>Signature</strong>
		<pre><code>public static string TempDirectoryRel(int portalId = -1)</code></pre>
		<strong>Parameters</strong>
		<ul>
			<li><code>portalId</code> (int) — optional, defaults to -1</li>
		</ul>
	</div>
</details>
<details class="clean-accordion">
	<summary>HomeDirectoryMapPath</summary>
	<div class="token-details">
		<p><strong>Description:</strong> Returns the physical server path to the portal's home (Portals/{id}/) directory.</p>
		<strong>Signature</strong>
		<pre><code>public static string HomeDirectoryMapPath(int portalId)</code></pre>
		<strong>Parameters</strong>
		<ul>
			<li><code>portalId</code> (int)</li>
		</ul>
	</div>
</details>
<details class="clean-accordion">
	<summary>AddLanguage</summary>
	<div class="token-details">
		<p><strong>Description:</strong> Adds a language (culture) to the specified portal.</p>
		<strong>Signature</strong>
		<pre><code>public static void AddLanguage(int portalId, string cultureCode)</code></pre>
		<strong>Parameters</strong>
		<ul>
			<li><code>portalId</code> (int)</li>
			<li><code>cultureCode</code> (string)</li>
		</ul>
	</div>
</details>
<details class="clean-accordion">
	<summary>RemoveLanguage</summary>
	<div class="token-details">
		<p><strong>Description:</strong> Removes a language (culture) from the specified portal.</p>
		<strong>Signature</strong>
		<pre><code>public static void RemoveLanguage(int portalId, string cultureCode)</code></pre>
		<strong>Parameters</strong>
		<ul>
			<li><code>portalId</code> (int)</li>
			<li><code>cultureCode</code> (string)</li>
		</ul>
	</div>
</details>
<details class="clean-accordion">
	<summary>UpdatePortalCopyright</summary>
	<div class="token-details">
		<p><strong>Description:</strong> Updates the copyright message (footer text) for the specified portal.</p>
		<strong>Signature</strong>
		<pre><code>public static void UpdatePortalCopyright(int portalId, string copyrightMessage)</code></pre>
		<strong>Parameters</strong>
		<ul>
			<li><code>portalId</code> (int) — The portal ID</li>
			<li><code>copyrightMessage</code> (string) — The new copyright message</li>
		</ul>
	</div>
</details>
<details class="clean-accordion">
	<summary>GetRootDomainUrl</summary>
	<div class="token-details">
		<p><strong>Description:</strong> Returns the full root domain URL (including scheme) for the specified portal. Uses the current portal if portalId is -1.</p>
		<strong>Signature</strong>
		<pre><code>public static string GetRootDomainUrl(int portalId = -1)</code></pre>
		<strong>Parameters</strong>
		<ul>
			<li><code>portalId</code> (int) — optional, defaults to -1</li>
		</ul>
	</div>
</details>
<details class="clean-accordion">
	<summary>ClearPortalContent</summary>
	<div class="token-details">
		<p><strong>Description:</strong> Clears all Rocket content data for the specified portal.</p>
		<strong>Signature</strong>
		<pre><code>public static void ClearPortalContent(int portalId)</code></pre>
		<strong>Parameters</strong>
		<ul>
			<li><code>portalId</code> (int)</li>
		</ul>
	</div>
</details>
<details class="clean-accordion">
	<summary>GetCurrentPageSkinCssPath</summary>
	<div class="token-details">
		<p><strong>Description:</strong> Returns the relative URL path to the CSS file for the skin applied to the current page.</p>
		<strong>Signature</strong>
		<pre><code>public static string GetCurrentPageSkinCssPath()</code></pre>
	</div>
</details>
<details class="clean-accordion">
	<summary>GetCurrentPortalCssPath</summary>
	<div class="token-details">
		<p><strong>Description:</strong> Returns the relative URL path to the portal.css file for the current portal (/Portals/{portalId}/portal.css).</p>
		<strong>Signature</strong>
		<pre><code>public static string GetCurrentPortalCssPath()</code></pre>
	</div>
</details>
<details class="clean-accordion">
	<summary>GetEffectiveSkinSrcForCurrentPage</summary>
	<div class="token-details">
		<p><strong>Description:</strong> Returns the effective skin source path for the current page, resolving portal and page-level skin settings.</p>
		<strong>Signature</strong>
		<pre><code>public static string GetEffectiveSkinSrcForCurrentPage()</code></pre>
	</div>
</details>
<details class="clean-accordion">
	<summary>GetPortalThemeSkinSrc</summary>
	<div class="token-details">
		<p><strong>Description:</strong> Returns the skin source path configured as the portal-level default theme for the specified portal.</p>
		<strong>Signature</strong>
		<pre><code>public static string GetPortalThemeSkinSrc(int portalId)</code></pre>
		<strong>Parameters</strong>
		<ul>
			<li><code>portalId</code> (int)</li>
		</ul>
	</div>
</details>
<details class="clean-accordion">
	<summary>GetCurrentScheme</summary>
	<div class="token-details">
		<p><strong>Description:</strong> Returns the current HTTP scheme (e.g. "http" or "https") for the active request.</p>
		<strong>Signature</strong>
		<pre><code>public static string GetCurrentScheme()</code></pre>
	</div>
</details>
<details class="clean-accordion">
	<summary>GetCurrentBaseUrl</summary>
	<div class="token-details">
		<p><strong>Description:</strong> Returns the base URL (scheme + domain) for the current request.</p>
		<strong>Signature</strong>
		<pre><code>public static string GetCurrentBaseUrl()</code></pre>
	</div>
</details>
