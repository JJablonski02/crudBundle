using Entities;
using ServiceContracts;
using ServiceContracts.DTO;

namespace Services
{
    public class CountriesService : ICountriesService
    {
        private readonly List<Country> _countries;

        //constructor
        public CountriesService(bool initialize = true)
        {
            _countries= new List<Country>();
            if (initialize)
            {
                _countries.AddRange(new List<Country>()
                {
                new Country() { CountryID = Guid.Parse("A6771060-810D-477B-BF72-68DB2FEC6BE7"), CountryName = "Pakistan" },
                new Country() { CountryID = Guid.Parse("B852AC49-E73A-484F-9D4B-DF16174D600C"), CountryName = "Uzbekistan" },
                new Country() { CountryID = Guid.Parse("5CA2AFDE-5418-4DF7-88A7-CD06CFDDC225"), CountryName = "Kamerun" },
                new Country() { CountryID = Guid.Parse("07CCF5FF-74FC-4A31-9A6F-A169D26CCD95"), CountryName = "Laos" },
                new Country() { CountryID = Guid.Parse("B1569B9D-A29D-4982-B2C4-C8A007A67D94"), CountryName = "Sri Lanka" },
                });
            }
        }
        public CountryResponse AddCountry(CountryAddRequest? countryAddRequest)
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
            if (_countries.Where(temp => temp.CountryName == countryAddRequest.CountryName).Count() > 0)
            {
                throw new ArgumentException("Given country name already exists");
            }

            //Convert object from CountryAddRequest to Country type
            Country country = countryAddRequest.ToCountry(); 

            //generate guid CountryID
            country.CountryID = Guid.NewGuid();

            //Add country object into _countries
            _countries.Add(country);

            return country.ToCountryResponse();
        }

        public List<CountryResponse> GetAllCountries()
        {
            return _countries.Select(country => country.ToCountryResponse()).ToList();
        }

        public CountryResponse? GetCountryByCountryID(Guid? countryID)
        {
            if(countryID == null)
                return null;

            Country? country_response_from_list = _countries.FirstOrDefault(temp => temp.CountryID == countryID);

            if (country_response_from_list == null)
                return null;

            return country_response_from_list.ToCountryResponse();
        }
    }
}