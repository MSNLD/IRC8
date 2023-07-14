﻿using Irc.Enumerations;
using Irc.Interfaces;
using Irc.Modes;

namespace Irc.Extensions.Modes.Channel;

public class Knock : ModeRuleChannel, IModeRule
{
    public Knock() : base(ExtendedResources.ChannelModeKnock)
    {
    }

    public new EnumIrcError Evaluate(IChatObject source, IChatObject target, bool flag, string parameter)
    {
        return EvaluateAndSet(source, target, flag, parameter);
    }
}