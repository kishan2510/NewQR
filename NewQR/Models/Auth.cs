using System.ComponentModel.DataAnnotations;

namespace NewQR.Models
{
    //Defining my model as auth which contains parameters as userid email and password
    //after this we add igration and update databse to make a table accordingly.
    public class Auth
    {
        [Key]
        public int UserId { get; set; }

        [EmailAddress]
        [Required(ErrorMessage = "Please Enter Email")] //Added server side authentication of asp.net 
        [Display(Name = "Enter Your Email")]
        public string Email { get; set; }

        [Required(ErrorMessage = "Please Enter Password")]
        [DataType(DataType.Password)]
        [RegularExpression(@"^(?=.*[A-Za-z])(?=.*\d)[A-Za-z\d]{6,20}$", ErrorMessage = "Password must be 6-20 characters long and contain at least one letter and one number.")]
        public string Password { get; set; }


    }
}
