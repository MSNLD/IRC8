using Irc.Enumerations;
using Irc.Interfaces;
using Irc.Objects;

namespace Irc.Modes
{
    public class ModeRuleChannel : ModeRule, IModeRule
    {
        private readonly EnumChannelAccessLevel accessLevel;

        public ModeRuleChannel(char modeChar, bool requiresParameter = false, int initialValue = 0,
            EnumChannelAccessLevel accessLevel = EnumChannelAccessLevel.ChatHost) :
            base(modeChar, requiresParameter, initialValue)
        {
            this.accessLevel = accessLevel;
        }

        public EnumIrcError Evaluate(IChatObject source, IChatObject target, bool flag, string parameter)
        {
            var user = (IUser)source;
            var channel = (IChannel)target;
            var member = channel.GetMember(user);

            if (member == null && !user.IsAdministrator()) return EnumIrcError.ERR_NOTONCHANNEL;

            if (member.GetLevel() < accessLevel) return EnumIrcError.ERR_NOCHANOP;

            return EnumIrcError.OK;
        }

        public interface IModeRuleCallback
        {
            IChatObject source { get; set; }
            IChatObject target { get; set; }
            bool flag { get; set; }
            string parameter { get; set; }
        }

        public EnumIrcError EvaluateAndExecute(IModeRuleCallback iModeRuleParams, Action<ModeRuleChannel.IModeRuleCallback> callback)
        {
            var result = Evaluate(iModeRuleParams.source, iModeRuleParams.target, iModeRuleParams.flag, iModeRuleParams.parameter);
            if (result == EnumIrcError.OK)
            {
                callback(iModeRuleParams);
            }

            return result;
        }

        public EnumIrcError EvaluateAndSet(IChatObject source, IChatObject target, bool flag, string parameter)
        {
            var result = Evaluate(source, target, flag, parameter);
            if (result == EnumIrcError.OK) SetChannelMode(source, (IChannel)target, flag, parameter);
            return result;
        }

        public void SetChannelMode(IChatObject source, IChannel target, bool flag, string parameter)
        {
            target.Modes.GetMode(ModeChar).Set(flag ? 1 : 0);
            DispatchModeChange(source, (ChatObject)target, flag, parameter);
        }
    }
}