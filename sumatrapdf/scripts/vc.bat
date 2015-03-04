@ECHO OFF

REM append ..\bin to PATH to make nasm.exe available
FOR %%p IN (nasm.exe) DO IF "%%~$PATH:p" == "" SET PATH=%PATH%;%~dp0..\bin

REM Allow to explicitly specify the desired Visual Studio version
IF /I "%1" == "vc13" GOTO TRY_VS13
IF /I "%1" == "vc12" GOTO TRY_VS12
IF /I "%1" == "vc10" GOTO TRY_VS10
IF /I "%1" == "vc9" GOTO TRY_VS9

REM Try Visual Studio 2008 first, as it still supports Windows 2000,
REM Then try 2010 and 2012

REM vs9 is VS 2008
:TRY_VS9
CALL "%VS90COMNTOOLS%\vsvars32.bat" 2>NUL
IF NOT ERRORLEVEL 1 EXIT /B

REM vs10 is VS 2010
:TRY_VS10
CALL "%VS100COMNTOOLS%\vsvars32.bat" 2>NUL
IF NOT ERRORLEVEL 1 EXIT /B

REM vs12 is VS 2012
:TRY_VS12
CALL "%VS110COMNTOOLS%\vsvars32.bat" 2>NUL
IF NOT ERRORLEVEL 1 EXIT /B
REM xp support http://blogs.msdn.com/b/vcblog/archive/2012/10/08/10357555.aspx
SET "INCLUDE=%ProgramFiles(x86)%\Microsoft SDKs\Windows\7.1A\Include;%INCLUDE%"
SET "PATH=%ProgramFiles(x86)%\Microsoft SDKs\Windows\7.1A\Bin;%PATH%"
SET "LIB=%ProgramFiles(x86)%\Microsoft SDKs\Windows\7.1A\Lib;%LIB%"
REM TODO: for 64bit it should be:
REM set LIB=%ProgramFiles(x86)%\Microsoft SDKs\Windows\7.1A\Lib\x64;%LIB%
set CL=/D_USING_V110_SDK71_;%CL%

REM vs13 is VS 2013
:TRY_VS13
CALL "%VS120COMNTOOLS%\vsvars32.bat" 2>NUL
IF NOT ERRORLEVEL 1 EXIT /B

ECHO Visual Studio 2013, 2012, 2010, or 2008 doesn't seem to be installed
EXIT /B 1
