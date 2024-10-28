using Irc.Extensions.Props.Channel;
using Irc.Objects.Collections;

namespace Irc.Objects.Channel;

internal class ChannelPropCollection : PropCollection
{
    public ChannelPropCollection()
    {
        AddProp(new OID());
        AddProp(new Name());
        AddProp(new Creation());
        AddProp(new Language());
        AddProp(new Ownerkey());
        AddProp(new Hostkey());
        AddProp(new Memberkey());
        AddProp(new Pics());
        AddProp(new Topic());
        AddProp(new Subject());
        AddProp(new Onjoin());
        AddProp(new Onpart());
        AddProp(new Lag());
        AddProp(new Client());
        AddProp(new ClientGUID());
        AddProp(new ServicePath());

        Properties.Add(Resources.IrcStrings.ChannelPropOID, "");
        Properties.Add(Resources.IrcStrings.ChannelPropName, "");
        Properties.Add(Resources.IrcStrings.ChannelPropCreation, "");
        Properties.Add(Resources.IrcStrings.ChannelPropLanguage, "");
        Properties.Add(Resources.IrcStrings.ChannelPropOwnerkey, "");
        Properties.Add(Resources.IrcStrings.ChannelPropHostkey, "");
        Properties.Add(Resources.IrcStrings.ChannelPropPICS, "");
        Properties.Add(Resources.IrcStrings.ChannelPropTopic, "");
        Properties.Add(Resources.IrcStrings.ChannelPropSubject, "");
        Properties.Add(Resources.IrcStrings.ChannelPropOnJoin, "");
        Properties.Add(Resources.IrcStrings.ChannelPropOnPart, "");
        Properties.Add(Resources.IrcStrings.ChannelPropLag, "");
        Properties.Add(Resources.IrcStrings.ChannelPropClient, "");
        Properties.Add(Resources.IrcStrings.ChannelPropClientGuid, "");
        Properties.Add(Resources.IrcStrings.ChannelPropServicePath, "");
    }
}