cd /d %~dp0

rd /q /s product
md product

cd ..\Main\build
call build.cmd
cd ..\..\Extensions\build
call build.cmd

cd ..\..\build-working-sample\product
copy ..\..\Extensions\build\product\*.* . /b /y
copy ..\..\Main\build\product\*.* . /b /y

set productPath=%~dp0product\
set productPath=%productPath:\=\\%

echo [>gator-console.json
echo 	{>>gator-console.json
echo 		'Id': 1,>>gator-console.json
echo 		'Location': '%productPath%Test.log',>>gator-console.json
echo 		'Parser': {>>gator-console.json
echo 			'AssemblyName': '%productPath%Parser.dll',>>gator-console.json
echo 			'ClassName': 'Gator.Extensions.Parser.TestParser'>>gator-console.json
echo 		},>>gator-console.json
echo 		'Listeners': [>>gator-console.json
echo 			{>>gator-console.json
echo 			'AssemblyName': '%productPath%Listener.dll',>>gator-console.json
echo 			'ClassName': 'Gator.Extensions.Listener.ConsoleOutput'>>gator-console.json
echo 			}>>gator-console.json
echo 		]>>gator-console.json
echo 	}>>gator-console.json
echo ]>>gator-console.json

copy "..\..\sample files\test.log" .
cd %~dp0
