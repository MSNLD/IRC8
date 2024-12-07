using Irc.Resources;

namespace Irc.Commands;

public class Pass : Command
{
    public Pass() : base(1, false)
    {
    }

    public override void Execute(ChatFrame chatFrame)
    {
        if (!chatFrame.User.IsRegistered())
            // TODO: Encrypt below pass
            chatFrame.User.Pass = chatFrame.Message.Parameters.First();
        else
            chatFrame.User.Send(Raws.IRC_RAW_462(chatFrame.Server, chatFrame.User));
    }
}