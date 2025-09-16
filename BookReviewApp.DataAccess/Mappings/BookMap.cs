using BookReviewApp.Domain.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace BookReviewApp.DataAccess.Mappings;

public class BookMap : IEntityTypeConfiguration<Book>
{
    public void Configure(EntityTypeBuilder<Book> builder)
    {
        builder.ToTable("Books");
        builder.HasKey(x => x.Id);

        builder.HasMany(b => b.Reviews)
            .WithOne(r => r.Book)
            .HasForeignKey(r => r.BookId)
            .OnDelete(DeleteBehavior.Cascade)
            .IsRequired();

        builder.Property(b => b.Title)
           .IsRequired()
           .HasMaxLength(100);

        builder.Property(b => b.Author)
               .IsRequired()
               .HasMaxLength(100);

        builder.Property(b => b.PublishedYear)
               .IsRequired();

        builder.Property(b => b.Genre)
               .IsRequired()
               .HasMaxLength(50);
    }
}
