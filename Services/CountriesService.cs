using Entities;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using OfficeOpenXml;
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

        public async Task<int> UploadFromCountriesFromExcelFile(IFormFile formFile)
        {
            MemoryStream memoryStream = new MemoryStream();
            await formFile.CopyToAsync(memoryStream);
            int countryInserted = 0;

            using (ExcelPackage excelPackage = new ExcelPackage(memoryStream))
            {
                ExcelWorksheet workSheet = excelPackage.Workbook.Worksheets["Countries"];

                int rowCount = workSheet.Dimension.Rows;

                for (int row = 2; row <= rowCount; row++)
                {
                    string? cellValue = Convert.ToString(workSheet.Cells[row, 1].Value);

                    if(!string.IsNullOrEmpty(cellValue))
                    {
                        string? countryName = cellValue;

                        if (_dbContext.Countries.Where(temp => temp.CountryName == countryName).Count() > 0)
                        {
                            Country country = new Country() { CountryName = countryName };
                            await _dbContext.SaveChangesAsync();

                            countryInserted++;
                        }
                    }
                }
            }
            return countryInserted;
        }
    }
}