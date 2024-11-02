using Irc.Enumerations;
using Irc.Objects;

namespace Irc.Interfaces;

public interface IChannel : IChatObject
{
    string? GetName();
    IChannelMember? GetMember(IUser? user);
    IChannelMember? GetMemberByNickname(string? nickname);
    bool HasUser(IUser? user);
    void Send(string message, ChatObject? u = null);
    new void Send(string message);
    new void Send(string message, EnumChannelAccessLevel accessLevel);
    IChannel Join(IUser? user, EnumChannelAccessResult accessResult = EnumChannelAccessResult.NONE);
    IChannel Part(IUser? user);
    IChannel Quit(IUser? user);
    IChannel Kick(IUser? source, IUser? target, string? reason);
    void SendMessage(IUser? user, string? message);
    void SendNotice(IUser? user, string? message);
    IList<IChannelMember?> GetMembers();
    new bool CanBeModifiedBy(IChatObject source);
    EnumIrcError CanModifyMember(IChannelMember? source, IChannelMember target, EnumChannelAccessLevel requiredLevel);

    void ProcessChannelError(EnumIrcError error, IServer server, IUser? source, ChatObject? target = null,
        string? data = null);

    IChannel SendTopic(IUser? user);
    IChannel SendTopic();
    IChannel SendNames(IUser? user);
    bool Allows(IUser? user);
    EnumChannelAccessResult GetAccess(IUser? user, string? key, bool isGoto = false);
    bool InviteMember(IUser user);
}