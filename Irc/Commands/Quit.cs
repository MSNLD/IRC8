using Irc.Objects;
using Irc.Resources;

namespace Irc.Commands;

internal class Quit : Command
{
    public Quit() : base(0, false)
    {
    }

    public override void Execute(ChatFrame chatFrame)
    {
        var server = chatFrame.Server;
        var user = chatFrame.User;

        var quitMessage = IrcStrings.Connresetbypeer;
        if (chatFrame.Message.Parameters.Count > 0) quitMessage = chatFrame.Message.Parameters.First();

        QuitChannels(user, quitMessage);
    }

    public static void QuitChannels(User? user, string? message)
    {
        var users = new HashSet<User?>();

        var channels = user.Channels.Keys;

        foreach (var channel in channels)
        {
            foreach (var member in channel.GetMembers()) users.Add(member.GetUser());
            channel.Quit(user);
        }

        user.Channels.Clear();

        var quitRaw = IrcRaws.RPL_QUIT(user, message);

        foreach (var targetUser in users) targetUser.Send(quitRaw);
        user.Disconnect(quitRaw);
    }
}