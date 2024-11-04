using Irc.Objects;
using Irc.Resources;

namespace Irc.Commands;

public class Eprivmsg : Command
{
    public Eprivmsg() : base(2)
    {
    }

    // EPRIVMSG %#OnStage :Why am I here?
    public override void Execute(ChatFrame chatFrame)
    {
        var targetName = chatFrame.Message.Parameters.First();
        var message = chatFrame.Message.Parameters[1];

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

            SendEprivmsg(chatFrame.User, channel, message);
        }
    }

    public static void SendEprivmsg(User? user, Channel? channel, string? message)
    {
        channel.Send(IrcxRaws.RPL_EPRIVMSG(user, channel, message));
    }
}