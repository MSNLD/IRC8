using Irc.Enumerations;
using Irc.Resources;

namespace Irc.Props.Channel
{
    internal class OID : PropRule
    {
        public OID() : base(IrcStrings.ChannelPropOID, EnumChannelAccessLevel.ChatMember,
            EnumChannelAccessLevel.None, IrcStrings.ChannelPropOIDRegex, "0", true)
        {
        }
    }
}