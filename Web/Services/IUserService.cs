using MarkdownBlog.Net.Web.Models.UserViewModel;

namespace MarkdownBlog.Net.Web.Services {
    public interface IUserService {
        int MinPasswordLength { get; }

        bool UserExists(string username);
        bool ValidateUser(string username, string password);
        UserCreateStatus CreateUser(string username, string password);
        bool ChangePassword(string username, string oldPassword, string newPassword);
    }
}