using Irc.Enumerations;

namespace Irc.Commands;

internal class Notice : Command
{
    public Notice() : base(2)
    {
    }

    public override EnumCommandDataType GetDataType()
    {
        return EnumCommandDataType.Standard;
    }

    public override void Execute(ChatFrame chatFrame)
    {
        Privmsg.SendMessage(chatFrame, true);
    }
}