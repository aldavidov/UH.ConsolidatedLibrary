using System.ComponentModel;

namespace UH.UserProfileTools
{
    public interface IValidatableTrackingObject :
        IRevertibleChangeTracking,
        INotifyPropertyChanged
    {
        bool IsValid { get; }
    }
}
