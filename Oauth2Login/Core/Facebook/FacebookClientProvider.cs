using Oauth2Login.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FacebookOAuthConfigurationElement = Oauth2Login.Configuration.Facebook.OAuthConfigurationElement;
namespace Oauth2Login.Core.Facebook
{
    public class FacebookClientProvider : AbstractClientProvider
    {
        public FacebookClientProvider() : base()
        {

        }

        protected FacebookClientProvider(OAuthWebConfigurationElement ccRoot, FacebookOAuthConfigurationElement ccOauth) : base(ccRoot , ccOauth)
        {
            Fields = ccOauth.Fields;
        }

        public string Fields { get; set; }
    }
}
