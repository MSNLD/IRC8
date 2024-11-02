using Irc.Enumerations;
using Irc.Interfaces;
using Irc.Objects;
using Irc.Resources;

namespace Irc.Commands;

public class Esubmit : Command, ICommand
{
    public Esubmit() : base(2)
    {
    }

    public new EnumCommandDataType GetDataType()
    {
        return EnumCommandDataType.None;
    }

    // ESUBMIT %#OnStage :Why am I here?
    public new void Execute(IChatFrame chatFrame)
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

            SubmitQuestion(chatFrame.User, channel, message);
        }
    }

    // TODO: Instead of EQUESTION this needs to be something else such as a EVENT etc

    public static void SubmitQuestion(User? user, Channel? channel, string? message)
    {
        channel.Send(IrcxRaws.RPL_EQUESTION(user, channel, user.ToString(), message));
    }
}