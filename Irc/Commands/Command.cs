using Irc.Resources;

namespace Irc.Commands;

public abstract class Command
{
    private readonly bool _registrationRequired;
    protected readonly int RequiredMaximumParameters;
    protected int RequiredMinimumParameters;

    public Command(int requiredMinimumParameters = 0, bool registrationRequired = true,
        int requiredMaximumParameters = -1)
    {
        RequiredMinimumParameters = requiredMinimumParameters;
        _registrationRequired = registrationRequired;
        RequiredMaximumParameters = requiredMaximumParameters;
    }

    public string? GetName()
    {
        return GetType().Name;
    }

    public abstract void Execute(ChatFrame chatFrame);

    public bool ParametersAreValid(ChatFrame chatFrame)
    {
        var parameterCount = chatFrame.Message.Parameters.Count;

        if (parameterCount < RequiredMinimumParameters)
        {
            chatFrame.User.Send(Raws.IRCX_ERR_NEEDMOREPARAMS_461(chatFrame.Server, chatFrame.User, GetName()));
            return false;
        }

        if (RequiredMaximumParameters > 0 && parameterCount > RequiredMaximumParameters)
        {
            chatFrame.User.Send(Raws.IRCX_ERR_TOOMANYARGUMENTS_901(chatFrame.Server, chatFrame.User, GetName()));
            return false;
        }

        return true;
    }

    public bool RegistrationNeeded(ChatFrame chatFrame)
    {
        if (!_registrationRequired || (_registrationRequired && chatFrame.User.IsRegistered())) return false;

        chatFrame.User.Send(Raws.IRCX_ERR_NOTREGISTERED_451(chatFrame.Server, chatFrame.User));
        return true;
    }
}