using Irc.Interfaces;
using Irc.Modes.Channel.Member;
using Irc.Modes.User;
using Irc.Objects.Collections;
using Irc.Resources;

namespace Irc.Objects.User
{
    public class UserModes : ModeCollection, IModeCollection
    {
        public UserModes()
        {
            modes.Add(IrcStrings.UserModeOper, new Oper());
            modes.Add(IrcStrings.UserModeInvisible, new Invisible());
            modes.Add(IrcStrings.UserModeSecure, new Secure());
            //modes.Add(IrcStrings.UserModeServerNotice, new Modes.User.ServerNotice());
            //modes.Add(IrcStrings.UserModeWallops, new Modes.User.WallOps());

            //IRCX
            modes.Add(IrcStrings.UserModeAdmin, new Admin());
            modes.Add(IrcStrings.UserModeIrcx, new Isircx());
            modes.Add(IrcStrings.UserModeGag, new Gag());

            //Apollo
            modes.Add(IrcStrings.UserModeHost, new Host());
        }

        public bool Oper
        {
            get => modes[IrcStrings.UserModeOper].Get() == 1;
            set => modes[IrcStrings.UserModeOper].Set(Convert.ToInt32(value));
        }

        public bool Invisible
        {
            get => modes[IrcStrings.UserModeInvisible].Get() == 1;
            set => modes[IrcStrings.UserModeInvisible].Set(Convert.ToInt32(value));
        }

        public bool Secure
        {
            get => modes[IrcStrings.UserModeSecure].Get() == 1;
            set => modes[IrcStrings.UserModeSecure].Set(Convert.ToInt32(value));
        }
    }
}