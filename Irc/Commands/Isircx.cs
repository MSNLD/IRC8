using Irc.Resources;

namespace Irc.Commands;

internal class Isircx : Command
{
    public Isircx() : base(0, false)
    {
    }

    public override void Execute(ChatFrame chatFrame)
    {
        chatFrame.User.Send(Raws.IRCX_ERR_NOTIMPLEMENTED(chatFrame.Server, chatFrame.User, nameof(Access)));
    }
}