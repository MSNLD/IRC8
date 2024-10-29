using Irc.Enumerations;
using Irc.Resources;

namespace Irc.Props.Channel
{
    internal class ClientGUID : PropRule
    {
        // The CLIENTGUID channel property contains a GUID that defines the client protocol to be used within the channel.
        // This property may be set and read like the LAG property. 
        public ClientGUID() : base(IrcStrings.ChannelPropClientGuid, EnumChannelAccessLevel.None,
            EnumChannelAccessLevel.None, IrcStrings.GenericProps, string.Empty, true)
        {
        }
    }
}