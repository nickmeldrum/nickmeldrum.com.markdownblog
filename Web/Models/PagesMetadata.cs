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
                        if (HttpContext.Current.Cache["PagesMetadata"] == null) {
                            var showDrafts = ((Site)HttpContext.Current.Application["SiteSettings"]).ShowDrafts;

                            HttpContext.Current.Cache.Add(
                                "PagesMetadata",
                                new PagesMetadata(new ContentItemsMetaData<ContentItemMetaData>(), showDrafts),
                                null, DateTime.Now.AddHours(1), Cache.NoSlidingExpiration, CacheItemPriority.High, null);
                        }
                    }
                }

                return (PagesMetadata)HttpContext.Current.Cache["PagesMetadata"];
            }
        }

        public static readonly string PagesRoot = "Pages";

        public IEnumerable<ContentItemMetaData> List { get; private set; }

        public PagesMetadata(ContentItemsMetaData<ContentItemMetaData> contentItemsMetaData)
            : this(contentItemsMetaData, false) {
        }

        public PagesMetadata(ContentItemsMetaData<ContentItemMetaData> contentItemsMetaData, bool includeDrafts) {
            if (includeDrafts)
                List = contentItemsMetaData.ListIncludingDrafts(PagesRoot);
            else
                List = contentItemsMetaData.List(PagesRoot);
        }
    }
}