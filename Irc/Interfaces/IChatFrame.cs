using Irc.Objects;

namespace Irc.Interfaces;

public interface IChatFrame
{
    long SequenceId { get; set; }
    Message Message { get; }
    Server Server { get; }
    User? User { get; }
}