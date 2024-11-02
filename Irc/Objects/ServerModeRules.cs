using Irc.Interfaces;

namespace Irc.Objects;

public class ServerModeRules
{
    public static Dictionary<char, IModeRule> ModeRules = new();
}