﻿using Irc.Enumerations;
using Irc.Interfaces;
using Irc.Resources;

namespace Irc.Commands;

public class Pass : Command, ICommand
{
    public Pass() : base(1, false)
    {
    }

    public new EnumCommandDataType GetDataType()
    {
        return EnumCommandDataType.None;
    }

    public new void Execute(IChatFrame chatFrame)
    {
        if (!chatFrame.User.IsRegistered())
            // TODO: Encrypt below pass
            chatFrame.User.Pass = chatFrame.Message.Parameters.First();
        else
            chatFrame.User.Send(IrcRaws.IRC_RAW_462(chatFrame.Server, chatFrame.User));
    }
}