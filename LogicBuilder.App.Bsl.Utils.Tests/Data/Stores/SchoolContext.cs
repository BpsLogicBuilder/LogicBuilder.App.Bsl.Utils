using LogicBuilder.Data;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using System.Reflection;

namespace LogicBuilder.App.Bsl.Utils.Tests.Data.Stores
{
    public class SchoolContext(DbContextOptions<SchoolContext> options) : BaseDbContext(options)
    {
        public DbSet<Course> Courses { get; set; }
        public DbSet<Enrollment> Enrollments { get; set; }
        public DbSet<Student> Students { get; set; }
        public DbSet<Department> Departments { get; set; }
        public DbSet<Instructor> Instructors { get; set; }
        public DbSet<OfficeAssignment> OfficeAssignments { get; set; }
        public DbSet<CourseAssignment> CourseAssignments { get; set; }
        public DbSet<LookUps> LookUps { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            foreach (Type modelType in this.GetType().GetProperties()
                .Where(p => p.PropertyType.Name == "DbSet`1")
                .Select(p => p.PropertyType.GetGenericArguments()[0])
                .Where(t => typeof(BaseData).IsAssignableFrom(t)))
            {
                modelBuilder.Entity(modelType).Ignore(nameof(BaseData.EntityState));
            }

            base.OnModelCreating(modelBuilder);
        }
    }
}
