using Irc.Interfaces;

namespace Irc.Props;

public class PropCollection
{
    public Dictionary<string, IPropRule> Prop = new(StringComparer.InvariantCultureIgnoreCase);
}