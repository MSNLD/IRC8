using Irc.Enumerations;
using Irc.Objects;
using Irc.Resources;

namespace Irc.Modes.User;

public class Invisible : ModeRule
{
    public Invisible() : base(Tokens.UserModeInvisible)
    {
    }

    public override EnumIrcError Evaluate(ChatObject source, ChatObject? target, bool flag, string? parameter)
    {
        if (source == target)
        {
            target.Modes[Tokens.UserModeInvisible] = Convert.ToInt32(flag);
            DispatchModeChange(source, target, flag, parameter);
            return EnumIrcError.OK;
        }

        return EnumIrcError.ERR_NOSUCHCHANNEL;
    }
}