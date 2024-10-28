using Irc.Props;
using Irc.Resources;

namespace Irc.Extensions.Props.Channel;

internal class OID : PropRule
{
    public OID() : base(IrcStrings.ChannelPropOID, EnumChannelAccessLevel.ChatMember,
        EnumChannelAccessLevel.None, IrcStrings.ChannelPropOIDRegex, "0", true)
    {
    }
}