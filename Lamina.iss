#define MyAppName "Lamina ✦"
#define MyAppVersion "11.28000.17.0"
#define MyAppPublisher "Chill-Astro Software"
#define MyAppURL "https://github.com/Chill-Astro/Lamina-Calculator"

; Define both architecture-specific filenames
#define MyAppMsixX64 "Lamina_11.28000.17.0_x64.msix"
#define MyAppMsixArm64 "Lamina_11.28000.17.0_arm64.msix"
#define MyAppCertName "Lamina_11.28000.17.0.cer"

[Setup]
AppId={{633C1E5F-90A3-492B-933F-84ECEE95A462}
AppVerName={#MyAppName} Installer Helper
AppName={#MyAppName}
AppVersion={#MyAppVersion}
; Allow the installer to run on x64 and ARM64
ArchitecturesAllowed=x64compatible arm64
ArchitecturesInstallIn64BitMode=x64compatible arm64
DefaultDirName={autopf}\Chill-Astro\Lamina
LicenseFile=LICENSE.txt
PrivilegesRequired=admin
UninstallDisplayIcon={app}\Lamina.ico
WizardStyle=modern dynamic windows11
OutputBaseFilename=Setup
DisableWelcomePage=no
SolidCompression=yes

[Files]
; Include both MSIX files in the installer package
Source: "C:\Users\Master\Chill-Astro\Lamina-Calculator\src\Installer\*"; DestDir: "{app}"; Flags: ignoreversion

[Run]
; 1. Install the Certificate (Universal for x64 and ARM64)
; We use certutil.exe to add the certificate to the Local Machine's Root store.
Filename: "certutil.exe"; \
    Parameters: "-addstore -f ""Root"" ""{app}\{#MyAppCertName}"""; \
    StatusMsg: "Installing Security Certificate..."; \
    Flags: runhidden

; 2. Install x64 MSIX (Only if the OS is NOT ARM64)
Filename: "powershell.exe"; \
    Parameters: "-ExecutionPolicy Bypass -Command ""Add-AppxPackage -Path '{app}\{#MyAppMsixX64}'"""; \
    Check: "not IsArm64"; \
    StatusMsg: "Registering x64 App Package..."; \
    Flags: runhidden

; 3. Install ARM64 MSIX (Only if the OS is ARM64)
Filename: "powershell.exe"; \
    Parameters: "-ExecutionPolicy Bypass -Command ""Add-AppxPackage -Path '{app}\{#MyAppMsixArm64}'"""; \
    Check: "IsArm64"; \
    StatusMsg: "Registering ARM64 App Package..."; \
    Flags: runhidden

[UninstallRun]
Filename: "powershell.exe"; \
    Parameters: "-ExecutionPolicy Bypass -Command ""Get-AppxPackage -Name '*Lamina*' | Remove-AppxPackage"""; \
    Flags: runhidden

[Code]
// Helper function to detect ARM64
function IsArm64: Boolean;
begin
  Result := (ProcessorArchitecture = paARM64);
end;