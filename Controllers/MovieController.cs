using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Fall2024_Assignment3_gdhakal.Data;
using Fall2024_Assignment3_gdhakal.Models;
using System.Numerics;
using Fall2024_Assignment3_gdhakal.Services;

namespace Fall2024_Assignment3_gdhakal.Controllers
{
    public class MovieController : Controller
    {
        private readonly AzureOpenAIService _openAIService;
        private readonly ApplicationDbContext _context;

        public MovieController(AzureOpenAIService openAIService, ApplicationDbContext context)
        {
            _openAIService = openAIService;
            _context = context;
        }

        // GET: Movie
        public async Task<IActionResult> Index()
        {
            return View(await _context.Movie.ToListAsync());
        }

        // GET: Movie/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            // Load movie with related actors through MovieActor join table
            var movie = await _context.Movie
                .Include(m => m.MovieActors)
                .ThenInclude(ma => ma.Actor)  // Include actors through the join table
                .FirstOrDefaultAsync(m => m.Id == id);

            if (movie == null)
            {
                return NotFound();
            }

            // Fetch AI-generated reviews with sentiment
            var reviewsWithSentiment = await _openAIService.MovieReviewsMultipleCalls(movie.Title, movie.Year);

            // Calculate the average sentiment
            double averageSentiment = reviewsWithSentiment.Any() ? reviewsWithSentiment.Average(r => r.Sentiment) : 0.0;

            // Create the ViewModel and pass the data
            var viewModel = new MovieDetailsViewModel
            {
                Movie = movie,
                ReviewsWithSentiment = reviewsWithSentiment,
                AverageSentiment = averageSentiment,
                Actors = movie.MovieActors.Select(ma => ma.Actor).ToList()  // Add actors to the ViewModel
            };

            // Return the view with the ViewModel
            return View(viewModel);
        }


        // GET: Movie/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Movie/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Title,Imdb,Genre,Year")] Movie movie, IFormFile poster)
        {
            if (ModelState.IsValid)
            {
                if (poster != null || poster!.Length > 0)
                {
                    using var memoryStream = new MemoryStream();
                    poster.CopyTo(memoryStream);
                    movie.Poster = memoryStream.ToArray();
                }

                _context.Add(movie);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(movie);
        }

        // GET: Movie/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var movie = await _context.Movie.FindAsync(id);
            if (movie == null)
            {
                return NotFound();
            }
            return View(movie);
        }

        // POST: Movie/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Title,Imdb,Genre,Year")] Movie movie, IFormFile poster)
        {
            if (id != movie.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    if (poster != null && poster.Length > 0)
                    {
                        using var memoryStream = new MemoryStream();
                        await poster.CopyToAsync(memoryStream);
                        movie.Poster = memoryStream.ToArray(); // Convert the file to byte array
                    }
                    else
                    {
                        // If no new photo is uploaded, retain the existing photo
                        var existingMovie = await _context.Movie.AsNoTracking().FirstOrDefaultAsync(a => a.Id == id);
                        if (existingMovie != null)
                        {
                            movie.Poster = existingMovie.Poster; // Keep the existing photo
                        }
                    }

                    _context.Update(movie);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!MovieExists(movie.Id))
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
            return View(movie);
        }

        // GET: Movie/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var movie = await _context.Movie
                .FirstOrDefaultAsync(m => m.Id == id);
            if (movie == null)
            {
                return NotFound();
            }

            return View(movie);
        }

        // POST: Movie/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var movie = await _context.Movie.FindAsync(id);
            if (movie != null)
            {
                _context.Movie.Remove(movie);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool MovieExists(int id)
        {
            return _context.Movie.Any(e => e.Id == id);
        }
    }
}
