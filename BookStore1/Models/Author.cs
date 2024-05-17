using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace BookStore1.Models
{
    public class Author
    {
        public int Id { get; set; }
        [MaxLength(50)]
        [Display(Name="First Name")]
        public string FirstName { get; set; }
        [MaxLength(50)]
        [Display(Name="Last Name")]

        public string LastName { get; set; }
        [DataType(DataType.Date)]
        [Display(Name="Birth Date")]
        public DateTime? BirthDate { get; set; }
        [MaxLength(50)]

        public string? Nationality { get; set; }
        [MaxLength(50)]

        public string? Gender { get; set; }
        public string FullName
        {
            get { return String.Format("{0} {1}", FirstName, LastName); }
        }
        public ICollection<Book>? Books { get; set; }
    }
}
