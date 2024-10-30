namespace Irc.Interfaces;

public interface IExtendedServerObject
{
    void ProcessCookie(IUser user, string name, string value);
}