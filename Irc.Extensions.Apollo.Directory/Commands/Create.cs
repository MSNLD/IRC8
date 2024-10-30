using Irc.Commands;
using Irc.Enumerations;
using Irc.Interfaces;
using Interfaces_ICommand = Irc.Interfaces.ICommand;

namespace Irc.Extensions.Apollo.Directory.Commands;

internal class Create : Command, Interfaces_ICommand
{
    public Create()
    {
        _requiredMinimumParameters = 1;
    }

    public EnumCommandDataType GetDataType()
    {
        return EnumCommandDataType.None;
    }

    public void Execute(IChatFrame chatFrame)
    {
        chatFrame.User.Send(Raw.IRCX_RPL_FINDS_613(chatFrame.Server, chatFrame.User));
    }
}