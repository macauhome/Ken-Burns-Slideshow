# Windows 11 screensaver migration checklist for Ken Burns Slideshow

This document captures the exact Windows-only validation and packaging steps required after the project was migrated from the legacy .NET Framework 4.5.1 project model to .NET 10 WPF.

## 1. Required environment

Use a Windows 11 machine with:

- Visual Studio 2022
- .NET 10 SDK
- .NET desktop development workload
- Windows App SDK / WPF support enabled

## 2. Project target

The project file must target:

- `net10.0-windows`
- `UseWPF=true`
- `UseWindowsForms=true`

This preserves the existing WPF UI and the folder browser used by the settings screen.

## 3. Build steps

Open a Developer PowerShell or Command Prompt as Administrator and run:

```powershell
cd C:\path\to\Ken-Burns-Slideshow

# Restore and build in Release mode
 dotnet restore
 dotnet build "Ken Burns Slideshow.vbproj" -c Release
```

Expected output:

- a Windows executable is produced under:
  `bin\Release\net10.0-windows\`

## 4. Screensaver startup contract

The app must keep the following command-line behavior:

- `/s` -> normal slideshow mode
- `/p` -> preview mode
- `/c` -> settings dialog

This is the minimum contract required for Windows Screen Saver Settings to work correctly.

## 5. Packaging as a .scr file

After a successful Release build, rename the generated executable:

```powershell
Copy-Item "bin\Release\net10.0-windows\Ken Burns Slideshow.exe" "Ken Burns Slideshow.scr"
```

Then copy the .scr file to a usable location, such as:

```powershell
Copy-Item "Ken Burns Slideshow.scr" "C:\Windows\System32\Ken Burns Slideshow.scr"
```

## 6. Test in Windows Screen Saver Settings

1. Open Settings > Personalization > Lock screen > Screen saver settings
2. Choose the newly installed .scr entry
3. Click Preview to verify preview mode
4. Set the screensaver and wait for activation to verify full-screen mode
5. Use the settings button or `/c` path to verify config UI opens correctly

## 7. Functional validation checklist

### Startup modes
- [ ] `/s` starts slideshow in full-screen mode
- [ ] `/p` opens in preview rectangle and does not show a full-screen app
- [ ] `/c` opens the configuration dialog without requiring the slideshow window
- [ ] default startup without args still launches slideshow normally

### Configuration behavior
- [ ] settings dialog loads folders from the current config
- [ ] folder add/edit/remove works
- [ ] config.xml is still written correctly
- [ ] image/music folder recursion still behaves correctly

### Slideshow behavior
- [ ] slideshow loads images from configured folders
- [ ] transitions still function correctly
- [ ] background music plays and loops
- [ ] fade/exit behavior still works on ESC or close actions

### Preview mode specifics
- [ ] preview window is embedded in the Screen Saver preview pane
- [ ] it fits the preview rectangle without extending outside the host boundary
- [ ] it does not show a taskbar entry or behave like a separate desktop app

## 8. Common issues to watch for

### Preview HWND parenting
If preview mode is not embedded properly:

- the screen saver may fill the whole desktop instead of the preview pane
- the app may reappear as a separate top-level window
- the preview may ignore the host rectangle size

### WPF thread lifetime
If the slideshow is launched in settings or preview mode:

- UI thread shutdown can cancel the app unexpectedly
- timers or animations may continue after the window closes

### FolderBrowserDialog
Because the app still uses Windows Forms folder selection, ensure `UseWindowsForms=true` remains enabled in the project file.

## 9. Final packaging command example

```powershell
$buildDir = "bin\Release\net10.0-windows"
$exe = Join-Path $buildDir "Ken Burns Slideshow.exe"
$scr = "Ken Burns Slideshow.scr"
Copy-Item $exe $scr
Copy-Item $scr "C:\Windows\System32\Ken Burns Slideshow.scr"
```

## 10. Note about this environment

The current workspace is running in a Linux container, so the actual .NET 10 Windows build cannot be verified here. The above steps are the correct Windows-side validation path for the migrated project.
