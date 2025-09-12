using BookReviewApp.Domain.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace BookReviewApp.DataAccess.Mappings;

public class ReviewVoteMap : IEntityTypeConfiguration<ReviewVote>
{
    public void Configure(EntityTypeBuilder<ReviewVote> builder)
    {
        builder.HasKey(v => v.Id);

        builder.Property(v => v.IsUpvote)
            .IsRequired();

        builder.Property(v => v.UserId)
            .IsRequired();

        builder.HasOne(u => u.User)
            .WithMany(u => u.Votes)
            .HasForeignKey(rv => rv.UserId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(rv => rv.Review)
            .WithMany(r => r.Votes)
            .HasForeignKey(r => r.ReviewId)
            .OnDelete(DeleteBehavior.NoAction);

        builder.HasIndex(v => new { v.ReviewId, v.UserId })
               .IsUnique();
    }
}
