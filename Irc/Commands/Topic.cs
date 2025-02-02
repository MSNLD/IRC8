﻿using Irc.Enumerations;
using Irc.Objects;
using Irc.Resources;

namespace Irc.Commands;

internal class Topic : Command
{
    public Topic() : base(2)
    {
    }

    public override void Execute(ChatFrame chatFrame)
    {
        var source = chatFrame.User;
        var channelName = chatFrame.Message.Parameters.First();
        var topic = chatFrame.Message.Parameters[1];

        if (chatFrame.Message.Parameters.Count > 2) topic = chatFrame.Message.Parameters[2];

        var channel = chatFrame.Server.GetChannelByName(channelName);
        if (channel == null)
        {
            chatFrame.User.Send(Raws.IRCX_ERR_NOSUCHCHANNEL_403(chatFrame.Server, chatFrame.User,
                chatFrame.Message.Parameters.First()));
        }
        else
        {
            var result = ProcessTopic(chatFrame, channel, source, topic);
            switch (result)
            {
                case EnumIrcError.ERR_NOTONCHANNEL:
                {
                    chatFrame.User.Send(Raws.IRCX_ERR_NOTONCHANNEL_442(chatFrame.Server, source, channel));
                    break;
                }
                case EnumIrcError.ERR_NOCHANOP:
                {
                    chatFrame.User.Send(
                        Raws.IRCX_ERR_CHANOPRIVSNEEDED_482(chatFrame.Server, source, channel));
                    break;
                }
                case EnumIrcError.OK:
                {
                    channel.Send(Raws.RPL_TOPIC_IRC(chatFrame.Server, source, channel, topic));
                    break;
                }
            }
        }
    }

    public static EnumIrcError ProcessTopic(ChatFrame chatFrame, Channel channel, Objects.User? source, string? topic)
    {
        if (!channel.CanBeModifiedBy((ChatObject)source)) return EnumIrcError.ERR_NOTONCHANNEL;

        var sourceMember = channel.GetMember(source);

        if (sourceMember.GetLevel() < EnumChannelAccessLevel.ChatHost && channel.TopicOp)
            return EnumIrcError.ERR_NOCHANOP;

        channel.Props[Tokens.ChannelPropTopic] = topic;
        return EnumIrcError.OK;
    }
}