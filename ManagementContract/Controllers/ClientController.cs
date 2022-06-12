using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using ManagementContract.Data;
using ManagementContract.Models;
using Microsoft.Data.SqlClient;
using System.Text;

namespace ManagementContract.Controllers
{
    public class ClientController : Controller
    {
        private readonly ApplicationDbContext _context;


        public ClientController(ApplicationDbContext context)
        {
            _context = context;
        }

        public IActionResult ExportToCVS()
        {
            var builder = new StringBuilder();
            builder.AppendLine("Name,Surname,Mail,PhoneNumber,RodneCislo,Age");
            foreach (var clients in _context.Clients)
            {
                builder.AppendLine($"{clients.Name},{clients.Surname},{clients.Mail},{clients.PhoneNumber},{clients.RodneCislo},{clients.Age}");
            }
            return File(Encoding.UTF8.GetBytes(builder.ToString()), "text/csv", "clients.csv");

        }
        public async Task<IActionResult> Index()
        {
            return View(await _context.Clients.ToListAsync());
        }
        [HttpPost]
        public IActionResult Search(string searchphrase)
        {
            

            if (string.IsNullOrWhiteSpace(searchphrase))
            {
                return View(nameof(Index), _context.Clients);
            }
           var result= _context.Clients.FromSqlInterpolated($"SELECT * FROM Clients WHERE Name LIKE {searchphrase} OR Surname LIKE {searchphrase} OR Mail LIKE {searchphrase} ").ToList();

            return View(nameof(Index), result);
        }

        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var client = await _context.Clients.FindAsync(id);
            if (client == null)
            {
                return NotFound();
            }
            return View(client);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Name,Surname,Mail,PhoneNumber,RodneCislo,Age")] Client client)
        {
            if (id != client.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(client);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!ClientExists(client.Id))
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
            return View(client);
        }
        public async Task<IActionResult> Details(int? id)
        {

            if (id == null)
            {
                return NotFound();
            }

            var client = await _context.Clients
                .FirstOrDefaultAsync(m => m.Id == id);
            if (client == null)
            {
                return NotFound();
            }

            ViewData["AllContracts"] = _context.Contracts.Where(x => x.ClientId == id).ToList();

            return View(client);
        }


        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var client = await _context.Clients
                .FirstOrDefaultAsync(m => m.Id == id);

            if (client == null)
            {
                return NotFound();
            }

            return View(client);
        }

        // POST: Client/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var client = await _context.Clients.FindAsync(id);
            _context.Clients.Remove(client);

            List<Contract> conectedContracts = _context.Contracts.Where(a => a.ClientId == id).ToList();

            foreach (var item in conectedContracts)
            {
                _context.Contracts.Remove(item);
                await _context.SaveChangesAsync();
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }



        public IActionResult Create()
        {
            return View();
        }

        // POST: Client/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Name,Surname,Mail,PhoneNumber,RodneCislo,Age")] Client client)
        {
            
            if (ModelState.IsValid)
            {
                _context.Add(client);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(client);
        }
        private bool ClientExists(int id)
        {
            return _context.Clients.Any(e => e.Id == id);
        }
    }
}
