@echo off
setlocal

set "PROJECT_DIR=%~dp0..\EasyFramework.Excel"
set "PROJECT_FILE=%PROJECT_DIR%\EasyFramework.Excel.csproj"
set "OUTPUT_DIR=%~dp0EasyFramework.Excel"

echo ========================================
echo EasyFramework.Excel NativeAOT Publish
echo ========================================
echo Project: %PROJECT_FILE%
echo Output:  %OUTPUT_DIR%
echo.

if not exist "%PROJECT_FILE%" (
    echo [ERROR] Project file not found:
    echo %PROJECT_FILE%
    echo.
    pause
    exit /b 1
)

dotnet publish "%PROJECT_FILE%" -c Release -r win-x64 --self-contained true -p:PublishAot=true -o "%OUTPUT_DIR%"
set "EXIT_CODE=%ERRORLEVEL%"

echo.
if not "%EXIT_CODE%"=="0" (
    echo Publish failed. Exit code: %EXIT_CODE%
    pause
    exit /b %EXIT_CODE%
)

if exist "%OUTPUT_DIR%\EasyFramework.Excel.pdb" del /q "%OUTPUT_DIR%\EasyFramework.Excel.pdb"

echo Publish completed successfully.
pause
exit /b 0
