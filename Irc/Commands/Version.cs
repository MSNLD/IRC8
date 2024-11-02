using Irc.Enumerations;

namespace Irc.Commands;

public class Version : Command
{
    public Version() : base(0, false)
    {
    }

    public override EnumCommandDataType GetDataType()
    {
        return EnumCommandDataType.None;
    }

    public override void Execute(ChatFrame chatFrame)
    {
        chatFrame.User.Send(Raw.IRCX_RPL_VERSION_351(chatFrame.Server, chatFrame.User, chatFrame.Server.ServerVersion));
    }
}