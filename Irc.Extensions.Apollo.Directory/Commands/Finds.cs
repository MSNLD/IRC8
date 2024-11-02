using Irc.Commands;
using Irc.Enumerations;

namespace Irc.Extensions.Apollo.Directory.Commands;

internal class Finds : Command
{
    public Finds()
    {
        RequiredMinimumParameters = 1;
    }

    public override EnumCommandDataType GetDataType()
    {
        return EnumCommandDataType.None;
    }

    public override void Execute(ChatFrame chatFrame)
    {
        chatFrame.User?.Send(ApolloDirectoryRaws.RPL_FINDS_MSN((DirectoryServer)chatFrame.Server, chatFrame.User));
    }
}