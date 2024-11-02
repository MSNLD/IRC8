using Irc.Interfaces;
using Irc.Objects;

namespace Irc.Modes;

public static class ModeRules
{
    public static Dictionary<Type, Dictionary<char, IModeRule>> Rules =
        new()
        {
            { typeof(Objects.User), UserModeRules.ModeRules },
            { typeof(Objects.Channel), ChannelModeRules.ModeRules },
            { typeof(Server), ServerModeRules.ModeRules }
        };

    public static Dictionary<char, IModeRule> GetRules(ChatObject chatObject)
    {
        return Rules[chatObject.GetType()];
    }
}