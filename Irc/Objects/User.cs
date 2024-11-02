using System.Collections.Concurrent;
using System.Text;
using Irc.Enumerations;
using Irc.Interfaces;
using Irc.Modes;
using Irc.Protocols;
using Irc.Resources;
using Irc.Security.Packages;
using NLog;

namespace Irc.Objects;

public class User : ChatObject
{
    public static readonly NLog.Logger Log = LogManager.GetCurrentClassLogger();
    private readonly UserAccess _accessList = new();

    //public Access Access;
    private readonly IConnection _connection;
    private readonly IDataRegulator _dataRegulator;
    private readonly IFloodProtectionProfile _floodProtectionProfile;
    private readonly Queue<ModeOperation> _modeOperations = new();
    private bool _authenticated;

    private long _commandSequence;
    private bool _guest;
    private EnumUserAccessLevel _level;
    private bool _registered;
    private ISupportPackage _supportPackage;
    public IDictionary<Channel?, Member> Channels;

    public DateTime LastPing = DateTime.UtcNow;
    public long PingCount;

    public User(IConnection connection, IDataRegulator dataRegulator,
        IFloodProtectionProfile floodProtectionProfile,
        Server server)
    {
        Server = server;
        _connection = connection;
        _dataRegulator = dataRegulator;
        _floodProtectionProfile = floodProtectionProfile;
        _supportPackage = new ANON();
        Channels = new ConcurrentDictionary<Channel?, Member>();

        _connection.OnReceive += (sender, s) =>
        {
            LastPing = DateTime.UtcNow;
            PingCount = 0;
            var message = new Message(Protocol, s);
            if (message.HasCommand) _dataRegulator.PushIncoming(message);
        };

        Address.SetIP(connection.GetIp());

        Props.Add(IrcStrings.UserPropOid, "");
        Props.Add(IrcStrings.UserPropSubscriberInfo, "");
        Props.Add(IrcStrings.UserPropMsnProfile, "");
        Props.Add(IrcStrings.UserPropRole, "");

        // TODO: Add Modes
        Modes.Add(IrcStrings.UserModeOper, 0);
        Modes.Add(IrcStrings.UserModeInvisible, 0);
        Modes.Add(IrcStrings.UserModeSecure, 0);
        Modes.Add(IrcStrings.UserModeServerNotice, 0);
        Modes.Add(IrcStrings.UserModeWallops, 0);

        //IRCX
        Modes.Add(IrcStrings.UserModeAdmin, 0);
        Modes.Add(IrcStrings.UserModeIrcx, 0);
        Modes.Add(IrcStrings.UserModeGag, 0);

        //Apollo
        Modes.Add(IrcStrings.UserModeHost, 0);
    }

    public IProtocol Protocol { get; set; } = new Protocol();

    public EnumUserAccessLevel Level => GetLevel();

    public Address Address { get; set; } = new();
    private Profile Profile { get; } = new();

    public IAccessList AccessList => _accessList;
    public bool Utf8 { get; set; }
    public string? Client { get; set; }
    public string? Pass { get; set; }
    public DateTime LastIdle { get; set; } = DateTime.UtcNow;
    public DateTime LoggedOn { get; private set; } = DateTime.UtcNow;

    public Server Server { get; }

    public string? Nickname
    {
        get => Name;
        set
        {
            Name = value;
            Address.SetNickname(value);
        }
    }

    public bool Away { get; set; }
    public event EventHandler<string> OnSend;

    public void BroadcastToChannels(string data, bool excludeUser)
    {
        foreach (var channel in Channels.Keys) channel?.Send(data, this);
    }

    public void AddChannel(Channel? channel, Member member)
    {
        Channels.Add(channel, member);
    }

    public void RemoveChannel(Channel? channel)
    {
        Channels.Remove(channel);
    }

    public KeyValuePair<Channel, Member> GetChannelMemberInfo(Channel channel)
    {
        return Channels.FirstOrDefault(c => c.Key == channel);
    }

    public KeyValuePair<Channel, Member> GetChannelInfo(string Name)
    {
        return Channels.FirstOrDefault(c => c.Key.GetName() == Name);
    }

    public IDictionary<Channel?, Member> GetChannels()
    {
        return Channels;
    }

    public override void Send(string message)
    {
        _dataRegulator.PushOutgoing(message);
    }

    public override void Send(string message, ChatObject except = null)
    {
        throw new NotImplementedException();
    }

    public override void Send(string message, EnumChannelAccessLevel accessLevel)
    {
        Send(message);
    }

    public void Flush()
    {
        var totalBytes = _dataRegulator.GetOutgoingBytes();

        if (_dataRegulator.GetOutgoingBytes() > 0)
        {
            // Compensate for \r\n
            var queueLength = _dataRegulator.GetOutgoingQueueLength();
            var adjustedTotalBytes = totalBytes + queueLength * 2;

            var stringBuilder = new StringBuilder(adjustedTotalBytes);
            for (var i = 0; i < queueLength; i++)
            {
                stringBuilder.Append(_dataRegulator.PopOutgoing());
                stringBuilder.Append("\r\n");
            }

            Log.Info($"Sending[{Protocol.GetType().Name}/{Name}]: {stringBuilder}");
            _connection?.Send(stringBuilder.ToString());
        }
    }

    public void Disconnect(string message)
    {
        // Clean modes
        _modeOperations.Clear();

        Log.Info($"Disconnecting[{Protocol.GetType().Name}/{Name}]: {message}");
        _connection?.Disconnect($"{message}\r\n");
    }

    public IDataRegulator GetDataRegulator()
    {
        return _dataRegulator;
    }

    public IFloodProtectionProfile GetFloodProtectionProfile()
    {
        return _floodProtectionProfile;
    }

    public ISupportPackage GetSupportPackage()
    {
        return _supportPackage;
    }

    public void SetSupportPackage(ISupportPackage supportPackage)
    {
        _supportPackage = supportPackage;
    }

    public void SetProtocol(IProtocol protocol)
    {
        Protocol = protocol;
    }

    public IConnection GetConnection()
    {
        return _connection;
    }

    public EnumUserAccessLevel GetLevel()
    {
        return _level;
    }

    public void ChangeNickname(string? newNick, bool utf8Prefix)
    {
        var nickname = utf8Prefix ? $"'{newNick}" : newNick;
        var rawNicknameChange = Raw.RPL_NICK(Server, this, nickname);
        Send(rawNicknameChange);
        Nickname = nickname;

        foreach (var channel in Channels) channel.Key.Send(rawNicknameChange, this);
    }

    public Address GetAddress()
    {
        return Address;
    }

    public bool IsGuest()
    {
        return _guest;
    }

    public virtual void SetGuest(bool guest)
    {
        Profile.Guest = guest;
        if (Server.DisableGuestMode) return;
        _guest = guest;
    }

    public void SetLevel(EnumUserAccessLevel level)
    {
        _level = level;
    }

    public bool IsRegistered()
    {
        return _registered;
    }

    public bool IsAuthenticated()
    {
        return _authenticated;
    }

    public bool IsOn(Channel? channel)
    {
        return Channels.ContainsKey(channel);
    }

    public bool IsAnon()
    {
        return _supportPackage is ANON;
    }

    public bool IsSysop()
    {
        return Oper;
    }

    public bool IsAdministrator()
    {
        return Admin;
    }

    public virtual void SetAway(Server server, User? user, string? message)
    {
        Profile.Away = true;
        user.Away = true;
        foreach (var channelPair in user.GetChannels())
        {
            var channel = channelPair.Key;
            channel.Send(Raw.IRCX_RPL_USERNOWAWAY_822(server, user, message), user);
        }

        user.Send(Raw.IRCX_RPL_NOWAWAY_306(server, user));
    }

    public virtual void SetBack(Server server, User? user)
    {
        Profile.Away = false;
        user.Away = false;
        foreach (var channelPair in user.GetChannels())
        {
            var channel = channelPair.Key;
            channel.Send(Raw.IRCX_RPL_USERUNAWAY_821(server, user), user);
        }

        user.Send(Raw.IRCX_RPL_UNAWAY_305(server, user));
    }

    public virtual void PromoteToAdministrator()
    {
        Profile.Level = EnumUserAccessLevel.Administrator;
        Admin = true;
        ModeRule.DispatchModeChange(IrcStrings.UserModeAdmin, this, this, true, ToString());
        _level = EnumUserAccessLevel.Administrator;
        Send(Raw.IRCX_RPL_YOUREADMIN_386(Server, this));
    }

    public virtual void PromoteToSysop()
    {
        Profile.Level = EnumUserAccessLevel.Sysop;
        Oper = true;
        ModeRule.DispatchModeChange(IrcStrings.UserModeOper, this, this, true, ToString());
        _level = EnumUserAccessLevel.Sysop;
        Send(Raw.IRCX_RPL_YOUREOPER_381(Server, this));
    }

    public virtual void PromoteToGuide()
    {
        Profile.Level = EnumUserAccessLevel.Guide;
        Oper = true;
        ModeRule.DispatchModeChange(IrcStrings.UserModeOper, this, this, true, ToString());
        _level = EnumUserAccessLevel.Guide;
        Send(Raw.IRCX_RPL_YOUREGUIDE_629(Server, this));
    }

    public bool DisconnectIfOutgoingThresholdExceeded()
    {
        if (GetDataRegulator().IsOutgoingThresholdExceeded())
        {
            GetDataRegulator().Purge();
            Disconnect("Output quota exceeded");
            return true;
        }

        return false;
    }

    public bool DisconnectIfIncomingThresholdExceeded()
    {
        // Disconnect user if incoming quota exceeded
        if (GetDataRegulator().IsIncomingThresholdExceeded())
        {
            GetDataRegulator().Purge();
            Disconnect("Input quota exceeded");
            return true;
        }

        return false;
    }

    public void DisconnectIfInactive()
    {
        var seconds = (DateTime.UtcNow.Ticks - LastPing.Ticks) / TimeSpan.TicksPerSecond;
        if (seconds > (PingCount + 1) * Server.PingInterval)
        {
            if (PingCount < Server.PingAttempts)
            {
                Log.Debug($"Ping Count for {this} hit stage {PingCount + 1}");
                PingCount++;
                Send(Raw.RPL_PING(Server, this));
            }
            else
            {
                GetDataRegulator().Purge();
                Disconnect(Raw.IRCX_CLOSINGLINK_011_PINGTIMEOUT(Server, this, _connection.GetIp()));
            }
        }
    }

    public void Register()
    {
        var userAddress = GetAddress();
        var credentials = GetSupportPackage().GetCredentials();
        userAddress.User = credentials.GetUsername() ?? userAddress.MaskedIP;
        userAddress.Host = credentials.GetDomain();
        userAddress.Server = Server.Name;
        userAddress.RealName = credentials.Guest ? string.Empty : null;

        LoggedOn = DateTime.UtcNow;
        _authenticated = true;
        _registered = true;
    }

    public void Authenticate()
    {
        _authenticated = true;
    }

    public Queue<ModeOperation> GetModeOperations()
    {
        return _modeOperations;
    }

    public IChatFrame GetNextFrame()
    {
        _commandSequence++;
        var message = _dataRegulator.PopIncoming();
        return new ChatFrame
        {
            SequenceId = _commandSequence,
            Server = Server,
            User = this,
            Message = message
        };
    }

    public Profile GetProfile()
    {
        return Profile;
    }

    public override bool CanBeModifiedBy(ChatObject source)
    {
        return source == this;
    }

    #region Modes

    public bool Oper
    {
        get => Convert.ToBoolean(Modes[IrcStrings.UserModeOper]);
        set => Modes[IrcStrings.UserModeOper] = Convert.ToInt32(value);
    }

    public bool Invisible
    {
        get => Convert.ToBoolean(Modes[IrcStrings.UserModeInvisible]);
        set => Modes[IrcStrings.UserModeOper] = Convert.ToInt32(value);
    }

    public bool Secure
    {
        get => Convert.ToBoolean(Modes[IrcStrings.UserModeSecure]);
        set => Modes[IrcStrings.UserModeSecure] = Convert.ToInt32(value);
    }

    public bool ServerNotice
    {
        get => Convert.ToBoolean(Modes[IrcStrings.UserModeServerNotice]);
        set => Modes[IrcStrings.UserModeServerNotice] = Convert.ToInt32(value);
    }

    public bool Wallops
    {
        get => Convert.ToBoolean(Modes[IrcStrings.UserModeWallops]);
        set => Modes[IrcStrings.UserModeWallops] = Convert.ToInt32(value);
    }

    public bool Admin
    {
        get => Convert.ToBoolean(Modes[IrcStrings.UserModeAdmin]);
        set => Modes[IrcStrings.UserModeAdmin] = Convert.ToInt32(value);
    }


    public bool Ircx
    {
        get => Convert.ToBoolean(Modes[IrcStrings.UserModeIrcx]);
        set => Modes[IrcStrings.UserModeIrcx] = Convert.ToInt32(value);
    }


    public bool Gag
    {
        get => Convert.ToBoolean(Modes[IrcStrings.UserModeGag]);
        set => Modes[IrcStrings.UserModeGag] = Convert.ToInt32(value);
    }


    public bool Host
    {
        get => Convert.ToBoolean(Modes[IrcStrings.UserModeHost]);
        set => Modes[IrcStrings.UserModeHost] = Convert.ToInt32(value);
    }

    #endregion
}