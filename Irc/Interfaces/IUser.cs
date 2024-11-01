using Irc.Enumerations;
using Irc.Modes;
using Irc.Objects;
using Irc.Objects.User;

namespace Irc.Interfaces;

public interface IUser: IChatObject
{
    IServer Server { get; }
    Guid Id { get; }
    string ShortId { get; }
    string? Name { get; set; }
    string? Nickname { get; set; }
    bool Away { get; set; }
    string? Client { get; set; }
    string? Pass { get; set; }
    DateTime LastIdle { get; set; }
    DateTime LoggedOn { get; }
    bool Utf8 { get; set; }
    IProtocol Protocol { get; set; }
    IChatFrame GetNextFrame();
    void ChangeNickname(string? newNick, bool utf8Prefix);
    void SetGuest(bool guest);
    void SetAway(IServer server, IUser? user, string? message);
    void SetBack(IServer server, IUser? user);
    void SetLevel(EnumUserAccessLevel level);
    event EventHandler<string> OnSend;
    void BroadcastToChannels(string data, bool ExcludeUser);
    void AddChannel(IChannel? channel, IChannelMember member);
    void RemoveChannel(IChannel? channel);
    KeyValuePair<IChannel, IChannelMember> GetChannelMemberInfo(IChannel channel);
    KeyValuePair<IChannel, IChannelMember> GetChannelInfo(string name);
    IDictionary<IChannel?, IChannelMember> GetChannels();
    new void Send(string message);
    new void Send(string message, EnumChannelAccessLevel accessLevel);
    void Flush();
    void Disconnect(string message);
    IDataRegulator GetDataRegulator();
    IFloodProtectionProfile GetFloodProtectionProfile();
    ISupportPackage GetSupportPackage();
    void SetSupportPackage(ISupportPackage supportPackage);
    IConnection GetConnection();
    EnumUserAccessLevel GetLevel();
    Address GetAddress();
    bool IsGuest();
    bool IsRegistered();
    bool IsAuthenticated();
    bool IsAnon();
    bool IsSysop();
    bool IsAdministrator();
    bool IsOn(IChannel? channel);
    void PromoteToAdministrator();
    void PromoteToSysop();
    void PromoteToGuide();
    bool DisconnectIfOutgoingThresholdExceeded();
    bool DisconnectIfIncomingThresholdExceeded();
    string? ToString();
    void Register();
    void Authenticate();
    void DisconnectIfInactive();
    Queue<ModeOperation> GetModeOperations();
    Profile GetProfile();
    
    // Modes
    public bool Oper { get; set; }
    public bool Invisible { get; set; }
    public bool Secure { get; set; }
    public bool ServerNotice { get; set; }
    public bool Wallops { get; set; }
    public bool Admin { get; set; }
    public bool Ircx { get; set; }
    public bool Gag { get; set; }
    public bool Host { get; set; }
}