using System.ComponentModel.DataAnnotations;

namespace MarkdownBlog.Net.Web.Models.UserViewModel {
    public class ChangePasswordInput {
        [Required(ErrorMessage = "Old password is required")]
        public string OldPassword { get; set; }
        [Required(ErrorMessage = "New password is required")]
        public string NewPassword { get; set; }
        [Required(ErrorMessage = "Confirm password is required")]
        public string ConfirmPassword { get; set; }

        public static readonly string ValidationErrorChangePasswordFailed = "Attempt to change the password failed";
    }
}