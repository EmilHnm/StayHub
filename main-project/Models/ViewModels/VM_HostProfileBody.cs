using System.ComponentModel.DataAnnotations;

namespace main_project.Models.ViewModels;

public class VM_HostProfileBody
{
    [Required(ErrorMessage = "Họ tên chủ nhà trọ không được để trống")]
    [MaxLength(255, ErrorMessage = "Họ tên chủ nhà trọ không được vượt quá 255 ký tự")]
    public required string fullname { get; set; }

    [Required(ErrorMessage = "Số điện thoại không được để trống")]
    [RegularExpression(@"^0?\d{10}$", ErrorMessage = "Số điện thoại không hợp lệ")]
    public required string phone { get; set; }

}