using System.Configuration;

namespace MarkdownBlog.Net.Web.Models {
    public class TwitterTimeline {
        public TwitterTimeline() {
            Query = ConfigurationManager.AppSettings["TwitterTimelineQuery"] ?? "@DefaultTwitterQuery";
            Url = ConfigurationManager.AppSettings["TwitterTimelineUrl"] ?? "https://twitter.com/DefaultTwitterTimelineUrl";
            WidgetId = long.Parse(ConfigurationManager.AppSettings["TwitterTimelineWidgetId"] ?? "-1");
        }
        public string Query { get; private set; }
        public string Url { get; private set; }
        public long WidgetId { get; private set; }
    }
}