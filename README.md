# DNNrocket

#Create Project

1. Create new project  "Class Library (.Net Standard)" under "~\DesktopModules\DNNrocket" folder.
2. Set namespace and assembly name to appropriate naming convension.
3. Set post build events:

copy "$(ProjectDir)$(OutDir)$(TargetFileName)" "$(ProjectDir)..\..\..\bin\$(TargetFileName)"
copy "$(ProjectDir)$(OutDir)$(AssemblyName).pdb" "$(ProjectDir)..\..\..\bin\$(AssemblyName).pdb"

4. Rename first class to "startconnect" and rename the file to "startconnect.cs".  (by convension, not required.)
5. startconnect should inherit from ": DNNrocketAPI.APInterface"
6. Implement abstract class.
7. Add process code: EXAMPLE


        public override string ProcessCommand(string paramCmd, SimplisityInfo sInfo, string userHostAddress, string editlang = "")
        {
            //CacheUtils.ClearAllCache();

            var controlRelPath = "/DesktopModules/DNNrocket/TestList";

            var strOut = "ERROR!! - No Security rights or function command.  Ensure your systemprovider is defined. [testform]";

            switch (paramCmd)
            {
                case "testlist_get":
                    strOut = "get data";
                    break;
                default:
                    strOut = "default path";
                    break;
            }
            return strOut;
        }


8. Add reference to DNNrocketAPI and Simplisity.
9. Add folders "App_LocalResources" and "Themes\config-w3\default"
10. Add resx file to "App_LocalResources"



