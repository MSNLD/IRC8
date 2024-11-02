using Irc.Enumerations;
using Irc.Objects;
using Irc.Resources;

namespace Irc.Modes.Channel.Member;

public class Operator : ModeRule
{
    /*
     -> sky-8a15b323126 MODE #test +q Sky2k
    <- :sky-8a15b323126 482 Sky3k #test :You're not channel operator
    -> sky-8a15b323126 MODE #test +o Sky2k
    <- :sky-8a15b323126 482 Sky3k #test :You're not channel operator
    <- :Sky2k!~no@127.0.0.1 MODE #test +o Sky3k
    -> sky-8a15b323126 MODE #test +q Sky2k
    <- :sky-8a15b323126 485 Sky3k #test :You're not channel owner
    -> sky-8a15b323126 MODE #test +o Sky2k
    <- :sky-8a15b323126 485 Sky3k #test :You're not channel owner
     */
    public Operator() : base(IrcStrings.MemberModeHost, true)
    {
    }

    public override EnumIrcError Evaluate(ChatObject source, ChatObject? target, bool flag, string? parameter)
    {
        var channel = (Objects.Channel)target;
        if (!channel.CanBeModifiedBy(source)) return EnumIrcError.ERR_NOTONCHANNEL;

        var targetMember = channel.GetMemberByNickname(parameter);
        if (targetMember == null) return EnumIrcError.ERR_NOSUCHNICK;

        var sourceMember = channel.GetMember((Objects.User)source);

        var result = sourceMember.CanModify(targetMember, EnumChannelAccessLevel.ChatHost);
        if (result != EnumIrcError.OK) return result;

        if (targetMember.IsOwner())
        {
            targetMember.SetOwner(false);
            DispatchModeChange(IrcStrings.MemberModeOwner, source, target, false, targetMember.GetUser().ToString());
        }

        targetMember.SetHost(flag);
        DispatchModeChange(source, target, flag, targetMember.GetUser().ToString());
        return result;
    }
    
    public static void ExecuteHost(ChatObject sourceMember, ChatObject? channel, bool flag,
        Objects.Member targetMember)
    {
        if (flag && targetMember.IsOwner())
        {
            targetMember.SetOwner(false);
            DispatchModeChange(IrcStrings.MemberModeOwner, sourceMember, channel, false,
                targetMember.GetUser().ToString());
        }

        targetMember.SetHost(flag);
        DispatchModeChange(IrcStrings.MemberModeHost, sourceMember, channel, flag, targetMember.GetUser().ToString());
    }
}