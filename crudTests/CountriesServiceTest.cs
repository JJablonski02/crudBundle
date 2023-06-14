﻿using ServiceContracts;
using ServiceContracts.DTO;
using Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace crudTests
{
    public class CountriesServiceTest
    {

        private readonly ICountriesService _countriesService;

        //constructor
        public CountriesServiceTest()
        {
            _countriesService = new CountriesService();
        }

        #region AddCountry
        //When CountryAddRequest is null, should throw ArgumentNullException

        [Fact]
        public void AddCountry_NullCountry()
        {
            //Arrange
            CountryAddRequest? request = null;

            //Assert
            Assert.Throws<ArgumentNullException>(() =>
            {
                //Act
                _countriesService.AddCountry(request);
            });
        }

        //When the CountryName is null, it should throw ArgumentException

        [Fact]
        public void AddCountry_CountryNameIsNull()
        {
            //Arrange
            CountryAddRequest? request = new CountryAddRequest() 
            {
                CountryName = null 
            };

            //Assert
            Assert.Throws<ArgumentNullException>(() =>
            {
            //Act
            _countriesService.AddCountry(request);
            });
        }

        //When the CountryName is duplicate, it should throw ArgumentException

        [Fact]
        public void AddCountry_DuplicateCountryName()
        {
            //Arrange
            CountryAddRequest? request1 = new CountryAddRequest
            { CountryName = "USA" };
            CountryAddRequest? request2 = new CountryAddRequest
            { CountryName = "USA" };


            //Assert
            Assert.Throws<ArgumentException>(() =>
            { 
            //Act
            _countriesService.AddCountry(request1);
            _countriesService.AddCountry(request2);
            });
        }

        //When you supply proper CountryName, it should insert (add) the country to the existing list of countries

        [Fact]
        public void AddCountry_PropertyCountryDetails()
        {
            //Arrange
            CountryAddRequest? request = new CountryAddRequest()
            {
                CountryName = "Japan"
            };
                //Act
               CountryResponse response = _countriesService.AddCountry(request);
                List<CountryResponse> countries_from_GetAllCountries = _countriesService.GetAllCountries();

                //Assert
                Assert.True(response.CountryID != Guid.Empty);
                Assert.Contains(response, countries_from_GetAllCountries);
           
        }
      
        #endregion

        #region GetAllCountries

        [Fact]
        //The list of countries should be empty by default (before adding any countries)
        public void GetAllCountries_EmptyList()
        {
            //Acts
            List<CountryResponse> actual_country_response_list = _countriesService.GetAllCountries();

            //Assert
            Assert.Empty(actual_country_response_list);
        }

        [Fact]
        public void GetAllCountries_AddFewCountries()
        {
            //Arrange
            List<CountryAddRequest> country_request_list =
                new List<CountryAddRequest>() {
                    new CountryAddRequest() { CountryName = "USA"},
                    new CountryAddRequest() { CountryName = "UK" } 
                };
            //Act 
            List<CountryResponse> countries_list_from_add_country = new List<CountryResponse>();

            foreach(CountryAddRequest country_request in country_request_list)
            {
                countries_list_from_add_country.Add(_countriesService.AddCountry(country_request));
            }

            List<CountryResponse> actualCountryResponseList = _countriesService.GetAllCountries();

            //read each element froum countries_list_from_add_country
            foreach(CountryResponse expectedCountry in countries_list_from_add_country)
            {
                Assert.Contains(expectedCountry, actualCountryResponseList);
            }
        }
        #endregion
    }
}
