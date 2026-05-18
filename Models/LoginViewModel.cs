using System.ComponentModel.DataAnnotations;

namespace ChemicalSDS.Models
{
    public class LoginViewModel
    {
        [Required(ErrorMessage = "Username is required")]
        [Display(Name = "Username")]
        public string Username { get; set; } = string.Empty;

        [Required(ErrorMessage = "Password is required")]
        [DataType(DataType.Password)]
        public string Password { get; set; } = string.Empty;

        public int CaptchaNum1 { get; set; }
        public int CaptchaNum2 { get; set; }

        [Required(ErrorMessage = "Please solve the math challenge")]
        [Display(Name = "Answer")]
        public int? CaptchaAnswer { get; set; }
    }
}
