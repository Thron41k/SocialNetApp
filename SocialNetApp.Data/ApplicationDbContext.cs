using Microsoft.EntityFrameworkCore;
using SocialNetApp.Data.Models;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using SocialNetApp.Data.Configuration;

namespace SocialNetApp.Data;

public sealed class ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
    : IdentityDbContext<User>(options)
{
    public DbSet<Friend> Friends { get; set; }
    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);
        builder.ApplyConfiguration(new FriendConfiguration());
    }
}