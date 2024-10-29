using Irc.Interfaces;
using Irc.Objects;

namespace Irc.Extensions.Apollo.Directory
{
    public static class ApolloDirectoryRaws
    {
        public static string RPL_FINDS_MSN(DirectoryServer server, IUser user)
        {
            return $":{server} 613 {user} :{server.ChatServerIP} {server.ChatServerPORT}";
        }
    }
}