﻿using EntityObjects;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using RepositoryContracts;
using System.Linq.Expressions;

namespace Repositories
{
    public class PersonsRepository : IPersonsRepository
    {
        private readonly ApplicationDbContext _DbContext;
        private readonly ILogger<PersonsRepository> _logger;
        public PersonsRepository(ApplicationDbContext applicationDbContext, ILogger<PersonsRepository>  logger)
        {
            _DbContext = applicationDbContext;
            _logger = logger;
        }

        public async Task<Person> AddPerson(Person person)
        {
            _DbContext.Persons.Add(person);
            await _DbContext.SaveChangesAsync();

            return person;
        }

        public async Task<bool> DeletePersonByPersonID(Guid personID)
        {
            _DbContext.Persons.RemoveRange(_DbContext.Persons.Where(temp => temp.PersonID == personID));
            int rowsDeleted = await _DbContext.SaveChangesAsync();

            return rowsDeleted > 0;
        }

        public async Task<Person?> GetPersonByPersonID(Guid personID)
        {
            return await _DbContext.Persons.Include("Country").FirstOrDefaultAsync(temp => temp.PersonID == personID);
        }

        public async Task<List<Person>> GetAllPersons()
        {
            _logger.LogInformation("GetAllPersons of PersonRepository");
            return await _DbContext.Persons.Include("Country").ToListAsync();
        }

        public async Task<List<Person>> GetFilteredPersons(Expression<Func<Person, bool>> predicate)
        {
            _logger.LogInformation("GetFilteredPersons of PersonRepository");
            return await _DbContext.Persons.Include("Country").Where(predicate).ToListAsync();
        }

        public async Task<Person> UpdatePerson(Person person)
        {
            Person? matchingPerson = await _DbContext.Persons.FirstAsync(temp => temp.PersonID == person.PersonID);

            if (matchingPerson == null)
                return person;

            matchingPerson.PersonName = person.PersonName;
            matchingPerson.Email = person.Email;
            matchingPerson.DateOfBirth = person.DateOfBirth;
            matchingPerson.Gender = person.Gender;
            matchingPerson.CountryID = person.CountryID;
            matchingPerson.Address = person.Address;
            matchingPerson.ReceiveNewsLetters = person.ReceiveNewsLetters;

            int countUpdated = await _DbContext.SaveChangesAsync();
            return matchingPerson;


        }
    }
}