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
using BookStore1.Areas.Identity.Data;

namespace BookStore1.Controllers
{
    public class ReviewsController : Controller
    {
        private readonly BookStore1Context _context;

        public ReviewsController(BookStore1Context context)
        {
            _context = context;
        }

        // GET: Reviews
        public async Task<IActionResult> Index()
        { 
            var bookContext = _context.Review
                .Include(m => m.Book);
            return View(await bookContext.ToListAsync());
        }

        // GET: Reviews/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var review = await _context.Review
                .Include(m => m.Book)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (review == null)
            {
                return NotFound();
            }

            return View(review);
        }

        // GET: Reviews/Create
        public IActionResult Create()
        {
            var books = _context.Book.ToList();
            ViewBag.BookId = new SelectList(books, "Id", "Title");
            return View();
        }

        // POST: Reviews/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,BookId,AppUser,Comment,Rating")] Review review)
        {
            if (ModelState.IsValid)
            {
                _context.Add(review);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            var books = _context.Book.ToList(); 
            ViewBag.BookId = new SelectList(books, "Id", "Title");
            var users = await _context.Users.ToListAsync();
            ViewBag.AppUsers = new SelectList(users, "Id", "Name");
            return View(review);
        }
        //Get CreateReview
        public async Task<IActionResult> CreateReview()
        {
            var currentUser = await _context.Users.SingleOrDefaultAsync(u => u.UserName == User.Identity.Name);
            if (currentUser == null)
            {
                return NotFound();
            }
            var books = await _context.UserBooks
                                       .Include(b => b.Book)
                                       .Where(b => b.AppUser == currentUser.Id)
                                       .Select(b => new SelectListItem
                                       {
                                           Value = b.Id.ToString(),
                                           Text = b.Book.Title
                                       })
                                       .ToListAsync();
            var viewModel = new CreateReviewViewModel
            {
                Review = new Review(),
                BoughtBooks = books,
                User = currentUser
            };
            return View(viewModel);
        }
        [HttpPost]
        public async Task<IActionResult> CreateReview(CreateReviewViewModel viewModel)
        {
            if (!ModelState.IsValid)
            {
                _context.Add(viewModel.Review);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(viewModel);
        }

        // GET: Reviews/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var review = await _context.Review.FindAsync(id);
            if (review == null)
            {
                return NotFound();
            }
            ViewData["BookId"] = new SelectList(_context.Book, "Id", "Title", review.BookId);
            return View(review);
        }

        // POST: Reviews/Edit/5

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,BookId,AppUser,Comment,Rating")] Review review)
        {
            if (id != review.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(review);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!ReviewExists(review.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                ViewData["BookId"] = new SelectList(_context.Book, "Id", "Title", review.BookId);
                return RedirectToAction(nameof(Index));
            }
            return View(review);
        }

        // GET: Reviews/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var review = await _context.Review
                .Include(m => m.Book)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (review == null)
            {
                return NotFound();
            }

            return View(review);
        }

        // POST: Reviews/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var review = await _context.Review.FindAsync(id);
            if (review != null)
            {
                _context.Review.Remove(review);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool ReviewExists(int id)
        {
            return _context.Review.Any(e => e.Id == id);
        }
    }
}
