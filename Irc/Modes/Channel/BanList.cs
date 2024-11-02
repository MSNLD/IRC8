using Irc.Enumerations;
using Irc.Objects;
using Irc.Resources;

namespace Irc.Modes.Channel;

public class BanList : ModeRuleChannel
{
    public BanList() : base(IrcStrings.ChannelModeBan)
    {
    }

    public new EnumIrcError Evaluate(ChatObject source, ChatObject? target, bool flag, string? parameter)
    {
        //return EvaluateAndSet(source, target, flag, parameter);
        return EnumIrcError.OK;
    }
}