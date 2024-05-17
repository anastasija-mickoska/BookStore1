using Humanizer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using BookStore1.Areas.Identity.Data;
using static System.Net.WebRequestMethods;
namespace BookStore1.Models
{
    public class SeedData
    {
        public static async Task CreateUserRoles(IServiceProvider serviceProvider)
        {
            var RoleManager = serviceProvider.GetRequiredService<RoleManager<IdentityRole>>();
            var UserManager = serviceProvider.GetRequiredService<UserManager<BookStore1User>>();
            IdentityResult roleResult;
            //Add Admin Role
            var roleCheck = await RoleManager.RoleExistsAsync("Admin");
            if (!roleCheck) { roleResult = await RoleManager.CreateAsync(new IdentityRole("Admin")); }
            BookStore1User user = await UserManager.FindByEmailAsync("admin@bookstore.com");
            if (user == null)
            {
                var User = new BookStore1User();
                User.Email = "admin@mvcmovie.com";
                User.UserName = "admin@mvcmovie.com";
                string userPWD = "Admin123";
                IdentityResult chkUser = await UserManager.CreateAsync(User, userPWD);
                //Add default User to Role Admin
                if (chkUser.Succeeded) { var result1 = await UserManager.AddToRoleAsync(User, "Admin"); }
            }
            // Create "User" Role
            IdentityResult userRoleResult;
            var userRoleCheck = await RoleManager.RoleExistsAsync("User");
            if (!userRoleCheck)
            {
                userRoleResult = await RoleManager.CreateAsync(new IdentityRole("User"));
            }
        }

        public static void Initialize(IServiceProvider serviceProvider)
        {
            using (var context = new BookStore1.Data.BookStore1Context(
            serviceProvider.GetRequiredService<DbContextOptions<BookStore1.Data.BookStore1Context>>()))
            {
                CreateUserRoles(serviceProvider).Wait();

                if (context.Author.Any() || context.Genre.Any() || context.Book.Any() || context.Review.Any() || context.UserBooks.Any())
                {
                    return;
                }
                context.Author.AddRange(
                new Author { /*Id = 1,*/ FirstName = "William", LastName = "Shakespeare", BirthDate = DateTime.Parse("23-04-1564"), Nationality = "English", Gender = "Male" },
                new Author {/* Id = 2,*/ FirstName = "Miguel", LastName = "De Cervantes", BirthDate = DateTime.Parse("29-09-1547"), Nationality = "Spanish", Gender = "Male" },
                new Author { /*Id = 3,*/ FirstName = "Joanne", LastName = "Rowling", BirthDate = DateTime.Parse("31-07-1965"), Nationality = "British", Gender = "Female" }
                );
                 context.SaveChanges();
                context.Genre.AddRange(
               new Genre
               {
                   // Id = 1,
                   GenreName = "Comedy"
               },
               new Genre
               {
                   // Id = 2,
                   GenreName = "Drama"
               },
               new Genre
               {
                   //Id = 3,
                   GenreName = "Fantasy"
               },
               new Genre
               {
                   //Id = 4,
                   GenreName = "Tragedy"
               },
               new Genre
               {
                   // Id = 5,
                   GenreName = "Novel"
               }
               );
                context.SaveChanges();
                context.Book.AddRange(
                new Book
                {
                   // Id = 1,
                    Title = "Harry Potter and the Philosopher's Stone",
                    AuthorId = context.Author.Single(d => d.FirstName == "Joanne" && d.LastName == "Rowling").Id,
                    FrontPage = "https://upload.wikimedia.org/wikipedia/en/6/6b/Harry_Potter_and_the_Philosopher%27s_Stone_Book_Cover.jpg",
                    NumPages=500,
                    YearPublished = 1997,
                    DownloadUrl= "https://docenti.unimc.it/antonella.pascali/teaching/2018/19055/files/ultima-lezione/harry-potter-and-the-philosophers-stone",
                    Publisher = "Bloomsbury"
                },
                new Book
                {
                   // Id = 2,
                    Title = "Harry Potter and the Chamber of Secrets",
                    AuthorId = context.Author.Single(d => d.FirstName == "Joanne" && d.LastName == "Rowling").Id,
                    FrontPage = "https://upload.wikimedia.org/wikipedia/en/5/5c/Harry_Potter_and_the_Chamber_of_Secrets.jpg",
                    YearPublished = 1998,
                    Publisher = "Bloomsbury",
                    NumPages = 400,
                    DownloadUrl= "https://www.academia.edu/43266152/_J_K_Rowling_Harry_Potter_and_The_Chamber_of_Secrets"

                },
                new Book
                {
                   // Id = 3,
                    Title = "Romeo and Juliet",
                    AuthorId = context.Author.Single(d => d.FirstName == "William" && d.LastName == "Shakespeare").Id,
                    FrontPage = "https://www.bookshare.org/cover/KH/KH12Khz4jKmutRboZRJQUQuL9DKAffO7wt5GflY-52w-MEDIUM.jpg",
                    YearPublished = 1597,
                    NumPages=150,
                    Publisher="None",
                    DownloadUrl= "https://www.williamshakespeare.net/ebook-romeo-and-juliet.jsp"
                },
                new Book
                {
                   // Id = 4,
                    Title = "Don Quixote",
                    AuthorId = context.Author.Single(d => d.FirstName == "Miguel" && d.LastName == "De Cervantes").Id,
                    FrontPage = "https://res.cloudinary.com/bloomsbury-atlas/image/upload/w_568,c_scale/jackets/9781847493774.jpg",
                    YearPublished = 1615,
                    DownloadUrl="https://www.gutenberg.org/ebooks/996",
                    NumPages=500,
                    Publisher="None"
                },
                new Book
                {
                   // Id = 5,
                    Title = "A Midsummer Night's Dream",
                    AuthorId = context.Author.Single(d => d.FirstName == "William" && d.LastName == "Shakespeare").Id,
                    FrontPage = "https://cdn.kobo.com/book-images/e03369e5-d511-4e25-b2f2-e1ead1961dbe/1200/1200/False/a-midsummer-night-s-dream-265.jpg",
                    YearPublished = 1600,
                    DownloadUrl= "https://www.gutenberg.org/ebooks/1514",
                    NumPages=300,
                    Publisher="None"
                },
                new Book
                {
                   // Id = 6,
                    Title = "Hamlet",
                    AuthorId = context.Author.Single(d => d.FirstName == "William" && d.LastName == "Shakespeare").Id,
                    FrontPage = "https://cdn.kobo.com/book-images/6f7cdacf-5233-4f80-a29a-ca1046f13c87/1200/1200/False/hamlet-451.jpg",
                    YearPublished = 1623,
                    Publisher="None",
                    NumPages=400,
                    DownloadUrl= "https://www.gutenberg.org/ebooks/1524"
                }
                );
                context.SaveChanges();
                context.BookGenre.AddRange(
                new BookGenre { BookId = 1, GenreId = 3 },
                new BookGenre { BookId = 2, GenreId = 3 },
                new BookGenre { BookId = 3, GenreId = 2 },
                new BookGenre { BookId = 3, GenreId = 4 },
                new BookGenre { BookId = 4, GenreId = 5 },
                new BookGenre { BookId = 5, GenreId = 1 },
                new BookGenre { BookId = 6, GenreId = 2 },
                new BookGenre { BookId = 6, GenreId = 4 }
                );
                context.SaveChanges();

                context.UserBooks.AddRange(
                new UserBooks { AppUser = "User1", BookId = 2 },
                new UserBooks { AppUser = "User2", BookId = 3 },
                new UserBooks { AppUser = "User3", BookId = 1 },
                new UserBooks { AppUser = "User4", BookId = 2 },
                new UserBooks { AppUser = "User5", BookId = 6 },
                new UserBooks { AppUser = "User6", BookId = 4 }
                );
                context.SaveChanges();
                context.Review.AddRange(
                new Review { BookId = 1, AppUser = "User2", Comment = "Comment1", Rating = 10 },
                new Review { BookId = 4, AppUser = "User3", Comment = "Comment2", Rating = 7 },
                new Review { BookId = 3, AppUser = "User5", Comment = "Comment3", Rating = 4 }
                );
                context.SaveChanges();
            }
        }
    }
}