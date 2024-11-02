using Irc.Enumerations;
using Irc.Interfaces;
using Irc.Objects;
using Irc.Resources;

namespace Irc.Commands;

public class Equestion : Command, ICommand
{
    public Equestion() : base(3)
    {
    }

    public new EnumCommandDataType GetDataType()
    {
        return EnumCommandDataType.None;
    }

    // EQUESTION %#OnStage Nickname :Why am I here?
    public new void Execute(IChatFrame chatFrame)
    {
        var targetName = chatFrame.Message.Parameters.First();
        var nickname = chatFrame.Message.Parameters[1];
        var message = chatFrame.Message.Parameters[2];

        string?[] targets = targetName.Split(',', StringSplitOptions.RemoveEmptyEntries);
        foreach (var target in targets)
        {
            if (!Channel.ValidName(target))
            {
                chatFrame.User.Send(Raw.IRCX_ERR_NOSUCHCHANNEL_403(chatFrame.Server, chatFrame.User, target));
                return;
            }

            var chatObject = (ChatObject)chatFrame.Server.GetChannelByName(target);
            var channel = (Channel)chatObject;
            var channelMember = channel.GetMember(chatFrame.User);
            var isOnChannel = channelMember != null;

            if (!isOnChannel)
            {
                chatFrame.User.Send(
                    Raw.IRCX_ERR_NOTONCHANNEL_442(chatFrame.Server, chatFrame.User, channel));
                return;
            }

            if (!channel.OnStage)
            {
                chatFrame.User.Send(
                    Raw.IRCX_ERR_CANNOTSENDTOCHAN_404(chatFrame.Server, chatFrame.User, channel));
                return;
            }

            SubmitQuestion(chatFrame.User, channel, nickname, message);
        }
    }

    public static void SubmitQuestion(User? user, Channel? channel, string? nickname, string? message)
    {
        channel.Send(IrcxRaws.RPL_EQUESTION(user, channel, nickname, message));
    }
}