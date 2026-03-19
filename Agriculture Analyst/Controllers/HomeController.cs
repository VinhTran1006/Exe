using Agriculture_Analyst.Models;
using Agriculture_Analyst.Models.DTOs;
using Agriculture_Analyst.Services.Interfaces;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Hosting; // Thêm thư viện này
using Microsoft.AspNetCore.Http; // Thêm thư viện này
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Data;
using System.Diagnostics;
using System.IO; // Thêm thư viện này
using System.Security.Claims;
using System.Threading.Tasks;
using CloudinaryDotNet;
using CloudinaryDotNet.Actions;

namespace Agriculture_Analyst.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly IAuthService _authService;
        private readonly AgricultureAnalystDbContext _context;
        private readonly IWebHostEnvironment _env; // Khai báo thêm biến này

        // Cập nhật Constructor để Inject IWebHostEnvironment
        public HomeController(ILogger<HomeController> logger, IAuthService authService, AgricultureAnalystDbContext context, IWebHostEnvironment env)
        {
            _logger = logger;
            _authService = authService;
            _context = context;
            _env = env;
        }

        public IActionResult Index()
        {
            return View();
        }

        // ========================== DIỄN ĐÀN (GET) ==========================
        public IActionResult HomePage()
        {
            var userIdString = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(userIdString)) return RedirectToAction("Index"); // Yêu cầu đăng nhập

            int userId = int.Parse(userIdString);

            // 1. Lấy các vụ mùa ĐÃ KẾT THÚC của User để hiện trong Dropdown "Công bố báo cáo"
            ViewBag.HarvestedPlants = _context.Plants
                .Where(p => p.UserId == userId && (p.Status.ToLower().Contains("Đã thu hoạch") || p.Status.ToLower().Contains("xong")))
                .ToList();

            // 2. Lấy toàn bộ Bài viết trên Diễn đàn (Của tất cả mọi người)
            var posts = _context.Posts
                .Include(p => p.User)
                .Include(p => p.Plant) // Include Plant để lấy thông tin báo cáo đính kèm
                .OrderByDescending(p => p.CreatedAt)
                .ToList();

            return View(posts);
        }

        // ========================== ĐĂNG BÀI VIẾT (POST) ==========================
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CreatePost(string content, IFormFile? imageFile, int? plantId)
        {
            var userIdString = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(userIdString)) return RedirectToAction("Index");

            var post = new Post
            {
                UserId = int.Parse(userIdString),
                Content = content ?? "",
                CreatedAt = DateTime.Now,
                PlantId = plantId
            };

            // Xử lý lưu File Ảnh (Nếu người dùng có upload)
            if (imageFile != null && imageFile.Length > 0)
            {
                // Cấu hình tài khoản Cloudinary của bạn
                Account account = new Account("dyop5mqdp", "818898689433435", "FUXXYjFM6mhjT3tGexPuoOSoEKc");
                Cloudinary cloudinary = new Cloudinary(account);

                using (var stream = imageFile.OpenReadStream())
                {
                    var uploadParams = new ImageUploadParams()
                    {
                        File = new FileDescription(imageFile.FileName, stream),
                        Folder = "Agriculture_Posts" // Tạo thư mục trên Cloudinary
                    };

                    // Đẩy ảnh lên Cloudinary
                    var uploadResult = await cloudinary.UploadAsync(uploadParams);

                    // Lấy link URL an toàn (HTTPS) lưu vào Database
                    post.ImageUrl = uploadResult.SecureUrl.ToString();
                }
            }

            _context.Posts.Add(post);
            await _context.SaveChangesAsync();

            return RedirectToAction("HomePage");
        }


        // ========================== AUTHENTICATION ==========================
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

            var roles = await (
                from ur in _context.UserRoles
                join r in _context.Roles on ur.RoleId equals r.RoleId
                where ur.UserId == result.User.UserId
                select r.Name
            ).ToListAsync();

            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, result.User.UserId.ToString()),
                new Claim(ClaimTypes.Name, result.User.Username),
                new Claim(ClaimTypes.Email, result.User.Email)
            };

            foreach (var role in roles)
            {
                claims.Add(new Claim(ClaimTypes.Role, role));
            }

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
                    IsPersistent = true,
                    ExpiresUtc = DateTimeOffset.UtcNow.AddDays(7)
                }
            );

            return RedirectToAction("HomePage", "Home");
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Logout()
        {
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            return RedirectToAction("SignIn", "Home"); // Bạn có thể cần sửa lại là "Index" thay vì "SignIn" nếu trang gốc là Index
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