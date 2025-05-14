using System;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.IdentityModel.Tokens;
using Microsoft.Extensions.Configuration;
using BCrypt.Net;
using Backend.Data;
using Backend.Models;
using Backend.Dtos;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

namespace Backend.Service
{
    public class AuthService
    {
        private readonly AppDbContext _context;
        private readonly IConfiguration _configuration;

        public AuthService(AppDbContext context, IConfiguration configuration)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
            _configuration = configuration ?? throw new ArgumentNullException(nameof(configuration));
        }

        public async Task<AuthResponse> LoginAsync(LoginDto dto)
        {
            if (dto == null) throw new ArgumentNullException(nameof(dto));

            var user = await _context.Users
                .FirstOrDefaultAsync(u => u.email == dto.email);

            if (user == null || !BCrypt.Net.BCrypt.Verify(dto.password, user.password))
            {
                throw new Exception("Invalid email or password");
            }

            if (!user.active)
            {
                throw new Exception("User account is inactive");
            }

            var token = GenerateJwtToken(user);
            return new AuthResponse
            {
                token = token,
                userId = user.id
            };
        }

        public async Task<AuthResponse> RegisterAsync(RegisterDto dto)
        {
            if (dto == null) throw new ArgumentNullException(nameof(dto));

            var existingUser = await _context.Users
                .FirstOrDefaultAsync(u => u.email == dto.email);

            if (existingUser != null)
            {
                throw new Exception("Email already registered");
            }

            if (dto.password != dto.confirmPassword)
            {
                throw new Exception("Passwords do not match");
            }

            var user = new User
            {
                email = dto.email,
                fullname = dto.name,
                password = BCrypt.Net.BCrypt.HashPassword(dto.password),
                active = true,
                roleId = 1
            };

            _context.Users.Add(user);
            await _context.SaveChangesAsync();

            var token = GenerateJwtToken(user);
            return new AuthResponse
            {
                token = token,
                userId = user.id
            };
        }

        private string GenerateJwtToken(User user)
        {
            if (user == null) throw new ArgumentNullException(nameof(user));

            var claims = new[]
            {
                new Claim(ClaimTypes.NameIdentifier, user.id.ToString()),
                new Claim(ClaimTypes.Email, user.email),
                new Claim(ClaimTypes.Role, user.roleId.ToString())
            };

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["Jwt:Key"] ?? throw new InvalidOperationException("Jwt:Key is missing")));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var token = new JwtSecurityToken(
                issuer: _configuration["Jwt:Issuer"],
                audience: _configuration["Jwt:Audience"],
                claims: claims,
                expires: DateTime.Now.AddDays(7),
                signingCredentials: creds);

            return new JwtSecurityTokenHandler().WriteToken(token);
        }
    }
}