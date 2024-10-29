using Irc.IO;

namespace Irc.Interfaces
{
    public interface IFloodProtectionProfile
    {
        void SetFloodProtectionLevel(FloodProtectionLevel floodProtectionLevel);
        FloodProtectionLevel GetFloodProtectionLevel();
    }
}