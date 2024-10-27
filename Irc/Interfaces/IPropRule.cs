using Irc.Enumerations;
using Irc.Interfaces;

namespace Irc.Extensions.Interfaces;

public interface IPropRule
{
    EnumChannelAccessLevel ReadAccessLevel { get; }
    EnumChannelAccessLevel WriteAccessLevel { get; }
    string Name { get; }
    bool ReadOnly { get; }
    EnumIrcError EvaluateSet(IChatObject source, IChatObject target, string propValue);
    EnumIrcError EvaluateGet(IChatObject source, IChatObject target);
    string GetValue(IChatObject target);
    void SetValue(string value);
}