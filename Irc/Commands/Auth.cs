using Irc.Enumerations;
using Irc.Helpers;
using Irc.Security.Packages;

namespace Irc.Commands;

public class Auth : Command
{
    public Auth() : base(3, false)
    {
    }

    public override void Execute(ChatFrame chatFrame)
    {
        if (chatFrame.User.IsRegistered())
        {
            chatFrame.User.Send(Raw.IRCX_ERR_ALREADYREGISTERED_462(chatFrame.Server, chatFrame.User));
        }
        else if (chatFrame.User.IsAuthenticated())
        {
            chatFrame.User.Send(Raw.IRCX_ERR_ALREADYAUTHENTICATED_909(chatFrame.Server, chatFrame.User));
        }
        else
        {
            var parameters = chatFrame.Message.Parameters;

            var supportPackage = chatFrame.User.SupportPackage;
            var packageName = parameters[0];
            var sequence = parameters[1].ToUpper();
            var token = parameters[2].ToLiteral();

            if (sequence == "I")
            {
                var targetPackage = chatFrame.Server.GetSecurityManager()
                    .CreatePackageInstance(packageName, chatFrame.Server.GetCredentialManager());

                if (targetPackage == null)
                {
                    chatFrame.User.Send(Raw.IRCX_ERR_UNKNOWNPACKAGE_912(chatFrame.Server, chatFrame.User, packageName));
                    return;
                }

                if (supportPackage == null || supportPackage is ANON)
                {
                    supportPackage = chatFrame.Server.GetSecurityManager()
                        .CreatePackageInstance(packageName, chatFrame.Server.GetCredentialManager());

                    chatFrame.User.SupportPackage = supportPackage;
                }

                var supportPackageSequence =
                    supportPackage.InitializeSecurityContext(token, chatFrame.Server.RemoteIP);

                if (supportPackageSequence == EnumSupportPackageSequence.SSP_OK)
                {
                    var securityToken = supportPackage.CreateSecurityChallenge().ToEscape();
                    chatFrame.User.Send(Raw.RPL_AUTH_SEC_REPLY(packageName, securityToken));
                    // Send reply
                    return;
                }
            }
            else if (sequence == "S")
            {
                var supportPackageSequence =
                    chatFrame.User.SupportPackage.AcceptSecurityContext(token, chatFrame.Server.RemoteIP);
                if (supportPackageSequence == EnumSupportPackageSequence.SSP_OK)
                {
                    chatFrame.User.Authenticate();

                    var credentials = chatFrame.User.SupportPackage.GetCredentials();
                    if (credentials == null)
                    {
                        // Invalid credentials handle
                    }
                    else
                    {
                        var user = chatFrame.User.SupportPackage.GetCredentials().GetUsername();
                        var domain = chatFrame.User.SupportPackage.GetCredentials().GetDomain();
                        var userAddress = chatFrame.User.Address;
                        userAddress.User = credentials.GetUsername() ?? userAddress.MaskedIP;
                        userAddress.Host = credentials.GetDomain();
                        userAddress.Server = chatFrame.Server.Name;
                        var nickname = credentials.GetNickname();
                        if (nickname != null) chatFrame.User.Name = credentials.GetNickname();
                        if (credentials.Guest && chatFrame.User.Address.RealName == null)
                            userAddress.RealName = string.Empty;

                        chatFrame.User.Guest = credentials.Guest;
                        chatFrame.User.Level = credentials.GetLevel();

                        // TODO: find another way to work in Utf8 nicknames
                        if (chatFrame.User.Level >= EnumUserAccessLevel.Guide) chatFrame.User.Utf8 = true;

                        // Send reply
                        chatFrame.User.Send(Raw.RPL_AUTH_SUCCESS(packageName, $"{user}@{domain}", 0));
                    }

                    return;
                }

                if (supportPackageSequence == EnumSupportPackageSequence.SSP_CREDENTIALS)
                {
                    chatFrame.User.Send(Raw.RPL_AUTH_SEC_REPLY(packageName, "OK"));
                    return;
                }
            }

            // auth failed
            chatFrame.User.Disconnect(
                Raw.IRCX_ERR_AUTHENTICATIONFAILED_910(chatFrame.Server, chatFrame.User, packageName));
        }
    }
}