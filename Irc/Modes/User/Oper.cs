﻿using Irc.Enumerations;
using Irc.Objects;
using Irc.Resources;

namespace Irc.Modes.User;

public class Oper : ModeRule
{
    public Oper() : base(Tokens.UserModeOper)
    {
    }

    public override EnumIrcError Evaluate(ChatObject source, ChatObject? target, bool flag, string? parameter)
    {
        // :sky-8a15b323126 908 Sky :No permissions to perform command
        if (source is Objects.User && ((Objects.User)source).IsSysop() && flag == false)
        {
            target.Modes[Tokens.UserModeOper] = Convert.ToInt32(flag);
            DispatchModeChange(source, target, flag, parameter);
            return EnumIrcError.OK;
        }

        return EnumIrcError.ERR_NOPERMS;
    }
}