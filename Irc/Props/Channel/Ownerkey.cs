using Irc.Enumerations;
using Irc.Resources;

namespace Irc.Props.Channel
{
    internal class Ownerkey : PropRule
    {
        // The OWNERKEY channel property is the owner keyword that will provide owner access when entering the channel. The OWNERKEY property is limited to 31 characters. 
        // It may never be read

        public Ownerkey() : base(IrcStrings.ChannelPropOwnerkey, EnumChannelAccessLevel.None,
            EnumChannelAccessLevel.ChatOwner, IrcStrings.GenericProps, string.Empty)
        {
        }
    }
}