using AuthApi.Models.Entities;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;

namespace AuthApi.Data;

public class ApplicationContext : IdentityDbContext<ApplicationUser>
{
    public DbSet<ApplicationUser> User { get; set; }
    public ApplicationContext(DbContextOptions<ApplicationContext> options) : base(options) { }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        #region User
        
        modelBuilder.Entity<ApplicationUser>().Property(x => x.ClientId)
            .ValueGeneratedOnAdd()
            .Metadata.SetAfterSaveBehavior(PropertySaveBehavior.Ignore);
        
        modelBuilder.Entity<ApplicationUser>().HasIndex(x => x.ClientId)
            .IsUnique();
        
        #endregion
    }
}