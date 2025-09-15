using Microsoft.EntityFrameworkCore;

namespace ToDo.Models
{
    public class ToDoContext : DbContext
    {
        public ToDoContext(DbContextOptions<ToDoContext> options) : base(options) { }

        public DbSet<ToDo> ToDos { get; set; } = null!;
        public DbSet<Category> Categories { get; set; } = null!;
        public DbSet<Status> Statuses { get; set; } = null!;
        public DbSet<DeletedTask> DeletedTasks { get; set; } = null!; 

        // seed data
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Category>().HasData(
                new Category { CategoryId = 1, Name = "Work" },
                new Category { CategoryId = 2, Name = "Home" },
                new Category { CategoryId = 3, Name = "Exercise" },
                new Category { CategoryId = 4, Name = "Shopping" },
                new Category { CategoryId = 5, Name = "Contact" }
            );

            modelBuilder.Entity<Status>().HasData(
                new Status { StatusId = 1, Name = "Open" },
                new Status { StatusId = 2, Name = "Completed" }
            );
        }
    }
}
