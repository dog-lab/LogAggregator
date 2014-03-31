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

	Write-Host "Building $source\Entity\Entity.csproj" -ForegroundColor Green
	Exec { msbuild "$source\Entity\Entity.csproj" /t:Build /p:Configuration=Release /p:OutDir=$out /property:TargetFrameworkVersion=v2.0 }

	Write-Host "Building $source\Aggregator\Aggregator.csproj" -ForegroundColor Green
	Exec { msbuild "$source\Aggregator\Aggregator.csproj" /t:Build /p:Configuration=Release /p:OutDir=$out /property:TargetFrameworkVersion=v2.0 }

	Write-Host "Building $source\Console\Console.csproj" -ForegroundColor Green
	Exec { msbuild "$source\Console\Console.csproj" /t:Build /p:Configuration=Release /p:OutDir=$out /property:TargetFrameworkVersion=v2.0 }
}

Task Clean {
	Write-Host "Creating $out directory" -ForegroundColor Green
	
	if (Test-Path $out) {
		rd $out -rec -force | out-null
	}

	mkdir $out | out-null

	Write-Host "Cleaning $source\Entity\Entity.csproj" -ForegroundColor Green
	Exec { msbuild "$source\Entity\Entity.csproj" /t:Clean /p:Configuration=Release }

	Write-Host "Cleaning $source\Aggregator\Aggregator.csproj" -ForegroundColor Green
	Exec { msbuild "$source\Aggregator\Aggregator.csproj" /t:Clean /p:Configuration=Release }

	Write-Host "Cleaning $source\Console\Console.csproj" -ForegroundColor Green
	Exec { msbuild "$source\Console\Console.csproj" /t:Clean /p:Configuration=Release }
}