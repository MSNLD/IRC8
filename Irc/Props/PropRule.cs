using System.Text.RegularExpressions;
using Irc.Enumerations;
using Irc.Interfaces;
using Irc.Objects;

namespace Irc.Props;

public class PropRule : IPropRule
{
    public string ValidationMask;

    public PropRule()
    {
    }

    public PropRule(string? name, EnumChannelAccessLevel readAccessLevel, EnumChannelAccessLevel writeAccessLevel,
        string validationMask, string initialValue, bool readOnly = false)
    {
        Name = name;
        ReadAccessLevel = readAccessLevel;
        WriteAccessLevel = writeAccessLevel;
        ValidationMask = validationMask;
        Value = initialValue;
        ReadOnly = readOnly;
    }

    public Action<ChatObject, string> PostRule { get; set; } = null;
    public string Value { get; set; }

    public string? Name { set; get; }

    public EnumChannelAccessLevel ReadAccessLevel { get; set; }
    public EnumChannelAccessLevel WriteAccessLevel { get; set; }
    public bool ReadOnly { get; set; }

    public virtual EnumIrcError EvaluateSet(IChatObject source, IChatObject target, string? propValue)
    {
        if (target is IChannel)
        {
            var channel = (IChannel)target;
            var member = channel.GetMember((IUser)source);

            if (member == null) return EnumIrcError.ERR_NOPERMS;

            if (member.GetLevel() < WriteAccessLevel) return EnumIrcError.ERR_NOPERMS;
        }
        else if (WriteAccessLevel == EnumChannelAccessLevel.None || (target is IUser && source != target))
        {
            return EnumIrcError.ERR_NOPERMS;
        }

        // Otherwise perms are OK, it is the same user, or is a server
        var regEx = new Regex(ValidationMask);
        var match = regEx.Match(propValue);
        if (!match.Success || match.Value.Length != propValue.Length) return EnumIrcError.ERR_BADVALUE;

        if (PostRule != null) PostRule((ChatObject)target, propValue);

        return EnumIrcError.OK;
    }

    public virtual EnumIrcError EvaluateGet(IChatObject source, IChatObject target)
    {
        if (target is IChannel)
        {
            var channel = (IChannel)target;
            var member = channel.GetMember((IUser)source);

            if (member == null) return EnumIrcError.ERR_NOPERMS;

            if (member.GetLevel() < ReadAccessLevel) return EnumIrcError.ERR_NOPERMS;
        }
        else if (ReadAccessLevel == EnumChannelAccessLevel.None || (target is IUser && source != target))
        {
            return EnumIrcError.ERR_NOPERMS;
        }

        // Otherwise perms are OK, it is the same user, or is a server
        return EnumIrcError.OK;
    }
}