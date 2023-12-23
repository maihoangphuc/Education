using System.ComponentModel.DataAnnotations;

namespace Education.Models
{
    public class LoginModel
    {
        [Required(ErrorMessage = "Tên người dùng là bắt buộc.")]
        public string Username { get; set; }

        [Required(ErrorMessage = "Mật khẩu là bắt buộc.")]
        [MinLength(8, ErrorMessage = "Mật khẩu phải có ít nhất 8 ký tự.")]
        public string Password { get; set; }
    }
}
