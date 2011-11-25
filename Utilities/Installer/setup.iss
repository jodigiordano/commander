#define MyAppName "Commander"
#define MyAppVersion "0.1.2"
#define MyAppPublisher "Ephemere Games"
#define MyAppURL "http://www.playcommander.com"
#define MyAppExeName "Commander.exe"

[Setup]
AppId={{CE990129-4C19-4DDE-92E1-81198DD929A8}
AppName={#MyAppName}
AppVersion={#MyAppVersion}
AppPublisher={#MyAppPublisher}
AppPublisherURL={#MyAppURL}
AppSupportURL={#MyAppURL}
AppUpdatesURL={#MyAppURL}
DefaultDirName={pf}\Ephemere Games\Commander
DisableDirPage=yes
DefaultGroupName={#MyAppName}
DisableProgramGroupPage=yes
OutputBaseFilename=setup
Compression=lzma
SolidCompression=yes

[Languages]
Name: "english"; MessagesFile: "compiler:Default.isl"

[Tasks]
Name: "desktopicon"; Description: "{cm:CreateDesktopIcon}"; GroupDescription: "{cm:AdditionalIcons}"; Flags: unchecked

[Files]
Source: "src\*"; DestDir: "{app}"; Flags: ignoreversion recursesubdirs createallsubdirs

[Icons]
Name: "{group}\{#MyAppName}"; Filename: "{app}\{#MyAppExeName}"
Name: "{group}\{cm:ProgramOnTheWeb,{#MyAppName}}"; Filename: "{#MyAppURL}"
Name: "{group}\{cm:UninstallProgram,{#MyAppName}}"; Filename: "{uninstallexe}"
Name: "{commondesktop}\{#MyAppName}"; Filename: "{app}\{#MyAppExeName}"; Tasks: desktopicon

[Run]
Filename: "{app}\{#MyAppExeName}"; Description: "{cm:LaunchProgram,{#StringChange(MyAppName, "&", "&&")}}"; Flags: nowait postinstall skipifsilent;
Filename: "{app}\Prerequisites\dotnet.exe"; Parameters: "/q /norestart"; WorkingDir: "{app}\Prerequisites"; StatusMsg: "Installing .NET 4 (time to take a coffee...)";
Filename: "msiexec.exe"; Parameters: "/quiet /i ""{app}\Prerequisites\xna.msi"""; WorkingDir: "{app}\Prerequisites"; StatusMsg: "Installing XNA 4 (time to slowly drink that coffee...)";
