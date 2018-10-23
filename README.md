# Key generator for Strongly Named Assembly

This is the key generator for creating [strongly named assembly](https://docs.microsoft.com/en-us/dotnet/framework/app-domains/create-and-use-strong-named-assemblies)

# Requirements

* dotnet runtime 2.1 or later(required netcoreapp2.1)

# Installation

Itn.SnkUtils is packaged as [dotnet global tool](https://docs.microsoft.com/en-us/dotnet/core/tools/global-tools).
So you can get binary by the following command;

`dotnet tool install -g Itn.SnkUtils`

and you can now execute this by `dotnet snkutils [commands]`.

# Basic Usage

you can get option list by `dotnet snkutils --help` or `dotnet snkutils [command name] --help`

## Creating new one

`dotnet snkutils create [output path]`
you must specify `[output path]`.

## Export to PKCS8(openssl's RSA PEM format)

`dotnet snkutils convert-to [snk file path] [output file path]`
you can read info by `openssl rsa -in [output file path] -inform PEM -text`

## Import from PKCS8(openssl's RSA PEM format)

`dotnet snkutils convert-to [pem file path] [output snk file path]`
