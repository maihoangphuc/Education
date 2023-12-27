namespace Education.Models
{
    public class NewsCategoryModel<T>
    {
        public T? Data { get; set; }
    }

    public class NewsItemCategoryModel
    {
        public int Id { get; set; }
        public int? SchoolId { get; set; }
        public int? ParentId { get; set; }
        public string Name { get; set; }
        public string NameSlug { get; set; }
        public string Description { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}
