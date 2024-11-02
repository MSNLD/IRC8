using Irc.Enumerations;

namespace Irc.Commands;

public class Pong : Command
{
    public Pong() : base(0, false)
    {
    }

    public override EnumCommandDataType GetDataType()
    {
        return EnumCommandDataType.None;
    }

    public override void Execute(ChatFrame chatFrame)
    {
    }
}