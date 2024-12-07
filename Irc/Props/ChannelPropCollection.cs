using Irc.Enumerations;
using Irc.Objects;
using Irc.Resources;

namespace Irc.Props;

internal class ChannelPropCollection : PropCollection
{
    public static PropRule Account = new()
    {
        Name = Tokens.ChannelPropAccount,
        ReadAccessLevel = EnumChannelAccessLevel.ChatHost,
        WriteAccessLevel = EnumChannelAccessLevel.ChatHost,
        ValidationMask = Tokens.GenericProps,
        Value = string.Empty,
        ReadOnly = true
    };

    public static PropRule Client = new()
    {
        Name = Tokens.ChannelPropClient,
        ReadAccessLevel = EnumChannelAccessLevel.ChatMember,
        WriteAccessLevel = EnumChannelAccessLevel.ChatHost,
        ValidationMask = Tokens.GenericProps,
        Value = string.Empty,
        ReadOnly = true
    };

    public static PropRule ClientGuid = new()
    {
        Name = Tokens.ChannelPropClientGuid,
        ReadAccessLevel = EnumChannelAccessLevel.None,
        WriteAccessLevel = EnumChannelAccessLevel.None,
        ValidationMask = Tokens.GenericProps,
        Value = string.Empty,
        ReadOnly = true
    };

    public static PropRule Creation = new()
    {
        Name = Tokens.ChannelPropCreation,
        ReadAccessLevel = EnumChannelAccessLevel.ChatMember,
        WriteAccessLevel = EnumChannelAccessLevel.None,
        ValidationMask = Tokens.GenericProps,
        Value = string.Empty,
        ReadOnly = true
    };

    public static PropRule Hostkey = new()
    {
        Name = Tokens.ChannelPropHostkey,
        ReadAccessLevel = EnumChannelAccessLevel.None,
        WriteAccessLevel = EnumChannelAccessLevel.ChatOwner,
        ValidationMask = Tokens.GenericProps,
        Value = string.Empty,
        ReadOnly = false
    };

    public static PropRule Lag = new()
    {
        Name = Tokens.ChannelPropLag,
        ReadAccessLevel = EnumChannelAccessLevel.ChatHost,
        WriteAccessLevel = EnumChannelAccessLevel.ChatHost,
        ValidationMask = Tokens.ChannelPropLagRegex,
        Value = string.Empty,
        ReadOnly = false
    };

    public static PropRule Language = new()
    {
        Name = Tokens.ChannelPropLanguage,
        ReadAccessLevel = EnumChannelAccessLevel.ChatMember,
        WriteAccessLevel = EnumChannelAccessLevel.ChatHost,
        ValidationMask = Tokens.GenericProps,
        Value = string.Empty,
        ReadOnly = false
    };

    public static PropRule Memberkey = new()
    {
        Name = Tokens.ChannelPropMemberkey,
        ReadAccessLevel = EnumChannelAccessLevel.ChatMember,
        WriteAccessLevel = EnumChannelAccessLevel.ChatHost,
        ValidationMask = Tokens.GenericProps,
        Value = string.Empty,
        ReadOnly = false,
        PostRule = (chatObject, propValue) =>
        {
            if (!string.IsNullOrWhiteSpace(propValue))
            {
                ((Channel)chatObject).Key = true;
                ((Channel)chatObject).Props[Tokens.ChannelPropMemberkey] = propValue;
            }
            else
            {
                ((Channel)chatObject).Key = false;
                ((Channel)chatObject).Props[Tokens.ChannelPropMemberkey] = null;
            }
        }
    };

    public static PropRule Name = new()
    {
        Name = Tokens.ChannelPropName,
        ReadAccessLevel = EnumChannelAccessLevel.ChatMember,
        WriteAccessLevel = EnumChannelAccessLevel.None,
        ValidationMask = Tokens.GenericProps,
        Value = string.Empty,
        ReadOnly = true
    };

    public static PropRule Oid = new()
    {
        Name = Tokens.ChannelPropOID,
        ReadAccessLevel = EnumChannelAccessLevel.ChatMember,
        WriteAccessLevel = EnumChannelAccessLevel.None,
        ValidationMask = Tokens.ChannelPropOIDRegex,
        Value = "0",
        ReadOnly = true
    };

    public static PropRule Onjoin = new()
    {
        Name = Tokens.ChannelPropOnJoin,
        ReadAccessLevel = EnumChannelAccessLevel.ChatHost,
        WriteAccessLevel = EnumChannelAccessLevel.ChatHost,
        ValidationMask = Tokens.ChannelPropOnjoinRegex,
        Value = "",
        ReadOnly = false
    };

    public static PropRule Onpart = new()
    {
        Name = Tokens.ChannelPropOnPart,
        ReadAccessLevel = EnumChannelAccessLevel.ChatHost,
        WriteAccessLevel = EnumChannelAccessLevel.ChatHost,
        ValidationMask = Tokens.ChannelPropOnpartRegex,
        Value = "",
        ReadOnly = false
    };

    public static PropRule Ownerkey = new()
    {
        Name = Tokens.ChannelPropOwnerkey,
        ReadAccessLevel = EnumChannelAccessLevel.None,
        WriteAccessLevel = EnumChannelAccessLevel.ChatOwner,
        ValidationMask = Tokens.GenericProps,
        Value = string.Empty,
        ReadOnly = false
    };

    public static PropRule Pics = new()
    {
        Name = Tokens.ChannelPropPICS,
        ReadAccessLevel = EnumChannelAccessLevel.ChatMember,
        WriteAccessLevel = EnumChannelAccessLevel.None,
        ValidationMask = Tokens.ChannelPropPICSRegex,
        Value = string.Empty,
        ReadOnly = false
    };

    public static PropRule Servicepath = new()
    {
        Name = Tokens.ChannelPropServicePath,
        ReadAccessLevel = EnumChannelAccessLevel.None,
        WriteAccessLevel = EnumChannelAccessLevel.ChatOwner,
        ValidationMask = Tokens.ChannelPropPICSRegex,
        Value = string.Empty,
        ReadOnly = true
    };

    public static PropRule Subject = new()
    {
        Name = Tokens.ChannelPropSubject,
        ReadAccessLevel = EnumChannelAccessLevel.ChatMember,
        WriteAccessLevel = EnumChannelAccessLevel.None,
        ValidationMask = Tokens.GenericProps,
        Value = string.Empty,
        ReadOnly = true
    };

    public static PropRule Topic = new()
    {
        Name = Tokens.ChannelPropTopic,
        ReadAccessLevel = EnumChannelAccessLevel.ChatMember,
        WriteAccessLevel = EnumChannelAccessLevel.ChatHost,
        ValidationMask = Tokens.ChannelPropTopicRegex,
        Value = string.Empty,
        ReadOnly = false,
        PostRule = (chatObject, propValue) =>
        {
            ((Channel)chatObject).Props[Tokens.ChannelPropTopic] = propValue;
            ((Channel)chatObject).SendTopic();
        }
    };

    public static Dictionary<string?, PropRule> PropRules = new()
    {
        { Tokens.ChannelPropAccount, Account },
        { Tokens.ChannelPropClient, Client },
        { Tokens.ChannelPropClientGuid, ClientGuid },
        { Tokens.ChannelPropCreation, Creation },
        { Tokens.ChannelPropHostkey, Hostkey },
        { Tokens.ChannelPropLag, Lag },
        { Tokens.ChannelPropLanguage, Language },
        { Tokens.ChannelPropMemberkey, Memberkey },
        { Tokens.ChannelPropName, Name },
        { Tokens.ChannelPropOID, Oid },
        { Tokens.ChannelPropOnJoin, Onjoin },
        { Tokens.ChannelPropOnPart, Onpart },
        { Tokens.ChannelPropOwnerkey, Ownerkey },
        { Tokens.ChannelPropPICS, Pics },
        { Tokens.ChannelPropServicePath, Servicepath },
        { Tokens.ChannelPropSubject, Subject },
        { Tokens.ChannelPropTopic, Topic }
    };
}