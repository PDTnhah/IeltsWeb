using Backend.Dtos;
using Backend.Models;

namespace Backend.Service
{
    public interface IUserService
    {
        User CreateUser(UserDtos userDtos);
        string Login(string phoneNumber, string password);
    }
}