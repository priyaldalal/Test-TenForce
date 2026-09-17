using System.Collections.Generic;
using Test_Taste_Console_Application.Domain.DataTransferObjects;

namespace Test_Taste_Console_Application.Domain.Repositories.Interfaces
{
    /// <summary>
    /// Data Access Repository interface for fetching moon mass DTO data from the Solar System API.
    /// </summary>
    public interface IMoonRepository
    {
        /// <summary>
        /// Fetches moon DTOs containing mass data from the public API.
        /// </summary>
        /// <returns>Collection of <see cref="MoonDto"/>.</returns>
        IEnumerable<MoonDto> GetMoonDtosWithMass();
    }
}
