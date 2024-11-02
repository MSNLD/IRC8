using Irc.Enumerations;

namespace Irc.Commands;

internal class Whowas : Command
{
    public override EnumCommandDataType GetDataType()
    {
        return EnumCommandDataType.None;
    }

    public override void Execute(ChatFrame chatFrame)
    {
        chatFrame.User.Send(Raw.IRCX_ERR_COMMANDUNSUPPORTED_554(chatFrame.Server, chatFrame.User, nameof(Whowas)));
    }
}