namespace Education.Models
{
    public class NewsListModel<T>
    {
        public int Result { get; set; }
        public long Time { get; set; }
        public string DataDescription { get; set; }
        public T Data { get; set; }
    }

    public class NewsItemModel
    {
        public int Id { get; set; }
        public int SchoolId { get; set; }
        public int NewsCategoryId { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public bool IsHot { get; set; }
        public ImageObjModel ImageObj { get; set; }
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
