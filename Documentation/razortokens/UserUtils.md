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
	<summary>AddUserRole</summary>
	<div class="token-details">
		<strong>Signature</strong>
		<pre><code>public static void AddUserRole(int portalId, int userId, int roleId)</code></pre>
		<strong>Example</strong>
		<pre><code>AddUserRole(portalId, userId, roleId)</code></pre>
		<strong>Signature</strong>
		<pre><code>public static void AddUserRole(int portalId, int userId, int roleId, bool notifyUser)</code></pre>
		<strong>Example</strong>
		<pre><code>AddUserRole(portalId, userId, roleId, notifyUser)</code></pre>
	</div>
</details>
<details class="clean-accordion">
	<summary>AuthoriseUser</summary>
	<div class="token-details">
		<strong>Signature</strong>
		<pre><code>public static void AuthoriseUser(int portalId, int userId)</code></pre>
		<strong>Example</strong>
		<pre><code>AuthoriseUser(portalId, userId)</code></pre>
	</div>
</details>
<details class="clean-accordion">
	<summary>ChangePassword</summary>
	<div class="token-details">
		<strong>Signature</strong>
		<pre><code>public static bool ChangePassword(int portalId, int userId, string newPassword)</code></pre>
		<strong>Example</strong>
		<pre><code>ChangePassword(portalId, userId, newPassword)</code></pre>
	</div>
</details>
<details class="clean-accordion">
	<summary>ChangePasswordAction</summary>
	<div class="token-details">
		<p><strong>Description:</strong> 0 = OK 1 = password length fail 2 = Validate minimum non-alphanumeric characters fail 3 = Validate password strength regex fail 4 = general fail</p>
		<strong>Signature</strong>
		<pre><code>public static int ChangePasswordAction(int portalId, int userId, string newPassword)</code></pre>
		<strong>Example</strong>
		<pre><code>ChangePasswordAction(portalId, userId, newPassword)</code></pre>
	</div>
</details>
<details class="clean-accordion">
	<summary>CreateUser</summary>
	<div class="token-details">
		<strong>Signature</strong>
		<pre><code>public static string CreateUser(int portalId, string username, string email, string roleName = &quot;&quot;)</code></pre>
		<strong>Example</strong>
		<pre><code>CreateUser(portalId, username, email, roleName)</code></pre>
	</div>
</details>
<details class="clean-accordion">
	<summary>DeleteUser</summary>
	<div class="token-details">
		<p><strong>Description:</strong> Move user to recycle bin.</p>
		<strong>Signature</strong>
		<pre><code>public static void DeleteUser(int portalId, int userId, bool notify = false, bool deleteAdmin = false)</code></pre>
		<strong>Example</strong>
		<pre><code>DeleteUser(portalId, userId, notify, deleteAdmin)</code></pre>
	</div>
</details>
<details class="clean-accordion">
	<summary>DoLogin</summary>
	<div class="token-details">
		<p><strong>Description:</strong> Do login using gereric login form data. xpath must use...  var username = postInfo.GetXmlProperty(&quot;genxml/text/username&quot;); var password = postInfo.GetXmlProperty(&quot;genxml/hidden/password&quot;); var rememberme = postInfo.GetXmlPropertyBool(&quot;genxml/checkbox/rememberme&quot;);</p>
		<strong>Signature</strong>
		<pre><code>public static bool DoLogin(SessionParams sessionParams, string username, string password, bool rememberme)</code></pre>
		<strong>Example</strong>
		<pre><code>DoLogin(sessionParams, username, password, rememberme)</code></pre>
		<strong>Signature</strong>
		<pre><code>public static bool DoLogin(SimplisityInfo postInfo, SimplisityInfo paramInfo)</code></pre>
		<strong>Example</strong>
		<pre><code>DoLogin(postInfo, paramInfo)</code></pre>
	</div>
</details>
<details class="clean-accordion">
	<summary>GetCurrentUserDisplayName</summary>
	<div class="token-details">
		<strong>Signature</strong>
		<pre><code>public static string GetCurrentUserDisplayName()</code></pre>
		<strong>Example</strong>
		<pre><code>GetCurrentUserDisplayName()</code></pre>
	</div>
</details>
<details class="clean-accordion">
	<summary>GetCurrentUserEmail</summary>
	<div class="token-details">
		<strong>Signature</strong>
		<pre><code>public static string GetCurrentUserEmail()</code></pre>
		<strong>Example</strong>
		<pre><code>GetCurrentUserEmail()</code></pre>
	</div>
</details>
<details class="clean-accordion">
	<summary>GetCurrentUserId</summary>
	<div class="token-details">
		<strong>Signature</strong>
		<pre><code>public static int GetCurrentUserId()</code></pre>
		<strong>Example</strong>
		<pre><code>GetCurrentUserId()</code></pre>
	</div>
</details>
<details class="clean-accordion">
	<summary>GetCurrentUserName</summary>
	<div class="token-details">
		<strong>Signature</strong>
		<pre><code>public static string GetCurrentUserName()</code></pre>
		<strong>Example</strong>
		<pre><code>GetCurrentUserName()</code></pre>
	</div>
</details>
<details class="clean-accordion">
	<summary>GetRoleById</summary>
	<div class="token-details">
		<strong>Signature</strong>
		<pre><code>public static SimplisityRecord GetRoleById(int portalId, int roleId)</code></pre>
		<strong>Example</strong>
		<pre><code>GetRoleById(portalId, roleId)</code></pre>
	</div>
</details>
<details class="clean-accordion">
	<summary>GetRoleByName</summary>
	<div class="token-details">
		<strong>Signature</strong>
		<pre><code>public static SimplisityRecord GetRoleByName(int portalId, string roleName)</code></pre>
		<strong>Example</strong>
		<pre><code>GetRoleByName(portalId, roleName)</code></pre>
	</div>
</details>
<details class="clean-accordion">
	<summary>GetRoles</summary>
	<div class="token-details">
		<strong>Signature</strong>
		<pre><code>public static Dictionary&lt;int, string&gt; GetRoles(int portalId)</code></pre>
		<strong>Example</strong>
		<pre><code>GetRoles(portalId)</code></pre>
	</div>
</details>
<details class="clean-accordion">
	<summary>GetSuperUsers</summary>
	<div class="token-details">
		<strong>Signature</strong>
		<pre><code>public static List&lt;SimplisityRecord&gt; GetSuperUsers()</code></pre>
		<strong>Example</strong>
		<pre><code>GetSuperUsers()</code></pre>
	</div>
</details>
<details class="clean-accordion">
	<summary>GetUserData</summary>
	<div class="token-details">
		<strong>Signature</strong>
		<pre><code>public static UserData GetUserData(int portalId, int userId)</code></pre>
		<strong>Example</strong>
		<pre><code>GetUserData(portalId, userId)</code></pre>
	</div>
</details>
<details class="clean-accordion">
	<summary>GetUserDataByEmail</summary>
	<div class="token-details">
		<strong>Signature</strong>
		<pre><code>public static UserData GetUserDataByEmail(int portalId, string email)</code></pre>
		<strong>Example</strong>
		<pre><code>GetUserDataByEmail(portalId, email)</code></pre>
	</div>
</details>
<details class="clean-accordion">
	<summary>GetUserDataByUsername</summary>
	<div class="token-details">
		<strong>Signature</strong>
		<pre><code>public static UserData GetUserDataByUsername(int portalId, string username)</code></pre>
		<strong>Example</strong>
		<pre><code>GetUserDataByUsername(portalId, username)</code></pre>
	</div>
</details>
<details class="clean-accordion">
	<summary>GetUserIdByEmail</summary>
	<div class="token-details">
		<strong>Signature</strong>
		<pre><code>public static int GetUserIdByEmail(int portalId, string email)</code></pre>
		<strong>Example</strong>
		<pre><code>GetUserIdByEmail(portalId, email)</code></pre>
	</div>
</details>
<details class="clean-accordion">
	<summary>GetUserIdByUserName</summary>
	<div class="token-details">
		<strong>Signature</strong>
		<pre><code>public static int GetUserIdByUserName(int portalId, string username)</code></pre>
		<strong>Example</strong>
		<pre><code>GetUserIdByUserName(portalId, username)</code></pre>
	</div>
</details>
<details class="clean-accordion">
	<summary>GetUserProfileListField</summary>
	<div class="token-details">
		<p><strong>Description:</strong> Returns the key/value pairs of a DNN profile list field (e.g. a dropdown such as &quot;Location&quot;). The list entries are looked up from the DNN Lists table using the profile property&#39;s list name.</p>
		<strong>Signature</strong>
		<pre><code>public static Dictionary&lt;string, string&gt; GetUserProfileListField(int portalId, string fieldName)</code></pre>
		<strong>Example</strong>
		<pre><code>GetUserProfileListField(portalId, fieldName)</code></pre>
	</div>
</details>
<details class="clean-accordion">
	<summary>GetUserProfileProperties</summary>
	<div class="token-details">
		<strong>Signature</strong>
		<pre><code>public static Dictionary&lt;string, string&gt; GetUserProfileProperties(int portalId, int userId)</code></pre>
		<strong>Example</strong>
		<pre><code>GetUserProfileProperties(portalId, userId)</code></pre>
		<strong>Signature</strong>
		<pre><code>public static Dictionary&lt;string, string&gt; GetUserProfileProperties(string userId)</code></pre>
		<strong>Example</strong>
		<pre><code>GetUserProfileProperties(userId)</code></pre>
	</div>
</details>
<details class="clean-accordion">
	<summary>GetUserRoles</summary>
	<div class="token-details">
		<strong>Signature</strong>
		<pre><code>public static List&lt;string&gt; GetUserRoles(int portalid = -1, int userId = -1)</code></pre>
		<strong>Example</strong>
		<pre><code>GetUserRoles(portalid, userId)</code></pre>
	</div>
</details>
<details class="clean-accordion">
	<summary>GetUsers</summary>
	<div class="token-details">
		<p><strong>Description:</strong> Return list of users in XML SimplisityRecord.  XML Format &quot;user/email user/username user/userid user/displayname&quot;</p>
		<strong>Signature</strong>
		<pre><code>public static List&lt;SimplisityRecord&gt; GetUsers(int portalId, string inRole = &quot;&quot;)</code></pre>
		<strong>Example</strong>
		<pre><code>GetUsers(portalId, inRole)</code></pre>
		<strong>Signature</strong>
		<pre><code>public static List&lt;SimplisityRecord&gt; GetUsers(int portalId, int pageNumber, int pageSize, ref int totalRecord, bool includeDeleted = false, bool superUsersOnly = false)</code></pre>
		<strong>Example</strong>
		<pre><code>GetUsers(portalId, pageNumber, pageSize, totalRecord, includeDeleted, superUsersOnly)</code></pre>
		<strong>Signature</strong>
		<pre><code>public static List&lt;SimplisityRecord&gt; GetUsers(int portalId, string searchtext, int returnLimit = 100)</code></pre>
		<strong>Example</strong>
		<pre><code>GetUsers(portalId, searchtext, returnLimit)</code></pre>
	</div>
</details>
<details class="clean-accordion">
	<summary>GetUsersUserData</summary>
	<div class="token-details">
		<strong>Signature</strong>
		<pre><code>public static List&lt;UserData&gt; GetUsersUserData(int portalId, string searchtext, int returnLimit = 100)</code></pre>
		<strong>Example</strong>
		<pre><code>GetUsersUserData(portalId, searchtext, returnLimit)</code></pre>
	</div>
</details>
<details class="clean-accordion">
	<summary>GetValidUser</summary>
	<div class="token-details">
		<strong>Signature</strong>
		<pre><code>public static UserInfo GetValidUser(int PortalId, string username, string password)</code></pre>
		<strong>Example</strong>
		<pre><code>GetValidUser(PortalId, username, password)</code></pre>
	</div>
</details>
<details class="clean-accordion">
	<summary>HasModuleAccess</summary>
	<div class="token-details">
		<p><strong>Description:</strong> Check if a user has access to a specific module</p>
		<strong>Signature</strong>
		<pre><code>public static bool HasModuleAccess(int portalId, int userId, int moduleId, string permissionKey = &quot;VIEW&quot;)</code></pre>
		<strong>Example</strong>
		<pre><code>HasModuleAccess(portalId, userId, moduleId, permissionKey)</code></pre>
	</div>
</details>
<details class="clean-accordion">
	<summary>IsAdministrator</summary>
	<div class="token-details">
		<strong>Signature</strong>
		<pre><code>public static Boolean IsAdministrator()</code></pre>
		<strong>Example</strong>
		<pre><code>IsAdministrator()</code></pre>
		<strong>Signature</strong>
		<pre><code>public static bool IsAdministrator(int portalId, int userId)</code></pre>
		<strong>Example</strong>
		<pre><code>IsAdministrator(portalId, userId)</code></pre>
	</div>
</details>
<details class="clean-accordion">
	<summary>IsAuthorised</summary>
	<div class="token-details">
		<strong>Signature</strong>
		<pre><code>public static bool IsAuthorised()</code></pre>
		<strong>Example</strong>
		<pre><code>IsAuthorised()</code></pre>
		<strong>Signature</strong>
		<pre><code>public static bool IsAuthorised(int portalId, int userId)</code></pre>
		<strong>Example</strong>
		<pre><code>IsAuthorised(portalId, userId)</code></pre>
	</div>
</details>
<details class="clean-accordion">
	<summary>IsClientOnly</summary>
	<div class="token-details">
		<strong>Signature</strong>
		<pre><code>public static Boolean IsClientOnly()</code></pre>
		<strong>Example</strong>
		<pre><code>IsClientOnly()</code></pre>
	</div>
</details>
<details class="clean-accordion">
	<summary>IsEditor</summary>
	<div class="token-details">
		<strong>Signature</strong>
		<pre><code>public static Boolean IsEditor()</code></pre>
		<strong>Example</strong>
		<pre><code>IsEditor()</code></pre>
	</div>
</details>
<details class="clean-accordion">
	<summary>IsInRole</summary>
	<div class="token-details">
		<strong>Signature</strong>
		<pre><code>public static bool IsInRole(int portalId, int userId, string role)</code></pre>
		<strong>Example</strong>
		<pre><code>IsInRole(portalId, userId, role)</code></pre>
		<strong>Signature</strong>
		<pre><code>public static bool IsInRole(string role)</code></pre>
		<strong>Example</strong>
		<pre><code>IsInRole(role)</code></pre>
	</div>
</details>
<details class="clean-accordion">
	<summary>IsManager</summary>
	<div class="token-details">
		<strong>Signature</strong>
		<pre><code>public static Boolean IsManager()</code></pre>
		<strong>Example</strong>
		<pre><code>IsManager()</code></pre>
	</div>
</details>
<details class="clean-accordion">
	<summary>IsSuperUser</summary>
	<div class="token-details">
		<strong>Signature</strong>
		<pre><code>public static bool IsSuperUser(int portalId, int userId)</code></pre>
		<strong>Example</strong>
		<pre><code>IsSuperUser(portalId, userId)</code></pre>
		<strong>Signature</strong>
		<pre><code>public static bool IsSuperUser()</code></pre>
		<strong>Example</strong>
		<pre><code>IsSuperUser()</code></pre>
	</div>
</details>
<details class="clean-accordion">
	<summary>IsValidUser</summary>
	<div class="token-details">
		<strong>Signature</strong>
		<pre><code>public static bool IsValidUser(int PortalId, string username)</code></pre>
		<strong>Example</strong>
		<pre><code>IsValidUser(PortalId, username)</code></pre>
		<strong>Signature</strong>
		<pre><code>public static bool IsValidUser(int PortalId, string username, string password)</code></pre>
		<strong>Example</strong>
		<pre><code>IsValidUser(PortalId, username, password)</code></pre>
		<strong>Signature</strong>
		<pre><code>public static bool IsValidUser(int portalId, int userId)</code></pre>
		<strong>Example</strong>
		<pre><code>IsValidUser(portalId, userId)</code></pre>
	</div>
</details>
<details class="clean-accordion">
	<summary>LoginForm</summary>
	<div class="token-details">
		<strong>Signature</strong>
		<pre><code>public static string LoginForm(string systemkey, SimplisityInfo sInfo, string interfacekey, int userid)</code></pre>
		<strong>Example</strong>
		<pre><code>LoginForm(systemkey, sInfo, interfacekey, userid)</code></pre>
	</div>
</details>
<details class="clean-accordion">
	<summary>RegisterForm</summary>
	<div class="token-details">
		<strong>Signature</strong>
		<pre><code>public static string RegisterForm(SimplisityInfo systemInfo, SimplisityInfo sInfo, string interfacekey, int userid)</code></pre>
		<strong>Example</strong>
		<pre><code>RegisterForm(systemInfo, sInfo, interfacekey, userid)</code></pre>
	</div>
</details>
<details class="clean-accordion">
	<summary>RegisterUser</summary>
	<div class="token-details">
		<strong>Signature</strong>
		<pre><code>public static string RegisterUser(SimplisityInfo sInfo, string currentCulture = &quot;&quot;)</code></pre>
		<strong>Example</strong>
		<pre><code>RegisterUser(sInfo, currentCulture)</code></pre>
		<strong>Signature</strong>
		<pre><code>public static string RegisterUser(string displayname, string username, string useremail, string password, string confirmpassword, string currentCulture, bool approved)</code></pre>
		<strong>Example</strong>
		<pre><code>RegisterUser(displayname, username, useremail, password, confirmpassword, currentCulture, approved)</code></pre>
	</div>
</details>
<details class="clean-accordion">
	<summary>RemoveUser</summary>
	<div class="token-details">
		<p><strong>Description:</strong> Remove user from recyclebin (hard delete)</p>
		<strong>Signature</strong>
		<pre><code>public static void RemoveUser(int portalId, int userId, bool notify = false, bool deleteAdmin = false)</code></pre>
		<strong>Example</strong>
		<pre><code>RemoveUser(portalId, userId, notify, deleteAdmin)</code></pre>
	</div>
</details>
<details class="clean-accordion">
	<summary>RemoveUserRole</summary>
	<div class="token-details">
		<strong>Signature</strong>
		<pre><code>public static void RemoveUserRole(int portalId, int userId, int roleId)</code></pre>
		<strong>Example</strong>
		<pre><code>RemoveUserRole(portalId, userId, roleId)</code></pre>
	</div>
</details>
<details class="clean-accordion">
	<summary>ResetPass</summary>
	<div class="token-details">
		<strong>Signature</strong>
		<pre><code>public static bool ResetPass(string emailaddress, bool sendEmail)</code></pre>
		<strong>Example</strong>
		<pre><code>ResetPass(emailaddress, sendEmail)</code></pre>
	</div>
</details>
<details class="clean-accordion">
	<summary>SetCurrentUserDisplayName</summary>
	<div class="token-details">
		<strong>Signature</strong>
		<pre><code>public static void SetCurrentUserDisplayName(string displayName)</code></pre>
		<strong>Example</strong>
		<pre><code>SetCurrentUserDisplayName(displayName)</code></pre>
	</div>
</details>
<details class="clean-accordion">
	<summary>SetRemovalRequest</summary>
	<div class="token-details">
		<strong>Signature</strong>
		<pre><code>public static void SetRemovalRequest(int portalId, int userId, bool remove = true)</code></pre>
		<strong>Example</strong>
		<pre><code>SetRemovalRequest(portalId, userId, remove)</code></pre>
	</div>
</details>
<details class="clean-accordion">
	<summary>SetUserProfileProperties</summary>
	<div class="token-details">
		<strong>Signature</strong>
		<pre><code>public static void SetUserProfileProperties(String userId, Dictionary&lt;string, string&gt; properties)</code></pre>
		<strong>Example</strong>
		<pre><code>SetUserProfileProperties(userId, properties)</code></pre>
		<strong>Signature</strong>
		<pre><code>public static void SetUserProfileProperties(int portalId, int userId, Dictionary&lt;string, string&gt; properties)</code></pre>
		<strong>Example</strong>
		<pre><code>SetUserProfileProperties(portalId, userId, properties)</code></pre>
	</div>
</details>
<details class="clean-accordion">
	<summary>SignOut</summary>
	<div class="token-details">
		<strong>Signature</strong>
		<pre><code>public static void SignOut()</code></pre>
		<strong>Example</strong>
		<pre><code>SignOut()</code></pre>
	</div>
</details>
<details class="clean-accordion">
	<summary>UnAuthoriseUser</summary>
	<div class="token-details">
		<strong>Signature</strong>
		<pre><code>public static void UnAuthoriseUser(int portalId, int userId)</code></pre>
		<strong>Example</strong>
		<pre><code>UnAuthoriseUser(portalId, userId)</code></pre>
	</div>
</details>
<details class="clean-accordion">
	<summary>UnDeleteUser</summary>
	<div class="token-details">
		<strong>Signature</strong>
		<pre><code>public static void UnDeleteUser(int portalId, int userId)</code></pre>
		<strong>Example</strong>
		<pre><code>UnDeleteUser(portalId, userId)</code></pre>
	</div>
</details>
<details class="clean-accordion">
	<summary>UnlockUser</summary>
	<div class="token-details">
		<p><strong>Description:</strong> Unlocks a user account that has been locked out by DNN&#39;s membership provider (e.g. after too many failed login attempts). Also clears any pending &quot;update password&quot; requirement so the user can log in normally again.</p>
		<strong>Signature</strong>
		<pre><code>public static bool UnlockUser(int portalId, int userId)</code></pre>
		<strong>Example</strong>
		<pre><code>UnlockUser(portalId, userId)</code></pre>
		<strong>Signature</strong>
		<pre><code>public static bool UnlockUser(int portalId, string usernameOrEmail)</code></pre>
		<strong>Example</strong>
		<pre><code>UnlockUser(portalId, usernameOrEmail)</code></pre>
	</div>
</details>
<details class="clean-accordion">
	<summary>UpdateEmail</summary>
	<div class="token-details">
		<p><strong>Description:</strong> Updates the user&#39;s email address and, if the portal is configured to use email as username, also updates the username to match the new email.</p>
		<strong>Signature</strong>
		<pre><code>public static UserData UpdateEmail(int portalId, int userId, string newEmail)</code></pre>
		<strong>Example</strong>
		<pre><code>UpdateEmail(portalId, userId, newEmail)</code></pre>
	</div>
</details>
<details class="clean-accordion">
	<summary>UpdateOrCreateUserProfileProperty</summary>
	<div class="token-details">
		<p><strong>Description:</strong> Updates or creates a DNN user profile property for a user in the specified portal. If the property definition doesn&#39;t exist in the portal, it will be created.</p>
		<strong>Signature</strong>
		<pre><code>public static bool UpdateOrCreateUserProfileProperty(int userId, string key, string value, int portalId = 0, string category = &quot;Custom&quot;, bool visibilityAllUsers = false)</code></pre>
		<strong>Example</strong>
		<pre><code>UpdateOrCreateUserProfileProperty(userId, key, value, portalId, category, visibilityAllUsers)</code></pre>
	</div>
</details>
<details class="clean-accordion">
	<summary>UserLogin</summary>
	<div class="token-details">
		<p><strong>Description:</strong> Validates and logs in a user, using an explicit portalId rather than ambient PortalSettings.Current. This ensures the correct portal-level lockout/security settings (MaxInvalidPasswordAttempts, etc.) are enforced, which is critical in AJAX/API request pipelines where PortalSettings.Current may not be reliably populated.</p>
		<strong>Signature</strong>
		<pre><code>public static bool UserLogin(int portalId, string userHostAddress, string username, string password, bool rememberme)</code></pre>
		<strong>Example</strong>
		<pre><code>UserLogin(portalId, userHostAddress, username, password, rememberme)</code></pre>
	</div>
</details>
<details class="clean-accordion">
	<summary>UserLoginNoPassword</summary>
	<div class="token-details">
		<p><strong>Description:</strong> Logs a user in WITHOUT password verification. Used for trust-based flows (e.g. SSO token consumption, auto-login) where identity has already been established by another mechanism (signed token, etc).</p>
		<strong>Signature</strong>
		<pre><code>public static void UserLoginNoPassword(int portalId, string portalName, string userHostAddress, string username, bool rememberme)</code></pre>
		<strong>Example</strong>
		<pre><code>UserLoginNoPassword(portalId, portalName, userHostAddress, username, rememberme)</code></pre>
	</div>
</details>
