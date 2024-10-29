using Irc.Commands;
using Irc.Enumerations;

namespace Irc.Protocols
{
    public class IrcX : Irc
    {
        public IrcX()
        {
            AddCommand(new Commands.Access());
            AddCommand(new Away());
            AddCommand(new Create());
            AddCommand(new Data());
            AddCommand(new Event());
            AddCommand(new Isircx());
            AddCommand(new Kill());
            AddCommand(new Listx());
            AddCommand(new Reply());
            AddCommand(new Request());
            AddCommand(new Whisper());
            AddCommand(new Auth());
            AddCommand(new Ircx());
            AddCommand(new Prop());
            AddCommand(new Listx());
        }

        public override EnumProtocolType GetProtocolType()
        {
            return EnumProtocolType.IRCX;
        }
    }
}