using Irc.Enumerations;
using Irc.Objects;

namespace Irc.IO;

public class FloodProtectionManager
{
    private const long UnixEpoch = 0x089f7ff5f7b58000; //01/01/1970
    // TODO: Find a way to make the below delay work correctly again
    public int Delay = 0;
    public long LastProcessedTicks;

    public FloodProtectionManager()
    {
        LastProcessedTicks = UnixEpoch;
    }
    
    private bool CanProcess()
    {
        if (Delay > 0)
        {
            if ((DateTime.UtcNow.Ticks - LastProcessedTicks) / TimeSpan.TicksPerSecond >= Delay)
            {
                LastProcessedTicks = DateTime.UtcNow.Ticks;
                return true;
            }

            return false;
        }

        return true;
    }
    
    public EnumFloodResult Audit(EnumUserAccessLevel level)
    {
        if (level >= EnumUserAccessLevel.Guide) return EnumFloodResult.Ok;
        
        return CanProcess() ? EnumFloodResult.Ok : EnumFloodResult.Wait;
    }
}