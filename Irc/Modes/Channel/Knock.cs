using Irc.Enumerations;
using Irc.Interfaces;
using Irc.Resources;

namespace Irc.Modes.Channel;

public class Knock : ModeRuleChannel, IModeRule
{
    public Knock() : base(IrcStrings.ChannelModeKnock)
    {
    }

    public new EnumIrcError Evaluate(IChatObject source, IChatObject? target, bool flag, string? parameter)
    {
        return EvaluateAndSet(source, target, flag, parameter);
    }
}