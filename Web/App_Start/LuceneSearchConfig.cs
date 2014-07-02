using System.Web;
using Lucene.Net.Analysis;
using Lucene.Net.Analysis.Standard;
using Lucene.Net.Documents;
using Lucene.Net.Index;
using Lucene.Net.Store;
using System;
using System.Threading.Tasks;
using MarkdownBlog.Net.Web.App_Start;
using MarkdownBlog.Net.Web.Models;

[assembly: WebActivatorEx.PostApplicationStartMethod(typeof(LuceneSearchConfig), "InitializeSearch")]
[assembly: WebActivatorEx.ApplicationShutdownMethodAttribute(typeof(LuceneSearchConfig), "FinalizeSearch")]

namespace MarkdownBlog.Net.Web.App_Start {
    public class LuceneSearchConfig {
        public static Directory Directory;
        public static Analyzer Analyzer;
        public static IndexWriter Writer;

        public static void InitializeSearch() {
            var directoryPath = AppDomain.CurrentDomain.BaseDirectory + @"\App_Data\LuceneIndexes";
            Directory = FSDirectory.Open(directoryPath);
            Analyzer = new StandardAnalyzer(Lucene.Net.Util.Version.LUCENE_30);
            Writer = new IndexWriter(Directory, Analyzer, true, IndexWriter.MaxFieldLength.UNLIMITED);
            Task.Factory.StartNew(CreateIndex);
        }

        private static void CreateIndex()
        {
            var pages = new PagesMetadata(new ContentItemsMetaData<ContentItemMetaData>());
            var posts = new PostsMetadata(new ContentItemsMetaData<PostMetadata>());

            foreach (var page in pages.List) {
                var doc = new Document();

                doc.Add(new Field("Url", "/" + page.Slug, Field.Store.YES, Field.Index.NOT_ANALYZED));
                doc.Add(new Field("Title", page.Title, Field.Store.YES, Field.Index.ANALYZED));
                doc.Add(new Field("Body", new Page(page.Slug, pages).BodyWithoutHtml, Field.Store.YES, Field.Index.ANALYZED));

                Writer.AddDocument(doc);
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

                Writer.AddDocument(doc);
            }

            Writer.Optimize();
            Writer.Commit();
            Writer.Dispose();
        }

        public static void FinalizeSearch() {
            Directory.Dispose();
        }
    }
}