using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using HastaneNobetSistemi.Data;
using HastaneNobetSistemi.Models;

namespace HastaneNobetSistemi.Controllers
{
    public class BayramsController : Controller
    {
        private readonly AppDbContext _context;

        public BayramsController(AppDbContext context)
        {
            _context = context;
        }

        // GET: Bayrams
        public async Task<IActionResult> Index()
        {
            return _context.Bayramlar != null ?
                        View(await _context.Bayramlar.ToListAsync()) :
                        Problem("Entity set 'AppDbContext.Bayramlar'  is null.");
        }

        // GET: Bayrams/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null || _context.Bayramlar == null)
            {
                return NotFound();
            }

            var bayram = await _context.Bayramlar
                .FirstOrDefaultAsync(m => m.Id == id);
            if (bayram == null)
            {
                return NotFound();
            }

            return View(bayram);
        }

        // GET: Bayrams/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Bayrams/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,BayramAdi,BaslangicTarihi,BitisTarihi")] Bayram bayram)
        {
            if (ModelState.IsValid)
            {
                _context.Add(bayram);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(bayram);
        }

        // GET: Bayrams/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null || _context.Bayramlar == null)
            {
                return NotFound();
            }

            var bayram = await _context.Bayramlar.FindAsync(id);
            if (bayram == null)
            {
                return NotFound();
            }
            return View(bayram);
        }

        // POST: Bayrams/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,BayramAdi,BaslangicTarihi,BitisTarihi")] Bayram bayram)
        {
            if (id != bayram.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(bayram);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!BayramExists(bayram.Id))
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
            return View(bayram);
        }

        // GET: Bayrams/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null || _context.Bayramlar == null)
            {
                return NotFound();
            }

            var bayram = await _context.Bayramlar
                .FirstOrDefaultAsync(m => m.Id == id);
            if (bayram == null)
            {
                return NotFound();
            }

            return View(bayram);
        }

        // POST: Bayrams/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            if (_context.Bayramlar == null)
            {
                return Problem("Entity set 'AppDbContext.Bayramlar'  is null.");
            }
            var bayram = await _context.Bayramlar.FindAsync(id);
            if (bayram != null)
            {
                _context.Bayramlar.Remove(bayram);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool BayramExists(int id)
        {
            return (_context.Bayramlar?.Any(e => e.Id == id)).GetValueOrDefault();
        }
    }
}