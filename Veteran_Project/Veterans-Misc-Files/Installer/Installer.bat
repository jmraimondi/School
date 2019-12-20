@ECHO off

ECHO Installing application and photo directories...
mkdir C:\MuseumApp
robocopy .\Common C:\MuseumApp /e /z /ns /nc /nfl /ndl /np /njh /njs

ECHO Installing administrator icon...
@REM Create desktop icon - current user only
robocopy .\Shortcuts\Admin %USERPROFILE%\Desktop\ /z /ns /nc /nfl /ndl /np /njh /njs
@REM Create Start Menu icon - current user only
mkdir "%appdata%\Microsoft\Windows\Start Menu\Programs\MuseumApp"
robocopy .\Shortcuts\Admin "%appdata%\Microsoft\Windows\Start Menu\Programs\MuseumApp" /z /ns /nc /nfl /ndl /np /njh /njs

ECHO Installing viewer icon...
@REM Create desktop icon - all users - using seperate batch file to run as admin
powershell "Start-Process cmd -ArgumentList "/C",'""cd /d "%cd%"&&AllUserShortcuts.bat"'" -Verb RunAs"