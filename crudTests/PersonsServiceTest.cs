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
using RepositoryContracts;
using Moq;
using System.Linq.Expressions;
using Serilog.Extensions.Hosting;
using Microsoft.Extensions.Logging;


namespace crudTests
{
    public class PersonsServiceTest
    {
        private readonly IPersonsService _personService;

        private readonly Mock<IPersonsRepository> _personsRepositoryMock;
        private readonly IPersonsRepository _personsRepository;

        private readonly ITestOutputHelper _testOutputHelper;
        private readonly IFixture _fixture;

        public PersonsServiceTest(ITestOutputHelper testOutputHelper)
        {
            _fixture= new Fixture();
            _personsRepositoryMock = new Mock<IPersonsRepository>();
            _personsRepository = _personsRepositoryMock.Object;

            var loggerMock = new Mock<ILogger<PersonsService>>();
            var diagnosticContextMock = new Mock<DiagnosticContext>();

            _personService = new PersonsService(_personsRepository,loggerMock.Object, diagnosticContextMock.Object);

            _testOutputHelper = testOutputHelper;
        }
        #region AddPerson

        //When we supply null value as PersonAddRequest, it should throw ArgumentNullException
        [Fact]
        public async Task AddPerson_NullPerson_ToBeArgumentNullException()
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
        public async Task AddPerson_PersonNameIsNull_ToBeArgumentException()
        {
            //Arrange
            PersonAddRequest? personAddRequest = _fixture.Build<PersonAddRequest>().With(temp => temp.PersonName, null as string).Create();
            Person person = personAddRequest.ToPerson();

            //When PersonsRepository.AddPerson is called, it has to return the same "person" object
            _personsRepositoryMock.Setup(temp => temp.AddPerson(It.IsAny<Person>())).ReturnsAsync(person);

            //Act
            Func<Task> action = (async () =>
            {
                await _personService.AddPerson(personAddRequest);
            });

            //Assert
            await action.Should().ThrowAsync<ArgumentException>();


        }

        //When we supply proper person details, it shpuld insert the person into the persons list;
        //and it should return an object of PersonResponse, which includes with the newly generated person_id
        [Fact]
        public async Task   AddPerson_FullPersonDetails_ToBeSuccessful()
        {
            //Arrange
            PersonAddRequest? personAddRequest = _fixture.Build<PersonAddRequest>().With(temp => temp.Email, "example@example.com").Create();
            //Used With() method to specify the email output. Automatically generated guid email doesn't match with requirements.

            Person person = personAddRequest.ToPerson();
            PersonResponse person_response_expected = person.ToPersonResponse();

            _personsRepositoryMock.Setup(temp => temp.AddPerson(It.IsAny<Person>())).ReturnsAsync(new Person());


            //Act
            PersonResponse person_response_from_add = await _personService.AddPerson(personAddRequest);
            person_response_expected.PersonID = person_response_from_add.PersonID;

            //Assert
            //Assert.True(person_response_from_add.PersonID != Guid.Empty);

            person_response_from_add.PersonID.Should().NotBe(Guid.Empty);
            person_response_from_add.Should().Be(person_response_expected);

        }
        #endregion

        #region GetPersonByPersonID

        //If we supply null as PersonID, it should return as PersonResponse
        [Fact]
        public async Task GetPersonByPersonID_NullPersonID_ToBeNull()
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
        public async Task GetPersonByPersonID_WithPersonID_ToBeSuccessful()
        {
            //Arrange
            Person person = _fixture.Build<Person>().With(temp => temp.Email, "sample@example.com")
                .With(temp => temp.Country, null as Country)
                .Create();
            PersonResponse person_response_expected = person.ToPersonResponse();

            _personsRepositoryMock.Setup(temp => temp.GetPersonByPersonID(It.IsAny<Guid>())).ReturnsAsync(person);

            //Act
            PersonResponse? person_response_from_get 
                = await _personService.GetPersonByPersonID(person.PersonID);
            //Assert
            //Assert.Equal(person_response_from_add, person_response_from_get);
            person_response_from_get.Should().Be(person_response_expected);
        }
        #endregion

        #region GetAllPersons

        //The GetAllPersons() should return an empty list by default
        [Fact]
        public async Task GetAllPersons_EmptyList_ToBeEmpty()
        {
            //Arrange
            _personsRepositoryMock.Setup(temp => temp.GetAllPersons()).ReturnsAsync(new List<Person>());

            //Act
            List<PersonResponse> person_from_get = await _personService.GetAllPersons();

            //Assert
            //Assert.Empty(person_from_get);
            person_from_get.Should().BeEmpty();
            
        }
        //First, we will add few perons; and then we call GetAllPersons(), it should return the same persons that were added
        [Fact]
        public async Task GetAllPersons_WithFewPersons_ToBeSuccessful()
        {
            //Arrange
            List<Person> persons = new List<Person>()
            {
                _fixture.Build<Person>().With(temp => temp.Email, "somone_1@example.com")
                .With(temp => temp.Country, null as Country).Create(),

                _fixture.Build<Person>().With(temp => temp.Email, "somone_2@example.com")
                .With(temp => temp.Country, null as Country).Create(),

                _fixture.Build<Person>().With(temp => temp.Email, "somone_3@example.com")
                .With(temp => temp.Country, null as Country).Create(),
            };
            List<PersonResponse> person_response_list_expected = persons.Select(temp => temp.ToPersonResponse()).ToList();

            //print person_response_list_from_add
            _testOutputHelper.WriteLine("Expected:");
            foreach(PersonResponse person_response_from_add in person_response_list_expected)
            {
                _testOutputHelper.WriteLine(person_response_from_add.ToString());
            }
            _personsRepositoryMock.Setup(temp => temp.GetAllPersons()).ReturnsAsync(persons);

            //Act
            List<PersonResponse> persons_list_from_get = await _personService.GetAllPersons();

            //print persons_list_from_get
            _testOutputHelper.WriteLine("Actual:");
            foreach (PersonResponse person_response_from_get in persons_list_from_get)
            {
                _testOutputHelper.WriteLine(person_response_from_get.ToString());
            }
            //Assert
            persons_list_from_get.Should().BeEquivalentTo(person_response_list_expected);
        }

        #endregion

        #region GetFilteredPersons

        [Fact]
        public async void GetFilteredPersons_EmptySearchText_ToBeSuccessful()
        {
            //Arrange
            List<Person> persons = new List<Person>()
            {
                _fixture.Build<Person>().With(temp => temp.Email, "somone_1@example.com")
                .With(temp => temp.Country, null as Country).Create(),

                _fixture.Build<Person>().With(temp => temp.Email, "somone_2@example.com")
                .With(temp => temp.Country, null as Country).Create(),

                _fixture.Build<Person>().With(temp => temp.Email, "somone_3@example.com")
                .With(temp => temp.Country, null as Country).Create(),
            };

            List<PersonResponse> person_response_list_expected = persons.Select(temp => temp.ToPersonResponse()).ToList();

            //print person_response_list_from_add
            _testOutputHelper.WriteLine("Expected:");
            foreach (PersonResponse person_response_from_add in person_response_list_expected)
            {
                _testOutputHelper.WriteLine(person_response_from_add.ToString());
            }

            _personsRepositoryMock.Setup(temp => temp.GetFilteredPersons(It.IsAny<Expression<Func<Person, bool>>>())).ReturnsAsync(persons);
            //Act
            List<PersonResponse> persons_list_from_search = await _personService.GetFilteredPersons(nameof(Person.PersonName), "");

            //print persons_list_from_get
            _testOutputHelper.WriteLine("Actual:");
            foreach (PersonResponse person_response_from_get in persons_list_from_search)
            {
                _testOutputHelper.WriteLine(person_response_from_get.ToString());
            }

            //Assert
            persons_list_from_search.Should().BeEquivalentTo(person_response_list_expected);
        }

            //First  add few persons and then earch based on person name with some search string.
            //It should return the matching persons

            [Fact]
        public async Task GetFilteredPersons_SearchByPersonName_ToBeSuccessful()
        {
            //Arrange
            List<Person> persons = new List<Person>()
            {
                _fixture.Build<Person>().With(temp => temp.Email, "somone_1@example.com")
                .With(temp => temp.Country, null as Country)
                .Create(),

                _fixture.Build<Person>().With(temp => temp.Email, "somone_2@example.com")
                .With(temp => temp.Country, null as Country)
                .Create(),

                _fixture.Build<Person>().With(temp => temp.Email, "somone_3@example.com")
                .With(temp => temp.Country, null as Country)
                .Create(),
            };

            List<PersonResponse> person_response_list_expected = persons.Select(temp => temp.ToPersonResponse()).ToList();

            //print person_response_list_from_add
            _testOutputHelper.WriteLine("Expected");
            foreach (PersonResponse person_response_from_add in person_response_list_expected)
            {
                _testOutputHelper.WriteLine(person_response_from_add.ToString());
            }

            _personsRepositoryMock.Setup(temp => temp.GetFilteredPersons(It.IsAny<Expression<Func<Person, bool>>>())).ReturnsAsync(persons);
            //Act
            List<PersonResponse>? persons_list_from_search = await _personService.GetFilteredPersons(nameof(Person.PersonName), "");

            //print persons_list_from_get
            _testOutputHelper.WriteLine("Actual:");
            foreach (PersonResponse person_response_from_get in persons_list_from_search)
            {
                _testOutputHelper.WriteLine(person_response_from_get.ToString());
            }

            //Assert
            persons_list_from_search.Should().BeEquivalentTo(person_response_list_expected);
        }

        #endregion

        #region GetSortedPersons

        //When we sort based on PersonName in DESC, it should return persons list in descending on PersonName
        [Fact]
        public async Task GetSortedPersons_ToBeSuccessful()
        {
            List<Person> persons = new List<Person>()
            {
                _fixture.Build<Person>().With(temp => temp.Email, "somone_1@example.com")
                .With(temp => temp.Country, null as Country)
                .Create(),

                _fixture.Build<Person>().With(temp => temp.Email, "somone_2@example.com")
                .With(temp => temp.Country, null as Country)
                .Create(),

                _fixture.Build<Person>().With(temp => temp.Email, "somone_3@example.com")
                .With(temp => temp.Country, null as Country)
                .Create(),
            };
            List<PersonResponse> person_response_list_expected = persons.Select(temp => temp.ToPersonResponse()).ToList();

            _personsRepositoryMock.Setup(temp => temp.GetAllPersons()).ReturnsAsync(persons);
            
            //print person_response_list_from_add
            _testOutputHelper.WriteLine("Expected");
            foreach (PersonResponse person_response_from_add in person_response_list_expected)
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

            //Assert
            persons_list_from_sort.Should().BeInDescendingOrder(temp => temp.PersonName);

        }
        #endregion

        #region UpdatePerson

        //When we supply null as PersonUpdateRequest, it should throw ArgumentNullException
        [Fact]
        public async Task UpdatePerson_NullPerson_ToBeArgumentNullException()
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
        public async Task UpdatePerson_InvalidPersonID_ToBeArgumentException()
        {
            //Arrange
            PersonUpdateRequest? person_update_request = _fixture.Build<PersonUpdateRequest>()
             .Create();

            //Act
            Func<Task> action = async () =>
            {
                await _personService.UpdatePerson(person_update_request);
            };

            //Assert
            await action.Should().ThrowAsync<ArgumentException>();
        }
   

        //When we the person name is null it should throw ArgumentException
        [Fact]
        public async Task UpdatePerson_PersonNameIsNull_ToBeArgumentException()
        {
            //Arrange
            Person person = _fixture.Build<Person>()
             .With(temp => temp.PersonName, null as string)
             .With(temp => temp.Email, "someone@example.com")
             .With(temp => temp.Country, null as Country)
             .With(temp => temp.Gender, "Male")
             .Create();

            PersonResponse person_response_from_add = person.ToPersonResponse();

            PersonUpdateRequest person_update_request = person_response_from_add.ToPersonUpdateRequest();


            //Act
            var action = async () =>
            {
                await _personService.UpdatePerson(person_update_request);
            };

            //Assert
            await action.Should().ThrowAsync<ArgumentException>();
        }

        //First, add a new person and try update the person name and email
        [Fact]
        public async void UpdatePerson_PersonFullDetails_ToBeSuccessful()
        {
            //Arrange
            Person person = _fixture.Build<Person>()
             .With(temp => temp.Email, "someone@example.com")
             .With(temp => temp.Country, null as Country)
             .With(temp => temp.Gender, "Male")
             .Create();

            PersonResponse person_response_expected = person.ToPersonResponse();
            PersonUpdateRequest person_update_request = person_response_expected.ToPersonUpdateRequest();

            _personsRepositoryMock.Setup(temp => temp.UpdatePerson(It.IsAny<Person>())).ReturnsAsync(person);
            _personsRepositoryMock.Setup(temp => temp.GetPersonByPersonID(It.IsAny<Guid>())).ReturnsAsync(person);


            //Act
            PersonResponse person_response_from_update =
                await _personService.UpdatePerson(person_update_request);

            //Assert
            person_response_from_update.Should().Be(person_response_expected);
        }

        #endregion


        #region DeletePerson

        //If you supply an valid PersonID, it should return true
        [Fact]
        public async Task DeletePerson_ValidPersonID_ToBeSuccessful()
        {
            //Arrange
            Person person = _fixture.Build<Person>()
                .With(temp => temp.Email, "someone@exampple.com")
                .With(temp => temp.Country, null as Country)
                .With(temp => temp.Gender, "Male")
                .Create();

            _personsRepositoryMock.Setup(temp => temp.DeletePersonByPersonID(It.IsAny<Guid>())).ReturnsAsync(true);
            _personsRepositoryMock.Setup(temp => temp.GetPersonByPersonID(It.IsAny<Guid>())).ReturnsAsync(person);

            //Act
            bool isDeleted = await _personService.DeletePerson(person.PersonID);

            //Assert
            //Assert.True(isDeleted);
            isDeleted.Should().BeTrue();
        }

        //If you supply an invalid PersonID, it should return false
        [Fact]
        public async Task DeletePerson_InvalidPersonID_ToBeSuccessful()
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
