using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Linq; // Cho ToList
using Backend.Models;
using Backend.Dtos;
using Backend.Repositories;
using Backend.Exceptions;
using Backend.Data;
using AutoMapper; // Vẫn cần AutoMapper để map từ UserDtos sang User model
using BCrypt.Net;
using Microsoft.EntityFrameworkCore; // Cho ToListAsync

namespace Backend.Service
{
    public class UserService : IUserService
    {
        private readonly IUserRepository _userRepository;
        private readonly IRoleRepository _roleRepository;
        private readonly IMapper _mapper; // Vẫn cần để map DTO -> Model
        private readonly AppDbContext _context;

        public UserService(IUserRepository userRepository, IRoleRepository roleRepository, IMapper mapper, AppDbContext context)
        {
            _userRepository = userRepository;
            _roleRepository = roleRepository;
            _mapper = mapper;
            _context = context;
        }

        public async Task<User> CreateUserAsync(UserDtos userDtos)
        {
            if (await _userRepository.ExistsByEmailAsync(userDtos.email))
            {
                throw new InvalidParamException($"Email '{userDtos.email}' already exists.");
            }
            if (!string.IsNullOrEmpty(userDtos.phoneNumber) && await _userRepository.ExistsByPhoneNumberAsync(userDtos.phoneNumber))
            {
                throw new InvalidParamException($"Phone number '{userDtos.phoneNumber}' already exists.");
            }

            // Kiểm tra mật khẩu và reTypePassword
            if (userDtos.password != userDtos.reTypePassword)
            {
                throw new InvalidParamException("Passwords do not match.");
            }
            if (string.IsNullOrEmpty(userDtos.password) || userDtos.password.Length < 6)
            {
                throw new InvalidParamException("Password must be at least 6 characters long.");
            }


            var role = await _roleRepository.FindByIdAsync(userDtos.roleId);
            if (role == null)
            {
                throw new DataNotFoundException($"Role with id {userDtos.roleId} not found.");
            }

            // Map từ UserDtos sang User model
            var newUser = new User
            {
                fullname = userDtos.fullName,
                phoneNumber = userDtos.phoneNumber,
                address = userDtos.address,
                email = userDtos.email,
                password = BCrypt.Net.BCrypt.HashPassword(userDtos.password), // Hash mật khẩu
                dateOfBirth = userDtos.dateOfBirth, // Giữ lại nếu UserDtos có
                facebookAccountId = userDtos.facebookAccountId, // Giữ lại
                googleAccountId = userDtos.googleAccountId,   // Giữ lại
                roleId = role.id,
                active = true, // Mặc định active
                createdAt = DateTime.UtcNow,
                updatedAt = DateTime.UtcNow
            };
            // EF Core sẽ không tự động gán `role` object, chỉ cần `roleId`

            return await _userRepository.SaveAsync(newUser);
        }

        public async Task<User?> GetUserByIdAsync(long id)
        {
            return await _userRepository.FindByIdAsync(id); // Repository nên Include(u => u.role)
        }

        public async Task<User?> GetUserByEmailForAuthAsync(string email)
        {
            return await _userRepository.FindByEmailAsync(email); // Repository nên Include(u => u.role)
        }

        public async Task<(IEnumerable<User> Users, int TotalRecords, int TotalPages)> GetAllUsersPaginatedAsync(int page, int limit, string? searchTerm = null, long? roleId = null)
        {
            var (users, totalRecords) = await _userRepository.FindAllPaginatedAsync(page, limit, searchTerm, roleId);
            var totalPages = (int)Math.Ceiling((double)totalRecords / limit);
            return (users, totalRecords, totalPages);
            // Controller sẽ quyết định cách map sang DTO response (nếu cần)
        }

        public async Task<User?> UpdateUserByAdminAsync(long userId, UserDtos userDtos)
        {
            var existingUser = await _userRepository.FindByIdAsync(userId);
            if (existingUser == null)
            {
                return null; // Hoặc throw DataNotFoundException
            }

            // Kiểm tra Role mới có tồn tại không
            if (existingUser.roleId != userDtos.roleId)
            {
                if (!await _roleRepository.ExistsAsync(userDtos.roleId))
                {
                    throw new DataNotFoundException($"Role with ID {userDtos.roleId} not found.");
                }
            }

            // Cập nhật các trường cho phép từ UserDtos
            existingUser.fullname = userDtos.fullName;
            existingUser.phoneNumber = userDtos.phoneNumber;
            existingUser.address = userDtos.address;
            existingUser.dateOfBirth = userDtos.dateOfBirth; // Cập nhật nếu có trong DTO
            existingUser.facebookAccountId = userDtos.facebookAccountId; // Cập nhật
            existingUser.googleAccountId = userDtos.googleAccountId;   // Cập nhật
            existingUser.roleId = userDtos.roleId;
            // existingUser.active = userDtos.active; // UserDtos hiện tại không có trường active
            // Nếu admin cần thay đổi trạng thái active, UserDtos cần có trường này
            // Hoặc có API riêng để (de)activate user.
            existingUser.updatedAt = DateTime.UtcNow;

            // **KHÔNG CẬP NHẬT MẬT KHẨU Ở ĐÂY**
            // Nếu UserDtos có password và reTypePassword, và admin muốn đổi pass:
            // if (!string.IsNullOrEmpty(userDtos.password))
            // {
            //     if (userDtos.password != userDtos.reTypePassword)
            //     {
            //         throw new InvalidParamException("Passwords do not match for update.");
            //     }
            //     existingUser.password = BCrypt.Net.BCrypt.HashPassword(userDtos.password);
            // }

            await _userRepository.SaveAsync(existingUser);
            return await _userRepository.FindByIdAsync(userId); // Trả về user đã cập nhật (với Role)
        }

        public async Task<bool> DeleteUserByAdminAsync(long userId)
        {
            // Cần hàm Delete trong IUserRepository, ví dụ DeleteByIdAsync
            // return await _userRepository.DeleteByIdAsync(userId);
            var user = await _userRepository.FindByIdAsync(userId);
            if (user == null) return false;
            // Giả sử IUserRepository có hàm Delete(User user)
            // await _userRepository.DeleteAsync(user); // Cần tạo hàm này
            // Hoặc trực tiếp từ context nếu Repo không có:
            _context.Users.Remove(user); // _context là AppDbContext được inject
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> UserExistsAsync(long id)
        {
            return await _userRepository.ExistsAsync(id);
        }
        public async Task<bool> EmailExistsAsync(string email)
        {
            return await _userRepository.ExistsByEmailAsync(email);
        }
        public async Task<bool> PhoneNumberExistsAsync(string phoneNumber)
        {
            return await _userRepository.ExistsByPhoneNumberAsync(phoneNumber);
        }
        
        public async Task<User?> UpdateUserByEmailAsync(string email, UserDtos userDtos)
{
    var existingUser = await _userRepository.FindByEmailAsync(email);
    if (existingUser == null)
    {
        return null;
    }

    // Kiểm tra Role mới
    if (existingUser.roleId != userDtos.roleId)
    {
        if (!await _roleRepository.ExistsAsync(userDtos.roleId))
        {
            throw new DataNotFoundException($"Role with ID {userDtos.roleId} not found.");
        }
    }

    // Cập nhật
    existingUser.fullname = userDtos.fullName;
    existingUser.phoneNumber = userDtos.phoneNumber;
    existingUser.address = userDtos.address;
    existingUser.dateOfBirth = userDtos.dateOfBirth;

    await _userRepository.SaveAsync(existingUser);
    return await _userRepository.FindByEmailAsync(email);
}

    }
}