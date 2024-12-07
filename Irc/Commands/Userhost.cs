using Irc.Resources;

namespace Irc.Commands;

internal class Userhost : Command
{
    public Userhost() : base(1)
    {
    }

    public override void Execute(ChatFrame chatFrame)
    {
        var maxUsers = 30;
        var users = chatFrame.Server.GetUsersByList(chatFrame.Message.GetParameters(), ' ');
        if (users.Count < maxUsers)
            foreach (var user in users)
                chatFrame.User.Send(Raws.IRCX_RPL_USERHOST_302(chatFrame.Server, user));
        else
            chatFrame.User.Send(Raws.IRCX_ERR_TOOMANYARGUMENTS_901(chatFrame.Server, chatFrame.User, GetName()));
        // :sky-8a15b323126 901 Sky2k USERHOST :Too many arguments
    }
}