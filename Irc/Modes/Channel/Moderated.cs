using Irc.Enumerations;
using Irc.Interfaces;
using Irc.Resources;

namespace Irc.Modes.Channel
{
    public class Moderated : ModeRuleChannel, IModeRule
    {
        public Moderated() : base(IrcStrings.ChannelModeModerated)
        {
        }

        public new EnumIrcError Evaluate(IChatObject source, IChatObject target, bool flag, string parameter)
        {
            return EvaluateAndSet(source, target, flag, parameter);
        }
    }
}