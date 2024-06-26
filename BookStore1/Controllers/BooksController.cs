﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using BookStore1.Data;
using BookStore1.Models;
using System.IO;
using BookStore1.ViewModels;
using System.Net;
using Microsoft.AspNetCore.Authorization;
using BookStore1.Areas.Identity.Data;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Hosting;

namespace BookStore1.Controllers
{
    public class BooksController : Controller
    {
        private readonly BookStore1Context _context;
        private readonly UserManager<BookStore1User> _userManager;
        private readonly IWebHostEnvironment _environment;

        public BooksController(BookStore1Context context, UserManager<BookStore1User> userManager, IWebHostEnvironment environment)
        {
            _context = context;
            _userManager = userManager;
            _environment= environment;

        }
        [HttpPost]
        [Authorize(Roles="User")]
        public async Task<IActionResult> BuyBook(int id)
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                return Challenge(); 
            }

            var book = await _context.Book.FindAsync(id);
            if (book == null)
            {
                return NotFound();
            }

            var userBook = new UserBooks
            {
                AppUser = user.Id, 
                BookId = book.Id,
            };

            _context.UserBooks.Add(userBook);
            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(MyBooks));
        }

        [Authorize(Roles ="User")]
        public async Task<IActionResult> MyBooks()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                return Challenge(); 
            }

            var userBooks = await _context.UserBooks
                .Include(ub => ub.Book)
                .Where(ub => ub.AppUser == user.Id) 
                .ToListAsync();

            return View(userBooks);
        }
    // GET: Books
        public async Task<IActionResult> Index(string searchstring)
        {
            IQueryable<Book> books = _context.Book.Include(m => m.Author).Include(m => m.Genres).ThenInclude(m => m.Genre).Include(m => m.Reviews);
            IQueryable<string> title = _context.Book.OrderBy(b => b.Title).Select(b => b.Title).Distinct();

            if (!string.IsNullOrEmpty(searchstring))
            {
                books = books.Where(b =>
                b.Title.Contains(searchstring) ||
                b.Author.FirstName.Contains(searchstring) ||
                b.Author.LastName.Contains(searchstring) ||
                b.Genres.Any(b => b.Genre.GenreName.Contains(searchstring)
                ));
            }
            if(books != null)
            {
                foreach (var book in books)
                {
                    var distinctGenres = book.Genres
                        .Where(b => b.Genre != null)
                        .GroupBy(b => b.GenreId)
                        .Select(b => b.First())
                        .ToList();

                    book.Genres = distinctGenres;
                }
            }

             var book1 = new BooksFiltersViewModel
               {   
                   Books = await books.ToListAsync(),
                   Filters = new SelectList(await title.ToListAsync())
               };

               return View(book1); 
        } 

        // GET: Books/Details/5
        public async Task<IActionResult> Details(int? id)
        {

            if (id == null)
            {
                return NotFound();
            }

            var book = await _context.Book
                .Include(m => m.Author)
                .Include(m => m.Genres)
                .ThenInclude(m => m.Genre)
                .Distinct()
                .Include(m => m.Reviews)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (book == null)
            {
                return NotFound();
            }

            var averageRating = book.AverageRating();
            ViewData["AverageRating"] = averageRating;


            return View(book);
        }

        // GET: Books/Create
        [Authorize(Roles ="Admin")]
        public async Task<IActionResult> Create()
        {
            var genres = await _context.Genre.ToListAsync();
            ViewBag.Genres = new MultiSelectList(genres, "Id", "GenreName");
            ViewBag.AuthorId = new SelectList(await _context.Author.ToListAsync(), "Id", "FullName");
            return View();
        }

        // POST: Books/Create
        [Authorize(Roles = "Admin")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Title,YearPublished,NumPages,Description,Publisher,FrontPage,DownloadUrl,AuthorId")] Book book, int[] selected, IFormFile FrontPageFile, IFormFile DownloadUrlFile)
        {
            if (!ModelState.IsValid) 
        {
                if (FrontPageFile != null)
                {
                    var frontPagePath = Path.Combine(_environment.WebRootPath, "uploads", FrontPageFile.FileName);
                    using (var stream = new FileStream(frontPagePath, FileMode.Create))
                    {
                        await FrontPageFile.CopyToAsync(stream);
                    }
                    book.FrontPage = $"/uploads/{FrontPageFile.FileName}";
                }

                if (DownloadUrlFile != null)
                {
                    var downloadUrlPath = Path.Combine(_environment.WebRootPath, "uploads", DownloadUrlFile.FileName);
                    using (var stream = new FileStream(downloadUrlPath, FileMode.Create))
                    {
                        await DownloadUrlFile.CopyToAsync(stream);
                    }
                    book.DownloadUrl = $"/uploads/{DownloadUrlFile.FileName}";
                }
                _context.Add(book);
                if (selected != null)
                {
                    foreach (var genreId in selected)
                    {
                        _context.Add(new BookGenre { BookId = book.Id, GenreId = genreId });
                    }
                    await _context.SaveChangesAsync();
                    return RedirectToAction(nameof(Index));
                }
         }
            var authors = await _context.Author.ToListAsync();
            ViewBag.AuthorId = new SelectList(authors, "Id", "FullName", book.AuthorId);

            var genres = _context.Genre.ToList();
            var genreItems = genres.Select(genre => new SelectListItem
            {
                Text = genre.GenreName,
                Value = genre.Id.ToString(),
                Selected = book.Genres.Any(bg => bg.GenreId == genre.Id)
            }).ToList();

            ViewBag.Genres = genreItems;

            return View(book);
}


        // GET: Books/Edit/5
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var book = await _context.Book.Include(b => b.Genres).FirstOrDefaultAsync(b => b.Id == id);
            if (book == null)
            {
                return NotFound();
            }
            var allGenres = _context.Genre.ToList();

            var genreItems = allGenres.Select(genre => new SelectListItem
            {
                Text = genre.GenreName,
                Value = genre.Id.ToString(),
                Selected = book.Genres.Any(bg => bg.GenreId == genre.Id) 
            }).ToList();

            ViewBag.Genres = genreItems;

            var authors = await _context.Author.ToListAsync();
            ViewBag.AuthorId = new SelectList(authors, "Id", "FullName");

            return View(book);
        }

        // POST: Books/Edit/5
        [Authorize(Roles = "Admin")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Title,YearPublished,NumPages,Description,Publisher,FrontPage,DownloadUrl,AuthorId")] Book book, int[] selected, IFormFile FrontPageFile, IFormFile DownloadUrlFile)
        {
            if (id != book.Id)
            {
                return NotFound();
            }

            if (!ModelState.IsValid)
            {
                if (FrontPageFile != null)
                {
                    var frontPagePath = Path.Combine(_environment.WebRootPath, "uploads", FrontPageFile.FileName);
                    using (var stream = new FileStream(frontPagePath, FileMode.Create))
                    {
                        await FrontPageFile.CopyToAsync(stream);
                    }
                    book.FrontPage = $"/uploads/{FrontPageFile.FileName}";
                }

                if (DownloadUrlFile != null)
                {
                    var downloadUrlPath = Path.Combine(_environment.WebRootPath, "uploads", DownloadUrlFile.FileName);
                    using (var stream = new FileStream(downloadUrlPath, FileMode.Create))
                    {
                        await DownloadUrlFile.CopyToAsync(stream);
                    }
                    book.DownloadUrl = $"/uploads/{DownloadUrlFile.FileName}";
                }

                try
                {
                    var existingGenres = _context.BookGenre.Where(b => b.BookId == id);
                    _context.BookGenre.RemoveRange(existingGenres);

                    if (selected != null)
                    {
                        foreach (var genreId in selected)
                        {
                            _context.Add(new BookGenre { BookId = book.Id, GenreId = genreId });
                        }
                    }
                    _context.Update(book);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!BookExists(book.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            ViewBag.AuthorId = new SelectList(_context.Author, "Id", "FullName", book.AuthorId);
            return View(book);
        }

        // GET: Books/Delete/5
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var book = await _context.Book
                .Include(b => b.Author)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (book == null)
            {
                return NotFound();
            }
            ViewData["Author"] = new SelectList(_context.Set<Author>(), "Id", "FullName", book.AuthorId);
            return View(book);
        }

        // POST: Books/Delete/5
        [Authorize(Roles = "Admin")]
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var book = await _context.Book.FindAsync(id);
            if (book != null)
            {
                _context.Book.Remove(book);
            }
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool BookExists(int id)
        {
            return _context.Book.Any(e => e.Id == id);
        }
    }
}
