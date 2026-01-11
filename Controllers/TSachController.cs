using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using newltweb.Models;

namespace newltweb.Controllers
{
    public class TSachController : Controller
    {
        private readonly LtwebBtlContext _context;

        public TSachController(LtwebBtlContext context)
        {
            _context = context;
        }

        // GET: TSach
        public async Task<IActionResult> Index()
        {
            var ltwebBtlContext = _context.TSaches.Include(t => t.MaNxbNavigation).Include(t => t.MaTheLoaiNavigation);
            return View(await ltwebBtlContext.ToListAsync());
        }

        // GET: TSach/Details/5
        public async Task<IActionResult> Details(string id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var tSach = await _context.TSaches
                .Include(t => t.MaNxbNavigation)
                .Include(t => t.MaTheLoaiNavigation)
                .FirstOrDefaultAsync(m => m.MaSach == id);
            if (tSach == null)
            {
                return NotFound();
            }

            return View(tSach);
        }

        // GET: TSach/Create
        public IActionResult Create()
        {
            ViewData["MaNxb"] = new SelectList(_context.TNhaXuatBans, "MaNxb", "MaNxb");
            ViewData["MaTheLoai"] = new SelectList(_context.TTheLoais, "MaTheLoai", "MaTheLoai");
            return View();
        }

        // POST: TSach/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("MaSach,TenSach,TacGia,MaTheLoai,MaNxb,DonGiaNhap,DonGiaBan,SoLuong,SoTrang,TrongLuong,Anh")] TSach tSach)
        {
            if (ModelState.IsValid)
            {
                _context.Add(tSach);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["MaNxb"] = new SelectList(_context.TNhaXuatBans, "MaNxb", "MaNxb", tSach.MaNxb);
            ViewData["MaTheLoai"] = new SelectList(_context.TTheLoais, "MaTheLoai", "MaTheLoai", tSach.MaTheLoai);
            return View(tSach);
        }

        // GET: TSach/Edit/5
        public async Task<IActionResult> Edit(string id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var tSach = await _context.TSaches.FindAsync(id);
            if (tSach == null)
            {
                return NotFound();
            }
            ViewData["MaNxb"] = new SelectList(_context.TNhaXuatBans, "MaNxb", "MaNxb", tSach.MaNxb);
            ViewData["MaTheLoai"] = new SelectList(_context.TTheLoais, "MaTheLoai", "MaTheLoai", tSach.MaTheLoai);
            return View(tSach);
        }

        // POST: TSach/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(string id, [Bind("MaSach,TenSach,TacGia,MaTheLoai,MaNxb,DonGiaNhap,DonGiaBan,SoLuong,SoTrang,TrongLuong,Anh")] TSach tSach)
        {
            if (id != tSach.MaSach)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(tSach);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!TSachExists(tSach.MaSach))
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
            ViewData["MaNxb"] = new SelectList(_context.TNhaXuatBans, "MaNxb", "MaNxb", tSach.MaNxb);
            ViewData["MaTheLoai"] = new SelectList(_context.TTheLoais, "MaTheLoai", "MaTheLoai", tSach.MaTheLoai);
            return View(tSach);
        }

        // GET: TSach/Delete/5
        public async Task<IActionResult> Delete(string id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var tSach = await _context.TSaches
                .Include(t => t.MaNxbNavigation)
                .Include(t => t.MaTheLoaiNavigation)
                .FirstOrDefaultAsync(m => m.MaSach == id);
            if (tSach == null)
            {
                return NotFound();
            }

            return View(tSach);
        }

        // POST: TSach/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(string id)
        {
            var tSach = await _context.TSaches.FindAsync(id);
            if (tSach != null)
            {
                _context.TSaches.Remove(tSach);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool TSachExists(string id)
        {
            return _context.TSaches.Any(e => e.MaSach == id);
        }
    }
}
