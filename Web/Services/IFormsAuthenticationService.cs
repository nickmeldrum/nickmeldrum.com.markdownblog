namespace MarkdownBlog.Net.Web.Services {
    public interface IFormsAuthenticationService {
        void SignIn(string userName, bool createPersistentCookie);
        void SignOut();
    }
}