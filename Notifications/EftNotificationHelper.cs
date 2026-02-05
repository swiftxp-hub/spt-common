
using System;
using EFT.Communications;
using SwiftXP.SPT.Common.Loggers;

namespace SwiftXP.SPT.Common.Notifications;

public class NotificationsService
{
    private static readonly Lazy<NotificationsService> s_instance = new(() => new NotificationsService());

    private NotificationsService() { }

    public static void SendAlert(string message)
    {
        Send(message, ENotificationDurationType.Default, ENotificationIconType.Alert);
    }

    public static void SendLongAlert(string message)
    {
        Send(message, ENotificationDurationType.Long, ENotificationIconType.Alert);
    }

    public static void SendNotice(string message)
    {
        Send(message, ENotificationDurationType.Default, ENotificationIconType.Default);
    }

    public static void SendLongNotice(string message)
    {
        Send(message, ENotificationDurationType.Long, ENotificationIconType.Default);
    }

    public static void Send(string message, ENotificationDurationType duration, ENotificationIconType icon)
    {
        GClass2551 updatedPricesMessage = new(
            message,
            duration,
            icon
        );

        NotificationManagerClass.DisplayNotification(updatedPricesMessage);
    }

    public static NotificationsService Instance => s_instance.Value;
}
