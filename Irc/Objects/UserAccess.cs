using Irc.Access;
using Irc.Enumerations;

namespace Irc.Objects;

public class UserAccess : AccessList
{
    public UserAccess()
    {
        AccessEntries = new Dictionary<EnumAccessLevel, List<AccessEntry>>
        {
            { EnumAccessLevel.VOICE, new List<AccessEntry>() },
            { EnumAccessLevel.DENY, new List<AccessEntry>() }
        };
    }
}