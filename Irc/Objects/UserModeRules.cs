using Irc.Modes;
using Irc.Modes.Channel.Member;
using Irc.Modes.User;
using Irc.Resources;

namespace Irc.Objects;

public static class UserModeRules
{
    public static Dictionary<char, ModeRule> ModeRules = new()
    {
        { IrcStrings.UserModeOper, new Oper() },
        { IrcStrings.UserModeInvisible, new Invisible() },
        { IrcStrings.UserModeSecure, new Secure() },
        { IrcStrings.UserModeServerNotice, new ServerNotice() },
        { IrcStrings.UserModeWallops, new WallOps() },
        { IrcStrings.UserModeAdmin, new Admin() },
        { IrcStrings.UserModeIrcx, new Isircx() },
        { IrcStrings.UserModeGag, new Gag() },
        { IrcStrings.UserModeHost, new Host() }
    };
}