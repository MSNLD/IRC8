using Irc.Enumerations;
using Irc.Interfaces;
using Irc.Resources;

namespace Irc.Modes.User;

public class ServerNotice : ModeRule, IModeRule
{
    public ServerNotice() : base(IrcStrings.UserModeServerNotice)
    {
    }

    public new EnumIrcError Evaluate(IChatObject source, IChatObject target, bool flag, string parameter)
    {
        return EnumIrcError.OK;
    }
}