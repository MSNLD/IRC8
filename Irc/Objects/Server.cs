using System.Collections.Concurrent;
using System.Reflection;
using System.Text;
using Irc.Access;
using Irc.Commands;
using Irc.Enumerations;
using Irc.Extensions.Security.Packages;
using Irc.Interfaces;
using Irc.IO;
using Irc.Modes;
using Irc.Protocols;
using Irc.Resources;
using Irc.Security;
using Irc.Security.Credentials;
using Irc.Security.Packages;
using Irc.Security.Passport;
using NLog;
using Version = System.Version;

namespace Irc.Objects;

public class Server : ChatObject
{
    public static readonly NLog.Logger Log = LogManager.GetCurrentClassLogger();
    public static Dictionary<char, ModeRule> ModeRules = ServerModeRules.ModeRules;

    private readonly CancellationTokenSource _cancellationTokenSource = new();
    private readonly FloodProtectionManager _floodProtectionManager;
    private readonly PassportV4 _passport;
    private readonly Task _processingTask;
    private readonly SecurityManager _securityManager;
    private readonly ISocketServer _socketServer;
    private readonly ConcurrentQueue<User?> PendingNewUserQueue = new();
    private readonly ConcurrentQueue<User?> PendingRemoveUserQueue = new();
    public IDictionary<EnumProtocolType, Protocol> _protocols = new Dictionary<EnumProtocolType, Protocol>();
    public IList<Channel?> Channels;
    public IList<User?> Users = new List<User?>();

    public Server(ISocketServer socketServer,
        SecurityManager securityManager,
        FloodProtectionManager floodProtectionManager,
        Settings serverSettings,
        IList<Channel?> channels,
        ICredentialProvider? ntlmCredentialProvider)
    {
        Name = serverSettings.Name;
        Title = Name;
        _socketServer = socketServer;
        _securityManager = securityManager;
        _floodProtectionManager = floodProtectionManager;
        ServerSettings = serverSettings;
        Channels = channels;
        _processingTask = new Task(Process);
        _processingTask.Start();

        LoadSettingsFromDataStore();

        // TODO: Fix below
        // _dataStore.Set("supported.channel.modes",
        //     ChannelModes.Modes.Select(mode => mode.ToString()));
        // _dataStore.Set("supported.user.modes", UserModes.Modes.Select(mode => mode.ToString()));
        SupportPackages = ServerSettings.Packages;

        if (MaxAnonymousConnections > 0) _securityManager.AddSupportPackage(new ANON());
        if (SupportPackages.Contains("NTLM"))
            GetSecurityManager()
                .AddSupportPackage(new Ntlm(new NtlmProvider()));
        if (SupportPackages.Contains("GateKeeper"))
        {
            _passport = new PassportV4(serverSettings.PassportAppId, serverSettings.PassportAppSecret);
            securityManager.AddSupportPackage(new GateKeeper());
            securityManager.AddSupportPackage(new GateKeeperPassport(new PassportProvider(_passport)));
        }

        socketServer.OnClientConnecting += (sender, connection) =>
        {
            // TODO: Need to pass a Interfaced factory in to create the appropriate user
            // TODO: Need to start a new user out with protocol, below code is unreliable
            var user = CreateUser(connection);
            AddUser(user);

            connection.OnConnect += (o, integer) => { Log.Info("Connect"); };
            connection.OnReceive += (o, s) =>
            {
                //Console.WriteLine("OnRecv:" + s);
            };
            connection.OnDisconnect += (o, integer) => RemoveUser(user);
            connection.Accept();
        };
        socketServer.Listen();
    }

    public Settings ServerSettings { get; set; }
    public string[] SupportPackages { get; }

    public DateTime CreationDate => ServerSettings.Creation;

    // Server Properties To be moved to another class later
    public string? Title { get; private set; }
    public bool AnnonymousAllowed { get; }
    public int ChannelCount { get; }
    public IList<ChatObject> IgnoredUsers { get; }
    public IList<string> Info { get; }
    public int MaxMessageLength { get; } = 512;
    public int MaxInputBytes { get; private set; } = 512;
    public int MaxOutputBytes { get; private set; } = 4096;
    public int PingInterval { get; private set; } = 180;
    public int PingAttempts { get; private set; } = 3;
    public int MaxChannels { get; private set; } = 128;
    public int MaxConnections { get; private set; } = 10000;
    public int MaxAuthenticatedConnections { get; private set; } = 1000;
    public int MaxAnonymousConnections { get; private set; } = 1000;
    public int MaxGuestConnections { get; } = 1000;
    public bool BasicAuthentication { get; private set; } = true;
    public bool AnonymousConnections { get; private set; } = true;
    public int NetInvisibleCount { get; }
    public int NetServerCount { get; }
    public int NetUserCount { get; }
    public string SecurityPackages => _securityManager.GetSupportedPackages();
    public int SysopCount { get; }
    public int UnknownConnectionCount => _socketServer.CurrentConnections - NetUserCount;
    public string? RemoteIP { set; get; }
    public bool DisableGuestMode { set; get; }
    public bool DisableUserRegistration { get; set; }

    public Version ServerVersion { get; set; } = Assembly.GetExecutingAssembly().GetName().Version;

    public void SetMOTD(string motd)
    {
        var lines = motd.Split(new[] { '\n', '\r' }, StringSplitOptions.RemoveEmptyEntries);
        ServerSettings.Motd = lines;
    }

    public string[] GetMOTD()
    {
        return ServerSettings.Motd;
    }

    public void AddUser(User? user)
    {
        PendingNewUserQueue.Enqueue(user);
    }

    public void RemoveUser(User? user)
    {
        PendingRemoveUserQueue.Enqueue(user);
    }

    public void AddChannel(Channel channel)
    {
        Channels.Add(channel);
    }

    public void RemoveChannel(Channel channel)
    {
        Channels.Remove(channel);
    }

    public virtual Channel CreateChannel(string? name)
    {
        var channel = new Channel(name);
        return channel;
    }

    public virtual Channel CreateChannel(User? creator, string? name, string? key)
    {
        var channel = CreateChannel(name);
        channel.Props[IrcStrings.ChannelPropTopic] = name;
        // if (!string.IsNullOrEmpty(key))
        // {
        //     channel.Modes.Key = key;
        //     ChannelStore.Set("key", key);
        // }
        channel.Props[IrcStrings.ChannelPropOwnerkey] = key;
        channel.NoExtern = true;
        channel.TopicOp = true;
        channel.UserLimit = 50;
        AddChannel(channel);
        return channel;
    }

    public User? CreateUser(IConnection connection)
    {
        return new User(connection,
            new DataRegulator(MaxInputBytes, MaxOutputBytes),
            new FloodProtectionProfile(), this);
    }

    public IList<User?> GetUsers()
    {
        return Users;
    }


    public User? GetUserByNickname(string? nickname)
    {
        return Users.FirstOrDefault(user => string.Compare(user.GetAddress().Nickname.Trim(), nickname, true) == 0);
    }

    public User? GetUserByNickname(string? nickname, User? currentUser)
    {
        if (nickname.ToUpperInvariant() == currentUser.Name.ToUpperInvariant()) return currentUser;

        return GetUserByNickname(nickname);
    }

    public IList<User?> GetUsersByList(string nicknames, char separator)
    {
        List<string?> list = nicknames.Split(separator, StringSplitOptions.RemoveEmptyEntries).ToList();

        return GetUsersByList(list, separator);
    }

    public IList<User?> GetUsersByList(List<string?> nicknames, char separator)
    {
        return Users.Where(user =>
            nicknames.Contains(user.GetAddress().Nickname, StringComparer.InvariantCultureIgnoreCase)).ToList();
    }

    public IList<Channel?> GetChannels()
    {
        return Channels;
    }

    public string GetSupportedChannelModes()
    {
        // TODO: Fix below
        // return ServerSettings.Get("supported.channel.modes");
        return "";
    }

    public string GetSupportedUserModes()
    {
        // return ServerSettings.Get("supported.user.modes");
        return "";
    }

    public Channel GetChannelByName(string? name)
    {
        return Channels.SingleOrDefault(c =>
            string.Equals(c.GetName(), name, StringComparison.InvariantCultureIgnoreCase));
    }

    public ChatObject GetChatObject(string? name)
    {
        if (string.IsNullOrWhiteSpace(name)) return null;

        switch (name.Substring(0, 1))
        {
            case "*":
            case "$":
            {
                return this;
            }
            case "%":
            case "#":
            case "&":
                return GetChannelByName(name);
            default:
            {
                return (ChatObject)GetUserByNickname(name);
            }
        }
    }

    public Protocol GetProtocol(EnumProtocolType protocolType)
    {
        if (_protocols.TryGetValue(protocolType, out var protocol)) return protocol;
        return null;
    }

    public SecurityManager GetSecurityManager()
    {
        return _securityManager;
    }

    public ICredentialProvider? GetCredentialManager()
    {
        return null;
    }

    public void Shutdown()
    {
        _cancellationTokenSource.Cancel();
        _processingTask.Wait();
    }

    public override void Send(string message)
    {
        throw new NotImplementedException();
    }

    public override void Send(string message, ChatObject except = null)
    {
        throw new NotImplementedException();
    }

    public override void Send(string message, EnumChannelAccessLevel accessLevel)
    {
        throw new NotImplementedException();
    }

    public override bool CanBeModifiedBy(ChatObject source)
    {
        throw new NotImplementedException();
    }

    public override string? ToString()
    {
        return Name;
    }

    public void ProcessCookie(User user, string? name, string value)
    {
        if (name == IrcStrings.UserPropMsnRegCookie && user.IsAuthenticated() && !user.IsRegistered())
        {
            var nickname = _passport.ValidateRegCookie(value);
            if (nickname != null)
            {
                var encodedNickname = Encoding.Latin1.GetString(Encoding.UTF8.GetBytes(nickname));
                user.Nickname = encodedNickname;

                // Set the RealName to empty string to allow it to pass register
                user.GetAddress().RealName = string.Empty;
            }
        }
        else if (name == IrcStrings.UserPropSubscriberInfo && user.IsAuthenticated() && user.IsRegistered())
        {
            var subscribedString =
                _passport.ValidateSubscriberInfo(value, user.GetSupportPackage().GetCredentials().GetIssuedAt());
            int.TryParse(subscribedString, out var subscribed);
            if ((subscribed & 1) == 1) user.GetProfile().Registered = true;
        }
        else if (name == IrcStrings.UserPropMsnProfile && user.IsAuthenticated() && !user.IsRegistered())
        {
            int.TryParse(value, out var profileCode);
            user.GetProfile().SetProfileCode(profileCode);
        }
        else if (name == IrcStrings.UserPropRole && user.IsAuthenticated())
        {
            var dict = _passport.ValidateRole(value);
            if (dict == null) return;

            if (dict.ContainsKey("umode"))
            {
                var modes = dict["umode"];
                foreach (var mode in modes)
                {
                    // TODO: Make this work
                    // var modeRule = user.GetModes().GetMode(mode);
                    // modeRule?.Set(1);
                    // modeRule?.DispatchModeChange((ChatObject)user, (ChatObject)user, true);
                }
            }

            if (dict.ContainsKey("utype"))
            {
                var levelType = dict["utype"];

                switch (levelType)
                {
                    case "A":
                    {
                        user.ChangeNickname(user.Nickname, true);
                        user.PromoteToAdministrator();
                        break;
                    }
                    case "S":
                    {
                        user.ChangeNickname(user.Nickname, true);
                        user.PromoteToSysop();
                        break;
                    }
                    case "G":
                    {
                        user.ChangeNickname(user.Nickname, true);
                        user.PromoteToGuide();
                        break;
                    }
                }
            }
        }
    }

    public void LoadSettingsFromDataStore()
    {
        var title = ServerSettings.Title;
        var maxInputBytes = ServerSettings.MaxInputBytes;
        var maxOutputBytes = ServerSettings.MaxOutputBytes;
        var pingInterval = ServerSettings.PingInterval;
        var pingAttempts = ServerSettings.PingAttempts;
        var maxChannels = ServerSettings.MaxChannels;
        var maxConnections = ServerSettings.MaxConnections;
        var maxAuthenticatedConnections = ServerSettings.MaxAuthenticatedConnections;
        var maxAnonymousConnections = ServerSettings.MaxAnonymousConnections;
        var basicAuthentication = ServerSettings.BasicAuthentication;
        var anonymousConnections = ServerSettings.AnonymousConnections;

        if (!string.IsNullOrWhiteSpace(title)) Title = title;
        if (maxInputBytes > 0) MaxInputBytes = maxInputBytes;
        if (maxOutputBytes > 0) MaxOutputBytes = maxOutputBytes;
        if (pingInterval > 0) PingInterval = pingInterval;
        if (pingAttempts > 0) PingAttempts = pingAttempts;
        if (maxChannels > 0) MaxChannels = maxChannels;
        if (maxConnections > 0) MaxConnections = maxConnections;
        if (maxAuthenticatedConnections > 0) MaxAuthenticatedConnections = maxAuthenticatedConnections;
        MaxAnonymousConnections = maxAnonymousConnections;
        BasicAuthentication = basicAuthentication;
        AnonymousConnections = anonymousConnections;
    }

    private void Process()
    {
        var backoffMs = 0;
        while (!_cancellationTokenSource.IsCancellationRequested)
        {
            var hasWork = false;

            RemovePendingUsers();
            AddPendingUsers();

            // do stuff
            foreach (var user in Users)
            {
                if (user.DisconnectIfIncomingThresholdExceeded()) continue;

                if (user.GetDataRegulator().GetIncomingBytes() > 0)
                {
                    hasWork = true;
                    backoffMs = 0;

                    ProcessNextCommand(user);
                }

                ProcessNextModeOperation(user);

                if (!user.DisconnectIfOutgoingThresholdExceeded()) user.Flush();
                user.DisconnectIfInactive();
            }

            if (!hasWork)
            {
                if (backoffMs < 1000) backoffMs += 10;
                Thread.Sleep(backoffMs);
            }
        }
    }

    private void AddPendingUsers()
    {
        if (PendingNewUserQueue.Count > 0)
        {
            // add new pending users
            foreach (var user in PendingNewUserQueue)
            {
                user.Props[IrcStrings.UserPropOid] = "0";
                Users.Add(user);
            }

            Log.Debug($"Added {PendingNewUserQueue.Count} users. Total Users = {Users.Count}");
            PendingNewUserQueue.Clear();
        }
    }

    private void RemovePendingUsers()
    {
        if (PendingRemoveUserQueue.Count > 0)
        {
            // remove pending to be removed users

            foreach (var user in PendingRemoveUserQueue)
            {
                if (!Users.Remove(user))
                {
                    Log.Error($"Failed to remove {user}. Requeueing");
                    PendingRemoveUserQueue.Enqueue(user);
                    continue;
                }

                Quit.QuitChannels(user, "Connection reset by peer");
            }

            Log.Debug($"Removed {PendingRemoveUserQueue.Count} users. Total Users = {Users.Count}");
            PendingRemoveUserQueue.Clear();
        }
    }

    protected void AddCommand(Command command, EnumProtocolType fromProtocol = EnumProtocolType.IRC,
        string? name = null)
    {
        foreach (var protocol in _protocols)
            if (protocol.Key >= fromProtocol)
                protocol.Value.AddCommand(command, name);
    }

    protected void AddProtocol(EnumProtocolType protocolType, Protocol protocol, bool inheritCommands = true)
    {
        if (inheritCommands)
            for (var protocolIndex = 0; protocolIndex < (int)protocolType; protocolIndex++)
                if (_protocols.ContainsKey((EnumProtocolType)protocolIndex))
                    foreach (var command in _protocols[(EnumProtocolType)protocolIndex].GetCommands())
                        protocol.AddCommand(command.Value, command.Key);
        _protocols.Add(protocolType, protocol);
    }

    protected void FlushCommands()
    {
        foreach (var protocol in _protocols) protocol.Value.FlushCommands();
    }

    private void ProcessNextModeOperation(User? user)
    {
        var modeOperations = user.GetModeOperations();
        if (modeOperations.Count > 0) modeOperations.Dequeue().Execute();
    }

    // Ircx
    protected EnumChannelAccessResult CheckAuthOnly()
    {
        if (Modes[IrcStrings.ChannelModeAuthOnly] == 1)
            return EnumChannelAccessResult.ERR_AUTHONLYCHAN;
        return EnumChannelAccessResult.NONE;
    }

    protected EnumChannelAccessResult CheckSecureOnly()
    {
        // TODO: Whatever this is...
        return EnumChannelAccessResult.ERR_SECUREONLYCHAN;
    }

    private void ProcessNextCommand(User? user)
    {
        var message = user.GetDataRegulator().PeekIncoming();
        if (message == null) return;

        var command = message.GetCommand();
        if (command != null)
        {
            var floodResult = _floodProtectionManager.Audit(user.GetFloodProtectionProfile(),
                command.GetDataType(), user.GetLevel());
            if (floodResult == EnumFloodResult.Ok)
            {
                if (command is not Ping && command is not Pong) user.LastIdle = DateTime.UtcNow;

                Log.Trace($"Processing: {message.OriginalText}");

                var chatFrame = user.GetNextFrame();
                if (!command.RegistrationNeeded(chatFrame) && command.ParametersAreValid(chatFrame))
                    try
                    {
                        command.Execute(chatFrame);
                    }
                    catch (Exception e)
                    {
                        chatFrame.User.Send(
                            IrcRaws.IRC_RAW_999(chatFrame.Server, chatFrame.User, IrcStrings.ServerError));
                        Log.Error(e.ToString());
                    }

                // Check if user can register
                if (!chatFrame.User.IsRegistered()) Register.TryRegister(chatFrame);
            }
        }
        else
        {
            user.GetDataRegulator().PopIncoming();
            user.Send(Raw.IRCX_ERR_UNKNOWNCOMMAND_421(this, user, message.GetCommandName()));
            // command not found
        }
    }
}