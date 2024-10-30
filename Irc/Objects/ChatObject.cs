using Irc.Enumerations;
using Irc.Interfaces;
using Irc.Resources;

namespace Irc.Objects;

public class ChatObject : IChatObject
{
    protected readonly IModeCollection _modes;
    public readonly IDataStore DataStore;

    public ChatObject(IModeCollection modes, IDataStore dataStore)
    {
        _modes = modes;
        DataStore = dataStore;
        DataStore.SetId(Id.ToString());
    }

    public Dictionary<string, string?> Props { get; set; } = new()
    {
        { "NAME", null }
    };

    public virtual EnumUserAccessLevel Level => EnumUserAccessLevel.None;

    public virtual IModeCollection Modes => _modes;

    public IAccessList AccessList { get; }

    public IModeCollection GetModes()
    {
        return _modes;
    }

    public Guid Id { get; } = Guid.NewGuid();

    public string ShortId => Id.ToString().Split('-').Last();

    public string Name
    {
        get => Props["NAME"] ?? IrcStrings.Wildcard;
        set => Props["NAME"] = value;
    }

    public virtual void Send(string message)
    {
        throw new NotImplementedException();
    }

    public virtual void Send(string message, ChatObject except = null)
    {
        throw new NotImplementedException();
    }

    public virtual void Send(string message, EnumChannelAccessLevel accessLevel)
    {
        throw new NotImplementedException();
    }

    public override string ToString()
    {
        return Name;
    }

    public bool CanBeModifiedBy(IChatObject source)
    {
        throw new NotImplementedException();
    }
}