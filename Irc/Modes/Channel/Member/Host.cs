using Irc.Enumerations;
using Irc.Interfaces;
using Irc.Objects;
using Irc.Objects.Channel;
using Irc.Resources;

namespace Irc.Modes.Channel.Member;

public class Host : ModeRuleChannel, IModeRule
{
    public Host() : base(IrcStrings.UserModeHost, true)
    {
    }

    public new EnumIrcError Evaluate(IChatObject source, IChatObject target, bool flag, string parameter)
    {
        // TODO: Write this better
        if (target == source && flag)
        {
            if (string.IsNullOrWhiteSpace(parameter)) return EnumIrcError.OK;

            var user = (IUser)source;
            var channel = user.GetChannels().LastOrDefault().Key;
            var member = user.GetChannels().LastOrDefault().Value;
            if (channel.Props.GetProp("OWNERKEY").GetValue(target) == parameter)
            {
                Owner.ExecuteOwner(source, channel, true, member);
            }
            else if (channel.Props.GetProp("HOSTKEY").GetValue(target) == parameter)
            {
                if (member.IsOwner())
                {
                    member.SetOwner(false);
                    channel.Modes.GetMode('q').DispatchModeChange(source, channel, false, target.ToString());
                }

                member.SetHost(true);
                channel.Modes.GetMode('o').DispatchModeChange(source, channel, true, target.ToString());
            }

            return EnumIrcError.OK;
        }

        return EnumIrcError.ERR_UNKNOWNMODEFLAG;
    }
}