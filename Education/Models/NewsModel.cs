using System.ComponentModel.DataAnnotations;

namespace Education.Models
{
    public class NewsModel<T>
    {
        public T? Data { get; set; }
    }

    public class NewsItemModel
    {
        [Key]
        public int Id { get; set; }

        [Required(ErrorMessage = "Trường SchoolId là bắt buộc.")]
        public int? SchoolId { get; set; }

        [Required(ErrorMessage = "Trường NewsCategoryId là bắt buộc.")]
        public int? NewsCategoryId { get; set; }

        [Required(ErrorMessage = "Trường tên là bắt buộc.")]
        [StringLength(255, ErrorMessage = "Trường Name không được vượt quá 255 ký tự.")]
        public string Name { get; set; }

        [Required(ErrorMessage = "Trường mô tả là bắt buộc.")]
        [StringLength(1000, ErrorMessage = "Trường mô tả không được vượt quá 1000 ký tự.")]
        public string Description { get; set; }

        public bool IsHot { get; set; }

        public int Status { get; set; }

        [Required(ErrorMessage = "Trường MetaUrl là bắt buộc.")]
        public string MetaUrl { get; set; }

        public ImageObjModel? ImageObj { get; set; }

        [Required(ErrorMessage = "Trường datetime là bắt buộc.")]
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
