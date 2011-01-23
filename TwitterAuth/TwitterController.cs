using System;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;
using Twitterizer;
using System;
using System.Collections.Generic;
using System.Web;
using System.Web.Security;

namespace TwitterAuth {

    public abstract class TwitterController : Controller {

        public abstract string ConsumerKey { get; }

        public abstract string ConsumerSecret { get; }

        public abstract ActionResult Success();

        public abstract ActionResult Fail();

        public TwitterController() {
            if (string.IsNullOrEmpty(ConsumerKey)) {
                throw new ArgumentException("ConsumerKey is null or empty");
            }

            if (string.IsNullOrEmpty(ConsumerSecret)) {
                throw new ArgumentException("ConsumerSecret is null or empty");
            }
        }

        public void Logon() {
            HttpRequest request = System.Web.HttpContext.Current.Request;
            var callBackUrl = new Uri(request.Url.Scheme + "://" + request.Url.Authority + "/Users/Callback");
            OAuthTokenResponse oAuthTokenResponse = OAuthUtility.GetRequestToken(ConsumerKey, ConsumerSecret, callBackUrl.ToString());
            var uri = OAuthUtility.BuildAuthorizationUri(oAuthTokenResponse.Token, true);
            Response.Redirect(uri.ToString());
        }

        public ActionResult Logoff() {
            var formsAuthenticationTicket = new FormsAuthenticationTicket(1, "", DateTime.Now, DateTime.Now.AddMinutes(-30), false, "", FormsAuthentication.FormsCookiePath);
            Response.Cookies.Add(new HttpCookie(FormsAuthentication.FormsCookieName, FormsAuthentication.Encrypt(formsAuthenticationTicket)));
            return Redirect("~/");
        }

        public ActionResult Callback() {
            OAuthTokenResponse response = null;
            if(Request.QueryString["oauth_token"] == null)
            {
                //user is not auth. not sure if this will happen.
            } 
            else
            {
                //user is auth. now need to check if we have access?
                string oauthToken = Request.QueryString["oauth_token"].ToString();
                response = OAuthUtility.GetAccessToken(ConsumerKey, ConsumerSecret, oauthToken, "");
            }


           
            if (response != null) {
                var twitterResponse = new TwitterResponse
                                          {
                                              AccessSecret = response.TokenSecret,
                                              AccessToken = response.Token,
                                              Logon = response.ScreenName, //use consistent verbiage between username/logon/screen name.
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