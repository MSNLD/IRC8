using Irc.Enumerations;
using Irc.Interfaces;
using Irc.Resources;

namespace Irc.Modes.User;

public class WallOps : ModeRule, IModeRule
{
    public WallOps() : base(IrcStrings.UserModeWallops)
    {
    }

    public new EnumIrcError Evaluate(IChatObject source, IChatObject? target, bool flag, string? parameter)
    {
        return EnumIrcError.OK;
    }
}