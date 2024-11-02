using Irc.Enumerations;

namespace Irc.Commands;

public class Ping : Command
{
    public Ping() : base(1, false)
    {
    }

    public override EnumCommandDataType GetDataType()
    {
        return EnumCommandDataType.None;
    }

    public override void Execute(ChatFrame chatFrame)
    {
        chatFrame.User.Send($"PONG :{chatFrame.Message.Parameters.First()}");
    }
}