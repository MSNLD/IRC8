using Irc.Objects;

namespace Irc.Modes;

public static class ModeRules
{
    public static Dictionary<Type, Dictionary<char, ModeRule>> Rules =
        new()
        {
            { typeof(Objects.User), UserModeRules.ModeRules },
            { typeof(Objects.Channel), ChannelModeRules.ModeRules },
            { typeof(Server), ServerModeRules.ModeRules }
        };

    public static Dictionary<char, ModeRule> GetRules(ChatObject chatObject)
    {
        return Rules[chatObject.GetType()];
    }
}