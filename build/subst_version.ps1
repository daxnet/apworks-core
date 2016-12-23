#
# Substitute the version number for each AssemblyInfo.cs and project.json file
#
param (
	[string]$version = "1.0.0.0"
)

$files = Get-ChildItem . -include project.json,AssemblyInfo.cs -Recurse
foreach ($file in $files)
{
	(Get-Content $file.FullName) |
	ForEach-Object { $_ -replace "0.9999.0.9999", "$($version)" } |
	Set-Content $file.FullName
}
