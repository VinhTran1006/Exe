using Agriculture_Analyst.Models;
using Agriculture_Analyst.Models.DTOs;
using Agriculture_Analyst.Services.Interfaces;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Data;
using System.Diagnostics;
using System.Security.Claims;


namespace Agriculture_Analyst.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly IAuthService _authService;
        private readonly AgricultureAnalystDbContext _context;

        public HomeController(ILogger<HomeController> logger, IAuthService authService, AgricultureAnalystDbContext context)
        {
            _logger = logger;
            _authService = authService;
            _context = context;
        }

        public IActionResult Index()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> SignUp([FromForm] SignUpRequestDto request)
        {
            if (!ModelState.IsValid)
            {
                return View("Index", new { signupError = string.Join(", ", ModelState.Values.SelectMany(v => v.Errors.Select(e => e.ErrorMessage))) });
            }

            var result = await _authService.SignUpAsync(request);

            if (!result.Success)
            {
                return View("Index", new { signupError = result.Message });
            }

            TempData["SuccessMessage"] = "Account created successfully! Please sign in.";
            return View("Index", new { showSignIn = true });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> SignIn([FromForm] SignInRequestDto request)
        {
            if (!ModelState.IsValid)
            {
                return View("Index", new
                {
                    signinError = string.Join(", ",
                        ModelState.Values.SelectMany(v => v.Errors.Select(e => e.ErrorMessage)))
                });
            }

            var result = await _authService.SignInAsync(request);

            if (!result.Success)
            {
                return View("Index", new { signinError = result.Message });
            }

            //  LẤY ROLE TỪ DB
            var roles = await (
                from ur in _context.UserRoles
                join r in _context.Roles on ur.RoleId equals r.RoleId
                where ur.UserId == result.User.UserId
                select r.Name
            ).ToListAsync();

            //  TẠO CLAIMS
            var claims = new List<Claim>
    {
        new Claim(ClaimTypes.NameIdentifier, result.User.UserId.ToString()),
        new Claim(ClaimTypes.Name, result.User.Username),
        new Claim(ClaimTypes.Email, result.User.Email)
    };

            //   ADD ROLE VÀO CLAIM
            foreach (var role in roles)
            {
                claims.Add(new Claim(ClaimTypes.Role, role));
            }

            // 👉 4. SIGN IN
            var identity = new ClaimsIdentity(
                claims,
                CookieAuthenticationDefaults.AuthenticationScheme
            );

            var principal = new ClaimsPrincipal(identity);

            await HttpContext.SignInAsync(
                CookieAuthenticationDefaults.AuthenticationScheme,
                principal,
                new AuthenticationProperties
                {
                    IsPersistent = true,              // ⭐ GIỮ COOKIE SAU KHI TẮT TRÌNH DUYỆT
                    ExpiresUtc = DateTimeOffset.UtcNow.AddDays(7)
                }
            ); 
            

            return RedirectToAction("Index", "Plant");
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Logout()
        {
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            return RedirectToAction("SignIn", "Home");
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
