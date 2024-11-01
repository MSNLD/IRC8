using System.Text.RegularExpressions;
using Irc.Access;
using Irc.Commands;
using Irc.Enumerations;
using Irc.Interfaces;
using Irc.Modes;
using Irc.Resources;

namespace Irc.Objects.Channel;

public class Channel : ChatObject, IChannel
{
    private readonly ChannelAccess _accessList = new();
    protected readonly IList<IChannelMember?> _members = new List<IChannelMember?>();
    public HashSet<string> BanList = new();
    public HashSet<string> InviteList = new();
    public new static Dictionary<char, IModeRule> ModeRules = ChannelModeRules.ModeRules;

    public Channel(string? name)
    {
        SetName(name);
        Props["NAME"] = name;
        Props[IrcStrings.ChannelPropOID] = "";
        Props[IrcStrings.ChannelPropCreation] = "";
        Props[IrcStrings.ChannelPropLanguage] = "";
        Props[IrcStrings.ChannelPropOwnerkey] = "";
        Props[IrcStrings.ChannelPropHostkey] = "";
        Props[IrcStrings.ChannelPropPICS] = "";
        Props[IrcStrings.ChannelPropTopic] = "";
        Props[IrcStrings.ChannelPropSubject] = "";
        Props[IrcStrings.ChannelPropOnJoin] = "";
        Props[IrcStrings.ChannelPropOnPart] = "";
        Props[IrcStrings.ChannelPropLag] = "";
        Props[IrcStrings.ChannelPropClient] = "";
        Props[IrcStrings.ChannelPropClientGuid] = "";
        Props[IrcStrings.ChannelPropServicePath] = "";
        
        // TODO: Add Modes
        Modes[Resources.IrcStrings.ChannelModeInvite] = 0;
        Modes[Resources.IrcStrings.ChannelModeKey] = 0;
        Modes[Resources.IrcStrings.ChannelModeModerated] = 0;
        Modes[Resources.IrcStrings.ChannelModeNoExtern] = 0;
        Modes[Resources.IrcStrings.ChannelModePrivate] = 0;
        Modes[Resources.IrcStrings.ChannelModeSecret] = 0;
        Modes[Resources.IrcStrings.ChannelModeHidden] = 0;
        Modes[Resources.IrcStrings.ChannelModeTopicOp] = 0;
        Modes[Resources.IrcStrings.ChannelModeUserLimit] = 0;
        Modes[Resources.IrcStrings.ChannelModeAuthOnly] = 0;
        Modes[Resources.IrcStrings.ChannelModeProfanity] = 0;
        Modes[Resources.IrcStrings.ChannelModeRegistered] = 0;
        Modes[Resources.IrcStrings.ChannelModeKnock] = 0;
        Modes[Resources.IrcStrings.ChannelModeNoWhisper] = 0;
        Modes[Resources.IrcStrings.ChannelModeNoGuestWhisper] = 0;
        Modes[Resources.IrcStrings.ChannelModeCloneable] = 0;
        Modes[Resources.IrcStrings.ChannelModeClone] = 0;
        Modes[Resources.IrcStrings.ChannelModeService] = 0;
    }

    #region Modes
    // IRC
    public bool InviteOnly
    {
        get => Convert.ToBoolean(Modes[Resources.IrcStrings.ChannelModeInvite]); 
        set => Modes[Resources.IrcStrings.ChannelModeInvite] = Convert.ToInt32(value);
    }

    public bool Key
    {
        get => Convert.ToBoolean(Modes[Resources.IrcStrings.ChannelModeKey]); 
        set => Modes[Resources.IrcStrings.ChannelModeKey] = Convert.ToInt32(value);
    }

    public bool Moderated
    {
        get => Convert.ToBoolean(Modes[Resources.IrcStrings.ChannelModeModerated]); 
        set => Modes[Resources.IrcStrings.ChannelModeModerated] = Convert.ToInt32(value);
    }

    public bool NoExtern
    {
        get => Convert.ToBoolean(Modes[Resources.IrcStrings.ChannelModeNoExtern]); 
        set => Modes[Resources.IrcStrings.ChannelModeNoExtern] = Convert.ToInt32(value);
    }

    public bool Private
    {
        get => Convert.ToBoolean(Modes[Resources.IrcStrings.ChannelModePrivate]); 
        set => Modes[Resources.IrcStrings.ChannelModePrivate] = Convert.ToInt32(value);
    }

    public bool Secret
    {
        get => Convert.ToBoolean(Modes[Resources.IrcStrings.ChannelModeSecret]); 
        set => Modes[Resources.IrcStrings.ChannelModeSecret] = Convert.ToInt32(value);
    }

    public bool Hidden
    {
        get => Convert.ToBoolean(Modes[Resources.IrcStrings.ChannelModeHidden]); 
        set => Modes[Resources.IrcStrings.ChannelModeHidden] = Convert.ToInt32(value);
    }

    public bool TopicOp
    {
        get => Convert.ToBoolean(Modes[Resources.IrcStrings.ChannelModeTopicOp]); 
        set => Modes[Resources.IrcStrings.ChannelModeTopicOp] = Convert.ToInt32(value);
    }

    public int UserLimit
    {
        get => Modes[Resources.IrcStrings.ChannelModeUserLimit]; 
        set => Modes[Resources.IrcStrings.ChannelModeUserLimit] = value;
    }

    //IRCX
    public bool AuthOnly
    {
        get => Convert.ToBoolean(Modes[Resources.IrcStrings.ChannelModeAuthOnly]); 
        set => Modes[Resources.IrcStrings.ChannelModeAuthOnly] = Convert.ToInt32(value);
    }

    public bool Profanity
    {
        get => Convert.ToBoolean(Modes[Resources.IrcStrings.ChannelModeProfanity]); 
        set => Modes[Resources.IrcStrings.ChannelModeProfanity] = Convert.ToInt32(value);
    }

    public bool Registered
    {
        get => Convert.ToBoolean(Modes[Resources.IrcStrings.ChannelModeRegistered]); 
        set => Modes[Resources.IrcStrings.ChannelModeRegistered] = Convert.ToInt32(value);
    }

    public bool Knock
    {
        get => Convert.ToBoolean(Modes[Resources.IrcStrings.ChannelModeKnock]); 
        set => Modes[Resources.IrcStrings.ChannelModeKnock] = Convert.ToInt32(value);
    }

    public bool NoWhisper
    {
        get => Convert.ToBoolean(Modes[Resources.IrcStrings.ChannelModeNoWhisper]); 
        set => Modes[Resources.IrcStrings.ChannelModeNoWhisper] = Convert.ToInt32(value);
    }

    public bool NoGuestWhisper
    {
        get => Convert.ToBoolean(Modes[Resources.IrcStrings.ChannelModeNoGuestWhisper]); 
        set => Modes[Resources.IrcStrings.ChannelModeNoGuestWhisper] = Convert.ToInt32(value);
    }

    public bool Cloneable
    {
        get => Convert.ToBoolean(Modes[Resources.IrcStrings.ChannelModeCloneable]); 
        set => Modes[Resources.IrcStrings.ChannelModeCloneable] = Convert.ToInt32(value);
    }

    public bool Clone
    {
        get => Convert.ToBoolean(Modes[Resources.IrcStrings.ChannelModeClone]); 
        set => Modes[Resources.IrcStrings.ChannelModeClone] = Convert.ToInt32(value);
    }

    public bool Service
    {
        get => Convert.ToBoolean(Modes[Resources.IrcStrings.ChannelModeService]); 
        set => Modes[Resources.IrcStrings.ChannelModeService] = Convert.ToInt32(value);
    }
    #endregion

    public string? GetName()
    {
        return Name;
    }

    public IChannelMember? GetMember(IUser? user)
    {
        foreach (var channelMember in _members)
            if (channelMember?.GetUser() == user)
                return channelMember;

        return null;
    }

    public IChannelMember? GetMemberByNickname(string? nickname)
    {
        return _members.FirstOrDefault(member =>
            String.Compare(member?.GetUser().GetAddress().Nickname, nickname, StringComparison.OrdinalIgnoreCase) == 0);
    }

    public bool Allows(IUser? user)
    {
        if (HasUser(user)) return false;
        return true;
    }

    public virtual IChannel Join(IUser? user, EnumChannelAccessResult accessResult = EnumChannelAccessResult.NONE)
    {
        var joinMember = AddMember(user, accessResult);
        foreach (var channelMember in GetMembers())
        {
            var channelUser = channelMember.GetUser();
            if (channelUser.Protocol.Ircvers <= EnumProtocolType.IRC3)
            {
                channelMember.GetUser().Send(IrcRaws.RPL_JOIN(user, this));

                if (!joinMember.IsNormal())
                {
                    var modeChar = joinMember.IsOwner() ? 'q' : joinMember.IsHost() ? 'o' : 'v';
                    ModeRule.DispatchModeChange((ChatObject)channelUser, modeChar,
                        (ChatObject)user, this, true, user.ToString());
                }
            }
            else
            {
                channelUser.Send(IrcxRaws.RPL_JOIN_MSN(channelMember, this, joinMember));
            }
        }

        return this;
    }


    public IChannel SendTopic(IUser? user)
    {
        user.Send(Raw.IRCX_RPL_TOPIC_332(user.Server, user, this, Props[Resources.IrcStrings.ChannelPropTopic] ?? string.Empty));
        return this;
    }

    public IChannel SendTopic()
    {
        _members.ToList().ForEach(member => SendTopic(member.GetUser()));
        return this;
    }

    public IChannel SendNames(IUser? user)
    {
        Names.ProcessNamesReply(user, this);
        return this;
    }

    public IChannel Part(IUser? user)
    {
        Send(IrcRaws.RPL_PART(user, this));
        RemoveMember(user);
        return this;
    }

    public IChannel Quit(IUser? user)
    {
        RemoveMember(user);
        return this;
    }

    public IChannel Kick(IUser? source, IUser? target, string? reason)
    {
        Send(Raw.RPL_KICK_IRC(source, this, target, reason));
        RemoveMember(target);
        return this;
    }

    public void SendMessage(IUser? user, string? message)
    {
        Send(IrcRaws.RPL_PRIVMSG(user, this, message), (ChatObject)user);
    }

    public void SendNotice(IUser? user, string? message)
    {
        Send(IrcRaws.RPL_NOTICE(user, this, message), (ChatObject)user);
    }

    public IList<IChannelMember?> GetMembers()
    {
        return _members;
    }

    public bool HasUser(IUser? user)
    {
        foreach (var member in _members)
            // TODO: Re-enable below
            //if (CompareUserAddress(user, member.GetUser())) return true;
            if (CompareUserNickname(member.GetUser(), user) || CompareUserAddress(user, member.GetUser()))
                return true;

        return false;
    }

    public new bool CanBeModifiedBy(IChatObject source)
    {
        return source is IServer || ((IUser)source).GetChannels().Keys.Contains(this);
    }

    public EnumIrcError CanModifyMember(IChannelMember? source, IChannelMember target,
        EnumChannelAccessLevel requiredLevel)
    {
        // Oper check
        if (target.GetUser().GetLevel() >= EnumUserAccessLevel.Guide)
        {
            if (source.GetUser().GetLevel() < EnumUserAccessLevel.Guide) return EnumIrcError.ERR_NOIRCOP;
            // TODO: Maybe there is better raws for below
            if (source.GetUser().GetLevel() < EnumUserAccessLevel.Sysop &&
                source.GetUser().GetLevel() < target.GetUser().GetLevel()) return EnumIrcError.ERR_NOPERMS;
            if (source.GetUser().GetLevel() < EnumUserAccessLevel.Administrator &&
                source.GetUser().GetLevel() < target.GetUser().GetLevel()) return EnumIrcError.ERR_NOPERMS;
        }

        if (source.GetLevel() >= requiredLevel && source.GetLevel() >= target.GetLevel())
            return EnumIrcError.OK;
        if (!source.IsOwner() && (requiredLevel >= EnumChannelAccessLevel.ChatOwner ||
                                  target.GetLevel() >= EnumChannelAccessLevel.ChatOwner))
            return EnumIrcError.ERR_NOCHANOWNER;
        return EnumIrcError.ERR_NOCHANOP;
    }

    public void ProcessChannelError(EnumIrcError error, IServer server, IUser? source, ChatObject? target = null,
        string? data = null)
    {
        switch (error)
        {
            case EnumIrcError.ERR_NEEDMOREPARAMS:
            {
                // -> sky-8a15b323126 MODE #test +l hello
                // < - :sky - 8a15b323126 461 Sky MODE +l :Not enough parameters
                source.Send(Raw.IRCX_ERR_NEEDMOREPARAMS_461(server, source, data));
                break;
            }
            case EnumIrcError.ERR_NOCHANOP:
            {
                //:sky-8a15b323126 482 Sky3k #test :You're not channel operator
                source.Send(Raw.IRCX_ERR_CHANOPRIVSNEEDED_482(server, source, this));
                break;
            }
            case EnumIrcError.ERR_NOCHANOWNER:
            {
                //:sky-8a15b323126 482 Sky3k #test :You're not channel operator
                source.Send(Raw.IRCX_ERR_CHANQPRIVSNEEDED_485(server, source, this));
                break;
            }
            case EnumIrcError.ERR_NOIRCOP:
            {
                source.Send(Raw.IRCX_ERR_NOPRIVILEGES_481(server, source));
                break;
            }
            case EnumIrcError.ERR_NOTONCHANNEL:
            {
                source.Send(Raw.IRCX_ERR_NOTONCHANNEL_442(server, source, this));
                break;
            }
            // TODO: The below should not happen
            case EnumIrcError.ERR_NOSUCHNICK:
            {
                source.Send(Raw.IRCX_ERR_NOSUCHNICK_401(server, source, target.Name));
                break;
            }
            case EnumIrcError.ERR_NOSUCHCHANNEL:
            {
                source.Send(Raw.IRCX_ERR_NOSUCHCHANNEL_403(server, source, Name));
                break;
            }
            case EnumIrcError.ERR_CANNOTSETFOROTHER:
            {
                source.Send(Raw.IRCX_ERR_USERSDONTMATCH_502(server, source));
                break;
            }
            case EnumIrcError.ERR_UNKNOWNMODEFLAG:
            {
                source.Send(IrcRaws.IRC_RAW_501(server, source));
                break;
            }
            case EnumIrcError.ERR_NOPERMS:
            {
                source.Send(Raw.IRCX_ERR_SECURITY_908(server, source));
                break;
            }
        }
    }

    public override void Send(string message)
    {
        Send(message, null);
    }

    public override void Send(string message, ChatObject? u = null)
    {
        foreach (var channelMember in _members)
            if (channelMember.GetUser() != u)
                channelMember.GetUser().Send(message);
    }

    public override void Send(string message, EnumChannelAccessLevel accessLevel)
    {
        foreach (var channelMember in _members)
            if (channelMember.GetLevel() >= accessLevel)
                channelMember.GetUser().Send(message);
    }

    public virtual EnumChannelAccessResult GetAccess(IUser? user, string? key, bool IsGoto = false)
    {
        var hostKeyCheck = CheckHostKey(user, key);

        var accessLevel = GetChannelAccess(user);
        var accessResult = EnumChannelAccessResult.NONE;

        switch (accessLevel)
        {
            case EnumAccessLevel.OWNER:
            {
                accessResult = EnumChannelAccessResult.SUCCESS_OWNER;
                break;
            }
            case EnumAccessLevel.HOST:
            {
                accessResult = EnumChannelAccessResult.SUCCESS_HOST;
                break;
            }
            case EnumAccessLevel.VOICE:
            {
                accessResult = EnumChannelAccessResult.SUCCESS_VOICE;
                break;
            }
            case EnumAccessLevel.GRANT:
            {
                accessResult = EnumChannelAccessResult.SUCCESS_MEMBER;
                break;
            }
            case EnumAccessLevel.DENY:
            {
                accessResult = EnumChannelAccessResult.ERR_BANNEDFROMCHAN;
                break;
            }
        }

        var accessPermissions = (EnumChannelAccessResult)new[]
        {
            (int)GetAccessEx(user, key, IsGoto),
            (int)hostKeyCheck,
            (int)accessResult
        }.Max();

        return accessPermissions == EnumChannelAccessResult.NONE
            ? EnumChannelAccessResult.SUCCESS_GUEST
            : accessPermissions;
    }

    public virtual bool InviteMember(IUser user)
    {
        var address = user.GetAddress().GetAddress();
        return InviteList.Add(address);
    }

    public virtual bool BanMask(Address address)
    {
        var formattedAddress = address.GetAddress();
        return BanList.Add(formattedAddress);
    }

    public virtual bool UnbanMask(Address address)
    {
        var formattedAddress = address.GetAddress();
        return BanList.Remove(formattedAddress);
    }

    protected virtual IChannelMember AddMember(IUser? user,
        EnumChannelAccessResult accessResult = EnumChannelAccessResult.NONE)
    {
        var member = new Member.Member(user);

        if (accessResult == EnumChannelAccessResult.SUCCESS_OWNER) member.SetHost(true);
        else if (accessResult == EnumChannelAccessResult.SUCCESS_HOST) member.SetHost(true);
        else if (accessResult == EnumChannelAccessResult.SUCCESS_VOICE) member.SetVoice(true);

        _members.Add(member);
        user.AddChannel(this, member);
        return member;
    }

    private void RemoveMember(IUser? user)
    {
        var member = _members.Where(m => m.GetUser() == user).FirstOrDefault();
        _members.Remove(member);
        user.RemoveChannel(this);
    }

    public void SetName(string? Name)
    {
        this.Name = Name;
    }

    private static bool CompareUserAddress(IUser? user, IUser? otherUser)
    {
        if (otherUser == user || otherUser.GetAddress().UserHost == user.GetAddress().UserHost) return true;
        return false;
    }

    private static bool CompareUserNickname(IUser? user, IUser? otherUser)
    {
        return otherUser.GetAddress().Nickname.ToUpper() == user.GetAddress().Nickname.ToUpper();
    }

    public static bool ValidName(string? channel)
    {
        var regex = new Regex(IrcStrings.IrcxChannelRegex);
        return regex.Match(channel).Success;
    }

    public EnumChannelAccessResult GetAccessEx(IUser? user, string? key, bool IsGoto = false)
    {
        var operCheck = CheckOper(user);
        var keyCheck = CheckMemberKey(user, key);
        var inviteOnlyCheck = CheckInviteOnly(user);
        var userLimitCheck = CheckUserLimit(IsGoto);

        var accessPermissions = (EnumChannelAccessResult)new[]
        {
            (int)operCheck,
            (int)keyCheck,
            (int)inviteOnlyCheck,
            (int)userLimitCheck
        }.Max();

        return accessPermissions;
    }

    protected EnumChannelAccessResult CheckOper(IUser? user)
    {
        if (user.GetLevel() >= EnumUserAccessLevel.Guide) return EnumChannelAccessResult.SUCCESS_OWNER;
        return EnumChannelAccessResult.NONE;
    }

    protected EnumChannelAccessResult CheckMemberKey(IUser? user, string? key)
    {
        if (string.IsNullOrWhiteSpace(key)) return EnumChannelAccessResult.NONE;

        if (Modes[IrcStrings.ChannelModeKey] == 1)
        {
            if (Props[IrcStrings.ChannelPropMemberkey] == key)
                return EnumChannelAccessResult.SUCCESS_MEMBER;
            return EnumChannelAccessResult.ERR_BADCHANNELKEY;
        }

        return EnumChannelAccessResult.NONE;
    }

    protected EnumChannelAccessResult CheckInviteOnly(IUser? user)
    {
        if (Modes[IrcStrings.ChannelModeInvite] == 1)
            return InviteList.Contains(user.GetAddress().GetAddress())
                ? EnumChannelAccessResult.SUCCESS_MEMBER
                : EnumChannelAccessResult.ERR_INVITEONLYCHAN;

        return EnumChannelAccessResult.NONE;
    }

    protected EnumChannelAccessResult CheckUserLimit(bool IsGoto)
    {
        var modeLimit = Modes[IrcStrings.ChannelModeUserLimit];
        var serverLimit = 10000; //TODO: Change later
        var userLimit = modeLimit > 0 ? modeLimit : serverLimit;

        if (IsGoto) userLimit = (int)Math.Ceiling(userLimit * 1.2);

        if (GetMembers().Count >= userLimit) return EnumChannelAccessResult.ERR_CHANNELISFULL;
        return EnumChannelAccessResult.NONE;
    }

    public EnumAccessLevel GetChannelAccess(IUser? user)
    {
        var userAccessLevel = EnumAccessLevel.NONE;
        var addressString = user.GetAddress().GetFullAddress();
        var accessEntries = AccessList.GetEntries();

        foreach (var accessKvp in accessEntries)
        {
            var accessLevel = accessKvp.Key;
            var accessList = accessKvp.Value;

            foreach (var accessEntry in accessList)
            {
                var maskAddress = accessEntry.Mask;

                var regExStr = maskAddress.Replace("*", ".*").Replace("?", ".");
                var regEx = new Regex(regExStr, RegexOptions.IgnoreCase);
                if (regEx.Match(addressString).Success)
                    if ((int)accessLevel > (int)userAccessLevel)
                        userAccessLevel = accessLevel;
            }
        }

        return userAccessLevel;
    }


    protected EnumChannelAccessResult CheckHostKey(IUser? user, string? key)
    {
        if (string.IsNullOrWhiteSpace(key)) return EnumChannelAccessResult.NONE;

        if (Props["OWNERKEY"] == key)
            return EnumChannelAccessResult.SUCCESS_OWNER;
        if (Props["HOSTKEY"] == key) return EnumChannelAccessResult.SUCCESS_HOST;
        return EnumChannelAccessResult.NONE;
    }
}