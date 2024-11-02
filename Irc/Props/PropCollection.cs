namespace Irc.Props;

public class PropCollection
{
    public Dictionary<string, PropRule> Prop = new(StringComparer.InvariantCultureIgnoreCase);
}