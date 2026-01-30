using Entities;

namespace RepositoryContracts;

/// <summary>
/// Represents a repository interface for managing and performing operations related to countries.
/// </summary>
public interface ICountriesRepository
{
    /// <summary>
    /// Adds a new country to the repository.
    /// </summary>
    /// <param name="country">The Country object to be added to the repository.</param>
    /// <returns>A task that represents the asynchronous operation. The task result contains the added Country object.</returns>
    Task<Country> AddCountry(Country country);


    /// <summary>
    /// Retrieves all countries from the repository.
    /// </summary>
    /// <returns>A task that represents the asynchronous operation. The task result contains a list of Country objects.</returns>
    Task<List<Country>> GetAllCountries();

    /// <summary>
    /// Retrieves a country from the repository based on the specified country ID. otherwise return null
    /// </summary>
    /// <param name="countryID">The ID of the country to be retrieved.</param>
    /// <returns>A task that represents the asynchronous operation. The task result contains the Country object if found, otherwise null.</returns>
    Task<Country?> GetCountryByCountryID(Guid countryID);

    /// <summary>
    /// Retrieves a country from the repository based on the specified country name. If no match is found, returns null.
    /// </summary>
    /// <param name="countryName">The name of the country to be retrieved.</param>
    /// <returns>A task that represents the asynchronous operation. The task result contains the Country object if found, otherwise null.</returns>
    Task<Country?> GetCountryByCountryName(string countryName);
    
    
    
}