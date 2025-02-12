using Microsoft.EntityFrameworkCore;
using SlqStudio.Persistence.Models;

namespace SlqStudio.Persistence;

public class ApplicationDbContext : DbContext
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options) { }

    public DbSet<Course> Courses { get; set; }
    public DbSet<LabWork> LabWorks { get; set; }
    public DbSet<LabTask> LabTasks { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<LabWork>()
            .HasOne(l => l.Course)
            .WithMany(c => c.LabWorks)
            .HasForeignKey(l => l.CourseId);

        modelBuilder.Entity<LabTask>()
            .HasOne(t => t.LabWork)
            .WithMany(l => l.Tasks)
            .HasForeignKey(t => t.LabWorkId);
    }
}