using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace SCMAPI.Controllers
{
	public class HomeController : Controller
	{
		public ActionResult Index()
		{
			ViewBag.Title = "Home Page";

			return View();
		}
        public ActionResult SCMUI()
        {
            //ViewBag.Title = "Home Page";

            return Redirect("http://vscm-1089815394.ap-south-1.elb.amazonaws.com/");
        }
    }
}
