using Irc.Commands;
using Irc.Enumerations;

namespace Irc.Extensions.Apollo.Directory.Commands;

internal class Create : Command
{
    public Create()
    {
        RequiredMinimumParameters = 1;
    }

    public override EnumCommandDataType GetDataType()
    {
        return EnumCommandDataType.None;
    }

    public override void Execute(ChatFrame chatFrame)
    {
        chatFrame.User?.Send(Raw.IRCX_RPL_FINDS_613(chatFrame.Server, chatFrame.User));
    }
}