using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
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
    /// Data Access Repository for retrieving Moon DTOs directly from the Solar System REST API.
    /// </summary>
    public class MoonRepository : IMoonRepository
    {
        private readonly HttpClientService _httpClientService;

        /// <summary>
        /// Initializes a new instance of the <see cref="MoonRepository"/> class.
        /// </summary>
        /// <param name="httpClientService">Configured HTTP client service.</param>
        public MoonRepository(HttpClientService httpClientService)
        {
            _httpClientService = httpClientService ?? throw new ArgumentNullException(nameof(httpClientService));
        }

        /// <inheritdoc />
        public IEnumerable<MoonDto> GetMoonDtosWithMass()
        {
            try
            {
                var response = _httpClientService.Client
                    .GetAsync(UriPath.GetAllMoonsWithMassQueryParameters)
                    .Result;

                if (!response.IsSuccessStatusCode)
                {
                    Logger.Instance.Warn($"{LoggerMessage.GetRequestFailed}{response.StatusCode}");
                    Console.WriteLine($"{LoggerMessage.GetRequestFailed}{response.StatusCode}");
                    return new Collection<MoonDto>();
                }

                var content = response.Content.ReadAsStringAsync().Result;
                var results = JsonConvert.DeserializeObject<JsonResult<MoonDto>>(content);

                return results?.Bodies ?? new Collection<MoonDto>();
            }
            catch (Exception ex)
            {
                Logger.Instance.Error($"Error in MoonRepository.GetMoonDtosWithMass: {ex.Message}", ex);
                Console.WriteLine($"Error fetching moon mass data: {ex.Message}");
                return new Collection<MoonDto>();
            }
        }
    }
}
