using Irc.Enumerations;
using Irc.Objects;
using Irc.Resources;

namespace Irc.Modes.Channel;

public class Service : ModeRuleChannel
{
    public Service() : base(IrcStrings.ChannelModeService)
    {
    }

    public new EnumIrcError Evaluate(ChatObject source, ChatObject? target, bool flag, string? parameter)
    {
        return EvaluateAndSet(source, target, flag, parameter);
    }
}