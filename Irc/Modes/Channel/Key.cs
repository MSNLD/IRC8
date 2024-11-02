﻿using Irc.Enumerations;
using Irc.Objects;
using Irc.Resources;

namespace Irc.Modes.Channel;

internal class Key : ModeRuleChannel
{
    public Key() : base(IrcStrings.ChannelModeKey, true)
    {
    }

    public new EnumIrcError Evaluate(ChatObject source, ChatObject? target, bool flag, string? parameter)
    {
        var channel = (Objects.Channel)target;
        var member = channel.GetMember((Objects.User)source);
        if (member.GetLevel() >= EnumChannelAccessLevel.ChatHost)
        {
            // Unset key
            if (!flag && parameter == channel.Props[IrcStrings.ChannelPropMemberkey])
            {
                channel.Props[IrcStrings.ChannelPropMemberkey] = null;
                DispatchModeChange(source, (ChatObject)target, flag, parameter);
                return EnumIrcError.OK;
            }

            // Set key
            if (flag)
            {
                if (!string.IsNullOrWhiteSpace(channel.Props[IrcStrings.ChannelPropMemberkey]))
                    return EnumIrcError.ERR_KEYSET;

                channel.Props[IrcStrings.ChannelPropMemberkey] = flag ? parameter : null;
                DispatchModeChange(source, (ChatObject)target, flag, parameter);
            }

            return EnumIrcError.OK;
        }

        /* -> sky-8a15b323126 MODE #test +t
            <- :sky-8a15b323126 482 Sky2k #test :You're not channel operator */
        return EnumIrcError.ERR_NOCHANOP;
    }
}