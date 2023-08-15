using System;
using ServiceContracts.DTO;
using ServiceContracts.Enums;

namespace ServiceContracts
{
    public interface IPersonsDeleterService
    {
        /// <summary>
        /// Deletes a person on the given person id
        /// </summary>
        /// <param name="personID"></param>
        /// <returns>Returns true, if the deletion is successfull otherwise false</returns>
        Task<bool> DeletePerson(Guid? personID);
    }
}
