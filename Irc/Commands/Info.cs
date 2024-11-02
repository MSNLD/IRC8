using Irc.Enumerations;

namespace Irc.Commands;

internal class Info : Command
{
    public override EnumCommandDataType GetDataType()
    {
        return EnumCommandDataType.None;
    }

    public override void Execute(ChatFrame chatFrame)
    {
        chatFrame.User.Send(Raw.IRCX_RPL_RPL_INFO_371_VERS(chatFrame.Server, chatFrame.User,
            chatFrame.Server.ServerVersion));
        chatFrame.User.Send(Raw.IRCX_RPL_RPL_INFO_371_RUNAS(chatFrame.Server, chatFrame.User));
        chatFrame.User.Send(Raw.IRCX_RPL_RPL_INFO_371_UPTIME(chatFrame.Server, chatFrame.User,
            chatFrame.Server.ServerSettings.Creation));
        chatFrame.User.Send(Raw.IRCX_RPL_RPL_ENDOFINFO_374(chatFrame.Server, chatFrame.User));
    }
}