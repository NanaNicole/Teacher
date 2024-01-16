using Colegio.Models;
using Microsoft.EntityFrameworkCore;

namespace Colegio
{
    public class ColegioContext : DbContext, IColegioContext
    {
        public DbSet<TeacherDto> Teachers { get; set; }

        public ColegioContext(DbContextOptions<ColegioContext> options) : base(options) { }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<TeacherDto>(teacher =>
            {
                teacher.ToTable("Teacher");
                teacher.HasKey(x => x.Id);
                teacher.Property(x => x.Name).IsRequired().HasMaxLength(50);
                teacher.Property(x => x.Email).IsRequired().HasMaxLength(100);
                teacher.Property(x => x.TypeIdentification).IsRequired();
                teacher.Property(x => x.Identification).IsRequired();
                teacher.Property(x => x.GradeId).IsRequired();
            });
        }

        void IColegioContext.SaveChanges()
        {
            base.SaveChanges();
        }
    }
}
