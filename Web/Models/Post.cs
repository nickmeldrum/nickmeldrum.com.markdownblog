﻿using System;
using System.IO;
using System.Web;
using MarkdownSharp;

namespace MarkdownBlog.Net.Web.Models {
    public class Post {
        private readonly string _postExtension = ".md";

        private readonly string _postName;

        private readonly HttpContextWrapper _httpContext;
        private string _body;

        public Post(string postName, HttpContextWrapper httpContext)
        {
            _postName = postName;
            _httpContext = httpContext;
        }

        private string PostBodyPath { get { return _httpContext.Server.MapPath(Posts.PostsRoot + _postName + _postExtension); } }

        public string Body {
            get {
                if (string.IsNullOrWhiteSpace(_body)) {

                    if (!File.Exists(PostBodyPath))
                        throw new Exception("404!"); // TODO: do this as a proper 404!

                    using (var reader = new StreamReader(PostBodyPath)) {
                        _body = new Markdown().Transform(reader.ReadToEnd());
                    }
                }
                return _body;
            }
        }
    }
}