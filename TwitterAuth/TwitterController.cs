using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;
using DotNetOpenAuth.OAuth;
using DotNetOpenAuth.OAuth.Messages;

namespace TwitterAuth {

    public abstract class TwitterController : Controller {

        public abstract string ConsumerKey { get; }

        public abstract string ConsumerSecret { get; }

        private readonly InMemoryTokenManager _tokenManager;

        public TwitterController() {
            if (string.IsNullOrEmpty(ConsumerKey)) {
                throw new ArgumentException("ConsumerKey is null or empty");
            }

            if (string.IsNullOrEmpty(ConsumerSecret)) {
                throw new ArgumentException("ConsumerSecret is null or empty");
            }

            //Contract.Requires(string.IsNullOrEmpty(ConsumerKey)==false, "ConsumerKey Must Not Be Null");}
            //Contract.Requires<ArgumentNullException>(ConsumerKey != null);

            _tokenManager = TokenManagerManager.Create(ConsumerKey, ConsumerSecret);
        }

        public void Index() {
            Response.Write("tester;");
        }

        public void Logon() {
            var client = new TwitterClient(_tokenManager);
            client.StartAuthorization();
            //return null;
        }

        public ActionResult Logoff() {
            var formsAuthenticationTicket = new FormsAuthenticationTicket(1, "", DateTime.Now, DateTime.Now.AddMinutes(-30), false, "", FormsAuthentication.FormsCookiePath);
            Response.Cookies.Add(new HttpCookie(FormsAuthentication.FormsCookieName, FormsAuthentication.Encrypt(formsAuthenticationTicket)));
            return Redirect("~/");
        }

        public ActionResult Callback() {
            var client = new TwitterClient(_tokenManager);
            var response = client.FinishAuthorization();
            if (response != null) {
                var twitterResponse = new TwitterResponse
                                          {
                                              AccessSecret = response.TokenSecret,
                                              AccessToken = response.Token,
                                              Logon = ((AuthorizedTokenResponse)response).ExtraData["screen_name"],
                                          };

                TempData["twitterResponse"] = twitterResponse;
                return RedirectToAction("Success");
            } else {
                return RedirectToAction("Fail", response);
            }

            // show error
            //return View("LogOn");
        }
    }
}