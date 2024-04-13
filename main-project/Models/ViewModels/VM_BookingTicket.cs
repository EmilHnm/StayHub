using System.ComponentModel.DataAnnotations;

public class VM_BookingTicket
{
    [Required(ErrorMessage = "Mã phòng trọ là bắt buốc")]
    public int HostelId { get; set; }
    public DateTime Today { get; set; } = DateTime.Now;
    [Required(ErrorMessage = "Ngày bắt đầu là bắt buốc")]
    [DateGreaterThan("Today", ErrorMessage = "Ngày bắt đầu phải sau ngày hiện tại")]
    public DateTime StartDate { get; set; }
    [Required(ErrorMessage = "Ngày kết thúc là bắt buốc")]
    [DateGreaterThan("StartDate", numberOfDays: 3, ErrorMessage = "Ngày kết thúc phải sau ngày bắt đầu ít nhất 3 ngày")]
    public DateTime EndDate { get; set; }
}