using Microsoft.AspNetCore.Mvc.Rendering;
using BookStore1.Models;

namespace BookStore1.ViewModels
{
    public class BooksFiltersViewModel
    {
        public IList<Book>? Books { get; set; }
        public SelectList? Filters { get; set; }
        public string SearchString { get; set; }

        public IList<Review>? Reviews { get; set; }
        public double AverageRating()
        {
            if (Reviews == null || Reviews.Count == 0)
            {
                return 0;
            }
            int sumOfRatings = 0;
            foreach (var item in Reviews)
            {
                if (item.Rating != null)
                {
                    sumOfRatings += (int)item.Rating;

                }
            }
            double averageRating = (double)sumOfRatings / Reviews.Count;
            return averageRating;
        }
    }

    }