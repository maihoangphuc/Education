namespace Education.Models
{
    public class NewsModel<T>
    {
        public T? Data { get; set; }
    }

    public class NewsItemModel
    {
        public int Id { get; set; }
        public int SchoolId { get; set; }
        public int NewsCategoryId { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public bool IsHot { get; set; }
        public ImageObjModel? ImageObj { get; set; }
        public DateTime PublishedAt { get; set; }
    }

    public class ImageObjModel
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public string RelativeUrl { get; set; }
    }
}
