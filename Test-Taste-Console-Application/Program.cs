using System;
using System.IO;
using System.Reflection;
using log4net;
using log4net.Config;
using Microsoft.Extensions.DependencyInjection;
using Test_Taste_Console_Application.Constants;
using Test_Taste_Console_Application.Domain.Repositories;
using Test_Taste_Console_Application.Domain.Repositories.Interfaces;
using Test_Taste_Console_Application.Domain.Services;
using Test_Taste_Console_Application.Domain.Services.Interfaces;
using Test_Taste_Console_Application.Utilities;

namespace Test_Taste_Console_Application
{
    /// <summary>
    /// The main entry point class for the TenForce Hiring Solar System console application.
    /// Manages service configuration, dependency injection, and execution of screen and file output operations.
    /// Architecture enforces clear separation between Data Access (Repositories), Data Objects (DTOs & Domain Models), and Data Handling (Services).
    /// </summary>
    class Program
    {
        /// <summary>
        /// Main application entry point. Initializes dependency injection and triggers service operations.
        /// </summary>
        /// <param name="args">Command line arguments.</param>
        static void Main(string[] args)
        {
            try
            {
                Console.WriteLine("======================================================================");
                Console.WriteLine("TenForce Hiring - Solar System OpenData Application Execution");
                Console.WriteLine("=========================================================================");
                Console.WriteLine();
                Console.WriteLine("Initializing application services...");

                var serviceCollection = new ServiceCollection();

                // Configure dependency injection container and services
                ConfigureServices(serviceCollection);

                Console.WriteLine("Services configured successfully. Starting execution pipeline...\n");

                // Execute screen and file operations
                RunServiceOperations(serviceCollection);

                Console.WriteLine("=========================================================");
                Console.WriteLine("Execution completed successfully. All outputs generated.");
                Console.WriteLine("============================================================");
            }
            catch (Exception ex)
            {
                // Global unhandled exception handler
                Logger.Instance.Error($"Unhandled exception during main execution: {ex.Message}", ex);
                Console.WriteLine($"Critical Error: Unhandled exception encountered: {ex.Message}");
            }
        }

        /// <summary>
        /// Resolves output services from the Dependency Injection provider and executes screen and file operations.
        /// Keeps the user informed of each step during execution.
        /// </summary>
        /// <param name="serviceCollection">Service collection container.</param>
        private static void RunServiceOperations(IServiceCollection serviceCollection)
        {
            var serviceProvider = serviceCollection.BuildServiceProvider();

            try
            {
                // Retrieve output service implementations
                var screenOutputService = serviceProvider.GetService<IOutputService>();
                var fileOutputService = serviceProvider.GetService<IFileOutputService>();

                if (screenOutputService == null || fileOutputService == null)
                {
                    throw new InvalidOperationException("Failed to resolve output services from Dependency Injection container.");
                }


                Console.WriteLine("STAGE 1: Executing Console Screen Outputs");
                Console.WriteLine();

                try
                {
                    // 1. Output Planets with moons and their average moon temperature (New Extension Feature)
                    Console.WriteLine("Generating console output: Planets with Moons and Average Moon Temperature...");
                    screenOutputService.OutputAllPlanetsAndTheirAverageMoonTemperatureToConsole();

                    // 2. Output Planets and their average moon gravity
                    Console.WriteLine("Generating console output: Planets and Average Moon Gravity...");
                    screenOutputService.OutputAllPlanetsAndTheirAverageMoonGravityToConsole();

                    // 3. Output Moons and their mass values
                    Console.WriteLine("Generating console output: Moons and Their Mass...");
                    screenOutputService.OutputAllMoonsAndTheirMassToConsole();

                    // 4. Output Planets and their moons list
                    Console.WriteLine("Generating console output: Planets and Their Moons...");
                    screenOutputService.OutputAllPlanetsAndTheirMoonsToConsole();
                }
                catch (Exception exception)
                {
                    Logger.Instance.Error($"{LoggerMessage.ScreenOutputOperationFailed}{exception.Message}", exception);
                    Console.WriteLine($"{ExceptionMessage.ScreenOutputOperationFailed}{exception.Message}");
                    System.Diagnostics.Debug.WriteLine($"{ExceptionMessage.ScreenOutputOperationFailed}{exception.Message}");
                }

                Console.WriteLine("STAGE 2: Executing Disk File CSV Outputs");
                Console.WriteLine();

                try
                {
                    // 5. Output Planets with moons and their average moon temperature to CSV file on disk
                    Console.WriteLine("Writing CSV file to disk: AllPlanetsAndTheirAverageMoonTemperature.csv...");
                    fileOutputService.OutputAllPlanetsAndTheirAverageMoonTemperatureToFile();

                    // 6. Output Planets and their average moon gravity to CSV file on disk
                    Console.WriteLine("Writing CSV file to disk: AllPlanetsAndTheirAverageMoonGravity.csv...");
                    fileOutputService.OutputAllPlanetsAndTheirAverageMoonGravityToFile();

                    // 7. Output Moons and their mass to CSV file on disk
                    Console.WriteLine("Writing CSV file to disk: AllMoonsAndTheirMass.csv...");
                    fileOutputService.OutputAllMoonsAndTheirMassToFile();

                    // 8. Output Planets and their moons to CSV file on disk
                    Console.WriteLine("Writing CSV file to disk: AllPlanetsAndTheirMoons.csv...");
                    fileOutputService.OutputAllPlanetsAndTheirMoonsToFile();
                }
                catch (Exception exception)
                {
                    Logger.Instance.Error($"{LoggerMessage.FileOutputOperationFailed}{exception.Message}", exception);
                    Console.WriteLine($"{ExceptionMessage.FileOutputOperationFailed}{exception.Message}");
                    System.Diagnostics.Debug.WriteLine($"{ExceptionMessage.FileOutputOperationFailed}{exception.Message}");
                }
            }
            finally
            {
                // Clean up service provider resources
                serviceProvider.Dispose();
            }
        }

        /// <summary>
        /// Configures dependency injection services for Data Access Repositories, Domain Data Services, and Output Services.
        /// </summary>
        /// <param name="serviceCollection">Service collection container.</param>
        private static void ConfigureServices(IServiceCollection serviceCollection)
        {
            // Configure log4net logging framework from configuration file
            XmlConfigurator.Configure(LogManager.GetRepository(Assembly.GetEntryAssembly()),
                new FileInfo(ConfigurationFileName.Logger));

            // Register HTTP client service with Typed Client pattern
            serviceCollection.AddHttpClient<HttpClientService>();

            // Register Data Access Repositories (Data Access Layer)
            serviceCollection.AddSingleton<IPlanetRepository, PlanetRepository>();
            serviceCollection.AddSingleton<IMoonRepository, MoonRepository>();

            // Register Business Data Handling Services (Service Layer)
            serviceCollection.AddSingleton<IPlanetService, PlanetService>();
            serviceCollection.AddSingleton<IMoonService, MoonService>();

            // Register Presentation/Output Services (Output Layer)
            serviceCollection.AddSingleton<IOutputService, ScreenOutputService>();
            serviceCollection.AddSingleton<IFileOutputService, FileOutputService>();
        }
    }
}
