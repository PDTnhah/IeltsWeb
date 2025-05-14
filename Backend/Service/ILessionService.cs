using System.Collections.Generic;
using Backend.Models;
using Backend.Dtos;
using Backend.Responses;
using Backend.Exceptions;

namespace Backend.Service
{
    public interface ILessionService
    {
        Lession CreateLession(LessionDtos lessionDtos);
        Lession GetLessionById(long id);
        List<LessionResponse> GetAllLessions(int page, int limit);
        int GetLessionCount();
        Lession UpdateLession(long id, LessionDtos lessionDtos);
        void DeleteLession(long id);
        bool ExistsByName(string name);
        LessionImage CreateLessionImage(long id, LessionImageDto lessionImageDto);
    }
}