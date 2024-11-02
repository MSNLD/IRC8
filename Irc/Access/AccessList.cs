using Irc.Enumerations;

namespace Irc.Access;

public class AccessList
{
    public Dictionary<EnumAccessLevel, List<AccessEntry>> Entries = new() 
    {
        { EnumAccessLevel.OWNER, new List<AccessEntry>() },
        { EnumAccessLevel.HOST, new List<AccessEntry>() },
        { EnumAccessLevel.VOICE, new List<AccessEntry>() },
        { EnumAccessLevel.DENY, new List<AccessEntry>() },
        { EnumAccessLevel.GRANT, new List<AccessEntry>() }
    };

    public EnumAccessError Add(AccessEntry accessEntry)
    {
        var accessList = Get(accessEntry.AccessLevel);
        if (accessList == null) return EnumAccessError.IRCERR_BADLEVEL;

        var entry = accessList.FirstOrDefault(entry => entry.Mask == accessEntry.Mask);
        if (entry != null) return EnumAccessError.IRCERR_DUPACCESS;

        accessList.Add(accessEntry);
        return EnumAccessError.SUCCESS;
    }

    public EnumAccessError Delete(AccessEntry accessEntry)
    {
        var accessList = Get(accessEntry.AccessLevel);
        if (accessList == null) return EnumAccessError.IRCERR_BADLEVEL;

        var entry = accessList.FirstOrDefault(entry => entry.Mask == accessEntry.Mask);
        if (entry == null) return EnumAccessError.IRCERR_MISACCESS;

        accessList.Remove(entry);
        return EnumAccessError.SUCCESS;
    }

    public List<AccessEntry>? Get(EnumAccessLevel accessLevel)
    {
        Entries.TryGetValue(accessLevel, out var list);
        return list;
    }

    public AccessEntry Get(EnumAccessLevel accessLevel, string mask)
    {
        var accessList = Get(accessLevel);
        if (accessList == null) return null;

        return accessList.FirstOrDefault(entry => entry.Mask == mask);
    }

    public EnumAccessError Clear(EnumUserAccessLevel userAccessLevel, EnumAccessLevel accessLevel)
    {
        var hasRemaining = false;
        Entries
            .Where(kvp => accessLevel == EnumAccessLevel.All || kvp.Key == accessLevel)
            .ToList()
            .ForEach(
                kvp =>
                {
                    Entries[kvp.Key] = kvp.Value.Where(accessEntry => accessEntry.EntryLevel > userAccessLevel)
                        .ToList();
                    if (Entries[kvp.Key].Count > 0) hasRemaining = true;
                }
            );

        if (hasRemaining) return EnumAccessError.IRCERR_INCOMPLETE;
        return EnumAccessError.SUCCESS;
    }
}