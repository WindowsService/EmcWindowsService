%~dp0InstallUtil.exe %~dp0WindowsService.exe
Net Start MyService
sc config MyService start= auto
pause

