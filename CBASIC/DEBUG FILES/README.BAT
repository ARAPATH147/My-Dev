echo off
if %1. == P. goto P
if %1. == p. goto P
if %1. == D. goto D
if %1. == d. goto D
cls
echo To display README file on screen, type A:README D and press Enter
echo -
echo To print the readme file, type A:README P and press enter
goto R99
:D
more < a:readme.doc
goto R99
:P
echo Turn on your printer.  Align printer form.
pause
copy a:readme.doc prn:
:R99
