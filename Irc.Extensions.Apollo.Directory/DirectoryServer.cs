using Irc.Commands;
using Irc.Enumerations;
using Irc.Extensions.Apollo.Directory.Commands;
using Irc.Interfaces;
using Irc.IO;
using Irc.Objects;
using Irc.Security;
using Nick = Irc.Extensions.Apollo.Directory.Commands.Nick;
using User = Irc.Commands.User;
using Version = Irc.Commands.Version;

namespace Irc.Extensions.Apollo.Directory;

public class DirectoryServer : Server
{
    public string? ChatServerIp;
    public string? ChatServerPort;

    public DirectoryServer(ISocketServer socketServer,
        SecurityManager securityManager,
        Settings serverSettings,
        IList<Channel?> channels,
        ICredentialProvider? ntlmCredentialProvider = null)
        : base(socketServer,
            securityManager,
            serverSettings,
            channels,
            ntlmCredentialProvider)
    {
        DisableGuestMode = true;
        DisableUserRegistration = true;
        FlushCommands();
        Protocol.AddCommand(new Ircvers());
        Protocol.AddCommand(new Auth());
        Protocol.AddCommand(new Pass());
        Protocol.AddCommand(new Nick());
        Protocol.AddCommand(new User());
        Protocol.AddCommand(new Finds());
        Protocol.AddCommand(new Prop());
        Protocol.AddCommand(new Create());
        Protocol.AddCommand(new Ping());
        Protocol.AddCommand(new Pong());
        Protocol.AddCommand(new Version());
        Protocol.AddCommand(new WebIrc());
    }
}