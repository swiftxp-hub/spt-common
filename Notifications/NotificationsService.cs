
using System;
using EFT.Communications;
using SwiftXP.SPT.ShowMeTheMoney;

namespace SwiftXP.SPT.Common.Notifications;

public class NotificationsService
{
    private static readonly Lazy<NotificationsService> instance = new(() => new NotificationsService());

    private NotificationsService() { }

    public void SendLongAlert(string message)
    {
        Send(message, ENotificationDurationType.Long, ENotificationIconType.Alert);
    }

    public void SendLongNotice(string message)
    {
        Send(message, ENotificationDurationType.Long, ENotificationIconType.Default);
    }

    public void Send(string message, ENotificationDurationType duration, ENotificationIconType icon)
    {
        try
        {
            GClass2314 updatedPricesMessage = new(
                message,
                duration,
                icon
            );

            NotificationManagerClass.DisplayNotification(updatedPricesMessage);
        }
        catch (Exception exception)
        {
            Plugin.SimpleSptLogger.LogException(exception);
        }
    }

    public static NotificationsService Instance => instance.Value;
}