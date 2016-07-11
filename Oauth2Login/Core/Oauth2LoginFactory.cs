using System;
using System.Collections;
using System.Configuration;
using Oauth2Login.Configuration;
using FacebookOAuthConfigurationElementCollection = Oauth2Login.Configuration.Facebook.OAuthConfigurationElementCollection;
using FacebookOAuthConfigurationElement = Oauth2Login.Configuration.Facebook.OAuthConfigurationElement;

namespace Oauth2Login.Core
{
    public class Oauth2LoginFactory
    {
        public static T CreateClient<T>(string configName) where T : AbstractClientProvider, new()
        {
            if (String.IsNullOrEmpty(configName))
            {
                throw new Exception("Invalid configuration name");
            }

            configName = configName.ToLower();

            switch (configName)
            {
                case "facebook":
                    return GetDefaultClient<T, OAuthWebConfigurationElement, FacebookOAuthConfigurationElementCollection>(configName);
                default:
                    return GetDefaultClient<T, OAuthWebConfigurationElement, OAuthConfigurationElementCollection>(configName);
            }
        }

        private static string GetSectionName(string configName)
        {
            string baseConfigName = "oauth2.login.configuration";
            switch (configName)
            {
                case "facebook":
                    return string.Format("{0}.{1}", baseConfigName, configName);
                default:
                    return baseConfigName;
            }
        }

        private static T GetDefaultClient<T, WebElement, OAuthElementCollection>(string configName)
            where T : AbstractClientProvider, new()
            where WebElement : new()
            where OAuthElementCollection : ConfigurationElementCollection, new()
        {
            var ccRoot =
                ConfigurationManager.GetSection(GetSectionName(configName)) as IBaseOAuthConfigurationSection<WebElement, OAuthElementCollection>;

            if (ccRoot != null)
            {
                var ccWebElem = ccRoot.WebConfiguration;

                IEnumerator configurationReader = ccRoot.OAuthVClientConfigurations.GetEnumerator();

                OAuthConfigurationElement ccOauth = GetCcOAuth(configName, configurationReader);

                if (ccOauth != null)
                {
                    var constructorParams = new object[]
                    {
                        ccWebElem,
                        ccOauth
                    };
                    var client = (T)Activator.CreateInstance(typeof(T), constructorParams);

                    return client;
                }
                else
                {
                    throw new Exception("ERROR: [MultiOAuthFactroy] ConfigurationName is not found!");
                }

            }

            return default(T);
        }

        private static OAuthConfigurationElement GetCcOAuth(string configName, IEnumerator configurationReader)
        {
            string oAuthProfileName = "";
            if (configName.Equals("facebook", StringComparison.OrdinalIgnoreCase))
            {
                oAuthProfileName = "main";
            }
            else
            {
                //google , WindowsLive , PayPal , Twitter
                oAuthProfileName = configName;
            }


            OAuthConfigurationElement ccOauth = null;
            while (configurationReader.MoveNext())
            {
                var currentOauthElement = configurationReader.Current as OAuthConfigurationElement;
                if (currentOauthElement != null && currentOauthElement.Name == oAuthProfileName)
                {
                    ccOauth = currentOauthElement;
                    break;
                }
            }

            return ccOauth;
        }
    }
}