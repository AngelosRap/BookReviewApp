using BookReviewApp.Domain.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace BookReviewApp.DataAccess.Mappings;

public class ReviewMap : IEntityTypeConfiguration<Review>
{
    public void Configure(EntityTypeBuilder<Review> builder)
    {
        builder.ToTable("Reviews");
        builder.HasKey(r => r.Id);

        builder.HasOne(r => r.User)
            .WithMany(u => u.Reviews)
            .OnDelete(DeleteBehavior.Restrict)
            .IsRequired();

        builder.HasMany(r => r.Votes)
            .WithOne(v => v.Review)
            .HasForeignKey(v => v.ReviewId)
            .OnDelete(DeleteBehavior.Cascade)
            .IsRequired();

        builder.Property(r => r.Content)
           .IsRequired()
           .HasMaxLength(1000);

        builder.Property(r => r.Rating)
               .IsRequired();

        builder.Property(r => r.BookId)
               .IsRequired();

    }
}