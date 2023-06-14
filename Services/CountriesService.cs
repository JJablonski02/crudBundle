﻿using Entities;
using ServiceContracts;
using ServiceContracts.DTO;

namespace Services
{
    public class CountriesService : ICountriesService
    {
        private readonly List<Country> _countries;

        //constructor
        public CountriesService()
        {
            _countries= new List<Country>();
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
    }
}