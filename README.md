# DNNrocket

# Install Dev system

1. Install DNN
2. Create folder /DesktopModules/DNNrocket
3. Clone GutHub repo to the DNNrocket folder
4. Run SQL scripts from DNN SQL console. /DesktopModules/DNNrocket/API/Installation (01.00.00.SqlDataProvider)
5. Use VS to compile.  Missing links and references could cause issue on inital setup.  (See below)

- Use NuGet to link RazorEngine to RocketAPI.  
- System.Drawing.Common.dll, required. .Net core does not support drawing yet.  This must be copied local from the reference properties, so the release build can put it in the install zip.
- System.Drawing.Common.dll is used for image manipulation and resize on upload.

- RazorEngine.dll & System.Drawing.Common.dll must be moved to the DNN bin folder, take from /DesktopModules/DNNrocket/API/bin

# Create .Net Standard Project

1. Create new project  "Class Library (.Net Standard)" under "~\DesktopModules\<YourCompany>" folder.
2. Set namespace and assembly name to appropriate naming convension.
3. Set post build events:

copy "$(ProjectDir)$(OutDir)$(TargetFileName)" "$(ProjectDir)..\\..\\..\bin\$(TargetFileName)"
copy "$(ProjectDir)$(OutDir)$(AssemblyName).pdb" "$(ProjectDir)..\\..\\..\bin\$(AssemblyName).pdb"

4. Add reference to DNNrocketAPI and Simplisity.
5. Rename first class to "StartConnect" and rename the file to "StartConnect.cs".  (by convension, not required.)
6. StartConnect should inherit from ": DNNrocketAPI.APInterface"
7. Implement abstract class.
8. Add process code: EXAMPLE

        private static RocketInterface _rocketInterface;
        private static string _editLang;

        public override Dictionary<string, object> ProcessCommand(string paramCmd, SimplisityInfo systemInfo, SimplisityInfo interfaceInfo, SimplisityInfo postInfo, string langRequired = "")
        {
            _rocketInterface = new RocketInterface(interfaceInfo);

            _editLang = langRequired;
            if (_editLang == "") _editLang = DNNrocketUtils.GetEditCulture();

            var strOut = "ERROR!! - No Security rights or function command.  Ensure your systemkey is defined. [testform]";

            switch (paramCmd)
            {
                case "rocketcommerce_getstuff":
                    strOut = "get stuff";
                    break;
                default:
                    strOut = "default path";
                    break;
            }


            return DNNrocketUtils.ReturnString(strOut);

        }



9. Add folders "App_LocalResources" and "Themes\config-w3\1.0\default"
10. Add resx file to "App_LocalResources"


# Create System Config. Data

For each application in DNNrocket system we have a system record, which keeps essential configuration data.  This needs to be created for the API system to work. It is this system data the tells the API which provider software needs to be ran for each operation.

1. Go into the System Admin page "~/desktopmodules/dnnrocket/adminsystem.html".
2. Add a new System, Enter a "System Key" (this is the system key, lowercase, no spaces) , "System Name" and "Encrytion Key" are minimum requirment.
3. Add an interface, the interfaces are the link between you API code and the simplisity call.  There can be multiple interface options, but usually one interface for each operation.

NOTE: Validation is not implemented.

- Interface Key*: Identificcation key, this is passed to the API. as part of the command or as a hidden field called "interfaceKey". (this is the system key, lowercase, no spaces)
- Entity Type Key : Data key to be used when we write data to the Database, this is the TypeCode column of the DNNrocket table.
- Group: menu Group, defined in the Group Tab.  There can be a 2 level menu, groups are the first level.
- Interface Icon Class : Font Awesome class. Example: <i class="fab fa-readme"></i> 
- Default Theme : Default theme folder to be used. Usually "config-w3".  This is where the interface will look for the Theme, under the Control path "Theme" folder.
- Default Template : Default razor template to be used.
- Default Command : Default API command to be used.  The command should be is 2 sections with an underscore used as the seperation. "interfacekey_cmd" - The 1 section will be used as the interface key, if the "interfacekey" is not passed to the server as a hidden field.
- NameSpace and Class*:  Namespace and class of the assembly which implements the "ProcessCommand" method.  By convention the "StartConnect" class name is used.
- Assembly*: The name of The interface Assembly.
- Provider Type : The type of provider can be attached (Defined in the "Providers Types" tab), this is for advanced systems and not used in normal operations.
- Control Relative Path : This is the relative path to the "Themes" folder.  It is the parent of the "Themes" folder, usually the module path.

- Active will give a option to identify is the interface is active, The logic should be defined in StartConnect.
- Cahced can be turned on by interface, logic should be defined in StartConnect.
- Security roles can be assigned to the interface so only those roles can use the interface.  The logic to prevent the access should be defined in the StartConnect class.
- Display on menu option.

(*) = required

Optional
--------

1. Menu Groups: Setup Groups for a 2 level menu.
2. Provider Type: Define provider types, used for advanced system were we need to know different provider types, so we can run them on certain events.
3. Indes Fields:  Database index can be created on XML fields to speed operation. The selection process tend to be very quick, but sorting is slow for XML fields, so sometimes an index is required.  Do not create unless you need these.
4. Settings: Any setting values can be defined for the system.

Load System Data
----------------

The system data can be loaded by adding an XML file into the "~/desktopmodules/dnnrocket/API/systems" folder.  The XML can be taken from the "System Data" tab,  copy and paste the system XML data into a file and save.  By convension the file name should be in this format {*******_system.xml}.  This file will be loaded when the System Admin UI is view. 


Build Installation Package
--------------------------

Install and use DNNpackager https://github.com/leedavi/DNNpackager

1. Compile all projects in the DNNrocket project. https://github.com/SesameRocket/DNNrocket
2. Run .\DNNpackager.dnnpack file with DNNpackager.exe.

This will build an install package in the .\Installation folder of the DNNrocket project.
