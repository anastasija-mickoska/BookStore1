using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using BookStore1.Areas.Identity.Data;
using BookStore1.Models;
using System.IO;
using System.Numerics;

namespace BookStore1.Data
{
    public class BookStore1Context : IdentityDbContext<BookStore1User>
    {
        public BookStore1Context (DbContextOptions<BookStore1Context> options)
            : base(options)
        {
        }
        public DbSet<Book> Book { get; set; } = default!;
        public DbSet<Author> Author { get; set; } = default!;
        public DbSet<Genre> Genre { get; set; } = default!;
        public DbSet<Review> Review { get; set; } =default!;
        public DbSet<BookGenre> BookGenre { get; set; } = default!;
        public DbSet<UserBooks> UserBooks { get; set; } = default!;
        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);
 
            //eden zanr ima povekje knigi
            builder.Entity<BookGenre>()
            .HasOne<Genre>(p => p.Genre)
            .WithMany(p => p.Books)
            .HasForeignKey(p => p.GenreId);

            //edna kniga ima povekje zanrovi
            builder.Entity<BookGenre>()
            .HasOne<Book>(p => p.Book)
            .WithMany(p => p.Genres)
            .HasForeignKey(p => p.BookId);

            //za edna kniga ima povekje reviews, eden review e za edna kniga
            builder.Entity<Review>()
            .HasOne<Book>(p => p.Book)
            .WithMany(p => p.Reviews)
            .HasForeignKey(p => p.BookId);

            //edna kniga ima eden avtor, eden avtor ima povekje knigi
            builder.Entity<Book>()
            .HasOne<Author>(p => p.Author)
            .WithMany(p => p.Books)
            .HasForeignKey(p => p.AuthorId);

            //edna kniga moze da ja koristat povekje korisnici, eden korisnik koristi edna kniga
            builder.Entity<UserBooks>()
            .HasOne<Book>(p => p.Book)
            .WithMany(p => p.Users)
            .HasForeignKey(p => p.BookId);

            builder.Entity<UserBooks>()
            .HasOne(p => p.User)
            .WithMany(p => p.Books)
            .HasForeignKey(p => p.AppUser);

        } 
    }
}
