using Irc.Enumerations;
using Irc.Objects;
using Irc.Resources;

namespace Irc.Modes.User;

public class ServerNotice : ModeRule
{
    public ServerNotice() : base(Tokens.UserModeServerNotice)
    {
    }

    public override EnumIrcError Evaluate(ChatObject source, ChatObject? target, bool flag, string? parameter)
    {
        return EnumIrcError.OK;
    }
}