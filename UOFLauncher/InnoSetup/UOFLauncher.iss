#dim Version[4]
#expr ParseVersion("..\bin\Release\UOFLauncher.exe", Version[0], Version[1], Version[2], Version[3])
#define MyAppVersion Str(Version[0]) + "." + Str(Version[1]) + "." + Str(Version[2]) + "." + Str(Version[3])
#define MyAppName "Ultima Online Forever Launcher"
#define MyAppExeName "UOFLauncher"

[Setup]
AppName={#MyAppName}
AppVersion={#MyAppVersion}
AppId={#MyAppName}
DefaultDirName="{src}"
Compression=lzma2
SolidCompression=false
DisableReadyPage=no
DisableReadyMemo=no
DisableStartupPrompt=yes
DisableFinishedPage=yes
Uninstallable=no
OutputDir=Output\
OutputBaseFilename="Launcher-update"
PrivilegesRequired=admin

[CustomMessages]
AppName={#MyAppExeName}
LaunchProgram=Start UOFLauncher after finishing installation

[Files]
Source: "..\bin\Release\UOFLauncher.exe"; DestDir: {app}; Flags: ignoreversion

[RUN]
Filename: {app}\{cm:AppName}.exe; Description: {cm:LaunchProgram,{cm:AppName}}; Flags: nowait postinstall runascurrentuser

[Languages]
Name: "en"; MessagesFile: "compiler:Default.isl"
Name: "de"; MessagesFile: "compiler:Languages\German.isl"

#include "Scripts\products.iss"
#include "Scripts\products\stringversion.iss"
#include "Scripts\products\winversion.iss"
#include "Scripts\products\fileversion.iss"
#include "Scripts\products\dotnetfxversion.iss"
#include "Scripts\products\msi31.iss"
#include "Scripts\products\dotnetfx45.iss"
#include "Scripts\products\vcredist2010.iss"
#include "Scripts\products\vcredist2013.iss"
#include "scripts\products\detectDirectX.iss"

[Code]
function InitializeSetup(): Boolean;
begin
	initwinversion();
	msi31('3.1');
	dotnetfx45(1);
	vcredist2010();
	vcredist2013();
	directX();
	Result := true;
end;
