﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Fall2024_Assignment3_gdhakal.Data;
using Fall2024_Assignment3_gdhakal.Models;

namespace Fall2024_Assignment3_gdhakal.Controllers
{
    public class MovieActorController : Controller
    {
        private readonly ApplicationDbContext _context;

        public MovieActorController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: MovieActor
        public async Task<IActionResult> Index()
        {
            var applicationDbContext = _context.MovieActor.Include(m => m.Actor).Include(m => m.Movie);
            return View(await applicationDbContext.ToListAsync());
        }

        // GET: MovieActor/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var movieActor = await _context.MovieActor
                .Include(m => m.Actor)
                .Include(m => m.Movie)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (movieActor == null)
            {
                return NotFound();
            }

            return View(movieActor);
        }

        // GET: MovieActor/Create
        public IActionResult Create()
        {
            ViewData["ActorID"] = new SelectList(_context.Actor, "Id", "Name");
            ViewData["MovieID"] = new SelectList(_context.Movie, "Id", "Title");
            return View();
        }

        // POST: MovieActor/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,MovieID,ActorID")] MovieActor movieActor)
        {
            bool alreadyExists = await _context.MovieActor
                .AnyAsync(ma => ma.MovieID == movieActor.MovieID && ma.ActorID == movieActor.ActorID);

            if (ModelState.IsValid && !alreadyExists)
            {
                _context.Add(movieActor);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }

            // Add an error if the actor has already been added for this movie
            ModelState.AddModelError("", "Cannot add the same actor multiple times for the same movie");

            // Re-populate the dropdowns in case of validation failure
            ViewData["ActorID"] = new SelectList(_context.Actor, "Id", "Name", movieActor.ActorID);
            ViewData["MovieID"] = new SelectList(_context.Movie, "Id", "Title", movieActor.MovieID);

            return View(movieActor);
        }

        // GET: MovieActor/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var movieActor = await _context.MovieActor.FindAsync(id);
            if (movieActor == null)
            {
                return NotFound();
            }
            ViewData["ActorID"] = new SelectList(_context.Actor, "Id", "Name", movieActor.ActorID);
            ViewData["MovieID"] = new SelectList(_context.Movie, "Id", "Title", movieActor.MovieID);
            return View(movieActor);
        }

        // POST: MovieActor/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,MovieID,ActorID")] MovieActor movieActor)
        {
            if (id != movieActor.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(movieActor);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!MovieActorExists(movieActor.Id))
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
            ViewData["ActorID"] = new SelectList(_context.Actor, "Id", "Name", movieActor.ActorID);
            ViewData["MovieID"] = new SelectList(_context.Movie, "Id", "Title", movieActor.MovieID);
            return View(movieActor);
        }

        // GET: MovieActor/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var movieActor = await _context.MovieActor
                .Include(m => m.Actor)
                .Include(m => m.Movie)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (movieActor == null)
            {
                return NotFound();
            }

            return View(movieActor);
        }

        // POST: MovieActor/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var movieActor = await _context.MovieActor.FindAsync(id);
            if (movieActor != null)
            {
                _context.MovieActor.Remove(movieActor);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool MovieActorExists(int id)
        {
            return _context.MovieActor.Any(e => e.Id == id);
        }
    }
}
