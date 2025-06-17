using JuniorSteps.Models;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;

namespace JuniorSteps.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
        {
        }

        public DbSet<Post> Posts { get; set; }
        public DbSet<Category> Categories { get; set; }


        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<Category>().HasData(
                new Category { Id = 1, Name = ".NET" },
                new Category { Id = 2, Name = "C#" }
            );
        }

    }


}
