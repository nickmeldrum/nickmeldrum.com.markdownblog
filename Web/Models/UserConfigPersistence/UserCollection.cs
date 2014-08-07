using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using MarkdownBlog.Net.Web.Models.UserViewModel;

namespace MarkdownBlog.Net.Web.Models.UserConfigPersistence {
    public class UserCollection {
        public IEnumerable<User> Users {
            get {
                return ConfigurationManager.AppSettings.AllKeys
                .Where(k => k.StartsWith("username-"))
                .Select(u => new User {
                    Username = u.Split('-')[1],
                    Password = ConfigurationManager.AppSettings[u],
                    Role = u.Split('-')[2]
                });
            }
        }

        public User GetUser(string userName)
        {
            var user = Users.SingleOrDefault(u => string.Equals(u.Username, userName, StringComparison.InvariantCultureIgnoreCase));
            if (user == null)
                throw new KeyNotFoundException(string.Format("User {0} wasn't found", userName));
            return user;
        }
    }
}