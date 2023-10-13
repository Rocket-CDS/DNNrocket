# DNNrocket v1.1.0.3

## Dependencies

##### RazorEngine.dll & System.Drawing.Common.dll
- Use NuGet to link RazorEngine to RocketAPI.  
- System.Drawing.Common.dll, required. .Net core does not support drawing yet.  This must be copied local from the reference properties, so the release build can put it in the install zip.


## Create .Net Standard Project

https://github.com/SesameRocket/RocketSystemProjectTemplate


## Build Installation Package

Install and use DNNpackager https://github.com/leedavi/DNNpackager

All assemblies are copied to the '$(ProjectDir)..\bin\' folder when a project is compiled.  
When DNNpackager.exe is used to open "..\DNNpackager.dnnpack" in 'release' config, an DNNrocket Insall zip package is created in '\\DNNrocket\\Installation' folder.  

**Right click on the DNNpackager.dnnpack file in VS and select Open With..**

## Release Version Control
DNNrocket is made of multiple projects that can be updated independantly.  
Each project version can be different to the release version.  
The release version is in "DNNpackages.dnnpack" and "DNNrocketAPI.dnn" files and should be updated for all occurences in these files.  
Finding which assembly versions should be in a release can be found in GitHub, by looking at the version tag.  The tag will be the same as the release version.  

**DNNpackages.dnnpack changes**
```
  <!-- To build install file use "Solution Directory" -->
	<version>1.0.14</version>
```
**DNNrocketAPI.dnn changes**
```
	<package name="RocketSystem" type="Library" version="1.0.14">
		....
	<package name="RocketPortal" type="Library" version="1.0.14">
		....
	<package name="RocketAppTheme" type="Library" version="1.0.14">
```

