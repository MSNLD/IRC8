using Irc.Enumerations;
using Irc.Helpers;
using Irc.Objects;
using Irc.Resources;

namespace Irc.Commands;

internal class Listx : Command
{
    public Listx() : base(1)
    {
    }

    public override EnumCommandDataType GetDataType()
    {
        return EnumCommandDataType.None;
    }

    public override void Execute(ChatFrame chatFrame)
    {
        var server = chatFrame.Server;
        var user = chatFrame.User;
        var parameters = chatFrame.Message.Parameters;


        var channels = server.GetChannels();
        var firstParam = parameters.FirstOrDefault();

        if (firstParam != null && Channel.ValidName(firstParam))
        {
            channels = new List<Channel?>();
            var channelNames = Tools.CSVToArray(firstParam);
            foreach (var channelName in channelNames)
                if (Channel.ValidName(channelName))
                {
                    var channel = server.GetChannelByName(channelName);

                    if (channel == null)
                    {
                        user.Send(Raw.IRCX_ERR_BADCOMMAND_900(server, user, nameof(Listx)));
                        return;
                    }

                    channels.Add(channel);
                }
        }

        ListChannels(server, user, channels);
    }

    public static void ListChannels(Server server, User? user, IList<Channel?> channels)
    {
        // Case "811"      ' Start of LISTX
        user.Send(Raw.IRCX_RPL_LISTXSTART_811(server, user));
        foreach (var channel in channels)
            if (user.IsOn(channel) ||
                user.GetLevel() >= EnumUserAccessLevel.Guide ||
                (!((Channel)channel).Secret && !channel.Private))
                //  :TK2CHATCHATA04 812 'Admin_Koach %#Roomname +tnfSl 0 50 :%Chatroom\c\bFor\bBL\bGames\c\bFun\band\bEvents.
                user.Send(Raw.IRCX_RPL_LISTXLIST_812(
                    server,
                    user,
                    channel,
                    string.Join("", channel.Modes.Keys),
                    channel.GetMembers().Count,
                    channel.UserLimit,
                    channel.Props[IrcStrings.ChannelPropTopic] ?? string.Empty
                ));
        user.Send(Raw.IRCX_RPL_LISTXEND_817(server, user));
    }
}