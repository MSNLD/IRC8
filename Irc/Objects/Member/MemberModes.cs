using Irc.Interfaces;
using Irc.Modes.Channel.Member;
using Irc.Resources;

namespace Irc.Objects.Member;

public class MemberModes : ModeCollection, IMemberModes
{
    public MemberModes()
    {
        modes.Add(IrcStrings.MemberModeHost, new Operator());
        modes.Add(IrcStrings.MemberModeVoice, new Voice());
        modes.Add(IrcStrings.MemberModeOwner, new Owner());
    }

    public string GetListedMode()
    {
        if (IsOwner()) return IrcStrings.MemberModeFlagOwner.ToString();
        if (IsHost()) return IrcStrings.MemberModeFlagHost.ToString();
        if (IsVoice()) return IrcStrings.MemberModeFlagVoice.ToString();

        return "";
    }

    public char GetModeChar()
    {
        if (IsOwner()) return IrcStrings.MemberModeOwner;
        if (IsHost()) return IrcStrings.MemberModeHost;
        if (IsVoice()) return IrcStrings.MemberModeVoice;

        return (char)0;
    }

    public bool IsOwner()
    {
        // TODO: Need to think about a better way of handling the below
        return modes.ContainsKey(IrcStrings.MemberModeOwner) && GetModeChar(IrcStrings.MemberModeOwner) > 0;
    }

    public bool IsHost()
    {
        return GetModeChar(IrcStrings.MemberModeHost) > 0;
    }

    public bool IsVoice()
    {
        return GetModeChar(IrcStrings.MemberModeVoice) > 0;
    }

    public bool IsNormal()
    {
        return !IsOwner() && !IsHost() && !IsVoice();
    }

    public void SetHost(bool flag)
    {
        modes[IrcStrings.MemberModeHost].Set(flag ? 1 : 0);
    }

    public void SetVoice(bool flag)
    {
        modes[IrcStrings.MemberModeVoice].Set(flag ? 1 : 0);
    }

    public void SetNormal()
    {
        SetOwner(false);
        SetHost(false);
        SetVoice(false);
    }

    public void SetOwner(bool flag)
    {
        modes[IrcStrings.MemberModeOwner].Set(flag ? 1 : 0);
    }
}