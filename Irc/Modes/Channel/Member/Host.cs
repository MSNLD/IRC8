using Irc.Enumerations;
using Irc.Objects;
using Irc.Resources;

namespace Irc.Modes.Channel.Member;

public class Host : ModeRuleChannel
{
    public Host() : base(IrcStrings.UserModeHost, true)
    {
    }

    public override EnumIrcError Evaluate(ChatObject source, ChatObject? target, bool flag, string? parameter)
    {
        // TODO: Write this better
        if (target == source && flag)
        {
            if (string.IsNullOrWhiteSpace(parameter)) return EnumIrcError.OK;

            var user = (Objects.User)source;
            var channel = user.Channels.LastOrDefault().Key;
            var member = user.Channels.LastOrDefault().Value;
            if (channel.Props["OWNERKEY"] == parameter)
            {
                if (member.IsOwner())
                {
                    member.SetHost(false);
                    Operator.ExecuteHost(source, channel, false, member);
                }
                
                Owner.ExecuteOwner(source, channel, true, member);
            }
            else if (channel.Props["HOSTKEY"] == parameter)
            {
                if (member.IsOwner())
                {
                    member.SetOwner(false);
                    Owner.ExecuteOwner(source, channel, false, member);
                }

                member.SetHost(true);
                Operator.ExecuteHost(source, channel, true, member);
            }

            return EnumIrcError.OK;
        }

        return EnumIrcError.ERR_UNKNOWNMODEFLAG;
    }
}