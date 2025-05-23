using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Security.Claims;
using System.Text.Json;
using System.Threading.Tasks;
using fashion_shop.Core.Common;
using fashion_shop.Core.DTOs.Responses.User;
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
        public int ProductItemId { get; set; }
        public int Quantity { get; set; }
    }

    private readonly ILogger<CartController> _logger;
    private readonly SignInManager<User> _signInManager;
    private readonly UserManager<User> _userManager;
    private readonly ICartService _cartService;
    private readonly IProductService _productService;
    private readonly ISettingService _settingService;

    public CartController(
        ILogger<CartController> logger,
        SignInManager<User> signInManager,
        ICartService cartService,
        IProductService productService,
        UserManager<User> userManager,
        ISettingService settingService)
    {
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        _signInManager = signInManager ?? throw new ArgumentNullException(nameof(signInManager));
        _cartService = cartService ?? throw new ArgumentNullException(nameof(cartService));
        _productService = productService ?? throw new ArgumentNullException(nameof(productService));
        _userManager = userManager ?? throw new ArgumentNullException(nameof(userManager));
        _settingService = settingService ?? throw new ArgumentNullException(nameof(settingService));
    }

    [HttpGet("index")]
    public async Task<IActionResult> Index()
    {
        var cart = GetCart();

        var cartItems = cart.ToDictionary(item => item.ProductItemId, item => item.Quantity);

        var cartItemData = await _cartService.GetListAsync(cartItems);

        ViewBag.CartItems = cartItemData;

        return View();
    }

    [HttpGet("add-to-cart")]
    public async Task<IActionResult> AddToCart(int productItemId)
    {
        var cart = GetCart();

        // check exist product
        var existingItem = cart?.FirstOrDefault(c => c.ProductItemId == productItemId);

        if (await _cartService.CheckProductExistByProductItemIdAsync(productItemId) is false)
        {
            return NotFound();
        }

        if (existingItem == null)
        {
            cart?.Add(new CartItem { ProductItemId = productItemId, Quantity = 1 });
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
    public IActionResult RemoveInCart(int productItemId)
    {
        var cart = GetCart();

        // check exist product
        var existingItem = cart?.FirstOrDefault(c => c.ProductItemId == productItemId);

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

        var cartItems = cart.ToDictionary(item => item.ProductItemId, item => item.Quantity);

        var cartItemData = await _cartService.GetListAsync(cartItems);

        var user = await _userManager.GetUserAsync(User);

        ViewBag.CartItems = cartItemData;
        ViewBag.UserProfile = user;
        ViewBag.BasicInfo = await _settingService.GetSettingAsync<BasicInfoSettingResponse>(SettingPrefixConstants.BasicInfoPrefix);

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
            var resp = await _cartService.CheckoutCartAsync(Int32.Parse(userId), cart.ToDictionary(item => item.ProductItemId, item => item.Quantity));

            if (resp)
            {
                Response.Cookies.Append(cartId, JsonSerializer.Serialize(new List<CartItem>()), new CookieOptions
                {
                    HttpOnly = true,
                    Secure = false,
                    SameSite = SameSiteMode.Lax,
                    Expires = DateTimeOffset.UtcNow.AddDays(30)
                });

                return Ok();
            }
        }

        return BadRequest("Cart empty");
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