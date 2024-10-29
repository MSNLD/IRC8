using Irc.Enumerations;
using Irc.Resources;

namespace Irc.Props.Channel
{
    internal class Language : PropRule
    {
        // The LANGUAGE channel property is the preferred language type. The LANGUAGE property is a string limited to 31 characters. 
        public Language() : base(IrcStrings.ChannelPropLanguage, EnumChannelAccessLevel.ChatMember,
            EnumChannelAccessLevel.ChatHost, IrcStrings.GenericProps, string.Empty)
        {
        }
    }
}