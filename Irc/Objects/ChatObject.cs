using Irc.Access;
using Irc.Enumerations;
using Irc.Interfaces;
using Irc.Resources;

namespace Irc.Objects;

public class ChatObject : IChatObject
{
    public Dictionary<char, int> Modes { get; set; } = new()
    {
    };

    public Dictionary<string, string?> Props { get; set; } = new()
    {
        { "NAME", null }
    };

    public IAccessList AccessList { get; set; } = new AccessList();


    public virtual EnumUserAccessLevel Level => EnumUserAccessLevel.None;

    public Guid Id { get; } = Guid.NewGuid();

    public string ShortId => Id.ToString().Split('-').Last();

    public string Name
    {
        get => Props["NAME"] ?? IrcStrings.Wildcard;
        set => Props["NAME"] = value;
    }

    public virtual void Send(string message) => throw new NotImplementedException();
    public virtual void Send(string message, ChatObject except = null) => throw new NotImplementedException();
    public virtual void Send(string message, EnumChannelAccessLevel accessLevel) => throw new NotImplementedException();
    public bool CanBeModifiedBy(IChatObject source) => throw new NotImplementedException();
    
    public override string ToString()
    {
        return Name;
    }
}