using System;
using Newtonsoft.Json;

namespace MarkdownBlog.Net.Web.Models
{
    public class PostMetadata : ContentItemMetaData
    {
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public DateTime PublishDate { get; set; }
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public DateTime LastUpdatedDate { get; set; }
        public string Author { get; set; }
        public string ShortDescription { get; set; }
    }
}