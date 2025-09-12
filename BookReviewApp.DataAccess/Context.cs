using BookReviewApp.DataAccess.Mappings;
using BookReviewApp.Domain.Models;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace BookReviewApp.DataAccess
{
    public class Context(DbContextOptions<Context> options) : IdentityDbContext<AppUser>(options)
    {
        public DbSet<Book> Books { get; set; }
        public DbSet<Review> Reviews { get; set; }
        public DbSet<ReviewVote> ReviewVotes { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder
                .ApplyConfiguration(new BookMap())
                .ApplyConfiguration(new ReviewMap())
                .ApplyConfiguration(new ReviewVoteMap());

            base.OnModelCreating(modelBuilder);
        }
    }

}
