namespace Irc.Interfaces;

public interface IChannelModes
{
    // IRC
    public bool InviteOnly { get; set; }
    public bool Key { get; set; }
    public bool Moderated { get; set; }
    public bool NoExtern { get; set; }
    public bool Private { get; set; }
    public bool Secret { get; set; }
    public bool Hidden { get; set; }
    public bool TopicOp { get; set; }
    public int UserLimit { get; set; }

    //IRCX
    bool AuthOnly { get; set; }
    bool Profanity { get; set; }
    bool Registered { get; set; }
    bool Knock { get; set; }
    bool NoWhisper { get; set; }
    bool NoGuestWhisper { get; set; }
    bool Cloneable { get; set; }
    bool Clone { get; set; }
    bool Service { get; set; }
}