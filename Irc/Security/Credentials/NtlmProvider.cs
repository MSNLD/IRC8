using Irc.Interfaces;

namespace Irc.Security.Credentials;

public class NtlmProvider : ICredentialProvider
{
    public Credential? ValidateTokens(Dictionary<string, string> tokens)
    {
        throw new NotImplementedException();
    }

    public Credential GetUserCredentials(string domain, string username)
    {
        throw new NotImplementedException();
    }
}