using Irc.Props;
using Irc.Resources;

namespace Irc.Extensions.Props.Channel;

internal class ServicePath : PropRule
{
    public ServicePath() : base(IrcStrings.ChannelPropServicePath, EnumChannelAccessLevel.None,
        EnumChannelAccessLevel.ChatOwner, IrcStrings.GenericProps, string.Empty, true)
    {
    }
}