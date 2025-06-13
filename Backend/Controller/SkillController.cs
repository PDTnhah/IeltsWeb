using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Linq;
using System;
using Backend.Models;
using Backend.Dtos;
using Backend.Service;
using Backend.Exceptions;
using Microsoft.AspNetCore.Authorization;

namespace Backend.Controller
{
    [ApiController]
    [Route("api/v1/skills")] // Endpoint public hoặc cho user đã login
    public class SkillController : ControllerBase
    {
        private readonly ISkillService _skillService;
        public SkillController(ISkillService skillService) { _skillService = skillService; }

        [HttpGet]
        // [AllowAnonymous] // Nếu ai cũng xem được
        // [Authorize(Roles="User,Admin")] // Hoặc chỉ User nếu Admin dùng endpoint riêng
        public async Task<IActionResult> GetAllSkillsForUser()
        {
            var skills = await _skillService.GetAllSkillsAsync();
            return Ok(skills);
        }
    }
}