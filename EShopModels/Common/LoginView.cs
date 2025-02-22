using System.ComponentModel.DataAnnotations;

namespace EShopModels.Common
{
    public class LoginView
    {
        public LoginView(string email, string password, bool rememberme)
        {
            this.Email = email;
            this.Password = password;
            this.RememberMe = rememberme;
        }

        [Required]
        [Display(Name = "Email Address")]
        [DataType(DataType.EmailAddress)]
        public string Email { get; set; }
        [Required]
        [DataType(DataType.Password)]
        [Display(Name = "Password")]
        public string Password { get; set; }
        [Display(Name = "Remember Me")]
        public bool RememberMe { get; set; }
        public string? ReturnUrl { get; set; }
    } 
}