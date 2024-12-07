using Irc.Enumerations;
using Irc.Objects;
using Irc.Resources;

namespace Irc.Modes.User;

public class Secure : ModeRule
{
    public Secure() : base(Tokens.UserModeSecure)
    {
    }

    public override EnumIrcError Evaluate(ChatObject source, ChatObject? target, bool flag, string? parameter)
    {
        return EnumIrcError.ERR_NOPERMS;
    }
}