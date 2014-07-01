using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using MarkdownSharp;

namespace MarkdownBlog.Net.Web.Models {
    public class ContentItem {
        private readonly string _contentItemExtension = ".md";

        private readonly HttpContextWrapper _httpContext;

        private readonly string _contentItemName;
        private readonly string _contentRoot;

        private string _body;
        public ContentItemMetaData Metadata { get; set; }

        public ContentItem(string contentItemName, string contentRoot, HttpContextWrapper httpContext, IEnumerable<ContentItemMetaData> list)
        {
            _contentItemName = contentItemName;
            _contentRoot = contentRoot;
            _httpContext = httpContext;

            if (!File.Exists(ContentBodyPath) || !list.Any(p => p.Slug == _contentItemName))
            {
                throw new FileNotFoundException();
            }

            Metadata = list.Single(p => p.Slug == _contentItemName);
        }

        private string ContentBodyPath { get { return _httpContext.Server.MapPath(_contentRoot + _contentItemName + _contentItemExtension); } }

        public string Body {
            get {
                if (string.IsNullOrWhiteSpace(_body)) {

                    if (!File.Exists(ContentBodyPath))
                        throw new Exception("404!"); // TODO: do this as a proper 404!

                    using (var reader = new StreamReader(ContentBodyPath)) {
                        _body = new Markdown().Transform(reader.ReadToEnd());
                    }
                }
                return _body;
            }
        }
    }
}