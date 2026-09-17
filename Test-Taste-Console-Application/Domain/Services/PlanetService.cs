using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Test_Taste_Console_Application.Domain.DataTransferObjects;
using Test_Taste_Console_Application.Domain.Objects;
using Test_Taste_Console_Application.Domain.Repositories.Interfaces;
using Test_Taste_Console_Application.Domain.Services.Interfaces;
using Test_Taste_Console_Application.Utilities;

namespace Test_Taste_Console_Application.Domain.Services
{
    /// <summary>
    /// Business Service layer for managing and processing Planet domain objects.
    /// Handles mapping from Data Transfer Objects (DTOs) to Domain Models, concurrent moon data aggregation, and in-memory caching.
    /// </summary>
    public class PlanetService : IPlanetService
    {
        private readonly IPlanetRepository _planetRepository;
        private IEnumerable<Planet> _cachedPlanets;

        /// <summary>
        /// Initializes a new instance of the <see cref="PlanetService"/> class.
        /// </summary>
        /// <param name="planetRepository">The planet data access repository.</param>
        public PlanetService(IPlanetRepository planetRepository)
        {
            _planetRepository = planetRepository ?? throw new ArgumentNullException(nameof(planetRepository));
        }

        /// <summary>
        /// Retrieves all planets and populates their moon domain objects.
        /// Returns cached dataset on subsequent calls for high performance.
        /// </summary>
        /// <returns>Collection of <see cref="Planet"/> domain objects.</returns>
        public IEnumerable<Planet> GetAllPlanets()
        {
            // Return cached data if already loaded
            if (_cachedPlanets != null && _cachedPlanets.Any())
            {
                return _cachedPlanets;
            }

            Console.WriteLine("Loading planets data from API...");
            var allPlanetsWithTheirMoons = new Collection<Planet>();

            try
            {
                // Retrieve raw DTOs from repository (Data Access Layer)
                var planetDtos = _planetRepository.GetPlanetDtos();

                // Process each planet DTO and aggregate moon details
                foreach (var planetDto in planetDtos)
                {
                    if (planetDto.Moons != null && planetDto.Moons.Any())
                    {
                        Console.WriteLine($"Loading details for {planetDto.Moons.Count} moon(s) of planet '{planetDto.Id}'...");
                        var newMoonsCollection = new Collection<MoonDto>();

                        // Concurrently fetch moon details via repository
                        var moonTasks = planetDto.Moons.Select(moon => _planetRepository.GetMoonDtoByIdAsync(moon.URLId));
                        var fetchedMoons = Task.WhenAll(moonTasks).Result;

                        foreach (var moonDto in fetchedMoons)
                        {
                            if (moonDto != null)
                            {
                                newMoonsCollection.Add(moonDto);
                            }
                        }
                        planetDto.Moons = newMoonsCollection;
                    }

                    // Convert DTO into domain object
                    allPlanetsWithTheirMoons.Add(new Planet(planetDto));
                }
            }
            catch (Exception ex)
            {
                Logger.Instance.Error($"Error in PlanetService.GetAllPlanets: {ex.Message}", ex);
                Console.WriteLine($"Error processing planets data: {ex.Message}");
            }

            // Cache dataset in memory
            _cachedPlanets = allPlanetsWithTheirMoons;
            return allPlanetsWithTheirMoons;
        }

        /// <summary>
        /// Utility function to normalize text strings by removing diacritical marks (accents).
        /// </summary>
        /// <param name="text">The string to normalize.</param>
        /// <returns>Normalized string without accents.</returns>
        private static string RemoveDiacritics(string text)
        {
            if (string.IsNullOrEmpty(text)) return text;

            var normalizedString = text.Normalize(NormalizationForm.FormD);
            var stringBuilder = new StringBuilder(capacity: normalizedString.Length);

            for (int i = 0; i < normalizedString.Length; i++)
            {
                char c = normalizedString[i];
                var unicodeCategory = CharUnicodeInfo.GetUnicodeCategory(c);
                if (unicodeCategory != UnicodeCategory.NonSpacingMark)
                {
                    stringBuilder.Append(c);
                }
            }

            return stringBuilder
                .ToString()
                .Normalize(NormalizationForm.FormC);
        }
    }
}
