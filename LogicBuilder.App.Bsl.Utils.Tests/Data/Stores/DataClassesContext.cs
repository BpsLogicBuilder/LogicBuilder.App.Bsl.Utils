using LogicBuilder.Data;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using System.Reflection;

namespace LogicBuilder.App.Bsl.Utils.Tests.Data.Stores
{
    public class DataClassesContext(DbContextOptions<DataClassesContext> options) : DbContext(options)
    {
        public DbSet<Product> Products { get; set; }
        public DbSet<Category> Categories { get; set; }
        public DbSet<Address> Addresses { get; set; }
        public DbSet<DataTypes> DataTypes { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            foreach (Type modelType in this.GetType().GetProperties()
                .Where(p => p.PropertyType.Name == "DbSet`1")
                .Select(p => p.PropertyType.GetGenericArguments()[0])
                .Where(t =>typeof(BaseData).IsAssignableFrom(t)))
            {
                modelBuilder.Entity(modelType).Ignore(nameof(BaseData.EntityState));
            }

            base.OnModelCreating(modelBuilder);
        }
    }
}
