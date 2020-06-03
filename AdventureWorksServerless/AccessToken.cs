using Microsoft.IdentityModel.Clients.ActiveDirectory;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace AdventureWorksServerless
{
  public static class AccessToken
  {
    public static async Task<string> GetAccessTokenAsync(string clientId, string clientSecret, string authority, string resource, string scope)
    {
      var authContext = new AuthenticationContext(authority, TokenCache.DefaultShared);
      var clientCred = new ClientCredential(clientId, clientSecret);
      var result = await authContext.AcquireTokenAsync(resource, clientCred);

      if (result == null)
      {
        throw new InvalidOperationException("Could not get token");
      }

      return result.AccessToken;
    }
  }
}
