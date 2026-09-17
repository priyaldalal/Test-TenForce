using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using Test_Taste_Console_Application.Constants;
using Test_Taste_Console_Application.Domain.Objects;
using Test_Taste_Console_Application.Domain.Services.Interfaces;
using Test_Taste_Console_Application.Utilities;

namespace Test_Taste_Console_Application.Domain.Services
{
    /// <inheritdoc />
    public class FileOutputService : IFileOutputService
    {
        private readonly IPlanetService _planetService;
        private readonly IMoonService _moonService;

        public FileOutputService(IPlanetService planetService, IMoonService moonService)
        {
            _planetService = planetService;
            _moonService = moonService;
        }

        private void EnsureOutputFolderExists()
        {
            if (!Directory.Exists(PathName.PathToOutputFolder))
            {
                Directory.CreateDirectory(PathName.PathToOutputFolder);
                Console.WriteLine($"{OutputString.FolderCreated}{PathName.PathToOutputFolder}");
            }
        }

        public void OutputAllPlanetsAndTheirMoonsToFile()
        {
            try
            {
                EnsureOutputFolderExists();
                var filePath = Path.Combine(PathName.PathToOutputFolder, PathName.AllPlanetsAndTheirMoonsFile);
                Console.WriteLine($"Writing file: {filePath}...");

                var planets = _planetService.GetAllPlanets().ToArray();
                var lines = new List<string>
                {
                    $"{OutputString.PlanetNumber},{OutputString.PlanetId},{OutputString.PlanetSemiMajorAxis},{OutputString.TotalMoons},{OutputString.MoonNumber},{OutputString.MoonId}"
                };

                for (int i = 0; i < planets.Length; i++)
                {
                    var planet = planets[i];
                    var planetIdFormatted = CultureInfoUtility.TextInfo.ToTitleCase(planet.Id);
                    if (planet.HasMoons())
                    {
                        for (int k = 0; k < planet.Moons.Count; k++)
                        {
                            var moon = planet.Moons.ElementAt(k);
                            var moonIdFormatted = CultureInfoUtility.TextInfo.ToTitleCase(moon.Id);
                            lines.Add($"{i + 1},{planetIdFormatted},{planet.SemiMajorAxis},{planet.Moons.Count},{k + 1},{moonIdFormatted}");
                        }
                    }
                    else
                    {
                        lines.Add($"{i + 1},{planetIdFormatted},{planet.SemiMajorAxis},0,-,-");
                    }
                }

                File.WriteAllLines(filePath, lines, Encoding.UTF8);
                Console.WriteLine($"{OutputString.FileCreated}{filePath}");
                Console.WriteLine();
            }
            catch (Exception ex)
            {
                Logger.Instance.Error($"{LoggerMessage.FileOutputOperationFailed}{ex.Message}");
                Console.WriteLine($"{ExceptionMessage.FileOutputOperationFailed}{ex.Message}");
            }
        }

        public void OutputAllMoonsAndTheirMassToFile()
        {
            try
            {
                EnsureOutputFolderExists();
                var filePath = Path.Combine(PathName.PathToOutputFolder, PathName.AllMoonsAndTheirMassFile);
                Console.WriteLine($"Writing file: {filePath}...");

                var moons = _moonService.GetAllMoons().ToArray();
                var lines = new List<string>
                {
                    $"{OutputString.MoonNumber},{OutputString.MoonId},{OutputString.MoonMassExponent},{OutputString.MoonMassValue}"
                };

                for (int i = 0; i < moons.Length; i++)
                {
                    var moon = moons[i];
                    var moonIdFormatted = CultureInfoUtility.TextInfo.ToTitleCase(moon.Id);
                    lines.Add($"{i + 1},{moonIdFormatted},{moon.MassExponent},{moon.MassValue}");
                }

                File.WriteAllLines(filePath, lines, Encoding.UTF8);
                Console.WriteLine($"{OutputString.FileCreated}{filePath}");
                Console.WriteLine();
            }
            catch (Exception ex)
            {
                Logger.Instance.Error($"{LoggerMessage.FileOutputOperationFailed}{ex.Message}");
                Console.WriteLine($"{ExceptionMessage.FileOutputOperationFailed}{ex.Message}");
            }
        }

        public void OutputAllPlanetsAndTheirAverageMoonGravityToFile()
        {
            try
            {
                EnsureOutputFolderExists();
                var filePath = Path.Combine(PathName.PathToOutputFolder, PathName.AllPlanetsAndTheirAverageMoonGravityFile);
                Console.WriteLine($"Writing file: {filePath}...");

                var planets = _planetService.GetAllPlanets().ToArray();
                var lines = new List<string>
                {
                    $"{OutputString.PlanetId},{OutputString.PlanetMoonAverageGravity}"
                };

                foreach (var planet in planets)
                {
                    var planetIdFormatted = CultureInfoUtility.TextInfo.ToTitleCase(planet.Id);
                    if (planet.HasMoons())
                    {
                        lines.Add($"{planetIdFormatted},{planet.AverageMoonGravity:F4}");
                    }
                    else
                    {
                        lines.Add($"{planetIdFormatted},-");
                    }
                }

                File.WriteAllLines(filePath, lines, Encoding.UTF8);
                Console.WriteLine($"{OutputString.FileCreated}{filePath}");
                Console.WriteLine();
            }
            catch (Exception ex)
            {
                Logger.Instance.Error($"{LoggerMessage.FileOutputOperationFailed}{ex.Message}");
                Console.WriteLine($"{ExceptionMessage.FileOutputOperationFailed}{ex.Message}");
            }
        }

        public void OutputAllPlanetsAndTheirAverageMoonTemperatureToFile()
        {
            try
            {
                EnsureOutputFolderExists();
                var filePath = Path.Combine(PathName.PathToOutputFolder, PathName.AllPlanetsAndTheirAverageMoonTemperatureFile);
                Console.WriteLine($"Writing file: {filePath}...");

                var planets = _planetService.GetAllPlanets().ToArray();
                var lines = new List<string>
                {
                    $"{OutputString.PlanetId},{OutputString.PlanetMoonAverageTemperature}"
                };

                foreach (var planet in planets)
                {
                    if (planet.HasMoons())
                    {
                        var planetIdFormatted = CultureInfoUtility.TextInfo.ToTitleCase(planet.Id);
                        lines.Add($"{planetIdFormatted},{planet.AverageMoonTemperature:F2}");
                    }
                }

                File.WriteAllLines(filePath, lines, Encoding.UTF8);
                Console.WriteLine($"{OutputString.FileCreated}{filePath}");
                Console.WriteLine();
            }
            catch (Exception ex)
            {
                Logger.Instance.Error($"{LoggerMessage.FileOutputOperationFailed}{ex.Message}");
                Console.WriteLine($"{ExceptionMessage.FileOutputOperationFailed}{ex.Message}");
            }
        }
    }
}
