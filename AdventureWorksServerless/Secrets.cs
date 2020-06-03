using Azure.Identity;
using Azure.Security.KeyVault.Secrets;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace AdventureWorksServerless
{
  public static class Secrets
  {
    public static string GetSecretFromKeyVault(string keyVaultUrl, string secretName)
    {
      var client = new SecretClient(vaultUri: new Uri(keyVaultUrl), credential: new DefaultAzureCredential());
      KeyVaultSecret secret = client.GetSecret(secretName);

      return secret.Value;
    }
  }
}
