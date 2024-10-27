using Irc.Enumerations;
using Irc.Interfaces;
using Irc.Resources;

namespace Irc.Extensions.Props.Channel;

public class Topic : PropRule
{
    public Topic() : base(IrcStrings.ChannelPropTopic, EnumChannelAccessLevel.ChatMember,
        EnumChannelAccessLevel.ChatHost, IrcStrings.ChannelPropTopicRegex, string.Empty)
    {
    }

    public override string GetValue(IChatObject target)
    {
        return ((IChannel)target).ChannelStore.Get("topic");
    }

    public override EnumIrcError EvaluateSet(IChatObject source, IChatObject target, string propValue)
    {
        var result = base.EvaluateSet(source, target, propValue);
        if (result != EnumIrcError.OK) return result;

        var channel = (IChannel)target;
        channel.ChannelStore.Set("topic", propValue);
        return result;
    }
}