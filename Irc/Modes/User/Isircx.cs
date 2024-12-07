using Irc.Enumerations;
using Irc.Objects;
using Irc.Resources;

namespace Irc.Modes.User;

public class Isircx : ModeRule
{
    public Isircx() : base(Tokens.UserModeIrcx)
    {
    }

    public override EnumIrcError Evaluate(ChatObject source, ChatObject? target, bool flag, string? parameter)
    {
        return EnumIrcError.ERR_UNKNOWNMODEFLAG;
    }
}