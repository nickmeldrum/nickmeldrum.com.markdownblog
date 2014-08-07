using System.ComponentModel.DataAnnotations;
using System.Security.Principal;

namespace MarkdownBlog.Net.Web.Models.UserViewModel {
    public class User : IPrincipal {
        public const string ValidationMessageUsernameRequired = "Username is required";
        public const string ValidationMessagePasswordRequired = "Password is required";
        public const string ValidationMessageUsernameOrPasswordIncorrect = "Username or password is incorrect";

        [Display(Name = "Username")]
        [DataType(DataType.Text)]
        [StringLength(250, MinimumLength = 4)]
        [Required(ErrorMessage = ValidationMessageUsernameRequired)]
        public string Username { get; set; }

        [Required(ErrorMessage = ValidationMessagePasswordRequired)]
        [DataType(DataType.Password)]
        [StringLength(250, MinimumLength = 6)]
        public string Password { get; set; }

        public string Role { get; set; }

        public string ReturnUrl { get; set; }


        public bool IsInRole(string role) {
            return Role == role;
        }

        public IIdentity Identity {
            get {
                return new Identity(IsInRole("User") || IsInRole("Admin"), Username);
            }
        }
    }
}