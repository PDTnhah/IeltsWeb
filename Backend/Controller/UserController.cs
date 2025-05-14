using Microsoft.AspNetCore.Mvc;
using System.Linq;
using System.Collections.Generic;
using System;
using Backend.Dtos;
using Backend.Service;
using Backend.Models;
using Backend.Exceptions;

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

        [HttpPost("register")]
        public IActionResult CreateUser([FromBody] UserDtos userDtos)
        {
            if (!ModelState.IsValid)
            {
                var errors = ModelState.Values
                    .SelectMany(v => v.Errors)
                    .Select(e => e.ErrorMessage)
                    .ToList();
                return BadRequest(errors);
            }

            if (userDtos.password != userDtos.reTypePassword)
            {
                return BadRequest("Passwords do not match");
            }

            try
            {
                var newUser = _userService.CreateUser(userDtos);
                return Ok("Register successfully");
            }
            catch (DataNotFoundException ex)
            {
                return BadRequest(ex.Message);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPost("login")]
        public IActionResult Login([FromBody] UserLoginDto userLoginDto)
        {
            if (!ModelState.IsValid)
            {
                var errors = ModelState.Values
                    .SelectMany(v => v.Errors)
                    .Select(e => e.ErrorMessage)
                    .ToList();
                return BadRequest(errors);
            }

            try
            {
                var token = _userService.Login(userLoginDto.phoneNumber, userLoginDto.password);
                return Ok(token);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
    }
}