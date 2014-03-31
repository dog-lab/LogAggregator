param(
	$task = "default",
	$parameters=@{}
)

function ExitWithCode([string] $exitCode) {
	$host.SetShouldExit($exitCode);
	exit;
}
	
$scriptPath = $MyInvocation.MyCommand.Path
$scriptDir = Split-Path $scriptPath
Remove-Module [p]sake -ErrorAction SilentlyContinue

try {
	Set-ExecutionPolicy unrestricted
	Import-Module "$scriptDir\psake.psm1"
	$psake.use_exit_on_error = $true
	Invoke-Psake "$scriptDir\default.ps1" $task -framework '2.0' -parameters $parameters
	ExitWithCode($LastExitcode)
} catch {
	Write-Error $_
	ExitWithCode 9
}