using Irc.Resources;

namespace Irc.Commands;

internal class Trace : Command
{
    public override void Execute(ChatFrame chatFrame)
    {
        chatFrame.User.Send(Raws.IRCX_ERR_COMMANDUNSUPPORTED_554(chatFrame.Server, chatFrame.User, nameof(Trace)));
    }
}