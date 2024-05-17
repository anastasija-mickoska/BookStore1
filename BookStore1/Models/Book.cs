using Microsoft.AspNetCore.Mvc.ViewEngines;
using System.ComponentModel.DataAnnotations;

namespace BookStore1.Models
{
    public class Book
    {
        public int Id { get; set; }
        [MaxLength(100)]
        [Required]
        public string Title { get; set; }
        public int? YearPublished { get; set; }
        public int? NumPages { get; set; }
        public string? Description { get; set; }
        [MaxLength(50)]
        public string? Publisher { get; set; }
        public string? FrontPage { get; set; }
        public string? DownloadUrl { get; set; }
        public int AuthorId { get; set; }
        public Author Author { get; set; }
        public ICollection<BookGenre>? Genres { get; set; }
        public ICollection<Review>? Reviews { get; set; }
        public ICollection<UserBooks>? Users { get; set; }

        public double AverageRating()
        {
            if (Reviews == null || Reviews.Count == 0)
            {
                return 0;
            }
            int sumOfRatings = 0;
            foreach (var item in Reviews)
            {
                if(item.Rating != null)
                {
                    sumOfRatings += (int)item.Rating;

                }
            }
            double averageRating = (double)sumOfRatings / Reviews.Count;
            return averageRating;
        }
    }
}
