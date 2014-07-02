using Lucene.Net.Documents;

namespace MarkdownBlog.Net.Web.Models {
    public class SearchResult {
        public string Url { get; set; }
        public string Title { get; set; }
        public string Body { get; set; }
        public string Description { get; set; }
        public string PublishDate { get; set; }
        public string LastUpdatedDate { get; set; }
        public string Author { get; set; }

        public SearchResult(Document luceneDocument) {
            Url = luceneDocument.Get("Url");
            Title = luceneDocument.Get("Title");
            Body = luceneDocument.Get("Body");
            Description = luceneDocument.Get("Description");
            PublishDate = luceneDocument.Get("PublishDate");
            LastUpdatedDate = luceneDocument.Get("LastUpdatedDate");
            Author = luceneDocument.Get("Author");
        }
    }
}