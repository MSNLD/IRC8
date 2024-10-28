using Irc.Props;
using Irc.Resources;

namespace Irc.Extensions.Props.Channel;

internal class Name : PropRule
{
    // limited to 200 bytes including 1 or 2 characters for channel prefix
    public Name() : base(IrcStrings.ChannelPropName, EnumChannelAccessLevel.ChatMember,
        EnumChannelAccessLevel.None, IrcStrings.GenericProps, string.Empty, true)
    {
    }
}