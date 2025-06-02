using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore;
using SocialNetApp.Data.Models;

namespace SocialNetApp.Data.Configuration;

public class FriendConfiguration : IEntityTypeConfiguration<Friend>
{

    public void Configure(EntityTypeBuilder<Friend> builder)
    {
        builder.ToTable("UserFriends").HasKey(p => p.Id);
        builder.Property(x => x.Id).UseIdentityColumn();
        builder.HasOne(f => f.User)
            .WithMany(u => u.Friends)
            .HasForeignKey(f => f.UserId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(f => f.CurrentFriend)
            .WithMany()
            .HasForeignKey(f => f.CurrentFriendId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}