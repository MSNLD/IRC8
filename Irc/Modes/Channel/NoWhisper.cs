using Irc.Enumerations;
using Irc.Interfaces;
using Irc.Modes;
using Irc.Resources;

namespace Irc.Extensions.Modes.Channel;

public class NoWhisper : ModeRuleChannel, IModeRule
{
    public NoWhisper() : base(IrcStrings.ChannelModeNoWhisper)
    {
    }

    public new EnumIrcError Evaluate(IChatObject source, IChatObject target, bool flag, string parameter)
    {
        return EvaluateAndSet(source, target, flag, parameter);
    }
}