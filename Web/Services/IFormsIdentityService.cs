namespace MarkdownBlog.Net.Web.Services {
    public interface IFormsIdentityService {
        bool IsAuthenticated();
        string Username { get; }
    }
}