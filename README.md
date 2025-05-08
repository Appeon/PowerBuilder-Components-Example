# PowerBuilder-Components-Example 

This application demonstrates the utilization of [PowerBuilder 2025](https://www.appeon.com/products/powerbuilder)'s .NET Assembly integration features such as: object instantiation, function calling with automatic native-type conversion and callback invocation to implement features not available in PowerBuilder (e.g. FTP File Transfer, QR Code generation, and more).

## Project Structure

This project is composed of two entities, the PowerBuilder Workspace and a C# Solution (GitHub URL: https://github.com/Appeon/PowerBuilder-Components-Example/). They are structured as follows:

```
.
├── C# Solution
│   ├── Appeon.Windows.PBInterop
│   ├── BarcodeGeneration
│   ├── CSharpFunctions
│   ├── ClassesDemo
│   ├── DdeTools
│   ├── DdeTools.Common
│   ├── DdeTools.DdeClient
│   ├── DdeTools.DdeServer
│   ├── DdeTools.PowerBuilderAdapter
│   ├── DdeTools.PowerBuilderAdapter.Common
│   ├── DdeTools.PowerBuilderAdapter.DdeClient
│   ├── DdeTools.PowerBuilderAdapter.DdeServer
│   ├── FtpClientWrapper
│   ├── FunctionsDemo
│   ├── OpenAITools
│   ├── PbExtensions
│   ├── PbExtensions.Windows
│   ├── PowerBuilderEventInvoker.DotNet
│   ├── PowerBuilderEventInvoker.DotNetFramework
│   ├── ProcessTools
│   ├── QRCodeDecoder
│   ├── QRCoderWrapper
│   ├── QRDecoderWrapper
│   ├── ScreenTools
│   ├── ScreenToolsWrapper
│   ├── SmsMessaging.Common
│   ├── SmsMessaging.Twilio
│   ├── ScreenToolsWrapper
│   ├── SysInfoTools
│   └── XmlEditor
└── Example Components App
    └── Components App
```

## Requirements

Running this project requires the following pieces of software to be present on the machine:

1. [PowerBuilder 2025](https://www.appeon.com/products/powerbuilder) 
2. [.NET Framework 4.8 Runtime ](https://dotnet.microsoft.com/en-us/download/dotnet-framework/net48) 
3. [.NET 8 Runtime](https://dotnet.microsoft.com/en-us/download/dotnet/8.0)

## Setting up the project

Most examples don't rely on any external components (i.e. database connection) and are able to run as-is, with the following exceptions:

1. FTP - Requires access to an FTP server.
2. Get OS Info - Requires to be run as Administrator.

## Running the Project

To run the application:

1. Open the `Example Components App\componentsapp.pbw` workspace on PowerBuilder.

2. Run the project.

3. Use the RibbonBar buttons to open the different views. Each view focuses on a different feature of the .NET Assembly Integration and provides examples.

### Sections

The following are the singular views in the demo and the C# project and class they rely on.

#### Functions

Demonstrates the passing of arguments to C# functions, both by value and by reference.

#### Classes

This view demonstrates how to handle the return values of functions/methods that return objects.

#### Callbacks

This view sets up a callback that is then passed to the .NET Assembly, that is then called when the operation finishes. 

#### QR Code

Utilizes [QrCoder](https://github.com/codebude/QRCoder), a popular C# QR Code generation library and wraps it around a compatibility layer to make it usable from PowerBuilder.

#### Barcode

Uses [barcodelib](https://github.com/barnhill/barcodelib) to generate barcodes of several standards and uses them in PowerBuilder

#### FTP

Transfer files to and from an FTP server by making use of C#'s [FluentFTP](https://github.com/robinrodricks/FluentFTP) Library.

#### XML Editor

Open, manipulate and save XML documents by using C#'s native XMLDocument object hierarchy.

#### DDE

Use [NDDE](https://github.com/anphonic/NDde) on C# to communicate with applications using the DDE protocol. Included as well is a replacement implementation for PowerBuilder's DDE features, which were deprecated starting from version [2022 R2](https://docs.appeon.com/pb/whats_new/Discontinued_Obsolete_features.html) and an example of using it.

#### OpenAI

Make use of OpenAI's Completions and Image Generation features through a C# [OpenAI](https://github.com/betalgo/openai) library.

#### Send SMS

Sends SMS messages by using [Twilio's C# APIs](https://github.com/twilio/twilio-csharp).

#### Get OS Info

Uses Windows's Management Instrumentation's APIs to retrieve device and process information.

## C# Solution Description

The following are the C# solutions located in the .NET folder and their respective role:

- Appeon.Windows.PBInterop - Contains C# bridges to Windows APIs.
- BarcodeGeneration - Wraps barcodelib utils so it can be used from PowerBuilder
- CSharpFunctions - Demonstrates calling C# functions from PowerBuilder.
- ClassesDemo - Demonstrates working with C# classes from PowerBuilder.
- DdeTools - Contains the Factory object for the DdeClient and DdeServer objects.
- DdeTools.Common - Common code for all the DdeTools hierarchy.
- DdeTools.DdeClient - Client side of the DDE Communication module and its PowerBuilder wrapper.
- DdeTools.DdeServer - Server side of the DDE Communication module and its PowerBuilder wrapper.
- DdeTools.PowerBuilderAdapter - Object hierarchy for the objects that replace PowerBuilder's DDE functionality.
- DdeTools.PowerBuilderAdapter.Common - Common objects for the PB DDE Adapter.
- DdeTools.PowerBuilderAdapter.DdeClient - Adapter for the DDE Client functions.
- DdeTools.PowerBuilderAdapter.DdeServer - Adapter for the DDE Server functions.
- FtpClientWrapper - Objects for FTP file transferring and a wrapper for PB.
- OpenAITools - Objects for communicating with the OpenAI service and fetching Completions and Image Generation results.
- PowerBuilderEventInvoker.DotNet - Common code for PB Event invocation on .NET.
- PowerBuilderEventInvoker.DotNetFramework - Common code for PB Event invocation on .NET Framework.
- QRCodeDecoder - Library for QR Code generation and interpretation.
- QRDecoderWrapper - Wrapper for the QRCodeDecoder library.
- QrCoderWrapper - Wrapper for the QRCodeDecoder library.
- PbExtensions - Contains a C# implementation of commonly used features not present in PB by default (such as string splitting, directory listing, and more).
- PbExtensions.Windows - Same as PbExtensions, but for Windows-only features.
- ScreenTools - Tools for taking screenshots.
- ScreenToolsWrapper - PB Wrapper for ScreenTools.
- SmsMessaging.Common - Interface abstraction for a very simplistic messaging service
- SmsMessaging.Twilio - Twilio implementation of the messaging service
- SysInfoTools - Tools for retrieving system process and resource utilization percentage.
- XmlEditor - Tools for handling XML documents and its nodes.
