using Irc.Enumerations;
using Irc.Interfaces;
using Irc.Modes;
using Irc.Resources;

namespace Irc.Extensions.Apollo.Modes.Channel;

public class OnStage : ModeRuleChannel, IModeRule
{
    public OnStage() : base(IrcStrings.ChannelModeOnStage)
    {
    }

    public new EnumIrcError Evaluate(IChatObject source, IChatObject target, bool flag, string parameter)
    {
        return EvaluateAndSet(source, target, flag, parameter);
    }
}