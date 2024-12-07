using Irc.Objects;
using Irc.Resources;

namespace Irc.Commands;

internal class List : Command
{
    public override void Execute(ChatFrame chatFrame)
    {
        var server = chatFrame.Server;
        var user = chatFrame.User;
        var parameters = chatFrame.Message.Parameters;

        var channels = server.GetChannels().Where(c => c.Modes[Tokens.ChannelModeSecret] != 1).ToList();
        if (parameters.Count > 0)
        {
            List<string?> channelNames = parameters.First().Split(',', StringSplitOptions.RemoveEmptyEntries).ToList();

            channels = server
                .GetChannels()
                .Where(c => c.Modes[Tokens.ChannelModeSecret] != 1
                            && channelNames.Contains(c.GetName(), StringComparer.InvariantCultureIgnoreCase)).ToList();
        }

        ListChannels(server, user, channels);
    }

    public void ListChannels(Server server, Objects.User? user, IList<Channel?> channels)
    {
        user.Send(Raws.IRCX_RPL_MODE_321(server, user));
        foreach (var channel in channels) user.Send(Raws.IRCX_RPL_MODE_322(server, user, channel));
        user.Send(Raws.IRCX_RPL_MODE_323(server, user));
    }
}