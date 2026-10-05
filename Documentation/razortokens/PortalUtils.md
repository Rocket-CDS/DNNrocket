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
	<summary>ActivateSystem</summary>
	<div class="token-details">
		<strong>Signature</strong>
		<pre><code>public static void ActivateSystem(int portalId, SystemLimpet systemData, SimplisityInfo postInfo = null)</code></pre>
		<strong>Example</strong>
		<pre><code>ActivateSystem(portalId, systemData, postInfo)</code></pre>
	</div>
</details>
<details class="clean-accordion">
	<summary>AddLanguage</summary>
	<div class="token-details">
		<strong>Signature</strong>
		<pre><code>public static void AddLanguage(int portalId, string cultureCode)</code></pre>
		<strong>Example</strong>
		<pre><code>AddLanguage(portalId, cultureCode)</code></pre>
	</div>
</details>
<details class="clean-accordion">
	<summary>AddPortalAlias</summary>
	<div class="token-details">
		<strong>Signature</strong>
		<pre><code>public static void AddPortalAlias(int portalId, string portalAlias, string cultureCode = &quot;&quot;)</code></pre>
		<strong>Example</strong>
		<pre><code>AddPortalAlias(portalId, portalAlias, cultureCode)</code></pre>
	</div>
</details>
<details class="clean-accordion">
	<summary>BackUpDirectoryMapPath</summary>
	<div class="token-details">
		<strong>Signature</strong>
		<pre><code>public static string BackUpDirectoryMapPath(int portalId = -1)</code></pre>
		<strong>Example</strong>
		<pre><code>BackUpDirectoryMapPath(portalId)</code></pre>
	</div>
</details>
<details class="clean-accordion">
	<summary>BuildDefaultSystems</summary>
	<div class="token-details">
		<strong>Signature</strong>
		<pre><code>public static void BuildDefaultSystems(int portalId)</code></pre>
		<strong>Example</strong>
		<pre><code>BuildDefaultSystems(portalId)</code></pre>
	</div>
</details>
<details class="clean-accordion">
	<summary>BuildPortal</summary>
	<div class="token-details">
		<strong>Signature</strong>
		<pre><code>public static void BuildPortal(int portalid, int portalAdminUserId, string buildconfigfile)</code></pre>
		<strong>Example</strong>
		<pre><code>BuildPortal(portalid, portalAdminUserId, buildconfigfile)</code></pre>
	</div>
</details>
<details class="clean-accordion">
	<summary>ClearPortalContent</summary>
	<div class="token-details">
		<strong>Signature</strong>
		<pre><code>public static void ClearPortalContent(int portalId)</code></pre>
		<strong>Example</strong>
		<pre><code>ClearPortalContent(portalId)</code></pre>
	</div>
</details>
<details class="clean-accordion">
	<summary>CreatePortal</summary>
	<div class="token-details">
		<strong>Signature</strong>
		<pre><code>public static int CreatePortal(string portalName, string strPortalAlias, int userId = -1, string description = &quot;NewPortal&quot;, string cultureCode = &quot;en-US&quot;, bool useEmailAsUserName = true)</code></pre>
		<strong>Example</strong>
		<pre><code>CreatePortal(portalName, strPortalAlias, userId, description, cultureCode, useEmailAsUserName)</code></pre>
	</div>
</details>
<details class="clean-accordion">
	<summary>CreatePortalFolder</summary>
	<div class="token-details">
		<strong>Signature</strong>
		<pre><code>public static void CreatePortalFolder(DotNetNuke.Entities.Portals.PortalSettings PortalSettings, string FolderName)</code></pre>
		<strong>Example</strong>
		<pre><code>CreatePortalFolder(PortalSettings, FolderName)</code></pre>
	</div>
</details>
<details class="clean-accordion">
	<summary>CreateRocketDirectories</summary>
	<div class="token-details">
		<strong>Signature</strong>
		<pre><code>public static void CreateRocketDirectories(int portalId = -1)</code></pre>
		<strong>Example</strong>
		<pre><code>CreateRocketDirectories(portalId)</code></pre>
	</div>
</details>
<details class="clean-accordion">
	<summary>DefaultPortalAlias</summary>
	<div class="token-details">
		<strong>Signature</strong>
		<pre><code>public static string DefaultPortalAlias(int portalId = -1)</code></pre>
		<strong>Example</strong>
		<pre><code>DefaultPortalAlias(portalId)</code></pre>
		<strong>Signature</strong>
		<pre><code>public static string DefaultPortalAlias(int portalId, string cultureCode)</code></pre>
		<strong>Example</strong>
		<pre><code>DefaultPortalAlias(portalId, cultureCode)</code></pre>
	</div>
</details>
<details class="clean-accordion">
	<summary>DeletePortal</summary>
	<div class="token-details">
		<strong>Signature</strong>
		<pre><code>public static void DeletePortal(int portalId)</code></pre>
		<strong>Example</strong>
		<pre><code>DeletePortal(portalId)</code></pre>
	</div>
</details>
<details class="clean-accordion">
	<summary>DeletePortalAlias</summary>
	<div class="token-details">
		<strong>Signature</strong>
		<pre><code>public static void DeletePortalAlias(int portalId, string portalAlias)</code></pre>
		<strong>Example</strong>
		<pre><code>DeletePortalAlias(portalId, portalAlias)</code></pre>
	</div>
</details>
<details class="clean-accordion">
	<summary>DNNrocketThemesDirectoryMapPath</summary>
	<div class="token-details">
		<strong>Signature</strong>
		<pre><code>public static string DNNrocketThemesDirectoryMapPath(int portalId = -1)</code></pre>
		<strong>Example</strong>
		<pre><code>DNNrocketThemesDirectoryMapPath(portalId)</code></pre>
	</div>
</details>
<details class="clean-accordion">
	<summary>DNNrocketThemesDirectoryRel</summary>
	<div class="token-details">
		<strong>Signature</strong>
		<pre><code>public static string DNNrocketThemesDirectoryRel(int portalId = -1)</code></pre>
		<strong>Example</strong>
		<pre><code>DNNrocketThemesDirectoryRel(portalId)</code></pre>
	</div>
</details>
<details class="clean-accordion">
	<summary>DomainSubUrl</summary>
	<div class="token-details">
		<strong>Signature</strong>
		<pre><code>public static string DomainSubUrl(int portalId = -1)</code></pre>
		<strong>Example</strong>
		<pre><code>DomainSubUrl(portalId)</code></pre>
	</div>
</details>
<details class="clean-accordion">
	<summary>EditorTemplate</summary>
	<div class="token-details">
		<strong>Signature</strong>
		<pre><code>public static string EditorTemplate()</code></pre>
		<strong>Example</strong>
		<pre><code>EditorTemplate()</code></pre>
	</div>
</details>
<details class="clean-accordion">
	<summary>EnablePopups</summary>
	<div class="token-details">
		<strong>Signature</strong>
		<pre><code>public static void EnablePopups(int portalId, bool value)</code></pre>
		<strong>Example</strong>
		<pre><code>EnablePopups(portalId, value)</code></pre>
	</div>
</details>
<details class="clean-accordion">
	<summary>GetAllPortalIds</summary>
	<div class="token-details">
		<strong>Signature</strong>
		<pre><code>public static List&lt;int&gt; GetAllPortalIds()</code></pre>
		<strong>Example</strong>
		<pre><code>GetAllPortalIds()</code></pre>
	</div>
</details>
<details class="clean-accordion">
	<summary>GetAllPortalRecords</summary>
	<div class="token-details">
		<strong>Signature</strong>
		<pre><code>public static List&lt;SimplisityRecord&gt; GetAllPortalRecords()</code></pre>
		<strong>Example</strong>
		<pre><code>GetAllPortalRecords()</code></pre>
	</div>
</details>
<details class="clean-accordion">
	<summary>GetCurrentBaseUrl</summary>
	<div class="token-details">
		<strong>Signature</strong>
		<pre><code>public static string GetCurrentBaseUrl()</code></pre>
		<strong>Example</strong>
		<pre><code>GetCurrentBaseUrl()</code></pre>
	</div>
</details>
<details class="clean-accordion">
	<summary>GetCurrentPageSkinCssPath</summary>
	<div class="token-details">
		<strong>Signature</strong>
		<pre><code>public static string GetCurrentPageSkinCssPath()</code></pre>
		<strong>Example</strong>
		<pre><code>GetCurrentPageSkinCssPath()</code></pre>
	</div>
</details>
<details class="clean-accordion">
	<summary>GetCurrentPortalCssPath</summary>
	<div class="token-details">
		<strong>Signature</strong>
		<pre><code>public static string GetCurrentPortalCssPath()</code></pre>
		<strong>Example</strong>
		<pre><code>GetCurrentPortalCssPath()</code></pre>
	</div>
</details>
<details class="clean-accordion">
	<summary>GetCurrentPortalId</summary>
	<div class="token-details">
		<strong>Signature</strong>
		<pre><code>public static int GetCurrentPortalId()</code></pre>
		<strong>Example</strong>
		<pre><code>GetCurrentPortalId()</code></pre>
	</div>
</details>
<details class="clean-accordion">
	<summary>GetCurrentPortalSettings</summary>
	<div class="token-details">
		<strong>Signature</strong>
		<pre><code>public static PortalSettings GetCurrentPortalSettings()</code></pre>
		<strong>Example</strong>
		<pre><code>GetCurrentPortalSettings()</code></pre>
	</div>
</details>
<details class="clean-accordion">
	<summary>GetCurrentScheme</summary>
	<div class="token-details">
		<strong>Signature</strong>
		<pre><code>public static string GetCurrentScheme()</code></pre>
		<strong>Example</strong>
		<pre><code>GetCurrentScheme()</code></pre>
	</div>
</details>
<details class="clean-accordion">
	<summary>GetDefaultLanguage</summary>
	<div class="token-details">
		<strong>Signature</strong>
		<pre><code>public static string GetDefaultLanguage(int portalId)</code></pre>
		<strong>Example</strong>
		<pre><code>GetDefaultLanguage(portalId)</code></pre>
	</div>
</details>
<details class="clean-accordion">
	<summary>GetDomainFromUrl</summary>
	<div class="token-details">
		<strong>Signature</strong>
		<pre><code>public static string GetDomainFromUrl(string url)</code></pre>
		<strong>Example</strong>
		<pre><code>GetDomainFromUrl(url)</code></pre>
	</div>
</details>
<details class="clean-accordion">
	<summary>GetEffectiveSkinSrcForCurrentPage</summary>
	<div class="token-details">
		<strong>Signature</strong>
		<pre><code>public static string GetEffectiveSkinSrcForCurrentPage()</code></pre>
		<strong>Example</strong>
		<pre><code>GetEffectiveSkinSrcForCurrentPage()</code></pre>
	</div>
</details>
<details class="clean-accordion">
	<summary>GetPortal</summary>
	<div class="token-details">
		<strong>Signature</strong>
		<pre><code>public static PortalInfo GetPortal(int portalId)</code></pre>
		<strong>Example</strong>
		<pre><code>GetPortal(portalId)</code></pre>
	</div>
</details>
<details class="clean-accordion">
	<summary>GetPortalAlias</summary>
	<div class="token-details">
		<strong>Signature</strong>
		<pre><code>public static string GetPortalAlias(string lang, int portalid = -1)</code></pre>
		<strong>Example</strong>
		<pre><code>GetPortalAlias(lang, portalid)</code></pre>
	</div>
</details>
<details class="clean-accordion">
	<summary>GetPortalAliases</summary>
	<div class="token-details">
		<strong>Signature</strong>
		<pre><code>public static List&lt;string&gt; GetPortalAliases(int portalId)</code></pre>
		<strong>Example</strong>
		<pre><code>GetPortalAliases(portalId)</code></pre>
	</div>
</details>
<details class="clean-accordion">
	<summary>GetPortalAliasesWithCultureCode</summary>
	<div class="token-details">
		<strong>Signature</strong>
		<pre><code>public static Dictionary&lt;string, string&gt; GetPortalAliasesWithCultureCode(int portalId)</code></pre>
		<strong>Example</strong>
		<pre><code>GetPortalAliasesWithCultureCode(portalId)</code></pre>
	</div>
</details>
<details class="clean-accordion">
	<summary>GetPortalByModuleID</summary>
	<div class="token-details">
		<strong>Signature</strong>
		<pre><code>public static int GetPortalByModuleID(int moduleId)</code></pre>
		<strong>Example</strong>
		<pre><code>GetPortalByModuleID(moduleId)</code></pre>
	</div>
</details>
<details class="clean-accordion">
	<summary>GetPortalId</summary>
	<div class="token-details">
		<strong>Signature</strong>
		<pre><code>public static int GetPortalId()</code></pre>
		<strong>Example</strong>
		<pre><code>GetPortalId()</code></pre>
	</div>
</details>
<details class="clean-accordion">
	<summary>GetPortalIdByAlias</summary>
	<div class="token-details">
		<strong>Signature</strong>
		<pre><code>public static int GetPortalIdByAlias(string portalAlias)</code></pre>
		<strong>Example</strong>
		<pre><code>GetPortalIdByAlias(portalAlias)</code></pre>
	</div>
</details>
<details class="clean-accordion">
	<summary>GetPortalIdBySiteKey</summary>
	<div class="token-details">
		<strong>Signature</strong>
		<pre><code>public static int GetPortalIdBySiteKey(string siteKey)</code></pre>
		<strong>Example</strong>
		<pre><code>GetPortalIdBySiteKey(siteKey)</code></pre>
	</div>
</details>
<details class="clean-accordion">
	<summary>GetPortalName</summary>
	<div class="token-details">
		<strong>Signature</strong>
		<pre><code>public static String GetPortalName(int portalId)</code></pre>
		<strong>Example</strong>
		<pre><code>GetPortalName(portalId)</code></pre>
	</div>
</details>
<details class="clean-accordion">
	<summary>GetPortals</summary>
	<div class="token-details">
		<strong>Signature</strong>
		<pre><code>public static List&lt;int&gt; GetPortals()</code></pre>
		<strong>Example</strong>
		<pre><code>GetPortals()</code></pre>
	</div>
</details>
<details class="clean-accordion">
	<summary>GetPortalSettings</summary>
	<div class="token-details">
		<strong>Signature</strong>
		<pre><code>public static PortalSettings GetPortalSettings()</code></pre>
		<strong>Example</strong>
		<pre><code>GetPortalSettings()</code></pre>
		<strong>Signature</strong>
		<pre><code>public static PortalSettings GetPortalSettings(int portalId)</code></pre>
		<strong>Example</strong>
		<pre><code>GetPortalSettings(portalId)</code></pre>
	</div>
</details>
<details class="clean-accordion">
	<summary>GetPortalThemeSkinSrc</summary>
	<div class="token-details">
		<strong>Signature</strong>
		<pre><code>public static string GetPortalThemeSkinSrc(int portalId)</code></pre>
		<strong>Example</strong>
		<pre><code>GetPortalThemeSkinSrc(portalId)</code></pre>
	</div>
</details>
<details class="clean-accordion">
	<summary>GetRootDomainUrl</summary>
	<div class="token-details">
		<strong>Signature</strong>
		<pre><code>public static string GetRootDomainUrl(int portalId = -1)</code></pre>
		<strong>Example</strong>
		<pre><code>GetRootDomainUrl(portalId)</code></pre>
	</div>
</details>
<details class="clean-accordion">
	<summary>GetUserRegistration</summary>
	<div class="token-details">
		<strong>Signature</strong>
		<pre><code>public static int GetUserRegistration(int portalId)</code></pre>
		<strong>Example</strong>
		<pre><code>GetUserRegistration(portalId)</code></pre>
	</div>
</details>
<details class="clean-accordion">
	<summary>HomeDirectoryMapPath</summary>
	<div class="token-details">
		<strong>Signature</strong>
		<pre><code>public static string HomeDirectoryMapPath(int portalId)</code></pre>
		<strong>Example</strong>
		<pre><code>HomeDirectoryMapPath(portalId)</code></pre>
	</div>
</details>
<details class="clean-accordion">
	<summary>HomeDirectoryRel</summary>
	<div class="token-details">
		<strong>Signature</strong>
		<pre><code>public static string HomeDirectoryRel(int portalId)</code></pre>
		<strong>Example</strong>
		<pre><code>HomeDirectoryRel(portalId)</code></pre>
	</div>
</details>
<details class="clean-accordion">
	<summary>HomeDNNrocketDirectoryMapPath</summary>
	<div class="token-details">
		<strong>Signature</strong>
		<pre><code>public static string HomeDNNrocketDirectoryMapPath(int portalId = -1)</code></pre>
		<strong>Example</strong>
		<pre><code>HomeDNNrocketDirectoryMapPath(portalId)</code></pre>
	</div>
</details>
<details class="clean-accordion">
	<summary>HomeDNNrocketDirectoryRel</summary>
	<div class="token-details">
		<strong>Signature</strong>
		<pre><code>public static string HomeDNNrocketDirectoryRel(int portalId = -1)</code></pre>
		<strong>Example</strong>
		<pre><code>HomeDNNrocketDirectoryRel(portalId)</code></pre>
	</div>
</details>
<details class="clean-accordion">
	<summary>LoginTabId</summary>
	<div class="token-details">
		<strong>Signature</strong>
		<pre><code>public static int LoginTabId(int portalId)</code></pre>
		<strong>Example</strong>
		<pre><code>LoginTabId(portalId)</code></pre>
		<strong>Signature</strong>
		<pre><code>public static int LoginTabId()</code></pre>
		<strong>Example</strong>
		<pre><code>LoginTabId()</code></pre>
	</div>
</details>
<details class="clean-accordion">
	<summary>PageHeadTextAppend</summary>
	<div class="token-details">
		<strong>Signature</strong>
		<pre><code>public static void PageHeadTextAppend(int portalId, string value)</code></pre>
		<strong>Example</strong>
		<pre><code>PageHeadTextAppend(portalId, value)</code></pre>
	</div>
</details>
<details class="clean-accordion">
	<summary>PageHeadTextUpdate</summary>
	<div class="token-details">
		<strong>Signature</strong>
		<pre><code>public static void PageHeadTextUpdate(int portalId, string value)</code></pre>
		<strong>Example</strong>
		<pre><code>PageHeadTextUpdate(portalId, value)</code></pre>
	</div>
</details>
<details class="clean-accordion">
	<summary>PortalExists</summary>
	<div class="token-details">
		<strong>Signature</strong>
		<pre><code>public static bool PortalExists(int portalId = -1)</code></pre>
		<strong>Example</strong>
		<pre><code>PortalExists(portalId)</code></pre>
	</div>
</details>
<details class="clean-accordion">
	<summary>Registration</summary>
	<div class="token-details">
		<p><strong>Description:</strong> Set portal Registration type.</p>
		<strong>Signature</strong>
		<pre><code>public static void Registration(int portalId, int regType)</code></pre>
		<strong>Example</strong>
		<pre><code>Registration(portalId, regType)</code></pre>
	</div>
</details>
<details class="clean-accordion">
	<summary>RemoveLanguage</summary>
	<div class="token-details">
		<strong>Signature</strong>
		<pre><code>public static void RemoveLanguage(int portalId, string cultureCode)</code></pre>
		<strong>Example</strong>
		<pre><code>RemoveLanguage(portalId, cultureCode)</code></pre>
	</div>
</details>
<details class="clean-accordion">
	<summary>RootDomain</summary>
	<div class="token-details">
		<strong>Signature</strong>
		<pre><code>public static string RootDomain(int portalId = -1)</code></pre>
		<strong>Example</strong>
		<pre><code>RootDomain(portalId)</code></pre>
	</div>
</details>
<details class="clean-accordion">
	<summary>SetDefaultLanguage</summary>
	<div class="token-details">
		<strong>Signature</strong>
		<pre><code>public static void SetDefaultLanguage(int portalId, string cultureCode)</code></pre>
		<strong>Example</strong>
		<pre><code>SetDefaultLanguage(portalId, cultureCode)</code></pre>
	</div>
</details>
<details class="clean-accordion">
	<summary>SetPrimaryPortalAlias</summary>
	<div class="token-details">
		<strong>Signature</strong>
		<pre><code>public static void SetPrimaryPortalAlias(int portalId, string portalAlias, bool isPrimary = true)</code></pre>
		<strong>Example</strong>
		<pre><code>SetPrimaryPortalAlias(portalId, portalAlias, isPrimary)</code></pre>
	</div>
</details>
<details class="clean-accordion">
	<summary>SetSearchTabId</summary>
	<div class="token-details">
		<strong>Signature</strong>
		<pre><code>public static void SetSearchTabId(int portalId, int tabId)</code></pre>
		<strong>Example</strong>
		<pre><code>SetSearchTabId(portalId, tabId)</code></pre>
	</div>
</details>
<details class="clean-accordion">
	<summary>SetUserRegistration</summary>
	<div class="token-details">
		<strong>Signature</strong>
		<pre><code>public static void SetUserRegistration(int portalId, int value)</code></pre>
		<strong>Example</strong>
		<pre><code>SetUserRegistration(portalId, value)</code></pre>
	</div>
</details>
<details class="clean-accordion">
	<summary>SSLSetup</summary>
	<div class="token-details">
		<strong>Signature</strong>
		<pre><code>public static void SSLSetup(int portalId, int value)</code></pre>
		<strong>Example</strong>
		<pre><code>SSLSetup(portalId, value)</code></pre>
	</div>
</details>
<details class="clean-accordion">
	<summary>TempDirectoryMapPath</summary>
	<div class="token-details">
		<strong>Signature</strong>
		<pre><code>public static string TempDirectoryMapPath(int portalId = -1)</code></pre>
		<strong>Example</strong>
		<pre><code>TempDirectoryMapPath(portalId)</code></pre>
	</div>
</details>
<details class="clean-accordion">
	<summary>TempDirectoryRel</summary>
	<div class="token-details">
		<strong>Signature</strong>
		<pre><code>public static string TempDirectoryRel(int portalId = -1)</code></pre>
		<strong>Example</strong>
		<pre><code>TempDirectoryRel(portalId)</code></pre>
	</div>
</details>
<details class="clean-accordion">
	<summary>UpdatePortalCopyright</summary>
	<div class="token-details">
		<p><strong>Description:</strong> Updates the copyright message (footer text) for a specific portal</p>
		<strong>Signature</strong>
		<pre><code>public static void UpdatePortalCopyright(int portalId, string copyrightMessage)</code></pre>
		<strong>Example</strong>
		<pre><code>UpdatePortalCopyright(portalId, copyrightMessage)</code></pre>
	</div>
</details>
<details class="clean-accordion">
	<summary>UpdatePortalSetting</summary>
	<div class="token-details">
		<strong>Signature</strong>
		<pre><code>public static void UpdatePortalSetting(int portalId, string settingName, string settingValue)</code></pre>
		<strong>Example</strong>
		<pre><code>UpdatePortalSetting(portalId, settingName, settingValue)</code></pre>
	</div>
</details>
<details class="clean-accordion">
	<summary>UseEmailAsUserName</summary>
	<div class="token-details">
		<strong>Signature</strong>
		<pre><code>public static void UseEmailAsUserName(int portalId, bool value)</code></pre>
		<strong>Example</strong>
		<pre><code>UseEmailAsUserName(portalId, value)</code></pre>
	</div>
</details>
