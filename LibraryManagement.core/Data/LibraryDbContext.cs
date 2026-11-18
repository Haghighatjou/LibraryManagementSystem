
using Microsoft.EntityFrameworkCore;
using LibraryManagement.Core.Models;
using LibraryManagement.core.Models;


namespace LibraryManagement.Core.Data
{
    public class LibraryDbContext:DbContext
    {
        public LibraryDbContext(DbContextOptions<LibraryDbContext> options) : base(options)
        {
        }

        public DbSet<Book> Books { get; set; }
        public DbSet<Loan> Loans { get; set; } 
        public DbSet<Member> Members { get; set; }
        public DbSet<LoanReport> LoanReport { get; set; }
        public DbSet<AppUser> Users { get; set; }
        

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<LoanReport>(entity =>
            {
                entity.HasNoKey();
                entity.ToView("LoanReport", "dbo");
            });

            modelBuilder.Entity<AppUser>().HasIndex(u => u.Username).IsUnique();
        }
    }
}
