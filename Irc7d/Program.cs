using System.Net;
using System.Reflection;
using System.Text.Json;
using Irc.Extensions.Apollo.Directory;
using Irc.IO;
using Irc.Logger;
using Irc.Objects;
using Irc.Security;
using Irc.Security.Credentials;
using Microsoft.Extensions.CommandLineUtils;
using NLog;

namespace Irc7d;

internal class Program
{
    public static readonly Logger Log = LogManager.GetCurrentClassLogger();

    private static Server? server;
    private static CancellationTokenSource? cancellationTokenSource;

    private static void Main(string[] args)
    {
        Logging.Attach();
        var app = new CommandLineApplication();
        app.Name = AppDomain.CurrentDomain.FriendlyName;
        app.Description = "irc7 daemon";
        app.HelpOption("-h|--help");

        var configOption = app.Option("-c|--config <configfile>",
            "The path to the configuration json file (default ./config.json)", CommandOptionType.SingleValue);
        var bindIpOption = app.Option("-i|--ip <bindip>", "The ip for the server to bind on (default 0.0.0.0)",
            CommandOptionType.SingleValue);
        var bindOption = app.Option("-p|--port <bindport>", "The port for the server to bind on (default 6667)",
            CommandOptionType.SingleValue);
        var backlogOption = app.Option("-k|--backlog <backlogsize>",
            "The backlog for connecting sockets (default 1024)", CommandOptionType.SingleValue);
        var bufferSizeOption = app.Option("-z|--buffer <buffersize>", "The incoming buffer size in bytes (default 512)",
            CommandOptionType.SingleValue);
        var maxConnectionsPerIpOption = app.Option("-m|--maxconn <maxconnections>",
            "The maximum connections per IP that can connect (default 128)", CommandOptionType.SingleValue);
        var fqdnOption = app.Option("-f|--fqdn <fqdn>", "The FQDN of the machine (default localhost)",
            CommandOptionType.SingleValue);
        var serverType = app.Option("-t|--type <type>",
            "Type of server e.g. IRC, IRCX, MSN, DIR", CommandOptionType.SingleValue);
        var chatServerIP = app.Option("-s|--server",
            "The Chat Server IP and Port e.g. 127.0.0.1:6667 (temporary, DIR mode only)",
            CommandOptionType.SingleValue);
        var versionOption = app.Option("-v|--version", "The version of the Chat Server", CommandOptionType.NoValue);

        app.OnExecute(async () =>
        {
            if (app.OptionHelp.HasValue())
            {
                app.ShowHint();
                return 0;
            }

            if (versionOption.HasValue())
            {
                Log.Info(Assembly.GetExecutingAssembly().GetName().Version?.ToString());
                return 0;
            }

            var ip = IPAddress.Any;
            if (bindIpOption.HasValue()) ip = IPAddress.Parse(bindIpOption.Value());

            var port = 6667;
            if (bindOption.HasValue()) port = Convert.ToInt32(bindOption.Value());

            var backlog = 1024;
            if (backlogOption.HasValue()) backlog = Convert.ToInt32(backlogOption.Value());

            var bufferSize = 512;
            if (bufferSizeOption.HasValue()) bufferSize = Convert.ToInt32(bufferSizeOption.Value());

            var maxConnections = 128;
            if (maxConnectionsPerIpOption.HasValue())
                maxConnections = Convert.ToInt32(maxConnectionsPerIpOption.Value());

            var fqdn = "localhost";
            if (fqdnOption.HasValue()) fqdn = fqdnOption.Value();

            var forwardServer = "";
            if (chatServerIP.HasValue()) forwardServer = chatServerIP.Value();

            Enum.TryParse<IrcType>(serverType.Value(), true, out var type);

            var socketServer = new SocketServer(ip, port, backlog, maxConnections, bufferSize);
            socketServer.OnListen += (sender, server1) =>
            {
                Log.Info(
                    $"Listening on {ip}:{port} backlog={backlog} buffer={bufferSize} maxconn={maxConnections} fqdn={fqdn} type={type} {(chatServerIP.HasValue() ? "forwardserver=" : "")}{forwardServer}");
            };

            var credentials = new Dictionary<string, Credential>();
            if (File.Exists("DefaultCredentials.json"))
                credentials =
                    JsonSerializer.Deserialize<Dictionary<string, Credential>>(
                        await File.ReadAllTextAsync("DefaultCredentials.json"));

            if (credentials == null)
            {
                Log.Info("No credentials found.");
                return 0;
            }

            var credentialProvider = new NTLMCredentials(credentials);
            var settings = JsonSerializer.Deserialize<Settings>(File.ReadAllText("DefaultServer.json")) ??
                           new Settings();

            switch (type)
            {
                case IrcType.Dir:
                {
                    server = new DirectoryServer(socketServer,
                        new SecurityManager(),
                        settings,
                        new List<Channel?>(),
                        credentialProvider);

                    string?[] parts = forwardServer.Split(':');
                    if (parts.Length > 0) ((DirectoryServer)server).ChatServerIp = parts[0];
                    if (parts.Length > 1) ((DirectoryServer)server).ChatServerPort = parts[1];

                    break;
                }
                default:
                {
                    server = new Server(socketServer,
                        new SecurityManager(),
                        settings,
                        new List<Channel?>(),
                        credentialProvider);
                    break;
                }
            }

            server.ServerVersion = Assembly.GetExecutingAssembly().GetName().Version ?? new Version(1, 0, 0);
            server.RemoteIP = fqdn;

            var defaultChannels =
                JsonSerializer.Deserialize<List<DefaultChannel>>(File.ReadAllText("DefaultChannels.json"));
            if (defaultChannels != null)
                foreach (var defaultChannel in defaultChannels)
                {
                    var name = $"%#{defaultChannel.Name}";
                    var channel = server.CreateChannel(name);
                    channel.Props["TOPIC"] = defaultChannel.Topic;
                    foreach (var keyValuePair in defaultChannel.Modes)
                        channel.Modes[keyValuePair.Key] = keyValuePair.Value;

                    foreach (var keyValuePair in defaultChannel.Props)
                        channel.Props[keyValuePair.Key] = keyValuePair.Value;

                    server.AddChannel(channel);
                }

            cancellationTokenSource = new CancellationTokenSource();
            AppDomain.CurrentDomain.ProcessExit += CurrentDomain_ProcessExit;
            Console.CancelKeyPress += CurrentDomain_ProcessExit;
            await Task.Delay(-1, cancellationTokenSource.Token).ContinueWith(t => { });

            return 0;
        });

        app.Execute(args);
    }

    private static void CurrentDomain_ProcessExit(object? sender, EventArgs e)
    {
        Log.Info("Shutting down...");
        server?.Shutdown();
        cancellationTokenSource?.Cancel();
    }

    private enum IrcType
    {
        Chat,
        Dir
    }
}