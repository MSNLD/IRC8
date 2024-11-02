using Irc.Enumerations;
using Irc.Interfaces;
using Irc.Objects;

namespace Irc.Modes;

public class ModeRuleChannel : ModeRule, IModeRule
{
    private readonly EnumChannelAccessLevel accessLevel;

    public ModeRuleChannel(char modeChar, bool requiresParameter = false, int initialValue = 0,
        EnumChannelAccessLevel accessLevel = EnumChannelAccessLevel.ChatHost) :
        base(modeChar, requiresParameter, initialValue)
    {
        this.accessLevel = accessLevel;
    }

    public EnumIrcError Evaluate(ChatObject source, ChatObject? target, bool flag, string? parameter)
    {
        var user = (Objects.User)source;
        var channel = (Objects.Channel)target;
        var member = channel.GetMember(user);

        if (member == null && !user.IsAdministrator()) return EnumIrcError.ERR_NOTONCHANNEL;

        if (member.GetLevel() < accessLevel) return EnumIrcError.ERR_NOCHANOP;

        if (PostRule != null) PostRule((ChatObject)target, flag, parameter);

        return EnumIrcError.OK;
    }

    public EnumIrcError EvaluateAndExecute(IModeRuleCallback iModeRuleParams, Action<IModeRuleCallback> callback)
    {
        var result = Evaluate(iModeRuleParams.source, iModeRuleParams.target, iModeRuleParams.flag,
            iModeRuleParams.parameter);
        if (result == EnumIrcError.OK) callback(iModeRuleParams);

        return result;
    }

    public EnumIrcError EvaluateAndSet(ChatObject source, ChatObject? target, bool flag, string? parameter)
    {
        var result = Evaluate(source, target, flag, parameter);
        if (result == EnumIrcError.OK) SetChannelMode(source, (Objects.Channel)target, flag, parameter);
        return result;
    }

    public void SetChannelMode(ChatObject source, Objects.Channel target, bool flag, string? parameter)
    {
        target.Modes[ModeChar] = flag ? 1 : 0;
        DispatchModeChange(source, (ChatObject)target, flag, parameter);
    }

    public interface IModeRuleCallback
    {
        ChatObject source { get; set; }
        ChatObject? target { get; set; }
        bool flag { get; set; }
        string? parameter { get; set; }
    }
}