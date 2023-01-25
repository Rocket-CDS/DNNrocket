# AppTheme

## Front View
The "Front View" is the term used to describe the website public view.
## Admin View
The "Admin View" is the term used to describe the website private administration UI.
## View.cshtml
The "View.cshtml" file is always the starting template for the front view of the module.
## AdminDetail.cshtml
The "AdminDetail.cshtml" file is always the starting template for the admin view of the module.
## Dependancies
Dependancies are CSS or JS files that are required for the theme to function.  The dependancy system stops duplicate CSS and JS files being injected.  
Dependancies ONLY work on the Front View of the website.  For Admin View use "AdminFirstHeader.cshtml" or "AdminLastHeader.cshtml" templates.

**Tokens**  

\{domainurl} = Protocol and domain URL of RocketAPI.
\{appthemefolder} = Relitive URL of the from view AppTheme.
\{appthemesystemfolder} = Relitive URL of the system.

