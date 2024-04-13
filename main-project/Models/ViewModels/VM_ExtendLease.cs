using System.ComponentModel.DataAnnotations;
using Mysqlx;
using Validation;

namespace main_project.Models.ViewModels;

public class VM_ExtendLease
{
    [Required(ErrorMessage = "Mã hợp đồng là bắt buộc")]
    public int LeaseId { get; set; }
    [Required(ErrorMessage = "Ngày kết thúc mới là bắt buộc")]
    public DateTime NewEndDate { get; set; }
}