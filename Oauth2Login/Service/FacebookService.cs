using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Runtime.Serialization;
using System.Web;
using Newtonsoft.Json;
using Oauth2Login.Client;
using Oauth2Login.Core;
using Oauth2Login.Core.Facebook;

namespace Oauth2Login.Service
{
    public interface IBaseOAuth2Service
    {
        string BeginAuthentication();

        string ValidateLogin(HttpRequestBase request);
    }

    public class FacebookService : BaseOauth2Service<FacebookClientProvider>, IBaseOAuth2Service
    {
        private static string _oauthUrl = "";



        public FacebookService(FacebookClientProvider oClient) : base(oClient)
        {

        }

        public override string BeginAuthentication()
        {
            var qstring = QueryStringBuilder.Build(
                "client_id", Client.ClientId,
                "redirect_uri", Client.CallBackUrl,
                "scope", Client.Scope,
                "state", "",
                "display", "popup"
                );

            _oauthUrl = "https://www.facebook.com/v2.6/dialog/oauth?" + qstring;

            return _oauthUrl;
        }

        public override string RequestToken(HttpRequestBase request)
        {
            var code = request.Params["code"];
            if (String.IsNullOrEmpty(code))
                return OAuth2Consts.ACCESS_DENIED; 

            string tokenUrl = string.Format("https://graph.facebook.com/oauth/access_token?");
            string postData = QueryStringBuilder.Build(
                "client_id", Client.ClientId,
                "redirect_uri", Client.CallBackUrl,
                "client_secret", Client.ClientSecret,
                "code", code
            );

            string resonseJson = HttpPost(tokenUrl, postData);
            resonseJson = "{\"" + resonseJson.Replace("=", "\":\"").Replace("&", "\",\"") + "\"}";
            return JsonConvert.DeserializeAnonymousType(resonseJson, new { access_token = "" }).access_token;
        }

        public override void RequestUserProfile()
        {


            string profileUrl = "https://graph.facebook.com/me?access_token=" + Client.Token + GetFields(Client.Fields);

            string result = HttpGet(profileUrl);

            ParseUserData<FacebookUserData>(result);
        }

        private string GetFields(string fields)
        {
            if (!string.IsNullOrWhiteSpace(fields))
            {
                return string.Format("&fields=" + fields);
            }
            else
            {
                return "";
            }
        }
    }

    public class FacebookUserData : BaseUserData
    {
        public FacebookUserData() : base(ExternalAuthServices.Facebook) { }

        public string id { get; set; }
        public string email { get; set; }
        public string name { get; set; }
        public string first_name { get; set; }
        public string last_name { get; set; }
        public string link { get; set; }
        public string gender { get; set; }
        public string picture { get; set; }
        public string locale { get; set; }
        public int timezone { get; set; }
        public bool verified { get; set; }

        // override
        public override string UserId { get { return id; } }
        public override string Email { get { return email; } }
        public override string FullName { get { return name; } }

        public override string PhoneNumber { get { return null; } }
    }
}