using Microsoft.AspNetCore.Cors.Infrastructure;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using newltweb.DTOs.Cart;
using newltweb.Models;
using newltweb.Services;
using System.Diagnostics;
using IUserIdProvider = newltweb.Services.IUserIdProvider;

namespace newltweb.Controllers
{
    public class CartController : Controller
    {
        private readonly ILogger<CartController> _logger;
        private readonly ICartService _cartService;
        private readonly IOrderService _orderService;
        private readonly IUserIdProvider _userIdProvider;

        public CartController(ICartService cartService, IOrderService orderService, IUserIdProvider userIdProvider, ILogger<CartController> logger)
        {
            _cartService = cartService;
            _orderService = orderService;
            _userIdProvider = userIdProvider;
            _logger = logger;
        }

        [HttpGet("cart")]
        public async Task<IActionResult> Index()
        {
            var (username, userId) = GlobalCurrentUser.Get();
            var snapshot = await _cartService.GetCartSnapshotAsync(userId);

            ViewBag.Breadcrumbs = new List<(string Text, string Href)> {
                ("Trang chủ", Url.Action("Index", "Home")),
                ("Giỏ hàng", null)
            };
            return View(snapshot);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Add(string maSach, int soLuong = 1)
        {
            var (username, userId) = GlobalCurrentUser.Get();
            await _cartService.AddToCartAsync(userId, maSach, soLuong);
            /*if (Request.Headers["X-Requested-With"] == "XMLHttpRequest")
            {
                var count = await _cartService.GetCartItemCountAsync(CurrentUserId());
                return Json(new { success = true, cartItemCount = count });
            }*/
            return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Buy([FromForm] int cartId)
        {
            var (username, userId) = GlobalCurrentUser.Get();

            var snapshot = await _cartService.GetCartSnapshotAsync(userId);
            if (snapshot == null || snapshot.MaGh != cartId || snapshot.Items == null || !snapshot.Items.Any())
            {
                ModelState.AddModelError("", "Giỏ hàng không hợp lệ");
                return View("Index", snapshot ?? new CartSnapshot());
            }

            var validation = await _cartService.ValidateCartForCheckoutAsync(userId);
            if (!validation.IsValid)
            {
                ModelState.AddModelError("", validation.ErrorMessage);

                snapshot = await _cartService.GetCartSnapshotAsync(userId);
                return View("Index", snapshot);
            }

            var orderResult = await _orderService.CreateOrderFromCartAsync(userId);
            if (!orderResult.Success)
            {
                ModelState.AddModelError("", orderResult.ErrorMessage ?? "Không thể tạo đơn hàng");
                snapshot = await _cartService.GetCartSnapshotAsync(userId);
                return View("Index", snapshot);
            }

            return RedirectToAction("Confirm", "Order", new { id = orderResult.OrderId });
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Update(int maSachGH, int soLuong)
        {
            var (username, userId) = GlobalCurrentUser.Get();

            await _cartService.UpdateQuantityAsync(userId, maSachGH, soLuong);
            return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ChangeAmount(int maSachGH, int amount, int delta)
        {
            var (username, userId) = GlobalCurrentUser.Get();

            await _cartService.UpdateQuantityAsync(userId, maSachGH, amount+delta);
            return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Remove(int maSachGH)
        {
            var (username, userId) = GlobalCurrentUser.Get();

            await _cartService.RemoveItemAsync(userId, maSachGH);
            return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Clear()
        {
            var (username, userId) = GlobalCurrentUser.Get();

            await _cartService.ClearCartAsync(userId);
            return RedirectToAction(nameof(Index));
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
