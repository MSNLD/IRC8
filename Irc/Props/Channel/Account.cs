using Irc.Enumerations;
using Irc.Resources;

namespace Irc.Props.Channel
{
    internal class Account : PropRule
    {
        // The ACCOUNT channel property contains an implementation-dependant string for attaching a security account.
        // This controls access to the channel using the native OS security system.
        // The ACCOUNT property is limited to 31 characters.
        // It can only be read by sysop managers, sysops and owners of the channel.

        public Account() : base(IrcStrings.ChannelPropAccount, EnumChannelAccessLevel.ChatHost,
            EnumChannelAccessLevel.ChatHost, IrcStrings.GenericProps, string.Empty, true)
        {
        }
    }
}
