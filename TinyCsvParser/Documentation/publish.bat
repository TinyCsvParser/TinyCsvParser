@ECHO OFF

cd %~dp0

set GIT_EXECUTABLE="C:\PROGRAM FILES (X86)\Git\cmd\git.exe"

:: Switch to GitHub Pages branch:
%GIT_EXECUTABLE% checkout gh-pages
:: Merge the changes from the master branch:
%GIT_EXECUTABLE% merge -X theirs -X subtree=TinyCsvParser/Documentation/build/html master
:: And publish online:
%GIT_EXECUTABLE% push origin gh-pages

pause
