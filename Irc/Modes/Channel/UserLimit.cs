using Irc.Enumerations;
using Irc.Interfaces;
using Irc.Resources;

namespace Irc.Modes.Channel;

public class UserLimit : ModeRuleChannel, IModeRule
{
    public UserLimit() : base(IrcStrings.ChannelModeUserLimit, true)
    {
    }

    public new EnumIrcError Evaluate(IChatObject source, IChatObject target, bool flag, string parameter)
    {
        var result = base.Evaluate(source, target, flag, parameter);
        if (result != EnumIrcError.OK) return result;

        var user = (IUser)source;
        var channel = (IChannel)target;
        var isAdministrator = user.IsAdministrator();

        if (flag == false)
        {
            if (isAdministrator)
            {
                // TODO: Currently does not support unsetting limit without extra parameter

                channel.UserLimit = 0;
                DispatchModeChange(source, target, false, string.Empty);
            }

            return EnumIrcError.OK;
        }


        if (!int.TryParse(parameter, out var limit)) return EnumIrcError.ERR_NEEDMOREPARAMS;

        if (limit > 0 && (limit <= 100 || isAdministrator))
        {
            channel.UserLimit = limit;
            DispatchModeChange(source, target, true, limit.ToString());
        }

        return EnumIrcError.OK;
    }
}