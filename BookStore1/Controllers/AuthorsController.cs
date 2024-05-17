using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using BookStore1.Data;
using BookStore1.ViewModels;
using BookStore1.Models;

namespace BookStore1.Controllers
{
    public class AuthorsController : Controller
    {
        private readonly BookStore1Context _context;

        public AuthorsController(BookStore1Context context)
        {
            _context = context;
        }

        // GET: Authors
        public async Task<IActionResult> Index(string searchString)
        {
            IQueryable<Author> authors = _context.Author.AsQueryable();

            if (!string.IsNullOrEmpty(searchString))
            {
                authors = authors.Where(a => a.FirstName.Contains(searchString)
                || a.LastName.Contains(searchString));
            }

            var authorList = await authors.ToListAsync();

            var viewModelList = authorList.Select(author => new AuthorBookViewModel
            {
                Author = author,
            });

            return View(viewModelList);
        }

        // GET: Authors/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var author = await _context.Author
                .Include(m => m.Books)
                .ThenInclude(m => m.Genres)
                .ThenInclude(m => m.Genre)
                .Include(m => m.Books)
                .ThenInclude(m => m.Reviews)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (author == null)
            {
                return NotFound();
            }
            var booksByAuthor = await _context.Book
            .Where(b => b.AuthorId == id)
            .ToListAsync();
            var viewModel = new AuthorBookViewModel
            {
                Author = author,
                BooksByAuthor = booksByAuthor
            };
            return View(viewModel);
        }

        // GET: Authors/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Authors/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,FirstName,LastName,BirthDate,Nationality,Gender")] Author author)
        {
            if (ModelState.IsValid)
            {
                _context.Add(author);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(author);
        }

        // GET: Authors/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var author =  _context.Author.Include(m => m.Books).FirstOrDefault(m => m.Id == id);

            if (author == null)
            {
                return NotFound();
            }
            var books = _context.Book.AsEnumerable();
            books = books.OrderBy(s => s.Title);

            if (author.Books != null)
            {
                var selected = author.Books.Select(s => s.Id).ToList();
                AuthorBookViewModel viewmodel = new AuthorBookViewModel
                {
                    Author = author,
                    BookList = new MultiSelectList(books, "Id", "Title"),
                    SelectedBooks = selected.Cast<int?>().ToList(),
                };
                return View(viewmodel);
            }
            else
            {
                AuthorBookViewModel viewmodel = new AuthorBookViewModel
                {
                    Author = author,
                    BookList = new MultiSelectList(books, "Id", "Title"),
                    SelectedBooks = new List<int>().Cast<int?>() // Provide an empty list of selected books
                };
                return View(viewmodel);
            }

        }

        // POST: Authors/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,FirstName,LastName,BirthDate,Nationality,Gender")] AuthorBookViewModel author)
        {
            if (id != author.Author.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(author);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!AuthorExists(author.Author.Id))
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
            return View(author);
        }

        // GET: Authors/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var author = await _context.Author
                .FirstOrDefaultAsync(m => m.Id == id);
            if (author == null)
            {
                return NotFound();
            }

            return View(author);
        }

        // POST: Authors/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var author = await _context.Author.FindAsync(id);
            if (author != null)
            {
                _context.Author.Remove(author);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool AuthorExists(int id)
        {
            return _context.Author.Any(e => e.Id == id);
        }
    }
}
