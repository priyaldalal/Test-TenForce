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
    /// <inheritdoc />
    public class PlanetService : IPlanetService
    {
        private readonly HttpClientService _httpClientService;
        private IEnumerable<Planet> _cachedPlanets;

        public PlanetService(HttpClientService httpClientService)
        {
            _httpClientService = httpClientService;
        }

        public IEnumerable<Planet> GetAllPlanets()
        {
            if (_cachedPlanets != null && _cachedPlanets.Any()){
                    return _cachedPlanets;
            }

            Console.WriteLine("Loading planets data from API...");
            var allPlanetsWithTheirMoons = new Collection<Planet>();

            try
            {
                var response = _httpClientService.Client
                    .GetAsync(UriPath.GetAllPlanetsWithMoonsQueryParameters)
                    .Result;

                //If the status code isn't 200-299, then the function returns an empty collection.
                if (!response.IsSuccessStatusCode)
                {
                    Logger.Instance.Warn($"{LoggerMessage.GetRequestFailed}{response.StatusCode}");
                    
                    
                    Console.WriteLine($"{LoggerMessage.GetRequestFailed}{response.StatusCode}");
                    return allPlanetsWithTheirMoons;
                }

                var content = response.Content.ReadAsStringAsync().Result;

                //The JSON converter uses DTO's to deserialize the response content.
                var results = JsonConvert.DeserializeObject<JsonResult<PlanetDto>>(content);

                if (results == null || results.Bodies == null) return allPlanetsWithTheirMoons;

                //Process planets and fetch details for their moons concurrently for high performance
                foreach (var planet in results.Bodies)
                {
                    if (planet.Moons != null && planet.Moons.Any())
                    {
                        Console.WriteLine($"Loading details for {planet.Moons.Count} moon(s) of planet '{planet.Id}'...");
                        var newMoonsCollection = new Collection<MoonDto>();

                        var moonTasks = planet.Moons.Select(async moon =>
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
                            }
                            catch (Exception ex)
                            {
                                Logger.Instance.Error($"Failed to fetch moon data for {moon.URLId}: {ex.Message}");
                            }
                            return null;
                        });

                        var fetchedMoons = Task.WhenAll(moonTasks).Result;
                        foreach (var moonDto in fetchedMoons)
                        {
                            if (moonDto != null)
                            {
                                newMoonsCollection.Add(moonDto);
                            }
                        }
                        planet.Moons = newMoonsCollection;
                    }
                    allPlanetsWithTheirMoons.Add(new Planet(planet));
                }
            }
            catch (Exception ex)
            {
                Logger.Instance.Error($"Error loading planets: {ex.Message}");
                Console.WriteLine($"Error loading planets: {ex.Message}");
            }

            _cachedPlanets = allPlanetsWithTheirMoons;
            return allPlanetsWithTheirMoons;
        }

        private static string RemoveDiacritics(string text)
        {
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
