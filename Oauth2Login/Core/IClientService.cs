using System.Collections.Generic;

namespace Oauth2Login.Core
{
    public interface IClientService<ClientProvider>
        where ClientProvider : AbstractClientProvider
    {
        void CreateOAuthClient(IOAuthContext oContext);
        void CreateOAuthClient(ClientProvider oClient);
    }
}