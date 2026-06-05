; TEMPLATE: Inno Setup Script for Command Palette Extensions
;
; To use this template for a new extension:
; 1. Copy this file to your extension's project folder as "setup-template.iss"
; 2. Replace EXTENSION_NAME with your extension name (e.g., CmdPalMyExtension)
; 3. Replace DISPLAY_NAME with your extension's display name (e.g., My Extension)
; 4. Replace DEVELOPER_NAME with your name (e.g., Your Name Here)
; 5. Replace CLSID-HERE with extensions CLSID
; 6. Update the default version to match your project file

#define AppVersion "1.3.0.0"

[Setup]
AppId={{GUID-HERE}}
AppName=Define Word
AppVersion={#AppVersion}
AppPublisher=TheStarSlayer
DefaultDirName={autopf}\DefineWordExtension
OutputDir=bin\Release\installer
OutputBaseFilename=DefineWordExtension-Setup-{#AppVersion}
Compression=lzma
SolidCompression=yes
MinVersion=10.0.19041

[Languages]
Name: "english"; MessagesFile: "compiler:Default.isl"

[Files]
Source: "bin\Release\win-x64\publish\*"; DestDir: "{app}"; Flags: ignoreversion recursesubdirs

[Icons]
Name: "{group}\Define Word"; Filename: "{app}\DefineWordExtension.exe"

[Registry]
Root: HKCU; Subkey: "SOFTWARE\Classes\CLSID\{{aa4ce889-5441-4fca-a6b6-11fce9e0f6ce}}"; ValueData: "DefineWordExtension"
Root: HKCU; Subkey: "SOFTWARE\Classes\CLSID\{{aa4ce889-5441-4fca-a6b6-11fce9e0f6ce}}\LocalServer32"; ValueData: "{app}\DefineWordExtension.exe -RegisterProcessAsComServer"