using System;
using System.Collections.Generic;
using System.ServiceModel.Syndication;
using System.Web;
using MarkdownBlog.Net.Web.Controllers;

namespace MarkdownBlog.Net.Web.Models {
    public class Feed
    {
        private readonly Site _site;
        private readonly HttpContextWrapper _contextWrapper;
        private SyndicationFeed _feed;

        public Feed(PostsMetadata postsMetadata, HttpContextWrapper contextWrapper) {
            _site = new Site();
            _contextWrapper = contextWrapper;

            SetupFeed();

            AddPostsToFeed(postsMetadata);
        }

        public FeedResult GetFeedXml(FeedType feedType) {
            switch (feedType) {
                case FeedType.atom:
                    return new FeedResult(new Atom10FeedFormatter(_feed));
                case FeedType.rss:
                    return new FeedResult(new Rss20FeedFormatter(_feed));
            }

            throw new Exception("Unknown feed type");
        }

        private void SetupFeed() {
            _feed = new SyndicationFeed {
                Title = SyndicationContent.CreatePlaintextContent(_site.Name),
                Description = SyndicationContent.CreatePlaintextContent(_site.Description),
                Copyright = SyndicationContent.CreatePlaintextContent("Copyright " + _site.Owner),
                Language = "en-gb"
            };

            _feed.Links.Add(SyndicationLink.CreateAlternateLink(_contextWrapper.GetAbsoluteUrl("~/blog")));
            _feed.Links.Add(SyndicationLink.CreateSelfLink(new Uri(_contextWrapper.Request.Url.AbsoluteUri)));
        }

        private void AddPostsToFeed(PostsMetadata postsMetadata) {
            var feedItems = new List<SyndicationItem>();

            foreach (var post in postsMetadata.List) {
                var item = new SyndicationItem {
                    Title = SyndicationContent.CreatePlaintextContent(post.Title),
                    Summary = SyndicationContent.CreatePlaintextContent(post.ShortDescription)
                };

                if (post.PublishDate != DateTime.MinValue)
                    item.PublishDate = post.PublishDate;

                if (post.LastUpdatedDate != DateTime.MinValue)
                    item.LastUpdatedTime = post.LastUpdatedDate;
                else if (post.LastUpdatedDate == DateTime.MinValue && post.PublishDate != DateTime.MinValue)
                    item.LastUpdatedTime = post.PublishDate;

                item.Links.Add(SyndicationLink.CreateSelfLink(_contextWrapper.GetAbsoluteUrl("~/blog/" + post.Slug)));
                item.Authors.Add(new SyndicationPerson { Name = post.Author });

                feedItems.Add(item);
            }
            _feed.Items = feedItems;
        }
    }
}