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
        public DbSet<Essay> Essays { get; set; }
        public DbSet<Question> Questions { get; set; }
        public DbSet<Choice> Choices { get; set; }
        public DbSet<LessionAttempt> LessionAttempts { get; set; }
        public DbSet<AnswerAttempt> AnswerAttempts { get; set; }

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

            modelBuilder.Entity<Lession>()
                .HasMany(l => l.Questions)
                .WithOne(q => q.Lession)
                .HasForeignKey(q => q.lession_id)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<Question>()
                .HasMany(q => q.Choices)
                .WithOne(c => c.Question)
                .HasForeignKey(c => c.question_id)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<Lession>()
                .HasOne(l => l.skill)
                .WithMany(s => s.Lessions)
                .HasForeignKey(l => l.skillId);

             modelBuilder.Entity<LessionAttempt>()
                .HasMany(la => la.AnswerAttempts)
                .WithOne(aa => aa.LessionAttempt)
                .HasForeignKey(aa => aa.lession_attempt_id)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<AnswerAttempt>()
                .HasOne(aa => aa.Question)
                .WithMany()
                .HasForeignKey(aa => aa.question_id)
                .OnDelete(DeleteBehavior.Restrict); // Hoặc Cascade

            modelBuilder.Entity<AnswerAttempt>()
                .HasOne(aa => aa.SelectedChoice)
                .WithMany()
                .HasForeignKey(aa => aa.selected_choice_id)
                .OnDelete(DeleteBehavior.SetNull);

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

            modelBuilder.Entity<LessionAttempt>()
                .HasMany(la => la.AnswerAttempts)
                .WithOne(aa => aa.LessionAttempt)
                .HasForeignKey(aa => aa.lession_attempt_id)
                .OnDelete(DeleteBehavior.Cascade); // Xóa câu trả lời nếu lần làm bài bị xóa

            modelBuilder.Entity<AnswerAttempt>()
                .HasOne(aa => aa.Question)
                .WithMany() // Một Question có thể có nhiều AnswerAttempt từ các LessionAttempt khác nhau
                .HasForeignKey(aa => aa.question_id)
                .OnDelete(DeleteBehavior.Restrict); // Hoặc Cascade nếu muốn xóa answer khi question bị xóa (cân nhắc)

            modelBuilder.Entity<AnswerAttempt>()
                .HasOne(aa => aa.SelectedChoice)
                .WithMany() // Một Choice có thể được chọn trong nhiều AnswerAttempt
                .HasForeignKey(aa => aa.selected_choice_id)
                .OnDelete(DeleteBehavior.SetNull); // Nếu Choice bị xóa, vẫn giữ lại AnswerAttempt nhưng selected_choice_id là null

        }
    }
}