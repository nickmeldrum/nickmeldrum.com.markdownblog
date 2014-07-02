using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Caching;

namespace MarkdownBlog.Net.Web.Models {
    public class PostsMetadata {
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
                        HttpContext.Current.Cache.Add("PostsMetadata", new PostsMetadata(new ContentItemsMetaData<PostMetadata>()), null, DateTime.Now.AddHours(1), Cache.NoSlidingExpiration, CacheItemPriority.High, null);
                }
             }

              return (PostsMetadata)HttpContext.Current.Cache["PostsMetadata"];
          }
       }

        public static readonly string PostsRoot = "Posts";

        public IList<PostMetadata> List { get; private set; }
        public IList<PostMetadata> Latest(int number) { return List.OrderByDescending(p => p.PublishDate).Take(number).ToList(); }

        public IEnumerable<PostMetaDataWithMonthAndYearGrouping> MonthlyArchiveLinks {
            get
            {
                return List.GroupBy(
                        k => new DateTime(k.PublishDate.Year, k.PublishDate.Month, 1),
                        (key, g) => new PostMetaDataWithMonthAndYearGrouping { MonthAndYearGrouping = key, PostMetaDataList = g }).ToList();
            }
        }

        public IEnumerable<PostMetadata> PostsByMonth(string month, int year) {
            return List.Where(p => p.PublishDate.ToString("MMM") == month && p.PublishDate.Year == year);
        }

        public PostsMetadata(ContentItemsMetaData<PostMetadata> contentItemsMetaData)
        {
            List = contentItemsMetaData.List(PostsRoot);
        }
    }
}