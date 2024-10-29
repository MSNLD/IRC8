using Irc.Enumerations;
using Irc.Interfaces;
using Irc.Resources;

namespace Irc.Props.User
{
    public class Msnprofile : PropRule
    {
        public Msnprofile() : base(IrcStrings.UserPropMsnProfile, EnumChannelAccessLevel.ChatMember,
            EnumChannelAccessLevel.ChatMember, IrcStrings.GenericProps, "0", true)
        {
        }

        public override EnumIrcError EvaluateSet(IChatObject source, IChatObject target, string propValue)
        {
            if (source != target) return EnumIrcError.ERR_NOPERMS;

            var user = (Objects.User.User)source;
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
        }
    }
}