using MainApi.Models.Entities;
using Microsoft.EntityFrameworkCore;

namespace MainApi.Context
{
    public class ApplicationContext : DbContext//, IAppContext
    {
        private readonly DbContextOptions<ApplicationContext> _options;
        public DbSet<Article> Articles { get; set; }
        public DbSet<Category> Categories { get; set; }
        public DbSet<Tag> Tags { get; set; }
        public DbSet<User> Users { get; set; }

        public ApplicationContext(DbContextOptions<ApplicationContext> options) : base(options)
        {
            _options = options;
        }
        public ApplicationContext() { }


        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.EnableSensitiveDataLogging();
            //if (!optionsBuilder.IsConfigured)
            //{
            //    optionsBuilder.UseSqlServer();

            //}
            //optionsBuilder.UseSqlServerO();
            base.OnConfiguring(optionsBuilder);
            var options = optionsBuilder;
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {


            // DataSeed.SeedData(modelBuilder);

            base.OnModelCreating(modelBuilder);

            #region User
            modelBuilder.Entity<User>().Property(x => x.UserName)
                .IsRequired()
                .HasMaxLength(50);

            modelBuilder.Entity<User>().HasKey(x => x.UserId);
            modelBuilder.Entity<User>().HasMany(x => x.Articles)
                .WithOne(x => x.User) 
                .HasForeignKey(x => x.UserId);
            modelBuilder.Entity<User>().Property(e => e.UserId).UseIdentityColumn();
            modelBuilder.Entity<User>().Property(e => e.Deleted).HasDefaultValue(false);
            #endregion

            #region Article
            modelBuilder.Entity<Article>().HasOne(x => x.User)
                .WithMany(x => x.Articles)
                .HasForeignKey(x => x.UserId);
            modelBuilder.Entity<Article>().HasKey(x => x.ArticleId);
            modelBuilder.Entity<Article>().Property(e => e.ArticleId).UseIdentityColumn();
            modelBuilder.Entity<Article>().HasOne(x => x.Category)
                .WithMany(x => x.Articles)
                .HasForeignKey(x => x.CategoryId);

            modelBuilder.Entity<Article>().HasMany(x => x.Tags)
                .WithMany(x => x.Articles);

            #endregion

            #region Category
            modelBuilder.Entity<Category>().HasOne(x => x.User) 
                .WithMany(x=> x.Categories)
                .HasForeignKey(x => x.UserId);
            modelBuilder.Entity<Category>().HasKey(x => x.CategoryId);
            modelBuilder.Entity<Category>().Property(e => e.CategoryId).UseIdentityColumn();
            modelBuilder.Entity<Category>().HasMany(x => x.Articles)
                .WithOne(x => x.Category)
                .HasForeignKey(x => x.CategoryId);
            #endregion

            #region Tag
            modelBuilder.Entity<Tag>().HasIndex(x => x.Title)
                .IsUnique();
            #endregion
        }
        public ApplicationContext CreateDbContext()
        {
            return new ApplicationContext(_options);
        }
        public async Task<int> SaveChangesAsync()
        {
            return await base.SaveChangesAsync();
        }
    }
}
