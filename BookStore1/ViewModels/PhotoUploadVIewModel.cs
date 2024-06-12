using BookStore1.Models;
namespace BookStore1.ViewModels
{
    public class PhotoUploadVIewModel
    {
        public Book book {  get; set; }
        public string? FrontPage { get; set; }
        public string? DownloadUrl { get; set; }
        public IFormFile FrontPageFile { get; set; }
        public IFormFile DownloadUrlFile { get; set; }

    }
}
