using System;
using ServiceContracts.DTO;
using ServiceContracts.Enums;

namespace ServiceContracts
{
    public interface IPersonsSorterService

    {
        /// <summary>
        /// Returns sorted list of persons
        /// </summary>
        /// <param name="allPersons"></param>
        /// <param name="sortBy">Name of the property (key), based on which the persons should be sorted</param>
        /// <param name="sortOrder"></param>
        /// <returns>Returns sorted persons as PersonResponse object</returns>
        Task<List<PersonResponse>> GetSortedPersons(List<PersonResponse> allPersons, string sortBy, SortOrderOptions sortOrder);
    }
}
