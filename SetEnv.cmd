echo on

set scriptPath=%~dp0
set externalsPath=%scriptPath%Externals
set binPath=%scriptPath%bin

copy "%externalsPath%\\FileHelpers\\Binaries\\DotNet 2.0\\FileHelpers.dll" %binPath%
set "PATH=%PATH%;%scriptPath%\\bin"

set binPath=
set externalsPath=
set scriptPath=