using Irc.Enumerations;
using Irc.Interfaces;
using Irc.Resources;

namespace Irc.Modes.Channel
{
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
}