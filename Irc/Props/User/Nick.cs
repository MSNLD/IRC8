using Irc.Enumerations;
using Irc.Interfaces;
using Irc.IO;
using Irc.Resources;

namespace Irc.Props.User
{
    internal class Nick : PropRule, IPropRule
    {
        // limited to 200 bytes including 1 or 2 characters for channel prefix
        public Nick() : base(IrcStrings.UserPropNickname, EnumChannelAccessLevel.ChatMember,
            EnumChannelAccessLevel.None, IrcStrings.GenericProps, string.Empty, true)
        {
        }
    }
}