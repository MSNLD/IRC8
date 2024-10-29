using Irc.Enumerations;
using Irc.Resources;

namespace Irc.Props.Channel
{
    internal class Subject : PropRule
    {
        public Subject() : base(IrcStrings.ChannelPropSubject, EnumChannelAccessLevel.ChatMember,
            EnumChannelAccessLevel.None, IrcStrings.GenericProps, string.Empty, true)
        {
        }
    }
}