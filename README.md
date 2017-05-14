# Apworks Core
Apworks framework supporting .NET Core.

Branch          | All Platforms     | Windows Server 2012   | Ubuntu 16.04.2             
----------------|-------------------|-----------------------|--------------------------
master          | [![Build Status](http://daxnet-win-svr.eastasia.cloudapp.azure.com:8080/buildStatus/icon?job=apworks-core)](http://daxnet-win-svr.eastasia.cloudapp.azure.com:8080/view/apworks/job/apworks-core/) | [![Build Status](http://daxnet-win-svr.eastasia.cloudapp.azure.com:8080/buildStatus/icon?job=apworks-core-win)](http://daxnet-win-svr.eastasia.cloudapp.azure.com:8080/view/apworks/job/apworks-core-win/) | [![Build Status](http://daxnet-win-svr.eastasia.cloudapp.azure.com:8080/buildStatus/icon?job=apworks-core-ubuntu)](http://daxnet-win-svr.eastasia.cloudapp.azure.com:8080/view/apworks/job/apworks-core-ubuntu/)

## Introduction
Apworks is an application development framework which provides essential components and building blocks for creating, implementing and deploying enterprise applications with either classic layered architecture or event-driven CQRS architecture styles. The original repo of Apworks framework is [here](https://github.com/daxnet/Apworks), which was targeting specifically on .NET Framework 4.5.1 based on some legacy technologies and not being maintained anymore.

Apworks Core is the rewrite of the original Apworks framework, with the following enhancements or features built in:

- Built based on .NET Core (targeting .NET Framework 4.6.1, NetStandard 1.6), which means Apworks Core can work in both Windows and Linux systems
- Integrated with ASP.NET Core seamlessly. Fluent APIs provided by the integration component allows developers to enable Apworks features in ASP.NET Core very easily and smoothly
- Cloud-friendly, support on modern development and DevOps tech stack, such as Microservices, Hypertext Application Language (a.k.a. HAL), Docker, MongoDB, etc.
- Native support of task based programming model (async/await)
- Native support of custom entity identifier
- And so on...

## How to Build

### Build on Windows

1. Make sure Windows Powershell 4.0 or above is installed on your Windows machine
2. Install latest version of .NET Core SDK
2. Install Git
3. Clone this repo with the following command:

	`git clone https://github.com/daxnet/apworks-core`

4. Go into the `build-scripts` folder, execute the following command (arguments in the square bracket are optional):

	`powershell -F build.ps1 [-runUnitTests] [-runIntegrationTests]`

The build artifacts will be placed in the `build` directory under the project root.

### Build on Linux

1. Instal Powershell for Linux, for more information about the installation please refer to: [https://github.com/powershell/powershell#get-powershell](https://github.com/powershell/powershell#get-powershell)
2. Install latest version of .NET Core SDK
3. Install Git
4. Clone this repo with the following command:

	`git clone https://github.com/daxnet/apworks-core`

4. Go into the `build-scripts` folder, execute the following command (arguments in the square bracket are optional):

	`powershell -F build.ps1 -framework netstandard1.6 [-runUnitTests] [-runIntegrationTests]`

The build artifacts will be placed in the `build` directory under the project root.

### Build Script Arguments

The `build.ps1` script accepts the following command line arguments:

- `-version` (optional, string, default `1.0.0`): The version number used for both assemblies and NuGet package
- `-generatePackages` (optional, boolean, default `false`): Indicates if the NuGet packages should be generated instead of only perform the build. _Note that in Linux systems you shouldn't use this argument, as it will build the projects on multiple .NET framework versions, which include .NET Framework 4.6.1 that is not supported by Linux_
- `-framework` (optional, string, default `all`): Specifies the targeting framework of the generated assemblies, this option will be ignored if the `-generatedPackages` is specified. _Note that in Linux systems you must specify the framework version to targeting `netstandard1.6` as the full .NET Framework 4.6.1 is not supported in Linux_
- `-runUnitTests` (optional, boolean, default `false`): Indicates if the unit tests should run
- `-runIntegrationTests` (optional, boolean, default `false`): Indicates if the integration tests should run

## Examples

Please refer to [Apworks Examples](https://github.com/daxnet/apworks-examples) Github repo for the example applications that make use of Apworks Core framework.

## Get Started
1. Add `https://www.myget.org/F/daxnet-apworks/api/v3/index.json` as one of your NuGet package source for the packages in **stable release** versions
2. Add `https://www.myget.org/F/daxnet-apworks-pre/api/v3/index.json` as one of your NuGet package source for the packages in **preview** or **development** versions
3. In Visual Studio 2015/2017, open _Package Manager Console_
4. Select the project that you want to add Apworks Core packages, and execute:
	- `Install-Package Apworks`
	- You can add additional Apworks Core packages by using the `Install-Package` command followed by the package name. E.g. `Install-Package Apworks.Integration.AspNetCore`
	- You can also specify the version of the package that you wish to use, e.g. `Install-Package Apworks.Integration.AspNetCore -Version 1.0`

For more information about using Apworks Core framework in your application scaffold, please read the Apworks Core documentation.

## Components and Packages

Component | Package Name                      | Package Source (preview)         | Package Source (stable)
-----------------------|--------------------------|----------------------------------|--------------------------
Base Framework Library | Apworks                   | [![MyGet Badge](https://buildstats.info/myget/daxnet-apworks-pre/Apworks)](https://www.myget.org/feed/daxnet-apworks-pre/package/nuget/Apworks) | (n/a)
ASP.NET Core integration component | Apworks.Integration.AspNetCore | [![MyGet Badge](https://buildstats.info/myget/daxnet-apworks-pre/Apworks.Integration.AspNetCore)](https://www.myget.org/feed/daxnet-apworks-pre/package/nuget/Apworks.Integration.AspNetCore) | (n/a)
MongoDB repository implementation | Apworks.Repositories.MongoDB | [![MyGet Badge](https://buildstats.info/myget/daxnet-apworks-pre/Apworks.Repositories.MongoDB)](https://www.myget.org/feed/daxnet-apworks-pre/package/nuget/Apworks.Repositories.MongoDB) | (n/a)
Entity Framework Core repository implementation | Apworks.Repositories.EntityFramework | [![MyGet Badge](https://buildstats.info/myget/daxnet-apworks-pre/Apworks.Repositories.EntityFramework)](https://www.myget.org/feed/daxnet-apworks-pre/package/nuget/Apworks.Repositories.EntityFramework) | (n/a)
Simple repository implementation | Apworks.Repositories.Simple | [![MyGet Badge](https://buildstats.info/myget/daxnet-apworks-pre/Apworks.Repositories.Simple)](https://www.myget.org/feed/daxnet-apworks-pre/package/nuget/Apworks.Repositories.Simple) | (n/a)
Simple event store implementation | Apworks.EventStore.Simple | [![MyGet Badge](https://buildstats.info/myget/daxnet-apworks-pre/Apworks.EventStore.Simple)](https://www.myget.org/feed/daxnet-apworks-pre/package/nuget/Apworks.EventStore.Simple) | (n/a)
Common library for Event Store implementation with ADO.NET | Apworks.EventStore.AdoNet | [![MyGet Badge](https://buildstats.info/myget/daxnet-apworks-pre/Apworks.EventStore.AdoNet)](https://www.myget.org/feed/daxnet-apworks-pre/package/nuget/Apworks.EventStore.AdoNet) | (n/a)
Event Store implementation on PostgreSQL | Apworks.EventStore.PostgreSQL | [![MyGet Badge](https://buildstats.info/myget/daxnet-apworks-pre/Apworks.EventStore.PostgreSQL)](https://www.myget.org/feed/daxnet-apworks-pre/package/nuget/Apworks.EventStore.PostgreSQL) | (n/a)
Simple messaging Infrastructure implementation | Apworks.Messaging.Simple | [![MyGet Badge](https://buildstats.info/myget/daxnet-apworks-pre/Apworks.Messaging.Simple)](https://www.myget.org/feed/daxnet-apworks-pre/package/nuget/Apworks.Messaging.Simple) | (n/a)
Querying parsers implemented with [Irony](https://github.com/daxnet/Irony "Irony") | Apworks.Querying.Parsers.Irony | [![MyGet Badge](https://buildstats.info/myget/daxnet-apworks-pre/Apworks.Querying.Parsers.Irony)](https://www.myget.org/feed/daxnet-apworks-pre/package/nuget/Apworks.Querying.Parsers.Irony) | (n/a)
Json serialization implementation | Apworks.Serialization.Json | [![MyGet Badge](https://buildstats.info/myget/daxnet-apworks-pre/Apworks.Serialization.Json)](https://www.myget.org/feed/daxnet-apworks-pre/package/nuget/Apworks.Serialization.Json) | (n/a)

## Documentation
Please find the latest documentation [here](http://apworks-core.readthedocs.io/en/latest/).

## References
- [Legacy Apworks project](https://github.com/daxnet/apworks)

