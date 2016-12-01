using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace RoseBud.Controllers
{
    public class MovieListsController : Controller
    {
        // GET: MovieList
        public ActionResult Index()
        {
            return View();
        }
    }
}