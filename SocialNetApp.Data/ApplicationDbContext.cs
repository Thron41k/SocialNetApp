using Microsoft.EntityFrameworkCore;
using SocialNetApp.Data.Models;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using SocialNetApp.Data.Configuration;

namespace SocialNetApp.Data;

public sealed class ApplicationDbContext : IdentityDbContext<User>
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
    {
        Database.Migrate();
    }

    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);
        builder.ApplyConfiguration(new FriendConfiguration());
        builder.ApplyConfiguration(new MessageConfiguration());
    }
}