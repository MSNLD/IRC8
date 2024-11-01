using Irc.Interfaces;
using Irc.Objects;
using Irc.Objects.Channel;
using Irc.Objects.Server;
using Irc.Objects.User;
using Irc.Resources;

namespace Irc.Modes;

public class ModeEngine
{
    private readonly IModeRules _modeRules;
    private readonly Dictionary<char, ModeRule> modeRules = new();

    public ModeEngine(IModeRules modeRules)
    {
        this._modeRules = modeRules;
    }

    public void AddModeRule(char modeChar, ModeRule modeRule)
    {
        modeRules[modeChar] = modeRule;
    }

    public static void Breakdown(IUser? source, ChatObject target, string? modeString,
        Queue<string> modeParameters)
    {
        var modeOperations = source.GetModeOperations();
        var modeFlag = true;

        foreach (var c in modeString)
            switch (c)
            {
                case '+':
                case '-':
                {
                    modeFlag = c == '+';
                    break;
                }
                default:
                {
                    var modeRules = ModeRules.GetRules(target);
                    var modeRule = modeRules.ContainsKey(c) ? modeRules[c] : null;
                    
                    if (modeRule == null)
                    {
                        // Unknown mode char
                        // :sky-8a15b323126 472 Sky S :is unknown mode char to me
                        source.Send(Raw.IRCX_ERR_UNKNOWNMODE_472(source.Server, source, c));
                        continue;
                    }
                    
                    var modeCollection = target.Modes;
                    var exists = modeCollection.ContainsKey(c);
                    var modeValue = exists ? modeCollection[c] : -1;

                    string parameter = null;
                    // TODO: Here need to get all mode rules depending on object type
                    if (modeRule.RequiresParameter)
                    {
                        if (modeParameters != null && modeParameters.Count > 0)
                        {
                            parameter = modeParameters.Dequeue();
                        }
                        else
                        {
                            // Not enough parameters
                            //:sky-8a15b323126 461 Sky MODE +q :Not enough parameters
                            source.Send(Raw.IRCX_ERR_NEEDMOREPARAMS_461(source.Server, source,
                                $"{IrcStrings.CommandMode} {c}"));
                            continue;
                        }
                    }


                    modeOperations.Enqueue(
                        new ModeOperation
                        {
                            Mode = modeRule,
                            Source = source,
                            Target = target,
                            ModeFlag = modeFlag,
                            ModeParameter = parameter
                        }
                    );

                    break;
                }
            }
    }
}