using System.ComponentModel.DataAnnotations;

namespace main_project.Models.ViewModels;

public class VM_HostelForm
{
    public int? Id { get; set; }
    [Required(ErrorMessage = "Tên nhà trọ không được để trống")]
    public required string Name { get; set; } = "";
    [Required(ErrorMessage = "Địa chỉ không được để trống")]
    public required string Address { get; set; } = "";
    [Required(ErrorMessage = "Mô tả không được để trống")]
    public required string Description { get; set; } = "";
    [Required(ErrorMessage = "Số phòng không được để trống")]
    [Range(1, int.MaxValue, ErrorMessage = "Số phòng phải lớn hơn 1")]
    public required int Rooms { get; set; } = 0;
    [Required(ErrorMessage = "Số tiên không được để trống")]
    [Range(10000, int.MaxValue, ErrorMessage = "Số tiên phải lớn hơn 10000đ")]
    public required int Fee { get; set; } = 0;
    [Required(ErrorMessage = "Loại phòng trọ là bắt buộc")]
    public required int AccommodationTypeId { get; set; } = 0;
    public List<int>? Utilities { get; set; }
    public List<IFormFile>? ImageFiles { get; set; }
    public List<string>? ImageFileURLs { get; set; }
    [RegularExpression(@"^(active|inactive)$", ErrorMessage = "Trạng thái không hợp lệ")]
    public string? Status { get; set; }
}