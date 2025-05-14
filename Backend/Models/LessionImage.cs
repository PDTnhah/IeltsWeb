using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Backend.Models
{
    [Table("lession_images")]
    public class LessionImage
    {
        public const int maximumImagesPerLession = 5;

        [Key]
        public long id { get; set; }

        [ForeignKey("Lession")]
        [Column("lession_id")]
        public long lessionId { get; set; }

        public Lession lession { get; set; }

        [StringLength(300)]
        [Column("image_url")]
        public string imageUrl { get; set; }
    }
}