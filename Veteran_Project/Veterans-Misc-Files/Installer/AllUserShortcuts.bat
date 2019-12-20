@ECHO off

@REM This is in a seperate batch file because it needs administrator privileges

@REM Create desktop icons - all users
robocopy .\Shortcuts\AllUsers %public%\Desktop /z /ns /nc /nfl /ndl /np /njh /njs
@REM Create Start Menu icons - all users
mkdir "%ProgramData%\Microsoft\Windows\Start Menu\Programs\MuseumApp"
robocopy .\Shortcuts\AllUsers "%ProgramData%\Microsoft\Windows\Start Menu\Programs\MuseumApp" /z /ns /nc /nfl /ndl /np /njh /njs

@REM Deny all rights to everyone to admin files, then add full rights to user that installed the app
icacls "C:\MuseumApp\Museum Admin.exe" /inheritance:r /Q
icacls "C:\MuseumApp\Museum Admin.exe" /grant *S-1-3-0:F /Q
icacls "C:\MuseumApp\Museum Admin.exe.config" /inheritance:r /Q
icacls "C:\MuseumApp\Museum Admin.exe.config" /grant *S-1-3-0:F /Q