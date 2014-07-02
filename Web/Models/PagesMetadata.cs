using System;
using System.Collections.Generic;
using System.Web;
using System.Web.Caching;

namespace MarkdownBlog.Net.Web.Models {
    public class PagesMetadata {
        private static object syncRoot = new Object();

        public static PagesMetadata Instance {
            get {
                if (HttpContext.Current.Cache["PagesMetadata"] == null) {
                    lock (syncRoot) {
                        if (HttpContext.Current.Cache["PagesMetadata"] == null)
                            HttpContext.Current.Cache.Add("PagesMetadata", new PagesMetadata(new ContentItemsMetaData<ContentItemMetaData>(new HttpContextWrapper(HttpContext.Current))), null, DateTime.Now.AddHours(1), Cache.NoSlidingExpiration, CacheItemPriority.High, null);
                    }
                }

                return (PagesMetadata)HttpContext.Current.Cache["PagesMetadata"];
            }
        }

        public static readonly string PagesRoot = "~/Pages/";

        public IList<ContentItemMetaData> List { get; private set; }

        private PagesMetadata(ContentItemsMetaData<ContentItemMetaData> contentItemsMetaData) {
            List = contentItemsMetaData.List(PagesRoot);
        }
    }
}