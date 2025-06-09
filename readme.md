# O365 Typosquat Library

A C# library for detecting typosquatting domains and testing their availability on Microsoft 365 services. Based on the work of O365Squad: https://github.com/O365Squad/O365-Squatting.

## Features

- Generate typosquat domain variations using multiple techniques:
  - Homoglyph substitution (visual character replacements)
  - Character omission and transposition
  - Pluralization and common suffixes
- Test domain availability across 119+ TLDs including major second-level domains
- Check Microsoft 365 service availability (OnMicrosoft, SharePoint, domain resolution)
- Async/await support for high-performance testing
- Available as a NuGet package

## Usage

```csharp
using O365TypoSquat;

var service = new O365TypoSquatService();

// Generate typosquat variations with original TLD
var variations = service.GenerateTypoSquatVariations("google.com");

// Generate variations across all TLDs (com, net, org, co.uk, com.au, etc.)
var allTldVariations = service.GenerateTypoSquatVariationsWithTlds("google.com");

// Test a single domain
var result = await service.TestDomainAsync("g00gle.com");
Console.WriteLine($"OnMicrosoft: {result.OnMicrosoftTest}");
Console.WriteLine($"SharePoint: {result.SharePointTest}"); 
Console.WriteLine($"Domain: {result.DomainTest}");

// Generate and test all variations
var results = await service.GenerateAndTestTypoSquatsAsync("google.com");

// Generate and test with TLD variations (comprehensive scan)
var allResults = await service.GenerateAndTestTypoSquatsWithTldsAsync("google.com");
```

## TLD Coverage

The library tests against 119 domains including:
- Standard TLDs: .com, .net, .org, .info, .biz
- Country codes: .us, .uk, .de, .fr, .au, .ca
- Second-level domains: .co.uk, .com.au, .co.nz, .com.br, .co.za
- Regional variations for major markets worldwide

## Installation

```bash
dotnet add package O365TypoSquat
```

## Building from Source

```bash
git clone https://github.com/Biztactix/O365Squat.git
cd O365Squat
dotnet build
```