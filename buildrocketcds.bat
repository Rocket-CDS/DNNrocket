@echo off
setlocal EnableExtensions

set "ROOT=%~dp0"
pushd "%ROOT%" || (
    echo ERROR: Cannot switch to solution directory: "%ROOT%"
    exit /b 1
)

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

rem ============================================================
rem HARD-CODED PACKAGE CALLS (one per .dnnpack)
rem ============================================================

set "PACKNAME=RocketCDS.dnnpack"
if not exist "%ROOT%RocketCDS.dnnpack" goto :pack_file_missing
echo ---- Packaging: %PACKNAME%
"%PACKAGER%" "%ROOT%RocketCDS.dnnpack"
if errorlevel 1 goto :pack_failed
echo ---- Packaging OK: %PACKNAME%
echo.

set "PACKNAME=RocketCDSupgrade.dnnpack"
if not exist "%ROOT%RocketCDSupgrade.dnnpack" goto :pack_file_missing
echo ---- Packaging: %PACKNAME%
"%PACKAGER%" "%ROOT%RocketCDSupgrade.dnnpack"
if errorlevel 1 goto :pack_failed
echo ---- Packaging OK: %PACKNAME%
echo.

set "PACKNAME=RocketCDSrazor.dnnpack"
if not exist "%ROOT%RocketCDSrazor.dnnpack" goto :pack_file_missing
echo ---- Packaging: %PACKNAME%
"%PACKAGER%" "%ROOT%RocketCDSrazor.dnnpack"
if errorlevel 1 goto :pack_failed
echo ---- Packaging OK: %PACKNAME%
echo.

set "PACKNAME=RocketCDSrazorupgrade.dnnpack"
if not exist "%ROOT%RocketCDSrazorupgrade.dnnpack" goto :pack_file_missing
echo ---- Packaging: %PACKNAME%
"%PACKAGER%" "%ROOT%RocketCDSrazorupgrade.dnnpack"
if errorlevel 1 goto :pack_failed
echo ---- Packaging OK: %PACKNAME%
echo.

echo.
echo ============================================================
echo ================ ALL PACKAGES SUCCEEDED ===================
echo ============================================================
echo.
goto :done_ok

:pack_file_missing
echo.
echo ************************************************************
echo *** ERROR: PACK FILE NOT FOUND: %PACKNAME%
echo ************************************************************
echo.
goto :done_fail

:pack_failed
echo.
echo ************************************************************
echo *** PACKAGING FAILED: %PACKNAME%
echo ************************************************************
echo.
goto :done_fail

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
goto :done_fail

:packager_not_found
echo.
echo ************************************************************
echo *** ERROR: DNNpackager.exe not found: %PACKAGER%
echo ************************************************************
echo.
goto :done_fail

:msbuild_not_found
echo.
echo ************************************************************
echo *** ERROR: MSBuild not found
echo ************************************************************
echo.
goto :done_fail

:done_ok
popd
exit /b 0

:done_fail
popd
exit /b 1