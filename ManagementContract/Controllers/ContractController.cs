using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ManagementContract.Data;
using ManagementContract.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Text;

namespace ManagementContract.Controllers
{
    public class ContractController : Controller
    {
        private readonly ApplicationDbContext _context;

        public ContractController(ApplicationDbContext context)
        {
            _context = context;
        }
        public IActionResult Index()
        {
            return View(_context.Contracts.ToList());
        }
        public IActionResult  ExportToCVS()
        {
            var builder = new StringBuilder();
            builder.AppendLine("RegistrationNumber,Institution,ClosingDate,ExpiryDate,TerminationDate");
            foreach (var collection in _context.Contracts)
            {
                builder.AppendLine($"{collection.RegistrationNumber},{collection.Institution},{collection.ClosingDate},{collection.ExpiryDate},{collection.TerminationDate}");
            }
            return File(Encoding.UTF8.GetBytes(builder.ToString()), "text/csv", "contracts.csv");

        }
        public IActionResult Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var contract = _context.Contracts.FirstOrDefault(m => m.Id == id);
            if (contract == null)
            {
                return NotFound();
            }

            return View(contract);
        }

        public IActionResult Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var contractModel = _context.Contracts.FirstOrDefault(m => m.Id == id);

            if (contractModel == null)
            {
                return NotFound();
            }

            ViewData["Client"] = _context.Clients.Where(c => c.Id == contractModel.ClientId).ToList().FirstOrDefault();

            var conectedConsultant = from p in _context.ConsultantContract
                                    where p.ContractId == id
                                    select p.Consultant;
            ViewData["AllConsultants"] = conectedConsultant.ToList();

            return View(contractModel);
        }
        // POST: Contract/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var contract = await _context.Contracts.FindAsync(id);
            _context.Contracts.Remove(contract);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }
        // POST: Contract/Create
        [HttpPost]
        public IActionResult Create(Contract contract, int[] consultants)
        {
            if (ModelState.IsValid)
            {
                if (consultants != null)
                {
                    foreach (var cons in _context.Consultants.Where(c => consultants.Contains(c.Id)))
                    {
                        ConsultantContract ConsCont = new ConsultantContract ();

                        ConsCont.ContractId = contract.Id;

                        ConsCont.Contract = contract;

                        ConsCont.ConsultantId = cons.Id;

                        ConsCont.Consultant = cons;

                        contract.Consultants.Add(ConsCont);

                    }
                }
                _context.Add(contract);
                _context.SaveChanges();
                return RedirectToAction(nameof(Index));
            }
            return View(contract);
        }
        public IActionResult Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var contract = _context.Contracts.Find(id);
            if (contract == null)
            {
                return NotFound();
            }
            return View(contract);
        }

        // POST: Contract/Edit/5
        public IActionResult Create()
        {
            ViewData["AllClients"] = _context.Clients.ToList();
            ViewData["AllConsultants"] = _context.Consultants.ToList();
            return View();
        }



        [HttpPost]
        public IActionResult  Edit(int id, Contract contract)
        {
            if (id != contract.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(contract);
                    _context.SaveChanges();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!ContractModelExists(contract.Id))
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
            return View(contract);
        }

        [HttpPost]
        public IActionResult Search(string searchphrase)
        {


            if (string.IsNullOrWhiteSpace(searchphrase))
            {
                return View(nameof(Index), _context.Consultants);
            }
            var result = _context.Contracts.FromSqlInterpolated($"SELECT * FROM Contracts WHERE Institution LIKE {searchphrase} OR RegistrationNumber LIKE {searchphrase} OR ExpiryDate LIKE {searchphrase} ").ToList();


            return View(nameof(Index), result);
        }

        private bool ContractModelExists(int id)
        {
            return _context.Contracts.Any(e => e.Id == id);
        }

    }
}
