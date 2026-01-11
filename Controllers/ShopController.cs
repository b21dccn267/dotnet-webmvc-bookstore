using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using Microsoft.IdentityModel.Tokens;
using newltweb.Models;

namespace newltweb.Controllers
{
    public class ShopController : Controller
    {
        private readonly LtwebBtlContext _db;
        public ShopController(LtwebBtlContext db)
        {
            _db = db;
        }
        //pageSize = number of items in one page, adjust as needed
        [HttpGet]
        public async Task<IActionResult> Index(string? q, string? categoryId, string? sort, int page = 1, int pageSize = 12)
        {
            ViewBag.Categories = await _db.TTheLoais.OrderBy(tl => tl.TenTheLoai).ToListAsync();
            var query = _db.TSaches
                        .AsNoTracking()
                        .Include(s => s.MaTheLoaiNavigation)
                        .AsQueryable();

            if (!string.IsNullOrEmpty(categoryId)) {
                query = query.Where(s => s.MaTheLoai == categoryId);
                ViewBag.SelectedCategoryId = categoryId;
            }
            else {
                ViewBag.SelectedCategoryId = null;
            }

            if (!string.IsNullOrWhiteSpace(q))
            {
                string term = $"%{q.Trim()}%";
                query = query.Where(s =>
                    EF.Functions.Like(s.TenSach, term) ||
                    EF.Functions.Like(s.TacGia, term));
            }

            var key = (sort ?? string.Empty).Trim().ToLowerInvariant();
            query = key switch
            {
                "default" => query.OrderBy(s => s.MaSach),
                "price-asc" => query.OrderBy(s => s.DonGiaBan).ThenBy(s => s.TenSach),
                "price-desc" => query.OrderByDescending(s => s.DonGiaBan).ThenBy(s => s.TenSach),
                //"popularity" => query.OrderByDescending(s => s.SalesCount).ThenBy(s => s.TenSach),
                "newest" => query.OrderByDescending(s => s.MaSach).ThenBy(s => s.TenSach),
                _ => query.OrderBy(s => s.MaSach)
            };

            var totalItems = await query.CountAsync();

            var books = await query.Skip((page - 1) * pageSize)
                                   .Take(pageSize)
                                   .ToListAsync();
            /*
            // compute sales counts per book (sum of quantities). returns 0 when no rows.
            var counts = await _db.OrderLines
                .GroupBy(ol => ol.MaSach)
                .Select(g => new { MaSach = g.Key, Count = g.Sum(x => x.SoLuong) })
                .ToDictionaryAsync(x => x.MaSach, x => x.Count);

            ViewBag.SalesCounts = counts; // Dictionary<int,int>*/

            ViewBag.Query = q ?? string.Empty;
            ViewBag.Sort = sort;
            ViewBag.Page = page;
            ViewBag.PageSize = pageSize;
            ViewBag.TotalItems = totalItems;
            ViewBag.TotalPages = (int)Math.Ceiling(totalItems / (double)pageSize);
            ViewBag.Breadcrumbs = new List<(string Text, string Href)> {
                ("Trang chủ", Url.Action("Index", "Home")),
                ("Sản phẩm", null),
            };

            return View(books);
        }

        [HttpGet]
        public async Task<IActionResult> Details(string id)
        {
            ViewBag.Categories = await _db.TTheLoais.OrderBy(tl => tl.TenTheLoai).ToListAsync();
            var book = await _db.TSaches.Include(s => s.MaTheLoaiNavigation)
                                .Include(s => s.MaNxbNavigation)
                                .FirstOrDefaultAsync(s => s.MaSach == id);
            if (book == null) return NotFound();
            var tl = book.MaTheLoai;
            var items = await _db.TSaches
                            .Where(s => s.MaTheLoai == tl)
                            .OrderByDescending(s => s.MaSach)
                            .Take(4)
                            .ToListAsync();
            ViewBag.RelatedBooks = items;
            ViewBag.Breadcrumbs = new List<(string Text, string Href)> {
                ("Trang chủ", Url.Action("Index", "Home")),
                ("Sản phẩm", Url.Action("Index", "Shop")),
                (book.TenSach, null)
            };
            return View(book);
        }
    }
}
