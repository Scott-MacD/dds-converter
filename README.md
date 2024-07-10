# DDS-Converter

DDS-Converter is a utility to convert DirectDraw Surface (DDS) files to PNG format using C#. This project includes a C# console application and a Node.js script to call the C# executable.

## Features

- Convert DDS files (including BC1 and BC3 formats) to PNG.
- Handles decompression and format conversion for compatibility with ImageSharp.

## Requirements

- .NET SDK 7.0 or later

## Getting Started

### Prerequisites

Ensure you have the following installed:

- [.NET SDK 7.0](https://dotnet.microsoft.com/download/dotnet/7.0)

### Installation

1. Clone the repository:

    ```bash
    git clone https://github.com/your-username/dds-converter.git
    cd dds-converter
    ```

2. Restore the .NET project dependencies:

    ```bash
    dotnet restore
    ```

3. Publish the C# project:

    ```bash
    dotnet publish -c Release -r win-x64 --self-contained -o "publish"
    ```

### Usage

CLI

    ```bash
    DDS-Converter <input.dds> <output.png>
    ```

Javascript

    ```javascript
    import convertDdsToPng from './index.js';

    const ddsPath = 'path/to/your/file.dds';
    const outputPath = 'path/to/output/file.png';

    convertDdsToPng(ddsPath, outputPath)
        .then(() => {
            console.log('Conversion successful');
        })
        .catch((err) => {
            console.error('Conversion failed:', err);
        });
    ```