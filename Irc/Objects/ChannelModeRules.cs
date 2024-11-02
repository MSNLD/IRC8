using Irc.Modes;
using Irc.Modes.Channel;
using Irc.Modes.Channel.Member;
using Irc.Resources;

namespace Irc.Objects;

public static class ChannelModeRules
{
    /*
o - give/take channel operator privileges;
p - private channel flag;
s - secret channel flag;
i - invite-only channel flag;
t - topic settable by channel operator only flag;
n - no messages to channel from clients on the outside;
m - moderated channel;
l - set the user limit to channel;
b - set a ban mask to keep users out;
v - give/take the ability to speak on a moderated channel;
k - set a channel key (password).
*/

    public static Dictionary<char, ModeRule> ModeRules = new()
    {
        // IRC
        { IrcStrings.MemberModeHost, new Operator() },
        { IrcStrings.MemberModeVoice, new Voice() },
        { IrcStrings.ChannelModePrivate, new Private() },
        { IrcStrings.ChannelModeSecret, new Secret() },
        { IrcStrings.ChannelModeHidden, new Hidden() },
        { IrcStrings.ChannelModeInvite, new InviteOnly() },
        { IrcStrings.ChannelModeTopicOp, new TopicOp() },
        { IrcStrings.ChannelModeNoExtern, new NoExtern() },
        { IrcStrings.ChannelModeModerated, new Moderated() },
        { IrcStrings.ChannelModeUserLimit, new UserLimit() },
        { IrcStrings.ChannelModeBan, new BanList() },
        { IrcStrings.ChannelModeKey, new Key() },

        // IRCX
        { IrcStrings.ChannelModeAuthOnly, new AuthOnly() },
        { IrcStrings.ChannelModeProfanity, new NoFormat() },
        { IrcStrings.ChannelModeRegistered, new Registered() },
        { IrcStrings.ChannelModeKnock, new Knock() },
        { IrcStrings.ChannelModeNoWhisper, new NoWhisper() },
        { IrcStrings.ChannelModeAuditorium, new Auditorium() },
        { IrcStrings.ChannelModeCloneable, new Cloneable() },
        { IrcStrings.ChannelModeClone, new Clone() },
        { IrcStrings.ChannelModeService, new Service() },
        { IrcStrings.MemberModeOwner, new Owner() },

        // Apollo
        { IrcStrings.ChannelModeNoGuestWhisper, new NoGuestWhisper() },
        { IrcStrings.ChannelModeOnStage, new OnStage() },
        { IrcStrings.ChannelModeSubscriber, new Subscriber() }
    };

    // public override string ToString()
    // {
    //     // TODO: <MODESTRING> Fix the below for Limit and Key on mode string
    //     var limit = base.Modes['l'].Get() > 0 ? $" {base.Modes['l'].Get()}" : string.Empty;
    //     var key = base.Modes['k'].Get() != 0 ? $" {keypass}" : string.Empty;
    //
    //     return
    //         $"{new string(base.Modes.Where(mode => mode.Value.Get() > 0).Select(mode => mode.Key).ToArray())}{limit}{key}";
    // }
}