using Irc.Commands;
using Irc.Enumerations;
using Irc.Interfaces;
using Irc.Objects;
using Version = Irc.Commands.Version;

namespace Irc.Protocols;

public class Protocol : IProtocol
{
    protected Dictionary<string?, ICommand> Commands = new(StringComparer.InvariantCultureIgnoreCase);

    public Protocol()
    {
        AddCommand(new Auth());
        AddCommand(new Ircvers());
        AddCommand(new Ircx());
        AddCommand(new Privmsg());
        AddCommand(new Notice());
        AddCommand(new Ping());
        AddCommand(new Nick());
        AddCommand(new UserCommand(), "User");
        AddCommand(new List());
        AddCommand(new Mode());
        AddCommand(new Join());
        AddCommand(new Part());
        AddCommand(new Kick());
        AddCommand(new Kill());
        AddCommand(new Names());
        AddCommand(new Userhost());
        AddCommand(new Version());
        AddCommand(new Info());
        AddCommand(new Pong());
        AddCommand(new Pass());
        AddCommand(new Quit());
        AddCommand(new Trace());
        AddCommand(new Ison());
        AddCommand(new Time());
        AddCommand(new Admin());
        AddCommand(new Links());
        AddCommand(new Who());
        AddCommand(new Whois());
        AddCommand(new Users());
        AddCommand(new Topic());
        AddCommand(new Invite());
        AddCommand(new WebIrc());

        // IRCX
        AddCommand(new Commands.Access());
        AddCommand(new Away());
        AddCommand(new Create());
        AddCommand(new Data());
        AddCommand(new Event());
        AddCommand(new Isircx());
        AddCommand(new Kill());
        AddCommand(new Listx());
        AddCommand(new Reply());
        AddCommand(new Request());
        AddCommand(new Whisper());
        AddCommand(new Auth());
        AddCommand(new Ircx());
        AddCommand(new Prop());
        AddCommand(new Listx());

        // IRC3
        AddCommand(new Goto());
        AddCommand(new Esubmit());
        AddCommand(new Eprivmsg());
        AddCommand(new Equestion());
    }

    public EnumProtocolType Ircvers { get; set; } = EnumProtocolType.IRC;

    public ICommand? GetCommand(string? name)
    {
        Commands.TryGetValue(name, out var command);
        return command;
    }

    public Dictionary<string?, ICommand> GetCommands()
    {
        return Commands;
    }

    public void AddCommand(ICommand command, string? name = null)
    {
        if (!Commands.ContainsKey(name == null ? command.GetName() : name))
            Commands.Add(name ?? command.GetName(), command);
    }

    public void SetVers(EnumProtocolType ircvers)
    {
        Ircvers = ircvers;
    }

    public void FlushCommands()
    {
        Commands.Clear();
    }

    public string GetFormat(User user)
    {
        return GetFormat(Ircvers, user);
    }

    public string FormattedUser(Member member)
    {
        var modeChar = string.Empty;
        if (!member.IsNormal()) modeChar += member.IsOwner() ? '.' : member.IsHost() ? '@' : '+';

        var profile = GetFormat(Ircvers, member.GetUser());

        return $"{profile}{modeChar}{member.GetUser().GetAddress().Nickname}";
    }

    public static string GetFormat(EnumProtocolType ircvers, User user)
    {
        switch ((int)ircvers)
        {
            case 5:
            case 6:
                return $"{user.GetProfile().Irc5_ToString()},";
            case 7:
                return $"{user.GetProfile().Irc7_ToString()},";
            case 8:
                return $"{user.GetProfile()},";
            default:
                return "";
        }
    }

    public void UpdateCommand(ICommand command, string? name = null)
    {
        var commandName = name ?? command.GetName();
        if (Commands.ContainsKey(commandName))
            Commands[commandName] = command;
    }
}