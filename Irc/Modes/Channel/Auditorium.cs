using Irc.Enumerations;
using Irc.Interfaces;
using Irc.Resources;

namespace Irc.Modes.Channel;

public class Auditorium : ModeRuleChannel, IModeRule
{
    public Auditorium() : base(IrcStrings.ChannelModeAuditorium)
    {
    }

    public new EnumIrcError Evaluate(IChatObject source, IChatObject? target, bool flag, string? parameter)
    {
        return EvaluateAndSet(source, target, flag, parameter);
    }
}