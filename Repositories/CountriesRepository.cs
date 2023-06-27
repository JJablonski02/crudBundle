using Entities;
using Microsoft.EntityFrameworkCore;
using RepositoryContracts;

namespace Repositories
{
    public class CountriesRepository : RepositoryContracts.ICountriesRepository
    {
        private readonly Entities.ApplicationDbContext _DbContext;
        public CountriesRepository(Entities.ApplicationDbContext applicationDbContext)
        {
            _DbContext = applicationDbContext;
        }

        public async Task<Country> AddCountry(Country country)
        {
            _DbContext.Countries.Add(country);
            await _DbContext.SaveChangesAsync();

            return country;
        }

        public async Task<List<Country>> GetAllCountries()
        {
            return await _DbContext.Countries.ToListAsync();
        }

        public async Task<Country?> GetCountryByCountryID(Guid countryID)
        {
            return await _DbContext.Countries.FirstOrDefaultAsync(temp => temp.CountryID == countryID);
        }

        public async Task<Country?> GetCountryByCountryName(string countryName)
        {
            return await _DbContext.Countries.FirstOrDefaultAsync(temp => temp.CountryName == countryName);
        }
    }
}