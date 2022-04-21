# DNNrocket v1.0+

## Dependencies

##### RazorEngine.dll & System.Drawing.Common.dll
- Use NuGet to link RazorEngine to RocketAPI.  
- System.Drawing.Common.dll, required. .Net core does not support drawing yet.  This must be copied local from the reference properties, so the release build can put it in the install zip.


## Create .Net Standard Project

https://github.com/SesameRocket/RocketSystemProjectTemplate


## Build Installation Package

Install and use DNNpackager https://github.com/leedavi/DNNpackager

All assemblies are copied tothe '$(ProjectDir)..\bin\' folder when a project is compiled.  
When DNNpackager.exe is used to open "..\DNNpackager.dnnpack" in 'release' config, an DNNrocket Insall zip package is created in '\\DNNrocket\\Installation' folder.  

**Right click on the DNNpackager.dnnpack file in VS and select Open With..**

