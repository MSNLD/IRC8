using Irc.Enumerations;
using Irc.Interfaces;
using Irc.Resources;

namespace Irc.Modes.Channel;

public class NoGuestWhisper : ModeRuleChannel, IModeRule
{
    public NoGuestWhisper() : base(IrcStrings.ChannelModeNoGuestWhisper)
    {
    }

    public new EnumIrcError Evaluate(IChatObject source, IChatObject? target, bool flag, string? parameter)
    {
        return EvaluateAndSet(source, target, flag, parameter);
    }
}