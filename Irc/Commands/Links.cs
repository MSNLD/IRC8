using Irc.Resources;

namespace Irc.Commands;

public class Links : Command
{
    public override void Execute(ChatFrame chatFrame)
    {
        /*
         -> sky-8a15b323126 LINKS
         <- :sky-8a15b323126 364 Sky sky-8a15b323126 sky-8a15b323126 :0 P0 Microsoft Exchange Chat Service
         <- :sky-8a15b323126 365 Sky * :End of /LINKS list.
         */
        var linkCount = 0;
        chatFrame.User.Send(IrcRaws.IRC_RAW_364(chatFrame.Server, chatFrame.User, chatFrame.Server.ToString(),
            linkCount));
        chatFrame.User.Send(IrcRaws.IRC_RAW_365(chatFrame.Server, chatFrame.User, "*"));
    }
}