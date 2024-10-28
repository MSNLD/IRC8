using Irc.Extensions.Interfaces;

namespace Irc.Interfaces;

public interface IPropCollection
{
    Dictionary<string, string> Properties { get; set; }
    IPropRule GetProp(string name);
    List<IPropRule> GetProps();
    void SetProp(string name, string value);
}