namespace MainApi.Models.Filters
{
    public class Article
    {
        public int? CategoryId { get; set; }
        public int? UserId { get; set; }
        public bool? IsDeleted { get; set; }
        public bool? IsVisible { get; set; }

        public Sort Sort { get; set; } = Sort.Default;
    }

    public enum Sort
    {
        Default = 0,
        Ascending = 1,
        Descending = 2,
    }
}
