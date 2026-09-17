using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Test_Taste_Console_Application.Constants;
using Test_Taste_Console_Application.Domain.DataTransferObjects;
using Test_Taste_Console_Application.Domain.DataTransferObjects.JsonObjects;
using Test_Taste_Console_Application.Domain.Repositories.Interfaces;
using Test_Taste_Console_Application.Domain.Services;
using Test_Taste_Console_Application.Utilities;

namespace Test_Taste_Console_Application.Domain.Repositories
{
    /// <summary>
    /// Data Access Repository for retrieving Planet and Moon DTOs directly from the Solar System REST API.
    /// Encapsulates all raw HTTP requests and JSON deserialization logic.
    /// </summary>
    public class PlanetRepository : IPlanetRepository
    {
        private readonly HttpClientService _httpClientService;

        /// <summary>
        /// Initializes a new instance of the <see cref="PlanetRepository"/> class.
        /// </summary>
        /// <param name="httpClientService">Configured HTTP client service.</param>
        public PlanetRepository(HttpClientService httpClientService)
        {
            _httpClientService = httpClientService ?? throw new ArgumentNullException(nameof(httpClientService));
        }

        /// <inheritdoc />
        public IEnumerable<PlanetDto> GetPlanetDtos()
        {
            try
            {
                var response = _httpClientService.Client
                    .GetAsync(UriPath.GetAllPlanetsWithMoonsQueryParameters)
                    .Result;

                if (!response.IsSuccessStatusCode)
                {
                    Logger.Instance.Warn($"{LoggerMessage.GetRequestFailed}{response.StatusCode}");
                    Console.WriteLine($"{LoggerMessage.GetRequestFailed}{response.StatusCode}");
                    return new Collection<PlanetDto>();
                }

                var content = response.Content.ReadAsStringAsync().Result;
                var results = JsonConvert.DeserializeObject<JsonResult<PlanetDto>>(content);

                return results?.Bodies ?? new Collection<PlanetDto>();
            }
            catch (Exception ex)
            {
                Logger.Instance.Error($"Error in PlanetRepository.GetPlanetDtos: {ex.Message}", ex);
                Console.WriteLine($"Error fetching planet data: {ex.Message}");
                return new Collection<PlanetDto>();
            }
        }

        /// <inheritdoc />
        public async Task<MoonDto> GetMoonDtoByIdAsync(string moonUrlId)
        {
            try
            {
                var moonResponse = await _httpClientService.Client
                    .GetAsync(UriPath.GetMoonByIdQueryParameters + moonUrlId);

                if (moonResponse.IsSuccessStatusCode)
                {
                    var moonContent = await moonResponse.Content.ReadAsStringAsync();
                    return JsonConvert.DeserializeObject<MoonDto>(moonContent);
                }
                else
                {
                    Logger.Instance.Warn($"Moon request for {moonUrlId} returned status {moonResponse.StatusCode}");
                }
            }
            catch (Exception ex)
            {
                Logger.Instance.Error($"Error fetching moon DTO for {moonUrlId}: {ex.Message}", ex);
            }
            return null;
        }
    }
}
