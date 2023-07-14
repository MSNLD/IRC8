﻿using Irc.Constants;
using Irc.Enumerations;
using Irc.Interfaces;

namespace Irc.Modes.Channel;

public class NoExtern : ModeRuleChannel, IModeRule
{
    public NoExtern() : base(Resources.ChannelModeNoExtern)
    {
    }

    public new EnumIrcError Evaluate(IChatObject source, IChatObject target, bool flag, string parameter)
    {
        return EvaluateAndSet(source, target, flag, parameter);
    }
}