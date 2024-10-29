using Irc.Enumerations;
using Irc.Resources;

namespace Irc.Props.Channel
{
    internal class Creation : PropRule
    {
        // The CREATION channel property is the time that the channel was created, in number of seconds elapsed since midnight (00:00:00), January 1, 1970, (coordinated universal time)
        public Creation() : base(IrcStrings.ChannelPropCreation, EnumChannelAccessLevel.ChatMember,
            EnumChannelAccessLevel.None, IrcStrings.GenericProps, IrcStrings.GetEpochNowInSeconds().ToString(), true)
        {
        }
    }
}