using System.Configuration;

namespace Oauth2Login.Configuration.Facebook
{
    public class OAuthConfigurationSection : ConfigurationSection,
         IBaseOAuthConfigurationSection<OAuthWebConfigurationElement, OAuthConfigurationElementCollection>
    {
        [ConfigurationProperty("web", IsRequired = false)]
        public OAuthWebConfigurationElement WebConfiguration
        {
            get { return base["web"] as OAuthWebConfigurationElement; }
        }

        [ConfigurationProperty("oauth", IsKey = false, IsRequired = true)]
        [ConfigurationCollection(typeof(OAuthConfigurationElementCollection),
            CollectionType = ConfigurationElementCollectionType.AddRemoveClearMap)]
        public virtual OAuthConfigurationElementCollection OAuthVClientConfigurations
        {
            get { return base["oauth"] as OAuthConfigurationElementCollection; }
        }
    }
}