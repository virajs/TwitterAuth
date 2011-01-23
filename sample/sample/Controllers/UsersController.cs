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
            get { return "bcTjf9zofNIpfPMgSayKw"; }
        }

        public override string ConsumerSecret
        {
            get { return "mDFOdhXMHAQkGGTWm6TKrJL0AukKqGVy084sb44"; }
        }


        public ActionResult Index()
        {
            var twitterResponse = (ITwitterResponse)TempData["twitterResponse"];
            return View();
        }


        public override ActionResult Success()
        {
            var twitterResponse = (ITwitterResponse)TempData["twitterResponse"];
            return Redirect("~/");
        }

        /// <summary>
        /// This action will be called in the event of an auth failure.
        /// </summary>
        /// <returns></returns>
        public override ActionResult Fail()
        {
            return View();
        }

    }
}
