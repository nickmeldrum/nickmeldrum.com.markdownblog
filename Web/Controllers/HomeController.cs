using System;
using System.IO;
using System.Web;
using System.Web.Mvc;
using MarkdownBlog.Net.Web.Models;
using Microsoft.WindowsAzure.Storage.Blob;

namespace MarkdownBlog.Net.Web.Controllers
{
    public class HomeController : ControllerBase
    {
        public ActionResult Index()
        {
            return View(PostsMetadata.Instance.Latest(3));
        }

        public ActionResult Sitemap()
        {
            Response.ContentType = "text/xml";
            return View(new Sitemap());
        }

        public ActionResult DoNotTrack()
        {
            Response.Cookies.Add(new HttpCookie("DoNotTrack", "dnt") {Domain = ".nickmeldrum.com", Expires = DateTime.Now.AddYears(10)});

            return View();
        }

        [HttpPost]
        public ActionResult UploadFile(HttpPostedFileBase file) {
            if (file != null && file.ContentLength > 0) {
                try
                {
                    var cloudAccount = Azure.GetStorageAccount();
                    var blobStorage = cloudAccount.CreateCloudBlobClient();
                    var container = blobStorage.GetContainerReference("uploads");
                    container.CreateIfNotExists();

                    var blockBlob = container.GetBlockBlobReference(file.FileName);
                    blockBlob.Properties.ContentType = file.ContentType;
                    blockBlob.UploadFromStream(file.InputStream);

                }
                catch (Exception ex)
                {
                    return Content("<pre>" + ex.Message + "\r\n" + ex.Source + "\r\n" + ex.StackTrace + "</pre>");
                }
            }
            return RedirectToAction("Index");
        }

    }
}
