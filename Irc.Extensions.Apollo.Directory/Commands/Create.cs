using Irc.Commands;
using Irc.Resources;

namespace Irc.Extensions.Apollo.Directory.Commands;

internal class Create : Command
{
    public Create()
    {
        RequiredMinimumParameters = 1;
    }

    public override void Execute(ChatFrame chatFrame)
    {
        chatFrame.User?.Send(Raws.IRCX_RPL_FINDS_613(chatFrame.Server, chatFrame.User));
    }
}