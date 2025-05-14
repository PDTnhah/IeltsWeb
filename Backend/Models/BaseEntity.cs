using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace Backend.Models
{
    public abstract class BaseEntity
    {
        [Column("created_at")]
        public DateTime createdAt { get; set; }

        [Column("updated_at")]
        public DateTime updatedAt { get; set; }

        public BaseEntity()
        {
            createdAt = DateTime.Now;
            updatedAt = DateTime.Now;
        }

        public void OnUpdate()
        {
            updatedAt = DateTime.Now;
        }
    }
}