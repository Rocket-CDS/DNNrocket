
# GetCurrentUserId
**Description**: Gets the ID of the currently logged-in user.
**Signature**
```csharp
public static int GetCurrentUserId()
```
***
# RemoveUser
**Description**: Permanently removes a user from the database (hard delete).
**Signature**
```csharp
public static void RemoveUser(int portalId, int userId, bool notify = false, bool deleteAdmin = false)
```
***
# DeleteUser
**Description**: Moves a user to the recycle bin (soft delete).
**Signature**
```csharp
public static void DeleteUser(int portalId, int userId, bool notify = false, bool deleteAdmin = false)
```
***
# UnDeleteUser
**Description**: Restores a user from the recycle bin.
**Signature**
```csharp
public static void UnDeleteUser(int portalId, int userId)
```
***
# GetCurrentUserDisplayName
**Description**: Gets the display name of the currently logged-in user.
**Signature**
```csharp
public static string GetCurrentUserDisplayName()
```
***
# SetCurrentUserDisplayName
**Description**: Sets the display name for the currently logged-in user.
**Signature**
```csharp
public static void SetCurrentUserDisplayName(string displayName)
```
***
# GetCurrentUserName
**Description**: Gets the username of the currently logged-in user.
**Signature**
```csharp
public static string GetCurrentUserName()
```
***
# GetCurrentUserEmail
**Description**: Gets the email address of the currently logged-in user.
**Signature**
```csharp
public static string GetCurrentUserEmail()
```
***
# GetUserRoles
**Description**: Gets a list of roles for a specified user or the current user.
**Signature**
```csharp
public static List<string> GetUserRoles(int portalid = -1, int userId = -1)
```
***
# ResetPass
**Description**: Initiates the password reset process for a user by their email address.
**Signature**
```csharp
public static bool ResetPass(string emailaddress, bool sendEmail)
```
***
# ChangePassword
**Description**: Changes the password for a specified user.
**Signature**
```csharp
public static bool ChangePassword(int portalId, int userId, string newPassword)
```
***
# DoLogin
**Description**: Logs a user in with the provided credentials.
**Signature**
```csharp
public static bool DoLogin(SessionParams sessionParams, string username, string password, bool rememberme)
```
***
# UserLogin
**Description**: Performs a user login action.
**Signature**
```csharp
public static bool UserLogin(string userHostAddress, string username, string password, bool rememberme)
```
***
# UserLogin
**Description**: Logs in a user programmatically without a password check, assuming they are already authenticated.
**Signature**
```csharp
public static void UserLogin(int portalId, string portalName, string userHostAddress, string username, bool rememberme)
```
***
# DoLogin
**Description**: Logs a user in using data from a generic login form (SimplisityInfo).
**Signature**
```csharp
public static bool DoLogin(SimplisityInfo postInfo, SimplisityInfo paramInfo)
```
***
# LoginForm
**Description**: Renders a login form using a Razor template.
**Signature**
```csharp
public static string LoginForm(string systemkey, SimplisityInfo sInfo, string interfacekey, int userid)
```
***
# RegisterForm
**Description**: Renders a user registration form using a Razor template.
**Signature**
```csharp
public static string RegisterForm(SimplisityInfo systemInfo, SimplisityInfo sInfo, string interfacekey, int userid)
```
***
# RegisterUser
**Description**: Registers a new user based on data from a SimplisityInfo object.
**Signature**
```csharp
public static string RegisterUser(SimplisityInfo sInfo, string currentCulture = "")
```
***
# RegisterUser
**Description**: Registers a new user with the specified details.
**Signature**
```csharp
public static string RegisterUser(string displayname, string username, string useremail, string password, string confirmpassword, string currentCulture, bool approved)
```
***
# CreateUser
**Description**: Creates a new user in the specified portal and optionally assigns them to a role.
**Signature**
```csharp
public static string CreateUser(int portalId, string username, string email, string roleName = "")
```
***
# GetUserDataByUsername
**Description**: Gets a UserData object for a user by their username.
**Signature**
```csharp
public static UserData GetUserDataByUsername(int portalId, string username)
```
***
# GetUserDataByEmail
**Description**: Gets a UserData object for a user by their email address.
**Signature**
```csharp
public static UserData GetUserDataByEmail(int portalId, string email)
```
***
# GetUserData
**Description**: Gets a UserData object containing detailed information about a user.
**Signature**
```csharp
public static UserData GetUserData(int portalId, int userId)
```
***
# IsAuthorised
**Description**: Checks if the current user is authorized (approved).
**Signature**
```csharp
public static bool IsAuthorised()
```
***
# IsAuthorised
**Description**: Checks if a specific user is authorized (approved).
**Signature**
```csharp
public static bool IsAuthorised(int portalId, int userId)
```
***
# UnAuthoriseUser
**Description**: Marks a user as not approved.
**Signature**
```csharp
public static void UnAuthoriseUser(int portalId, int userId)
```
***
# AuthoriseUser
**Description**: Marks a user as approved.
**Signature**
```csharp
public static void AuthoriseUser(int portalId, int userId)
```
***
# UpdateEmail
**Description**: Updates the email address for a user.
**Signature**
```csharp
public static UserData UpdateEmail(int portalId, int userId, string email)
```
***
# GetUserProfileProperties
**Description**: Gets a dictionary of all profile properties for a user.
**Signature**
```csharp
public static Dictionary<string, string> GetUserProfileProperties(int portalId, int userId)
```
***
# GetUserProfileProperties
**Description**: Gets a dictionary of all profile properties for a user by their user ID as a string.
**Signature**
```csharp
public static Dictionary<string, string> GetUserProfileProperties(string userId)
```
***
# SetUserProfileProperties
**Description**: Sets multiple profile properties for a user.
**Signature**
```csharp
public static void SetUserProfileProperties(String userId, Dictionary<string, string> properties)
```
***
# SetUserProfileProperties
**Description**: Sets multiple profile properties for a user.
**Signature**
```csharp
public static void SetUserProfileProperties(int portalId, int userId, Dictionary<string, string> properties)
```
***
# GetUserIdByUserName
**Description**: Gets a user's ID from their username.
**Signature**
```csharp
public static int GetUserIdByUserName(int portalId, string username)
```
***
# GetUsers
**Description**: Gets a list of users as SimplisityRecord objects, optionally filtered by role.
**Signature**
```csharp
public static List<SimplisityRecord> GetUsers(int portalId, string inRole = "")
```
***
# GetUsers
**Description**: Gets a paginated list of users as SimplisityRecord objects.
**Signature**
```csharp
public static List<SimplisityRecord> GetUsers(int portalId, int pageNumber, int pageSize, ref int totalRecord, bool includeDeleted = false, bool superUsersOnly = false)
```
***
# GetUsers
**Description**: Searches for users by a text string and returns a list of SimplisityRecord objects.
**Signature**
```csharp
public static List<SimplisityRecord> GetUsers(int portalId, string searchtext, int returnLimit = 100)
```
***
# GetUsersUserData
**Description**: Searches for users by a text string and returns a list of UserData objects.
**Signature**
```csharp
public static List<UserData> GetUsersUserData(int portalId, string searchtext, int returnLimit = 100)
```
***
# GetSuperUsers
**Description**: Gets a list of all super users as SimplisityRecord objects.
**Signature**
```csharp
public static List<SimplisityRecord> GetSuperUsers()
```
***
# GetUserIdByEmail
**Description**: Gets a user's ID from their email address.
**Signature**
```csharp
public static int GetUserIdByEmail(int portalId, string email)
```
***
# IsInRole
**Description**: Checks if a specific user is in a given role.
**Signature**
```csharp
public static bool IsInRole(int portalId, int userId, string role)
```
***
# IsInRole
**Description**: Checks if the current user is in a given role.
**Signature**
```csharp
public static bool IsInRole(string role)
```
***
# IsSuperUser
**Description**: Checks if a specific user is a super user.
**Signature**
```csharp
public static bool IsSuperUser(int portalId, int userId)
```
***
# IsSuperUser
**Description**: Checks if the current user is a super user.
**Signature**
```csharp
public static bool IsSuperUser()
```
***
# IsClientOnly
**Description**: Checks if the current user has only the 'Collaborator' role and no higher-level roles.
**Signature**
```csharp
public static Boolean IsClientOnly()
```
***
# IsManager
**Description**: Checks if the current user is in the 'Manager' or 'Administrators' role.
**Signature**
```csharp
public static Boolean IsManager()
```
***
# IsEditor
**Description**: Checks if the current user has at least 'Collaborator' level permissions.
**Signature**
```csharp
public static Boolean IsEditor()
```
***
# IsAdministrator
**Description**: Checks if the current user is in the 'Administrators' role.
**Signature**
```csharp
public static Boolean IsAdministrator()
```
***
# IsAdministrator
**Description**: Checks if a specific user is an administrator.
**Signature**
```csharp
public static bool IsAdministrator(int portalId, int userId)
```
***
# GetValidUser
**Description**: Validates a user's credentials and returns the UserInfo object if valid.
**Signature**
```csharp
public static UserInfo GetValidUser(int PortalId, string username, string password)
```
***
# IsValidUser
**Description**: Checks if a user exists by username.
**Signature**
```csharp
public static bool IsValidUser(int PortalId, string username)
```
***
# IsValidUser
**Description**: Checks if a user's credentials are valid.
**Signature**
```csharp
public static bool IsValidUser(int PortalId, string username, string password)
```
***
# IsValidUser
**Description**: Checks if a user exists by user ID.
**Signature**
```csharp
public static bool IsValidUser(int portalId, int userId)
```
***
# GetRoleById
**Description**: Gets role information as a SimplisityRecord by role ID.
**Signature**
```csharp
public static SimplisityRecord GetRoleById(int portalId, int roleId)
```
***
# GetRoles
**Description**: Gets a dictionary of all roles in a portal.
**Signature**
```csharp
public static Dictionary<int, string> GetRoles(int portalId)
```
***
# GetRoleByName
**Description**: Gets role information as a SimplisityRecord by role name.
**Signature**
```csharp
public static SimplisityRecord GetRoleByName(int portalId, string roleName)
```
***
# AddUserRole
**Description**: Adds a user to a role.
**Signature**
```csharp
public static void AddUserRole(int portalId, int userId, int roleId)
```
***
# AddUserRole
**Description**: Adds a user to a role and optionally notifies the user.
**Signature**
```csharp
public static void AddUserRole(int portalId, int userId, int roleId, bool notifyUser)
```
***
# RemoveUserRole
**Description**: Removes a user from a role.
**Signature**
```csharp
public static void RemoveUserRole(int portalId, int userId, int roleId)
```
***
# SignOut
**Description**: Signs the current user out.
**Signature**
```csharp
public static void SignOut()
```
***
# HasModuleAccess
**Description**: Checks if a user has a specific level of access to a module.
**Signature**
```csharp
public static bool HasModuleAccess(int portalId, int userId, int moduleId, string permissionKey = "VIEW")
```
***
# UpdateOrCreateUserProfileProperty
**Description**: Updates or creates a DNN user profile property. If the property definition doesn't exist, it will be created.
**Signature**
```csharp
public static bool UpdateOrCreateUserProfileProperty(int userId, string key, string value, int portalId = 0, string category = "Custom", bool visibilityAllUsers = false)
```
