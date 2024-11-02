using Irc.Objects;

namespace Irc.Interfaces;

public interface IExtendedServerObject
{
    void ProcessCookie(User? user, string name, string? value);
}