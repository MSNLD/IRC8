using Irc.Enumerations;
using Irc.Interfaces;
using Irc.Objects;
using Irc.Resources;

namespace Irc.Modes.Channel.Member;

public class Host : ModeRuleChannel, IModeRule
{
    public Host() : base(IrcStrings.UserModeHost, true)
    {
    }

    public new EnumIrcError Evaluate(ChatObject source, ChatObject? target, bool flag, string? parameter)
    {
        // TODO: Write this better
        if (target == source && flag)
        {
            if (string.IsNullOrWhiteSpace(parameter)) return EnumIrcError.OK;

            var user = (Objects.User)source;
            var channel = user.GetChannels().LastOrDefault().Key;
            var member = user.GetChannels().LastOrDefault().Value;
            if (channel.Props["OWNERKEY"] == parameter)
            {
                Owner.ExecuteOwner(source, channel, true, member);
            }
            else if (channel.Props["HOSTKEY"] == parameter)
            {
                if (member.IsOwner())
                {
                    member.SetOwner(false);
                    DispatchModeChange((ChatObject)channel, IrcStrings.MemberModeOwner, (ChatObject)source,
                        (ChatObject)target, false, target.ToString());
                    //channel.Modes.GetMode('q').DispatchModeChange(source, channel, false, target.ToString());
                }

                member.SetHost(true);
                DispatchModeChange((ChatObject)channel, IrcStrings.MemberModeHost, (ChatObject)source,
                    (ChatObject)target, true, target.ToString());
                // channel.Modes.GetMode('o').DispatchModeChange(source, channel, true, target.ToString());
            }

            return EnumIrcError.OK;
        }

        return EnumIrcError.ERR_UNKNOWNMODEFLAG;
    }
}