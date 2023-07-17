@echo off
setlocal

set CWD=%CD%

rem Select msbuild.exe version
rem set MSBUILD="c:\Program Files (x86)\MSBuild\14.0\Bin\MSBuild.exe"
rem set MSBUILD="c:\Program Files (x86)\Microsoft Visual Studio\2017\Community\MSBuild\15.0\Bin\MSBuild.exe"
set MSBUILD="c:\Program Files (x86)\Microsoft Visual Studio\2017\Professional\MSBuild\15.0\Bin\MSBuild.exe"

rem Generate strong key: sn -k keyfile.snk
set KEYFILE=%CD%\WebPViewer.snk

rem Enable assmebly signing
set KEYSIGN=true

rem Path to release file
set RELEASE_EXE=Release\webpviewer.exe

rem Using: make.bat strongname
if "%1"=="keyfile" goto create-keyfile
if "%1"=="check" goto check-signature

if not exist %MSBUILD% goto error-msbuild

if not exist Release mkdir Release
if exist Release del /Q Release\*.*

if not "%KEYSIGN%"=="true" (
	rem Build "unsigned" assemblies
	%MSBUILD% WebPViewer.sln /t:Build /p:OutputPath=%CWD%\Release /p:Configuration=Release
	rem if not "%ERRORLEVEL%"=="0" goto error-compile
)

if "%KEYSIGN%"=="true" (
  rem Check if signging key file exists
  if not exist "%KEYFILE%" (
    echo Error: Signing key file is missing.
    echo Hint: Run 'make keyfile' inside Developer Command Prompt for Visual Studio.
    exit /b 1
  )
	rem Build "signed" strong name assmeblies
	%MSBUILD% WebPViewer.sln /t:Build /p:OutputPath=%CWD%\Release /p:Configuration=Release /p:SignAssembly=true /p:AssemblyOriginatorKeyFile=%KEYFILE% /p:DelaySign=false
	rem if not "%ERRORLEVEL%"=="0" goto error-compile
)

rem Check if release file exists
if not exist "%RELEASE_EXE%" goto error-compile

rem Clean up
del /Q Release\*.pdb

echo Done!
goto end

:create-keyfile
rem Check if 'sn' command exists
where sn >nul 2>&1
set WHERE_SN=%ERRORLEVEL%

rem Check if running inside developer console
if not "%WHERE_SN%" == "0" (
  echo Error: Requires 'sn.exe' to generate a signing key.
  echo Hint: Run in Developer Command Prompt for Visual Studio.
  exit /b 1
)

rem Check if signging key file exists
if exist "%KEYFILE%" (
  echo Signing key file already exists. No changes.
  exit /b 0
)

rem Check if signging key file exists
if not exist "%KEYFILE%" (
  rem Generate a new keyfile
  sn -k "%KEYFILE%"
)

rem Check again if generated
if not exist "%KEYFILE%" (
  echo Error: Cannot generate .snk file.
  exit /b 1
)

rem echo Keyfile generated: %KEYFILE%
echo Done!
goto end

:check-signature
rem Check if "sn.exe" is a available
sn >nul 2>&1
if not "%ERRORLEVEL%" == "0" (
  echo Error: Requires Developer Command Prompt for Visual Studio
	exit /b 1
)

if not exist "%RELEASE_EXE%" (
  echo Error: Project not built yet. Hint: Run make.
	exit /b 1
)

rem Check the second argument
sn -T %RELEASE_EXE% | findstr Public
goto end

:error-msbuild
echo Error: msbuild.exe not found.
echo Hint: edit make.bat and update the path.
endlocal
exit /b 0

:error-compile
echo Failed to build the release!
endlocal
exit /b 0

:end
endlocal
exit /b 0