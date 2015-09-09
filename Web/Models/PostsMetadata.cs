using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Caching;

namespace MarkdownBlog.Net.Web.Models {
    public class PostsMetadata {
        private readonly bool includeDrafts;

        private static object syncRoot = new Object();

        public static PostsMetadata Instance
       {
          get 
          {
              if (HttpContext.Current.Cache["PostsMetadata"] == null) 
             {
                lock (syncRoot) 
                {
                    if (HttpContext.Current.Cache["PostsMetadata"] == null)
                    {
                        var showDrafts = ((Site) HttpContext.Current.Application["SiteSettings"]).ShowDrafts;
                        HttpContext.Current.Cache.Add(
                            "PostsMetadata",
                            new PostsMetadata(new ContentItemsMetaData<PostMetadata>(), showDrafts),
                            null, DateTime.Now.AddHours(1), Cache.NoSlidingExpiration, CacheItemPriority.High, null);
                    }
                }
             }

              return (PostsMetadata)HttpContext.Current.Cache["PostsMetadata"];
          }
       }

        public static readonly string PostsRoot = "Posts";

        public IEnumerable<PostMetadata> List { get; private set; }

        public IEnumerable<PostMetadata> Latest(int number)
        {
            var latest = this.List.OrderByDescending(p => p.PublishDate).Take(number);
            
            if (this.includeDrafts)
                latest = this.List.Where(p => p.Published == false).Union(latest);

            return latest;
        }

        public IEnumerable<PostMetaDataWithMonthAndYearGrouping> MonthlyArchiveLinks {
            get
            {
                return List.GroupBy(
                        k => new DateTime(k.PublishDate.Year, k.PublishDate.Month, 1),
                        (key, g) => new PostMetaDataWithMonthAndYearGrouping { MonthAndYearGrouping = key, PostMetaDataList = g });
            }
        }

        public IEnumerable<PostMetadata> PostsByMonth(string month, int year) {
            return List.Where(p => p.PublishDate.ToString("MMM").ToLowerInvariant() == month.ToLowerInvariant() && p.PublishDate.Year == year);
        }

        public PostsMetadata(ContentItemsMetaData<PostMetadata> contentItemsMetaData)
            : this(contentItemsMetaData, false) {
        }

        public PostsMetadata(ContentItemsMetaData<PostMetadata> contentItemsMetaData, bool includeDrafts)
        {
            this.includeDrafts = includeDrafts;
            if (includeDrafts)
                List = contentItemsMetaData.ListIncludingDrafts(PostsRoot);
            else
                List = contentItemsMetaData.List(PostsRoot);
        }
    }
}