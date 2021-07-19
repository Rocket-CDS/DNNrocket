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


Build Installation Package
--------------------------

Install and use DNNpackager https://github.com/leedavi/DNNpackager

1. Compile all projects in the DNNrocket project. https://github.com/SesameRocket/DNNrocket
2. Run .\DNNpackager.dnnpack file with DNNpackager.exe.

This will build an install package in the .\Installation folder of the DNNrocket project.
