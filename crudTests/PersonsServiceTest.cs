using Entities;
using ServiceContracts;
using ServiceContracts.DTO;
using ServiceContracts.Enums;
using Services;
using System;
using System.Net;
using System.Reflection;
using Xunit;
using Xunit.Abstractions;

namespace crudTests
{
    public class PersonsServiceTest
    {
        private readonly IPersonsService _personService;
        private readonly ICountriesService _countriesService;
        private readonly ITestOutputHelper _testOutputHelper;
        public PersonsServiceTest(ITestOutputHelper testOutputHelper)
        {
            _personService = new PersonsService();
            _countriesService = new CountriesService();
            _testOutputHelper = testOutputHelper;
        }
        #region AddPerson

        //When we supply null value as PersonAddRequest, it should throw ArgumentNullException
        [Fact]
        public void AddPerson_NullPerson()
        {
            //Arrange
            PersonAddRequest? personAddRequest = null;

            //Act
            Assert.Throws<ArgumentNullException>(() => _personService.AddPerson(personAddRequest));
            
        }

        //When we supply null value as PersonName, it should throw ArgumentNullException
        [Fact]
        public void AddPerson_PersonNameIsNull()
        {
            //Arrange
            PersonAddRequest? personAddRequest = new PersonAddRequest() { PersonName = null };

            //Act
            Assert.Throws<ArgumentException>(() => _personService.AddPerson(personAddRequest));

        }

        //When we supply proper person details, it shpuld insert the person into the persons list;
        //and it should return an object of PersonResponse, which includes with the newly generated person_id

        [Fact]
        public void AddPerson_ProperPersonDetails()
        {
            //Arrange
            PersonAddRequest? personAddRequest = new PersonAddRequest() { PersonName = "Tom" , 
                Email = "tom@example.com", 
                Address = "sample address", 
                CountryID = Guid.NewGuid(), 
                Gender=ServiceContracts.Enums.GenderOptions.Male,
                DateOfBirth = DateTime.Parse("2002-05-28")};

            //Act
            PersonResponse person_response_from_add = _personService.AddPerson(personAddRequest);
            List<PersonResponse> persons_list = _personService.GetAllPersons();

            //Asert
            Assert.True(person_response_from_add.PersonID != Guid.Empty);
            Assert.Contains(person_response_from_add, persons_list);

        }
        #endregion

        #region GetPersonByPersonID

        //If we supply null as PersonID, it should return as PersonResponse
        [Fact]
        public void GetPersonByPersonID_NullPersonID()
        {
            //Arrange
            Guid? PersonID = null;

            //Act
            PersonResponse? person_response_from_get = _personService.GetPersonByPersonID(PersonID);

            //Assert
            Assert.Null(person_response_from_get);
        }

        //If we supply a valid person id, it should return valid person details as PersonResponse object
        [Fact]
        public void GetPersonByPersonID_WithPersonID()
        {
            //Arrange

            CountryAddRequest country_request = new CountryAddRequest()
            {
                CountryName = "Canada"
            };
            CountryResponse country_response = _countriesService.AddCountry(country_request);

            //Act
            PersonAddRequest person_request = new PersonAddRequest()
            {
                PersonName = "person name...",
                Email = "example@example.com",
                Address = "address",
                CountryID = country_response.CountryID,
                DateOfBirth = DateTime.Parse("2002-02-02"),
                Gender = GenderOptions.Male,
                ReceiveNewsLetters = false
            };
            PersonResponse person_response_from_add = _personService.AddPerson(person_request);

            PersonResponse? person_response_from_get 
                = _personService.GetPersonByPersonID(person_response_from_add.PersonID);
            //Assert
            Assert.Equal(person_response_from_add, person_response_from_get);
        }
        #endregion

        #region GetAllPersons

        //The GetAllPersons() should return an empty list by default
        [Fact]
        public void GetAllPersons()
        {
            //Act
            List<PersonResponse> person_from_get = _personService.GetAllPersons();

            //Assert
            Assert.Empty(person_from_get);
            
        }
        //First, we will add few perons; and then we call GetAllPersons(), it should return the same persons that were added
        [Fact]
        public void GetAllPersons_AddFewPersons()
        {
            CountryAddRequest country_request_1 = new CountryAddRequest()
            {
                CountryName = "Usa"
            };
            CountryAddRequest country_request_2 = new CountryAddRequest()
            {
                CountryName = "Great Britain"
            };

            CountryResponse country_response_1 = _countriesService.AddCountry(country_request_1);
            CountryResponse country_response_2 = _countriesService.AddCountry(country_request_2);

            PersonAddRequest person_request_1 = new PersonAddRequest()
            {
                PersonName = "Sam",
                Email = "sam@example.pl",
                Address = "SomeAddress",
                Gender = GenderOptions.Male,
                CountryID = country_response_1.CountryID,
                DateOfBirth = DateTime.Parse("2002/05/06"),
                ReceiveNewsLetters = true,
            };
            PersonAddRequest person_request_2 = new PersonAddRequest()
            {
                PersonName = "Max",
                Email = "Max@example.pl",
                Address = "SomeAddress2",
                Gender = GenderOptions.Male,
                CountryID = country_response_2.CountryID,
                DateOfBirth = DateTime.Parse("2003/05/06"),
                ReceiveNewsLetters = false,
            };
            PersonAddRequest person_request_3 = new PersonAddRequest()
            {
                PersonName = "Loi",
                Email = "Loi@example.pl",
                Address = "SomeAddress3",
                Gender = GenderOptions.Male,
                CountryID = country_response_2.CountryID,
                DateOfBirth = DateTime.Parse("1999/05/06"),
                ReceiveNewsLetters = true,
            };
            List<PersonAddRequest> person_requests = new List<PersonAddRequest>() { person_request_1, person_request_2, person_request_3 };
            List<PersonResponse> person_response_from_list_from_add = new List<PersonResponse>();

            foreach (PersonAddRequest person_request in person_requests)
            {
                PersonResponse person_response = _personService.AddPerson(person_request);
                person_response_from_list_from_add.Add(person_response);
            }
            //print person_response_list_from_add
            _testOutputHelper.WriteLine("Expected");
            foreach(PersonResponse person_response_from_add in person_response_from_list_from_add)
            {
                _testOutputHelper.WriteLine(person_response_from_add.ToString());
            }
            //Act
            List<PersonResponse> persons_list_from_get = _personService.GetAllPersons();

            //print persons_list_from_get
            _testOutputHelper.WriteLine("Actual");
            foreach (PersonResponse person_response_from_get in persons_list_from_get)
            {
                _testOutputHelper.WriteLine(person_response_from_get.ToString());
            }

            foreach (PersonResponse person_response_from_add in person_response_from_list_from_add)
            {
                Assert.Contains(person_response_from_add, persons_list_from_get);
            }
        }

        #endregion

        #region GetFilteredPersons

        [Fact]
        public void GetFilteredPersons_EmptySearchText()
        {
            CountryAddRequest country_request_1 = new CountryAddRequest()
            {
                CountryName = "Usa"
            };
            CountryAddRequest country_request_2 = new CountryAddRequest()
            {
                CountryName = "Great Britain"
            };

            CountryResponse country_response_1 = _countriesService.AddCountry(country_request_1);
            CountryResponse country_response_2 = _countriesService.AddCountry(country_request_2);

            PersonAddRequest person_request_1 = new PersonAddRequest()
            {
                PersonName = "Sam",
                Email = "sam@example.pl",
                Address = "SomeAddress",
                Gender = GenderOptions.Male,
                CountryID = country_response_1.CountryID,
                DateOfBirth = DateTime.Parse("2002/05/06"),
                ReceiveNewsLetters = true,
            };
            PersonAddRequest person_request_2 = new PersonAddRequest()
            {
                PersonName = "Max",
                Email = "Max@example.pl",
                Address = "SomeAddress2",
                Gender = GenderOptions.Male,
                CountryID = country_response_2.CountryID,
                DateOfBirth = DateTime.Parse("2003/05/06"),
                ReceiveNewsLetters = false,
            };
            PersonAddRequest person_request_3 = new PersonAddRequest()
            {
                PersonName = "Lamar",
                Email = "Lamar@example.pl",
                Address = "SomeAddress3",
                Gender = GenderOptions.Male,
                CountryID = country_response_2.CountryID,
                DateOfBirth = DateTime.Parse("1999/05/06"),
                ReceiveNewsLetters = true,
            };
            List<PersonAddRequest> person_requests = new List<PersonAddRequest>() { person_request_1, person_request_2, person_request_3 };
            List<PersonResponse> person_response_list_from_add = new List<PersonResponse>();

            foreach (PersonAddRequest person_request in person_requests)
            {
                PersonResponse person_response = _personService.AddPerson(person_request);
                person_response_list_from_add.Add(person_response);
            }
            //print person_response_list_from_add
            _testOutputHelper.WriteLine("Expected");
            foreach (PersonResponse person_response_from_add in person_response_list_from_add)
            {
                _testOutputHelper.WriteLine(person_response_from_add.ToString());
            }
            //Act
            List<PersonResponse>? persons_list_from_search = _personService.GetFilteredPersons(nameof(Person.Name), "ma");

            //print persons_list_from_get
            _testOutputHelper.WriteLine("Actual");
            foreach (PersonResponse person_response_from_get in persons_list_from_search)
            {
                _testOutputHelper.WriteLine(person_response_from_get.ToString());
            }

            foreach (PersonResponse person_response_from_add in person_response_list_from_add)
            {
                if (person_response_from_add.PersonName != null)
                {
                    if (person_response_from_add.PersonName.Contains("ma", StringComparison.OrdinalIgnoreCase))
                    {
                        Assert.Contains(person_response_from_add, persons_list_from_search);
                    }
                }

            }
        }

            //First we will add few persons; and then we will search based on person name with some search string.
            //It should return the matching persons

            [Fact]
        public void GetFilteredPersons_SearchByPersonName()
        {
            CountryAddRequest country_request_1 = new CountryAddRequest()
            {
                CountryName = "Usa"
            };
            CountryAddRequest country_request_2 = new CountryAddRequest()
            {
                CountryName = "Great Britain"
            };

            CountryResponse country_response_1 = _countriesService.AddCountry(country_request_1);
            CountryResponse country_response_2 = _countriesService.AddCountry(country_request_2);

            PersonAddRequest person_request_1 = new PersonAddRequest()
            {
                PersonName = "Sam",
                Email = "sam@example.pl",
                Address = "SomeAddress",
                Gender = GenderOptions.Male,
                CountryID = country_response_1.CountryID,
                DateOfBirth = DateTime.Parse("2002/05/06"),
                ReceiveNewsLetters = true,
            };
            PersonAddRequest person_request_2 = new PersonAddRequest()
            {
                PersonName = "Max",
                Email = "Max@example.pl",
                Address = "SomeAddress2",
                Gender = GenderOptions.Male,
                CountryID = country_response_2.CountryID,
                DateOfBirth = DateTime.Parse("2003/05/06"),
                ReceiveNewsLetters = false,
            };
            PersonAddRequest person_request_3 = new PersonAddRequest()
            {
                PersonName = "Lamar",
                Email = "Lamar@example.pl",
                Address = "SomeAddress3",
                Gender = GenderOptions.Male,
                CountryID = country_response_2.CountryID,
                DateOfBirth = DateTime.Parse("1999/05/06"),
                ReceiveNewsLetters = true,
            };
            List<PersonAddRequest> person_requests = new List<PersonAddRequest>() { person_request_1, person_request_2, person_request_3 };
            List<PersonResponse> person_response_from_list_from_add = new List<PersonResponse>();

            foreach (PersonAddRequest person_request in person_requests)
            {
                PersonResponse person_response = _personService.AddPerson(person_request);
                person_response_from_list_from_add.Add(person_response);
            }
            //print person_response_list_from_add
            _testOutputHelper.WriteLine("Expected");
            foreach (PersonResponse person_response_from_add in person_response_from_list_from_add)
            {
                _testOutputHelper.WriteLine(person_response_from_add.ToString());
            }
            //Act
            List<PersonResponse>? persons_list_from_search = _personService.GetFilteredPersons(nameof(Person.Name), "ma");

            //print persons_list_from_get
            _testOutputHelper.WriteLine("Actual");
            foreach (PersonResponse person_response_from_get in persons_list_from_search)
            {
                _testOutputHelper.WriteLine(person_response_from_get.ToString());
            }

            foreach (PersonResponse person_response_from_add in person_response_from_list_from_add)
            {
                if (person_response_from_add.PersonName != null)
                {
                    if (person_response_from_add.PersonName.Contains("ma", StringComparison.OrdinalIgnoreCase))
                    {
                        Assert.Contains(person_response_from_add, persons_list_from_search);
                    }
                }
            }
        }

        #endregion

        #region GetSortedPersons

        //When we sort based on PersonName in DESC, it should return persons list in descending on PersonName
        [Fact]
        public void GetSortedPersons()
        {
            CountryAddRequest country_request_1 = new CountryAddRequest()
            {
                CountryName = "Usa"
            };
            CountryAddRequest country_request_2 = new CountryAddRequest()
            {
                CountryName = "Great Britain"
            };

            CountryResponse country_response_1 = _countriesService.AddCountry(country_request_1);
            CountryResponse country_response_2 = _countriesService.AddCountry(country_request_2);

            PersonAddRequest person_request_1 = new PersonAddRequest()
            {
                PersonName = "Sam",
                Email = "sam@example.pl",
                Address = "SomeAddress",
                Gender = GenderOptions.Male,
                CountryID = country_response_1.CountryID,
                DateOfBirth = DateTime.Parse("2002/05/06"),
                ReceiveNewsLetters = true,
            };
            PersonAddRequest person_request_2 = new PersonAddRequest()
            {
                PersonName = "Max",
                Email = "Max@example.pl",
                Address = "SomeAddress2",
                Gender = GenderOptions.Male,
                CountryID = country_response_2.CountryID,
                DateOfBirth = DateTime.Parse("2003/05/06"),
                ReceiveNewsLetters = false,
            };
            PersonAddRequest person_request_3 = new PersonAddRequest()
            {
                PersonName = "Lamar",
                Email = "Lamar@example.pl",
                Address = "SomeAddress3",
                Gender = GenderOptions.Male,
                CountryID = country_response_2.CountryID,
                DateOfBirth = DateTime.Parse("1999/05/06"),
                ReceiveNewsLetters = true,
            };

            List<PersonAddRequest> person_requests = new List<PersonAddRequest>() { person_request_1, person_request_2, person_request_3 };
            List<PersonResponse> person_response_from_list_from_add = new List<PersonResponse>();

            foreach (PersonAddRequest person_request in person_requests)
            {
                PersonResponse person_response = _personService.AddPerson(person_request);
                person_response_from_list_from_add.Add(person_response);
            }
            //print person_response_list_from_add
            _testOutputHelper.WriteLine("Expected");
            foreach (PersonResponse person_response_from_add in person_response_from_list_from_add)
            {
                _testOutputHelper.WriteLine(person_response_from_add.ToString());
            }
            List<PersonResponse> allPersons = _personService.GetAllPersons();

            //Act
            List<PersonResponse>? persons_list_from_sort = _personService.GetSortedPersons(allPersons ,nameof(Person.Name), SortOrderOptions.DESC);

            //print persons_list_from_get
            _testOutputHelper.WriteLine("Actual");
            foreach (PersonResponse person_response_from_get in persons_list_from_sort)
            {
                _testOutputHelper.WriteLine(person_response_from_get.ToString());
            }
            person_response_from_list_from_add = person_response_from_list_from_add.OrderByDescending(temp => temp.PersonName).ToList();


            //Assert
            for (int i = 0; i<person_response_from_list_from_add.Count; i++)
            {
                Assert.Equal(person_response_from_list_from_add[i], persons_list_from_sort[i]);
            }
        }
        #endregion

        #region UpdatePerson

        //When we supply null as PersonUpdateRequest, it should throw ArgumentNullException
        [Fact]
        public void UpdatePerson_NullPerson()
        {
            //Arrange
            PersonUpdateRequest? person_update_request = null;

            //Assert
            Assert.Throws<ArgumentNullException>(() =>
            {
                _personService.UpdatePerson(person_update_request);
            });
        }

        //When we supply null invalid person id, it should throw ArgumentException
        [Fact]
        public void UpdatePerson_InvalidPersonID()
        {
            //Arrange
            PersonUpdateRequest? person_update_request = new
                PersonUpdateRequest() { PersonID = Guid.NewGuid() };

            //Assert
            Assert.Throws<ArgumentException>(() =>
            {
                _personService.UpdatePerson(person_update_request);
            });
        }

        //When we the person name is null it should throw ArgumentException
        [Fact]
        public void UpdatePerson_PersonNameIsNull()
        {
            //Arrange
            CountryAddRequest country_add_request = new CountryAddRequest() { CountryName = "UK" };
            CountryResponse country_response_from_add = _countriesService.AddCountry(country_add_request);

            PersonAddRequest person_add_requst = new PersonAddRequest()
            {
                PersonName = "John",
                CountryID = country_response_from_add.CountryID,
                Email = "john@example.com",
                Address = "address",
                DateOfBirth = DateTime.Parse("2002-04-04"),
                Gender = GenderOptions.Male,
                ReceiveNewsLetters= true
            };
            PersonResponse person_response_from_add = _personService.AddPerson(person_add_requst);


            PersonUpdateRequest? person_update_request = 
                person_response_from_add.ToPersonUpdateRequest();
            person_update_request.PersonName = null;

            //Assert
            Assert.Throws<ArgumentException>(() =>
            {
                //Act
                _personService.UpdatePerson(person_update_request);
            });
        }

        //First, add a new person and try update the person name and email
        [Fact]
        public void UpdatePerson_PersonFullDetailsUpdation()
        {
            //Arrange
            CountryAddRequest country_add_request = new CountryAddRequest() { CountryName = "UK" };
            CountryResponse country_response_from_add = _countriesService.AddCountry(country_add_request);

            PersonAddRequest person_add_requst = new PersonAddRequest()
            {
                PersonName = "John",
                CountryID = country_response_from_add.CountryID,
                Address = "Route 66",
                DateOfBirth = DateTime.Parse("2003-02-05"),
                Email = "john@example.com",
                Gender = GenderOptions.Male,
                ReceiveNewsLetters = true
            };
            PersonResponse person_response_from_add = _personService.AddPerson(person_add_requst);


            PersonUpdateRequest? person_update_request =
                person_response_from_add.ToPersonUpdateRequest();
            person_update_request.PersonName = "William";
            person_update_request.Email = "william@example.com";

            //Act
            PersonResponse person_response_from_update =
                _personService.UpdatePerson(person_update_request);

            PersonResponse? person_response_from_get = 
                _personService.GetPersonByPersonID(person_response_from_update.PersonID);

            //Assert
            Assert.Equal(person_response_from_get, person_response_from_update);
        }

        #endregion

        #region DeletePerson

        //If you supply an valid PersonID, it should return true
        [Fact]
        public void DeletePerson_ValidPersonID()
        {
            //Arrange
            CountryAddRequest country_add_request = new CountryAddRequest() { CountryName = "USA" };
            CountryResponse country_response_from_add = _countriesService.AddCountry(country_add_request);

            PersonAddRequest person_add_request = new PersonAddRequest()
            {
                PersonName = "Jones",
                Address = "address",
                Email = "email@example.com",
                CountryID = country_response_from_add.CountryID,
                DateOfBirth = DateTime.Parse("2010-05-02"),
                Gender = GenderOptions.Male,
                ReceiveNewsLetters = true
            };
            PersonResponse person_response_from_add = _personService.AddPerson(person_add_request);

            //Act
            bool isDeleted = _personService.DeletePerson(person_response_from_add.PersonID);

            //Assert
            Assert.True(isDeleted);
        }

        //If you supply an invalid PersonID, it should return false
        [Fact]
        public void DeletePerson_InvalidPersonID()
        {
            //Act
            bool isDeleted = _personService.DeletePerson(Guid.NewGuid());

            //Assert
            Assert.False(isDeleted);
        }

        #endregion
    }
}
