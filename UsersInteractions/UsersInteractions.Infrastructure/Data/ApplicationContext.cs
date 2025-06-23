using Microsoft.EntityFrameworkCore;
using UsersInteractions.Application.Data;
using UsersInteractions.Domain.Models;

namespace UsersInteractions.Infrastructure.Data
{
    public class ApplicationContext : DbContext, IApplicationContext
    {
        private readonly DbContextOptions<ApplicationContext> _options;
        public DbSet<ChatMessage> ChatMessages { get; set; }
        public DbSet<Comment> Comments { get; set; }

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
            
        }
        public IApplicationContext CreateDbContext()
        {
            return new ApplicationContext(_options);
        }
        public async Task<int> SaveChangesAsync()
        {
            return await base.SaveChangesAsync();
        }
    }
}
