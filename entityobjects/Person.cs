using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace EntityObjects
{
    public class Person
    {
        [Key]
        public Guid PersonID { get; set; }

        [StringLength(50)] //nvarchar (40) in db
        //[Required]
        public string? PersonName { get; set; }

        [StringLength(50)]
        public string? Email { get; set; }
        public DateTime? DateOfBirth { get; set; }

        [StringLength(10)]
        public string? Gender { get; set; }
        public Guid? CountryID { get; set; }

        [StringLength(200)] 
        public string? Address { get; set; }

        public bool ReceiveNewsLetters { get; set; }

        public string? TIN { get; set; } // Text identification number

        [ForeignKey("CountryID")]
        public virtual Country? Country { get; set; }

        public override string ToString()
        {
            return $"Person ID: {PersonID}, " +
                $"Person Name: {PersonName}, " +
                $"Email: {Email}, " +
                $"Date of Birth: {DateOfBirth?.ToString("dd MM YYYY")}, " +
                $"Gender: {Gender}, " +
                $"CountryID: {CountryID}, " +
                $"Country: {Country?.CountryName}, " +
                $"Address: {Address}, " +
                $"Receive News Letters: {ReceiveNewsLetters} ";
        }
    }
}
