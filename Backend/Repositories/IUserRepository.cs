using Backend.Models;

namespace Backend.Repositories
{
    public interface IUserRepository
    {
        bool ExistsByPhoneNumber(string phoneNumber);
        User FindByPhoneNumber(string phoneNumber);
        User Save(User user);
    }
}