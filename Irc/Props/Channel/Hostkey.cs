using Irc.Enumerations;
using Irc.Resources;

namespace Irc.Props.Channel
{
    internal class Hostkey : PropRule
    {
        // The HOSTKEY channel property is the host keyword that will provide host (channel op) access when entering the channel. 
        // It may never be read.
        public Hostkey() : base(IrcStrings.ChannelPropHostkey, EnumChannelAccessLevel.None,
            EnumChannelAccessLevel.ChatOwner, IrcStrings.GenericProps, string.Empty)
        {
        }
    }
}