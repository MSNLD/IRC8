using Irc.Interfaces;
using Irc.Modes.Channel;
using Irc.Modes.Channel.Member;
using Irc.Objects.Collections;
using Irc.Resources;

namespace Irc.Objects.Channel
{
    public class ChannelModes : ModeCollection, IChannelModes
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

        public ChannelModes()
        {
            modes.Add(IrcStrings.MemberModeHost, new Operator());
            modes.Add(IrcStrings.MemberModeVoice, new Voice());
            modes.Add(IrcStrings.ChannelModePrivate, new Private());
            modes.Add(IrcStrings.ChannelModeSecret, new Secret());
            modes.Add(IrcStrings.ChannelModeHidden, new Hidden());
            modes.Add(IrcStrings.ChannelModeInvite, new InviteOnly());
            modes.Add(IrcStrings.ChannelModeTopicOp, new TopicOp());
            modes.Add(IrcStrings.ChannelModeNoExtern, new NoExtern());
            modes.Add(IrcStrings.ChannelModeModerated, new Moderated());
            modes.Add(IrcStrings.ChannelModeUserLimit, new UserLimit());
            modes.Add(IrcStrings.ChannelModeBan, new BanList());
            modes.Add(IrcStrings.ChannelModeKey, new Key());

            // IRCX

            modes.Add(IrcStrings.ChannelModeAuthOnly, new AuthOnly());
            modes.Add(IrcStrings.ChannelModeProfanity, new NoFormat());
            modes.Add(IrcStrings.ChannelModeRegistered, new Registered());
            modes.Add(IrcStrings.ChannelModeKnock, new Knock());
            modes.Add(IrcStrings.ChannelModeNoWhisper, new NoWhisper());
            modes.Add(IrcStrings.ChannelModeAuditorium, new Auditorium());
            modes.Add(IrcStrings.ChannelModeCloneable, new Cloneable());
            modes.Add(IrcStrings.ChannelModeClone, new Clone());
            modes.Add(IrcStrings.ChannelModeService, new Service());
            modes.Add(IrcStrings.MemberModeOwner, new Owner());
        
            // Apollo
            modes.Add(IrcStrings.ChannelModeNoGuestWhisper, new NoGuestWhisper());
            modes.Add(IrcStrings.ChannelModeOnStage, new OnStage());
            modes.Add(IrcStrings.ChannelModeSubscriber, new Subscriber());
        }

        // IRCX


        public bool Auditorium
        {
            get => modes[IrcStrings.ChannelModeAuditorium].Get() == 1;
            set => modes[IrcStrings.ChannelModeAuditorium].Set(Convert.ToInt32(value));
        }

        public bool InviteOnly
        {
            get => modes[IrcStrings.ChannelModeInvite].Get() == 1;
            set => modes[IrcStrings.ChannelModeInvite].Set(Convert.ToInt32(value));
        }

        public string Key
        {
            get => keypass;
            set
            {
                var hasKey = !string.IsNullOrWhiteSpace(value);
                modes[IrcStrings.ChannelModeKey].Set(hasKey);
                keypass = value;
            }
        }

        public bool Moderated
        {
            get => modes[IrcStrings.ChannelModeModerated].Get() == 1;
            set => modes[IrcStrings.ChannelModeModerated].Set(Convert.ToInt32(value));
        }

        public bool NoExtern
        {
            get => modes[IrcStrings.ChannelModeNoExtern].Get() == 1;
            set => modes[IrcStrings.ChannelModeNoExtern].Set(Convert.ToInt32(value));
        }

        public bool Private
        {
            get => modes[IrcStrings.ChannelModePrivate].Get() == 1;
            set => modes[IrcStrings.ChannelModePrivate].Set(Convert.ToInt32(value));
        }

        public bool Secret
        {
            get => modes[IrcStrings.ChannelModeSecret].Get() == 1;
            set => modes[IrcStrings.ChannelModeSecret].Set(Convert.ToInt32(value));
        }

        public bool Hidden
        {
            get => modes[IrcStrings.ChannelModeHidden].Get() == 1;
            set => modes[IrcStrings.ChannelModeHidden].Set(Convert.ToInt32(value));
        }

        public bool TopicOp
        {
            get => modes[IrcStrings.ChannelModeTopicOp].Get() == 1;
            set => modes[IrcStrings.ChannelModeTopicOp].Set(Convert.ToInt32(value));
        }

        public int UserLimit
        {
            get => modes[IrcStrings.ChannelModeUserLimit].Get();
            set => modes[IrcStrings.ChannelModeUserLimit].Set(value);
        }

        public bool NoGuestWhisper
        {
            get => modes[IrcStrings.ChannelModeNoGuestWhisper].Get() == 1;
            set => modes[IrcStrings.ChannelModeNoGuestWhisper].Set(Convert.ToInt32(value));
        }

        public bool AuthOnly
        {
            get => modes[IrcStrings.ChannelModeAuthOnly].Get() == 1;
            set => modes[IrcStrings.ChannelModeAuthOnly].Set(Convert.ToInt32(value));
        }

        public bool Profanity
        {
            get => modes[IrcStrings.ChannelModeProfanity].Get() == 1;
            set => modes[IrcStrings.ChannelModeProfanity].Set(Convert.ToInt32(value));
        }

        public bool Registered
        {
            get => modes[IrcStrings.ChannelModeRegistered].Get() == 1;
            set => modes[IrcStrings.ChannelModeRegistered].Set(Convert.ToInt32(value));
        }

        public bool Knock
        {
            get => modes[IrcStrings.ChannelModeKnock].Get() == 1;
            set => modes[IrcStrings.ChannelModeKnock].Set(Convert.ToInt32(value));
        }

        public bool NoWhisper
        {
            get => modes[IrcStrings.ChannelModeNoWhisper].Get() == 1;
            set => modes[IrcStrings.ChannelModeNoWhisper].Set(Convert.ToInt32(value));
        }

        public bool Cloneable
        {
            get => modes[IrcStrings.ChannelModeCloneable].Get() == 1;
            set => modes[IrcStrings.ChannelModeCloneable].Set(Convert.ToInt32(value));
        }

        public bool Clone
        {
            get => modes[IrcStrings.ChannelModeClone].Get() == 1;
            set => modes[IrcStrings.ChannelModeClone].Set(Convert.ToInt32(value));
        }

        public bool Service
        {
            get => modes[IrcStrings.ChannelModeService].Get() == 1;
            set => modes[IrcStrings.ChannelModeService].Set(Convert.ToInt32(value));
        }

        public override string ToString()
        {
            // TODO: <MODESTRING> Fix the below for Limit and Key on mode string
            var limit = modes['l'].Get() > 0 ? $" {modes['l'].Get()}" : string.Empty;
            var key = modes['k'].Get() != 0 ? $" {keypass}" : string.Empty;

            return
                $"{new string(modes.Where(mode => mode.Value.Get() > 0).Select(mode => mode.Key).ToArray())}{limit}{key}";
        }
    
        public bool OnStage
        {
            get => modes[IrcStrings.ChannelModeOnStage].Get() == 1;
            set => modes[IrcStrings.ChannelModeOnStage].Set(Convert.ToInt32(value));
        }

        public bool Subscriber
        {
            get => modes[IrcStrings.ChannelModeSubscriber].Get() == 1;
            set => modes[IrcStrings.ChannelModeSubscriber].Set(Convert.ToInt32(value));
        }
    }
}