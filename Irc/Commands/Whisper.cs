﻿using Irc.Enumerations;
using Irc.Objects;
using Irc.Resources;

namespace Irc.Commands;

internal class Whisper : Command
{
    public Whisper() : base(1, true, 3)
    {
    }

    public override void Execute(ChatFrame chatFrame)
    {
        // <sender> WHISPER <channel> <nick list> :<text>

        var server = chatFrame.Server;
        var user = chatFrame.User;

        if (chatFrame.Message.Parameters.Count == 1)
        {
            user.Send(Raws.IRC_ERR_NORECIPIENT_411(server, user, nameof(Whisper)));
            return;
        }

        if (chatFrame.Message.Parameters.Count == 2)
        {
            user.Send(Raws.IRC_ERR_NOTEXT_412(server, user, nameof(Whisper)));
            return;
        }

        var channelName = chatFrame.Message.Parameters.First();
        var channel = chatFrame.Server.GetChannelByName(channelName);
        if (channel == null)
        {
            user.Send(Raws.IRCX_ERR_NOSUCHCHANNEL_403(server, user, channelName));
            return;
        }

        var channelModes = channel.Modes;

        if (!user.IsOn(channel))
        {
            chatFrame.User.Send(
                Raws.IRCX_ERR_NOTONCHANNEL_442(server, user, channel));
            return;
        }

        if (channel.NoWhisper)
        {
            user.Send(Raws.IRCX_ERR_NOWHISPER_923(server, user, channel));
            return;
        }

        if (channel.NoGuestWhisper && user.IsGuest() && user.Level < EnumUserAccessLevel.Guide)
        {
            user.Send(Raws.IRCX_ERR_NOWHISPER_923(server, user, channel));
            return;
        }

        var targetNickname = chatFrame.Message.Parameters[1];
        var target = channel.GetMemberByNickname(targetNickname);
        if (target == null)
        {
            user.Send(Raws.IRCX_ERR_NOSUCHNICK_401(server, user, targetNickname));
            return;
        }

        var message = chatFrame.Message.Parameters[2];

        if (target.GetUser().Protocol.Ircvers < EnumProtocolType.IRCX)
            // PRIVMSG
            target.GetUser().Send(
                Raws.RPL_PRIVMSG_USER(chatFrame.Server, chatFrame.User, (ChatObject)target.GetUser(), message)
            );
        else
            target.GetUser().Send(
                Raws.RPL_CHAN_WHISPER(chatFrame.Server, chatFrame.User, channel, (ChatObject)target.GetUser(), message)
            );
    }
}