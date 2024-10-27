using Irc.Enumerations;
using Irc.Extensions.Props;
using Irc.Interfaces;
using Irc.Objects;
using Irc.Objects.Server;
using Irc.Resources;

namespace Irc.Props.User;

public class Role : PropRule
{
    private readonly IServer _server;

    public Role(IServer server) : base(IrcStrings.UserPropRole, EnumChannelAccessLevel.None,
        EnumChannelAccessLevel.ChatMember, IrcStrings.GenericProps, "0", true)
    {
        _server = server;
    }

    public override EnumIrcError EvaluateSet(IChatObject source, IChatObject target, string propValue)
    {
        _server.ProcessCookie((IUser)source, IrcStrings.UserPropRole, propValue);
        return EnumIrcError.NONE;
    }
}