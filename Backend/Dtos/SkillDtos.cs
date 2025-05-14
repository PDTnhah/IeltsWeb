using System.ComponentModel.DataAnnotations;

namespace Backend.Dtos
{
    public class SkillDtos
    {
        [Required(ErrorMessage = "Skill name cannot be empty")]
        public string name { get; set; }
    }
}