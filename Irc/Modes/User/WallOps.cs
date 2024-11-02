using Irc.Enumerations;
using Irc.Interfaces;
using Irc.Objects;
using Irc.Resources;

namespace Irc.Modes.User;

public class WallOps : ModeRule, IModeRule
{
    public WallOps() : base(IrcStrings.UserModeWallops)
    {
    }

    public new EnumIrcError Evaluate(ChatObject source, ChatObject? target, bool flag, string? parameter)
    {
        return EnumIrcError.OK;
    }
}