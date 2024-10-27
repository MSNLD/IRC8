using Irc.Enumerations;
using Irc.Interfaces;
using Irc.IO;
using Irc.Resources;

namespace Irc.Objects;

public class ChatObject : IChatObject
{
    protected readonly IModeCollection _modes;
    private readonly IPropCollection _props;
    public readonly IDataStore DataStore;

    public ChatObject(IModeCollection modes, IPropCollection props, IDataStore dataStore)
    {
        _modes = modes;
        _props = props;
        DataStore = dataStore;
        DataStore.SetId(Id.ToString());
    }

    public virtual EnumUserAccessLevel Level => EnumUserAccessLevel.None;

    public virtual IModeCollection Modes => _modes;

    public IPropCollection Props => _props;
    public IAccessList AccessList { get; }

    public IModeCollection GetModes()
    {
        return _modes;
    }

    public Guid Id { get; } = Guid.NewGuid();

    public string ShortId => Id.ToString().Split('-').Last();

    public string Name
    {
        get => DataStore.Get("Name") ?? IrcStrings.Wildcard;
        set => DataStore.Set("Name", value);
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

    public virtual bool CanBeModifiedBy(ChatObject source)
    {
        throw new NotImplementedException();
    }
}