using System.Collections.Generic;

namespace MarkdownBlog.Net.Web.Models {
    public class Pages {
        public static readonly string PagesRoot = "~/Pages/";

        public IList<ContentItemMetaData> List { get; private set; }

        public Pages(ContentItemsMetaData<ContentItemMetaData> contentItemsMetaData) {
            List = contentItemsMetaData.List(PagesRoot);
        }
    }
}