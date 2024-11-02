using Irc.Enumerations;
using Irc.Interfaces;
using Irc.Objects;
using Irc.Resources;

namespace Irc.Modes.User;

public class Oper : ModeRule, IModeRule
{
    public Oper() : base(IrcStrings.UserModeOper)
    {
    }

    public new EnumIrcError Evaluate(ChatObject source, ChatObject? target, bool flag, string? parameter)
    {
        // :sky-8a15b323126 908 Sky :No permissions to perform command
        if (source is Objects.User && ((Objects.User)source).IsSysop() && flag == false)
        {
            target.Modes[IrcStrings.UserModeOper] = Convert.ToInt32(flag);
            DispatchModeChange(source, target, flag, parameter);
            return EnumIrcError.OK;
        }

        return EnumIrcError.ERR_NOPERMS;
    }
}