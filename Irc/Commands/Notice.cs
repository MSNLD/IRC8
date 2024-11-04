namespace Irc.Commands;

internal class Notice : Command
{
    public Notice() : base(2)
    {
    }

    public override void Execute(ChatFrame chatFrame)
    {
        Privmsg.SendMessage(chatFrame, true);
    }
}