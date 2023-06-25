using Entities;
using Microsoft.EntityFrameworkCore;
using ServiceContracts;
using ServiceContracts.DTO;

namespace Services
{
    public class CountriesService : ICountriesService
    {
        private readonly PersonsDbContext _dbContext;

        //constructor
        public CountriesService(PersonsDbContext personsDbContext)
        {
            _dbContext = personsDbContext;
        }
        public async Task<CountryResponse> AddCountry(CountryAddRequest? countryAddRequest)
        {

            //Validation: CountryAddRequest parameter cannot be null
            if (countryAddRequest == null)
            {
                throw new ArgumentNullException(nameof(countryAddRequest));
            }
            //Validation: CountryName cannot be null
            if (countryAddRequest.CountryName == null)
            {
                throw new ArgumentNullException(nameof(countryAddRequest.CountryName));

            }
            //Validation: CountryName canot be duplicate
            if (await _dbContext.Countries.CountAsync(temp => temp.CountryName == countryAddRequest.CountryName) > 0)
            {
                throw new ArgumentException("Given country name already exists");
            }

            //Convert object from CountryAddRequest to Country type
            Country country = countryAddRequest.ToCountry();

            //generate guid CountryID
            country.CountryID = Guid.NewGuid();

            //Add country object into _countries
            _dbContext.Countries.Add(country);
            await _dbContext.SaveChangesAsync();

            return country.ToCountryResponse();
        }

        public async Task<List<CountryResponse>> GetAllCountries()
        {
            return await _dbContext.Countries.Select(country => country.ToCountryResponse()).ToListAsync();
        }

        public async Task<CountryResponse?> GetCountryByCountryID(Guid? countryID)
        {
            if (countryID == null)
                return null;

            Country? country_response_from_list = await _dbContext.Countries.FirstOrDefaultAsync(temp => temp.CountryID == countryID);

            if (country_response_from_list == null)
                return null;

            return country_response_from_list.ToCountryResponse();
        }
    }
}