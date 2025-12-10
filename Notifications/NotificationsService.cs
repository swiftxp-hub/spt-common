
using System;
using EFT.Communications;
using SwiftXP.SPT.Common.Loggers;

namespace SwiftXP.SPT.Common.Notifications;

public class NotificationsService
{
    private static readonly Lazy<NotificationsService> instance = new(() => new NotificationsService());

    private NotificationsService() { }

    public void SendAlert(string message)
    {
        Send(message, ENotificationDurationType.Default, ENotificationIconType.Alert);
    }

    public void SendLongAlert(string message)
    {
        Send(message, ENotificationDurationType.Long, ENotificationIconType.Alert);
    }

    public void SendNotice(string message)
    {
        Send(message, ENotificationDurationType.Default, ENotificationIconType.Default);
    }

    public void SendLongNotice(string message)
    {
        Send(message, ENotificationDurationType.Long, ENotificationIconType.Default);
    }

    public void Send(string message, ENotificationDurationType duration, ENotificationIconType icon)
    {
        GClass2314 updatedPricesMessage = new(
            message,
            duration,
            icon
        );

        NotificationManagerClass.DisplayNotification(updatedPricesMessage);
    }

    public static NotificationsService Instance => instance.Value;
}