@ECHO OFF

cd %~dp0

:: Switch to GitHub Pages branch:
git checkout gh-pages
:: Merge the changes from the master branch:
git merge -X theirs -X subtree=TinyCsvParser/Documentation/build/html master
:: And publish online:
git push origin gh-pages

pause
