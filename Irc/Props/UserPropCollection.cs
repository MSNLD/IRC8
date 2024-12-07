using Irc.Enumerations;
using Irc.Objects;
using Irc.Resources;

namespace Irc.Props;

public class UserPropCollection : PropCollection
{
    public static PropRule Msnprofile = new()
    {
        Name = Tokens.UserPropMsnProfile,
        ReadAccessLevel = EnumChannelAccessLevel.ChatMember,
        WriteAccessLevel = EnumChannelAccessLevel.ChatMember,
        ValidationMask = Tokens.GenericProps,
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
        Name = Tokens.UserPropNickname,
        ReadAccessLevel = EnumChannelAccessLevel.ChatMember,
        WriteAccessLevel = EnumChannelAccessLevel.None,
        ValidationMask = Tokens.GenericProps,
        Value = string.Empty,
        ReadOnly = true
    };

    public static PropRule Role = new()
    {
        Name = Tokens.UserPropRole,
        ReadAccessLevel = EnumChannelAccessLevel.None,
        WriteAccessLevel = EnumChannelAccessLevel.ChatMember,
        ValidationMask = Tokens.GenericProps,
        Value = string.Empty,
        ReadOnly = true,
        PostRule = (chatObject, propValue) =>
        {
            ((User)chatObject).Server.ProcessCookie((User)chatObject,
                Tokens.UserPropRole, propValue);
        }
    };

    public static PropRule Subscriberinfo = new()
    {
        Name = Tokens.UserPropSubscriberInfo,
        ReadAccessLevel = EnumChannelAccessLevel.None,
        WriteAccessLevel = EnumChannelAccessLevel.ChatMember,
        ValidationMask = Tokens.GenericProps,
        Value = "0",
        ReadOnly = true,
        PostRule = (chatObject, propValue) =>
        {
            ((User)chatObject).Server.ProcessCookie((User)chatObject,
                Tokens.UserPropSubscriberInfo, propValue);
        }
    };

    public static PropRule Oid = new()
    {
        Name = Tokens.UserPropOid,
        ReadAccessLevel = EnumChannelAccessLevel.ChatMember,
        WriteAccessLevel = EnumChannelAccessLevel.None,
        ValidationMask = Tokens.ChannelPropOIDRegex,
        Value = "0",
        ReadOnly = true
    };

    public static Dictionary<string?, PropRule> PropRules = new()
    {
        { Tokens.UserPropOid, Oid },
        { Tokens.UserPropNickname, Nick },
        { Tokens.UserPropSubscriberInfo, Subscriberinfo },
        { Tokens.UserPropMsnProfile, Msnprofile },
        { Tokens.UserPropRole, Role }
    };
}