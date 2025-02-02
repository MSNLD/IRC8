﻿using Irc.Enumerations;
using Irc.Resources;

namespace Irc.Objects;

public class Member : ChatObject
{
    protected readonly User? _user;

    public Member(User? User)
    {
        _user = User;
        Modes[Tokens.MemberModeOwner] = 0;
        Modes[Tokens.MemberModeHost] = 0;
        Modes[Tokens.MemberModeVoice] = 0;
    }

    public EnumChannelAccessLevel GetLevel()
    {
        if (IsOwner())
            return EnumChannelAccessLevel.ChatOwner;

        if (IsHost())
            return EnumChannelAccessLevel.ChatHost;

        if (IsVoice())
            return EnumChannelAccessLevel.ChatVoice;

        return EnumChannelAccessLevel.ChatMember;
    }

    public EnumIrcError CanModify(Member target, EnumChannelAccessLevel requiredLevel, bool operCheck = true)
    {
        if (operCheck)
            // Oper check
            if (target.GetUser().Level >= EnumUserAccessLevel.Guide)
            {
                if (_user.Level < EnumUserAccessLevel.Guide) return EnumIrcError.ERR_NOIRCOP;
                // TODO: Maybe there is better raws for below
                if (_user.Level < EnumUserAccessLevel.Sysop && _user.Level < target.GetUser().Level)
                    return EnumIrcError.ERR_NOPERMS;
                if (_user.Level < EnumUserAccessLevel.Administrator &&
                    _user.Level < target.GetUser().Level) return EnumIrcError.ERR_NOPERMS;
            }

        if (!IsOwner() && requiredLevel >= EnumChannelAccessLevel.ChatOwner) return EnumIrcError.ERR_NOCHANOWNER;
        if (!IsOwner() && !IsHost() && requiredLevel >= EnumChannelAccessLevel.ChatVoice)
            return EnumIrcError.ERR_NOCHANOP;
        return EnumIrcError.OK;
    }

    public User? GetUser()
    {
        return _user;
    }

    public string GetModeString()
    {
        return $"{new string(Modes.Where(mode => mode.Value > 0).Select(mode => mode.Key).ToArray())}";
    }

    public string GetListedMode()
    {
        if (IsOwner()) return Tokens.MemberModeFlagOwner.ToString();
        if (IsHost()) return Tokens.MemberModeFlagHost.ToString();
        if (IsVoice()) return Tokens.MemberModeFlagVoice.ToString();

        return "";
    }

    public char GetModeChar()
    {
        if (IsOwner()) return Tokens.MemberModeOwner;
        if (IsHost()) return Tokens.MemberModeHost;
        if (IsVoice()) return Tokens.MemberModeVoice;

        return (char)0;
    }

    public bool IsOwner()
    {
        // TODO: Need to think about a better way of handling the below
        return Modes.ContainsKey(Tokens.MemberModeOwner) && Modes[Tokens.MemberModeOwner] > 0;
    }

    public bool IsHost()
    {
        return Modes[Tokens.MemberModeHost] > 0;
    }

    public bool IsVoice()
    {
        return Modes[Tokens.MemberModeVoice] > 0;
    }

    public bool IsNormal()
    {
        return !IsOwner() && !IsHost() && !IsVoice();
    }

    public void SetOwner(bool flag)
    {
        Modes[Tokens.MemberModeOwner] = flag ? 1 : 0;
    }

    public void SetHost(bool flag)
    {
        Modes[Tokens.MemberModeHost] = flag ? 1 : 0;
    }

    public void SetVoice(bool flag)
    {
        Modes[Tokens.MemberModeVoice] = flag ? 1 : 0;
    }

    public void SetNormal()
    {
        SetOwner(false);
        SetHost(false);
        SetVoice(false);
    }

    public override void Send(string message)
    {
        _user.Send(message);
    }

    public override void Send(string message, ChatObject except = null)
    {
        _user.Send(message, except);
    }

    public override void Send(string message, EnumChannelAccessLevel accessLevel)
    {
        _user.Send(message, accessLevel);
    }

    public override bool CanBeModifiedBy(ChatObject source)
    {
        throw new NotImplementedException();
    }
}