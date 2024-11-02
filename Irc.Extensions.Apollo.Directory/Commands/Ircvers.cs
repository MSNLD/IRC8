using Irc.Commands;
using Irc.Enumerations;

namespace Irc.Extensions.Apollo.Directory.Commands;

internal class Ircvers : Command
{
    public Ircvers() : base(2, false)
    {
    }

    public override EnumCommandDataType GetDataType()
    {
        return EnumCommandDataType.Standard;
    }

    public override void Execute(ChatFrame chatFrame)
    {
    }
}