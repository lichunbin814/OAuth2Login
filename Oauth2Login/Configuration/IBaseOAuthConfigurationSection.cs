using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Oauth2Login.Configuration
{
    public interface IBaseOAuthConfigurationSection<WebElement, OAuthElementCollection>
          where WebElement : new()
          where OAuthElementCollection : ConfigurationElementCollection, new()
    {
        WebElement WebConfiguration
        {
            get;
        }

        OAuthElementCollection OAuthVClientConfigurations
        {
            get;
        }
    }
}
