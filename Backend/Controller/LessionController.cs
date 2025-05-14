using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Http;
using System.IO;
using System;
using System.Linq;
using System.Threading.Tasks;
using System.Collections.Generic;
using Backend.Models;
using Backend.Dtos;
using Backend.Responses;
using Backend.Service;
using Backend.Exceptions;

namespace Backend.Controller
{
    [ApiController]
    [Route("api/v1/lessions")]
    public class LessionController : ControllerBase
    {
        private readonly ILessionService _lessionService;

        public LessionController(ILessionService lessionService)
        {
            _lessionService = lessionService;
        }

        [HttpGet]
        public IActionResult GetLessions([FromQuery] int page, [FromQuery] int limit)
        {
            var lessions = _lessionService.GetAllLessions(page, limit);
            var totalRecords = _lessionService.GetLessionCount();
            int totalPages = (int)Math.Ceiling((double)totalRecords / limit);

            return Ok(new LessionListResponse
            {
                lessions = lessions,
                totalPages = totalPages
            });
        }

         [HttpGet("{id}")]
        public IActionResult GetLessionById(long id)
        {
            try
            {
                var lession = _lessionService.GetLessionById(id);
                if (lession == null)
                {
                    return NotFound(new { message = "Lesson not found" });
                }
                return Ok(lession);
            }
            catch (DataNotFoundException ex)
            {
                return NotFound(ex.Message);
            }
            catch (Exception ex)
            {
                // Log ex
                return StatusCode(500, "Error retrieving lesson details.");
            }
        }

        [HttpPost]
        public IActionResult CreateLession([FromBody] LessionDtos lessionDtos)
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
                var newLession = _lessionService.CreateLession(lessionDtos);
                return Ok(newLession);
            }
            catch (DataNotFoundException ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPost("uploads/{id}")]
        public async Task<IActionResult> UploadImages(long id, [FromForm(Name = "files")] List<IFormFile> files)
        {
            try
            {
                var existingLession = _lessionService.GetLessionById(id);

                if (files.Count > LessionImage.maximumImagesPerLession)
                {
                    return BadRequest("You can only upload maximum 5 images");
                }

                var lessionImages = new List<LessionImage>();

                foreach (var file in files)
                {
                    if (file.Length == 0)
                    {
                        continue;
                    }

                    if (file.Length > 10 * 1024 * 1024)
                    {
                        return StatusCode(StatusCodes.Status413PayloadTooLarge, "File is too large! Maximum size is 10MB");
                    }

                    if (!file.ContentType.StartsWith("image/"))
                    {
                        return StatusCode(StatusCodes.Status415UnsupportedMediaType, "File must be an image");
                    }

                    string filename = await StoreFile(file);

                    var lessionImage = _lessionService.CreateLessionImage(id, new LessionImageDto
                    {
                        imageUrl = filename
                    });

                    lessionImages.Add(lessionImage);
                }

                return Ok(lessionImages);
            }
            catch (DataNotFoundException ex)
            {
                return BadRequest(ex.Message);
            }
            catch (InvalidParamException ex)
            {
                return BadRequest(ex.Message);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        private async Task<string> StoreFile(IFormFile file)
        {
            if (!IsImageFile(file))
            {
                throw new IOException("Invalid image format");
            }

            var filename = Path.GetFileName(file.FileName);
            var uniqueFileName = $"{Guid.NewGuid()}_{filename}";
            var uploadDir = Path.Combine(Directory.GetCurrentDirectory(), "Uploads");

            if (!Directory.Exists(uploadDir))
            {
                Directory.CreateDirectory(uploadDir);
            }

            var destination = Path.Combine(uploadDir, uniqueFileName);
            using (var stream = new FileStream(destination, FileMode.Create))
            {
                await file.CopyToAsync(stream);
            }

            return uniqueFileName;
        }

        private bool IsImageFile(IFormFile file)
        {
            return file.ContentType.StartsWith("image/");
        }

        [HttpPut("{id}")]
        public IActionResult UpdateLession(long id, [FromBody] LessionDtos lessionDtos)
        {
            try
            {
                var updatedLession = _lessionService.UpdateLession(id, lessionDtos);
                return Ok(updatedLession);
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

        [HttpDelete("{id}")]
        public IActionResult DeleteLession(long id)
        {
            try
            {
                _lessionService.DeleteLession(id);
                return Ok("Lession deleted successfully");
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
    }
}