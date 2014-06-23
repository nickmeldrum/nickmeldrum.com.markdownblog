using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace MarkdownBlog.Net.Web.Models {
    public class Posts {
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

        private readonly string _metadataFile = "metadata.json";
        private readonly HttpContextWrapper _httpContext;

        public Posts(HttpContextWrapper httpContext) {
            _httpContext = httpContext;

            using (var reader = new StreamReader(MetaDataFilePath)) {
                List = JsonConvert.DeserializeObject<List<PostMetadata>>(reader.ReadToEnd(), new IsoDateTimeConverter());
            }
        }

        private string MetaDataFilePath { get { return _httpContext.Server.MapPath(PostsRoot + _metadataFile); } }
    }
}