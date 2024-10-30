using Irc.Commands;
using Irc.Enumerations;
using Irc.Extensions.Apollo.Directory.Commands;
using Irc.Interfaces;
using Irc.Objects.Server;
using Nick = Irc.Extensions.Apollo.Directory.Commands.Nick;
using Version = Irc.Commands.Version;

namespace Irc.Extensions.Apollo.Directory;

public class DirectoryServer : Server
{
    public string ChatServerIP;
    public string ChatServerPORT;

    public DirectoryServer(ISocketServer socketServer,
        ISecurityManager securityManager,
        IFloodProtectionManager floodProtectionManager,
        IDataStore dataStore,
        IList<IChannel> channels,
        ICredentialProvider? ntlmCredentialProvider = null)
        : base(socketServer,
            securityManager,
            floodProtectionManager,
            dataStore,
            channels,
            ntlmCredentialProvider)
    {
        DisableGuestMode = true;
        DisableUserRegistration = true;
        FlushCommands();
        AddCommand(new Ircvers());
        AddCommand(new Auth());
        AddCommand(new Pass());
        AddCommand(new Nick());
        AddCommand(new UserCommand(), EnumProtocolType.IRC, "User");
        AddCommand(new Finds());
        AddCommand(new Prop());
        AddCommand(new Create());
        AddCommand(new Ping());
        AddCommand(new Pong());
        AddCommand(new Version());
        AddCommand(new WebIrc());
    }
}