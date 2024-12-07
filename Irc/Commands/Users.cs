using Irc.Resources;

namespace Irc.Commands;

public class Users : Command
{
    public override void Execute(ChatFrame chatFrame)
    {
        // -> sky-8a15b323126 USERS
        // < - :sky - 8a15b323126 446 Sky2k: USERS has been disabled

        chatFrame.User.Send(Raws.IRC_RAW_446(chatFrame.Server, chatFrame.User));
    }
}