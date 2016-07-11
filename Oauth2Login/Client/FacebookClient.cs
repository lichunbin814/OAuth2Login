using Oauth2Login.Configuration;
using Oauth2Login.Core;
using Oauth2Login.Core.Facebook;
using Oauth2Login.Service;
using FacebookOAuthConfigurationElement = Oauth2Login.Configuration.Facebook.OAuthConfigurationElement;
namespace Oauth2Login.Client
{
    public class FacebookClient : FacebookClientProvider
    {
        public FacebookClient()
        {
        }

        public FacebookClient(OAuthWebConfigurationElement ccRoot, FacebookOAuthConfigurationElement ccOauth)
            : base(ccRoot, ccOauth)
        {
            
        }

         
    }
}