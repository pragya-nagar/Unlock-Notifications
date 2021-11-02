

namespace OKRNotification.Common
{
    public enum MessageTypes
    {
        /// <summary>
        /// The information
        /// </summary>
        Info,
        /// <summary>
        /// The success
        /// </summary>
        Success,
        /// <summary>
        /// The alert
        /// </summary>
        Alert,
        /// <summary>
        /// The warning
        /// </summary>
        Warning,
        /// <summary>
        /// The error
        /// </summary>
        Error,
    }

    public enum NotificationTypeId
    {
        ProvideFeedback =1,
        AskFeedback = 2,
        Comments = 3
    }
}
