using Irc.Commands;
using Irc.Helpers;
using Irc.Resources;

namespace Irc.Extensions.Apollo.Directory.Commands;

public class Nick : Command
{
    public Nick() : base(1, false)
    {
    }

    public override void Execute(ChatFrame chatFrame)
    {
        var hopcount = string.Empty;
        if (chatFrame.Message.Parameters.Count > 1) hopcount = chatFrame.Message.Parameters[1];

        // Is user not registered?
        // Set nickname according to regulations (should be available in user object and changes based on what they authenticated as)
        HandlePreauthNicknameChange(chatFrame);
    }

    public static bool ValidateNickname(string? nickname)
    {
        var mask = Tokens.PreAuthNicknameMask;

        return nickname.Length <= Tokens.MaxFieldLen &&
               RegularExpressions.Match(mask, nickname, true);
    }

    public static bool HandlePreauthNicknameChange(ChatFrame chatFrame)
    {
        var nickname = chatFrame.Message.Parameters.First();
        // UTF8 / Guest / Normal / Admin/Sysop/Guide OK
        if (!ValidateNickname(nickname))
        {
            chatFrame.User?.Send(Raws.IRCX_ERR_ERRONEOUSNICK_432(chatFrame.Server, chatFrame.User, nickname));
            return false;
        }

        if (chatFrame.User != null) chatFrame.User.Nickname = nickname;
        return true;
    }
}