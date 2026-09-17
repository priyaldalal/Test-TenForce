using System.Collections.Generic;
using System.Collections.ObjectModel;
using Newtonsoft.Json;
using Test_Taste_Console_Application.Constants;
using Test_Taste_Console_Application.Domain.DataTransferObjects;
using Test_Taste_Console_Application.Domain.DataTransferObjects.JsonObjects;
using Test_Taste_Console_Application.Domain.Objects;
using Test_Taste_Console_Application.Domain.Services.Interfaces;
using Test_Taste_Console_Application.Utilities;
using System;
using System.Linq;

namespace Test_Taste_Console_Application.Domain.Services
{
    /// <inheritdoc />
    public class MoonService : IMoonService
    {
        private readonly HttpClientService _httpClientService;
        private IEnumerable<Moon> _cachedMoons;


        public MoonService(HttpClientService httpClientService)
        {
            _httpClientService = httpClientService;
        }

        public IEnumerable<Moon> GetAllMoons()
        {
            if (_cachedMoons != null && _cachedMoons.Any())
            {
                return _cachedMoons;
            }

            Console.WriteLine("Loading moons data from API");
            var allMoons = new Collection<Moon>();

            try
            {
                var response = _httpClientService.Client
                    .GetAsync(UriPath.GetAllMoonsWithMassQueryParameters)
                    .Result;

                //If the status code isn't 200-299, then the function returns an empty collection.
                if (!response.IsSuccessStatusCode)
                {
                    Logger.Instance.Warn($"{LoggerMessage.GetRequestFailed}{response.StatusCode}");
                    Console.WriteLine($"{LoggerMessage.GetRequestFailed}{response.StatusCode}");
                    return allMoons;
                }

                var content = response.Content.ReadAsStringAsync().Result;
                
                //The JSON converter uses DTO's, that can be found in the DataTransferObjects folder, to deserialize the response content.
                //The JSON converter uses DTO's to deserialize response content.
                var results = JsonConvert.DeserializeObject<JsonResult<MoonDto>>(content);

                if (results == null || results.Bodies == null) 
                    return allMoons;

                foreach (MoonDto moonDto in results.Bodies)
                {
                    allMoons.Add(new Moon(moonDto));
                }
            }
            catch (Exception ex)
            {
                Logger.Instance.Error($"Error loading moons: {ex.Message}");
                Console.WriteLine($"Error loading moons: {ex.Message}");
            }

            _cachedMoons = allMoons;
            return allMoons;
        }
    }
}
