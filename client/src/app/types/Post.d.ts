type PostDetailItem = {
    id: string
    articleName: string
    articleDescription: Date
    visibility: boolean
    tags: string[]
    category: Category
}

type PostListItem = {
    id: string
    name: string
    description: Date
    visibility: boolean
    tags: string[]
    category: Category
}
// ArticleName = a.Name,
//     ArticleDescription = a.Description,
//     Visibility = a.Visibility,
//     Tags = a.Tags.Select(c => c.Title).ToList(),