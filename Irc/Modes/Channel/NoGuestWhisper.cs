using Irc.Enumerations;
using Irc.Objects;
using Irc.Resources;

namespace Irc.Modes.Channel;

public class NoGuestWhisper : ModeRuleChannel
{
    public NoGuestWhisper() : base(Tokens.ChannelModeNoGuestWhisper)
    {
    }

    public new EnumIrcError Evaluate(ChatObject source, ChatObject? target, bool flag, string? parameter)
    {
        return EvaluateAndSet(source, target, flag, parameter);
    }
}