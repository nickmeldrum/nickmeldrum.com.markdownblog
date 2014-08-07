using System.Security.Principal;

namespace MarkdownBlog.Net.Web.Models.UserViewModel {
    public class Identity : IIdentity {
        private readonly bool isAuthenticated;
        private readonly string name;

        public Identity(bool isAuthenticated, string name) {
            this.isAuthenticated = isAuthenticated;
            this.name = name;
        }

        public string AuthenticationType {
            get { return "Forms"; }
        }

        public bool IsAuthenticated {
            get { return isAuthenticated; }
        }

        public string Name {
            get { return name; }
        }
    }
}