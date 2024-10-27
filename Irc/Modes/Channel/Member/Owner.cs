using Irc.Enumerations;
using Irc.Interfaces;
using Irc.Objects;
using Irc.Resources;

namespace Irc.Modes.Channel.Member;

public class Owner : ModeRule, IModeRule
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
    public Owner() : base(IrcStrings.MemberModeOwner, true)
    {
    }

    public EnumIrcError Evaluate(IChatObject source, IChatObject target, bool flag, string parameter)
    {
        var channel = (IChannel)target;
        if (!channel.CanBeModifiedBy(source)) return EnumIrcError.ERR_NOTONCHANNEL;

        var targetMember = channel.GetMemberByNickname(parameter);
        if (targetMember == null) return EnumIrcError.ERR_NOSUCHNICK;

        var sourceMember = channel.GetMember((IUser)source);

        var result = sourceMember.CanModify(targetMember, EnumChannelAccessLevel.ChatOwner);
        if (result == EnumIrcError.OK) ExecuteOwner(source, target, flag, targetMember);

        return result;
    }

    public static void ExecuteOwner(IChatObject sourceMember, IChatObject channel, bool flag, IChannelMember targetMember)
    {
        if (flag && targetMember.IsHost())
        {
            targetMember.SetHost(false);
            DispatchModeChange(IrcStrings.MemberModeHost, sourceMember, channel, false, targetMember.GetUser().ToString());
        }

        targetMember.SetOwner(flag);
        DispatchModeChange(IrcStrings.MemberModeOwner, sourceMember, channel, flag, targetMember.GetUser().ToString());
    }
}