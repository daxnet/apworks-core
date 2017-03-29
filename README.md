# Apworks Core
Apworks framework supporting .NET Core.

Branch          | All               | Windows           | Ubuntu Linux             
----------------|-------------------|-------------------|--------------------------
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

## Get Started
1. Add `https://www.myget.org/F/daxnet-apworks/api/v3/index.json` as one of your NuGet package source
2. In Visual Studio 2015/2017, open Package Manager Console
3. Select the project that you want to add Apworks Core packages, and execute:
	- `Install-Package Apworks`
	- You can add additional Apworks Core packages by using the `Install-Package` command followed by the package name. E.g. `Install-Package Apworks.Integration.AspNetCore`
	- You can also specify the version of the package that you wish to use, e.g. `Install-Package Apworks.Integration.AspNetCore -Version 1.0`

For more information about using Apworks Core framework in your application scaffold, please read the Apworks Core documentation.

## Documentation
Please find the latest documentation [here](http://apworks-core.readthedocs.io/en/latest/).

## References
- [Legacy Apworks project](https://github.com/daxnet/apworks)

