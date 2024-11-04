using Irc.Enumerations;
using Irc.Resources;

namespace Irc.Commands;

internal class Ircvers : Command
{
    public Ircvers() : base(2, false)
    {
    }

    public override void Execute(ChatFrame chatFrame)
    {
        //return;
        if (chatFrame.User.IsRegistered())
        {
            chatFrame.User.Send(Raw.IRCX_ERR_ALREADYREGISTERED_462(chatFrame.Server, chatFrame.User));
        }
        else
        {
            var ircvers = chatFrame.Message.Parameters[0].ToUpper();

            if (ircvers.Length == 4 && ircvers.StartsWith("IRC") && char.IsNumber(ircvers.Last()))
            {
                if (Enum.TryParse<EnumProtocolType>(ircvers, true, out var enumProtocolType))
                {
                    chatFrame.User.Protocol.Ircvers = enumProtocolType;
                    chatFrame.User.Client = chatFrame.Message.Parameters[1];

                    var isircx = enumProtocolType > EnumProtocolType.IRC;
                    chatFrame.User.Send(Raw.IRCX_RPL_IRCX_800(chatFrame.Server, chatFrame.User, isircx ? 1 : 0, 0,
                        chatFrame.Server.MaxInputBytes, IrcStrings.IRCXOptions));

                    // TODO: Fix below
                    // chatFrame.User.Send(Raw.IRCX_ERR_BADVALUE_906(chatFrame.Server, chatFrame.User, ircvers));
                }

                return;
            }

            chatFrame.User.Send(Raw.IRCX_ERR_BADVALUE_906(chatFrame.Server, chatFrame.User, ircvers));
        }
    }
}