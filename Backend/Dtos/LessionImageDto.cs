using System.ComponentModel.DataAnnotations;

namespace Backend.Dtos
{
    public class LessionImageDto
    {
        public long lessionId { get; set; }

        [StringLength(200, MinimumLength = 3, ErrorMessage = "Image Url must be between 3 and 200 characters")]
        public string imageUrl { get; set; }
    }
}