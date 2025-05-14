// using Microsoft.EntityFrameworkCore;

// namespace Backend.Data
// {
//     public class SkillsetDbContext : DbContext
//     {
//         public SkillsetDbContext(DbContextOptions<SkillsetDbContext> options) : base(options) { }

//         public DbSet<User> Users { get; set; }
//         public DbSet<Product> Products { get; set; }
//         public DbSet<Category> Categories { get; set; }
//     }

//     public class User
//     {
//         public int Id { get; set; }
//         public string FullName { get; set; }
//         public string PhoneNumber { get; set; }
//         public string Password { get; set; }
//     }

//     public class Product
//     {
//         public int Id { get; set; }
//         public string Name { get; set; }
//         public float Price { get; set; }
//         public string Thumbnail { get; set; }
//     }

//     public class Category
//     {
//         public int Id { get; set; }
//         public string Name { get; set; }
//     }
// }