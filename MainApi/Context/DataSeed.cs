using Faker;
using MainApi.Models.Entities;
using Microsoft.EntityFrameworkCore;

namespace MainApi.Context;

public static class DataSeed
{
    public static void SeedData(ModelBuilder modelBuilder)
    {
        // Seed Users
        // var user = new User
        // {
        //     UserId = 1,
        //     UserName = Internet.UserName(),
        //     Deleted = false
        // };
        // modelBuilder.Entity<User>().HasData(user);

        // Seed Categories
        var categories = new List<Category>
        {
            new Category { CategoryId = 1, CategoryName = "Tech", CategoryDescription = "Technology articles", UserId = 1 },
            new Category { CategoryId = 2, CategoryName = "Life", CategoryDescription = "Lifestyle articles", UserId = 1 }
        };
        modelBuilder.Entity<Category>().HasData(categories);

        // Seed Tags
        var tags = new List<Tag>
        {
            new Tag { TagId = 1, Title = "C#" },
            new Tag { TagId = 2, Title = "Blazor" },
            new Tag { TagId = 3, Title = "AI" }
        };
        modelBuilder.Entity<Tag>().HasData(tags);

        // Seed Articles
        var articles = Enumerable.Range(1, 5).Select(i => new Article
        {
            ArticleId = i,
            Name = Lorem.Sentence(),
            Description = Lorem.Paragraphs(1).First(),
            CategoryId = categories[(i - 1) % categories.Count].CategoryId,
            UserId = 1,
            Visibility = true
        }).ToList();
        modelBuilder.Entity<Article>().HasData(articles);

        // Seed Article-Tag many-to-many (join table)
        var articleTags = new List<object>();
        int relId = 1;
        foreach (var article in articles)
        {
            var tagIds = tags.OrderBy(_ => System.Guid.NewGuid()).Take(2).Select(t => t.TagId);
            foreach (var tagId in tagIds)
            {
                articleTags.Add(new { ArticlesArticleId = article.ArticleId, TagsTagId = tagId });
            }
        }
        modelBuilder.Entity("ArticleTag").HasData(articleTags);
    }
}