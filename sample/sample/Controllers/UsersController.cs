using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using TwitterAuth;

namespace sample.Controllers
{
    public class UsersController : TwitterController
    {
        //
        // GET: /Users/

        public override string ConsumerKey
        {
            get { return "ConsumerKey Goes Here"; }
        }

        public override string ConsumerSecret
        {
            get { return "ConsumerSecret Goes Here"; }
        }

        public ActionResult Index()
        {
            return View();
        }


        public ActionResult Success()
        {
            var twitterResponse = (ITwitterResponse)TempData["twitterResponse"];
            return Redirect("~/");
        }

        /// <summary>
        /// This action will be called in the event of an auth failure.
        /// </summary>
        /// <returns></returns>
        public ActionResult Fail()
        {
            return View();
        }

    }
}
