using System.Collections.Generic;

namespace MarkdownBlog.Net.Web.Models {
    public class PagesMetadata {
        public static readonly string PagesRoot = "~/Pages/";

        public IList<ContentItemMetaData> List { get; private set; }

        public PagesMetadata(ContentItemsMetaData<ContentItemMetaData> contentItemsMetaData) {
            List = contentItemsMetaData.List(PagesRoot);
        }
    }
}