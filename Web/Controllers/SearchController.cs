using Lucene.Net.Analysis.Standard;
using Lucene.Net.QueryParsers;
using Lucene.Net.Search;
using Lucene.Net.Store;
using MarkdownBlog.Net.Web.Models;
using System.Collections.Generic;
using System.Web.Mvc;

namespace MarkdownBlog.Net.Web.Controllers {
    public class SearchController : Controller {
        public ActionResult Index(string searchText) {
            var searchResults = new List<SearchResult>();
            var indexDirectory = Server.MapPath("~/App_Data/LuceneIndexes");
            var analyzer = new StandardAnalyzer(Lucene.Net.Util.Version.LUCENE_30);
            var searcher = new IndexSearcher(FSDirectory.Open(indexDirectory));
            var parser = new QueryParser(Lucene.Net.Util.Version.LUCENE_30, "Body", analyzer);
            var searchQuery = parser.Parse(searchText);
            var hits = searcher.Search(searchQuery, 200);
            var results = hits.TotalHits;
            for (var i = 0; i < results; i++) {
                var doc = searcher.Doc(hits.ScoreDocs[i].Doc);
                var searchResult = new SearchResult(doc);
                searchResults.Add(searchResult);
            }

            return View(searchResults);
        }
    }
}