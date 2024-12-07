using System.Globalization;
using Irc.Enumerations;
using Irc.Objects;

namespace Irc.Resources;

public static class Raws
{
        public static string IRCX_CLOSINGLINK(Server server, User? user, string code, string message)
    {
        return $"ERROR :Closing Link: {user}[{user.Address.RemoteIP}] {code} ({message})";
    }

    public static string IRCX_CLOSINGLINK_007_SYSTEMKILL(Server server, User? user, string ip)
    {
        return $"ERROR :Closing Link: {user}[{ip}] 007 (Killed by system operator)";
    }

    public static string IRCX_CLOSINGLINK_011_PINGTIMEOUT(Server server, User user, string ip)
    {
        return $"ERROR :Closing Link: {user}[{ip}] 011 (Ping timeout)";
    }

    public static string RPL_AUTH_SEC_REPLY(string? package, string token)
    {
        return $"AUTH {package} S :{token}";
    }

    public static string RPL_AUTH_SUCCESS(string? package, string address, int oid)
    {
        return $"AUTH {package} * {address} {oid}";
    }

    public static string RPL_MODE_IRC(User user, ChatObject? target, string modeString)
    {
        return $":{user.Address} MODE {target} {modeString}";
    }

    public static string RPL_TOPIC_IRC(Server server, User? user, Channel channel, string? topic)
    {
        return $":{user.Address} TOPIC {channel} :{topic}";
    }

    public static string RPL_PROP_IRCX(Server server, User? user, ChatObject chatObject, string? propName,
        string? propValue)
    {
        return $":{user.Address} PROP {chatObject} {propName} :{propValue}";
    }

    public static string RPL_KICK_IRC(User? user, Channel channel, User? target, string? reason)
    {
        return $":{user.Address} KICK {channel} {target} :{reason}";
    }

    public static string RPL_KILL_IRC(User? user, User? targetUser, string? message)
    {
        return $":{user.Address} KILL {targetUser} :{message}";
    }

    public static string RPL_CHAN_WHISPER(Server server, User? user, Channel channel, ChatObject target,
        string? message)
    {
        return $":{user.Address} WHISPER {channel} {target} :{message}";
    }

    public static string RPL_NOTICE_USER(Server server, User? user, ChatObject target, string? message)
    {
        return $":{user.Address} NOTICE {target} :{message}";
    }

    public static string RPL_PRIVMSG_USER(Server server, User? user, ChatObject target, string? message)
    {
        return $":{user.Address} PRIVMSG {target} :{message}";
    }

    public static string RPL_INVITE(Server server, User? user, User targetUser, string? host, Channel? channel)
    {
        return $":{user.Address} INVITE {targetUser} {host} {channel}";
    }

    public static string RPL_NICK(Server server, User user, string? newNick)
    {
        return $":{user.Address} NICK {newNick}";
    }

    public static string RPL_PING(Server server, User user)
    {
        return $"PING :{server}";
    }

    public static string IRCX_RPL_WELCOME_001(Server server, User? user)
    {
        return $":{server} 001 {user} :Welcome to the {server.Title} server, {user}";
    }

    public static string IRCX_RPL_WELCOME_002(Server server, User? user, Version version)
    {
        return
            $":{server} 002 {user} :Your host is {server}, running version {version.Major}.{version.Minor}.{version.Revision}";
    }

    public static string IRCX_RPL_WELCOME_003(Server server, User? user)
    {
        return $":{server} 003 {user} :This server was created {server.CreationDate}";
    }

    public static string IRCX_RPL_WELCOME_004(Server server, User? user, Version version)
    {
        return
            $":{server} 004 {user} {server} {version.Major}.{version.Minor}.{version.Revision} {server.GetSupportedUserModes()} {server.GetSupportedChannelModes()}";
    }

    public static string IRCX_RPL_UMODEIS_221(Server server, User? user, string modes)
    {
        return $":{server} 221 {user} {modes}";
    }

    public static string IRCX_RPL_LUSERCLIENT_251(Server server, User? user, int users, int invisible, int servers)
    {
        return $":{server} 251 {user} :There are {users} users and {invisible} invisible on {servers} servers";
    }

    public static string IRCX_RPL_LUSEROP_252(Server server, User? user, int operators)
    {
        return $":{server} 252 {user} {operators} :operator(s) online";
    }

    public static string IRCX_RPL_LUSERUNKNOWN_253(Server server, User? user, int unknown)
    {
        return $":{server} 253 {user} {unknown} :unknown connection(s)";
    }

    public static string IRCX_RPL_LUSERCHANNELS_254(Server server, User? user)
    {
        return $":{server} 254 {user} {server.GetChannels().Count} :channels formed";
    }

    public static string IRCX_RPL_LUSERME_255(Server server, User? user, int clients, int servers)
    {
        return $":{server} 255 {user} :I have {clients} clients and {servers} servers";
    }

    public static string IRCX_RPL_LUSERS_265(Server server, User? user, int localUsers, int localMax)
    {
        return $":{server} 265 {user} :Current local users: {localUsers} Max: {localMax}";
    }

    public static string IRCX_RPL_GUSERS_266(Server server, User? user, int globalUsers, int globalMax)
    {
        return $":{server} 266 {user} :Current global users: {globalUsers} Max: {globalMax}";
    }

    public static string IRCX_RPL_USERHOST_302(Server server, User? user)
    {
        return $":{server} 302 {user} :{user}={user}!~{user.Address.GetUserHost()}";
    }

    public static string IRCX_RPL_UNAWAY_305(Server server, User? user)
    {
        return $":{server} 305 {user} :You are no longer marked as being away";
    }

    public static string IRCX_RPL_NOWAWAY_306(Server server, User? user)
    {
        return $":{server} 306 {user} :You have been marked as being away";
    }

    public static string IRCX_RPL_ENDOFWHO_315(Server server, User? user, string? criteria)
    {
        return $":{server} 315 {user} {criteria} :End of /WHO list";
    }

    public static string IRCX_RPL_WHOISIP_320(Server server, User? user, User targetUser)
    {
        return $":{server} 320 {user} {targetUser} :from IP {targetUser.GetConnection().GetIp()}";
    }

    public static string IRCX_RPL_MODE_321(Server server, User? user)
    {
        return $":{server} 321 {user} Channel :Users  Name";
    }

    public static string IRCX_RPL_MODE_322(Server server, User? user, Channel? channel)
    {
        return
            $":{server} 322 {user} {channel} {channel.GetMembers().Count} :{channel.Props[Tokens.ChannelPropTopic] ?? string.Empty}";
    }

    public static string IRCX_RPL_MODE_323(Server server, User? user)
    {
        return $":{server} 323 {user} :End of /LIST";
    }

    public static string IRCX_RPL_MODE_324(Server server, User? user, Channel channel, string modes)
    {
        return $":{server} 324 {user} {channel} +{modes}";
    }

    public static string IRCX_RPL_TOPIC_332(Server server, User? user, Channel channel, string topic)
    {
        return $":{server} 332 {user} {channel} :{topic}";
    }

    public static string IRCX_RPL_VERSION_351(Server server, User? user, Version version)
    {
        return
            $":{server} 351 {user} {version.Major}.{version.Minor}.{version.Revision}.{version.Build} {server} :{server} {version.Major}.{version.Minor}";
    }

    public static string IRCX_RPL_WHOREPLY_352(Server server, User? user, string? channelName, string? userName,
        string? hostName, string? serverName, string? nickName, string userStatus, int hopCount, string? realName)
    {
        return
            $":{server} 352 {user} {channelName} {userName} {hostName} {serverName} {nickName} {userStatus} :{hopCount} {realName}";
    }

    public static string IRCX_RPL_NAMEREPLY_353(Server server, User? user, Channel channel, char channelType,
        string names)
    {
        return $":{server} 353 {user} {channelType} {channel} :{names}";
    }

    public static string IRCX_RPL_ENDOFNAMES_366(Server server, User? user, Channel channel)
    {
        return $":{server} 366 {user} {channel} :End of /NAMES list.";
    }

    public static string IRCX_RPL_RPL_INFO_371_UPTIME(Server server, User? user, DateTime creationDate)
    {
        // TODO: Format creation date
        return $":{server} 371 {user} :On-line since {creationDate}";
    }

    public static string IRCX_RPL_RPL_INFO_371_VERS(Server server, User? user, Version version)
    {
        // TODO: Get Full Name
        return $":{server} 371 {user} :{server.Name} {server.ServerVersion.Major}.{server.ServerVersion.Minor}";
    }

    public static string IRCX_RPL_RPL_INFO_371_RUNAS(Server server, User? user)
    {
        // TODO: Get Full Name
        return $":{server} 371 {user} :This server is running as an IRC.{server.GetType().Name}";
    }

    public static string IRCX_RPL_RPL_MOTD_372(Server server, User? user, string message)
    {
        return $":{server} 372 {user} :- {message}";
    }

    public static string IRCX_RPL_RPL_ENDOFINFO_374(Server server, User? user)
    {
        return $":{server} 374 {user} :End of /INFO list";
    }

    public static string IRCX_RPL_RPL_MOTDSTART_375(Server server, User? user)
    {
        return $":{server} 375 {user} :- {server} Message of the Day -";
    }

    public static string IRCX_RPL_RPL_ENDOFMOTD_376(Server server, User? user)
    {
        return $":{server} 376 {user} :End of /MOTD command";
    }

    public static string IRCX_RPL_YOUREOPER_381(Server server, User user)
    {
        return $":{server} 381 {user} :You are now an IRC operator";
    }

    public static string IRCX_RPL_YOUREADMIN_386(Server server, User user)
    {
        return $":{server} 386 {user} :You are now an IRC administrator";
    }

    public static string IRCX_RPL_TIME_391(Server server, User? user)
    {
        //<- :sky-8a15b323126 391 Sky sky-8a15b323126 :Wed Aug 10 18:27:41 2022
        return
            $":{server} 391 {user} {server} :{DateTime.Now.ToString("ddd MMM dd HH:mm:ss yyyy", CultureInfo.CreateSpecificCulture("en-us"))}";
    }

    public static string IRCX_ERR_NOSUCHNICK_401(Server server, User? user, string? target)
    {
        return $":{server} 401 {user} {target} :No such nick/channel";
    }

    public static string IRCX_ERR_NOSUCHCHANNEL_403(Server server, User? user, string? channel)
    {
        return $":{server} 403 {user} {channel} :No such channel";
    }

    public static string IRCX_ERR_CANNOTSENDTOCHAN_404(Server server, User? user, Channel? channel)
    {
        return $":{server} 404 {user} {channel} :Cannot send to channel";
    }

    public static string IRCX_ERR_TOOMANYCHANNELS_405(Server server, User? user, string? channel)
    {
        return $":{server} 405 {user} {channel} :You have joined too many channels";
    }

    public static string IRC_ERR_NORECIPIENT_411(Server server, User? user, string command)
    {
        return $":{server} 411 {user} :No recipient given ({command})";
    }

    public static string IRC_ERR_NOTEXT_412(Server server, User? user, string command)
    {
        return $":{server} 412 {user} :No text to send ({command})";
    }

    public static string IRCX_ERR_UNKNOWNCOMMAND_421(Server server, User? user, string? command)
    {
        return $":{server} 421 {user} {command} :Unknown command";
    }

    public static string IRCX_ERR_NOMOTD_422(Server server, User? user)
    {
        return $":{server} 422 {user} :MOTD File is missing";
    }

    public static string IRCX_ERR_ERRONEOUSNICK_432(Server server, User? user, string? nickname)
    {
        return $":{server} 432 {nickname} :Erroneous nickname";
    }

    public static string IRCX_ERR_NICKINUSE_433(Server server, User? user)
    {
        return $":{server} 433 * {user} :Nickname is already in use";
    }

    public static string IRCX_ERR_NONICKCHANGES_439(Server server, User? user, string? nickname)
    {
        return $":{server} 439 {user} {nickname} :Nick name changes not permitted.";
    }

    public static string IRCX_ERR_NOTONCHANNEL_442(Server server, User? user, Channel? channel)
    {
        return $":{server} 442 {user} {channel} :You're not on that channel";
    }

    public static string IRCX_ERR_USERONCHANNEL_443(Server server, User user, Channel? channel)
    {
        return $":{server} 443 {user} {channel} {user} :is already on channel";
    }

    public static string IRCX_ERR_NOTREGISTERED_451(Server server, User? user)
    {
        return $":{server} 451 {user} :You have not registered";
    }

    public static string IRCX_ERR_NEEDMOREPARAMS_461(Server server, User? user, string? command)
    {
        return $":{server} 461 {user} {command} :Not enough parameters";
    }

    public static string IRCX_ERR_ALREADYREGISTERED_462(Server server, User? user)
    {
        return $":{server} 462 {user} :You may not reregister";
    }

    public static string IRCX_ERR_KEYSET_467(Server server, User? user, Channel channel)
    {
        return $":{server} 467 {user} {channel} :Channel key already set";
    }

    public static string IRCX_ERR_CHANNELISFULL_471(Server server, User? user, Channel channel)
    {
        return $":{server} 471 {user} {channel} :Cannot join channel (+l)";
    }

    public static string IRCX_ERR_UNKNOWNMODE_472(Server server, User? user, char mode)
    {
        return $":{server} 472 {user} {mode} :is unknown mode char to me";
    }

    public static string IRCX_ERR_INVITEONLYCHAN_473(Server server, User? user, Channel channel)
    {
        return $":{server} 473 {user} {channel} :Cannot join channel (+i)";
    }

    public static string IRCX_ERR_BADCHANNELKEY_475(Server server, User? user, Channel channel)
    {
        return $":{server} 475 {user} {channel} :Cannot join channel (+k)";
    }

    public static string IRCX_ERR_NOPRIVILEGES_481(Server server, User? user)
    {
        return $":{server} 481 {user} :Permission Denied - You're not an IRC operator";
    }

    public static string IRCX_ERR_CHANOPRIVSNEEDED_482(Server server, User? user, Channel? channel)
    {
        return $":{server} 482 {user} {channel} :You're not channel operator";
    }

    public static string IRCX_ERR_CHANQPRIVSNEEDED_485(Server server, User? user, ChatObject? channel)
    {
        return $":{server} 485 {user} {channel} :You're not channel owner";
    }

    public static string IRCX_ERR_USERSDONTMATCH_502(Server server, User? user)
    {
        return $":{server} 502 {user} :Cant change mode for other users";
    }

    public static string IRCX_ERR_COMMANDUNSUPPORTED_554(Server server, User? user, string command)
    {
        return $":{server} 555 {user} {command} :Command not supported.";
    }

    public static string IRCX_RPL_FINDS_613(Server server, User? user)
    {
        //return $":{server} 613 {user} :%s %s";
        return $":{server} 613 {user} :{server.RemoteIP} 6667";
    }

    public static string IRCX_RPL_YOUREGUIDE_629(Server server, User user)
    {
        return $":{server} 629 {user} :You are now an IRC guide";
    }

    public static string IRC2_RPL_WHOISSECURE_671(Server server, User? user, User targetUser)
    {
        return $":{server} 671 {user} {targetUser} :is using a secure connection";
    }

    public static string IRCX_RPL_IRCX_800(Server server, User? user, int isircx, int ircxversion, int buffsize,
        string options)
    {
        return $":{server} 800 {user} {isircx} {ircxversion} {server.SecurityPackages} {buffsize} {options}";
    }

    public static string IRCX_RPL_ACCESSADD_801(Server server, User? user, ChatObject targetObject,
        string accessLevel, string? mask, int duration, string? address, string? reason)
    {
        return $":{server} 801 {user} {targetObject} {accessLevel} {mask} {duration} {address} :{reason}";
    }

    public static string IRCX_RPL_ACCESSDELETE_802(Server server, User? user, ChatObject targetObject,
        string accessLevel, string? mask, int duration, string? address, string? reason)
    {
        return $":{server} 802 {user} {targetObject} {accessLevel} {mask} {duration} {address} :{reason}";
    }

    public static string IRCX_RPL_ACCESSSTART_803(Server server, User? user, ChatObject targetObject)
    {
        return $":{server} 803 {user} {targetObject} :Start of access entries";
    }

    public static string IRCX_RPL_ACCESSLIST_804(Server server, User? user, ChatObject targetObject,
        string accessLevel, string? mask, int duration, string? address, string? reason)
    {
        return $":{server} 804 {user} {targetObject} {accessLevel} {mask} {duration} {address} :{reason}";
    }

    public static string IRCX_RPL_ACCESSEND_805(Server server, User? user, ChatObject targetObject)
    {
        return $":{server} 805 {user} {targetObject} :End of access entries";
    }

    public static string IRCX_RPL_LISTXSTART_811(Server server, User? user)
    {
        return $":{server} 811 {user} :Start of ListX";
    }

    public static string IRCX_RPL_LISTXLIST_812(Server server, User? user, Channel? channel, string modes,
        int memberCount, int memberLimit, string topic)
    {
        return $":{server} 812 {user} {channel} {modes} {memberCount} {memberLimit} :{topic}";
    }

    public static string IRCX_RPL_LISTXEND_817(Server server, User? user)
    {
        return $":{server} 817 {user} :End of ListX";
    }


    public static string IRCX_RPL_ACCESSCLEAR_820(Server server, User? user, ChatObject targetObject,
        EnumAccessLevel accessLevel)
    {
        var level = accessLevel == EnumAccessLevel.All ? "*" : accessLevel.ToString();
        return $":{server} 820 {user} {targetObject} {level} :Clear";
    }

    public static string IRCX_RPL_USERUNAWAY_821(Server server, User? user)
    {
        return $":{user.Address} 821 :User unaway";
    }

    public static string IRCX_RPL_USERNOWAWAY_822(Server server, User? user, string? reason)
    {
        return $":{user.Address} 822 :{reason}";
    }

    public static string IRCX_ERR_BADCOMMAND_900(Server server, User? user, string? command)
    {
        return $":{server} 900 {user} {command} :Bad command";
    }

    public static string IRCX_ERR_TOOMANYARGUMENTS_901(Server server, User? user, string? commandName)
    {
        return $":{server} 901 {user} {commandName} :Too many arguments";
    }

    public static string IRCX_ERR_BADLEVEL_903(Server server, User? user, string? level)
    {
        return $":{server} 903 {user} %s :Bad level";
    }

    public static string IRCX_ERR_BADPROPERTY_905(Server server, User? user, string? property)
    {
        return $":{server} 905 {user} {property} :Bad property specified";
    }

    public static string IRCX_ERR_BADVALUE_906(Server server, User? user, string? value)
    {
        return $":{server} 906 {user} {value} :Bad value specified";
    }

    public static string IRCX_ERR_SECURITY_908(Server server, User? user)
    {
        return $":{server} 908 {user} :No permissions to perform command";
    }

    public static string IRCX_ERR_ALREADYAUTHENTICATED_909(Server server, User? user)
    {
        return $":{server} 909 {user} %s :Already authenticated";
    }

    public static string IRCX_ERR_AUTHENTICATIONFAILED_910(Server server, User? user, string? package)
    {
        return $":{server} 910 {user} {package} :Authentication failed";
    }

    public static string IRCX_ERR_UNKNOWNPACKAGE_912(Server server, User? user, string? package)
    {
        return $":{server} 912 {user} {package} :Unsupported authentication package";
    }

    public static string IRCX_ERR_NOACCESS_913(Server server, User? user, ChatObject chatObject)
    {
        return $":{server} 913 {user} {chatObject} :No access";
    }

    public static string IRCX_ERR_DUPACCESS_914(Server server, User? user)
    {
        return $":{server} 914 {user} :Duplicate access entry";
    }

    public static string IRCX_ERR_NOWHISPER_923(Server server, User? user, Channel channel)
    {
        return $":{server} 923 {user} {channel} :Does not permit whispers";
    }

    public static string IRCX_ERR_NOSUCHOBJECT_924(Server server, User? user, string? objectName)
    {
        return $":{server} 924 {user} {objectName} :No such object found";
    }

    public static string IRCX_ERR_ALREADYONCHANNEL_927(Server server, User? user, Channel channel)
    {
        return $":{server} 927 {user} {channel} :Already in the channel.";
    }

    public static string IRCX_ERR_U_NOTINCHANNEL_928(Server server, User? user)
    {
        return $"{server} 928 {user} :You're not in a channel";
    }

    public static string IRCX_ERR_TOOMANYINVITES_929(Server server, User? user, User targetUser,
        Channel? targetChannel)
    {
        return $":{server} 929 {user} {targetUser} {targetChannel} :Cannot invite. Too many invites.";
    }

    public static string IRCX_ERR_NOTIMPLEMENTED(Server server, User? user, string command)
    {
        return $":{server} 999 {user} :%s Sorry, this command is not implemented yet.";
    }
    public static string IRC_RAW_256(Server server, User? user)
    {
        return $":{server} 256 {user} :Administrative info about {server}";
    }

    public static string IRC_RAW_257(Server server, User? user, string message)
    {
        return $":{server} 257 {user} :{message}";
    }

    public static string IRC_RAW_258(Server server, User? user, string message)
    {
        return $":{server} 258 {user} :{message}";
    }

    public static string IRC_RAW_259(Server server, User? user, string email)
    {
        return $":{server} 259 {user} :{email}";
    }

    public static string IRC_RAW_303(Server server, User? user, string names)
    {
        return $":{server} 303 {user} :{names}";
    }

    public static string IRC_RAW_311(Server server, User? user, User targetUser)
    {
        return
            $":{server} 311 {user} {targetUser} {targetUser.Address.User} {targetUser.Address.Host} * :{targetUser.Address.RealName}";
    }

    public static string IRC_RAW_312(Server server, User user, User targetUser)
    {
        return $":{server} 312 {user} {targetUser} {server} :{server.Info}";
    }

    public static string IRC_RAW_313(Server server, User? user, User targetUser)
    {
        return $":{server} 313 {user} {targetUser} :is an IRC operator";
    }

    public static string IRC_RAW_317(Server server, User? user, User targetUser, long seconds, long epoch)
    {
        return $":{server} 317 {user} {targetUser} {seconds} {epoch} :seconds idle, signon time";
    }

    public static string IRC_RAW_318(Server server, User? user, string? targetNicknames)
    {
        return $":{server} 318 {user} {targetNicknames} :End of /WHOIS list";
    }

    public static string IRC_RAW_319(Server server, User? user, User targetUser, string channelList)
    {
        return $":{server} 319 {user} {targetUser} :{channelList}";
    }

    public static string IRC_RAW_353(Server server, User user, Channel channel, string names)
    {
        return $":{server} 353 {user} = {channel} :{names}";
    }

    public static string IRC_RAW_364(Server server, User? user, string? mask, int hopcount)
    {
        return $":{server} 364 {user} {mask} {server} :{hopcount} P0 {server.Info}";
    }

    public static string IRC_RAW_365(Server server, User? user, string? mask)
    {
        return $":{server} 365 {user} {mask} :End of /LINKS list.";
    }

    public static string IRC_RAW_423(Server server, User? user)
    {
        return $":{server} 423 {user} {server} :No administrative info available ";
    }

    public static string IRC_RAW_446(Server server, User? user)
    {
        return $":{server} 446 {user} :USERS has been disabled";
    }

    public static string IRC_RAW_462(Server server, User? user)
    {
        return $":{server} 462 {user} :You may not reregister";
    }

    public static string IRC_RAW_472(Server server, User user, string modeChar)
    {
        return $":{server} 472 {user} {modeChar} :is unknown mode char to me";
    }

    public static string IRC_RAW_481(Server server, User user)
    {
        return $":{server} 481 {user} :Permission Denied - You're not an IRC operator";
    }

    public static string IRC_RAW_501(Server server, User? user)
    {
        return $":{server} 501 {user} :Unknown MODE flag";
    }

    public static string IRC_RAW_908(Server server, User user)
    {
        return $":{server} 908 {user} :No permissions to perform command";
    }

    public static string IRC_RAW_999(Server server, User? user, string message)
    {
        return $":{server} 999 {user} :Oops! We've hit a snag: {message}";
    }

    public static string RPL_JOIN(User? user, Channel channel)
    {
        return $":{user.Address} JOIN :{channel}";
    }

    public static string RPL_PART(User? user, Channel channel)
    {
        return $":{user.Address} PART {channel}";
    }

    public static string RPL_PRIVMSG(User? user, Channel channel, string? message)
    {
        return $":{user.Address} PRIVMSG {channel} :{message}";
    }

    public static string RPL_NOTICE(User? user, Channel channel, string? message)
    {
        return $":{user.Address} NOTICE {channel} :{message}";
    }

    public static string RPL_QUIT(User? user, string? message)
    {
        return $":{user.Address} QUIT :{message}";
    }
}

public class IrcxRaws
{ 
    public static string IRCX_RPL_PROPLIST_818(Server server, User? user, ChatObject chatObject,
        string? propName, string? propValue)
    {
        return $":{server} 818 {user} {chatObject} {propName} :{propValue}";
    }

    public static string IRCX_RPL_PROPEND_819(Server server, User? user, ChatObject chatObject)
    {
        return $":{server} 819 {user} {chatObject} :End of properties";
    }

    // Apollo
    public static string RPL_JOIN_MSN(Member? channelMember, Channel channel, Member joinMember)
    {
        var sourceUser = channelMember.GetUser();
        var joinUser = joinMember.GetUser();
        var listedMode = joinMember.GetListedMode();
        var listedModeString = !string.IsNullOrWhiteSpace(listedMode) ? $",{listedMode}" : "";
        return
            $":{joinUser.Address} JOIN {sourceUser.Protocol.GetFormat(joinUser)}{listedModeString} :{channel}";
    }

    public static string RPL_EPRIVMSG(User? user, Channel? channel, string? message)
    {
        return $":{user.Address} EPRIVMSG {channel} :{message}";
    }

    public static string RPL_EQUESTION(User? user, Channel? channel, string? nickname, string? message)
    {
        return $":{user.Address} EQUESTION {channel} {nickname} {channel} :{message}";
    }
}