using Irc.Resources;

namespace Irc.Extensions.Props.Channel;

internal class ClientGUID : PropRule
{
    // The CLIENTGUID channel property contains a GUID that defines the client protocol to be used within the channel.
    // This property may be set and read like the LAG property. 
    public ClientGUID() : base(IrcStrings.ChannelPropClientGuid, EnumChannelAccessLevel.None,
        EnumChannelAccessLevel.None, IrcStrings.GenericProps, string.Empty, true)
    {
    }
}