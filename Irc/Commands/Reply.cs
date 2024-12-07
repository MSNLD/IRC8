using Irc.Resources;

namespace Irc.Commands;

internal class Reply : Command
{
    public override void Execute(ChatFrame chatFrame)
    {
        chatFrame.User.Send(Raws.IRCX_ERR_NOTIMPLEMENTED(chatFrame.Server, chatFrame.User, nameof(Access)));
    }
}