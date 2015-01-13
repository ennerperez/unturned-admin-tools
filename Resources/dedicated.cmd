@echo off
echo Starting server...
:Starting
unturned.exe -port:25444 -players:16 -nographics -pei -normal -nosync -pvp -sv 
REM -batchmode
echo Warning, server crash detected. Restarting...
goto Starting