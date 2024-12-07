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
        { Tokens.MemberModeHost, new Operator() },
        { Tokens.MemberModeVoice, new Voice() },
        { Tokens.ChannelModePrivate, new Private() },
        { Tokens.ChannelModeSecret, new Secret() },
        { Tokens.ChannelModeHidden, new Hidden() },
        { Tokens.ChannelModeInvite, new InviteOnly() },
        { Tokens.ChannelModeTopicOp, new TopicOp() },
        { Tokens.ChannelModeNoExtern, new NoExtern() },
        { Tokens.ChannelModeModerated, new Moderated() },
        { Tokens.ChannelModeUserLimit, new UserLimit() },
        { Tokens.ChannelModeBan, new BanList() },
        { Tokens.ChannelModeKey, new Key() },

        // IRCX
        { Tokens.ChannelModeAuthOnly, new AuthOnly() },
        { Tokens.ChannelModeProfanity, new NoFormat() },
        { Tokens.ChannelModeRegistered, new Registered() },
        { Tokens.ChannelModeKnock, new Knock() },
        { Tokens.ChannelModeNoWhisper, new NoWhisper() },
        { Tokens.ChannelModeAuditorium, new Auditorium() },
        { Tokens.ChannelModeCloneable, new Cloneable() },
        { Tokens.ChannelModeClone, new Clone() },
        { Tokens.ChannelModeService, new Service() },
        { Tokens.MemberModeOwner, new Owner() },

        // Apollo
        { Tokens.ChannelModeNoGuestWhisper, new NoGuestWhisper() },
        { Tokens.ChannelModeOnStage, new OnStage() },
        { Tokens.ChannelModeSubscriber, new Subscriber() }
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