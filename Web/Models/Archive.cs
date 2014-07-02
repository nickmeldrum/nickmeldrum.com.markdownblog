using System.Collections.Generic;

namespace MarkdownBlog.Net.Web.Models {
    public class Archive {
        public string Month { get; set; }
        public int Year { get; set; }

        public IEnumerable<PostMetadata> ArchivePosts { get { return PostsMetadata.Instance.PostsByMonth(Month, Year); } }
    }
}