using System.ComponentModel.DataAnnotations;

namespace FreyaCare.ViewModels;

public class LoginViewModel
{
    [Required]
    [Display(Name = "Personal Code")]
    public string PersonalCode { get; set; } = string.Empty;

    [Required]
    [DataType(DataType.Password)]
    [Display(Name = "Password")]
    public string Password { get; set; } = string.Empty;
}