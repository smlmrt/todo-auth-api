using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using TodoAuthApi.Data;
using TodoAuthApi.Models;

namespace TodoAuthApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly AppDbContext _context;
        private readonly IConfiguration _configuration;

        public AuthController(AppDbContext context, IConfiguration configuration)
        {
            _context = context;
            _configuration = configuration;
        }

        // KULLANICI KAYIT OLMA (REGISTER)
        [HttpPost("register")]
        public IActionResult Register(UserDto request)
        {
            // Kullanıcı adı daha önce alınmış mı kontrol et
            if (_context.Users.Any(u => u.Username == request.Username))
            {
                return BadRequest("Bu kullanıcı adı zaten alınmış.");
            }

            // Şifreyi BCrypt ile geri döndürülemez şekilde şifrele (Hash)
            string passwordHash = BCrypt.Net.BCrypt.HashPassword(request.Password);

            var user = new User
            {
                Username = request.Username, 
                PasswordHash = passwordHash  
            };

            _context.Users.Add(user);
            _context.SaveChanges();

            return Ok("Kullanıcı başarıyla oluşturuldu.");
        }

        // KULLANICI GİRİŞ YAPMA (LOGIN)
        [HttpPost("login")]
        public IActionResult Login(UserDto request)
        {
            // Kullanıcıyı veritabanında bul (Buradaki u.Username büyük harfle olmalı)
            var user = _context.Users.FirstOrDefault(u => u.Username == request.Username);
            if (user == null)
            {
                return BadRequest("Kullanıcı bulunamadı.");
            }

            // Gönderilen şifre ile veritabanındaki şifrelenmiş (hash) şifreyi karşılaştır
            if (!BCrypt.Net.BCrypt.Verify(request.Password, user.PasswordHash))
            {
                return BadRequest("Hatalı şifre.");
            }

            // Şifre doğruysa JWT Token (Dijital Anahtar) Üret
            var token = CreateToken(user);
            return Ok(new { Token = token });
        }

        // JWT ÜRETİCİ METOT (Arka Planda Çalışır)
        private string CreateToken(User user)
        {
            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
                new Claim(ClaimTypes.Name, user.Username) 
            };

            // Buradaki _configuration tekil (sonunda 's' yok)
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["Jwt:Key"]!));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256Signature);

            var token = new JwtSecurityToken(
                issuer: _configuration["Jwt:Issuer"],
                audience: _configuration["Jwt:Audience"],
                claims: claims,
                expires: DateTime.Now.AddDays(1),
                signingCredentials: creds
            );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }
    }

    public class UserDto
    {
        public string Username { get; set; } = string.Empty;
        public string Password { get; set; } = string.Empty;
    }
}