#
# Substitute the version number for each AssemblyInfo.cs and project.json file
#
param (
	[string]$version = "1.0.0"
)

$files = Get-ChildItem . -include AssemblyInfo.cs,*.csproj -Recurse
foreach ($file in $files)
{
	(Get-Content $file.FullName) |
	ForEach-Object { $_ -replace "0.999.0", "$($version)" } |
	Set-Content $file.FullName
}
