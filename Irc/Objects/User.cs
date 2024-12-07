using System.Collections.Concurrent;
using System.Text;
using Irc.Access;
using Irc.Enumerations;
using Irc.Interfaces;
using Irc.IO;
using Irc.Modes;
using Irc.Protocols;
using Irc.Resources;
using Irc.Security;
using Irc.Security.Packages;
using NLog;

namespace Irc.Objects;

public class User : ChatObject
{
    public static readonly NLog.Logger Log = LogManager.GetCurrentClassLogger();

    private readonly IConnection _connection;
    private readonly DataRegulator _dataRegulator;
    public Queue<ModeOperation> ModeOperations { set; get; } = new();
    private bool _authenticated;

    private long _commandSequence;
    private bool _guest;
    private bool _registered;
    public SupportPackage SupportPackage { get; set; } = new ANON();
    public ConcurrentDictionary<Channel, Member> Channels;
    public FloodProtectionManager FloodProtection { get; } = new();

    public DateTime LastPing = DateTime.UtcNow;
    public long PingCount;

    public User(IConnection connection, DataRegulator dataRegulator, Server server)
    {
        Server = server;
        _connection = connection;
        _dataRegulator = dataRegulator;
        Channels = new ConcurrentDictionary<Channel, Member>();

        _connection.OnReceive += (_, s) =>
        {
            LastPing = DateTime.UtcNow;
            PingCount = 0;
            var message = new Message(Protocol, s);
            if (message.HasCommand) _dataRegulator.PushIncoming(message);
        };

        Address.SetIP(connection.GetIp());

        Props.Add(Tokens.UserPropOid, "");
        Props.Add(Tokens.UserPropSubscriberInfo, "");
        Props.Add(Tokens.UserPropMsnProfile, "");
        Props.Add(Tokens.UserPropRole, "");

        // IRC
        Modes.Add(Tokens.UserModeOper, 0);
        Modes.Add(Tokens.UserModeInvisible, 0);
        Modes.Add(Tokens.UserModeSecure, 0);
        Modes.Add(Tokens.UserModeServerNotice, 0);
        Modes.Add(Tokens.UserModeWallops, 0);

        //IRCX
        Modes.Add(Tokens.UserModeAdmin, 0);
        Modes.Add(Tokens.UserModeIrcx, 0);
        Modes.Add(Tokens.UserModeGag, 0);

        //Apollo
        Modes.Add(Tokens.UserModeHost, 0);
        
        // Access
        AccessList.Entries = new Dictionary<EnumAccessLevel, List<AccessEntry>>
        {
            { EnumAccessLevel.VOICE, new List<AccessEntry>() },
            { EnumAccessLevel.DENY, new List<AccessEntry>() }
        };
    }

    public Protocol Protocol { get; set; } = new();
    public Address Address { get; set; } = new();
    public Profile Profile { get; } = new();

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

    public void BroadcastToChannels(string data, bool excludeUser) => 
        Channels.Keys.ToList().ForEach( c => c.Send(data, excludeUser ? this : null) );

    public void AddChannel(Channel channel, Member member)
    {
        // TODO: What if this fails?
        Channels.TryAdd(channel, member);
    }

    public void RemoveChannel(Channel channel)
    {
        // TODO: What if this fails?
        Channels.TryRemove(channel, out _);
    }

    public KeyValuePair<Channel, Member> GetChannelMemberInfo(Channel channel)
    {
        return Channels.FirstOrDefault(c => c.Key == channel);
    }

    public KeyValuePair<Channel, Member> GetChannelInfo(string Name)
    {
        return Channels.FirstOrDefault(c => c.Key.GetName() == Name);
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

        if (_dataRegulator.GetOutgoingBytes() <= 0) return;
        
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

    public void Disconnect(string message)
    {
        // Clean modes
        ModeOperations.Clear();

        Log.Info($"Disconnecting[{Protocol.GetType().Name}/{Name}]: {message}");
        _connection?.Disconnect($"{message}\r\n");
    }

    public DataRegulator DataRegulator => _dataRegulator;
    public IConnection GetConnection()
    {
        return _connection;
    }

    public void ChangeNickname(string? newNick, bool utf8Prefix)
    {
        var nickname = utf8Prefix ? $"'{newNick}" : newNick;
        var rawNicknameChange = Raws.RPL_NICK(Server, this, nickname);
        Send(rawNicknameChange);
        Nickname = nickname;

        Channels.Keys.ToList().ForEach( c => c.Send(rawNicknameChange, this));
    }

    public bool IsGuest()
    {
        return _guest;
    }

    public virtual bool Guest
    {
        set
        {
            Profile.Guest = value;
            if (Server.DisableGuestMode) return;
            _guest = value;
        }
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
        return SupportPackage is ANON;
    }

    public bool IsSysop()
    {
        return Oper;
    }

    public bool IsAdministrator()
    {
        return Admin;
    }

    public virtual void SetAway(string? message)
    {
        Profile.Away = true;
        Away = true;
        BroadcastToChannels(Raws.IRCX_RPL_USERNOWAWAY_822(Server, this, message), true);
        Send(Raws.IRCX_RPL_NOWAWAY_306(Server, this));
    }

    public virtual void SetBack()
    {
        Profile.Away = false;
        Away = false;
        BroadcastToChannels(Raws.IRCX_RPL_USERUNAWAY_821(Server, this), true);
        Send(Raws.IRCX_RPL_UNAWAY_305(Server, this));
    }

    public virtual void PromoteToAdministrator()
    {
        Profile.Level = EnumUserAccessLevel.Administrator;
        Admin = true;
        ModeRule.DispatchModeChange(Tokens.UserModeAdmin, this, this, true, ToString());
        Level = EnumUserAccessLevel.Administrator;
        Send(Raws.IRCX_RPL_YOUREADMIN_386(Server, this));
    }

    public virtual void PromoteToSysop()
    {
        Profile.Level = EnumUserAccessLevel.Sysop;
        Oper = true;
        ModeRule.DispatchModeChange(Tokens.UserModeOper, this, this, true, ToString());
        Level = EnumUserAccessLevel.Sysop;
        Send(Raws.IRCX_RPL_YOUREOPER_381(Server, this));
    }

    public virtual void PromoteToGuide()
    {
        Profile.Level = EnumUserAccessLevel.Guide;
        Oper = true;
        ModeRule.DispatchModeChange(Tokens.UserModeOper, this, this, true, ToString());
        Level = EnumUserAccessLevel.Guide;
        Send(Raws.IRCX_RPL_YOUREGUIDE_629(Server, this));
    }

    public bool DisconnectIfOutgoingThresholdExceeded()
    {
        if (DataRegulator.IsOutgoingThresholdExceeded())
        {
            DataRegulator.Purge();
            Disconnect("Output quota exceeded");
            return true;
        }

        return false;
    }

    public bool DisconnectIfIncomingThresholdExceeded()
    {
        // Disconnect user if incoming quota exceeded
        if (DataRegulator.IsIncomingThresholdExceeded())
        {
            DataRegulator.Purge();
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
                Send(Raws.RPL_PING(Server, this));
            }
            else
            {
                DataRegulator.Purge();
                Disconnect(Raws.IRCX_CLOSINGLINK_011_PINGTIMEOUT(Server, this, _connection.GetIp()));
            }
        }
    }

    public void Register()
    {
        var userAddress = Address;
        var credentials = SupportPackage.GetCredentials();
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

    public ChatFrame GetNextFrame()
    {
        _commandSequence++;
        var message = _dataRegulator.PopIncoming();
        return new ChatFrame(
            _commandSequence,
            Server,
            this,
            message
        );
    }

    public override bool CanBeModifiedBy(ChatObject source)
    {
        return source == this;
    }

    #region Modes

    public bool Oper
    {
        get => Convert.ToBoolean(Modes[Tokens.UserModeOper]);
        set => Modes[Tokens.UserModeOper] = Convert.ToInt32(value);
    }

    public bool Invisible
    {
        get => Convert.ToBoolean(Modes[Tokens.UserModeInvisible]);
        set => Modes[Tokens.UserModeOper] = Convert.ToInt32(value);
    }

    public bool Secure
    {
        get => Convert.ToBoolean(Modes[Tokens.UserModeSecure]);
        set => Modes[Tokens.UserModeSecure] = Convert.ToInt32(value);
    }

    public bool ServerNotice
    {
        get => Convert.ToBoolean(Modes[Tokens.UserModeServerNotice]);
        set => Modes[Tokens.UserModeServerNotice] = Convert.ToInt32(value);
    }

    public bool Wallops
    {
        get => Convert.ToBoolean(Modes[Tokens.UserModeWallops]);
        set => Modes[Tokens.UserModeWallops] = Convert.ToInt32(value);
    }

    public bool Admin
    {
        get => Convert.ToBoolean(Modes[Tokens.UserModeAdmin]);
        set => Modes[Tokens.UserModeAdmin] = Convert.ToInt32(value);
    }


    public bool Ircx
    {
        get => Convert.ToBoolean(Modes[Tokens.UserModeIrcx]);
        set => Modes[Tokens.UserModeIrcx] = Convert.ToInt32(value);
    }


    public bool Gag
    {
        get => Convert.ToBoolean(Modes[Tokens.UserModeGag]);
        set => Modes[Tokens.UserModeGag] = Convert.ToInt32(value);
    }


    public bool Host
    {
        get => Convert.ToBoolean(Modes[Tokens.UserModeHost]);
        set => Modes[Tokens.UserModeHost] = Convert.ToInt32(value);
    }

    #endregion
}