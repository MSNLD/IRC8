using Irc.Commands;
using Irc.Enumerations;
using Irc.Objects;

namespace Irc.Protocols;

public class Protocol
{
    public Dictionary<string, Command> Commands = new(StringComparer.InvariantCultureIgnoreCase);

    public Protocol()
    {
        AddCommand(new WebIrc());
        AddCommand(new Auth());
        AddCommand(new Ircvers());
        AddCommand(new Ircx());
        AddCommand(new Privmsg());
        AddCommand(new Notice());
        AddCommand(new Ping());
        AddCommand(new Nick());
        AddCommand(new Commands.User());
        AddCommand(new List());
        AddCommand(new Mode());
        AddCommand(new Join());
        AddCommand(new Part());
        AddCommand(new Kick());
        AddCommand(new Kill());
        AddCommand(new Names());
        AddCommand(new Userhost());
        AddCommand(new Pong());
        AddCommand(new Pass());
        AddCommand(new Quit());
        AddCommand(new Topic());
        AddCommand(new Away());
        AddCommand(new Isircx());
        AddCommand(new Whisper());
        AddCommand(new Prop());
        AddCommand(new Commands.Access());
    }

    public EnumProtocolType Ircvers { get; set; } = EnumProtocolType.IRC;

    public Command? GetCommand(string? name)
    {
        Commands.TryGetValue(name, out var command);
        return command;
    }

    public void AddCommand(Command command)
    {
        Commands.Add(command.GetType().Name, command);
    }

    public void FlushCommands()
    {
        Commands.Clear();
    }

    public string GetFormat(Objects.User user)
    {
        return GetFormat(Ircvers, user);
    }

    public string FormattedUser(Member member)
    {
        var modeChar = string.Empty;
        if (!member.IsNormal()) modeChar += member.IsOwner() ? '.' : member.IsHost() ? '@' : '+';

        var profile = GetFormat(Ircvers, member.GetUser());

        return $"{profile}{modeChar}{member.GetUser().Address.Nickname}";
    }

    public static string GetFormat(EnumProtocolType ircvers, Objects.User user)
    {
        switch ((int)ircvers)
        {
            case 5:
            case 6:
                return $"{user.Profile.Irc5_ToString()},";
            case 7:
                return $"{user.Profile.Irc7_ToString()},";
            case 8:
                return $"{user.Profile},";
            default:
                return "";
        }
    }
}