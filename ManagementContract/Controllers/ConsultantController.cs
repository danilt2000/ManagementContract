using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using ManagementContract.Data;
using ManagementContract.Models;
using System.Text;

namespace ManagementContract.Controllers
{
    public class ConsultantController : Controller
    {
        private readonly ApplicationDbContext _context;

        public ConsultantController(ApplicationDbContext context)
        {
            _context = context;
        }
        public IActionResult ExportToCVS()
        {
            var builder = new StringBuilder();
            builder.AppendLine("Name,Surname,Mail,PhoneNumber,RodneCislo,Age");
            foreach (var consultants in _context.Consultants)
            {
                builder.AppendLine($"{consultants.Name},{consultants.Surname},{consultants.Mail},{consultants.PhoneNumber},{consultants.RodneCislo},{consultants.Age}");
            }
            return File(Encoding.UTF8.GetBytes(builder.ToString()), "text/csv", "consultants.csv");

        }

        // GET: Consultant
        public async Task<IActionResult> Index()
        {
            return View(await _context.Consultants.ToListAsync());
        }

        [HttpPost]
        public IActionResult Search(string searchphrase)
        {


            if (string.IsNullOrWhiteSpace(searchphrase))
            {
                return View(nameof(Index), _context.Consultants);
            }
            var result = _context.Consultants.FromSqlInterpolated($"SELECT * FROM Consultants WHERE Name LIKE {searchphrase} OR Surname LIKE {searchphrase} OR Mail LIKE {searchphrase} ").ToList();


            return View(nameof(Index), result);
        }
        // GET: Consultant/Details/5
        public async Task<IActionResult> Details(int? id)//ТУТ ПЕРЕВРОВЕРИТЬ 
        {
            if (id == null)
            {
                return NotFound();
            }

            var consultant = await _context.Consultants
                .FirstOrDefaultAsync(m => m.Id == id);
            if (consultant == null)
            {
                return NotFound();
            }

           
            var conectedContracts = from p in _context.ConsultantContract
                                 where p.ConsultantId == id
                                 select p.Contract;
            ViewData["AllContratcsWithConsultant"]= conectedContracts.ToList();

           
            ViewData["AllClients"] = _context.Clients.ToList();
            return View(consultant);
        }

        // GET: Consultant/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Consultant/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Name,Surname,Mail,PhoneNumber,RodneCislo,Age")] Consultant consultant)
        {
            if (ModelState.IsValid)
            {
                _context.Add(consultant);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(consultant);
        }

        // GET: Consultant/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var consultant = await _context.Consultants.FindAsync(id);
            if (consultant == null)
            {
                return NotFound();
            }
            return View(consultant);
        }

        // POST: Consultant/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Name,Surname,Mail,PhoneNumber,RodneCislo,Age")] Consultant consultant)
        {
            if (id != consultant.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(consultant);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!ConsultantModelExists(consultant.Id))
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
            return View(consultant);
        }

        // GET: Consultant/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var consultant = await _context.Consultants
                .FirstOrDefaultAsync(m => m.Id == id);

            if (consultant == null)
            {
                return NotFound();
            }

            return View(consultant);
        }

       // POST: Consultant/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
             
            var consultant = await _context.Consultants.FindAsync(id);
            _context.Consultants.Remove(consultant);

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool ConsultantModelExists(int id)
        {
            return _context.Consultants.Any(e => e.Id == id);
        }
    }
}
