using Irc.Enumerations;
using Irc.Objects;
using Irc.Resources;

namespace Irc.Modes.User;

public class WallOps : ModeRule
{
    public WallOps() : base(Tokens.UserModeWallops)
    {
    }

    public override EnumIrcError Evaluate(ChatObject source, ChatObject? target, bool flag, string? parameter)
    {
        return EnumIrcError.OK;
    }
}