﻿using Irc.Enumerations;
using Irc.Interfaces;
using Irc.Modes;
using Irc.Resources;

namespace Irc.Extensions.Modes.User;

public class Isircx : ModeRule, IModeRule
{
    public Isircx() : base(IrcStrings.UserModeIrcx)
    {
    }

    public new EnumIrcError Evaluate(IChatObject source, IChatObject target, bool flag, string parameter)
    {
        return EnumIrcError.ERR_UNKNOWNMODEFLAG;
    }
}