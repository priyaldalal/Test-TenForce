using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using Test_Taste_Console_Application.Domain.DataTransferObjects;
using Test_Taste_Console_Application.Domain.Objects;
using Test_Taste_Console_Application.Domain.Repositories.Interfaces;
using Test_Taste_Console_Application.Domain.Services.Interfaces;
using Test_Taste_Console_Application.Utilities;

namespace Test_Taste_Console_Application.Domain.Services
{
    /// <summary>
    /// Business Service layer for managing and processing Moon domain objects.
    /// Maps Moon DTOs into domain objects and caches datasets in memory.
    /// </summary>
    public class MoonService : IMoonService
    {
        private readonly IMoonRepository _moonRepository;
        private IEnumerable<Moon> _cachedMoons;

        /// <summary>
        /// Initializes a new instance of the <see cref="MoonService"/> class.
        /// </summary>
        /// <param name="moonRepository">The moon data access repository.</param>
        public MoonService(IMoonRepository moonRepository)
        {
            _moonRepository = moonRepository ?? throw new ArgumentNullException(nameof(moonRepository));
        }

        /// <summary>
        /// Retrieves all moons and their mass values from the data access repository.
        /// Returns cached dataset on subsequent calls for high performance.
        /// </summary>
        /// <returns>Collection of <see cref="Moon"/> domain objects.</returns>
        public IEnumerable<Moon> GetAllMoons()
        {
            // Return cached dataset if already loaded
            if (_cachedMoons != null && _cachedMoons.Any())
            {
                return _cachedMoons;
            }

            Console.WriteLine("Loading moons data from API...");
            var allMoons = new Collection<Moon>();

            try
            {
                // Fetch DTOs from repository (Data Access Layer)
                var moonDtos = _moonRepository.GetMoonDtosWithMass();

                foreach (MoonDto moonDto in moonDtos)
                {
                    // Convert DTO into domain object
                    allMoons.Add(new Moon(moonDto));
                }
            }
            catch (Exception ex)
            {
                Logger.Instance.Error($"Error in MoonService.GetAllMoons: {ex.Message}", ex);
                Console.WriteLine($"Error processing moons data: {ex.Message}");
            }

            // Cache dataset in memory
            _cachedMoons = allMoons;
            return allMoons;
        }
    }
}
