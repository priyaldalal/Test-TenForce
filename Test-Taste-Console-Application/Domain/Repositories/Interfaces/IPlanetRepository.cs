using System.Collections.Generic;
using System.Threading.Tasks;
using Test_Taste_Console_Application.Domain.DataTransferObjects;

namespace Test_Taste_Console_Application.Domain.Repositories.Interfaces
{
    /// <summary>
    /// Data Access Repository interface for fetching raw planet and moon DTO data from the Solar System API.
    /// </summary>
    public interface IPlanetRepository
    {
        /// <summary>
        /// Fetches planet DTOs with moon references from the public API.
        /// </summary>
        /// <returns>Collection of <see cref="PlanetDto"/>.</returns>
        IEnumerable<PlanetDto> GetPlanetDtos();

        /// <summary>
        /// Asynchronously fetches full details for a specific moon by its ID.
        /// </summary>
        /// <param name="moonUrlId">The URL identifier for the moon.</param>
        /// <returns>Task returning <see cref="MoonDto"/>.</returns>
        Task<MoonDto> GetMoonDtoByIdAsync(string moonUrlId);
    }
}
