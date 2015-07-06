using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using MarkdownSharp;

namespace MarkdownBlog.Net.Web.Models {
    public class ContentItem {
        private readonly string _contentItemExtension = ".md";

        private readonly string _contentItemName;
        private readonly string _contentRoot;

        private string _body;
        public ContentItemMetaData Metadata { get; set; }

        public ContentItem(string contentItemName, string contentRoot, IEnumerable<ContentItemMetaData> list)
        {
            _contentItemName = contentItemName;
            _contentRoot = contentRoot;

            if (!File.Exists(ContentBodyPath) || !list.Any(p => p.Slug == _contentItemName))
            {
                throw new FileNotFoundException(string.Format("file: '{0}' not found when tring to load a contentitem", this.ContentBodyPath), this.ContentBodyPath);
            }

            Metadata = list.Single(p => p.Slug == _contentItemName);
        }

        private string ContentBodyPath { get { return Path.Combine(AppDomain.CurrentDomain.BaseDirectory, _contentRoot, _contentItemName + _contentItemExtension); } }

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