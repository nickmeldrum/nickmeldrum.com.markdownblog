namespace MarkdownBlog.Net.Web.Models {
    public class StackOverflowFlairViewModel {
        private readonly StackOverflowFlair _data;

        public StackOverflowFlairViewModel(StackOverflowFlair data) {
            _data = data;
        }

        public string Link {
            get {
                return _data.UseStackExchangeFlair
                    ? string.Format("http://stackexchange.com/users/{0}", _data.Id)
                    : string.Format("http://stackoverflow.com/users/{0}/{1}", _data.Id, _data.UserName);
            }
        }

        public string ImageLink {
            get {
                return string.Format("http://{0}.com/users/flair/{1}.png",
                    _data.UseStackExchangeFlair ? "stackexchange" : "stackoverflow",
                    _data.Id);
             }
        }

        public string Text {
            get {
                return _data.UseStackExchangeFlair
                    ? string.Format("profile for {0} on Stack Exchange, a network of free, community-driven Q&A sites", _data.DisplayName)
                    : string.Format("profile for {0} at Stack Overflow, Q&A for professional and enthusiast programmers", _data.DisplayName);
            }
        }
    }
}