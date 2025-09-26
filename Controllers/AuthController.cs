using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using StilSepetiApp.Data;
using StilSepetiApp.DTO;
using StilSepetiApp.Models;
using StilSepetiApp.Services;
using Microsoft.AspNetCore.Identity;

namespace StilSepetiApp.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly AppDbContext _context;
        private readonly TokenService _tokenService;
        private readonly ILogger<AuthController> _logger;

        public AuthController(AppDbContext context, TokenService tokenService, ILogger<AuthController> logger)
        {
            _context = context;
            _tokenService = tokenService;
            _logger = logger;
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] Logindto loginDto)
        {
            var user = await _context.Users.FirstOrDefaultAsync(u => u.Email == loginDto.Email);
            if (user == null)
            {
                _logger.LogWarning("Giriş başarısız: Email={Email}", loginDto.Email);
                return Unauthorized("Geçersiz e-posta veya şifre.");
            }

            var passwordHasher = new PasswordHasher<User>();
            var verificationResult = passwordHasher.VerifyHashedPassword(user, user.PasswordHash, loginDto.Password);

            if (verificationResult == PasswordVerificationResult.Failed)
            {
                _logger.LogWarning("Giriş başarısız: Email={Email}", loginDto.Email);
                return Unauthorized("Geçersiz e-posta veya şifre.");
            }


            var token = _tokenService.CreateToken(user);
            _logger.LogInformation("Giriş başarılı: KullanıcıId={UserId}, Email={Email}", user.userId, user.Email);

            return Ok(new { token });
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] Registerdto registerDto)
        {
            var existingUser = await _context.Users.AnyAsync(u => u.Email == registerDto.Email);
            if (existingUser)
            {
                _logger.LogWarning("Kayıt başarısız: Email zaten kullanılıyor. Email={Email}", registerDto.Email);
                return BadRequest("Bu e-posta zaten kayıtlı.");
            }
            var passwordHasher = new PasswordHasher<User>();

            var newUser = new User
            {
                Username = registerDto.Username,
                Email = registerDto.Email,
                PasswordHash = registerDto.Password,
                Role = registerDto.Role
            };
           

            _context.Users.Add(newUser);
            await _context.SaveChangesAsync();

            _logger.LogInformation("Yeni kullanıcı kaydı oluşturuldu: KullanıcıId={UserId}, Email={Email}", newUser.userId, newUser.Email);

            var token = _tokenService.CreateToken(newUser);
            return Ok(new
            {
                token,
                user = new
                {
                    newUser.userId,
                    newUser.Username,
                    newUser.Email,
                    newUser.Role
                }
            });


        }
    }
}