using BookStore1.Areas.Identity.Data;
using BookStore1.Models;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace BookStore1.ViewModels
{
    public class CreateReviewViewModel
    {
        public Review Review { get; set; }
        public List<SelectListItem> BoughtBooks { get; set; }
        public BookStore1User User { get; set; }


    }
}
