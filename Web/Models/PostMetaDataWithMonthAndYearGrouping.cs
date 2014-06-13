using System;
using System.Collections.Generic;

namespace MarkdownBlog.Net.Web.Models {
    public class PostMetaDataWithMonthAndYearGrouping {
        public DateTime MonthAndYearGrouping { get; set; }
        public IEnumerable<PostMetadata> PostMetaDataList { get; set; }
    }
}