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

                CreateAuthCookie(response.ScreenName, response.Token);

                TempData["twitterResponse"] = twitterResponse;
                return RedirectToAction("Success");
            }
            return RedirectToAction("Fail", response);
        }

        private static void CreateAuthCookie(string username, string token)
        {
            //Get ASP.NET to create a forms authentication cookie (based on settings in web.config)~
            HttpCookie cookie = FormsAuthentication.GetAuthCookie(username, false);

            //Decrypt the cookie
            FormsAuthenticationTicket ticket = FormsAuthentication.Decrypt(cookie.Value);

            //Create a new ticket using the details from the generated cookie, but store the username & token passed in from the authentication method
            FormsAuthenticationTicket newticket = new FormsAuthenticationTicket(ticket.Version, ticket.Name, ticket.IssueDate, ticket.Expiration,ticket.IsPersistent, token);

            // Encrypt the ticket & store in the cookie
            cookie.Value = FormsAuthentication.Encrypt(newticket);

            // Update the outgoing cookies collection.
            System.Web.HttpContext.Current.Response.Cookies.Set(cookie);
        }
    }
}