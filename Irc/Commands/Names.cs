using Irc.Enumerations;
using Irc.Interfaces;
using Irc.Objects.Channel;

namespace Irc.Commands;

internal class Names : Command, ICommand
{
    public Names() : base(1)
    {
    }

    public new EnumCommandDataType GetDataType()
    {
        return EnumCommandDataType.None;
    }

    public new void Execute(IChatFrame chatFrame)
    {
        var user = chatFrame.User;
        string?[] channelNames = chatFrame.Message.Parameters.First()
            .Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries);

        foreach (var channelName in channelNames)
        {
            var channel = chatFrame.Server.GetChannelByName(channelName.Trim());

            if (channel != null)
            {
                if (user.IsOn(channel) || ((Channel)channel) is { Private: false, Secret: false })
                    ProcessNamesReply(user, channel);
            }
            else
            {
                chatFrame.User.Send(Raw.IRCX_ERR_NOSUCHCHANNEL_403(chatFrame.Server, chatFrame.User, channelName));
            }
        }
    }

    public static void ProcessNamesReply(IUser? user, IChannel channel)
    {
        // RFC 2812 "=" for others(public channels).
        var channelType = '=';

        if (((Channel)channel).Secret)
            // RFC 2812 "@" is used for secret channels
            channelType = '@';
        else if (((Channel)channel).Private)
            // RFC 2812 "*" for private
            channelType = '*';

        user.Send(
            Raw.IRCX_RPL_NAMEREPLY_353(user.Server, user, channel, channelType,
                string.Join(' ',
                    channel.GetMembers().Select(m =>
                        $"{user.Protocol.FormattedUser(m)}"
                    )
                )
            )
        );
        user.Send(Raw.IRCX_RPL_ENDOFNAMES_366(user.Server, user, channel));
    }
}