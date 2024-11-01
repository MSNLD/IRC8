using Irc.Enumerations;
using Irc.Objects.Channel;
using Irc.Resources;

namespace Irc.Props;

internal class ChannelPropCollection : PropCollection
{
    public static PropRule Account = new()
    {
        Name = IrcStrings.ChannelPropAccount,
        ReadAccessLevel = EnumChannelAccessLevel.ChatHost,
        WriteAccessLevel = EnumChannelAccessLevel.ChatHost,
        ValidationMask = IrcStrings.GenericProps,
        Value = string.Empty,
        ReadOnly = true
    };

    public static PropRule Client = new()
    {
        Name = IrcStrings.ChannelPropClient,
        ReadAccessLevel = EnumChannelAccessLevel.ChatMember,
        WriteAccessLevel = EnumChannelAccessLevel.ChatHost,
        ValidationMask = IrcStrings.GenericProps,
        Value = string.Empty,
        ReadOnly = true
    };

    public static PropRule ClientGuid = new()
    {
        Name = IrcStrings.ChannelPropClientGuid,
        ReadAccessLevel = EnumChannelAccessLevel.None,
        WriteAccessLevel = EnumChannelAccessLevel.None,
        ValidationMask = IrcStrings.GenericProps,
        Value = string.Empty,
        ReadOnly = true
    };

    public static PropRule Creation = new()
    {
        Name = IrcStrings.ChannelPropCreation,
        ReadAccessLevel = EnumChannelAccessLevel.ChatMember,
        WriteAccessLevel = EnumChannelAccessLevel.None,
        ValidationMask = IrcStrings.GenericProps,
        Value = string.Empty,
        ReadOnly = true
    };

    public static PropRule Hostkey = new()
    {
        Name = IrcStrings.ChannelPropHostkey,
        ReadAccessLevel = EnumChannelAccessLevel.None,
        WriteAccessLevel = EnumChannelAccessLevel.ChatOwner,
        ValidationMask = IrcStrings.GenericProps,
        Value = string.Empty,
        ReadOnly = false
    };

    public static PropRule Lag = new()
    {
        Name = IrcStrings.ChannelPropLag,
        ReadAccessLevel = EnumChannelAccessLevel.ChatHost,
        WriteAccessLevel = EnumChannelAccessLevel.ChatHost,
        ValidationMask = IrcStrings.ChannelPropLagRegex,
        Value = string.Empty,
        ReadOnly = false
    };

    public static PropRule Language = new()
    {
        Name = IrcStrings.ChannelPropLanguage,
        ReadAccessLevel = EnumChannelAccessLevel.ChatMember,
        WriteAccessLevel = EnumChannelAccessLevel.ChatHost,
        ValidationMask = IrcStrings.GenericProps,
        Value = string.Empty,
        ReadOnly = false
    };

    public static PropRule Memberkey = new()
    {
        Name = IrcStrings.ChannelPropMemberkey,
        ReadAccessLevel = EnumChannelAccessLevel.ChatMember,
        WriteAccessLevel = EnumChannelAccessLevel.ChatHost,
        ValidationMask = IrcStrings.GenericProps,
        Value = string.Empty,
        ReadOnly = false,
        PostRule = (chatObject, propValue) =>
        {
            if (!string.IsNullOrWhiteSpace(propValue))
            {
                ((Channel)chatObject).Key = true;
                ((Channel)chatObject).Props[Resources.IrcStrings.ChannelPropMemberkey] = propValue;
            }
            else
            {
                ((Channel)chatObject).Key = false;
                ((Channel)chatObject).Props[Resources.IrcStrings.ChannelPropMemberkey] = null;
            }
        }
    };

    public static PropRule Name = new()
    {
        Name = IrcStrings.ChannelPropName,
        ReadAccessLevel = EnumChannelAccessLevel.ChatMember,
        WriteAccessLevel = EnumChannelAccessLevel.None,
        ValidationMask = IrcStrings.GenericProps,
        Value = string.Empty,
        ReadOnly = true
    };

    public static PropRule Oid = new()
    {
        Name = IrcStrings.ChannelPropOID,
        ReadAccessLevel = EnumChannelAccessLevel.ChatMember,
        WriteAccessLevel = EnumChannelAccessLevel.None,
        ValidationMask = IrcStrings.ChannelPropOIDRegex,
        Value = "0",
        ReadOnly = true
    };

    public static PropRule Onjoin = new()
    {
        Name = IrcStrings.ChannelPropOnJoin,
        ReadAccessLevel = EnumChannelAccessLevel.ChatHost,
        WriteAccessLevel = EnumChannelAccessLevel.ChatHost,
        ValidationMask = IrcStrings.ChannelPropOnjoinRegex,
        Value = "",
        ReadOnly = false
    };

    public static PropRule Onpart = new()
    {
        Name = IrcStrings.ChannelPropOnPart,
        ReadAccessLevel = EnumChannelAccessLevel.ChatHost,
        WriteAccessLevel = EnumChannelAccessLevel.ChatHost,
        ValidationMask = IrcStrings.ChannelPropOnpartRegex,
        Value = "",
        ReadOnly = false
    };

    public static PropRule Ownerkey = new()
    {
        Name = IrcStrings.ChannelPropOwnerkey,
        ReadAccessLevel = EnumChannelAccessLevel.None,
        WriteAccessLevel = EnumChannelAccessLevel.ChatOwner,
        ValidationMask = IrcStrings.GenericProps,
        Value = string.Empty,
        ReadOnly = false
    };

    public static PropRule Pics = new()
    {
        Name = IrcStrings.ChannelPropPICS,
        ReadAccessLevel = EnumChannelAccessLevel.ChatMember,
        WriteAccessLevel = EnumChannelAccessLevel.None,
        ValidationMask = IrcStrings.ChannelPropPICSRegex,
        Value = string.Empty,
        ReadOnly = false
    };

    public static PropRule Servicepath = new()
    {
        Name = IrcStrings.ChannelPropServicePath,
        ReadAccessLevel = EnumChannelAccessLevel.None,
        WriteAccessLevel = EnumChannelAccessLevel.ChatOwner,
        ValidationMask = IrcStrings.ChannelPropPICSRegex,
        Value = string.Empty,
        ReadOnly = true
    };

    public static PropRule Subject = new()
    {
        Name = IrcStrings.ChannelPropSubject,
        ReadAccessLevel = EnumChannelAccessLevel.ChatMember,
        WriteAccessLevel = EnumChannelAccessLevel.None,
        ValidationMask = IrcStrings.GenericProps,
        Value = string.Empty,
        ReadOnly = true
    };

    public static PropRule Topic = new()
    {
        Name = IrcStrings.ChannelPropTopic,
        ReadAccessLevel = EnumChannelAccessLevel.ChatMember,
        WriteAccessLevel = EnumChannelAccessLevel.ChatHost,
        ValidationMask = IrcStrings.ChannelPropTopicRegex,
        Value = string.Empty,
        ReadOnly = false,
        PostRule = (chatObject, propValue) =>
        {
            ((Channel)chatObject).Props[Resources.IrcStrings.ChannelPropTopic] = propValue;
        }
    };

    public static Dictionary<string?, PropRule> PropRules = new()
    {
        { IrcStrings.ChannelPropAccount, Account },
        { IrcStrings.ChannelPropClient, Client },
        { IrcStrings.ChannelPropClientGuid, ClientGuid },
        { IrcStrings.ChannelPropCreation, Creation },
        { IrcStrings.ChannelPropHostkey, Hostkey },
        { IrcStrings.ChannelPropLag, Lag },
        { IrcStrings.ChannelPropLanguage, Language },
        { IrcStrings.ChannelPropMemberkey, Memberkey },
        { IrcStrings.ChannelPropName, Name },
        { IrcStrings.ChannelPropOID, Oid },
        { IrcStrings.ChannelPropOnJoin, Onjoin },
        { IrcStrings.ChannelPropOnPart, Onpart },
        { IrcStrings.ChannelPropOwnerkey, Ownerkey },
        { IrcStrings.ChannelPropPICS, Pics },
        { IrcStrings.ChannelPropServicePath, Servicepath },
        { IrcStrings.ChannelPropSubject, Subject },
        { IrcStrings.ChannelPropTopic, Topic }
    };
}