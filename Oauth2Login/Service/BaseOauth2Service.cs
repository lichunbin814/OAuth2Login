using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Text;
using System.Web;
using Newtonsoft.Json;
using Oauth2Login.Client;
using Oauth2Login.Core;

namespace Oauth2Login.Service
{
    public abstract class BaseOauth2Service : BaseOauth2Service<AbstractClientProvider>
    {
        public BaseOauth2Service(AbstractClientProvider oClient) : base(oClient)
        {

        }
    }

    public abstract class BaseOauth2Service<TClientProvider> : IClientService<TClientProvider>
        where TClientProvider : AbstractClientProvider
    {
        public TClientProvider Client { get; set; }

        protected BaseOauth2Service(TClientProvider oClient)
        {
            Client = oClient;
        }

        public void CreateOAuthClient(IOAuthContext oContext)
        {
            Client = oContext.Client as TClientProvider;
        }

        public void CreateOAuthClient(TClientProvider oClient)
        {
            Client = oClient;
        }

        protected string HttpGet(string url)
        {
            var header = new NameValueCollection
            {
                {"Accept-Language", "en-US"}
            };

            return RestfullRequest.Request(url, "GET", "application/x-www-form-urlencoded", header, null, Client.Proxy);
        }

        protected string HttpPost(string urlToPost, string postData)
        {
            var result = RestfullRequest.Request(urlToPost, "POST", "application/x-www-form-urlencoded",
                    null, postData, Client.Proxy);

            return result;
        }

        // oh you abstract base class, leave something for children to implement
        public abstract string BeginAuthentication();
        public abstract string RequestToken(HttpRequestBase request);
        public abstract void RequestUserProfile();

        // TODO: This looks horrible, refactor using generics
        public static IBaseOAuth2Service GetService(string id)
        {
            switch (id.ToLower())
            {
                case "google":
                    return new GoogleService(Oauth2LoginFactory.CreateClient<GoogleClient>("Google"));
                case "facebook":
                    return new FacebookService(Oauth2LoginFactory.CreateClient<FacebookClient>("Facebook"));
                // Need to transition WindowLive to new base class
                //case "windowslive":
                //    return new WindowsLiveService(Oauth2LoginFactory.CreateClient<WindowsLiveClient>("WindowsLive"));
                //    break;
                case "paypal":
                    return new PayPalService(Oauth2LoginFactory.CreateClient<PayPalClient>("PayPal"));
                case "twitter":
                    return new TwitterService(Oauth2LoginFactory.CreateClient<TwitterClient>("Twitter"));
                default:
                    return null;
            }
        }


        public string ValidateLogin(HttpRequestBase request)
        {
            // client token
            string tokenResult = RequestToken(request);
            if (tokenResult == OAuth2Consts.ACCESS_DENIED)
                return Client.FailedRedirectUrl;

            Client.Token = tokenResult;

            // client profile
            RequestUserProfile();

            UserData.OAuthToken = Client.Token;
            UserData.OAuthTokenSecret = Client.TokenSecret;

            return null;
        }

        public void ImpersonateUser(string oauthToken, string oauthTokenSecret)
        {
            Client.Token = oauthToken;
            Client.TokenSecret = oauthTokenSecret;
        }

        protected void ParseUserData<TData>(string json) where TData : BaseUserData
        {
            UserDataJsonSource = json;
            UserData = ParseJson<TData>(json);
        }

        protected T ParseJson<T>(string json)
        {
            return JsonConvert.DeserializeAnonymousType(json, (T)Activator.CreateInstance(typeof(T)));
        }

        public BaseUserData UserData { get; set; }
        public string UserDataJsonSource { get; set; }
    }
}
