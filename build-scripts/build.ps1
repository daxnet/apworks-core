#==================================================================================================================                                                                                          
#         ,::i                                                           BBB                
#        BBBBBi                                                         EBBB                
#       MBBNBBU                                                         BBB,                
#      BBB. BBB     BBB,BBBBM   BBB   UBBB   MBB,  LBBBBBO,   :BBG,BBB :BBB  .BBBU  kBBBBBF 
#     BBB,  BBB    7BBBBS2BBBO  BBB  iBBBB  YBBJ :BBBMYNBBB:  FBBBBBB: OBB: 5BBB,  BBBi ,M, 
#    MBBY   BBB.   8BBB   :BBB  BBB .BBUBB  BB1  BBBi   kBBB  BBBM     BBBjBBBr    BBB1     
#   BBBBBBBBBBBu   BBB    FBBP  MBM BB. BB BBM  7BBB    MBBY .BBB     7BBGkBB1      JBBBBi  
#  PBBBFE0GkBBBB  7BBX   uBBB   MBBMBu .BBOBB   rBBB   kBBB  ZBBq     BBB: BBBJ   .   iBBB  
# BBBB      iBBB  BBBBBBBBBE    EBBBB  ,BBBB     MBBBBBBBM   BBB,    iBBB  .BBB2 :BBBBBBB7  
# vr7        777  BBBu8O5:      .77r    Lr7       .7EZk;     L77     .Y7r   irLY  JNMMF:    
#                LBBj
# 
#  Apworks Application Development Framework
#  Copyright (C) 2009-2017 by daxnet.
#  Licensed under the Apache License, Version 2.0 (the "License");
#  you may not use this file except in compliance with the License.
#  You may obtain a copy of the License at
#     http://www.apache.org/licenses/LICENSE-2.0
#  Unless required by applicable law or agreed to in writing, software
#  distributed under the License is distributed on an "AS IS" BASIS,
#  WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
#  See the License for the specific language governing permissions and
#  limitations under the License.
#==================================================================================================================

# Project Build Script for Windows Powershell
# Copyright (C) by Sunny Chen (daxnet) 2009-2017
# Licensed under Apache 2.0
#
# Parameters:
# -version               Specifies the version number
# -generatePackages      Switch indicates whether NuGet packages should be generated
#                        instead of built artifacts
# -framework             Specifies the framework of the build. This parameter is ignored
#                        when -generatePackages parameter is specified
# -runUnitTests          Switch indicates if the unit tests should run
# -runIntegrationTests   Switch indicates if the integration tests should run

param (
	[string]$version = "1.0.0",
	[switch]$generatePackages = $false,
	[string]$framework = "all",
	[switch]$runUnitTests = $false,
	[switch]$runIntegrationTests = $false
)

$workspace = (Split-Path $PSScriptRoot -Parent).ToString()

echo "Building Apworks from $workspace ..."

echo "Substituting version number to $version ..."
# Firstly substitutes the version number on both .cs and .csproj files
# to ensure that the version number for assembly and NuGet package is consistent.
$files = Get-ChildItem $workspace -Include AssemblyInfo.cs,*.csproj -Recurse
foreach ($file in $files)
{
	(Get-Content $file.FullName) |
	ForEach-Object { $_ -replace "0.999.0", "$($version)" } |
	Set-Content $file.FullName
}

# Switch to the workspace folder and restore the packages
echo "Restoring packages ..."
cd $workspace
dotnet restore -s https://api.nuget.org/v3/index.json -s https://www.myget.org/F/daxnet-utils/api/v3/index.json

# Remove the "build" folder, if it exists
if (Test-Path -Path $workspace\build) {
	Remove-Item -Recurse -Force $workspace\build
}

# Enumerates all the folders under 'src' and perform either build or generates the NuGet Packages
# according to the $generatePackages argument
$srcDirs = Get-ChildItem $workspace\src -Directory -Filter "Apworks*"
foreach ($srcDir in $srcDirs)
{
	$projectDirectory = $srcDir.FullName;
	echo "Building project $projectDirectory ..."
	if ($generatePackages) {
		dotnet pack --output $workspace\build -c Release $projectDirectory
	} else {
		$csproj = $projectDirectory + "\" + $srcDir.Name + ".csproj"
		if ($framework -eq "all") {
			dotnet build --output $workspace\build -c Release $csproj
		} else {
			dotnet build -f $framework --output $workspace\build -c Release $csproj
		}
	}
}

# Run unit tests, if the user wants to
if ($runUnitTests) {
	if (Test-Path -Path $workspace\tests\Apworks.Tests\TestResults) {
		Remove-Item -Recurse -Force $workspace\tests\Apworks.Tests\TestResults
	}
	cd $workspace\tests\Apworks.Tests
	dotnet test --logger "trx"
}
