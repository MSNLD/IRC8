namespace Irc.Commands;

internal class Away : Command
{
    public override void Execute(ChatFrame chatFrame)
    {
        var server = chatFrame.Server;
        var user = chatFrame.User;
        if (chatFrame.Message.Parameters.Count == 0)
        {
            user.SetBack();
            return;
        }

        var reason = chatFrame.Message.Parameters.First();
        user.SetAway(reason);
    }
}