Properties {
	$base = Split-Path $psake.build_script_file	
	$source = "$base\.."
	$out = "$base\product"
}
FormatTaskName (("-"*25) + "[{0}]" + ("-"*25))

Task default -Depends Clean, Build

Task Build -Depends Clean {
	if (!$out.EndsWith("\\")) {
		$out="$($out)\" #see : http://www.markhneedham.com/blog/2008/08/14/msbuild-use-outputpath-instead-of-outdir/
	}

	Write-Host "Building $source\Listener\Listener.csproj" -ForegroundColor Green
	Exec { msbuild "$source\Listener\Listener.csproj" /t:Build /p:Configuration=Release /p:OutDir=$out /property:TargetFrameworkVersion=v2.0  } 

	Write-Host "Building $source\Parser\Parser.csproj" -ForegroundColor Green
	Exec { msbuild "$source\Parser\Parser.csproj" /t:Build /p:Configuration=Release /p:OutDir=$out /property:TargetFrameworkVersion=v2.0  } 
}

Task Clean {
	Write-Host "Creating $out directory" -ForegroundColor Green

	if (Test-Path $out) {	
		rd $out -rec -force | out-null
	}
	
	mkdir $out | out-null
	
	Write-Host "Cleaning $source\Listener\Listener.csproj" -ForegroundColor Green
	Exec { msbuild "$source\Listener\Listener.csproj" /t:Clean /p:Configuration=Release } 
	
	Write-Host "Cleaning $source\Parser\Parser.csproj" -ForegroundColor Green
	Exec { msbuild "$source\Parser\Parser.csproj" /t:Clean /p:Configuration=Release } 
}