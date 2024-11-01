using Irc.Interfaces;
using Irc.Objects;
using Irc.Objects.Channel;
using Irc.Objects.Server;
using Irc.Objects.User;

namespace Irc.Modes;

public static class ModeRules
{
    public static Dictionary<Type, Dictionary<char, IModeRule>> Rules =
        new()
        {
            { typeof(Objects.User.User), UserModeRules.ModeRules },
            { typeof(Objects.Channel.Channel), ChannelModeRules.ModeRules },
            { typeof(Objects.Server.Server), ServerModeRules.ModeRules }
        };

    public static Dictionary<char, IModeRule> GetRules(ChatObject chatObject)
    {
        return Rules[chatObject.GetType()];
    }
}