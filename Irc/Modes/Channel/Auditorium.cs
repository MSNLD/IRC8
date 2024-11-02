using Irc.Enumerations;
using Irc.Objects;
using Irc.Resources;

namespace Irc.Modes.Channel;

public class Auditorium : ModeRuleChannel
{
    public Auditorium() : base(IrcStrings.ChannelModeAuditorium)
    {
    }

    public new EnumIrcError Evaluate(ChatObject source, ChatObject? target, bool flag, string? parameter)
    {
        return EvaluateAndSet(source, target, flag, parameter);
    }
}