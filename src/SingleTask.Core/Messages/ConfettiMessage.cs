using CommunityToolkit.Mvvm.Messaging.Messages;

namespace SingleTask.Core.Messages;

public class ConfettiMessage : ValueChangedMessage<bool>
{
    public ConfettiMessage(bool value) : base(value)
    {
    }
}
