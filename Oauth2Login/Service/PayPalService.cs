﻿using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Runtime.Serialization;
using System.Web;
using Newtonsoft.Json;
using Oauth2Login.Client;
using Oauth2Login.Core;

namespace Oauth2Login.Service
{
    public class PayPalService : BaseOauth2Service<AbstractClientProvider> , IBaseOAuth2Service
    {
        private static string _oauthUrl = "";

        private string ApiUrlOauth
        {
            get
            {
                return Client.Endpoint == OAuth2Consts.SANDBOX
                    ? "https://api.sandbox.paypal.com"
                    : "https://api.paypal.com";
            }
        }

        private string LoginUrlOauth
        {
            get
            {
                return Client.Endpoint == OAuth2Consts.SANDBOX
                    ? "https://www.sandbox.paypal.com"
                    : "https://www.paypal.com";
            }
        }

        public PayPalService(AbstractClientProvider oClient)
            : base(oClient)
        {
        }

        public override string BeginAuthentication()
        {
            var qstring = QueryStringBuilder.Build(
                "client_id", Client.ClientId,
                "response_type", "code",
                "redirect_uri", Client.CallBackUrl,
                "scope", Client.Scope
                );

            _oauthUrl = LoginUrlOauth + "/webapps/auth/protocol/openidconnect/v1/authorize?" + qstring;

            return _oauthUrl;
        }

        public override string RequestToken(HttpRequestBase request)
        {
            var code = request.Params["code"];
            if (String.IsNullOrEmpty(code))
                return OAuth2Consts.ACCESS_DENIED;

            var oauthUrl = ApiUrlOauth + "/v1/identity/openidconnect/tokenservice";
            var postData = QueryStringBuilder.Build(
                "grant_type", "authorization_code",
                "redirect_uri", Client.CallBackUrl,
                "code", code,
                "client_id", Client.ClientId,
                "client_secret", Client.ClientSecret
                );
            var responseJson = HttpPost(oauthUrl, postData);
            return JsonConvert.DeserializeAnonymousType(responseJson, new { access_token = "" }).access_token;
        }

        public override void RequestUserProfile()
        {
            var profileUrl = ApiUrlOauth + "/v1/identity/openidconnect/userinfo/?schema=openid";

            var header = new NameValueCollection
            {
                {"Accept-Language", "en_US"},
                {"Authorization", "Bearer " + Client.Token}
            };
            var result = RestfullRequest.Request(profileUrl, "POST", "application/json", header, null, Client.Proxy);

            ParseUserData<PayPalUserData>(result);
        }
    }

    public class PayPalUserData : BaseUserData
    {
        public PayPalUserData() : base(ExternalAuthServices.PayPal) { }

        public string user_id { get; set; }
        public string email { get; set; }
        public string phone_number { get; set; }
        public string name { get; set; }
        public Address address { get; set; }
        public string language { get; set; }
        public string locale { get; set; }
        public string zoneinfo { get; set; }
        public DateTime birthday { get; set; }
        public string given_name { get; set; }
        public string family_name { get; set; }
        public string verified_email { get; set; }
        public string gender { get; set; }
        public string picture { get; set; }

        public class Address
        {
            public int postal_code { get; set; }
            public string locality { get; set; }
            public string region { get; set; }
            public string country { get; set; }
            public string street_address { get; set; }
        }

        // override
        public override string UserId { get { return user_id; } }
        public override string Email { get { return email; } }
        public override string PhoneNumber { get { return phone_number; } }
        public override string FullName { get { return name; } }
    }
}