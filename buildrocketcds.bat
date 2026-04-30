@echo off
setlocal EnableExtensions

set "ROOT=%~dp0"
set "SLN=%ROOT%RocketCDS.sln"
set "CONFIG=R All"
set "PLATFORM=Any CPU"
set "BUILDLOG=%ROOT%_buildrocketcds.log"
set "PACKAGER=C:\Program Files (x86)\Nevoweb\DNNpackager\DNNpackager.exe"

echo.
echo ============================================================
echo =============== BUILDROCKETCDS START ======================
echo ============================================================
echo.

rem ---- Locate MSBuild ----
set "MSBUILD="
for /f "usebackq delims=" %%i in (`"%ProgramFiles(x86)%\Microsoft Visual Studio\Installer\vswhere.exe" -latest -requires Microsoft.Component.MSBuild -find MSBuild\**\Bin\MSBuild.exe`) do set "MSBUILD=%%i"

if not defined MSBUILD goto :msbuild_not_found

rem ---- Build all projects first ----
echo Building solution: "%SLN%"
echo Configuration: %CONFIG% ^| %PLATFORM%
echo Log: "%BUILDLOG%"
echo.

"%MSBUILD%" "%SLN%" /m /t:Restore,Build /p:Configuration="%CONFIG%" /p:Platform="%PLATFORM%" /fl /flp:logfile="%BUILDLOG%";verbosity=normal
if errorlevel 1 goto :build_failed

echo.
echo ================= BUILD SUCCEEDED ==========================
echo.

if not exist "%PACKAGER%" goto :packager_not_found
echo Using packager: "%PACKAGER%"
echo.

call :run_pack "%ROOT%RocketCDS.dnnpack" "RocketCDS.dnnpack"
if errorlevel 1 exit /b 1

call :run_pack "%ROOT%RocketCDSupgrade.dnnpack" "RocketCDSupgrade.dnnpack"
if errorlevel 1 exit /b 1

call :run_pack "%ROOT%RocketCDSrazor.dnnpack" "RocketCDSrazor.dnnpack"
if errorlevel 1 exit /b 1

call :run_pack "%ROOT%RocketCDSrazorupgrade.dnnpack" "RocketCDSrazorupgrade.dnnpack"
if errorlevel 1 exit /b 1

echo.
echo ============================================================
echo ================ ALL PACKAGES SUCCEEDED ===================
echo ============================================================
echo.
exit /b 0

:run_pack
set "PACKFILE=%~1"
set "PACKNAME=%~2"

if not exist "%PACKFILE%" (
    echo.
    echo ************************************************************
    echo *** ERROR: PACK FILE NOT FOUND: %PACKNAME%
    echo ************************************************************
    exit /b 1
)

echo ---- Packaging: %PACKNAME%
"%PACKAGER%" "%PACKFILE%"
if errorlevel 1 (
    echo.
    echo ************************************************************
    echo *** PACKAGING FAILED: %PACKNAME%
    echo ************************************************************
    exit /b 1
)
echo ---- Packaging OK: %PACKNAME%
echo.
exit /b 0

:build_failed
echo.
echo ############################################################
echo ###### BUILD FAILED - PACKAGING WAS STOPPED ###############
echo ############################################################
echo.

set "FIRSTERROR=(No error line found in log)"
for /f "usebackq delims=" %%L in (`powershell -NoProfile -ExecutionPolicy Bypass -Command "$m=Select-String -Path '%BUILDLOG%' -Pattern ': error ' | Select-Object -First 1; if($m){$m.Line}else{'(No error line found in log)'}"`) do set "FIRSTERROR=%%L"

echo First error:
echo %FIRSTERROR%
echo.
echo Full log:
echo "%BUILDLOG%"
echo.
exit /b 1

:packager_not_found
echo.
echo ************************************************************
echo *** ERROR: DNNpackager.exe not found: %PACKAGER%
echo ************************************************************
echo.
exit /b 1

:msbuild_not_found
echo.
echo ************************************************************
echo *** ERROR: MSBuild not found
echo ************************************************************
echo.
exit /b 1