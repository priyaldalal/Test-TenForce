using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Test_Taste_Console_Application.Constants;
using Test_Taste_Console_Application.Domain.DataTransferObjects;
using Test_Taste_Console_Application.Domain.DataTransferObjects.JsonObjects;
using Test_Taste_Console_Application.Domain.Objects;
using Test_Taste_Console_Application.Domain.Services.Interfaces;
using Test_Taste_Console_Application.Utilities;

namespace Test_Taste_Console_Application.Domain.Services
{
    /// <summary>
    /// Service responsible for fetching planet and moon data from the Solar System OpenData REST API.
    /// Provides in-memory caching and high-performance concurrent processing of moon details.
    /// </summary>
    public class PlanetService : IPlanetService
    {
        private readonly HttpClientService _httpClientService;
        private IEnumerable<Planet> _cachedPlanets;

        /// <summary>
        /// Initializes a new instance of the <see cref="PlanetService"/> class.
        /// </summary>
        /// <param name="httpClientService">The HTTP client service instance configured with Bearer authentication.</param>
        public PlanetService(HttpClientService httpClientService)
        {
            _httpClientService = httpClientService ?? throw new ArgumentNullException(nameof(httpClientService));
        }

        /// <summary>
        /// Retrieves all planet bodies and their associated moon objects from the Solar System OpenData API.
        /// Returns cached data on subsequent calls to eliminate redundant network requests.
        /// </summary>
        /// <returns>A collection of <see cref="Planet"/> domain objects containing populated moon data.</returns>
        public IEnumerable<Planet> GetAllPlanets()
        {
            // Return cached dataset if available
            if (_cachedPlanets != null && _cachedPlanets.Any())
            {
                return _cachedPlanets;
            }

            Console.WriteLine("Loading planets data from API...");
            var allPlanetsWithTheirMoons = new Collection<Planet>();

            try
            {
                // Execute HTTP GET request to retrieve all planet bodies
                var response = _httpClientService.Client
                    .GetAsync(UriPath.GetAllPlanetsWithMoonsQueryParameters)
                    .Result;

                // Validate HTTP status code
                if (!response.IsSuccessStatusCode)
                {
                    Logger.Instance.Warn($"{LoggerMessage.GetRequestFailed}{response.StatusCode}");
                    Console.WriteLine($"{LoggerMessage.GetRequestFailed}{response.StatusCode}");
                    return allPlanetsWithTheirMoons;
                }

                // Read JSON response payload
                var content = response.Content.ReadAsStringAsync().Result;

                // Deserialize JSON payload into DTO objects
                var results = JsonConvert.DeserializeObject<JsonResult<PlanetDto>>(content);
                if (results == null || results.Bodies == null)
                {
                    Logger.Instance.Warn("API returned null response or empty bodies list.");
                    return allPlanetsWithTheirMoons;
                }

                // Iterate through returned planet DTOs and populate moon details
                foreach (var planetDto in results.Bodies)
                {
                    if (planetDto.Moons != null && planetDto.Moons.Any())
                    {
                        Console.WriteLine($"Loading details for {planetDto.Moons.Count} moon(s) of planet '{planetDto.Id}'...");
                        var newMoonsCollection = new Collection<MoonDto>();

                        // Concurrently fetch full details for each moon to optimize performance
                        var moonTasks = planetDto.Moons.Select(async moon =>
                        {
                            try
                            {
                                var moonResponse = await _httpClientService.Client
                                    .GetAsync(UriPath.GetMoonByIdQueryParameters + moon.URLId);

                                if (moonResponse.IsSuccessStatusCode)
                                {
                                    var moonContent = await moonResponse.Content.ReadAsStringAsync();
                                    return JsonConvert.DeserializeObject<MoonDto>(moonContent);
                                }
                                else
                                {
                                    Logger.Instance.Warn($"Moon request for {moon.URLId} failed with status {moonResponse.StatusCode}");
                                }
                            }
                            catch (Exception ex)
                            {
                                Logger.Instance.Error($"Error fetching moon data for {moon.URLId}: {ex.Message}", ex);
                            }
                            return null;
                        });

                        // Await completion of all concurrent moon requests
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

                    // Convert DTO to domain object and add to results collection
                    allPlanetsWithTheirMoons.Add(new Planet(planetDto));
                }
            }
            catch (Exception ex)
            {
                Logger.Instance.Error($"Error loading planets: {ex.Message}", ex);
                Console.WriteLine($"Error loading planets: {ex.Message}");
            }

            // Cache retrieved dataset for subsequent calls
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
