using Irc.IO;
using Irc.Objects.Collections;
using Irc.Objects.Server;
using Irc.Props.User;

namespace Irc.Objects.User;

public class UserPropCollection : PropCollection
{
    public UserPropCollection(IDataStore dataStore, IServer server)
    {
        AddProp(new OID(dataStore));
        AddProp(new Nick(dataStore));
        AddProp(new SubscriberInfo(server));
        AddProp(new Msnprofile());
        AddProp(new Role(server));
    }
}