using Microsoft.EntityFrameworkCore;
using Backend.Models;

namespace Backend.Data
{
    public class AppDbContext : DbContext
    {
        public DbSet<Lession> Lessions { get; set; }
        public DbSet<LessionImage> LessionImages { get; set; }
        public DbSet<Skill> Skills { get; set; }
        public DbSet<Role> Roles { get; set; }
        public DbSet<SkillResult> SkillResults { get; set; }
        public DbSet<SocialAccount> SocialAccounts { get; set; }
        public DbSet<Token> Tokens { get; set; }
        public DbSet<User> Users { get; set; }

        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<Lession>()
                .HasOne(l => l.skill)
                .WithMany()
                .HasForeignKey(l => l.skillId);

            modelBuilder.Entity<LessionImage>()
                .HasOne(li => li.lession)
                .WithMany()
                .HasForeignKey(li => li.lessionId);

            modelBuilder.Entity<SkillResult>()
                .HasOne(sr => sr.skill)
                .WithMany()
                .HasForeignKey(sr => sr.skillId);

            modelBuilder.Entity<SkillResult>()
                .HasOne(sr => sr.user)
                .WithMany()
                .HasForeignKey(sr => sr.userId);

            modelBuilder.Entity<User>()
                .HasOne(u => u.role)
                .WithMany()
                .HasForeignKey(u => u.roleId);

            modelBuilder.Entity<Token>()
                .HasOne(t => t.user)
                .WithMany()
                .HasForeignKey(t => t.userId);
        }
    }
}