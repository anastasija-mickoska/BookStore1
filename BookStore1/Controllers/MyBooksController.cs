using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using BookStore1.Data;
using BookStore1.Models;
using BookStore1.ViewModels;
using System.Security.Claims;
using System.Net;

namespace BookStore1.Controllers
{
    public class MyBooksController : Controller
    {
        private readonly BookStore1Context _context;
        private readonly MyBooks _myBooks;

        public MyBooksController(BookStore1Context context, MyBooks myBooks)
        {
            _context = context;
            _myBooks = myBooks;
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> BuyBook(int bookId)
        {
            var book = await _context.Book.FindAsync(bookId);
            if (book == null)
            {
                return NotFound();
            }
            else
            {
                _myBooks.Books.Add(book);
                _context.MyBooks.Add(_myBooks);
            }
            await _context.SaveChangesAsync();

            return RedirectToAction("Index", "MyBooks");
        }

        // GET: MyBooks
        public async Task<IActionResult> Index(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }
            var book = await _context.Book.FindAsync(id);
            if (book == null)
            {
                return NotFound();
            }
            else
            {
                _myBooks.Books.Add(book);
                _context.MyBooks.Add(_myBooks);
            }
            await _context.SaveChangesAsync();
            var myBooks = await _context.MyBooks.Include(b => b.Books).FirstOrDefaultAsync();
            if(myBooks == null)
            {
                return NotFound();
            }
            return View(myBooks);
        }

        [HttpPost]
        public async Task<IActionResult> CreateReview(MyBooksReviewViewModel viewModel)
        {
            var selectedBook = _myBooks.Books.FirstOrDefault(b => b.Id == viewModel.selectedBookId);
            if (selectedBook == null)
            {
                return RedirectToAction("Index");
            }

            var review = viewModel.Review;
            _context.Add(review);
            await _context.SaveChangesAsync();

            return RedirectToAction("Index");
        }

        // GET: MyBooks/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var myBooks = await _context.MyBooks.FirstOrDefaultAsync(m => m.Id == id);
            if (myBooks == null)
            {
                return NotFound();
            }

            return View(myBooks);
        }
    }


}
