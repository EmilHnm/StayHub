using System.ComponentModel.DataAnnotations;

namespace main_project.Models.ViewModels;

public class VM_RenterProfileBody
{
    [Required(ErrorMessage = "Họ tên chủ nhà trọ không được để trống")]
    [MaxLength(255, ErrorMessage = "Họ tên chủ nhà trọ không được vượt quá 255 ký tự")]
    public required string fullname { get; set; }

    [Required(ErrorMessage = "Giới tính không được để trống")]
    [RegularExpression(@"^(Nam|Nữ)$", ErrorMessage = "Giới tính không hợp lệ")]
    public required string gender { get; set; }

    [Required(ErrorMessage = "Số điện thoại không được để trống")]
    [RegularExpression(@"^0?\d{10}$", ErrorMessage = "Số điện thoại không hợp lệ")]
    public required string phone { get; set; }

    [Required(ErrorMessage = "Địa chỉ không được để trống")]
    [MaxLength(255, ErrorMessage = "Địa chỉ không được vượt quá 255 ký tự")]
    public required string address { get; set; }

    [Required(ErrorMessage = "Số CCCD không được để trống")]
    [RegularExpression(@"^\d{12}$", ErrorMessage = "Số CCCD không hợp lệ")]
    public required string identityCardNumber { get; set; }

}