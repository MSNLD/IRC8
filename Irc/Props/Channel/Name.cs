using Irc.Enumerations;
using Irc.Resources;

namespace Irc.Props.Channel
{
    internal class Name : PropRule
    {
        // limited to 200 bytes including 1 or 2 characters for channel prefix
        public Name() : base(IrcStrings.ChannelPropName, EnumChannelAccessLevel.ChatMember,
            EnumChannelAccessLevel.None, IrcStrings.GenericProps, string.Empty, true)
        {
        }
    }
}