using MarkdownBlog.Net.Web.Models;
using System.Web;
using System.Web.Mvc;

namespace MarkdownBlog.Net.Web.Areas.Admin.Controllers {
    [Authorize(Roles = "Admin")]
    public class UploadController : Controller {
        [HttpPost]
        public ActionResult UploadFile(HttpPostedFileBase file) {
            if (file != null && file.ContentLength > 0) {
                var cloudAccount = Azure.GetStorageAccount();
                var blobStorage = cloudAccount.CreateCloudBlobClient();
                var container = blobStorage.GetContainerReference("uploads");
                container.CreateIfNotExists();

                var blockBlob = container.GetBlockBlobReference(file.FileName);
                blockBlob.Properties.ContentType = file.ContentType;
                blockBlob.UploadFromStream(file.InputStream);
            }

            return RedirectToAction("Index", "Home");
        }
    }
}
