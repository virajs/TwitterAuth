using System;
using System.Collections.Generic;
using System.Web;
using System.Web.Security;
using DotNetOpenAuth.Messaging;
using DotNetOpenAuth.OAuth;
using DotNetOpenAuth.OAuth.ChannelElements;
using DotNetOpenAuth.OAuth.Messages;

namespace TwitterAuth {

    public class TwitterClient {
        private static readonly ServiceProviderDescription ServiceDescription =
            new ServiceProviderDescription
                {
                    RequestTokenEndpoint = new MessageReceivingEndpoint(
                        "https://api.twitter.com/oauth/request_token",
                        HttpDeliveryMethods.GetRequest |
                        HttpDeliveryMethods.AuthorizationHeaderRequest),
                    UserAuthorizationEndpoint = new MessageReceivingEndpoint(
                        "https://api.twitter.com/oauth/authorize",
                        HttpDeliveryMethods.GetRequest |
                        HttpDeliveryMethods.AuthorizationHeaderRequest),
                    AccessTokenEndpoint = new MessageReceivingEndpoint(
                        "https://api.twitter.com/oauth/access_token",
                        HttpDeliveryMethods.GetRequest |
                        HttpDeliveryMethods.AuthorizationHeaderRequest),
                    TamperProtectionElements =
                        new ITamperProtectionChannelBindingElement[] { new HmacSha1SigningBindingElement() },
                };

        private readonly IConsumerTokenManager _tokenManager;

        public TwitterClient(IConsumerTokenManager tokenManager) {
            _tokenManager = tokenManager;
        }

        private string UserName { get; set; }

        public void StartAuthorization() {
            HttpRequest request = HttpContext.Current.Request;
            using (var twitter = new WebConsumer(ServiceDescription, _tokenManager))
            {
                Uri callBackUrl;
                callBackUrl = new Uri(request.Url.Scheme + "://" + request.Url.Authority + "/Users/Callback");    
                
                twitter.Channel.Send(twitter.PrepareRequestUserAuthorization(callBackUrl, null, null));
            }
        }

        public ITokenSecretContainingMessage FinishAuthorization() {
            using (var twitter = new WebConsumer(ServiceDescription, _tokenManager)) {
                AuthorizedTokenResponse accessTokenResponse = twitter.ProcessUserAuthorization();
                var response = accessTokenResponse as ITokenSecretContainingMessage;
                if (accessTokenResponse != null) {
                    UserName = accessTokenResponse.ExtraData["screen_name"];
                    CreateAuthCookie(UserName, accessTokenResponse.AccessToken);
                    return response;
                }
            }

            return null;
        }

        private static void CreateAuthCookie(string username, string token) {
            //Get ASP.NET to create a forms authentication cookie (based on settings in web.config)~
            HttpCookie cookie = FormsAuthentication.GetAuthCookie(username, false);

            //Decrypt the cookie
            FormsAuthenticationTicket ticket = FormsAuthentication.Decrypt(cookie.Value);

            //Create a new ticket using the details from the generated cookie, but store the username &
            //token passed in from the authentication method
            FormsAuthenticationTicket newticket = new FormsAuthenticationTicket(
                ticket.Version, ticket.Name, ticket.IssueDate, ticket.Expiration,
            ticket.IsPersistent, token);

            // Encrypt the ticket & store in the cookie
            cookie.Value = FormsAuthentication.Encrypt(newticket);

            // Update the outgoing cookies collection.
            HttpContext.Current.Response.Cookies.Set(cookie);
        }
    }
}