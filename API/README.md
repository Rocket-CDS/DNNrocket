# RocketEcommerce


**Post Build command (Example)**

C:\Nevoweb\Projects\PostBuildCmd\DNNrocket\API\www.dnnrocket.com.bat  $(ProjectDir) $(ProjectDir)$(OutDir) $(ConfigurationName)


**Post Build BAT file (Example)**


echo off
set mpath = "C:\Nevoweb\Websites\www.dnnrocket.com\Install\DesktopModules\DNNrocket\API"
set bpath = "C:\Nevoweb\Websites\www.dnnrocket.com\Install\bin"
C:\Nevoweb\Projects\Utils\DNNpackager\bin\netcoreapp3.1\DNNpackager.exe %1 %mpath% %2 %bpath% %3



