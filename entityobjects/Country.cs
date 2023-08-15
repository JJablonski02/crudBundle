﻿using System.ComponentModel.DataAnnotations;

namespace EntityObjects
{
    public class Country
    {
        /// <summary>
        /// Domain Model for Country
        /// </summary>

        [Key]
        public Guid CountryID { get; set; }
        public string? CountryName { get; set; }
        public virtual ICollection<Person>? Persons { get; set; }
    }
}