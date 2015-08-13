using Lucene.Net.Analysis.Standard;
using Lucene.Net.QueryParsers;
using Lucene.Net.Search;
using Lucene.Net.Store;
using Lucene.Net.Store.Azure;
using MarkdownBlog.Net.Web.Models;
using System.Collections.Generic;
using System.Web.Mvc;

namespace MarkdownBlog.Net.Web.Controllers {
    public class SearchController : Controller {
        public ActionResult Index(string searchText) {
            if (string.IsNullOrWhiteSpace(searchText))
            {
                return View((object)null);
            }
            
            var searchResults = new List<SearchResult>();
            var cloudAccount = Azure.GetStorageAccount();

            using (var cacheDirectory = new RAMDirectory()) {
                using (var azureDirectory = new AzureDirectory(cloudAccount, Azure.StorageContainerName, cacheDirectory)) {
                    using (var analyzer = new StandardAnalyzer(Lucene.Net.Util.Version.LUCENE_30)) {
                        using (var searcher = new IndexSearcher(azureDirectory)) {

                            var parser = new QueryParser(Lucene.Net.Util.Version.LUCENE_30, "Body", analyzer);
                            var searchQuery = parser.Parse(searchText);
                            var hits = searcher.Search(searchQuery, 200);
                            var results = hits.TotalHits;

                            for (var i = 0; i < results; i++) {
                                var doc = searcher.Doc(hits.ScoreDocs[i].Doc);
                                searchResults.Add(new SearchResult(doc));
                            }
                        }
                    }
                }
            }

            return View(searchResults);
        }
    }
}