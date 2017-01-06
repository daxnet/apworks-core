# This script will create a 'publish' folder under '/build' folder, compile the source code from '/src'
# with Release configuration, and generates the NuGet packages under 'publish' folder.

New-Item publish -ItemType directory -Force

$dirs = Get-ChildItem ../src -Directory

foreach ($dir in $dirs)
{
	dotnet pack $dir.FullName --configuration Release --output publish
}