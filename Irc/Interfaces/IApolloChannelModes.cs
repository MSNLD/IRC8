using Irc.Interfaces;

namespace Irc.Extensions.Apollo.Interfaces;

public interface IApolloChannelModes : IChannelModes
{
    bool OnStage { get; set; }
    bool Subscriber { get; set; }
}