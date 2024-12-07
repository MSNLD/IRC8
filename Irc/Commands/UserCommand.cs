using Irc.Resources;

namespace Irc.Commands;

public class UserCommand : Command
{
    public UserCommand() : base(4, false)
    {
    }

    public new string? GetName()
    {
        return "USER";
    }

    public override void Execute(ChatFrame chatFrame)
    {
        var address = chatFrame.User.Address;
        if (!string.IsNullOrWhiteSpace(address.RealName))
        {
            chatFrame.User.Send(Raw.IRCX_ERR_ALREADYREGISTERED_462(chatFrame.Server, chatFrame.User));
        }
        else
        {
            var parameters = chatFrame.Message.Parameters;
            // TODO: Check length
            if (string.IsNullOrWhiteSpace(address.RealName))
            {
                if (string.IsNullOrWhiteSpace(address.User)) address.User = parameters[0];
                if (string.IsNullOrWhiteSpace(address.Host)) address.Host = parameters[1];
                address.Server = chatFrame.Server.Name;
                address.RealName = parameters[3];
            }
        }
    }
}