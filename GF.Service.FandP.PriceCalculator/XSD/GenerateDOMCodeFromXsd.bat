@echo off
set xsdExePath=C:\Program Files (x86)\Microsoft SDKs\Windows\v10.0A\bin\NETFX 4.6.2 Tools\xsd.exe
set parameters=/c /n:GF.Service.FandP.PriceCalc.DOM /f /l:CS

"%xsdExePath%" FGXM14_1.xsd .\FGXMResponse14.xsd %parameters%

pause
