using Irc.Enumerations;
using Irc.Helpers;
using Irc.Resources;

namespace Irc.Commands;

public class Nick : Command
{
    public Nick() : base(1, false)
    {
    }

    public override void Execute(ChatFrame chatFrame)
    {
        // Is user not registered?
        // Set nickname according to regulations (should be available in user object and changes based on what they authenticated as)
        if (!chatFrame.User.IsAuthenticated()) HandlePreauthNicknameChange(chatFrame);
        else if (!chatFrame.User.IsRegistered()) HandlePreregNicknameChange(chatFrame);
        else HandleRegNicknameChange(chatFrame);
    }

    public static bool ValidateNickname(string? nickname, bool guest = false, bool oper = false, bool preAuth = false,
        bool preReg = false)
    {
        var mask = Tokens.PostAuthNicknameMask;

        if (preAuth) mask = Tokens.PreAuthNicknameMask;
        else if (oper) mask = Tokens.PostAuthOperNicknameMask;
        else if (guest) mask = Tokens.PostAuthGuestNicknameMask;

        return nickname != null &&
               nickname.Length <= Tokens.MaxFieldLen &&
               RegularExpressions.Match(mask, nickname, true);
    }

    public static bool HandlePreauthNicknameChange(ChatFrame chatFrame)
    {
        var nickname = chatFrame.Message.Parameters.First();
        // UTF8 / Guest / Normal / Admin/Sysop/Guide OK
        if (!ValidateNickname(nickname, preAuth: true))
        {
            chatFrame.User?.Send(Raws.IRCX_ERR_ERRONEOUSNICK_432(chatFrame.Server, chatFrame.User, nickname));
            return false;
        }

        if (chatFrame.User != null) chatFrame.User.Nickname = nickname;
        return true;
    }

    public static bool HandlePreregNicknameChange(ChatFrame chatFrame)
    {
        var nickname = chatFrame.Message.Parameters.First();
        var guest = chatFrame.User != null && chatFrame.User.IsGuest();
        var oper = chatFrame.User != null && chatFrame.User.Level >= EnumUserAccessLevel.Guide;

        if (!ValidateNickname(nickname, guest, oper, false, true))
        {
            chatFrame.User?.Send(Raws.IRCX_ERR_ERRONEOUSNICK_432(chatFrame.Server, chatFrame.User, nickname));
            return false;
        }

        if (chatFrame.User != null) chatFrame.User.Nickname = nickname;
        return true;
    }

    public static bool HandleRegNicknameChange(ChatFrame chatFrame)
    {
        var nickname = chatFrame.Message.Parameters.First();
        var guest = chatFrame.User != null && chatFrame.User.IsGuest();
        var oper = chatFrame.User != null && chatFrame.User.Level >= EnumUserAccessLevel.Guide;

        if (!guest && !oper)
        {
            chatFrame.User?.Send(Raws.IRCX_ERR_NONICKCHANGES_439(chatFrame.Server, chatFrame.User, nickname));
            return false;
        }

        var channels = chatFrame.User?.Channels;
        foreach (var channel in channels)
        foreach (var member in channel.Key?.GetMembers())
            if (member.GetUser().Nickname == nickname)
            {
                chatFrame.User?.Send(Raws.IRCX_ERR_NICKINUSE_433(chatFrame.Server, chatFrame.User));
                return false;
            }

        if (!ValidateNickname(nickname, guest, oper))
        {
            chatFrame.User.Send(Raws.IRCX_ERR_ERRONEOUSNICK_432(chatFrame.Server, chatFrame.User, nickname));
            return false;
        }

        chatFrame.User.ChangeNickname(nickname, false);
        return true;
    }
}