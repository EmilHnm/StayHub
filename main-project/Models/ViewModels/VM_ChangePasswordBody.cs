using System.ComponentModel.DataAnnotations;
using Mysqlx;
using Validation;

namespace main_project.Models.ViewModels;

public class VM_ChangePasswordBody
{
    [Required(ErrorMessage = "Mật khẩu hiện tại là bắt buộc")]
    public required string currentPassword { get; set; }

    [Required(ErrorMessage = "Mật khẩu mới là bắt buộc")]
    [MinLength(6, ErrorMessage = "Mật khẩu phải có ít nhất 6 ký tự")]
    [NotEqual(nameof(currentPassword), ErrorMessage = "Mật khẩu mới không được giống mật khẩu hiện tại")]
    public required string newPassword { get; set; }
    [Compare(nameof(newPassword), ErrorMessage = "Mật khẩu xác nhận phải giống mật khẩu mới")]
    public required string confirmNewPassword { get; set; }
}