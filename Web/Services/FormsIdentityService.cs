using System.Security.Principal;

namespace MarkdownBlog.Net.Web.Services {
    public class FormsIdentityService : IFormsIdentityService {
        private readonly IIdentity identity;

        public FormsIdentityService(IIdentity identity) {
            this.identity = identity;
        }

        public bool IsAuthenticated() {
            return identity != null && identity.IsAuthenticated;
        }

        public string Username {
            get { return (identity == null) ? "" : identity.Name; }
        }

    }
}