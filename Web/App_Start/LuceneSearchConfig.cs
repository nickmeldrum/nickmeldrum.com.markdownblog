using Lucene.Net.Analysis;
using Lucene.Net.Analysis.Standard;
using Lucene.Net.Documents;
using Lucene.Net.Index;
using Lucene.Net.Store;
using Lucene.Net.Store.Azure;
using MarkdownBlog.Net.Web.App_Start;
using MarkdownBlog.Net.Web.Models;

[assembly: WebActivatorEx.PostApplicationStartMethod(typeof(LuceneSearchConfig), "CreateIndex")]

namespace MarkdownBlog.Net.Web.App_Start {
    public class LuceneSearchConfig {
        public static void CreateIndex() {
            var cloudAccount = Azure.GetStorageAccount();

            using (var cacheDirectory = new RAMDirectory()) {
                using (var azureDirectory = new AzureDirectory(cloudAccount, "luceneIndex", cacheDirectory)) {
                    using (Analyzer analyzer = new StandardAnalyzer(Lucene.Net.Util.Version.LUCENE_30)) {
                        using (var indexWriter = new IndexWriter(azureDirectory, analyzer, true, IndexWriter.MaxFieldLength.UNLIMITED)) {
                            AddDocuments(indexWriter);

                            indexWriter.Commit();
                        }
                    }
                }
            }
        }

        private static void AddDocuments(IndexWriter writer) {
            var pages = new PagesMetadata(new ContentItemsMetaData<ContentItemMetaData>());
            var posts = new PostsMetadata(new ContentItemsMetaData<PostMetadata>());

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
                doc.Add(new Field("PublishDate", post.PublishDate.ToString("dd MMMM yyyy"), Field.Store.YES, Field.Index.NOT_ANALYZED));
                doc.Add(new Field("LastUpdatedDate", post.LastUpdatedDate.ToString("dd MMMM yyyy"), Field.Store.YES, Field.Index.NOT_ANALYZED));
                doc.Add(new Field("Author", post.Author, Field.Store.YES, Field.Index.ANALYZED));
                doc.Add(new Field("Body", new Post(post.Slug, posts).BodyWithoutHtml, Field.Store.YES, Field.Index.ANALYZED));

                writer.AddDocument(doc);
            }
        }
    }
}