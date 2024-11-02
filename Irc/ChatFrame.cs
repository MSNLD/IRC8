using Irc.Interfaces;
using Irc.Objects;

namespace Irc;

public class ChatFrame : IChatFrame
{
    public long SequenceId { get; set; }
    public Message Message { set; get; }
    public Server Server { set; get; }
    public User? User { set; get; }
}