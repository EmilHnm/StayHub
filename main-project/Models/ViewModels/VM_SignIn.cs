using System.ComponentModel.DataAnnotations;
using Mysqlx;

namespace main_project.Models.ViewModels
{
    public class VM_SignIn
    {
        [Required(ErrorMessage = "Email is required")]
        [EmailAddress(ErrorMessage = "Invalid email address")]
        public required string Email { get; set; }
        [Required(ErrorMessage = "Password is required")]
        [MinLength(6, ErrorMessage = "Password must be at least 6 characters long")]
        public required string Password { get; set; }
        public bool RememberMe { get; set; }
    }
}