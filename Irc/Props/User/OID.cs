using Irc.Enumerations;
using Irc.Interfaces;
using Irc.IO;
using Irc.Resources;

namespace Irc.Props.User
{
    internal class OID : PropRule
    {
        public OID() : base(IrcStrings.UserPropOid, EnumChannelAccessLevel.ChatMember,
            EnumChannelAccessLevel.None, IrcStrings.GenericProps, "0", true)
        {
        }
    }
}