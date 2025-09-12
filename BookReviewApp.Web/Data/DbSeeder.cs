using BookReviewApp.DataAccess;
using BookReviewApp.Domain.Models;
using Microsoft.AspNetCore.Identity;

namespace BookReviewApp.Web.Data;

public static class DbSeeder
{
    public static async Task SeedAsync(Context context, UserManager<AppUser> userManager)
    {
        // Seed users
        if (!userManager.Users.Any())
        {
            var alice = new AppUser { UserName = "alice", Email = "alice@test.com", EmailConfirmed = true };
            var bob = new AppUser { UserName = "bob", Email = "bob@test.com", EmailConfirmed = true };

            await userManager.CreateAsync(alice, "Password123!");
            await userManager.CreateAsync(bob, "Password123!");
        }

        // Seed books
        if (!context.Books.Any())
        {
            context.Books.AddRange(
                new Book { Title = "1984", Author = "George Orwell", PublishedYear = 1949, Genre = "Dystopian" },
                new Book { Title = "The Hobbit", Author = "J.R.R. Tolkien", PublishedYear = 1937, Genre = "Fantasy" }
            );
            await context.SaveChangesAsync();
        }

        // Seed reviews
        if (!context.Reviews.Any())
        {
            var alice = await userManager.FindByNameAsync("alice");
            var bob = await userManager.FindByNameAsync("bob");
            var firstBook = context.Books.First();

            var review1 = new Review
            {
                BookId = firstBook.Id,
                UserId = alice!.Id,
                Rating = 5,
                Content = "Amazing book!",
                DateCreated = DateTime.UtcNow
            };

            var review2 = new Review
            {
                BookId = firstBook.Id,
                UserId = bob!.Id,
                Rating = 4,
                Content = "Great read!",
                DateCreated = DateTime.UtcNow
            };

            await context.Reviews.AddRangeAsync(review1, review2);
            await context.SaveChangesAsync();

            // Seed review votes
            await context.ReviewVotes.AddRangeAsync(
                new ReviewVote
                {
                    ReviewId = review1.Id,
                    UserId = bob.Id,
                    IsUpvote = true
                },
                new ReviewVote
                {
                    ReviewId = review2.Id,
                    UserId = alice.Id,
                    IsUpvote = true
                }
            );

            await context.SaveChangesAsync();
        }
    }
}
