﻿using Irc.Extensions.Apollo.Objects.User;
using Irc.Interfaces;
using Irc.Objects;
using Irc.Objects.Server;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Irc.Extensions.Apollo
{
    public static class ApolloRaws
    {
        public static string RPL_JOIN_MSN(IProtocol protocol, ApolloUser user, IChannel channel)
        {
            return $":{user.GetAddress()} JOIN {protocol.GetFormat(user)} :{channel}";
        }
    }
}
