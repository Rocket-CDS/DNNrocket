# Create an AppTheme

An AppTheme is a group of razor templates to render content on a display or admin page.  
You do not need to know razor to build an AppTheme.  Rocket AppThemes uses a standard format to help.  
However....

**If you do not know HTML or CSS, you should learn that before trying to build your own AppTheme.**

#### The Editor

The best editor for working on AppThemes is Visual Studio, you can then use intelliSense.  **But any text editor will work.**

If you wish to setup intelliSense then have a look at the "**AppThemes-W3-CSS**" AppTheme Project.  
[https://github.com/Rocket-CDS/AppThemes-W3-CSS/blob/main/AppThemes-W3-CSS.csproj](https://github.com/Rocket-CDS/AppThemes-W3-CSS/blob/main/AppThemes-W3-CSS.csproj)  
It has a number of references to the bin folder of DNN, you need to copy the way that works.

#### **Step 1 - Create a Named Folder.**

In the RocketCDS installation you need to create a named folder for your AppTheme.  System level Rocket AppThemes are always created in 

```plaintext
/DesktopModules/RocketThemes/#AppThemeProjectName#/#systemkey#.#AppThemeName#/#version#
```

For this example we will use the default "AppThemes-W3-CSS" AppTheme Project folder. (You may find this example already setup in AppThemes-W3-CSS)
