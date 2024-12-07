using Irc.Objects;
using Irc.Resources;

namespace Irc.Commands;

internal class Names : Command
{
    public Names() : base(1)
    {
    }

    public override void Execute(ChatFrame chatFrame)
    {
        var user = chatFrame.User;
        string?[] channelNames = chatFrame.Message.Parameters.First()
            .Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries);

        foreach (var channelName in channelNames)
        {
            var channel = chatFrame.Server.GetChannelByName(channelName.Trim());

            if (channel != null)
            {
                if (user.IsOn(channel) || channel is { Private: false, Secret: false })
                    ProcessNamesReply(user, channel);
            }
            else
            {
                chatFrame.User.Send(Raws.IRCX_ERR_NOSUCHCHANNEL_403(chatFrame.Server, chatFrame.User, channelName));
            }
        }
    }

    public static void ProcessNamesReply(Objects.User? user, Channel channel)
    {
        // RFC 2812 "=" for others(public channels).
        var channelType = '=';

        if (channel.Secret)
            // RFC 2812 "@" is used for secret channels
            channelType = '@';
        else if (channel.Private)
            // RFC 2812 "*" for private
            channelType = '*';

        user.Send(
            Raws.IRCX_RPL_NAMEREPLY_353(user.Server, user, channel, channelType,
                string.Join(' ',
                    channel.GetMembers().Select(m =>
                        $"{user.Protocol.FormattedUser(m)}"
                    )
                )
            )
        );
        user.Send(Raws.IRCX_RPL_ENDOFNAMES_366(user.Server, user, channel));
    }
}