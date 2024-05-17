using BookStore1.Models;
namespace BookStore1.ViewModels
{
    public class MyBooksReviewViewModel
    {
        public List<Book> Books { get; set;}
        public int selectedBookId { get; set; }
        public Review Review { get; set; } 


    }
}
