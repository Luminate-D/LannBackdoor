# LannBackdoor
--- This is backdoor written in C# using .NET 6

### Prerequisites
- This project is using `.NET 6` so you need to install it

### Installing
1. Clone Repository
```sh
$ git clone https://github.com/Luminate-D/LannBackdoor.git
```

2. Open `LannBackdoor.sln` with your IDE (VS / JetBrains Rider)
3. Restore NuGet Packages

### Building & Running
- Make your changes and build & run using the button in the IDE

##### If you have got any issues, feel free to report them at the [issues page](https://github.com/Luminate-D/LannBackdoor/issues)

### Loading Modules
- Modules and their dependencies are loaded by server
- If you want to bundle module with backdoor, insert this code inside `Program.cs`
```cs
ModuleRegistry.LoadByAssembly(typeof(HERE_PUT_MODULE_CLASS).Assembly);
```

## Features & TODO List
- [x] Connecting to server using pattern xxxx-{0}.dom
- [x] Hiding window when Debug = false
- [ ] USB Spread
- [ ] LAN Spread
- [ ] Self Updating
- [x] Anti VM / Fake Environment / Sandbox
- [x] Anti Debug
- [ ] Installer
  - [ ] Escalation to Admin Rights
  - [x] Resolve & Create directory where to install
  - [x] Resolve valid URL & Verify with RSA
  - [x] Download client
  - [x] Install service
  - [x] Service
    - [x] Start Backdoor as LocalSystem
    - [x] Keep Backdoor launched (restart if closed)
- [ ] Modules
  - [x] System Module - Loaded by default
    - [x] Ping
    - [x] RSA Verify - Verify that server is original
    - [x] Load Assembly - Load assemblies required by other modules
    - [x] Load Module - Load modules
    - [x] List Modules
  - [x] Screen Module
    - [x] Take Screenshot
    - [ ] Record Video
    - [ ] UDP Video Streaming
      - [ ] Quality Settings
      - [ ] FPS Settings
    - [ ] GDI+ Addon
      - [ ] Draw image over screen
      - [ ] Play GIF/MP4 over screen
      - [ ] Glitching / Screen Floating
  - [x] Shell Module
    - [x] Open / Close Shell (by ID)
    - [x] Write Commands to Shell
    - [x] Log <std>, open, close back
  - [x] FS Module
    - [x] Download File
    - [x] Upload File / Folder (inside ZIP archive)
    - [x] Read Directory
  - [ ] Audio Module
    - [ ] Record Audio
    - [ ] UDP Audio Streaming (Two sides)
    - [ ] Play Audio
  - [ ] Camera Module
    - [ ] Take Camera Snapshot
    - [ ] Record Video
    - [ ] UDP Video Streaming
  - [ ] Fun Module
    - [ ] Input
      - [ ] Block Input (Keyboard / Mouse)
      - [ ] Inverse Mouse
      - [ ] Show / Hide Mouse
      - [ ] Force Keyboard Write
    - [x] Raise BSOD
    - [x] Open CDRom
    - [x] Turn monitor Off/On
    - [x] Shutdown / Restart PC
  - [ ] Encryption
    - [ ] Encrypt / Decrypt Files
  - [ ] Other
    - [ ] Install Driver
    - [ ] Printer (der Drucker)
      - [ ] Print Local Image / From URL
