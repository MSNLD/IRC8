using Irc.Enumerations;
using Irc.Interfaces;
using Irc.Objects;
using Irc.Resources;

namespace Irc.Modes.Channel.Member;

public class Voice : ModeRule, IModeRule
{
    public Voice() : base(IrcStrings.MemberModeVoice, true)
    {
    }

    public EnumIrcError Evaluate(IChatObject source, IChatObject target, bool flag, string parameter)
    {
        var channel = (IChannel)target;
        if (!channel.CanBeModifiedBy(source)) return EnumIrcError.ERR_NOTONCHANNEL;

        var targetMember = channel.GetMemberByNickname(parameter);
        if (targetMember == null) return EnumIrcError.ERR_NOSUCHNICK;

        var sourceMember = channel.GetMember((IUser)source);

        var result = sourceMember.CanModify(targetMember, EnumChannelAccessLevel.ChatVoice, false);
        if (result == EnumIrcError.OK)
        {
            targetMember.SetVoice(flag);
            DispatchModeChange(source, target, flag, targetMember.GetUser().ToString());
        }

        return result;
    }
}