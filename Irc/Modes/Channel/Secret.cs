using Irc.Enumerations;
using Irc.Interfaces;
using Irc.Resources;

namespace Irc.Modes.Channel
{
    public class Secret : ModeRuleChannel, IModeRule
    {
        public Secret() : base(IrcStrings.ChannelModeSecret)
        {
        }

        public EnumIrcError Evaluate(IChatObject source, IChatObject target, bool flag, string parameter)
        {
            var result = base.Evaluate(source, target, flag, parameter);
            if (result == EnumIrcError.OK)
            {
                var channel = (IChannel)target;

                if (flag)
                {
                    if (channel.Modes.Private)
                    {
                        channel.Modes.Private = false;
                        DispatchModeChange(IrcStrings.ChannelModePrivate, source, target, false, string.Empty);
                    }

                    if (channel.Modes.Hidden)
                    {
                        channel.Modes.Hidden = false;
                        DispatchModeChange(IrcStrings.ChannelModeHidden, source, target, false, string.Empty);
                    }
                }

                SetChannelMode(source, (IChannel)target, flag, parameter);
            }

            return result;
        }
    }
}