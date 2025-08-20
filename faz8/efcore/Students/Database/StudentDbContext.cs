using Microsoft.EntityFrameworkCore;

namespace Students.Database;

public class StudentDbContext : DbContext
{
    public DbSet<Student> Students { get; set; }
    public DbSet<StudentScore> StudentScore { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        optionsBuilder.UseNpgsql("Host=localhost;Port=5432;Database=efcore;Username=postgres;Password=admin");
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Student>().HasKey(s => s.StudentNumber);
        modelBuilder.Entity<Student>().ToTable("students");
        
        modelBuilder.Entity<StudentScore>().HasKey(ss => new { ss.StudentNumber, ss.Lesson });
        modelBuilder.Entity<StudentScore>().ToTable("studentscores");
    }
}
