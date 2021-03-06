﻿using MarkdownBlog.Net.Web.Models;
using System;
using System.Web;
using System.Web.Mvc;

namespace MarkdownBlog.Net.Web.Controllers
{
    public class HomeController : ControllerBase
    {
        public ActionResult Index()
        {
            return View(PostsMetadata.Instance.Latest(5));
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


    }
}
