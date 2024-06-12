using BookStore1.Areas.Identity.Data;
using System.ComponentModel.DataAnnotations;

namespace BookStore1.Models
{
    public class UserBooks
    {
        public int Id { get; set; }
        [MaxLength(450)]
        public string AppUser { get; set; }
        public int BookId { get; set; }
        public Book? Book { get; set; }
        public BookStore1User? User { get; set; }


    }
}
