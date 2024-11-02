using Irc.Enumerations;
using Irc.Objects;

namespace Irc.Interfaces;

public interface IPropRule
{
    EnumChannelAccessLevel ReadAccessLevel { get; }
    EnumChannelAccessLevel WriteAccessLevel { get; }
    string? Name { get; }
    bool ReadOnly { get; }
    EnumIrcError EvaluateSet(ChatObject source, ChatObject target, string? propValue);
    EnumIrcError EvaluateGet(ChatObject source, ChatObject target);
}