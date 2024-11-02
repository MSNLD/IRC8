using Irc.Objects;

namespace Irc;

public class ChatFrame(long sequenceId, Server server, User user, Message message)
{
    public long SequenceId { get; set; } = sequenceId;
    public Message Message { set; get; } = message;
    public Server Server { set; get; } = server;
    public User User { set; get; } = user;
}