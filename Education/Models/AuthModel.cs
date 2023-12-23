using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace Education.Models
{
    public class AccountModel
    {
        [DisplayName("Tên tài khoản")]
        [Required(ErrorMessage = "Tên người dùng là bắt buộc.")]
        public string Username { get; set; }

        [DisplayName("Mật khẩu")]
        [Required(ErrorMessage = "Mật khẩu là bắt buộc.")]
        [MinLength(8, ErrorMessage = "Mật khẩu phải có ít nhất 8 ký tự.")]
        public string Password { get; set; }
    }
}
