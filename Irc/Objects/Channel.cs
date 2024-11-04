using System.Text.RegularExpressions;
using Irc.Commands;
using Irc.Enumerations;
using Irc.Modes;
using Irc.Resources;

namespace Irc.Objects;

public class Channel : ChatObject
{
    public static Dictionary<char, ModeRule> ModeRules = ChannelModeRules.ModeRules;
    protected readonly IList<Member?> _members = new List<Member?>();
    public HashSet<string> BanList = new();
    public HashSet<string> InviteList = new();

    public Channel(string? name)
    {
        SetName(name);
        Props["NAME"] = name;
        Props[IrcStrings.ChannelPropOID] = "";
        Props[IrcStrings.ChannelPropCreation] = "";
        Props[IrcStrings.ChannelPropLanguage] = "";
        Props[IrcStrings.ChannelPropMemberkey] = "";
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
        Modes[IrcStrings.ChannelModeInvite] = 0;
        Modes[IrcStrings.ChannelModeKey] = 0;
        Modes[IrcStrings.ChannelModeModerated] = 0;
        Modes[IrcStrings.ChannelModeNoExtern] = 0;
        Modes[IrcStrings.ChannelModePrivate] = 0;
        Modes[IrcStrings.ChannelModeSecret] = 0;
        Modes[IrcStrings.ChannelModeHidden] = 0;
        Modes[IrcStrings.ChannelModeTopicOp] = 0;
        Modes[IrcStrings.ChannelModeUserLimit] = 0;
        Modes[IrcStrings.ChannelModeAuthOnly] = 0;
        Modes[IrcStrings.ChannelModeProfanity] = 0;
        Modes[IrcStrings.ChannelModeRegistered] = 0;
        Modes[IrcStrings.ChannelModeKnock] = 0;
        Modes[IrcStrings.ChannelModeNoWhisper] = 0;
        Modes[IrcStrings.ChannelModeNoGuestWhisper] = 0;
        Modes[IrcStrings.ChannelModeCloneable] = 0;
        Modes[IrcStrings.ChannelModeClone] = 0;
        Modes[IrcStrings.ChannelModeService] = 0;
    }

    public string? GetName()
    {
        return Name;
    }

    public Member? GetMember(User? user)
    {
        foreach (var channelMember in _members)
            if (channelMember?.GetUser() == user)
                return channelMember;

        return null;
    }

    public Member? GetMemberByNickname(string? nickname)
    {
        return _members.FirstOrDefault(member =>
            string.Compare((member?.GetUser().Address).Nickname, nickname, StringComparison.OrdinalIgnoreCase) == 0);
    }

    public bool Allows(User? user)
    {
        if (HasUser(user)) return false;
        return true;
    }

    public virtual Channel Join(User? user, EnumChannelAccessResult accessResult = EnumChannelAccessResult.NONE)
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
                    ModeRule.DispatchModeChange(channelUser, modeChar,
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


    public Channel SendTopic(User? user)
    {
        user.Send(Raw.IRCX_RPL_TOPIC_332(user.Server, user, this, Props[IrcStrings.ChannelPropTopic] ?? string.Empty));
        return this;
    }

    public Channel SendTopic()
    {
        _members.ToList().ForEach(member => SendTopic(member.GetUser()));
        return this;
    }

    public Channel SendNames(User? user)
    {
        Names.ProcessNamesReply(user, this);
        return this;
    }

    public Channel Part(User? user)
    {
        Send(IrcRaws.RPL_PART(user, this));
        RemoveMember(user);
        return this;
    }

    public Channel Quit(User? user)
    {
        RemoveMember(user);
        return this;
    }

    public Channel Kick(User? source, User? target, string? reason)
    {
        Send(Raw.RPL_KICK_IRC(source, this, target, reason));
        RemoveMember(target);
        return this;
    }

    public void SendMessage(User? user, string? message)
    {
        Send(IrcRaws.RPL_PRIVMSG(user, this, message), (ChatObject)user);
    }

    public void SendNotice(User? user, string? message)
    {
        Send(IrcRaws.RPL_NOTICE(user, this, message), (ChatObject)user);
    }

    public IList<Member?> GetMembers()
    {
        return _members;
    }

    public bool HasUser(User? user)
    {
        foreach (var member in _members)
            // TODO: Re-enable below
            //if (CompareUserAddress(user, member.GetUser())) return true;
            if (CompareUserNickname(member.GetUser(), user) || CompareUserAddress(user, member.GetUser()))
                return true;

        return false;
    }

    public override bool CanBeModifiedBy(ChatObject source)
    {
        return source is Server || ((User)source).Channels.Keys.Contains(this);
    }

    public EnumIrcError CanModifyMember(Member? source, Member target,
        EnumChannelAccessLevel requiredLevel)
    {
        // Oper check
        if (target.GetUser().Level >= EnumUserAccessLevel.Guide)
        {
            if (source.GetUser().Level < EnumUserAccessLevel.Guide) return EnumIrcError.ERR_NOIRCOP;
            // TODO: Maybe there is better raws for below
            if (source.GetUser().Level < EnumUserAccessLevel.Sysop &&
                source.GetUser().Level < target.GetUser().Level) return EnumIrcError.ERR_NOPERMS;
            if (source.GetUser().Level < EnumUserAccessLevel.Administrator &&
                source.GetUser().Level < target.GetUser().Level) return EnumIrcError.ERR_NOPERMS;
        }

        if (source.GetLevel() >= requiredLevel && source.GetLevel() >= target.GetLevel())
            return EnumIrcError.OK;
        if (!source.IsOwner() && (requiredLevel >= EnumChannelAccessLevel.ChatOwner ||
                                  target.GetLevel() >= EnumChannelAccessLevel.ChatOwner))
            return EnumIrcError.ERR_NOCHANOWNER;
        return EnumIrcError.ERR_NOCHANOP;
    }

    public void ProcessChannelError(EnumIrcError error, Server server, User? source, ChatObject? target = null,
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

    public virtual EnumChannelAccessResult GetAccess(User? user, string? key, bool IsGoto = false)
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

    public virtual bool InviteMember(User user)
    {
        var address = user.Address.GetAddress();
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

    protected virtual Member AddMember(User? user,
        EnumChannelAccessResult accessResult = EnumChannelAccessResult.NONE)
    {
        var member = new Member(user);

        if (accessResult == EnumChannelAccessResult.SUCCESS_OWNER) member.SetHost(true);
        else if (accessResult == EnumChannelAccessResult.SUCCESS_HOST) member.SetHost(true);
        else if (accessResult == EnumChannelAccessResult.SUCCESS_VOICE) member.SetVoice(true);

        _members.Add(member);
        user.AddChannel(this, member);
        return member;
    }

    private void RemoveMember(User? user)
    {
        var member = _members.Where(m => m.GetUser() == user).FirstOrDefault();
        _members.Remove(member);
        user.RemoveChannel(this);
    }

    public void SetName(string? Name)
    {
        this.Name = Name;
    }

    private static bool CompareUserAddress(User? user, User? otherUser)
    {
        if (otherUser == user || otherUser.Address.UserHost == user.Address.UserHost) return true;
        return false;
    }

    private static bool CompareUserNickname(User? user, User? otherUser)
    {
        return otherUser.Address.Nickname.ToUpper() == user.Address.Nickname.ToUpper();
    }

    public static bool ValidName(string? channel)
    {
        var regex = new Regex(IrcStrings.IrcxChannelRegex);
        return regex.Match(channel).Success;
    }

    public EnumChannelAccessResult GetAccessEx(User? user, string? key, bool IsGoto = false)
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

    protected EnumChannelAccessResult CheckOper(User? user)
    {
        if (user.Level >= EnumUserAccessLevel.Guide) return EnumChannelAccessResult.SUCCESS_OWNER;
        return EnumChannelAccessResult.NONE;
    }

    protected EnumChannelAccessResult CheckMemberKey(User? user, string? key)
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

    protected EnumChannelAccessResult CheckInviteOnly(User? user)
    {
        if (Modes[IrcStrings.ChannelModeInvite] == 1)
            return InviteList.Contains(user.Address.GetAddress())
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

    public EnumAccessLevel GetChannelAccess(User? user)
    {
        var userAccessLevel = EnumAccessLevel.NONE;
        var addressString = user.Address.GetFullAddress();
        var accessEntries = AccessList.Entries;

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


    protected EnumChannelAccessResult CheckHostKey(User? user, string? key)
    {
        if (string.IsNullOrWhiteSpace(key)) return EnumChannelAccessResult.NONE;

        if (Props["OWNERKEY"] == key)
            return EnumChannelAccessResult.SUCCESS_OWNER;
        if (Props["HOSTKEY"] == key) return EnumChannelAccessResult.SUCCESS_HOST;
        return EnumChannelAccessResult.NONE;
    }

    #region Modes

    // IRC
    public bool InviteOnly
    {
        get => Convert.ToBoolean(Modes[IrcStrings.ChannelModeInvite]);
        set => Modes[IrcStrings.ChannelModeInvite] = Convert.ToInt32(value);
    }

    public bool Key
    {
        get => Convert.ToBoolean(Modes[IrcStrings.ChannelModeKey]);
        set => Modes[IrcStrings.ChannelModeKey] = Convert.ToInt32(value);
    }

    public bool Moderated
    {
        get => Convert.ToBoolean(Modes[IrcStrings.ChannelModeModerated]);
        set => Modes[IrcStrings.ChannelModeModerated] = Convert.ToInt32(value);
    }

    public bool NoExtern
    {
        get => Convert.ToBoolean(Modes[IrcStrings.ChannelModeNoExtern]);
        set => Modes[IrcStrings.ChannelModeNoExtern] = Convert.ToInt32(value);
    }

    public bool Private
    {
        get => Convert.ToBoolean(Modes[IrcStrings.ChannelModePrivate]);
        set => Modes[IrcStrings.ChannelModePrivate] = Convert.ToInt32(value);
    }

    public bool Secret
    {
        get => Convert.ToBoolean(Modes[IrcStrings.ChannelModeSecret]);
        set => Modes[IrcStrings.ChannelModeSecret] = Convert.ToInt32(value);
    }

    public bool Hidden
    {
        get => Convert.ToBoolean(Modes[IrcStrings.ChannelModeHidden]);
        set => Modes[IrcStrings.ChannelModeHidden] = Convert.ToInt32(value);
    }

    public bool TopicOp
    {
        get => Convert.ToBoolean(Modes[IrcStrings.ChannelModeTopicOp]);
        set => Modes[IrcStrings.ChannelModeTopicOp] = Convert.ToInt32(value);
    }

    public int UserLimit
    {
        get => Modes[IrcStrings.ChannelModeUserLimit];
        set => Modes[IrcStrings.ChannelModeUserLimit] = value;
    }

    //IRCX
    public bool AuthOnly
    {
        get => Convert.ToBoolean(Modes[IrcStrings.ChannelModeAuthOnly]);
        set => Modes[IrcStrings.ChannelModeAuthOnly] = Convert.ToInt32(value);
    }

    public bool Profanity
    {
        get => Convert.ToBoolean(Modes[IrcStrings.ChannelModeProfanity]);
        set => Modes[IrcStrings.ChannelModeProfanity] = Convert.ToInt32(value);
    }

    public bool Registered
    {
        get => Convert.ToBoolean(Modes[IrcStrings.ChannelModeRegistered]);
        set => Modes[IrcStrings.ChannelModeRegistered] = Convert.ToInt32(value);
    }

    public bool Knock
    {
        get => Convert.ToBoolean(Modes[IrcStrings.ChannelModeKnock]);
        set => Modes[IrcStrings.ChannelModeKnock] = Convert.ToInt32(value);
    }

    public bool NoWhisper
    {
        get => Convert.ToBoolean(Modes[IrcStrings.ChannelModeNoWhisper]);
        set => Modes[IrcStrings.ChannelModeNoWhisper] = Convert.ToInt32(value);
    }

    public bool NoGuestWhisper
    {
        get => Convert.ToBoolean(Modes[IrcStrings.ChannelModeNoGuestWhisper]);
        set => Modes[IrcStrings.ChannelModeNoGuestWhisper] = Convert.ToInt32(value);
    }

    public bool Cloneable
    {
        get => Convert.ToBoolean(Modes[IrcStrings.ChannelModeCloneable]);
        set => Modes[IrcStrings.ChannelModeCloneable] = Convert.ToInt32(value);
    }

    public bool Clone
    {
        get => Convert.ToBoolean(Modes[IrcStrings.ChannelModeClone]);
        set => Modes[IrcStrings.ChannelModeClone] = Convert.ToInt32(value);
    }

    public bool Service
    {
        get => Convert.ToBoolean(Modes[IrcStrings.ChannelModeService]);
        set => Modes[IrcStrings.ChannelModeService] = Convert.ToInt32(value);
    }

    // Apollo

    public bool OnStage
    {
        get => Convert.ToBoolean(Modes[IrcStrings.ChannelModeOnStage]);
        set => Modes[IrcStrings.ChannelModeOnStage] = Convert.ToInt32(value);
    }

    #endregion
}