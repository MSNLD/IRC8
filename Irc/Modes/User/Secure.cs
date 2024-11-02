using Irc.Enumerations;
using Irc.Interfaces;
using Irc.Objects;
using Irc.Resources;

namespace Irc.Modes.User;

public class Secure : ModeRule, IModeRule
{
    public Secure() : base(IrcStrings.UserModeSecure)
    {
    }

    public new EnumIrcError Evaluate(ChatObject source, ChatObject? target, bool flag, string? parameter)
    {
        return EnumIrcError.ERR_NOPERMS;
    }
}