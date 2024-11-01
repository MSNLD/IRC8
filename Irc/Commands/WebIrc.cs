using Irc.Enumerations;
using Irc.Interfaces;
using Irc.Objects.User;
using Irc.Resources;
using NLog;

namespace Irc.Commands;
// Implementation based on IRCv3 Spec
// URL: https://ircv3.net/specs/extensions/webirc.html

public class WebIrc : Command, ICommand
{
    public static readonly NLog.Logger Log = LogManager.GetCurrentClassLogger();

    public WebIrc() : base(0, false)
    {
    }

    public new EnumCommandDataType GetDataType()
    {
        return EnumCommandDataType.None;
    }

    public new void Execute(IChatFrame chatFrame)
    {
        var remoteAddress = chatFrame.User.GetConnection().GetIp();

        if (chatFrame.SequenceId > 1 ||
            chatFrame.User.IsAuthenticated() ||
            chatFrame.User.IsRegistered())
        {
            Reject(chatFrame, remoteAddress);
            return;
        }

        var whitelistedIp = chatFrame.Server.ServerSettings.WebIrcIp;
        if (remoteAddress != whitelistedIp || chatFrame.Message.Parameters.Count() < 4)
        {
            Reject(chatFrame, remoteAddress);
            return;
        }

        var parameters = chatFrame.Message.Parameters;
        var password = parameters.FirstOrDefault();
        var gateway = parameters[1];
        var hostname = parameters[2];
        var ip = parameters[3];

        var expectedUser = chatFrame.Server.ServerSettings.WebIrcUser;
        var expectedPassword = chatFrame.Server.ServerSettings.WebIrcPassword;
        if (expectedUser != gateway || expectedPassword != password)
        {
            Reject(chatFrame, remoteAddress);
            return;
        }

        if (!chatFrame.User.GetConnection().TryOverrideRemoteAddress(ip, hostname))
        {
            Reject(chatFrame, remoteAddress);
            return;
        }

        if (parameters.Count >= 5)
        {
            var optionStrings = parameters.Skip(4).ToArray();

            var options = new Dictionary<string, string>(
                optionStrings
                    .Select(x => x.Split(' ', StringSplitOptions.RemoveEmptyEntries))
                    .SelectMany(i => i).ToArray()
                    .Select(y =>
                    {
                        var parts = y.Split('=', StringSplitOptions.RemoveEmptyEntries);
                        var key = parts.FirstOrDefault();
                        var value = parts.Length > 1 ? parts[1] : string.Empty;
                        return new KeyValuePair<string, string>(key, value);
                    })
            );

            foreach (var option in options)
                if (option.Key.ToLowerInvariant() == IrcStrings.webirc_option_secure)
                {
                    chatFrame.User.Secure = true;
                }
        }
    }

    public void Reject(IChatFrame chatFrame, string remoteAddress)
    {
        Log.Warn($"Unauthorized WEBIRC attempt from {remoteAddress}");
        var originalCommand = chatFrame.Message.OriginalText.Split(' ', StringSplitOptions.RemoveEmptyEntries)
            .FirstOrDefault();
        chatFrame.User.Send(Raw.IRCX_ERR_UNKNOWNCOMMAND_421(chatFrame.Server, chatFrame.User, originalCommand));
    }
}