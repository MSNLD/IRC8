using Irc.Enumerations;
using Irc.Objects;

namespace Irc.Interfaces;

public interface IChatObject
{
    EnumUserAccessLevel Level { get; }
    Dictionary<char, int> Modes { get; set; }
    Dictionary<string?, string?> Props { get; set; }
    void Send(string message);
    void Send(string message, EnumChannelAccessLevel accessLevel);
    string? ToString();
    bool CanBeModifiedBy(IChatObject source);
}