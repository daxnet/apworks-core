# Apworks Core
Apworks framework supporting .NET Core.

[![Build status](https://dev.azure.com/sunnycoding/Apworks-Core/_apis/build/status/Apworks%20Core%20Build%20Pipeline)](https://dev.azure.com/sunnycoding/Apworks-Core/_build/latest?definitionId=5)

## Introduction
Apworks is an application development framework which provides essential components and building blocks for creating, implementing and deploying enterprise applications with either classic layered architecture or event-driven CQRS architecture styles. The original repo of Apworks framework is [here](https://github.com/daxnet/Apworks), which was targeting specifically on .NET Framework 4.5.1 based on some legacy technologies and not being maintained anymore.

Apworks Core is the rewrite of the original Apworks framework, with the following enhancements or features built in:

- Built based on .NET Core (targeting NetStandard 2.0), which means Apworks Core can work in both Windows and Linux systems
- Integrated with ASP.NET Core seamlessly. Fluent APIs provided by the integration component allows developers to enable Apworks features in ASP.NET Core very easily and smoothly
- Cloud-friendly, support on modern development and DevOps tech stack, such as Microservices, Hypertext Application Language (a.k.a. HAL), Docker, MongoDB, etc.
- Native support of task based programming model (async/await)
- Native support of custom entity identifier
- And so on...

## How to Build

1. Install latest version of .NET Core SDK
2. Install Git
3. Clone this repo with the following command:

	`git clone https://github.com/daxnet/apworks-core`

4. Build Apworks with the following command:
   
	`dotnet build Apworks.sln`

## Examples

Please refer to [Apworks Examples](https://github.com/daxnet/apworks-examples) Github repo for the example applications that make use of Apworks Core framework.

## Documentation
Please find the latest documentation [here](http://apworks-core.readthedocs.io/en/latest/).

## References
- [Legacy Apworks project](https://github.com/daxnet/apworks)

