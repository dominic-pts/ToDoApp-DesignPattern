using Microsoft.EntityFrameworkCore;
using todo_app.Common;
using todo_app.Entities;

namespace todo_app.Data
{
    public class TodoDbContext:DbContext
    {
        public TodoDbContext(DbContextOptions<TodoDbContext> options) : base(options) { }
        public DbSet<Todo> Todos { get; set; }
        public override int SaveChanges()
        {
            var entries = ChangeTracker
                .Entries()
                .Where(e => e.Entity is BaseEntity && (
                        e.State == EntityState.Added
                        || e.State == EntityState.Modified));

            foreach (var entityEntry in entries)
            {
                ((IDatedModification)entityEntry.Entity).UpdateAt = DateTime.Now;

                if (entityEntry.State == EntityState.Added)
                {
                    ((IDatedModification)entityEntry.Entity).CreateAt = DateTime.Now;
                }
            }

            return base.SaveChanges();
        }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Todo>().HasKey(x => x.Id);
            modelBuilder.Entity<Todo>().Property(x=>x.Status).HasConversion<int>();
            base.OnModelCreating(modelBuilder);
        }
    }
}
