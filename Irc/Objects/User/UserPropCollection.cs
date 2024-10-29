using Irc.Enumerations;
using Irc.Interfaces;
using Irc.IO;
using Irc.Objects.Collections;
using Irc.Objects.Server;
using Irc.Props;
using Irc.Props.User;
using Irc.Resources;

namespace Irc.Objects.User
{
    public class UserPropCollection : PropCollection
    {
        public static Dictionary<string, PropRule> PropRules = new()
        {
            { Resources.IrcStrings.UserPropOid, new OID() },
            { Resources.IrcStrings.UserPropNickname, new Nick() },
            { Resources.IrcStrings.UserPropSubscriberInfo, new SubscriberInfo() },
            { Resources.IrcStrings.UserPropMsnProfile, new Msnprofile() },
            { Resources.IrcStrings.UserPropRole, new Role() },
        };
    }
}