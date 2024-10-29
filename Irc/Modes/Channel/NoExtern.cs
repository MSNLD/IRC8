using Irc.Enumerations;
using Irc.Interfaces;
using Irc.Resources;

namespace Irc.Modes.Channel
{
    public class NoExtern : ModeRuleChannel, IModeRule
    {
        public NoExtern() : base(IrcStrings.ChannelModeNoExtern)
        {
        }

        public new EnumIrcError Evaluate(IChatObject source, IChatObject target, bool flag, string parameter)
        {
            return EvaluateAndSet(source, target, flag, parameter);
        }
    }
}