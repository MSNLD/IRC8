namespace Irc.Resources;

public static class Tokens
{
    public static long Epoch = 621355968000000000;
    public static string? Connresetbypeer = "Connection reset by peer";
    public static string ServerError = "Server Error. Please report this to the Administrator";
    public static int MaxFieldLen = 64;

    // IRCX

    public static char ChannelModeAuthOnly = 'a'; // a - NTLM auth only
    public static char ChannelModeCloneable = 'd';
    public static char ChannelModeClone = 'e';
    public static char ChannelModeProfanity = 'f';
    public static char ChannelModeRegistered = 'r';
    public static char ChannelModeNoWhisper = 'w';
    public static char ChannelModeNoGuestWhisper = 'W';
    public static char ChannelModeService = 'z';
    public static char ChannelModeKnock = 'u';
    public static char ChannelModeAuditorium = 'x';

    public static char UserModeIrcx = 'x';
    public static char UserModeGag = 'z';

    // Apollo
    public static char ChannelModeSubscriber = 'S';
    public static char ChannelModeOnStage = 'g';
    public static char UserModeHost = 'h';

    #region User Properties
    public static string? UserPropOid = "OID";
    public static string? UserPropRole = "ROLE";
    public static string? UserPropSubscriberInfo = "SUBSCRIBERINFO";
    public static string? UserPropMsnProfile = "MSNPROFILE"; //
    public static string UserPropMsnRegCookie = "MSNREGCOOKIE";
    public static string UserPropNickname = "NICK";
    #endregion

    #region Channel Properties
    public static string ChannelPropName = "NAME";
    public static string? ChannelPropTopic = "TOPIC";
    public static string? ChannelPropLag = "LAG";
    public static string? ChannelPropLanguage = "LANGUAGE";
    public static string? ChannelPropSubject = "SUBJECT";
    public static string? ChannelPropMemberkey = "MEMBERKEY";
    public static string? ChannelPropOwnerkey = "OWNERKEY";
    public static string? ChannelPropHostkey = "HOSTKEY";
    public static string? ChannelPropCreation = "CREATION";
    public static string? ChannelPropOID = "OID";
    public static string? ChannelPropPICS = "PICS";
    public static string? ChannelPropOnJoin = "ONJOIN";
    public static string? ChannelPropOnPart = "ONPART";
    public static string? ChannelPropClientGuid = "CLIENTGUID";
    public static string ChannelPropOIDRegex = @"/0(?:[a-fA-F0-9]{8})?/";
    public static string ChannelPropPICSRegex = @"[\x20-\x7F]{1,255}";
    public static string ChannelPropTopicRegex = @"[\x20-\x7F]{1,160}";
    public static string ChannelPropOnjoinRegex = @"[\x20-\x7F]{1,255}";
    public static string ChannelPropOnpartRegex = @"[\x20-\x7F]{1,255}";
    public static string ChannelPropLagRegex = @"[0-2]{1}";
    #endregion


    #region Channel Resources

    public static char ChannelModePrivate = 'p';
    public static char ChannelModeSecret = 's';
    public static char ChannelModeHidden = 'h';
    public static char ChannelModeModerated = 'm';
    public static char ChannelModeNoExtern = 'n';
    public static char ChannelModeTopicOp = 't';
    public static char ChannelModeInvite = 'i';
    public static char ChannelModeUserLimit = 'l';
    public static char ChannelModeBan = 'b';
    public static char ChannelModeKey = 'k';

    public static char MemberModeOwner = 'q';
    public static char MemberModeHost = 'o';
    public static char MemberModeVoice = 'v';

    public static char MemberModeFlagOwner = '.';
    public static char MemberModeFlagHost = '@';
    public static char MemberModeFlagVoice = '+';

    #endregion

    #region "User Modes"

    public static char UserModeAdmin = 'a';
    public static char UserModeOper = 'o';
    public static char UserModeOwner = 'q';
    public static char UserModeInvisible = 'i';
    public static char UserModeServerNotice = 's';
    public static char UserModeSecure = 'S';
    public static char UserModeWallops = 'w';

    #endregion

    #region "Regular Expressions"

    public static string PreAuthNicknameMask = @"(^[>'][\x41-\xFF\-0-9]+$)|(^[\x41-\xFF][\x41-\xFF\-0-9]+$)";
    public static string PostAuthNicknameMask = @"^(?!(Sysop)|(Admin)|(Guide))[\x41-\xFF][\x41-\xFF\-0-9]+$";
    public static string PostAuthOperNicknameMask = @"^'?[\x41-\xFF][\x41-\xFF\-0-9]*$";
    public static string PostAuthGuestNicknameMask = @"^>[\x41-\xFF][\x41-\xFF\-0-9]*$";

    public static string GenericProps = @"[\x20-\x7F]{0,31}";
    public static string IrcxChannelRegex = @"%#[\x21-\x2B,\x2D-\xFF]{1,200}";

    #endregion

    #region Channel Properties

    public static string ChannelPropAccount = "ACCOUNT";
    public static string? ChannelPropServicePath = "SERVICEPATH";
    public static string? ChannelPropClient = "CLIENT";

    #endregion
}