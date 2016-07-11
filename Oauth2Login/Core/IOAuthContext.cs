using System.Collections.Generic;

namespace Oauth2Login.Core
{
    public interface IOAuthContext
    {
        AbstractClientProvider Client { get; set; }
        IClientService<AbstractClientProvider> Service { get; set; }

        string Token { get; set; }
        Dictionary<string, string> Profile { get; set; }
        string BeginAuth();
    }
}