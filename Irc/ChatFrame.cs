﻿using Irc.Interfaces;

namespace Irc;

public class ChatFrame : IChatFrame
{
    public long SequenceId { get; set; }
    public Message Message { set; get; }
    public IServer Server { set; get; }
    public IUser? User { set; get; }
}