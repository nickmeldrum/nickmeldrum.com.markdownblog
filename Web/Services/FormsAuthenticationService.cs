using System;
using System.Web.Security;

namespace MarkdownBlog.Net.Web.Services {
    public class FormsAuthenticationService : IFormsAuthenticationService {
        public void SignIn(string userName, bool createPersistentCookie) {
            if (string.IsNullOrEmpty(userName))
                throw new ArgumentException("Value cannot be null or empty.", "userName");

            FormsAuthentication.SetAuthCookie(userName, createPersistentCookie);
        }

        public void SignOut() {
            FormsAuthentication.SignOut();
        }
    }
}