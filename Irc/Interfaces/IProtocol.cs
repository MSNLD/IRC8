using Irc.Enumerations;
using Irc.Objects;

namespace Irc.Interfaces;

public interface IProtocol
{
    EnumProtocolType Ircvers { get; set; }
    ICommand? GetCommand(string? name);
    Dictionary<string?, ICommand> GetCommands();
    void AddCommand(ICommand command, string? name = null);
    void SetVers(EnumProtocolType ircvers);
    void FlushCommands();
    string FormattedUser(Member user);
    string GetFormat(User? user);
}