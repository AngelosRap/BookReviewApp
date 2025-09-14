using Microsoft.AspNetCore.Identity;

namespace BookReviewApp.Domain.Models;

public class AppUser : IdentityUser
{
    public ICollection<Review> Reviews { get; set; } = [];
    public ICollection<ReviewVote> Votes { get; set; } = [];
}
