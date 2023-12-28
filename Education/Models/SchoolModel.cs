using System.ComponentModel.DataAnnotations;

namespace Education.Models
{
    public class SchoolModel<T>
    {
        public T? Data { get; set; }
    }

    public class SchoolItemModel
    {
        [Key]
        public int Id { get; set; }
        public int? AddressId { get; set; }
        public string Name { get; set; }
        public string ShortName { get; set; }
        public string Description { get; set; }
        public string LogoUrl { get; set; }
        public string FaviconUrl { get; set; }
        public string HotLine { get; set; }
        public string Phone { get; set; }
        public string Email { get; set; }
        public string Facebook { get; set; }
        public string Twitter { get; set; }
        public string Instagram { get; set; }
        public string Zalo { get; set; }
        public string Youtube { get; set; }
        public string TiktokUrl { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}



