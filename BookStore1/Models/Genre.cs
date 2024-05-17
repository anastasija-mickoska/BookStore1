using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace BookStore1.Models
{
    public class Genre
    {
        public int Id { get; set; }
        [Display(Name="Genre Name")]
        [StringLength(50)]
        public string GenreName { get; set; }
        public ICollection<BookGenre>? Books { get; set; }
    }
}
