using Microsoft.AspNetCore.Http;
using ServiceContracts.DTO;

namespace ServiceContracts
{
    /// <summary>
    /// Represents business logic for manipulation Country entity
    /// </summary>
    public interface ICountriesService
    {
        /// <summary>
        /// Adds a country object to the list of countries
        /// </summary>
        /// <param name="countryAddRequest">Country
        /// object to add</param>
        /// <returns>Return the country object after adding it (including newly generated id) </returns>
        Task<CountryResponse> AddCountry(CountryAddRequest? countryAddRequest);

        /// <summary>
        /// Returns all countries from the list
        /// </summary>
        /// <returns>Returns all countries from the list as List of CountryResponse</returns>
        Task<List<CountryResponse>> GetAllCountries();

        /// <summary>
        /// Returns a country object based on the given id
        /// </summary>
        /// <param name="countryID">CountryID (guid) to search</param>
        /// <returns>Matching country as CountryResponse</returns>
        Task<CountryResponse?> GetCountryByCountryID(Guid? countryID);

        /// <summary>
        /// Uploads countries from excel file into database
        /// </summary>
        /// <param name="fomrFile"></param>
        /// <returns>Returns the country object after adding it (including newly generated </returns>
        Task<int> UploadCountriesFromExcelFile(IFormFile formFile);
    }
}
