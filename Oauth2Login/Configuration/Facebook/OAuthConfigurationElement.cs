using System.Configuration;

namespace Oauth2Login.Configuration.Facebook
{
    public class OAuthConfigurationElement : Configuration.OAuthConfigurationElement
    {
        [ConfigurationProperty("fields", IsRequired = false)]
        public string Fields
        {
            get { return base["fields"].ToString(); }
        }
    }
}