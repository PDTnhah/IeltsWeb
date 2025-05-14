using System;
using Backend.Models;
using Backend.Dtos;
using Backend.Repositories;
using Backend.Exceptions;

namespace Backend.Service
{
    public class UserService : IUserService
    {
        private readonly IUserRepository _userRepository;
        private readonly IRoleRepository _roleRepository;

        public UserService(IUserRepository userRepository, IRoleRepository roleRepository)
        {
            _userRepository = userRepository;
            _roleRepository = roleRepository;
        }

        public User CreateUser(UserDtos userDtos)
        {
            if (_userRepository.ExistsByPhoneNumber(userDtos.phoneNumber))
            {
                throw new Exception("Phone number already exists");
            }

            var role = _roleRepository.FindById(userDtos.roleId)
                ?? throw new DataNotFoundException("Role not found");

            var newUser = new User
            {
                fullname = userDtos.fullName,
                phoneNumber = userDtos.phoneNumber,
                address = userDtos.address,
                password = userDtos.password, // Nên mã hóa mật khẩu
                dateOfBirth = userDtos.dateOfBirth,
                facebookAccountId = userDtos.facebookAccountId,
                googleAccountId = userDtos.googleAccountId,
                role = role
            };

            return _userRepository.Save(newUser);
        }

        public string Login(string phoneNumber, string password)
        {
            // Cần triển khai logic xác thực và tạo token (JWT)
            throw new NotImplementedException("Login not implemented");
        }
    }
}