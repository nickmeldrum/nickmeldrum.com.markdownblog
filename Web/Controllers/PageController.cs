﻿using MarkdownBlog.Net.Web.Models;
using System.IO;
using System.Web.Mvc;

namespace MarkdownBlog.Net.Web.Controllers
{
    public class PageController : ControllerBase
    {
        public ActionResult GetPage(string pageName)
        {
            try {
                return string.IsNullOrWhiteSpace(pageName)
                    ? (ActionResult)new RedirectResult("/")
                    : View("Page", new Page(pageName));
            }
            catch (FileNotFoundException ex) {
                HttpContextWrapper.SendHttpStatusResponse(404);
            }

            return new RedirectResult("/");
        }
    }
}
