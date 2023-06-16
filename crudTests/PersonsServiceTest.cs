using ServiceContracts;
using ServiceContracts.DTO;
using Services;
using System;
using Xunit;

namespace crudTests
{
    public class PersonsServiceTest
    {
        private readonly IPersonsService _personsService;
        public PersonsServiceTest(IPersonsService personsService)
        {
            _personsService = new PersonsService();
        }
        #region AddPerson

        //When we supply null value as PersonAddRequest, it should throw ArgumentNullException

        [Fact]
        public void AddPerson_NullPerson()
        {
            //Arrange
            PersonAddRequest? personAddRequest = null;

            //Act
            Assert.Throws<ArgumentNullException>(() => _personsService.AddPerson(personAddRequest));
            
        }

        //When we supply null value as PersonName, it should throw ArgumentNullException
        [Fact]
        public void AddPerson_PersonNameIsNull()
        {
            //Arrange
            PersonAddRequest? personAddRequest = new PersonAddRequest() { PersonName = null };

            //Act
            Assert.Throws<ArgumentNullException>(() => _personsService.AddPerson(personAddRequest));

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
            PersonResponse person_response_from_add = _personsService.AddPerson(personAddRequest);
            List<PersonResponse> persons_list = _personsService.GetAllPersons();

            //Asert
            Assert.True(person_response_from_add.PersonID != Guid.Empty);
            Assert.Contains(person_response_from_add, persons_list);

        }
        #endregion

    }
}
