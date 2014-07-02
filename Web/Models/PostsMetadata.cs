using System;
using System.Collections.Generic;
using System.Linq;

namespace MarkdownBlog.Net.Web.Models {
    public class PostsMetadata {
        public static readonly string PostsRoot = "~/Posts/";

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