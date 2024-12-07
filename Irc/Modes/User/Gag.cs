using Irc.Enumerations;
using Irc.Objects;
using Irc.Resources;

namespace Irc.Modes.User;

public class Gag : ModeRule
{
    /*
     * The GAG mode is applied by a sysop or sysop manager on a user
         and may not be removed except by a sysop or sysop manager.
         The user may not be notified when this mode is applied because
         the mode can be a more effective tool for keeping order if the
         user doesn't know exactly when it is applied.

         The server will discard all messages from a user with GAG mode
         to any other user or to any channel.

         MODE <nick> { + | - }z
    */

    public Gag() : base(Tokens.UserModeGag)
    {
    }

    public override EnumIrcError Evaluate(ChatObject source, ChatObject? target, bool flag, string? parameter)
    {
        if (source.Level >= EnumUserAccessLevel.Sysop)
        {
            if (source.Level < target.Level) return EnumIrcError.ERR_NOPERMS;

            target.Modes[Tokens.UserModeGag] = Convert.ToInt32(flag);
            DispatchModeChange(source, target, flag, parameter);
            return EnumIrcError.OK;
        }
        // :sky-8a15b323126 502 Sky :Cant change mode for other users

        return EnumIrcError.ERR_CANNOTSETFOROTHER;
    }
}