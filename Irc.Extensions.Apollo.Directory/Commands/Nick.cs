using Irc.Commands;
using Irc.Enumerations;
using Irc.Helpers;
using Irc.Interfaces;
using Irc.Resources;

namespace Irc.Extensions.Apollo.Directory.Commands;

public class Nick : Command, ICommand
{
    public Nick() : base(1, false)
    {
    }

    public new EnumCommandDataType GetDataType()
    {
        return EnumCommandDataType.Standard;
    }

    public new void Execute(IChatFrame chatFrame)
    {
        var hopcount = string.Empty;
        if (chatFrame.Message.Parameters.Count > 1) hopcount = chatFrame.Message.Parameters[1];

        // Is user not registered?
        // Set nickname according to regulations (should be available in user object and changes based on what they authenticated as)
        HandlePreauthNicknameChange(chatFrame);
    }

    public static bool ValidateNickname(string? nickname)
    {
        var mask = IrcStrings.PreAuthNicknameMask;

        return nickname.Length <= IrcStrings.MaxFieldLen &&
               RegularExpressions.Match(mask, nickname, true);
    }

    public static bool HandlePreauthNicknameChange(IChatFrame chatFrame)
    {
        var nickname = chatFrame.Message.Parameters.First();
        // UTF8 / Guest / Normal / Admin/Sysop/Guide OK
        if (!ValidateNickname(nickname))
        {
            chatFrame.User?.Send(Raw.IRCX_ERR_ERRONEOUSNICK_432(chatFrame.Server, chatFrame.User, nickname));
            return false;
        }

        if (chatFrame.User != null) chatFrame.User.Nickname = nickname;
        return true;
    }
}