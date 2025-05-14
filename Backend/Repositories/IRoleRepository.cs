using Backend.Models;

namespace Backend.Repositories
{
    public interface IRoleRepository
    {
        Role FindById(long id);
        Role Save(Role role);
    }
}