using Irc.Enumerations;
using Irc.Objects;
using Irc.Resources;

namespace Irc.Commands;

internal class Invite : Command
{
    public Invite() : base(1)
    {
    }

    public override void Execute(ChatFrame chatFrame)
    {
        // Invite <nick>
        // Invite <nick> <channel>

        var targetNickname = chatFrame.Message.Parameters.FirstOrDefault();
        var targetUser = chatFrame.Server.GetUserByNickname(targetNickname);

        if (targetUser == null)
        {
            chatFrame.User.Send(Raws.IRCX_ERR_NOSUCHNICK_401(chatFrame.Server, chatFrame.User, targetNickname));
            return;
        }

        if (chatFrame.Message.Parameters.Count() == 1) InviteNickToCurrentChannel(chatFrame, targetUser);

        if (chatFrame.Message.Parameters.Count() > 1) InviteNickToSpecificChannel(chatFrame, targetUser);
    }


    public static void InviteNickToCurrentChannel(ChatFrame chatFrame, User targetUser)
    {
        var targetChannelKvp = chatFrame.User.Channels.FirstOrDefault();
        var targetChannel = targetChannelKvp.Key;
        var member = targetChannelKvp.Value;

        if (targetChannel == null)
        {
            chatFrame.User.Send(Raws.IRCX_ERR_U_NOTINCHANNEL_928(chatFrame.Server, chatFrame.User));
            return;
        }

        ProcessInvite(chatFrame, member, targetChannel, targetUser);
    }

    public static void InviteNickToSpecificChannel(ChatFrame chatFrame, User targetUser)
    {
        var targetChannelName = chatFrame.Message.Parameters[1];
        var targetChannel = chatFrame.Server.GetChannelByName(targetChannelName);
        if (targetChannel == null)
        {
            chatFrame.User.Send(Raws.IRCX_ERR_NOSUCHCHANNEL_403(chatFrame.Server, chatFrame.User, targetChannelName));
            return;
        }

        var member = targetChannel.GetMember(chatFrame.User);

        if (member == null && chatFrame.User.Level < EnumUserAccessLevel.Guide)
            chatFrame.User.Send(Raws.IRCX_ERR_NOTONCHANNEL_442(chatFrame.Server, chatFrame.User, targetChannel));

        ProcessInvite(chatFrame, member, targetChannel, targetUser);
    }

    public static void ProcessInvite(ChatFrame chatFrame, Member member, Channel? targetChannel,
        User targetUser)
    {
        if (((Channel)targetChannel).InviteOnly && member.GetLevel() < EnumChannelAccessLevel.ChatHost)
        {
            chatFrame.User.Send(Raws.IRCX_ERR_CHANOPRIVSNEEDED_482(chatFrame.Server, chatFrame.User, targetChannel));
            return;
        }

        if (targetUser.IsOn(targetChannel))
        {
            chatFrame.User.Send(Raws.IRCX_ERR_USERONCHANNEL_443(chatFrame.Server, targetUser, targetChannel));
            return;
        }

        if (!targetChannel.InviteMember(targetUser))
        {
            chatFrame.User.Send(Raws.IRCX_ERR_TOOMANYINVITES_929(chatFrame.Server, chatFrame.User, targetUser,
                targetChannel));
            return;
        }

        targetUser.Send(Raws.RPL_INVITE(chatFrame.Server, chatFrame.User, targetUser, chatFrame.Server.RemoteIP,
            targetChannel));
    }
}