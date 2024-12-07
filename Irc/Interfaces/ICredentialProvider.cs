using Irc.Security;

namespace Irc.Interfaces;

public interface ICredentialProvider
{
    Credential? ValidateTokens(Dictionary<string, string> tokens);
    Credential GetUserCredentials(string domain, string username);
}