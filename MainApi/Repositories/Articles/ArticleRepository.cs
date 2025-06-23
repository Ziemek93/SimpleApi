using MainApi.Context;
using MainApi.Models.ArticlesDao;
using MainApi.Models.Entities;
using Microsoft.EntityFrameworkCore;

namespace MainApi.Repositories.Articles;

public class ArticleRepository : IArticleRepository
{
    private readonly ApplicationContext _context;

    public ArticleRepository(ApplicationContext context)
    {
        _context = context;
    }

    public async Task<IEnumerable<Article>> GetArticlesAsync(CancellationToken token)
    {
        await using var context = _context.CreateDbContext();

        return await context.Articles
            //.Include(a => a.Comments)
            .Include(a => a.Tags)
            .ToListAsync(token); //GetArticles(context);
    }

    public async Task<bool> CategoryExistAsync(int id, CancellationToken token = default)
    {
        //using var context = _context;
        await using var context = _context.CreateDbContext();
        var categoryExists = await context.Categories.Where(c => c.CategoryId == id).AnyAsync();
        return categoryExists;
    }
    public async Task<bool> ArticleExistAsync(Models.Entities.Article request, CancellationToken token = default)
    {
        await using var context = _context.CreateDbContext();
        var categoryExists = context.Articles.Where(c => c.ArticleId == request.ArticleId).AnyAsync();
        return await categoryExists;
    }

    public async Task<int> AddArticleAsync(Models.Entities.Article request, CancellationToken token = default)
    {
        await using var context = _context.CreateDbContext();
        var response = context.Articles.Add(request);
        var result = await context.SaveChangesAsync(token);

        return result;
    }
    public async Task<Models.Entities.Article?> GetArticleAsync(int id, CancellationToken token = default)
    {
        await using var context = _context.CreateDbContext();
        var article = await context.Articles?
            .Where(x => x.ArticleId == id)
            //.Include(a => a.Comments)
            .Include(a => a.Tags)
            .FirstOrDefaultAsync(cancellationToken: token);
        return article;

    }

    private async Task<IEnumerable<Tag>> InsertTagsAsync(List<string> tagsTitles, ApplicationContext context, CancellationToken token = default)
    {
        {
            List<string> toRemove = new List<string>();

            foreach (var tag in tagsTitles)
            {
                var result = await context.Tags.Where(x => x.Title == tag).FirstOrDefaultAsync(token);
                if (result != null)
                {
                    toRemove.Add(tag);
                }

            }
            foreach (var title in toRemove)
            {
                tagsTitles.Remove(title);
            }
            if (tagsTitles.Count() > 0)
            {
                IEnumerable<Tag> tags = tagsTitles.Select(x => new Tag
                {
                    Title = x,
                });
                //context.Tags.AddRange(tags);//.ToList();
                //var ResponsResult = await context.SaveChangesAsync(token);

                return tags;

            }
            return null;
        };
    }
    public async Task<bool> EditArticleAsync(int id, ArticleDao req, CancellationToken token = default)
    {

        using (var context = _context)
        {
            var tags = await InsertTagsAsync(req.Tags, _context, token);

            var entity = await context.Articles
                .Where(x => x.ArticleId == id)
                .FirstOrDefaultAsync();
            if (entity == null)
            {
                return false;
            }
            entity.Name = req.ArticleName;
            entity.Description = req.ArticleDescription;
            entity.Tags = tags.ToList();

            var result = await context.SaveChangesAsync(token);

            return result != 0;

        };
    }

    public async Task<bool> DeleteArticleAsync(int id, CancellationToken token = default)
    {
        using (var context = _context)
        {
            var article = await context.Articles?
            .Where(x => x.ArticleId == id)
            //.Include(a => a.Comments)
            .FirstOrDefaultAsync(cancellationToken: token);
            if (article == null)
            {
                return false;
            }
            var result = _context.Articles.Remove(article);
            if (result != null)
            {
                return true;
            }
            return false;
        }
    }
}
