using Irc.Enumerations;
using Irc.Extensions.Props;
using Irc.Interfaces;
using Irc.Objects;
using Irc.Objects.Server;
using Irc.Resources;

namespace Irc.Props.User;

public class SubscriberInfo : PropRule
{
    private readonly IServer _server;

    public SubscriberInfo(IServer apolloServer) : base(IrcStrings.UserPropSubscriberInfo,
        EnumChannelAccessLevel.None, EnumChannelAccessLevel.ChatMember, IrcStrings.GenericProps, "0", true)
    {
        _server = apolloServer;
    }

    public override EnumIrcError EvaluateSet(IChatObject source, IChatObject target, string propValue)
    {
        _server.ProcessCookie((IUser)source, IrcStrings.UserPropSubscriberInfo, propValue);
        return EnumIrcError.NONE;
    }
}
