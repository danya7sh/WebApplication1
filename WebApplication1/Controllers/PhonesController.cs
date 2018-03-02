using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using WebApplication1.Models;
using Microsoft.AspNetCore.SignalR.Client;

namespace WebApplication1.Controllers
{
    public class PhonesController : Controller
    {
        private readonly MobileContext _context;

        public PhonesController(MobileContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index()
        {
            return View(await _context.Phones.ToListAsync());
        }

        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var phone = await _context.Phones.SingleOrDefaultAsync(m => m.Id == id);
            if (phone == null)
            {
                return NotFound();
            }

            return View(phone);
        }

        public IActionResult Create()
        {
            return View();
        }

        public async Task<IActionResult> Connection(String methodName, Phone phone)
        {
            var connection = new HubConnectionBuilder()
                .WithUrl("http://localhost:60199/chat")
                .Build();

            await connection.StartAsync();

            await connection.InvokeAsync("PhController", methodName, phone);

            return RedirectToAction("Index");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Phone phone)
        {
            if (ModelState.IsValid)
            {
                phone.CurrentDate = DateTime.Now;
                phone.CreatedDate = DateTime.Now;

                _context.Add(phone);
                await _context.SaveChangesAsync();
                return await Connection("Create", phone);
            }
            return View(phone);
        }

        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var phone = await _context.Phones.SingleOrDefaultAsync(m => m.Id == id);
            if (phone == null)
            {
                return NotFound();
            }
            return View(phone);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, Phone phone)
        {
            if (id != phone.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    phone.CurrentDate = DateTime.Now;

                    _context.Update(phone);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!PhoneExists(phone.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return await Connection("Edit", phone);
            }
            return View(phone);
        }

        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var phone = await _context.Phones.SingleOrDefaultAsync(m => m.Id == id);
            if (phone == null)
            {
                return NotFound();
            }

            return View(phone);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var phone = await _context.Phones.SingleOrDefaultAsync(m => m.Id == id);
            _context.Phones.Remove(phone);
            await _context.SaveChangesAsync();
            return await Connection("Delete", phone);
        }

        private bool PhoneExists(int id)
        {
            return _context.Phones.Any(e => e.Id == id);
        }
    }
}
