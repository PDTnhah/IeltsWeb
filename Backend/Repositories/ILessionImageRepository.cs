using Backend.Models;
using System.Collections.Generic;

namespace Backend.Repositories
{
    public interface ILessionImageRepository
    {
        List<LessionImage> FindByLessionId(long id);
        LessionImage Save(LessionImage lessionImage);
    }
}