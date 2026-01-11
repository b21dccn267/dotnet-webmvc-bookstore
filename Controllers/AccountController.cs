using System.Diagnostics;
using System.Security.Claims;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using newltweb.Models;
using newltweb.Services;
using Microsoft.Extensions.Logging;

namespace newltweb.Controllers
{
    //[Route("accounts")]
    public class AccountController : Controller
    {
        private readonly ILogger<AccountController> _logger;
        private readonly LtwebBtlContext _db;
        private readonly IUserIdProvider _userIdProvider;

        public AccountController(ILogger<AccountController> logger, LtwebBtlContext db, IUserIdProvider userIdProvider)
        {
            _logger = logger;
            _db = db;
            _userIdProvider = userIdProvider;
        }

        [HttpGet("accounts")]
        public IActionResult Index()
        {
            return View();
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login(string username, string password, bool rememberMe = false, string returnUrl = null)
        {
            if (string.IsNullOrWhiteSpace(username) || string.IsNullOrWhiteSpace(password))
            {
                ModelState.AddModelError(string.Empty, "Invalid username or password");
                ViewData["ReturnUrl"] = returnUrl;
                return View();
            }

           
            var match = await _db.TUsers.AnyAsync(u => u.Username == username && u.Password == password);

            if (!match)
            {
                await Task.Delay(200);
                ModelState.AddModelError(string.Empty, "Invalid username or password");
                ViewData["ReturnUrl"] = returnUrl;
                return View();
            }

            var user = await _db.TUsers
                .Where(u => u.Username == username)
                .Select(u => new { u.Username, u.IdKhachHang, u.IdNhanVien })
                .FirstOrDefaultAsync();

            string? associatedId = null;
            string? customerId = null;
            string? employeeId = null;

            if (user != null)
            {
                customerId = string.IsNullOrWhiteSpace(user.IdKhachHang) ? null : user.IdKhachHang;
                employeeId = string.IsNullOrWhiteSpace(user.IdNhanVien) ? null : user.IdNhanVien;

                associatedId = customerId ?? employeeId;
            }

            GlobalCurrentUser.Set(username.Trim(), associatedId);

            const string adminIdentifier = "000";

            if (customerId == null && string.Equals(employeeId, adminIdentifier, StringComparison.OrdinalIgnoreCase))
            {
                return RedirectToAction("Index", "Admin");
            }

            return RedirectToAction("Index", "Shop");
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
