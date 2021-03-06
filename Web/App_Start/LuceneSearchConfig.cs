﻿[assembly: WebActivatorEx.PostApplicationStartMethod(typeof(MarkdownBlog.Net.Web.App_Start.LuceneSearchConfig), "CreateIndex")]

namespace MarkdownBlog.Net.Web.App_Start
{
    using Lucene.Net.Analysis;
    using Lucene.Net.Analysis.Standard;
    using Lucene.Net.Documents;
    using Lucene.Net.Index;
    using Lucene.Net.Store;
    using Lucene.Net.Store.Azure;
    using MarkdownBlog.Net.Web.Models;
    using System;
    using System.Diagnostics;

    using Microsoft.WindowsAzure.Storage;

    public class LuceneSearchConfig
    {
        public static void CreateIndex() {
            try
            {
                var cloudAccount = Azure.GetStorageAccount();

                using (var cacheDirectory = new RAMDirectory())
                {
                    using (var azureDirectory = new AzureDirectory(cloudAccount, Azure.StorageContainerName, cacheDirectory))
                    {
                        using (Analyzer analyzer = new StandardAnalyzer(Lucene.Net.Util.Version.LUCENE_30))
                        {
                            using (var indexWriter = new IndexWriter(azureDirectory, analyzer, true, IndexWriter.MaxFieldLength.UNLIMITED))
                            {
                                AddDocuments(indexWriter);

                                indexWriter.Commit();
                            }
                        }
                    }
                }
            }
            catch (StorageException ex)
            {
                Trace.TraceError(ex.Message);
            }
        }

        private static void AddDocuments(IndexWriter writer) {
            var pages = PagesMetadata.Instance;
            var posts = PostsMetadata.Instance;

            foreach (var page in pages.List) {
                var doc = new Document();

                doc.Add(new Field("Url", "/" + page.Slug, Field.Store.YES, Field.Index.NOT_ANALYZED));
                doc.Add(new Field("Title", page.Title, Field.Store.YES, Field.Index.ANALYZED));
                doc.Add(new Field("Body", new Page(page.Slug, pages).BodyWithoutHtml, Field.Store.YES, Field.Index.ANALYZED));

                writer.AddDocument(doc);
            }

            foreach (var post in posts.List) {
                var doc = new Document();

                doc.Add(new Field("Url", "/blog/" + post.Slug, Field.Store.YES, Field.Index.NOT_ANALYZED));
                doc.Add(new Field("Title", post.Title, Field.Store.YES, Field.Index.ANALYZED));
                doc.Add(new Field("Description", post.ShortDescription, Field.Store.YES, Field.Index.ANALYZED));
                if (post.PublishDate != DateTime.MinValue)
                    doc.Add(new Field("PublishDate", post.PublishDate.ToString("dd MMMM yyyy"), Field.Store.YES, Field.Index.NOT_ANALYZED));
                if (post.LastUpdatedDate != DateTime.MinValue)
                    doc.Add(new Field("LastUpdatedDate", post.LastUpdatedDate.ToString("dd MMMM yyyy"), Field.Store.YES, Field.Index.NOT_ANALYZED));
                doc.Add(new Field("Author", post.Author, Field.Store.YES, Field.Index.ANALYZED));
                doc.Add(new Field("Body", new Post(post.Slug, posts).BodyWithoutHtml, Field.Store.YES, Field.Index.ANALYZED));

                writer.AddDocument(doc);
            }
        }
    }
}