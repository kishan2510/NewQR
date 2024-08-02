using System.ComponentModel.DataAnnotations;

namespace NewQR.Models
{
    public class Auth
    {
        [Key]
        public int UserId { get; set; }

        [EmailAddress]
        [Required(ErrorMessage = "Please Enter Email")]
        [Display(Name = "Enter Your Email")]
        public string Email { get; set; }

        [Required(ErrorMessage = "Please Enter Password")]
        [DataType(DataType.Password)]
        [RegularExpression(@"^(?=.*[A-Za-z])(?=.*\d)[A-Za-z\d]{6,20}$", ErrorMessage = "Password must be 6-20 characters long and contain at least one letter and one number.")]
        public string Password { get; set; }


    }
}
