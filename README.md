# TenForce Hiring - Developer (Test and Taste)

This project is a C# .NET console application developed for the TenForce developer hiring exercise. It communicates with the Solar System OpenData REST API to fetch astronomical data, calculate statistics for celestial bodies (such as average moon temperature and average moon gravity for planets), and output the results to both the console and CSV files on disk.

## Overview

The application fetches data about planets and moons from the public Solar System OpenData API.

Core functionality:
- List all planets and their moons.
- List all moons with their mass values and mass exponents.
- Filter planets that have at least one moon and calculate the average temperature of their moons.
- Calculate the average moon gravity for planets.
- Display formatted tabular data in the console and export CSV files to the `FileOutput` folder.

## Public API Reference

- API Home: https://api.le-systeme-solaire.net/en/
- Swagger Documentation: https://api.le-systeme-solaire.net/swagger/
- Endpoint: `/rest/bodies`
- Authentication: No authentication required.

## Prerequisites

- .NET 5.0 SDK or higher
- Git
- Visual Studio 2019/2022, Visual Studio Code, or JetBrains Rider

## Setup and Installation

1. Clone the repository:
   ```bash
   git clone <repository-url>
   cd Test-TenForce
   ```

2. Create and checkout your feature branch:
   ```bash
   git checkout -b main
   git checkout -b TenForce-yourinitials
   ```

3. Restore dependencies:
   ```bash
   dotnet restore
   ```

4. Build the solution:
   ```bash
   dotnet build
   ```

## How to Run

### Using .NET CLI

Run from the solution root directory:
```bash
dotnet run --project ./Test-Taste-Console-Application/Test-Taste-Console-Application.csproj
```

Or run from the project directory:
```bash
cd Test-Taste-Console-Application
dotnet run
```

### Using Visual Studio

1. Open `Test-Taste-Console-Application.sln`.
2. Set `Test-Taste-Console-Application` as the Startup Project.
3. Press `F5` to run with debugging or `Ctrl+F5` to run without debugging.

## Code Structure

The solution is divided into clear layers:

- `Domain/Repositories`: Data access layer using `HttpClient` to call the external API.
- `Domain/DataTransferObjects`: DTO classes for deserializing API JSON responses.
- `Domain/Objects`: Rich domain models (`Planet`, `Moon`) that encapsulate calculation logic like `AverageMoonTemperature` and `AverageMoonGravity`.
- `Domain/Services`: Business logic and presentation services (`PlanetService`, `MoonService`, `ScreenOutputService`, `FileOutputService`).
- `Utilities`: Helper classes for console output formatting, CSV generation, HTTP requests, and logging.
- `Constants`: Constant values for URLs, headers, formatting, and messages.

## Output Files

When the application runs, it generates four CSV files in the `FileOutput` directory:

- `AllPlanetsAndTheirMoons.csv`: Planets and all their moons.
- `AllMoonsAndTheirMass.csv`: Moons with their mass values and exponents.
- `AllPlanetsAndTheirAverageMoonGravity.csv`: Average moon gravity per planet.
- `AllPlanetsAndTheirAverageMoonTemperature.csv`: Average moon temperature per planet.

## Dependencies

- Microsoft.Extensions.DependencyInjection (5.0.2)
- Microsoft.Extensions.Http (5.0.0)
- Newtonsoft.Json (13.0.1)
- log4net (3.3.0)

## Branching and Submission

1. Commit your intermediate changes to your feature branch (`TenForce-yourinitials`).
2. When completed, merge your feature branch into `main`:
   ```bash
   git checkout main
   git merge TenForce-yourinitials
   ```
3. Push your branch to remote and share your repository link.
