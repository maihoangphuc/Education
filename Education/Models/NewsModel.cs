namespace Education.Models
{
    public class NewsListModel
    {
        public int Result { get; set; }
        public long Time { get; set; }
        public string DataDescription { get; set; }
        public List<NewsItemModel> Data { get; set; }
        public int Data2nd { get; set; }
        public ErrorModel Error { get; set; }
    }

    public class NewsItemModel
    {
        public int Id { get; set; }
        public int SchoolId { get; set; }
        public int NewsCategoryId { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public Boolean IsHot { get; set; }
        public ImageObjModel ImageObj { get; set; }
        public DateTime PublishedAt { get; set; }
        // Add other properties as needed
    }

    public class ImageObjModel
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public string RelativeUrl { get; set; }
    }

    public class ErrorModel
    {
        public int Code { get; set; }
        public string Message { get; set; }
    }
}
