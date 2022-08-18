﻿using Irc.Commands;
using Irc.Enumerations;
using Irc.Objects;

namespace Irc;

public class Protocol : IProtocol
{
    protected Dictionary<string, ICommand> Commands = new(StringComparer.InvariantCultureIgnoreCase);

    public ICommand GetCommand(string name)
    {
        throw new NotImplementedException();
    }

    public EnumProtocolType GetProtocolType()
    {
        throw new NotImplementedException();
    }

    public void AddCommand(ICommand command, string name = null)
    {
        Commands.Add(name ?? command.GetName(), command);
    }
    public void FlushCommands()
    {
        Commands.Clear();
    }

    public virtual string FormattedUser(IUser user)
    {
        return user.GetAddress().Nickname;
    }
}