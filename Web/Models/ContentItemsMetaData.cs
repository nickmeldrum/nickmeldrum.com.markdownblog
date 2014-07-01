using System.Collections.Generic;
using System.IO;
using System.Web;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace MarkdownBlog.Net.Web.Models {
    public class ContentItemsMetaData<T> where T : ContentItemMetaData {
        private readonly string _metadataFile = "metadata.json";
        private readonly HttpContextWrapper _httpContext;

        public ContentItemsMetaData(HttpContextWrapper httpContext) {
            _httpContext = httpContext;
        }

        public IList<T> List(string contentItemsRoot) {
            using (var reader = new StreamReader(MetaDataFilePath(contentItemsRoot))) {
                return JsonConvert.DeserializeObject<List<T>>(reader.ReadToEnd(), new IsoDateTimeConverter());
            }
        }

        private string MetaDataFilePath(string contentItemsRoot) {
            return _httpContext.Server.MapPath(contentItemsRoot + _metadataFile);
        }
    }
}