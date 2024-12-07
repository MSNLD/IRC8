using Irc.Modes;
using Irc.Modes.Channel.Member;
using Irc.Modes.User;
using Irc.Resources;

namespace Irc.Objects;

public static class UserModeRules
{
    public static Dictionary<char, ModeRule> ModeRules = new()
    {
        { Tokens.UserModeOper, new Oper() },
        { Tokens.UserModeInvisible, new Invisible() },
        { Tokens.UserModeSecure, new Secure() },
        { Tokens.UserModeServerNotice, new ServerNotice() },
        { Tokens.UserModeWallops, new WallOps() },
        { Tokens.UserModeAdmin, new Admin() },
        { Tokens.UserModeIrcx, new Isircx() },
        { Tokens.UserModeGag, new Gag() },
        { Tokens.UserModeHost, new Host() }
    };
}