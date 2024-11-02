using Irc.Enumerations;
using Irc.Interfaces;
using Irc.Objects;
using Irc.Resources;

namespace Irc.Modes.Channel;

public class Secret : ModeRuleChannel, IModeRule
{
    public Secret() : base(IrcStrings.ChannelModeSecret)
    {
    }

    public new EnumIrcError Evaluate(ChatObject source, ChatObject? target, bool flag, string? parameter)
    {
        var result = base.Evaluate(source, target, flag, parameter);
        if (result == EnumIrcError.OK)
        {
            var channel = (Objects.Channel)target;

            if (flag)
            {
                if (channel.Private)
                {
                    channel.Private = false;
                    DispatchModeChange(IrcStrings.ChannelModePrivate, source, target, false, string.Empty);
                }

                if (channel.Hidden)
                {
                    channel.Hidden = false;
                    DispatchModeChange(IrcStrings.ChannelModeHidden, source, target, false, string.Empty);
                }
            }

            SetChannelMode(source, (Objects.Channel)target, flag, parameter);
        }

        return result;
    }
}