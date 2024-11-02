using Irc.Objects;

namespace Irc.Extensions.Apollo.Directory;

public static class ApolloDirectoryRaws
{
    public static string RPL_FINDS_MSN(DirectoryServer server, User? user)
    {
        return $":{server} 613 {user} :{server.ChatServerIp} {server.ChatServerPort}";
    }
}