using Irc.Interfaces;

namespace Irc.Objects.Collections;

public class ModeCollection
{
    // TODO: <CHANKEY> Below is temporary until implemented properly
    protected string keypass = null;
    public Dictionary<char, IModeRule> Modes = new();

    public int GetModeChar(char mode)
    {
        Modes.TryGetValue(mode, out var value);
        return value.Get();
    }

    public string GetModeString()
    {
        return $"{new string(Modes.Where(mode => mode.Value.Get() > 0).Select(mode => mode.Key).ToArray())}";
    }

    public string GetSupportedModes()
    {
        return new string(Modes.Keys.OrderBy(x => x).ToArray());
    }

    public override string ToString()
    {
        return $"+{new string(Modes.Where(mode => mode.Value.Get() > 0).Select(mode => mode.Key).ToArray())}";
    }
}