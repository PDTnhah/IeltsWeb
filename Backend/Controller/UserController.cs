using Microsoft.AspNetCore.Mvc;
using System;
using System.Threading.Tasks;
using Backend.Dtos;
using Backend.Models;
using Backend.Service;

namespace Backend.Controller
{
    [ApiController]
    [Route("api/v1/users")]
    public class UserController : ControllerBase
    {
        private readonly IUserService _userService;

        public UserController(IUserService userService)
        {
            _userService = userService;
        }

        /// <summary>
        /// Lấy thông tin user theo ID
        /// GET api/v1/users/{id}
        /// </summary>
        [HttpGet("{id:long}")]
        public async Task<IActionResult> GetUser(long id)
        {
            var user = await _userService.GetUserByIdAsync(id);
            if (user == null)
                return NotFound(new { message = $"User with id {id} not found." });

            var response = new UserResponseDto
            {
                Id = user.id,
                FullName = user.fullname,
                Email = user.email,
                PhoneNumber = user.phoneNumber,
                Address = user.address,
                DateOfBirth = user.dateOfBirth,
                FacebookAccountId = user.facebookAccountId,
                GoogleAccountId = user.googleAccountId,
                RoleId = user.roleId,
                IsActive = user.active,
                CreatedAt = user.createdAt,
                UpdatedAt = user.updatedAt
            };
            return Ok(response);
        }

        /// <summary>
        /// Cập nhật thông tin user
        /// PUT api/v1/users/{id}
        /// </summary>
        [HttpPut("{id:long}")]
        public async Task<IActionResult> UpdateUser(long id, [FromBody] UserDtos dto)
        {
            // Có thể thêm xác thực userId từ token so với id

            // Gọi service để update
            var updated = await _userService.UpdateUserByAdminAsync(id, dto);
            if (updated == null)
                return NotFound(new { message = $"User with id {id} not found." });

            var response = new UserResponseDto
            {
                Id = updated.id,
                FullName = updated.fullname,
                Email = updated.email,
                PhoneNumber = updated.phoneNumber,
                Address = updated.address,
                DateOfBirth = updated.dateOfBirth,
                FacebookAccountId = updated.facebookAccountId,
                GoogleAccountId = updated.googleAccountId,
                RoleId = updated.roleId,
                IsActive = updated.active,
                CreatedAt = updated.createdAt,
                UpdatedAt = updated.updatedAt
            };
            return Ok(response);
        }

        [HttpGet("email/{email}")]
        public async Task<IActionResult> GetUserByEmail(string email)
        {
            var user = await _userService.GetUserByEmailForAuthAsync(email);
            if (user == null)
                return NotFound(new { message = $"User with email '{email}' not found." });

            var response = new UserResponseDto
            {
                Id = user.id,
                FullName = user.fullname,
                Email = user.email,
                PhoneNumber = user.phoneNumber,
                Address = user.address,
                DateOfBirth = user.dateOfBirth,
                FacebookAccountId = user.facebookAccountId,
                GoogleAccountId = user.googleAccountId,
                RoleId = user.roleId,
                IsActive = user.active,
                CreatedAt = user.createdAt,
                UpdatedAt = user.updatedAt
            };

            return Ok(response);
        }

// [HttpPut("email/{email}")]
// public async Task<IActionResult> UpdateUser(string email, [FromBody] UserUpdateDto updatedUser)
// {
//     var user = await _userService.GetUserByEmailForAuthAsync(email);
//     if (user == null) return NotFound("User not found");

//     user.fullname = updatedUser.fullName;
//     user.phoneNumber = updatedUser.phoneNumber;
//     user.address = updatedUser.address;
//     user.dateOfBirth = updatedUser.dateOfBirth;

//     await user.SaveAsync(user);

//     return Ok("User updated successfully");
// }
    }

    
}
