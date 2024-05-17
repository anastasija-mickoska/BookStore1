using Microsoft.AspNetCore.Mvc.Rendering;
using BookStore1.Models;
using System.Collections.Generic;

namespace BookStore1.ViewModels
{
    public class AuthorBookViewModel
    {
        public Author Author { get; set; }
        public IEnumerable<int?> SelectedBooks { get; set; }
        public IEnumerable<SelectListItem>? BookList { get; set; }
        public IEnumerable<Book> BooksByAuthor { get; set; }


    }
}
