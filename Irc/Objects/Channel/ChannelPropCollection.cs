using Irc.Objects.Collections;
using Irc.Props;
using Irc.Props.Channel;

namespace Irc.Objects.Channel
{
    internal class ChannelPropCollection : PropCollection
    {
        public static Dictionary<string, PropRule> PropRules = new()
        {
            { Resources.IrcStrings.ChannelPropOID, new OID() },
            { Resources.IrcStrings.ChannelPropLanguage, new Creation() },
            { Resources.IrcStrings.ChannelPropOwnerkey, new Language() },
            { Resources.IrcStrings.ChannelPropHostkey, new OID() },
            { Resources.IrcStrings.ChannelPropPICS, new OID() },
            { Resources.IrcStrings.ChannelPropTopic, new OID() },
            { Resources.IrcStrings.ChannelPropSubject, new OID() },
            { Resources.IrcStrings.ChannelPropOnJoin, new OID() },
            { Resources.IrcStrings.ChannelPropOnPart, new OID() },
            { Resources.IrcStrings.ChannelPropLag, new OID() },
            { Resources.IrcStrings.ChannelPropClient, new OID() },
            { Resources.IrcStrings.ChannelPropClientGuid, new OID() },
            { Resources.IrcStrings.ChannelPropServicePath, new OID() }
        };
    }
}