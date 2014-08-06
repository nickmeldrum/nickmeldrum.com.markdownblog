using System;
using MarkdownBlog.Net.Web.Models.UserConfigPersistence;
using MarkdownBlog.Net.Web.Models.UserViewModel;
using System.Linq;

namespace MarkdownBlog.Net.Web.Services {
    public class UserService : IUserService {
        private readonly UserCollection _userCollection;

        public UserService(UserCollection userCollection)
        {
            _userCollection = userCollection;
        }

        public int MinPasswordLength {
            get { return 4; }
        }

        public bool UserExists(string username) {
            return _userCollection.Users.Any(user => string.Equals(user.Username, username, StringComparison.InvariantCultureIgnoreCase));
        }

        public bool ValidateUser(string username, string password) {
            return _userCollection.Users
                .Any(user => string.Equals(user.Username, username, StringComparison.InvariantCultureIgnoreCase) && 
                user.Password == password);
        }

        public UserCreateStatus CreateUser(string username, string password) {
            throw new System.NotImplementedException();
        }

        public bool ChangePassword(string username, string oldPassword, string newPassword) {
            throw new System.NotImplementedException();
        }
    }
}