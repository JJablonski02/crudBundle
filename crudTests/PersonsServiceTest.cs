using Entities;
using EntityFrameworkCoreMock;
using Microsoft.EntityFrameworkCore;
using ServiceContracts;
using ServiceContracts.DTO;
using ServiceContracts.Enums;
using Services;
using Xunit;
using Xunit.Abstractions;
using AutoFixture;
using FluentAssertions;

namespace crudTests
{
    public class PersonsServiceTest
    {
        private readonly IPersonsService _personService;
        private readonly ICountriesService _countriesService;
        private readonly ITestOutputHelper _testOutputHelper;
        private readonly IFixture _fixture;
        public PersonsServiceTest(ITestOutputHelper testOutputHelper)
        {
            _fixture= new Fixture();

            var countriesInitialData = new List<Country>() { };
            var personsInitialData = new List<Person>() { };

            DbContextMock<ApplicationDbContext> dbContextMock =
                new DbContextMock<ApplicationDbContext>(new DbContextOptionsBuilder<ApplicationDbContext>().Options);

            ApplicationDbContext dbContext = dbContextMock.Object;
            dbContextMock.CreateDbSetMock(temp => temp.Countries, countriesInitialData);
            dbContextMock.CreateDbSetMock(temp => temp.Persons, personsInitialData);

            _countriesService = new CountriesService(dbContext);
            _personService = new PersonsService(dbContext, _countriesService);
            _testOutputHelper = testOutputHelper;
        }
        #region AddPerson

        //When we supply null value as PersonAddRequest, it should throw ArgumentNullException
        [Fact]
        public async Task AddPerson_NullPerson()
        {
            //Arrange
            PersonAddRequest? personAddRequest = null;

            //Act
            Func<Task> action = (async () =>
            {
                await _personService.AddPerson(personAddRequest);
            });

            await action.Should().ThrowAsync<ArgumentNullException>();


           // await Assert.ThrowsAsync<ArgumentNullException>(async() => await _personService.AddPerson(personAddRequest));
            
        }

        //When we supply null value as PersonName, it should throw ArgumentNullException
        [Fact]
        public async Task AddPerson_PersonNameIsNull()
        {
            //Arrange
            PersonAddRequest? personAddRequest = _fixture.Build<PersonAddRequest>().With(temp => temp.PersonName, null as string).Create();

            //Assert

            Func<Task> action = (async () =>
            {
                await _personService.AddPerson(personAddRequest);
            });
            //await Assert.ThrowsAsync<ArgumentException>(async () =>

            //Act

            await action.Should().ThrowAsync<ArgumentException>();
            //await _personService.AddPerson(personAddRequest));

        }

        //When we supply proper person details, it shpuld insert the person into the persons list;
        //and it should return an object of PersonResponse, which includes with the newly generated person_id

        [Fact]
        public async Task AddPerson_ProperPersonDetails()
        {
            //Arrange
            PersonAddRequest? personAddRequest = _fixture.Build<PersonAddRequest>().With(temp => temp.Email, "example@example.com").Create(); 
            //Used With() method to specify the email output. Automatically generated guid email doesn't match with requirements.


            //Act
            PersonResponse person_response_from_add = await _personService.AddPerson(personAddRequest);
            List<PersonResponse> persons_list = await _personService.GetAllPersons();

            //Assert

            //Assert.True(person_response_from_add.PersonID != Guid.Empty);
            //Assert.Contains(person_response_from_add, persons_list);

            person_response_from_add.PersonID.Should().NotBe(Guid.Empty);

            //Assert.Contains(person_response_from_add, persons_list);
            persons_list.Should().Contain(person_response_from_add);

        }
        #endregion

        #region GetPersonByPersonID

        //If we supply null as PersonID, it should return as PersonResponse
        [Fact]
        public async Task GetPersonByPersonID_NullPersonID()
        {
            //Arrange
            Guid? PersonID = null;

            //Act
            PersonResponse? person_response_from_get = await _personService.GetPersonByPersonID(PersonID);

            //Assert
            //Assert.Null(person_response_from_get);
            person_response_from_get.Should().BeNull();
        }

        //If we supply a valid person id, it should return valid person details as PersonResponse object
        [Fact]
        public async Task GetPersonByPersonID_WithPersonID()
        {
            //Arrange

            CountryAddRequest country_request = _fixture.Create<CountryAddRequest>();
            
            CountryResponse country_response = await _countriesService.AddCountry(country_request);

            //Act
            PersonAddRequest person_request = _fixture.Build<PersonAddRequest>().With(temp => temp.Email, "sample@example.com").Create();

            PersonResponse person_response_from_add = await _personService.AddPerson(person_request);

            PersonResponse? person_response_from_get 
                = await _personService.GetPersonByPersonID(person_response_from_add.PersonID);
            //Assert
            //Assert.Equal(person_response_from_add, person_response_from_get);
            person_response_from_get.Should().Be(person_response_from_add);
        }
        #endregion

        #region GetAllPersons

        //The GetAllPersons() should return an empty list by default
        [Fact]
        public async Task GetAllPersons()
        {
            //Act
            List<PersonResponse> person_from_get = await _personService.GetAllPersons();

            //Assert
            //Assert.Empty(person_from_get);
            person_from_get.Should().BeEmpty();
            
        }
        //First, we will add few perons; and then we call GetAllPersons(), it should return the same persons that were added
        [Fact]
        public async Task GetAllPersons_AddFewPersons()
        {
            CountryAddRequest country_request_1 = _fixture.Create<CountryAddRequest>();
            CountryAddRequest country_request_2 = _fixture.Create<CountryAddRequest>();
           
            CountryResponse country_response_1 = await _countriesService.AddCountry(country_request_1);
            CountryResponse country_response_2 = await _countriesService.AddCountry(country_request_2);

            PersonAddRequest person_request_1 = _fixture.Build<PersonAddRequest>().With(temp => temp.Email, "sam@example.pl").Create();
            PersonAddRequest person_request_2 = _fixture.Build<PersonAddRequest>().With(temp => temp.Email, "Max@example.pl").Create();
            PersonAddRequest person_request_3 = _fixture.Build<PersonAddRequest>().With(temp => temp.Email, "Loi@example.pl").Create();

            List<PersonAddRequest> person_requests = new List<PersonAddRequest>() { person_request_1, person_request_2, person_request_3 };
            List<PersonResponse> person_response_from_list_from_add = new List<PersonResponse>();

            foreach (PersonAddRequest person_request in person_requests)
            {
                PersonResponse person_response = await _personService.AddPerson(person_request);
                person_response_from_list_from_add.Add(person_response);
            }
            //print person_response_list_from_add
            _testOutputHelper.WriteLine("Expected");
            foreach(PersonResponse person_response_from_add in person_response_from_list_from_add)
            {
                _testOutputHelper.WriteLine(person_response_from_add.ToString());
            }
            //Act
            List<PersonResponse> persons_list_from_get = await _personService.GetAllPersons();

            //print persons_list_from_get
            _testOutputHelper.WriteLine("Actual");
            foreach (PersonResponse person_response_from_get in persons_list_from_get)
            {
                _testOutputHelper.WriteLine(person_response_from_get.ToString());
            }
            //Assert
            //foreach (PersonResponse person_response_from_add in person_response_from_list_from_add)
            //{
            //    //Assert.Contains(person_response_from_add, persons_list_from_get);
            //}
            persons_list_from_get.Should().BeEquivalentTo(person_response_from_list_from_add);
        }

        #endregion

        #region GetFilteredPersons

        [Fact]
        public async void GetFilteredPersons_EmptySearchText()
        {
            CountryAddRequest country_request_1 = _fixture.Create<CountryAddRequest>();
            CountryAddRequest country_request_2 = _fixture.Create<CountryAddRequest>();

            CountryResponse country_response_1 = await _countriesService.AddCountry(country_request_1);
            CountryResponse country_response_2 = await _countriesService.AddCountry(country_request_2);

            PersonAddRequest person_request_1 = _fixture.Build<PersonAddRequest>()
            .With(temp => temp.Email, "someone_1@example.com")
            .With(temp => temp.CountryID, country_response_1.CountryID)
            .Create();

            PersonAddRequest person_request_2 = _fixture.Build<PersonAddRequest>()
             .With(temp => temp.Email, "someone_2@example.com")
             .With(temp => temp.CountryID, country_response_2.CountryID)
             .Create();

            PersonAddRequest person_request_3 = _fixture.Build<PersonAddRequest>()
             .With(temp => temp.Email, "someone_3@example.com")
             .With(temp => temp.CountryID, country_response_2.CountryID)
             .Create();

            List<PersonAddRequest> person_requests = new List<PersonAddRequest>() { person_request_1, person_request_2, person_request_3 };
            List<PersonResponse> person_response_list_from_add = new List<PersonResponse>();

            foreach (PersonAddRequest person_request in person_requests)
            {
                PersonResponse person_response = await _personService.AddPerson(person_request);
                person_response_list_from_add.Add(person_response);
            }

            //print person_response_list_from_add
            _testOutputHelper.WriteLine("Expected");
            foreach (PersonResponse person_response_from_add in person_response_list_from_add)
            {
                _testOutputHelper.WriteLine(person_response_from_add.ToString());
            }
            //Act
            List<PersonResponse>? persons_list_from_search = await _personService.GetFilteredPersons(nameof(Person.PersonName), "");

            //print persons_list_from_get
            _testOutputHelper.WriteLine("Actual:");
            foreach (PersonResponse person_response_from_get in persons_list_from_search)
            {
                _testOutputHelper.WriteLine(person_response_from_get.ToString());
            }

            //Assert

            //foreach (PersonResponse person_response_from_add in person_response_list_from_add)
            //{
            //    if (person_response_from_add.PersonName != null)
            //    {
            //        if (person_response_from_add.PersonName.Contains("ma", StringComparison.OrdinalIgnoreCase))
            //        {
            //            Assert.Contains(person_response_from_add, persons_list_from_search);
            //        }
            //    }

            //}

            persons_list_from_search.Should().BeEquivalentTo(person_response_list_from_add);
        }

            //First we will add few persons; and then we will search based on person name with some search string.
            //It should return the matching persons

            [Fact]
        public async Task GetFilteredPersons_SearchByPersonName()
        {
            CountryAddRequest country_request_1 = _fixture.Create<CountryAddRequest>();
            CountryAddRequest country_request_2 = _fixture.Create<CountryAddRequest>();

            CountryResponse country_response_1 = await _countriesService.AddCountry(country_request_1);
            CountryResponse country_response_2 = await _countriesService.AddCountry(country_request_2);

            PersonAddRequest person_request_1 = _fixture.Build<PersonAddRequest>()
                .With(temp => temp.PersonName, "Smith")
                .With(temp => temp.Email, "smith@example.pl")
                .With(temp => temp.CountryID, country_response_1.CountryID)
                .Create();
            PersonAddRequest person_request_2 = _fixture.Build<PersonAddRequest>()
                .With(temp => temp.PersonName, "Max")
                .With(temp => temp.Email, "Max@example.pl")
                .With(temp => temp.CountryID, country_response_2.CountryID)
                .Create();
            PersonAddRequest person_request_3 = _fixture.Build<PersonAddRequest>()
                .With(temp => temp.PersonName, "Cris")
                .With(temp => temp.Email, "cris@example.pl")
                .With(temp => temp.CountryID, country_response_1.CountryID)
                .Create();

            List<PersonAddRequest> person_requests = new List<PersonAddRequest>() { person_request_1, person_request_2, person_request_3 };
            List<PersonResponse> person_response_from_list_from_add = new List<PersonResponse>();

            foreach (PersonAddRequest person_request in person_requests)
            {
                PersonResponse person_response = await _personService.AddPerson(person_request);
                person_response_from_list_from_add.Add(person_response);
            }
            //print person_response_list_from_add
            _testOutputHelper.WriteLine("Expected");
            foreach (PersonResponse person_response_from_add in person_response_from_list_from_add)
            {
                _testOutputHelper.WriteLine(person_response_from_add.ToString());
            }
            //Act
            List<PersonResponse> persons_list_from_search = await _personService.GetFilteredPersons(nameof(Person.PersonName), "ma");

            //print persons_list_from_get
            _testOutputHelper.WriteLine("Actual");
            foreach (PersonResponse person_response_from_get in persons_list_from_search)
            {
                _testOutputHelper.WriteLine(person_response_from_get.ToString());
            }

            //Assert

            //foreach (PersonResponse person_response_from_add in person_response_from_list_from_add)
            //{
            //    if (person_response_from_add.PersonName != null)
            //    {
            //        if (person_response_from_add.PersonName.Contains("ma", StringComparison.OrdinalIgnoreCase))
            //        {
            //            Assert.Contains(person_response_from_add, persons_list_from_search);
            //        }
            //    }
            //}
            persons_list_from_search.Should().OnlyContain(temp => temp.PersonName.Contains("ma", StringComparison.OrdinalIgnoreCase));
        }

        #endregion

        #region GetSortedPersons

        //When we sort based on PersonName in DESC, it should return persons list in descending on PersonName
        [Fact]
        public async Task GetSortedPersons()
        {
            CountryAddRequest country_request_1 = new CountryAddRequest()
            {
                CountryName = "Usa"
            };
            CountryAddRequest country_request_2 = new CountryAddRequest()
            {
                CountryName = "Great Britain"
            };

            CountryResponse country_response_1 = await _countriesService.AddCountry(country_request_1);
            CountryResponse country_response_2 = await _countriesService.AddCountry(country_request_2);

            PersonAddRequest person_request_1 = _fixture.Build<PersonAddRequest>().With(temp => temp.Email, "sam@example.pl").Create();
            PersonAddRequest person_request_2 = _fixture.Build<PersonAddRequest>().With(temp => temp.Email, "Max@example.pl").Create();
            PersonAddRequest person_request_3 = _fixture.Build<PersonAddRequest>().With(temp => temp.Email, "Lamar@example.pl").Create();

            List<PersonAddRequest> person_requests = new List<PersonAddRequest>() { person_request_1, person_request_2, person_request_3 };
            List<PersonResponse> person_response_from_list_from_add = new List<PersonResponse>();

            foreach (PersonAddRequest person_request in person_requests)
            {
                PersonResponse person_response = await _personService.AddPerson(person_request);
                person_response_from_list_from_add.Add(person_response);
            }
            //print person_response_list_from_add
            _testOutputHelper.WriteLine("Expected");
            foreach (PersonResponse person_response_from_add in person_response_from_list_from_add)
            {
                _testOutputHelper.WriteLine(person_response_from_add.ToString());
            }
            List<PersonResponse> allPersons = await _personService.GetAllPersons();

            //Act
            List<PersonResponse>? persons_list_from_sort = await _personService.GetSortedPersons(allPersons ,nameof(Person.PersonName), SortOrderOptions.DESC);

            //print persons_list_from_get
            _testOutputHelper.WriteLine("Actual");
            foreach (PersonResponse person_response_from_get in persons_list_from_sort)
            {
                _testOutputHelper.WriteLine(person_response_from_get.ToString());
            }
            person_response_from_list_from_add = person_response_from_list_from_add.OrderByDescending(temp => temp.PersonName).ToList();


            //Assert

            //for (int i = 0; i<person_response_from_list_from_add.Count; i++)
            //{
            //    Assert.Equal(person_response_from_list_from_add[i], persons_list_from_sort[i]);
            //}

            //persons_list_from_sort.Should().BeEquivalentTo(person_response_from_list_from_add);

            persons_list_from_sort.Should().BeInDescendingOrder(temp => temp.PersonName);
        }
        #endregion

        #region UpdatePerson

        //When we supply null as PersonUpdateRequest, it should throw ArgumentNullException
        [Fact]
        public async Task UpdatePerson_NullPerson()
        {
            //Arrange
            PersonUpdateRequest? person_update_request = null;

            //Act
            //await Assert.ThrowsAsync<ArgumentNullException>(async() =>
            //{
            //    await _personService.UpdatePerson(person_update_request);
            //});

            Func<Task> action = async () =>
            {
                await _personService.UpdatePerson(person_update_request);
            };

            //Assert
            await action.Should().ThrowAsync<ArgumentNullException>();
        }

        //When we supply null invalid person id, it should throw ArgumentException
        [Fact]
        public async Task UpdatePerson_InvalidPersonID()
        {
            //Arrange
            PersonUpdateRequest? person_update_request = _fixture.Create<PersonUpdateRequest>();

            ////Assert
            //await Assert.ThrowsAsync<ArgumentException>(async() =>
            //{
            //    //Act
            //    await _personService.UpdatePerson(person_update_request);
            //});

            //Act
            Func<Task> action = (async ()=>
            {
                await _personService.UpdatePerson(person_update_request);
            });

            //Assert
            await action.Should().ThrowAsync<ArgumentException>();
        }

        //When we the person name is null it should throw ArgumentException
        [Fact]
        public async Task UpdatePerson_PersonNameIsNull()
        {
            //Arrange
            CountryAddRequest country_add_request = _fixture.Create<CountryAddRequest>();
            CountryResponse country_response_from_add = await _countriesService.AddCountry(country_add_request);

            PersonAddRequest person_add_requst = _fixture.Build<PersonAddRequest>().With(temp => temp.Email, "sam@example.pl").Create();
            PersonResponse person_response_from_add = await _personService.AddPerson(person_add_requst);


            PersonUpdateRequest? person_update_request = 
                person_response_from_add.ToPersonUpdateRequest();
            person_update_request.PersonName = null;

            ////Assert
            //await Assert.ThrowsAsync<ArgumentException>(async() =>
            //{
            //    //Act
            //    await _personService.UpdatePerson(person_update_request);
            //});

            //Act
            Func<Task> action = (async () =>
            {
                await _personService.UpdatePerson(person_update_request);
            });

            //Assert
            await action.Should().ThrowAsync<ArgumentException>();
        }

        //First, add a new person and try update the person name and email
        [Fact]
        public async void UpdatePerson_PersonFullDetailsUpdation()
        {
            //Arrange
            CountryAddRequest country_add_request = _fixture.Create<CountryAddRequest>();
            CountryResponse country_response_from_add = await _countriesService.AddCountry(country_add_request);

            PersonAddRequest person_add_requst = _fixture.Build<PersonAddRequest>().With(temp => temp.Email, "john@example.pl").Create();
            PersonResponse person_response_from_add = await _personService.AddPerson(person_add_requst);


            PersonUpdateRequest? person_update_request =
                person_response_from_add.ToPersonUpdateRequest();
            person_update_request.PersonName = "William";
            person_update_request.Email = "william@example.com";

            //Act
            PersonResponse person_response_from_update =
                await _personService.UpdatePerson(person_update_request);

            PersonResponse? person_response_from_get = 
                await _personService.GetPersonByPersonID(person_response_from_update.PersonID);

            //Assert
            //Assert.Equal(person_response_from_get, person_response_from_update);

            person_response_from_update.Should().Be(person_response_from_get);
        }

        #endregion

        #region DeletePerson

        //If you supply an valid PersonID, it should return true
        [Fact]
        public async Task DeletePerson_ValidPersonID()
        {
            //Arrange
            CountryAddRequest country_add_request = new CountryAddRequest() { CountryName = "USA" };
            CountryResponse country_response_from_add = await _countriesService.AddCountry(country_add_request);

            PersonAddRequest person_add_request = _fixture.Build<PersonAddRequest>().With(temp => temp.Email, "jones@example.pl").Create();
            PersonResponse person_response_from_add = await _personService.AddPerson(person_add_request);

            //Act
            bool isDeleted = await _personService.DeletePerson(person_response_from_add.PersonID);

            //Assert
            //Assert.True(isDeleted);
            isDeleted.Should().BeTrue();
        }

        //If you supply an invalid PersonID, it should return false
        [Fact]
        public async Task DeletePerson_InvalidPersonID()
        {
            //Act
            bool isDeleted = await _personService.DeletePerson(Guid.NewGuid());

            //Assert
            //Assert.False(isDeleted);
            isDeleted.Should().BeFalse();
        }

        #endregion
    }
}
