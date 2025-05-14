using Backend.Models;
using System.Collections.Generic;

namespace Backend.Repositories
{
    public interface ILessionRepository
    {
        bool ExistsByName(string name);
        List<Lession> FindAll(int page, int limit);
        int Count();
        Lession Save(Lession lession);
        Lession FindById(long id);
        void Delete(Lession lession);
    }
}