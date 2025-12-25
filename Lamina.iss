#define MyAppName "Lamina"
#define MyAppVersion "11.26100.12.0"
#define MyAppPublisher "Chill-Astro Software"
#define MyAppURL "https://github.com/Chill-Astro/Lamina"
#define MyAppExeName "Lamina.exe"
#define MyAppAssocName MyAppName + " File"
#define MyAppAssocExt ".myp"
#define MyAppAssocKey "LaminaFile.myp"

[Setup]
AppId={{696541A4-84D5-49C0-83E4-28DE6A8BD9DA}
AppName={#MyAppName}
AppVersion={#MyAppVersion}
AppPublisher={#MyAppPublisher}
AppPublisherURL={#MyAppURL}
DefaultDirName={autopf}\Chill-Astro\{#MyAppName}
UninstallDisplayIcon={app}\{#MyAppExeName}
ArchitecturesAllowed=x64compatible
ArchitecturesInstallIn64BitMode=x64compatible
ChangesAssociations=yes
DisableProgramGroupPage=yes
LicenseFile=C:\Users\Master\Chill-Astro\Lamina\LICENSE.txt
PrivilegesRequired=admin
OutputBaseFilename=Lamina-Setup
SolidCompression=yes
DisableWelcomePage=no
WizardStyle=modern dynamic windows11

[Languages]
Name: "english"; MessagesFile: "compiler:Default.isl"

[Tasks]
; Restored Task UI
Name: "desktopicon"; Description: "{cm:CreateDesktopIcon}"; GroupDescription: "{cm:AdditionalIcons}"

[Files]
; 1. Copy EVERYTHING to the Downloads folder first for the "Sandbox" install
Source: "C:\Users\Master\Chill-Astro\Lamina\Lamina-Installer\*"; DestDir: "{userdocs}\..\Downloads\Lamina_App"; Flags: ignoreversion recursesubdirs createallsubdirs
; 2. Also copy to Program Files as the permanent home
Source: "C:\Users\Master\Chill-Astro\Lamina\Lamina-Installer\*"; DestDir: "{app}"; Flags: ignoreversion recursesubdirs createallsubdirs; Excludes: "*.msix,InstallDeps.ps1"

[Registry]
; Restored File Associations & Path
Root: HKA; Subkey: "Software\Classes\{#MyAppAssocExt}\OpenWithProgids"; ValueType: string; ValueName: "{#MyAppAssocKey}"; ValueData: ""; Flags: uninsdeletevalue
Root: HKA; Subkey: "Software\Classes\{#MyAppAssocKey}"; ValueType: string; ValueName: ""; ValueData: "{#MyAppAssocName}"; Flags: uninsdeletekey
Root: HKA; Subkey: "Software\Classes\{#MyAppAssocKey}\DefaultIcon"; ValueType: string; ValueName: ""; ValueData: "{app}\{#MyAppExeName},0"
Root: HKA; Subkey: "Software\Classes\{#MyAppAssocKey}\shell\open\command"; ValueType: string; ValueName: ""; ValueData: """{app}\{#MyAppExeName}"" ""%1"""
Root: HKLM; Subkey: "SYSTEM\CurrentControlSet\Control\Session Manager\Environment"; ValueName: "Path"; ValueType: expandsz; ValueData: "{olddata};{app}"

[Icons]
Name: "{autoprograms}\{#MyAppName}"; Filename: "{app}\{#MyAppExeName}"
Name: "{autodesktop}\{#MyAppName}"; Filename: "{app}\{#MyAppExeName}"; Tasks: desktopicon

[Run]
; The final "Launch" checkbox on the modern UI
Filename: "{app}\{#MyAppExeName}"; Description: "{cm:LaunchProgram,{#StringChange(MyAppName, '&', '&&')}}"; Flags: nowait postinstall skipifsilent

[Code]
procedure CurStepChanged(CurStep: TSetupStep);
var
  ResultCode: Integer;
  ScriptPath: String;
begin
  if CurStep = ssPostInstall then
  begin
      // Path to script in the Downloads Sandbox
      ScriptPath := ExpandConstant('{userdocs}\..\Downloads\Lamina_App\InstallDeps.ps1');
      
      // Run the script. We wait (ewWaitUntilTerminated) so the dependencies 
      // are fixed before the user can click the "Finish" button to launch.
      Exec('powershell.exe', '-ExecutionPolicy Bypass -WindowStyle Normal -File "' + ScriptPath + '"', '', SW_SHOW, ewWaitUntilTerminated, ResultCode);
  end;
end;