using Irc.Enumerations;
using Irc.Objects;
using Irc.Resources;

namespace Irc.Props;

public class UserPropCollection : PropCollection
{
    public static PropRule Msnprofile = new()
    {
        Name = IrcStrings.UserPropMsnProfile,
        ReadAccessLevel = EnumChannelAccessLevel.ChatMember,
        WriteAccessLevel = EnumChannelAccessLevel.ChatMember,
        ValidationMask = IrcStrings.GenericProps,
        Value = "0",
        ReadOnly = true,
        PostRule = (chatObject, propValue) =>
        {
            // TODO: Fix below
            /*
              if (source != target) return EnumIrcError.ERR_NOPERMS;

               var user = (Objects.User)source;
               if (int.TryParse(propValue, out var result))
               {
                   var profile = user.GetProfile();
                   if (profile.HasProfile)
                   {
                       user.Send(Raw.IRCX_ERR_ALREADYREGISTERED_462(user.Server, user));
                       return EnumIrcError.OK;
                   }

                   profile.SetProfileCode(result);
                   return EnumIrcError.OK;
               }

               return EnumIrcError.ERR_BADVALUE;
             */
        }
    };

    public static PropRule Nick = new()
    {
        Name = IrcStrings.UserPropNickname,
        ReadAccessLevel = EnumChannelAccessLevel.ChatMember,
        WriteAccessLevel = EnumChannelAccessLevel.None,
        ValidationMask = IrcStrings.GenericProps,
        Value = string.Empty,
        ReadOnly = true
    };

    public static PropRule Role = new()
    {
        Name = IrcStrings.UserPropRole,
        ReadAccessLevel = EnumChannelAccessLevel.None,
        WriteAccessLevel = EnumChannelAccessLevel.ChatMember,
        ValidationMask = IrcStrings.GenericProps,
        Value = string.Empty,
        ReadOnly = true,
        PostRule = (chatObject, propValue) =>
        {
            ((User)chatObject).Server.ProcessCookie((User)chatObject,
                IrcStrings.UserPropRole, propValue);
        }
    };

    public static PropRule Subscriberinfo = new()
    {
        Name = IrcStrings.UserPropSubscriberInfo,
        ReadAccessLevel = EnumChannelAccessLevel.None,
        WriteAccessLevel = EnumChannelAccessLevel.ChatMember,
        ValidationMask = IrcStrings.GenericProps,
        Value = "0",
        ReadOnly = true,
        PostRule = (chatObject, propValue) =>
        {
            ((User)chatObject).Server.ProcessCookie((User)chatObject,
                IrcStrings.UserPropSubscriberInfo, propValue);
        }
    };

    public static PropRule Oid = new()
    {
        Name = IrcStrings.UserPropOid,
        ReadAccessLevel = EnumChannelAccessLevel.ChatMember,
        WriteAccessLevel = EnumChannelAccessLevel.None,
        ValidationMask = IrcStrings.ChannelPropOIDRegex,
        Value = "0",
        ReadOnly = true
    };

    public static Dictionary<string?, PropRule> PropRules = new()
    {
        { IrcStrings.UserPropOid, Oid },
        { IrcStrings.UserPropNickname, Nick },
        { IrcStrings.UserPropSubscriberInfo, Subscriberinfo },
        { IrcStrings.UserPropMsnProfile, Msnprofile },
        { IrcStrings.UserPropRole, Role }
    };
}