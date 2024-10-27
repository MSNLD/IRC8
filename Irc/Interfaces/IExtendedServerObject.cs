using Irc.Objects;

namespace Irc.Extensions.Interfaces;

public interface IExtendedServerObject
{
    void ProcessCookie(IUser user, string name, string value);
}