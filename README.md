# Section Generator CLI

![Nuget](https://img.shields.io/nuget/v/SectionGeneratorWebbilir)

[Package Page](https://www.nuget.org/packages/SectionGeneratorWebbilir/)

## Overview

Section Generator CLI is a command-line tool developed for internal use at Webbilir. Its primary purpose is to automate the creation of new section components for both client and admin registration. Additionally, it supports the generation of inputs for text, images, and paragraphs on the admin side of a section, enabling developers to speed up their workflow.

## Installation

You can install the Section Generator CLI via NuGet:

```bash
dotnet tool install --global SectionGeneratorWebbilir --version 2.0.3
```

## Usage

### Basic Command

To generate a new section, use the following command:

```bash
section Example
```

This command will create a new section with the specified name.

### Creating Inputs

You can specify the type and number of inputs for a admin side of section. For example, to create a section with 3 paragraphs and 2 images, use:

```bash
section Example --pr 3 --im 2 --txt 1
```

-  `--pr 3`: 3 paragraphs
-  `--im 2`: 2 images
-  `--txt 1`: 1 textareas
