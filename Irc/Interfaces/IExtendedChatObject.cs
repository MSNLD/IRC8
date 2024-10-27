namespace Irc.Interfaces;

public interface IExtendedChatObject : IChatObject
{
    IPropCollection PropCollection { get; }
}