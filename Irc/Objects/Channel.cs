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
        Props[Tokens.ChannelPropOID] = "";
        Props[Tokens.ChannelPropCreation] = "";
        Props[Tokens.ChannelPropLanguage] = "";
        Props[Tokens.ChannelPropMemberkey] = "";
        Props[Tokens.ChannelPropOwnerkey] = "";
        Props[Tokens.ChannelPropHostkey] = "";
        Props[Tokens.ChannelPropPICS] = "";
        Props[Tokens.ChannelPropTopic] = "";
        Props[Tokens.ChannelPropSubject] = "";
        Props[Tokens.ChannelPropOnJoin] = "";
        Props[Tokens.ChannelPropOnPart] = "";
        Props[Tokens.ChannelPropLag] = "";
        Props[Tokens.ChannelPropClient] = "";
        Props[Tokens.ChannelPropClientGuid] = "";
        Props[Tokens.ChannelPropServicePath] = "";

        // TODO: Add Modes
        Modes[Tokens.ChannelModeInvite] = 0;
        Modes[Tokens.ChannelModeKey] = 0;
        Modes[Tokens.ChannelModeModerated] = 0;
        Modes[Tokens.ChannelModeNoExtern] = 0;
        Modes[Tokens.ChannelModePrivate] = 0;
        Modes[Tokens.ChannelModeSecret] = 0;
        Modes[Tokens.ChannelModeHidden] = 0;
        Modes[Tokens.ChannelModeTopicOp] = 0;
        Modes[Tokens.ChannelModeUserLimit] = 0;
        Modes[Tokens.ChannelModeAuthOnly] = 0;
        Modes[Tokens.ChannelModeProfanity] = 0;
        Modes[Tokens.ChannelModeRegistered] = 0;
        Modes[Tokens.ChannelModeKnock] = 0;
        Modes[Tokens.ChannelModeNoWhisper] = 0;
        Modes[Tokens.ChannelModeNoGuestWhisper] = 0;
        Modes[Tokens.ChannelModeCloneable] = 0;
        Modes[Tokens.ChannelModeClone] = 0;
        Modes[Tokens.ChannelModeService] = 0;
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
                channelMember.GetUser().Send(Raws.RPL_JOIN(user, this));

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
        user.Send(Raws.IRCX_RPL_TOPIC_332(user.Server, user, this, Props[Tokens.ChannelPropTopic] ?? string.Empty));
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
        Send(Raws.RPL_PART(user, this));
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
        Send(Raws.RPL_KICK_IRC(source, this, target, reason));
        RemoveMember(target);
        return this;
    }

    public void SendMessage(User? user, string? message)
    {
        Send(Raws.RPL_PRIVMSG(user, this, message), (ChatObject)user);
    }

    public void SendNotice(User? user, string? message)
    {
        Send(Raws.RPL_NOTICE(user, this, message), (ChatObject)user);
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
                source.Send(Raws.IRCX_ERR_NEEDMOREPARAMS_461(server, source, data));
                break;
            }
            case EnumIrcError.ERR_NOCHANOP:
            {
                //:sky-8a15b323126 482 Sky3k #test :You're not channel operator
                source.Send(Raws.IRCX_ERR_CHANOPRIVSNEEDED_482(server, source, this));
                break;
            }
            case EnumIrcError.ERR_NOCHANOWNER:
            {
                //:sky-8a15b323126 482 Sky3k #test :You're not channel operator
                source.Send(Raws.IRCX_ERR_CHANQPRIVSNEEDED_485(server, source, this));
                break;
            }
            case EnumIrcError.ERR_NOIRCOP:
            {
                source.Send(Raws.IRCX_ERR_NOPRIVILEGES_481(server, source));
                break;
            }
            case EnumIrcError.ERR_NOTONCHANNEL:
            {
                source.Send(Raws.IRCX_ERR_NOTONCHANNEL_442(server, source, this));
                break;
            }
            // TODO: The below should not happen
            case EnumIrcError.ERR_NOSUCHNICK:
            {
                source.Send(Raws.IRCX_ERR_NOSUCHNICK_401(server, source, target.Name));
                break;
            }
            case EnumIrcError.ERR_NOSUCHCHANNEL:
            {
                source.Send(Raws.IRCX_ERR_NOSUCHCHANNEL_403(server, source, Name));
                break;
            }
            case EnumIrcError.ERR_CANNOTSETFOROTHER:
            {
                source.Send(Raws.IRCX_ERR_USERSDONTMATCH_502(server, source));
                break;
            }
            case EnumIrcError.ERR_UNKNOWNMODEFLAG:
            {
                source.Send(Raws.IRC_RAW_501(server, source));
                break;
            }
            case EnumIrcError.ERR_NOPERMS:
            {
                source.Send(Raws.IRCX_ERR_SECURITY_908(server, source));
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
        var regex = new Regex(Tokens.IrcxChannelRegex);
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

        if (Modes[Tokens.ChannelModeKey] == 1)
        {
            if (Props[Tokens.ChannelPropMemberkey] == key)
                return EnumChannelAccessResult.SUCCESS_MEMBER;
            return EnumChannelAccessResult.ERR_BADCHANNELKEY;
        }

        return EnumChannelAccessResult.NONE;
    }

    protected EnumChannelAccessResult CheckInviteOnly(User? user)
    {
        if (Modes[Tokens.ChannelModeInvite] == 1)
            return InviteList.Contains(user.Address.GetAddress())
                ? EnumChannelAccessResult.SUCCESS_MEMBER
                : EnumChannelAccessResult.ERR_INVITEONLYCHAN;

        return EnumChannelAccessResult.NONE;
    }

    protected EnumChannelAccessResult CheckUserLimit(bool IsGoto)
    {
        var modeLimit = Modes[Tokens.ChannelModeUserLimit];
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
        get => Convert.ToBoolean(Modes[Tokens.ChannelModeInvite]);
        set => Modes[Tokens.ChannelModeInvite] = Convert.ToInt32(value);
    }

    public bool Key
    {
        get => Convert.ToBoolean(Modes[Tokens.ChannelModeKey]);
        set => Modes[Tokens.ChannelModeKey] = Convert.ToInt32(value);
    }

    public bool Moderated
    {
        get => Convert.ToBoolean(Modes[Tokens.ChannelModeModerated]);
        set => Modes[Tokens.ChannelModeModerated] = Convert.ToInt32(value);
    }

    public bool NoExtern
    {
        get => Convert.ToBoolean(Modes[Tokens.ChannelModeNoExtern]);
        set => Modes[Tokens.ChannelModeNoExtern] = Convert.ToInt32(value);
    }

    public bool Private
    {
        get => Convert.ToBoolean(Modes[Tokens.ChannelModePrivate]);
        set => Modes[Tokens.ChannelModePrivate] = Convert.ToInt32(value);
    }

    public bool Secret
    {
        get => Convert.ToBoolean(Modes[Tokens.ChannelModeSecret]);
        set => Modes[Tokens.ChannelModeSecret] = Convert.ToInt32(value);
    }

    public bool Hidden
    {
        get => Convert.ToBoolean(Modes[Tokens.ChannelModeHidden]);
        set => Modes[Tokens.ChannelModeHidden] = Convert.ToInt32(value);
    }

    public bool TopicOp
    {
        get => Convert.ToBoolean(Modes[Tokens.ChannelModeTopicOp]);
        set => Modes[Tokens.ChannelModeTopicOp] = Convert.ToInt32(value);
    }

    public int UserLimit
    {
        get => Modes[Tokens.ChannelModeUserLimit];
        set => Modes[Tokens.ChannelModeUserLimit] = value;
    }

    //IRCX
    public bool AuthOnly
    {
        get => Convert.ToBoolean(Modes[Tokens.ChannelModeAuthOnly]);
        set => Modes[Tokens.ChannelModeAuthOnly] = Convert.ToInt32(value);
    }

    public bool Profanity
    {
        get => Convert.ToBoolean(Modes[Tokens.ChannelModeProfanity]);
        set => Modes[Tokens.ChannelModeProfanity] = Convert.ToInt32(value);
    }

    public bool Registered
    {
        get => Convert.ToBoolean(Modes[Tokens.ChannelModeRegistered]);
        set => Modes[Tokens.ChannelModeRegistered] = Convert.ToInt32(value);
    }

    public bool Knock
    {
        get => Convert.ToBoolean(Modes[Tokens.ChannelModeKnock]);
        set => Modes[Tokens.ChannelModeKnock] = Convert.ToInt32(value);
    }

    public bool NoWhisper
    {
        get => Convert.ToBoolean(Modes[Tokens.ChannelModeNoWhisper]);
        set => Modes[Tokens.ChannelModeNoWhisper] = Convert.ToInt32(value);
    }

    public bool NoGuestWhisper
    {
        get => Convert.ToBoolean(Modes[Tokens.ChannelModeNoGuestWhisper]);
        set => Modes[Tokens.ChannelModeNoGuestWhisper] = Convert.ToInt32(value);
    }

    public bool Cloneable
    {
        get => Convert.ToBoolean(Modes[Tokens.ChannelModeCloneable]);
        set => Modes[Tokens.ChannelModeCloneable] = Convert.ToInt32(value);
    }

    public bool Clone
    {
        get => Convert.ToBoolean(Modes[Tokens.ChannelModeClone]);
        set => Modes[Tokens.ChannelModeClone] = Convert.ToInt32(value);
    }

    public bool Service
    {
        get => Convert.ToBoolean(Modes[Tokens.ChannelModeService]);
        set => Modes[Tokens.ChannelModeService] = Convert.ToInt32(value);
    }

    // Apollo

    public bool OnStage
    {
        get => Convert.ToBoolean(Modes[Tokens.ChannelModeOnStage]);
        set => Modes[Tokens.ChannelModeOnStage] = Convert.ToInt32(value);
    }

    #endregion
}