using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Security.Claims;
using System.Text.Json;
using System.Threading.Tasks;
using fashion_shop.Core.Entities;
using fashion_shop.Core.Interfaces.Repositories;
using fashion_shop.Core.Interfaces.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace fashion_shop.MVC.Controllers;

[Route("cart")]
[Authorize]
public class CartController : Controller
{
    public class CartItem
    {
        public int ProductId { get; set; }
        public int Quantity { get; set; }
    }

    private readonly ILogger<CartController> _logger;
    private readonly SignInManager<User> _signInManager;
    private readonly UserManager<User> _userManager;
    private readonly ICartService _cartService;
    private readonly IProductService _productService;

    public CartController(
        ILogger<CartController> logger,
        SignInManager<User> signInManager,
        ICartService cartService,
        IProductService productService,
        UserManager<User> userManager)
    {
        _logger = logger;
        _signInManager = signInManager;
        _cartService = cartService;
        _productService = productService;
        _userManager = userManager;
    }

    [HttpGet("index")]
    public async Task<IActionResult> Index()
    {
        var cart = GetCart();

        var cartItems = cart.ToDictionary(item => item.ProductId, item => item.Quantity);

        var cartItemData = await _cartService.GetListAsync(cartItems);

        ViewBag.CartItems = cartItemData;

        return View();
    }

    [HttpGet("add-to-cart")]
    public async Task<IActionResult> AddToCart(int productId)
    {
        var cart = GetCart();

        // check exist product
        var existingItem = cart?.FirstOrDefault(c => c.ProductId == productId);

        if (await _productService.GetDetailAsync(productId) is null)
        {
            return NotFound();
        }

        if (existingItem == null)
        {
            cart?.Add(new CartItem { ProductId = productId, Quantity = 1 });
        }

        // Update Cart
        var cartData = JsonSerializer.Serialize(cart);

        Response.Cookies.Append(GetCartId(), cartData, new CookieOptions
        {
            HttpOnly = true,
            Secure = false,
            SameSite = SameSiteMode.Lax,
            Expires = DateTimeOffset.UtcNow.AddDays(30)
        });

        return Ok(new { message = "Đã thêm sản phẩm vào giỏ hàng!" });
    }

    [HttpDelete("remove-in-cart")]
    public IActionResult RemoveInCart(int productId)
    {
        var cart = GetCart();

        // check exist product
        var existingItem = cart?.FirstOrDefault(c => c.ProductId == productId);

        if (existingItem is not null)
        {
            cart?.Remove(existingItem);
        }

        var cartData = JsonSerializer.Serialize(cart);

        Response.Cookies.Append(GetCartId(), cartData, new CookieOptions
        {
            HttpOnly = true,
            Secure = false,
            SameSite = SameSiteMode.Lax,
            Expires = DateTimeOffset.UtcNow.AddDays(30)
        });

        return Ok(new { message = "Đã xoá sản phẩm vào giỏ hàng!" });
    }

    [HttpGet("checkout")]
    public async Task<IActionResult> Checkout()
    {
        var cart = GetCart();

        var cartItems = cart.ToDictionary(item => item.ProductId, item => item.Quantity);

        var cartItemData = await _cartService.GetListAsync(cartItems);

        var user = await _userManager.GetUserAsync(User);

        ViewBag.CartItems = cartItemData;
        ViewBag.UserProfile = user;

        return View();
    }

    [HttpPost("checkout-cart")]
    public async Task<IActionResult> CheckoutCart()
    {
        var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

        var cartId = $"Cart_{userId}";

        var cartCookie = Request.Cookies[cartId];

        if (userId == null)
        {
            return BadRequest("Not found user");
        }

        if (string.IsNullOrEmpty(cartCookie))
        {
            return BadRequest("Wrong Cookie");
        }

        var cart = JsonSerializer.Deserialize<List<CartItem>>(cartCookie);

        if (cart?.Count > 0)
        {
            var resp = await _cartService.CheckoutCartAsync(Int32.Parse(userId), cart.ToDictionary(item => item.ProductId, item => item.Quantity));

            if (resp)
            {
                Response.Cookies.Append(cartId, JsonSerializer.Serialize(new List<CartItem>()), new CookieOptions
                {
                    HttpOnly = true,
                    Secure = false,
                    SameSite = SameSiteMode.Lax,
                    Expires = DateTimeOffset.UtcNow.AddDays(30)
                });
            }
        }
        else
        {
            return BadRequest("Cart empty");
        }

        return BadRequest();
    }


    [HttpGet("success-checkout")]
    public IActionResult SuccessCheckout()
    {
        return View();
    }


    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View("Error!");
    }

    private List<CartItem> GetCart()
    {
        var cartId = GetCartId();

        var cartCookie = Request.Cookies[cartId];

        var cart = new List<CartItem>();

        if (!string.IsNullOrEmpty(cartCookie))
        {
            cart = JsonSerializer.Deserialize<List<CartItem>>(cartCookie);
        }
        else
        {
            var cartData = JsonSerializer.Serialize(cart);

            Response.Cookies.Append(cartId, cartData, new CookieOptions
            {
                HttpOnly = true,
                Secure = false,
                SameSite = SameSiteMode.Lax,
                Expires = DateTimeOffset.UtcNow.AddDays(30)
            });
        }

        return cart ?? new List<CartItem>();
    }

    private string GetCartId()
    {
        var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

        return $"Cart_{userId}";
    }
}