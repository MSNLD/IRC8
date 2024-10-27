using Irc.Extensions.Props;
using Irc.Interfaces;
using Irc.IO;
using Irc.Resources;

namespace Irc.Props.User;

internal class OID : PropRule
{
    private readonly IDataStore dataStore;

    public OID(IDataStore dataStore) : base(IrcStrings.UserPropOid, EnumChannelAccessLevel.ChatMember,
        EnumChannelAccessLevel.None, IrcStrings.GenericProps, "0", true)
    {
        this.dataStore = dataStore;
    }

    public override string GetValue(IChatObject target)
    {
        return dataStore.Get(IrcStrings.UserPropOid);
    }
}