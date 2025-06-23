using MainApi.Models.Entities;
using Microsoft.EntityFrameworkCore;

namespace MainApi.Context
{
    public interface IAppContext
    {
        public DbSet<Article> Articles { get; set; }
        public DbSet<Category> Categories { get; set; }
        public DbSet<Tag> Tags { get; set; }
        public ApplicationContext CreateDbContext();
        Task<int> SaveChangesAsync();

    }
}
