using Irc.Enumerations;
using Irc.Interfaces;
using Irc.Modes;
using Irc.Resources;

namespace Irc.Extensions.Modes.Channel;

public class Cloneable : ModeRuleChannel, IModeRule
{
    public Cloneable() : base(IrcStrings.ChannelModeCloneable)
    {
    }

    public new EnumIrcError Evaluate(IChatObject source, IChatObject target, bool flag, string parameter)
    {
        return EvaluateAndSet(source, target, flag, parameter);
    }
}