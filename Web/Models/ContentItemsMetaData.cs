﻿using System;
using System.Linq;
using System.Collections.Generic;
using System.IO;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace MarkdownBlog.Net.Web.Models {
    public class ContentItemsMetaData<T> where T : ContentItemMetaData {
        private readonly string _metadataFile = "metadata.json";

        public IEnumerable<T> ListIncludingDrafts(string contentItemsRoot)
        {
            using (var reader = new StreamReader(MetaDataFilePath(contentItemsRoot))) {
                return JsonConvert.DeserializeObject<List<T>>(reader.ReadToEnd(), new IsoDateTimeConverter());
            }
        }

        public IEnumerable<T> List(string contentItemsRoot)
        {
            return ListIncludingDrafts(contentItemsRoot).Where(x => x.Published);
        }

        private string MetaDataFilePath(string contentItemsRoot) {
            return Path.Combine(AppDomain.CurrentDomain.BaseDirectory, contentItemsRoot, _metadataFile);
        }
    }
}