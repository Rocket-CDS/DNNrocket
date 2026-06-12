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
	<summary>GetCurrentUserId</summary>
	<div class="token-details">
		<p><strong>Description:</strong> Gets the ID of the currently logged-in user.</p>
		<strong>Signature</strong>
		<pre><code>public static int GetCurrentUserId()</code></pre>
	</div>
</details>
<details class="clean-accordion">
	<summary>RemoveUser</summary>
	<div class="token-details">
		<p><strong>Description:</strong> Permanently removes a user from the database (hard delete).</p>
		<strong>Signature</strong>
		<pre><code>public static void RemoveUser(int portalId, int userId, bool notify = false, bool deleteAdmin = false)</code></pre>
	</div>
</details>
<details class="clean-accordion">
	<summary>DeleteUser</summary>
	<div class="token-details">
		<p><strong>Description:</strong> Moves a user to the recycle bin (soft delete).</p>
		<strong>Signature</strong>
		<pre><code>public static void DeleteUser(int portalId, int userId, bool notify = false, bool deleteAdmin = false)</code></pre>
	</div>
</details>
<details class="clean-accordion">
	<summary>UnDeleteUser</summary>
	<div class="token-details">
		<p><strong>Description:</strong> Restores a user from the recycle bin.</p>
		<strong>Signature</strong>
		<pre><code>public static void UnDeleteUser(int portalId, int userId)</code></pre>
	</div>
</details>
<details class="clean-accordion">
	<summary>GetCurrentUserDisplayName</summary>
	<div class="token-details">
		<p><strong>Description:</strong> Gets the display name of the currently logged-in user.</p>
		<strong>Signature</strong>
		<pre><code>public static string GetCurrentUserDisplayName()</code></pre>
	</div>
</details>
<details class="clean-accordion">
	<summary>SetCurrentUserDisplayName</summary>
	<div class="token-details">
		<p><strong>Description:</strong> Sets the display name for the currently logged-in user.</p>
		<strong>Signature</strong>
		<pre><code>public static void SetCurrentUserDisplayName(string displayName)</code></pre>
	</div>
</details>
<details class="clean-accordion">
	<summary>GetCurrentUserName</summary>
	<div class="token-details">
		<p><strong>Description:</strong> Gets the username of the currently logged-in user.</p>
		<strong>Signature</strong>
		<pre><code>public static string GetCurrentUserName()</code></pre>
	</div>
</details>
<details class="clean-accordion">
	<summary>GetCurrentUserEmail</summary>
	<div class="token-details">
		<p><strong>Description:</strong> Gets the email address of the currently logged-in user.</p>
		<strong>Signature</strong>
		<pre><code>public static string GetCurrentUserEmail()</code></pre>
	</div>
</details>
<details class="clean-accordion">
	<summary>GetUserRoles</summary>
	<div class="token-details">
		<p><strong>Description:</strong> Gets a list of roles for a specified user or the current user.</p>
		<strong>Signature</strong>
		<pre><code>public static List&lt;string&gt; GetUserRoles(int portalid = -1, int userId = -1)</code></pre>
	</div>
</details>
<details class="clean-accordion">
	<summary>ResetPass</summary>
	<div class="token-details">
		<p><strong>Description:</strong> Initiates the password reset process for a user by their email address.</p>
		<strong>Signature</strong>
		<pre><code>public static bool ResetPass(string emailaddress, bool sendEmail)</code></pre>
	</div>
</details>
<details class="clean-accordion">
	<summary>ChangePassword</summary>
	<div class="token-details">
		<p><strong>Description:</strong> Changes the password for a specified user.</p>
		<strong>Signature</strong>
		<pre><code>public static bool ChangePassword(int portalId, int userId, string newPassword)</code></pre>
	</div>
</details>
<details class="clean-accordion">
	<summary>DoLogin</summary>
	<div class="token-details">
		<p><strong>Description:</strong> Logs a user in with the provided credentials.</p>
		<strong>Signature</strong>
		<pre><code>public static bool DoLogin(SessionParams sessionParams, string username, string password, bool rememberme)</code></pre>
	</div>
</details>
<details class="clean-accordion">
	<summary>UserLogin</summary>
	<div class="token-details">
		<p><strong>Description:</strong> Performs a user login action.</p>
		<strong>Signature</strong>
		<pre><code>public static bool UserLogin(string userHostAddress, string username, string password, bool rememberme)</code></pre>
	</div>
</details>
<details class="clean-accordion">
	<summary>UserLogin</summary>
	<div class="token-details">
		<p><strong>Description:</strong> Logs in a user programmatically without a password check, assuming they are already authenticated.</p>
		<strong>Signature</strong>
		<pre><code>public static void UserLogin(int portalId, string portalName, string userHostAddress, string username, bool rememberme)</code></pre>
	</div>
</details>
<details class="clean-accordion">
	<summary>DoLogin</summary>
	<div class="token-details">
		<p><strong>Description:</strong> Logs a user in using data from a generic login form (SimplisityInfo).</p>
		<strong>Signature</strong>
		<pre><code>public static bool DoLogin(SimplisityInfo postInfo, SimplisityInfo paramInfo)</code></pre>
	</div>
</details>
<details class="clean-accordion">
	<summary>LoginForm</summary>
	<div class="token-details">
		<p><strong>Description:</strong> Renders a login form using a Razor template.</p>
		<strong>Signature</strong>
		<pre><code>public static string LoginForm(string systemkey, SimplisityInfo sInfo, string interfacekey, int userid)</code></pre>
	</div>
</details>
<details class="clean-accordion">
	<summary>RegisterForm</summary>
	<div class="token-details">
		<p><strong>Description:</strong> Renders a user registration form using a Razor template.</p>
		<strong>Signature</strong>
		<pre><code>public static string RegisterForm(SimplisityInfo systemInfo, SimplisityInfo sInfo, string interfacekey, int userid)</code></pre>
	</div>
</details>
<details class="clean-accordion">
	<summary>RegisterUser</summary>
	<div class="token-details">
		<p><strong>Description:</strong> Registers a new user based on data from a SimplisityInfo object.</p>
		<strong>Signature</strong>
		<pre><code>public static string RegisterUser(SimplisityInfo sInfo, string currentCulture = "")</code></pre>
	</div>
</details>
<details class="clean-accordion">
	<summary>RegisterUser</summary>
	<div class="token-details">
		<p><strong>Description:</strong> Registers a new user with the specified details.</p>
		<strong>Signature</strong>
		<pre><code>public static string RegisterUser(string displayname, string username, string useremail, string password, string confirmpassword, string currentCulture, bool approved)</code></pre>
	</div>
</details>
<details class="clean-accordion">
	<summary>CreateUser</summary>
	<div class="token-details">
		<p><strong>Description:</strong> Creates a new user in the specified portal and optionally assigns them to a role.</p>
		<strong>Signature</strong>
		<pre><code>public static string CreateUser(int portalId, string username, string email, string roleName = "")</code></pre>
	</div>
</details>
<details class="clean-accordion">
	<summary>GetUserDataByUsername</summary>
	<div class="token-details">
		<p><strong>Description:</strong> Gets a UserData object for a user by their username.</p>
		<strong>Signature</strong>
		<pre><code>public static UserData GetUserDataByUsername(int portalId, string username)</code></pre>
	</div>
</details>
<details class="clean-accordion">
	<summary>GetUserDataByEmail</summary>
	<div class="token-details">
		<p><strong>Description:</strong> Gets a UserData object for a user by their email address.</p>
		<strong>Signature</strong>
		<pre><code>public static UserData GetUserDataByEmail(int portalId, string email)</code></pre>
	</div>
</details>
<details class="clean-accordion">
	<summary>GetUserData</summary>
	<div class="token-details">
		<p><strong>Description:</strong> Gets a UserData object containing detailed information about a user.</p>
		<strong>Signature</strong>
		<pre><code>public static UserData GetUserData(int portalId, int userId)</code></pre>
	</div>
</details>
<details class="clean-accordion">
	<summary>IsAuthorised</summary>
	<div class="token-details">
		<p><strong>Description:</strong> Checks if the current user is authorized (approved).</p>
		<strong>Signature</strong>
		<pre><code>public static bool IsAuthorised()</code></pre>
	</div>
</details>
<details class="clean-accordion">
	<summary>IsAuthorised</summary>
	<div class="token-details">
		<p><strong>Description:</strong> Checks if a specific user is authorized (approved).</p>
		<strong>Signature</strong>
		<pre><code>public static bool IsAuthorised(int portalId, int userId)</code></pre>
	</div>
</details>
<details class="clean-accordion">
	<summary>UnAuthoriseUser</summary>
	<div class="token-details">
		<p><strong>Description:</strong> Marks a user as not approved.</p>
		<strong>Signature</strong>
		<pre><code>public static void UnAuthoriseUser(int portalId, int userId)</code></pre>
	</div>
</details>
<details class="clean-accordion">
	<summary>AuthoriseUser</summary>
	<div class="token-details">
		<p><strong>Description:</strong> Marks a user as approved.</p>
		<strong>Signature</strong>
		<pre><code>public static void AuthoriseUser(int portalId, int userId)</code></pre>
	</div>
</details>
<details class="clean-accordion">
	<summary>UpdateEmail</summary>
	<div class="token-details">
		<p><strong>Description:</strong> Updates the email address for a user.</p>
		<strong>Signature</strong>
		<pre><code>public static UserData UpdateEmail(int portalId, int userId, string email)</code></pre>
	</div>
</details>
<details class="clean-accordion">
	<summary>GetUserProfileProperties</summary>
	<div class="token-details">
		<p><strong>Description:</strong> Gets a dictionary of all profile properties for a user.</p>
		<strong>Signature</strong>
		<pre><code>public static Dictionary&lt;string, string&gt; GetUserProfileProperties(int portalId, int userId)</code></pre>
	</div>
</details>
<details class="clean-accordion">
	<summary>GetUserProfileProperties</summary>
	<div class="token-details">
		<p><strong>Description:</strong> Gets a dictionary of all profile properties for a user by their user ID as a string.</p>
		<strong>Signature</strong>
		<pre><code>public static Dictionary&lt;string, string&gt; GetUserProfileProperties(string userId)</code></pre>
	</div>
</details>
<details class="clean-accordion">
	<summary>SetUserProfileProperties</summary>
	<div class="token-details">
		<p><strong>Description:</strong> Sets multiple profile properties for a user.</p>
		<strong>Signature</strong>
		<pre><code>public static void SetUserProfileProperties(String userId, Dictionary&lt;string, string&gt; properties)</code></pre>
	</div>
</details>
<details class="clean-accordion">
	<summary>SetUserProfileProperties</summary>
	<div class="token-details">
		<p><strong>Description:</strong> Sets multiple profile properties for a user.</p>
		<strong>Signature</strong>
		<pre><code>public static void SetUserProfileProperties(int portalId, int userId, Dictionary&lt;string, string&gt; properties)</code></pre>
	</div>
</details>
<details class="clean-accordion">
	<summary>GetUserIdByUserName</summary>
	<div class="token-details">
		<p><strong>Description:</strong> Gets a user's ID from their username.</p>
		<strong>Signature</strong>
		<pre><code>public static int GetUserIdByUserName(int portalId, string username)</code></pre>
	</div>
</details>
<details class="clean-accordion">
	<summary>GetUsers</summary>
	<div class="token-details">
		<p><strong>Description:</strong> Gets a list of users as SimplisityRecord objects, optionally filtered by role.</p>
		<strong>Signature</strong>
		<pre><code>public static List&lt;SimplisityRecord&gt; GetUsers(int portalId, string inRole = "")</code></pre>
	</div>
</details>
<details class="clean-accordion">
	<summary>GetUsers</summary>
	<div class="token-details">
		<p><strong>Description:</strong> Gets a paginated list of users as SimplisityRecord objects.</p>
		<strong>Signature</strong>
		<pre><code>public static List&lt;SimplisityRecord&gt; GetUsers(int portalId, int pageNumber, int pageSize, ref int totalRecord, bool includeDeleted = false, bool superUsersOnly = false)</code></pre>
	</div>
</details>
<details class="clean-accordion">
	<summary>GetUsers</summary>
	<div class="token-details">
		<p><strong>Description:</strong> Searches for users by a text string and returns a list of SimplisityRecord objects.</p>
		<strong>Signature</strong>
		<pre><code>public static List&lt;SimplisityRecord&gt; GetUsers(int portalId, string searchtext, int returnLimit = 100)</code></pre>
	</div>
</details>
<details class="clean-accordion">
	<summary>GetUsersUserData</summary>
	<div class="token-details">
		<p><strong>Description:</strong> Searches for users by a text string and returns a list of UserData objects.</p>
		<strong>Signature</strong>
		<pre><code>public static List&lt;UserData&gt; GetUsersUserData(int portalId, string searchtext, int returnLimit = 100)</code></pre>
	</div>
</details>
<details class="clean-accordion">
	<summary>GetSuperUsers</summary>
	<div class="token-details">
		<p><strong>Description:</strong> Gets a list of all super users as SimplisityRecord objects.</p>
		<strong>Signature</strong>
		<pre><code>public static List&lt;SimplisityRecord&gt; GetSuperUsers()</code></pre>
	</div>
</details>
<details class="clean-accordion">
	<summary>GetUserIdByEmail</summary>
	<div class="token-details">
		<p><strong>Description:</strong> Gets a user's ID from their email address.</p>
		<strong>Signature</strong>
		<pre><code>public static int GetUserIdByEmail(int portalId, string email)</code></pre>
	</div>
</details>
<details class="clean-accordion">
	<summary>IsInRole</summary>
	<div class="token-details">
		<p><strong>Description:</strong> Checks if a specific user is in a given role.</p>
		<strong>Signature</strong>
		<pre><code>public static bool IsInRole(int portalId, int userId, string role)</code></pre>
	</div>
</details>
<details class="clean-accordion">
	<summary>IsInRole</summary>
	<div class="token-details">
		<p><strong>Description:</strong> Checks if the current user is in a given role.</p>
		<strong>Signature</strong>
		<pre><code>public static bool IsInRole(string role)</code></pre>
	</div>
</details>
<details class="clean-accordion">
	<summary>IsSuperUser</summary>
	<div class="token-details">
		<p><strong>Description:</strong> Checks if a specific user is a super user.</p>
		<strong>Signature</strong>
		<pre><code>public static bool IsSuperUser(int portalId, int userId)</code></pre>
	</div>
</details>
<details class="clean-accordion">
	<summary>IsSuperUser</summary>
	<div class="token-details">
		<p><strong>Description:</strong> Checks if the current user is a super user.</p>
		<strong>Signature</strong>
		<pre><code>public static bool IsSuperUser()</code></pre>
	</div>
</details>
<details class="clean-accordion">
	<summary>IsClientOnly</summary>
	<div class="token-details">
		<p><strong>Description:</strong> Checks if the current user has only the 'Collaborator' role and no higher-level roles.</p>
		<strong>Signature</strong>
		<pre><code>public static Boolean IsClientOnly()</code></pre>
	</div>
</details>
<details class="clean-accordion">
	<summary>IsManager</summary>
	<div class="token-details">
		<p><strong>Description:</strong> Checks if the current user is in the 'Manager' or 'Administrators' role.</p>
		<strong>Signature</strong>
		<pre><code>public static Boolean IsManager()</code></pre>
	</div>
</details>
<details class="clean-accordion">
	<summary>IsEditor</summary>
	<div class="token-details">
		<p><strong>Description:</strong> Checks if the current user has at least 'Collaborator' level permissions.</p>
		<strong>Signature</strong>
		<pre><code>public static Boolean IsEditor()</code></pre>
	</div>
</details>
<details class="clean-accordion">
	<summary>IsAdministrator</summary>
	<div class="token-details">
		<p><strong>Description:</strong> Checks if the current user is in the 'Administrators' role.</p>
		<strong>Signature</strong>
		<pre><code>public static Boolean IsAdministrator()</code></pre>
	</div>
</details>
<details class="clean-accordion">
	<summary>IsAdministrator</summary>
	<div class="token-details">
		<p><strong>Description:</strong> Checks if a specific user is an administrator.</p>
		<strong>Signature</strong>
		<pre><code>public static bool IsAdministrator(int portalId, int userId)</code></pre>
	</div>
</details>
<details class="clean-accordion">
	<summary>GetValidUser</summary>
	<div class="token-details">
		<p><strong>Description:</strong> Validates a user's credentials and returns the UserInfo object if valid.</p>
		<strong>Signature</strong>
		<pre><code>public static UserInfo GetValidUser(int PortalId, string username, string password)</code></pre>
	</div>
</details>
<details class="clean-accordion">
	<summary>IsValidUser</summary>
	<div class="token-details">
		<p><strong>Description:</strong> Checks if a user exists by username.</p>
		<strong>Signature</strong>
		<pre><code>public static bool IsValidUser(int PortalId, string username)</code></pre>
	</div>
</details>
<details class="clean-accordion">
	<summary>IsValidUser</summary>
	<div class="token-details">
		<p><strong>Description:</strong> Checks if a user's credentials are valid.</p>
		<strong>Signature</strong>
		<pre><code>public static bool IsValidUser(int PortalId, string username, string password)</code></pre>
	</div>
</details>
<details class="clean-accordion">
	<summary>IsValidUser</summary>
	<div class="token-details">
		<p><strong>Description:</strong> Checks if a user exists by user ID.</p>
		<strong>Signature</strong>
		<pre><code>public static bool IsValidUser(int portalId, int userId)</code></pre>
	</div>
</details>
<details class="clean-accordion">
	<summary>GetRoleById</summary>
	<div class="token-details">
		<p><strong>Description:</strong> Gets role information as a SimplisityRecord by role ID.</p>
		<strong>Signature</strong>
		<pre><code>public static SimplisityRecord GetRoleById(int portalId, int roleId)</code></pre>
	</div>
</details>
<details class="clean-accordion">
	<summary>GetRoles</summary>
	<div class="token-details">
		<p><strong>Description:</strong> Gets a dictionary of all roles in a portal.</p>
		<strong>Signature</strong>
		<pre><code>public static Dictionary&lt;int, string&gt; GetRoles(int portalId)</code></pre>
	</div>
</details>
<details class="clean-accordion">
	<summary>GetRoleByName</summary>
	<div class="token-details">
		<p><strong>Description:</strong> Gets role information as a SimplisityRecord by role name.</p>
		<strong>Signature</strong>
		<pre><code>public static SimplisityRecord GetRoleByName(int portalId, string roleName)</code></pre>
	</div>
</details>
<details class="clean-accordion">
	<summary>AddUserRole</summary>
	<div class="token-details">
		<p><strong>Description:</strong> Adds a user to a role.</p>
		<strong>Signature</strong>
		<pre><code>public static void AddUserRole(int portalId, int userId, int roleId)</code></pre>
	</div>
</details>
<details class="clean-accordion">
	<summary>AddUserRole</summary>
	<div class="token-details">
		<p><strong>Description:</strong> Adds a user to a role and optionally notifies the user.</p>
		<strong>Signature</strong>
		<pre><code>public static void AddUserRole(int portalId, int userId, int roleId, bool notifyUser)</code></pre>
	</div>
</details>
<details class="clean-accordion">
	<summary>RemoveUserRole</summary>
	<div class="token-details">
		<p><strong>Description:</strong> Removes a user from a role.</p>
		<strong>Signature</strong>
		<pre><code>public static void RemoveUserRole(int portalId, int userId, int roleId)</code></pre>
	</div>
</details>
<details class="clean-accordion">
	<summary>SignOut</summary>
	<div class="token-details">
		<p><strong>Description:</strong> Signs the current user out.</p>
		<strong>Signature</strong>
		<pre><code>public static void SignOut()</code></pre>
	</div>
</details>
<details class="clean-accordion">
	<summary>HasModuleAccess</summary>
	<div class="token-details">
		<p><strong>Description:</strong> Checks if a user has a specific level of access to a module.</p>
		<strong>Signature</strong>
		<pre><code>public static bool HasModuleAccess(int portalId, int userId, int moduleId, string permissionKey = "VIEW")</code></pre>
	</div>
</details>
<details class="clean-accordion">
	<summary>UpdateOrCreateUserProfileProperty</summary>
	<div class="token-details">
		<p><strong>Description:</strong> Updates or creates a DNN user profile property. If the property definition doesn't exist, it will be created.</p>
		<strong>Signature</strong>
		<pre><code>public static bool UpdateOrCreateUserProfileProperty(int userId, string key, string value, int portalId = 0, string category = "Custom", bool visibilityAllUsers = false)</code></pre>
	</div>
</details>
