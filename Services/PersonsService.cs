using System;
using Entities;
using ServiceContracts.DTO;
using ServiceContracts;
using System.ComponentModel.DataAnnotations;
using Services.Helpers;

namespace Services
{
    public class PersonsService : IPersonsService
    {
        //private field
        private readonly List<Person> _persons;
        private readonly ICountriesService _countriesService;

        //ctor
            public PersonsService()
            {
                _persons = new List<Person>();
                _countriesService = new CountriesService();  
            }

        private PersonResponse ConvertPersonToPersonResponse(Person person)
        {
            PersonResponse personResponse = person.ToPersonResponse();
            personResponse.Country = _countriesService.GetCountryByCountryID(person.CountryID)?.CountryName;
            return personResponse;
        }

        public PersonResponse AddPerson(PersonAddRequest? personAddRequest)
        {
            //check if PersonAddRequest is not null
            if(personAddRequest is null)
            {
                throw new ArgumentNullException (nameof(personAddRequest));
            }
            //Validate PersonName
            if (string.IsNullOrEmpty(personAddRequest.PersonName))
            {
                throw new ArgumentException("Person cannot be blank");
            }

            //Model validation 
            ValidationHelper.ModelValidation(personAddRequest);

            //Convert PersonAddRequest into Person type
            Person person = personAddRequest.ToPerson();

            //Generate personID
            person.PersonID = Guid.NewGuid();

            //add person object to persons list
            _persons.Add(person);

            //convert the Person object into PersonResponse
            return ConvertPersonToPersonResponse(person);
            
        }

        public List<PersonResponse> GetAllPersons()
        {
            return _persons.Select(temp => temp.ToPersonResponse()).ToList();
        }

        public PersonResponse? GetPersonByPersonID(Guid? personID)
        {
            if (personID is null)
                return null;

            Person? person = _persons.FirstOrDefault(temp => temp.PersonID == personID);

            if (person is null)
                return null;

            return person.ToPersonResponse();
        }

        public List<PersonResponse> GetFilteredPersons(string searchBy, string? searchString)
        {
            List<PersonResponse> allPersons = GetAllPersons();
            List<PersonResponse> matchingPersons = allPersons;

            if(string.IsNullOrEmpty(searchBy) || string.IsNullOrEmpty(searchString))
                return matchingPersons;

            switch (searchBy)
            {
                case nameof(Person.Name):
                    matchingPersons = allPersons.Where(temp => (!string.IsNullOrEmpty(temp.PersonName) ? temp.PersonName.Contains
                        (searchString, StringComparison.OrdinalIgnoreCase) : true)).ToList();
                    break;

                case nameof(Person.Email):
                    matchingPersons = allPersons.Where(temp => (!string.IsNullOrEmpty(temp.Email) ? temp.Email.Contains
                        (searchString, StringComparison.OrdinalIgnoreCase) : true)).ToList();
                    break;

                case nameof(Person.DateOfBirth):
                    matchingPersons = allPersons.Where(temp => (temp.DateOfBirth != null) ? (temp.DateOfBirth.Value.ToString
                        ("dd MMMM yyyy").Contains(searchString, StringComparison.OrdinalIgnoreCase)) : true).ToList();
                    break;

                case nameof(Person.Gender):
                    matchingPersons = allPersons.Where(temp => (!string.IsNullOrEmpty(temp.Gender) ? temp.Gender.Contains
                        (searchString, StringComparison.OrdinalIgnoreCase) : true)).ToList();
                    break;

                case nameof(Person.CountryID):
                    matchingPersons = allPersons.Where(temp => (!string.IsNullOrEmpty(temp.Country) ? temp.Country.Contains
                        (searchString, StringComparison.OrdinalIgnoreCase) : true)).ToList();
                    break;

                case nameof(Person.Address):
                    matchingPersons = allPersons.Where(temp => (!string.IsNullOrEmpty(temp.Address) ? temp.Address.Contains
                        (searchString, StringComparison.OrdinalIgnoreCase) : true)).ToList();
                    break;

                default: matchingPersons = allPersons;
                    break;

            }
            return matchingPersons;
        }
    }
}
