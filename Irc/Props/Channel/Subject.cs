using Irc.Resources;

namespace Irc.Extensions.Props.Channel;

internal class Subject : PropRule
{
    public Subject() : base(IrcStrings.ChannelPropSubject, EnumChannelAccessLevel.ChatMember,
        EnumChannelAccessLevel.None, IrcStrings.GenericProps, string.Empty, true)
    {
    }
}